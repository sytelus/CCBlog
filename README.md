CCBlog
======

Tenent Specific TODOs:
----------------------
1. Replace apple-touch*.jpg and favicon.ico.
2. Change crossdomain.xml to allow other domains for Adobe Reader, Flash etc.
3. Fill out info in humans.txt.
4. Add any pages you want to block from bots in robots.txt
5. Fill <meta name="description" content=""> and <meta name="author" content="">
6. Update Google Analytics a/c info


TODOs:
------
Custom 404s --> use from boilerplate?
Merge initializr web.config
Add browser choice instead of just Chrome in Boilerplate
Use Squishit to compress js/css and cache it


How to Build and Run
--------------------
If you get following errors on build: 
	CA0058	Error Running Code Analysis	CA0058 : The referenced assembly 'System.Web.Mvc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' could not be found. This assembly is required for analysis and was referenced by: C:\GitHubSrc\CCBlog\CCBlog\bin\CCBlog.dll, C:\GitHubSrc\CCBlog\packages\DotNetOpenAuth.Core.4.1.0.12182\lib\net45-full\DotNetOpenAuth.Core.dll.	[Errors and Warnings]	(Global)	
	
	This is because DotNetOpenAuth is still referencing older version of MVC DLL while the project has another (latest). FxCop gets tripped over this due to *bug* documented here: http://davesbox.com/archive/2008/06/14/reference-resolutions-changes-in-code-analysis-and-fxcop-part-2.aspx
	
	To resolve this error:
	Using a text editor, open the FxCopCmd.exe.config file located in  %PROGRAMFILES(x86)%\Microsoft Visual Studio 11.0\Team Tools\Static Analysis Tools\FxCop. Change the AssemblyReferenceResolveMode from StrongName to StrongNameIgnoringVersion. Save the text file (note that you may have to start Notepad as Administrator to be able to save).

Another error you may get is because of schema warnings in web.config, again due to DotNetOpenAuth section. To resolve this, simply close the web.config file in editor before build.