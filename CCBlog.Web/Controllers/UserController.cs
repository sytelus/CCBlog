﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;
using CCBlog.Model.Contracts;
using CCBlog.Model.Poco;
using CCBlog.Repository;
using DotNetOpenAuth.OpenId.RelyingParty;
using System.Web.Security;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.Messaging;
using CommonUtils;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using System.Web.Configuration;


namespace CCBlog.Controllers
{
    public partial class UserController : AppControllerBase
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View("Login", new LoginModel());
        }

        [HttpGet]
        public ActionResult WhoAmI()
        {
            return Content(User.Identity.Name.AsNullIfEmpty() ?? "Not logged in");
        }


        [HttpGet]
        public ActionResult Login(string returnUrl)   
        {
            return View(new LoginModel() { ReturnUrl = GetSafeReturnUrl(returnUrl) });
        }

        [HttpGet]
        public ActionResult Logout(string returnUrl)
        {
            FormsAuthentication.SignOut();
            
            return Redirect(GetSafeReturnUrl(returnUrl));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AuthenticateOpenId(LoginModel model)
        {
            if (model.openid_identifier == null || !Identifier.IsValid(model.openid_identifier))
            {
                model.ErrorMessage = "The specified login identifier \"{0}\" is invalid".FormatEx(model.openid_identifier);

            }
            else
            {
                try
                {
                    var openid = new OpenIdRelyingParty();
                    var returnToUrl = new Uri(Url.Action("OpenIdCallback", "User", new { ReturnUrl = model.ReturnUrl, openid_identifier = model.openid_identifier }, Request.Url.Scheme), UriKind.Absolute);
                    IAuthenticationRequest request = openid.CreateRequest(Identifier.Parse(model.openid_identifier), Realm.AutoDetect, returnToUrl);

                    // Require some additional data
                    request.AddExtension(new ClaimsRequest
                    {
                        Email = DemandLevel.Require,
                        FullName = DemandLevel.Require,
                        Nickname = DemandLevel.Require,
                        Country = DemandLevel.Require,
                        Gender = DemandLevel.Require,
                        PostalCode = DemandLevel.Require,
                        TimeZone = DemandLevel.Require
                    });

                    return request.RedirectingResponse.AsActionResult();
                }
                catch (ProtocolException ex)
                {
                    model.ErrorMessage = "Unexpected error occured while creating OpenID request: \"{0}\"".FormatEx(ex.Message);
                }
            }

            return View("Login", model);
        }

        [HttpPost]  //Yahoo does POST
        [ActionName("OpenIdCallback")]
        [ValidateInput(false)]
        public ActionResult OpenIdCallbackPost(string returnUrl, string openid_identifier)
        {
            return OpenIdCallback(returnUrl, openid_identifier);
        }

        [HttpGet]   //Google does GET
        [ValidateInput(false)]
        public ActionResult OpenIdCallback(string returnUrl, string openid_identifier)
        {
            var model = new LoginModel { ReturnUrl = returnUrl, openid_identifier = openid_identifier };
            var openId = new OpenIdRelyingParty();
            var response = openId.GetResponse();

            if (response != null)
            {
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        var loggedUser = GetLoggedUser(response);
                        if (loggedUser == null || loggedUser.UserId == 0)
                        {
                            model.ErrorMessage = "Signin was successful but this user was not found in system";
                            break;
                        }

                        return SetAuthCookieAndRedirect(loggedUser, model.ReturnUrl);
                    case AuthenticationStatus.Canceled:
                        model.ErrorMessage = "Login was cancelled at the provider";
                        break;
                    case AuthenticationStatus.Failed:
                        model.ErrorMessage = "Login failed using the provided OpenID identifier \"{0}\" with error \"{1}\""
                                .FormatEx(response.FriendlyIdentifierForDisplay ?? string.Empty, response.Exception.IfNotNull(e => e.ToStringDescriptive(), string.Empty));
                        break;
                    default:
                        model.ErrorMessage = "Login failed due to unknwon status \"{0}\" using the provided OpenID identifier \"{1}\"  with error \"{2}\""
                                .FormatEx(response.Status.ToString(), response.FriendlyIdentifierForDisplay ?? string.Empty, response.Exception.IfNotNull(e => e.ToStringDescriptive(), string.Empty));
                        break;
                }
            }
            else
            {
                model.ErrorMessage = "OpenID Callback did not had Response!";
            }

            return View("Login", model);
        }

        private IUser GetLoggedUser(IAuthenticationResponse authResponse)
        {
            var sregResponse = authResponse.GetExtension<ClaimsResponse>();
            var fullName = sregResponse.IfNotNull(r => r.FullName.AsNullIfEmpty());
            var nickName = sregResponse.IfNotNull(r => r.Nickname.AsNullIfEmpty());
            var email = sregResponse.IfNotNull(r => r.Email.AsNullIfEmpty());

            var userToLogin = new User()
            {
                ClaimedIdentifier = authResponse.ClaimedIdentifier,
                Email = email,
                FullName = fullName,
                Nickname = nickName
            };

            var loggedUser = LoginUser(userToLogin, false); //setting to true could result in lots of users creating their logins unnecessorily

            return loggedUser;
        }

        private ActionResult SetAuthCookieAndRedirect(IUser loggedUser, string returnUrl)
        {
            FormsAuthentication.SetAuthCookie(loggedUser.UserId.ToStringInvariant(), false);

            var friendlyName = loggedUser.GetfriendlyName();
            var friendlyNameCookie = new HttpCookie("FriendlyName", friendlyName) { HttpOnly = true };
            Response.Cookies.Add(friendlyNameCookie);

            return Redirect(returnUrl);
        }


        private string GetSafeReturnUrl(string returnUrl)
        {
            return returnUrl ?? Request.UrlReferrer.IfNotNull(u => u.AbsoluteUri) ?? GetDefaultReturnUrl();
        }

        private string GetDefaultReturnUrl()
        {
            return Url.Action("Index", "Home", Request.Url.Scheme);
        }

        private IUser LoginUser(IUser user, bool createUserIfNotExists)
        {
            if (string.IsNullOrEmpty(user.ClaimedIdentifier))
                throw new ArgumentNullException("user.ClaimedIdentifier", "user.ClaimedIdentifier must be valid");

            //Find this user
            var foundUser = this.Repository.GetUser(user.ClaimedIdentifier);
            if (foundUser != null)
            {
                var updated = false;

                //Check if we have latest info
                if (!string.IsNullOrEmpty(user.FullName))
                {
                    foundUser.FullName = user.FullName;
                    updated = true;
                }

                if (!string.IsNullOrEmpty(user.Email))
                {
                    foundUser.Email = user.Email;
                    updated = true;
                }

                if (updated)
                    this.Repository.UpdateUser(foundUser);

                return foundUser;
            }
            else
            {
                if (createUserIfNotExists)
                {
                    if (user.UserId != 0)
                        throw new ArgumentOutOfRangeException("user.UserId must be 0 if new user needs to be created");

                    this.Repository.AddUser(user);

                    return user;
                }
                else return null;
            }
        }

    }
}
