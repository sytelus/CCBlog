using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CommonUtils
{
    public static class Utils
    {
        private static readonly double LogE2 = Math.Log(2);

        public static bool IsNullOrWhiteSpaceDotNet35(string input)
        {
            if (input == null)
                return true;
            else if (String.IsNullOrEmpty(input))
                return true;
            else if (input.Trim().Length == 0)
                return true;
            else
                return false;
        }

        public static void AppendLines(this StringBuilder sb, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                sb.AppendLine(value);
            }
        }

        public static double Log2(double x)
        {
            return Math.Log(x) / LogE2;
        }

        private readonly static Regex nonAlphaNumericRegex = new Regex(@"[\W]", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase); //remove all non-alpha chars
        public static string RemoveNonAlphaNumericChars(string s)
        {
            return nonAlphaNumericRegex.Replace(s, String.Empty);
        }

        public static IEnumerable<string> RemoveNullOrEmpty(this IEnumerable<string> source)
        {
            return source.Where(s => !string.IsNullOrEmpty(s));
        }

        public static string ToStringInvariant(this int k)
        {
            return k.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this int? k)
        {
            if (k == null)
                return Utils.IntMinValueAsString;
            else
                return k.Value.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this double k)
        {
            return k.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this float k)
        {
            return k.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this float? k)
        {
            if (k == null)
                return Utils.IntMinValueAsString;
            else
                return k.Value.ToString(CultureInfo.InvariantCulture);
        }
        public static void InsertAndExpand<T>(this List<T> list, int index, T item)
        {
            if (index < 0)
                throw new IndexOutOfRangeException("List index must be >= 0");
            else if (index < list.Count)
                list[index] = item;
            else if (index == list.Count)
                list.Add(item);
            else
            {
                list.AddRange(Enumerable.Repeat(default(T), index - list.Count + 1));
                list[index] = item;
            }
        }
        public static string ToStringInvariant(this double? k, bool allowNonNumericalValues)
        {
            if (k == null)
                return Utils.IntMinValueAsString;
            else if (!allowNonNumericalValues && (double.IsNaN(k.Value) || double.IsInfinity(k.Value)))
                throw new Exception("Double value is not expected to be NaN or Infinity");
            else
                return k.Value.ToString(CultureInfo.InvariantCulture);
        }

        private enum TermType
        {
            UpperCaseLetters,
            LowercaseLetters,
            NumberOrOther,
            None
        };

        public static bool IsOneSubStringOfOther(string s1, string s2)
        {
            if (s1.IsNullOrEmpty() && s2.IsNullOrEmpty())
                return true;
            else if (s1.IsNullOrEmpty() || s2.IsNullOrEmpty())
                return false;
            else
                return s1.StartsWith(s2) || s2.StartsWith(s1);
        }

        public static Dictionary<TKey, int> ToValueIndexDictionary<TKey>(this IEnumerable<TKey> keys)
        {
            var dictionary = new Dictionary<TKey, int>();

            var index = 0;
            foreach (var key in keys)
            {
                dictionary.Add(key, index);
                index++;
            }

            return dictionary;
        }

        private readonly static Dictionary<Type, DataContractJsonSerializer> dictionaryDataContractJsonSerializers = 
            new Dictionary<Type, DataContractJsonSerializer>()
                {
                    {typeof(IEnumerable<KeyValuePair<string, string>>), new DataContractJsonSerializer(typeof(IEnumerable<KeyValuePair<string, string>>))},
                    {typeof(IEnumerable<KeyValuePair<string, double>>), new DataContractJsonSerializer(typeof(IEnumerable<KeyValuePair<string, double>>))}
                };
        public static string SerializeToJson<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
        {
            if (dictionary != null)
                return SerializeToJson(
                    dictionaryDataContractJsonSerializers[typeof(IEnumerable<KeyValuePair<TKey, TValue>>)]
                    , dictionary);
            else
                return null;
        }
        public static IEnumerable<KeyValuePair<TKey, TValue>> DeserializeDictionaryFromJson<TKey, TValue>(string jsonSerializedDisctionary)
        {
            if (string.IsNullOrEmpty(jsonSerializedDisctionary))
                return null;

            return DeserializeFromJson<IEnumerable<KeyValuePair<TKey, TValue>>>(
                dictionaryDataContractJsonSerializers[typeof(IEnumerable<KeyValuePair<TKey, TValue>>)]
                , jsonSerializedDisctionary);
        }

        public static string ToStringNullSafe(this object o)
        {
            if (o == null)
                return null;
            else
                return o.ToString();
        }


        /// <summary>
        /// This method would break down string on boundry of letter or digits or multiple of upper/small case letters
        /// Examples:
        ///     Quest12 => {Quest, 12}
        ///     QUESTPolo23, Inc! => {QUEST, Polo, 23, Inc}
        ///     QuestPolo23 @ Corp => {Quest, Polo, 23, Corp}
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IEnumerable<string> WordBreak(this string content)
        {
            var term = new StringBuilder();
            var termType = TermType.None;
            foreach (var c in content)
            {
                var flushTermLength = -1;

                //Decide if we should return the accumulated term
                if (termType == TermType.UpperCaseLetters)
                {
                    if (char.IsLower(c))
                    {
                        if (term.Length > 1)
                            flushTermLength = term.Length - 1;
                    }
                    else if (!char.IsUpper(c))
                        flushTermLength = term.Length;
                }
                else if (termType == TermType.LowercaseLetters)
                {
                    if (char.IsUpper(c))
                    {
                        if (term.Length > 1)
                            flushTermLength = term.Length - 1;
                    }
                    else if (!char.IsLower(c))
                        flushTermLength = term.Length;
                }
                else if (termType == TermType.NumberOrOther)
                {
                    if (!char.IsDigit(c))
                        flushTermLength = term.Length;
                }


                if (flushTermLength > -1)
                {
                    var termToReturn = term.ToString();
                    var termToAdd = string.Empty;
                    if (flushTermLength < termToReturn.Length)
                    {
                        termToAdd = termToReturn.Substring(flushTermLength);
                        termToReturn = termToReturn.Substring(0, flushTermLength);
                    }

                    if (termToReturn.Length > 0)
                    {
                        yield return termToReturn;
                        term.Length = 0;
                    }

                    if (termToAdd.Length > 0)
                        term.Append(termToAdd);
                }

                if (char.IsLetterOrDigit(c))
                {
                    term.Append(c);

                    if (char.IsUpper(c))
                        termType = TermType.UpperCaseLetters;
                    else if (char.IsLower(c))
                        termType = TermType.LowercaseLetters;
                    else
                        termType = TermType.NumberOrOther;
                }
                else
                    termType = TermType.None;
            }
        }

        static readonly Regex wordBreaker = new Regex(@"[a-zA-Z]+|[0-9]+", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase); //remove all non-alpha chars, split on alpha num boundaries

        /// <summary>
        /// Returns terms without single letter buffering
        /// </summary>
        public static string[] GetTerms(string content)
        {
            MatchCollection termMatches = GetTermMatches(content);
            var terms = new string[termMatches.Count];

            for (int termIndex = 0; termIndex < termMatches.Count; termIndex++)
            {
                terms[termIndex] = termMatches[termIndex].Value;
            }

            return terms;
        }

        public static string[] Split(string[] delimiters, string s, bool removeEmptyEntries)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return s.Split(delimiters, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            }
            else
                return EmptyStringArray;
        }

        public static bool IsNumericalValue(this double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        public static IEnumerable<TElement> TryGetValues<TKey, TElement>(this ILookup<TKey, TElement> lookup, TKey key)
        {
            if (lookup == null || !lookup.Contains(key))
                return Enumerable.Empty<TElement>();

            return lookup[key];
        }

        public static double GetJaccardSimilarity<TKey, TValue>(IDictionary<TKey, TValue> value1Terms, IDictionary<TKey, TValue> value2Terms)
        {
            IDictionary<TKey, TValue> smallerValueTerms, largerValueTerms;
            if (value1Terms.Count < value2Terms.Count)
            {
                smallerValueTerms = value1Terms;
                largerValueTerms = value2Terms;
            }
            else
            {
                smallerValueTerms = value2Terms;
                largerValueTerms = value1Terms;
            }

            var intersectCount = smallerValueTerms.Keys.Count(smallerValueTerm => largerValueTerms.ContainsKey(smallerValueTerm));

            return intersectCount / (double)(value1Terms.Count + value2Terms.Count - intersectCount);
        }

        public static double GetJaccardSimilarity(HashSet<string> value1Terms, HashSet<string> value2Terms)
        {
            HashSet<string> smallerValueTerms, largerValueTerms;
            if (value1Terms.Count < value2Terms.Count)
            {
                smallerValueTerms = value1Terms;
                largerValueTerms = value2Terms;
            }
            else
            {
                smallerValueTerms = value2Terms;
                largerValueTerms = value1Terms;
            }

            var intersectCount = smallerValueTerms.Count(smallerValueTerm => largerValueTerms.Contains(smallerValueTerm));

            return intersectCount / (double) (value1Terms.Count + value2Terms.Count - intersectCount);
        }

        public static double GetDiceSimilarity(HashSet<string> value1Terms, HashSet<string> value2Terms)
        {
            HashSet<string> smallerValueTerms, largerValueTerms;
            if (value1Terms.Count < value2Terms.Count)
            {
                smallerValueTerms = value1Terms;
                largerValueTerms = value2Terms;
            }
            else
            {
                smallerValueTerms = value2Terms;
                largerValueTerms = value1Terms;
            }

            var intersectCount = smallerValueTerms.Count(smallerValueTerm => largerValueTerms.Contains(smallerValueTerm));

            return (intersectCount * 2.0) / (value1Terms.Count + value2Terms.Count);
        }

        public static double GetDiceSimilarity(string[] value1Terms, string[] value2Terms, double defaultForNullOrEmpty)
        {
            if (value1Terms.Length > 0 && value2Terms.Length > 0)
            {
                var value1TermsSet = value1Terms.ToHashSet();

                var totalTermCount = value1TermsSet.Count + value2Terms.Length;
                value1TermsSet.IntersectWith(value2Terms);
                var diceSimilarity = (2.0 * value1TermsSet.Count) / totalTermCount;

                return diceSimilarity;
            }
            else return defaultForNullOrEmpty;
        }

        public static IEnumerable<T> SelectRange<T>(this IList<T> source, int startIndex)
        {
            for (var sourceIndex = startIndex; sourceIndex < source.Count; sourceIndex++)
                yield return source[sourceIndex];
        }

        public static IEnumerable<T> SelectReverse<T>(this IList<T> source)
        {
            for(var sourceIndex = source.Count - 1; sourceIndex >= 0; sourceIndex--)
                yield return source[sourceIndex];
        }

        public static IEnumerable<KeyValuePair<T,T>> SelectPairs<T>(this IList<T> source)
        {
            for (var sourceIndex1 = 0; sourceIndex1 < source.Count; sourceIndex1++)
                for (var sourceIndex2 = sourceIndex1 + 1; sourceIndex2 < source.Count; sourceIndex2++)
                    yield return new KeyValuePair<T, T>(source[sourceIndex1], source[sourceIndex2]);
        }

        public static TResult IfNotNull<TInput, TResult>(this TInput input, Func<TInput, TResult> resultSelector, TResult resultIfNull) where TInput : class
        {
            if (input == null)
                return resultIfNull;
            else
                return resultSelector(input);
        }

        public static TResult IfNotNull<TInput, TResult>(this TInput input, Func<TInput, TResult> resultSelector) where TInput : class
        {
            return IfNotNull(input, resultSelector, default(TResult));
        }

        public static TV MostOccuring<T,TV>(this IEnumerable<T> sequenceElements, Func<T,TV> selector)
        {
            var frequencies =
                sequenceElements
                .GroupBy(sequenceElement => selector(sequenceElement))
                .Select(g => new { g.Key, Count = g.Count() }).ToArray();

            if (frequencies.Length == 0)
                return default(TV);

            var highestFrequency = frequencies.Max(frequency => frequency.Count);

            var valuesWithHighestFrequencies = frequencies.Where(frequency => frequency.Count == highestFrequency);

            return valuesWithHighestFrequencies.Select(frequency => frequency.Key).FirstOrDefault();
        }

        public static T MostOccuring<T>(this IEnumerable<T> sequenceElements)
        {
            return sequenceElements.MostOccuring(sequenceElement => sequenceElement);
        }

        /// <summary>
        /// Return terms with single letter buffering
        /// </summary>
        public static IEnumerable<string> GetTerms(string content, HashSet<string> filterSet)
        {
            return GetTermsWithBufferedSingleLetters(content, filterSet).Select(termKvp => termKvp.Key);
        }

        /// <summary>
        /// Return terms with single letter buffering
        /// </summary>
        public static Dictionary<string, int> GetTermsAsDictionary(string content, int positionMultiplier, HashSet<string> filterSet)
        {
            var terms = new Dictionary<string, int>();
            foreach (var termKvp in GetTermsWithBufferedSingleLetters(content, filterSet))
                terms[termKvp.Key] = termKvp.Value*positionMultiplier;

            return terms;
        }

        public static string CompactSerialize(double value)
        {
            return ByteArrayToBase64String(BitConverter.GetBytes(value));
        }
        public static Double CompactDeserializeDouble(string value)
        {
            return BitConverter.ToDouble(Base64StringToByteArray(value), 0);
        }

        /// <summary>
        /// This is enhanced version of GetTermMatches with the difference that multiple single letter terms are combined to 1.
        /// "abc xyz123~q!1." -> {"abc", "xyz", "123", "q", "1"}
        /// "abc xyz123~p q r ps!1." -> {"abc", "xyz", "123", "pqr", "ps", "1"}
        /// </summary>
        /// <param name="content">string to break</param>
        /// <param name="filterSet">These terms would not be processed (typically noise words)</param>
        private static IEnumerable<KeyValuePair<string,int>> GetTermsWithBufferedSingleLetters(string content, HashSet<string> filterSet)
        {
            //Break on any non-alpha chars
            var termMatches = GetTermMatches(content);
            var bufferedTerm = String.Empty;
            var bufferedTermIndex = -1;
            for (var termIndex = 0; termIndex < termMatches.Count; termIndex++)
            {
                var term = termMatches[termIndex].Value;

                if (term.Length == 1)
                {
                    bufferedTerm = String.Concat(bufferedTerm, term);
                    if (bufferedTermIndex == -1) bufferedTermIndex = termIndex;
                    continue;
                }
                else
                {
                    if (bufferedTerm.Length > 0)
                    {
                        if (filterSet==null || !filterSet.Contains(bufferedTerm))
                            yield return new KeyValuePair<string, int>(bufferedTerm, bufferedTermIndex);

                        bufferedTerm = String.Empty;
                        bufferedTermIndex = -1;
                    }

                    if (filterSet == null || !filterSet.Contains(term))
                        yield return new KeyValuePair<string, int>(term, termIndex);
                }
            }

            if (bufferedTerm.Length > 0)
            {
                if (filterSet == null || !filterSet.Contains(bufferedTerm))
                    yield return new KeyValuePair<string, int>(bufferedTerm, bufferedTermIndex);
            }
        }

        /// <summary>
        /// Cleans accented chars and breaks the string on any non-alpha chars. "abc xyz123~q!1." -> {"abc", "xyz", "123", "q", "1"}
        /// </summary>
        private static MatchCollection GetTermMatches(string content)
        {
            content = content ?? String.Empty;
            var asciiLowerCasedContent = ConvertCharsToASCII(content.Trim().ToLowerInvariant());

            var termMatches = wordBreaker.Matches(asciiLowerCasedContent);

            return termMatches;
        }

        public static readonly char[] TabDelimiter = new char[] { '\t' };
        public static readonly char[] CommaDelimiter = new char[] { ',' };
        public static readonly char[] SpaceDelimiter = new char[] { ' ' };
        public static readonly char[] TildaDelimiter = new char[] { '~' };
        public static readonly char[] DotDelimiter = new char[] { '.' };
        public static readonly char[] SemiColonDelimiter = new char[] { ';' };
        public static readonly char[] ColonDelimiter = new char[] { ':' };
        public static readonly string CommaDelimiterString = ",";
        public static readonly string SemiColonDelimiterString = ";";

        public static uint IncrementCountInDictionary<TKey>(this IDictionary<TKey, uint> dictionary, TKey key) 
        {
            uint existingCount = 0;
            if (dictionary.TryGetValue(key, out existingCount))
                dictionary[key] = existingCount + 1;
            else
                dictionary.Add(key, 1);

            return existingCount;
        }

        public static void IncrementCountInDictionary<TKey>(this IDictionary<TKey, float> dictionary, TKey key)
        {
            float existingCount;
            if (dictionary.TryGetValue(key, out existingCount))
                dictionary[key] = existingCount + 1;
            else
                dictionary.Add(key, 1);
        }

        public static Dictionary<string, string> Get2And3Grams(string[] tokens)
        {
            if (tokens != null && tokens.Length > 0)
            {
                var ngrams = new Dictionary<string, string>();

                string lastToken = tokens[0];
                string lastBiagram = null;
                for (var currentIndex = 1; currentIndex < tokens.Length; currentIndex++)
                {
                    var token = tokens[currentIndex];
                    var bigram = String.Concat(lastToken, token);
                    var fullForm = String.Concat(lastToken, " ", token);
                    ngrams[bigram] = fullForm;

                    if (lastBiagram != null)
                    {
                        var trigram = String.Concat(lastBiagram, token);
                        ngrams[trigram] = String.Concat(fullForm, " ", token);
                    }

                    lastToken = token;
                    lastBiagram = bigram;
                }

                return ngrams;
            }
            else return null;
        }

        public static string GetMD5HashString(string s)
        {
            return ByteArrayToBase64String(GetMD5Hash(s));
        }

        public static byte[] GetMD5Hash(string s)
        {
            // Note: the previous implementation caches md5Hasher as a static class variable,
            // which causes LDSEntity_Serialization_Test to fail and throw NullReferenceException,
            // when all tests are run.
            using (var md5Hasher = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(s));
                return data;
            }
        }

        public static IEnumerable<T> AsEnumerable<T>(T value)
        {
            return Enumerable.Repeat(value, 1);
        }
        public static IEnumerable<T> AsEnumerable<T>(params T[] values)
        {
            if (values != null)
                foreach (var value in values)
                    yield return value;
        }
        public static IEnumerable<T> AsEnumerable<T>(bool ignoreNull, params T[] values) where T : class
        {
            return AsEnumerable(values).Where(value => !ignoreNull || (value != null));
        }

        public static bool Overlaps<T>(this IList<T> list1, IList<T> list2) where T:IEquatable<T>
        {
            if (list1.Count < 2 || list2.Count < 2 || (list1.Count * list2.Count < 16))
            {
                foreach (var element1 in list1)
                {
                    foreach (var element2 in list2)
                    {
                        if (EqualityComparer<T>.Default.Equals(element1, element2)) 
                            return true;
                    }
                }

                return false;
            }
            else
                return list1.Intersect(list2).Any();
        }

        public static string ToBase64String(this Guid guid)
        {
            return ByteArrayToBase64String(guid.ToByteArray());
        }

        public static double GetLatitudeAtDistanceInMiles(double latitude, double distance)
        {
            return latitude + (distance / 69.1);
        }

        public static double GetLongitudeAtDistanceInMiles(double latitude, double longitude, double distance)
        {
            return longitude + (distance / (Math.Cos(latitude / 57.3) * 69.1));
        }

        public static double GetLatLongDistanceInMiles(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            var x = 69.1 * (latitude2 - latitude1);
            var y = 69.1 * (longitude2 - longitude1) * Math.Cos(latitude1 / 57.3);

            return Math.Sqrt(x * x + y * y);            
        }

        public static bool IsTrueOrNull(this bool? bn)
        {
            return !bn.HasValue || bn.Value;
        }

        public static bool IsTrue(this bool? bn)
        {
            return bn.HasValue && bn.Value;
        }

        public static bool IsFalse(this bool? bn)
        {
            return bn.HasValue && !bn.Value;
        }

        public static bool IsFalseOrNull(this bool? bn)
        {
            return !bn.HasValue || !bn.Value;
        }

        public static bool IsNullOrValue(this int? numberToCompare, int value)
        {
            return numberToCompare == null || numberToCompare.Value == value;
        }

        public static bool IsNullOrNotValue(this int? numberToCompare, int value)
        {
            return numberToCompare == null || numberToCompare.Value != value;
        }

        public static double GetLatLongDistanceInMiles(double latitude1, double longitude1, double latitude2, double longitude2, out double latitudeDifference, out double longitudeDifference)
        {
            var avgLongitude = (longitude1 + longitude2) / 2;
            var avgLatitude = (latitude1 + latitude2) / 2;
            latitudeDifference = GetLatLongDistanceInMiles(latitude1, avgLongitude, latitude2, avgLongitude); ;
            longitudeDifference = GetLatLongDistanceInMiles(avgLatitude, longitude1, avgLatitude, longitude2); ;

            return GetLatLongDistanceInMiles(latitude1, longitude1, latitude2, longitude2);
        }


        public static IEnumerable<XElement> GetElementsByPath(this XElement startElement, params string[] elementNamesInPath)
        {
            var currentElement = startElement;
            for (var elementNameIndex = 0; elementNameIndex < elementNamesInPath.Length - 1; elementNameIndex++)
            {
                var elementName = elementNamesInPath[elementNameIndex];
                currentElement = currentElement.Element(elementName);
                if (currentElement == null)
                    break;
            }

            return currentElement.IfNotNull(e => e.Elements(elementNamesInPath.Last()), Enumerable.Empty<XElement>());
        }

        public static string GetPathNodeValue(this XElement startElement, string attributeName
            , params string[] elementNamesInPath)
        {
            var nodeValues = GetPathNodeValueRecursive(startElement, attributeName, elementNamesInPath, 0);

            var concatenatedNodeValue = Utils.Concat("~", nodeValues);

            return concatenatedNodeValue;
        }


        private static IEnumerable<string> GetPathNodeValueRecursive(XElement startElement, string attributeName
            , string[] elementNamesInPath, int startElementNamesInPath)
        {
            if (startElementNamesInPath < elementNamesInPath.Length)
            {
                var elementName = elementNamesInPath[startElementNamesInPath];
                var elements = startElement.Elements(elementName);
                foreach (var element in elements)
                {
                    var nodeValues = GetPathNodeValueRecursive(element, attributeName, elementNamesInPath, startElementNamesInPath + 1);
                    foreach (var nodeValue in nodeValues)
                        yield return nodeValue;
                }
            }
            else
            {
                if (attributeName == null)
                    yield return startElement.Value;
                else
                {
                    foreach (var attribute in startElement.Attributes(attributeName))
                    {
                        yield return attribute.Value;
                    }
                }
            }
        }


        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> seqOfSeq)
        {
            foreach (var seq in seqOfSeq)
            {
                foreach (var element in seq)
                {
                    yield return element;
                }
            }
        }

        public static string[] GetLines(string text)
        {
            return (text ?? string.Empty).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsNullOrEmpty<T>(this T[] values)
        {
            return values == null || values.Length == 0;
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        private static Encoding asciiEncoder = Encoding.GetEncoding("iso-8859-8");  //ISO-Visual.Hebrew encoder http://www.codeproject.com/KB/cs/EncodingAccents.aspx
        public static string ConvertCharsToASCII(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                var bytes = asciiEncoder.GetBytes(input); //removes accents: "Olé" -> "Ole"
                var result = Encoding.UTF8.GetString(bytes);
                return result;
            }
            else return input;
        }


        private const int BigramLevel = 1, TrigramLevel = 2;
        /// <summary>
        /// ASUUMPTIONS: pased indexes are valid
        /// </summary>
        public static bool IsNGramMatch(string[] tokens1, string[] tokens2, ref int tokens1Index, ref int tokens2Index)
        {
            var currentToken1 = tokens1[tokens1Index];
            var currentToken2 = tokens2[tokens2Index];

            if (currentToken1 != currentToken2)
            {
                string token1Bigram;
                if (!DoesNGramLeftToRightMatch(tokens1, tokens2, ref tokens1Index, tokens2Index, BigramLevel, currentToken1, out token1Bigram))
                {
                    string token1Trigram;
                    if (!DoesNGramLeftToRightMatch(tokens1, tokens2, ref tokens1Index, tokens2Index, TrigramLevel, token1Bigram, out token1Trigram))
                    {
                        string token2Bigram;
                        if (!DoesNGramLeftToRightMatch(tokens2, tokens1, ref tokens2Index, tokens1Index, BigramLevel, currentToken2, out token2Bigram))
                        {
                            string token2Trigram;
                            return DoesNGramLeftToRightMatch(tokens2, tokens1, ref tokens2Index, tokens1Index, TrigramLevel, token2Bigram, out token2Trigram);
                        }
                        else return true;
                    }
                    else return true;
                }
                else return true;
            }
            else return true;
        }

        public static string Concat(string delimiter, bool ignoreNullOrEmptyValues, params string[] values)
        {
            return Concat(values, delimiter, ignoreNullOrEmptyValues);
        }

        public static string Concat(string[] values, string delimiter, bool ignoreNullOrEmptyValues)
        {
            return Concat(values, delimiter, ignoreNullOrEmptyValues, 0, null);
        }


        public static void DebugBreakIfEqual<T>(T value1Source, T value1Target) where T : IComparable<T>
        {
            if (value1Source.CompareTo(value1Target) == 0)
            {
                Console.Beep();
                Debugger.Break();
            }
        }

        public static void DebugBreakIfEqual<T>(T value1Source, T value1Target, T value2Source, T value2Target) where T : IComparable<T>
        {
            if (value1Source.CompareTo(value1Target) == 0 || value1Source.CompareTo(value2Target) == 0 
                || value2Source.CompareTo(value1Target) == 0 || value2Source.CompareTo(value2Target) == 0)
            {
                Console.Beep();
                Debugger.Break();
            }
        }

        public static T[] CreateInitializedArray<T>(int length, Func<int,T> elementFactory)
        {
            var array = new T[length];

            if (elementFactory != null)
            {
                for (var i = 0; i < length; i++)
                {
                    array[i] = elementFactory(i);
                }
            }

            return array;
        }

        public static T[][] CreateJaggedArray<T>(int m, int n)
        {
            var v = new T[m][];
            for (var i = 0; i < m; ++i) v[i] = new T[n];
            return v;
        }

        public static string Concat(string[] tokens, string delimiter, bool ignoreNullOrEmptyValues, int startIndex, Func<string, string> tokenProcessor)
        {
            if (tokens == null || tokens.Length == 0)
                return String.Empty;
            else
            {
                if (tokens.Length == 1)
                    return tokens[0] ?? String.Empty;
                else
                {
                    var concateResult = new StringBuilder();

                    for (var tokenIndex = startIndex; tokenIndex < tokens.Length; tokenIndex++)
                    {
                        var token = tokens[tokenIndex];

                        if (tokenProcessor != null)
                            token = tokenProcessor(token) ?? String.Empty;

                        if (ignoreNullOrEmptyValues && String.IsNullOrEmpty(token))
                            continue;

                        if (concateResult.Length > 0)
                            concateResult.Append(delimiter);

                        concateResult.Append(token);
                    }

                    return concateResult.ToString();
                }
            }
        }

        private static bool DoesNGramLeftToRightMatch(string[] tokens1, string[] tokens2, ref int tokens1Index, int tokens2Index, int ngramLevel, string previousNGram, out string ngram)
        {
            var tokens1NGramIndex = tokens1Index + ngramLevel;
            if (tokens1NGramIndex < tokens1.Length)
            {
                ngram = String.Concat(previousNGram, tokens1[tokens1NGramIndex]);
                var isMatch = (ngram == tokens2[tokens2Index]);
                if (isMatch)
                    tokens1Index = tokens1NGramIndex;

                return isMatch;
            }
            else
            {
                ngram = null;
                return false;
            }
        }

        public static void Swap<T>(ref T value1, ref T value2)
        {
            var temp = value1;
            value1 = value2;
            value2 = temp;
        }

        public static bool SwapIfLeftIsLargerValue(ref string leftValue, ref string rightValue)
        {
            if (String.CompareOrdinal(leftValue, rightValue) > 0)
            {
                Swap(ref leftValue, ref rightValue);
                return true;
            }
            else return false;
        }

        public static bool SwapIfLeftIsLargerLength(ref string leftValue, ref string rightValue)
        {
            if (leftValue.Length > rightValue.Length || (leftValue.Length == rightValue.Length) && String.CompareOrdinal(leftValue, rightValue) > 0)
            {
                Swap(ref leftValue, ref rightValue);
                return true;
            }
            else return false;
        }

        public static void ReplaceBiTriGramsFromLeftSet(Dictionary<string, int> rightTokensSet, Dictionary<string, int> leftTokensSet)
        {
            var rightTokens = rightTokensSet.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();

            for(var rightTokenIndex = 1; rightTokenIndex < rightTokens.Length; rightTokenIndex++)
            {
                var bigram = String.Concat(rightTokens[rightTokenIndex - 1], rightTokens[rightTokenIndex]);
                string trigram = null;
                if (rightTokenIndex > 1)
                    trigram = String.Concat(rightTokens[rightTokenIndex - 2], bigram);

                int leftTokenIndex;
                var trigramFound = false;
                if (trigram != null)
                {
                    trigramFound = leftTokensSet.TryGetValue(trigram, out leftTokenIndex);
                    if (trigramFound)
                    {
                        leftTokensSet.Remove(trigram);
                        leftTokensSet[rightTokens[rightTokenIndex-2]] = leftTokenIndex;
                        leftTokensSet[rightTokens[rightTokenIndex-1]] = leftTokenIndex + 1;
                        leftTokensSet[rightTokens[rightTokenIndex]] = leftTokenIndex + 2;
                    }
                }
                    
                if (!trigramFound && leftTokensSet.TryGetValue(bigram, out leftTokenIndex))
                {
                    leftTokensSet.Remove(bigram);
                    leftTokensSet[rightTokens[rightTokenIndex-1]] = leftTokenIndex;
                    leftTokensSet[rightTokens[rightTokenIndex]] = leftTokenIndex + 1;
                }
            }
        }

        /// <summary>
        /// ASSUMPTIONS: Each array is not-null, both arrays are of same length
        /// </summary>
        public static bool IsStringArrayEqual(string[] array1, string[] array2, bool ignoreAnyNullOrEmptyValue)
        {
            for(var arrayIndex = 0; arrayIndex < array1.Length; arrayIndex++)
            {
                if (array1[arrayIndex] == array2[arrayIndex] ||
                    (ignoreAnyNullOrEmptyValue && (String.IsNullOrEmpty(array1[arrayIndex]) ||  String.IsNullOrEmpty(array2[arrayIndex])))
                   )
                    continue;
                else
                {
                    return false;
                }
            }

            return true;
        }


        public static bool Is1EditDistanceAway(string token1, string token2, int minTokenLengthFor1EditDistanceErrors, int minTokenLengthFor2EditDistanceErrors)
        {
            bool is1EditDistanceAway;
            bool is2EditDistanceAway;

            Is1EditDistanceAway(token1, token2, minTokenLengthFor1EditDistanceErrors, minTokenLengthFor2EditDistanceErrors, out is1EditDistanceAway, out is2EditDistanceAway);

            return is1EditDistanceAway || is2EditDistanceAway;
        }

        public static void Is1EditDistanceAway(string token1, string token2, int minTokenLengthFor1EditDistanceErrors, int minTokenLengthFor2EditDistanceErrors, out bool is1EditDistanceAway, out bool is2EditDistanceAway)
        {
            if (token1.Length == token2.Length)    //1 edit distance update scenarios
            {
                if (token1.Length > minTokenLengthFor1EditDistanceErrors)
                {
                    var charDiffCount = 0;
                    for (var tokenIndex = 0; tokenIndex < token1.Length; tokenIndex++)
                    {
                        if (token1[tokenIndex] != token2[tokenIndex])
                        {
                            charDiffCount++;
                            if (charDiffCount > 2) break;   //detect up to 2 errors
                        }
                    }
                    is1EditDistanceAway = (token1.Length > minTokenLengthFor1EditDistanceErrors && charDiffCount == 1);        //Allow 1 update error for specified min length
                    is2EditDistanceAway = (token1.Length > minTokenLengthFor2EditDistanceErrors && charDiffCount == 2);    //Allow 2 update error for twice of specified min length
                }
                else
                {
                    is1EditDistanceAway = false;
                    is2EditDistanceAway = false;
                }
            }
            else if (token1.Length + token2.Length > (minTokenLengthFor1EditDistanceErrors * 2) && Math.Abs(token1.Length - token2.Length) == 1)    //1 edit distance delete scenarios
            {
                string smallerToken, largerToken;
                if (token1.Length > token2.Length)
                {
                    smallerToken = token2;
                    largerToken = token1;
                }
                else
                {
                    smallerToken = token1;
                    largerToken = token2;
                }

                var charDiffCount = 0;
                var largerTokenIndex = 0;
                var smallerTokenIndex = 0;
                while (smallerTokenIndex < smallerToken.Length && largerTokenIndex < largerToken.Length && charDiffCount < 2)
                {
                    if (smallerToken[smallerTokenIndex] != largerToken[largerTokenIndex])
                    {
                        largerTokenIndex++;
                        charDiffCount++;
                    }
                    else
                    {
                        smallerTokenIndex++;
                        largerTokenIndex++;
                    }
                }
                if (charDiffCount == 0) charDiffCount = 1;  //for loop ended without char diff so count for last char

                is1EditDistanceAway = charDiffCount == 1;
                is2EditDistanceAway = false;
            }
            else
            {
                is1EditDistanceAway = false;
                is2EditDistanceAway = false;
            }
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> kvps, bool updateExistingValues)
        {
            foreach (var kvp in kvps)
            {
                if (!dictionary.ContainsKey(kvp.Key))
                {
                    dictionary.Add(kvp);
                }
                else if (updateExistingValues)
                {
                    dictionary[kvp.Key] = kvp.Value;
                }
            }
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            if (keys == null)
                return;

            using (var valuesEnumerator = values.GetEnumerator())
            {
                foreach (var key in keys)
                {
                    var valueAvailable = valuesEnumerator.MoveNext();
                    if (!valueAvailable)
                        throw new IndexOutOfRangeException("values sequence is of less size than keys sequence");
                    dictionary.Add(key, valuesEnumerator.Current);
                }
            }
        }

        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> elementsToAdd, bool allowDuplicates)
        {
            if (elementsToAdd == null)
                return;

            foreach (var element in elementsToAdd)
            {
                var isAdded = set.Add(element);
                if (!isAdded && !allowDuplicates)
                    throw new ArgumentException("Attempto add duplicate elements when duplicates were not expected");
            }            
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elementsToAdd)
        {
            if (elementsToAdd == null)
                return;

            foreach (var element in elementsToAdd)
            {
                collection.Add(element);
            }
        }
        public static bool RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keysToRemove)
        {
            var isAnyRemoved = false;
            foreach (var key in keysToRemove)
            {
                var isRemoved = dictionary.Remove(key);
                isAnyRemoved = isAnyRemoved || isRemoved;
            }

            return isAnyRemoved;
        }

        public static bool RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> elementsToRemove)
        {
            if (elementsToRemove == null)
                return false;

            var isAnyRemoved = false;
            foreach (var element in elementsToRemove)
            {
                var isRemoved = collection.Remove(element);
                isAnyRemoved = isAnyRemoved || isRemoved;
            }

            return isAnyRemoved;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sequence)
        {
            return ToHashSet(sequence, false, false);
        }
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sequence, bool returnNullIfEmpty, bool trimExcess)
        {
            HashSet<T> hashSet = returnNullIfEmpty ? null : new HashSet<T>(sequence);

            if (returnNullIfEmpty)
            {
                foreach (var item in sequence)
                {
                    if (hashSet == null) 
                        hashSet = new HashSet<T>();

                    hashSet.Add(item);
                }
            }

            if (trimExcess && hashSet != null)
                hashSet.TrimExcess(); 

            return hashSet;
        }
        public static Queue<T> ToQueue<T>(this IEnumerable<T> sequence)
        {
            var queue = new Queue<T>(sequence);
            //queue.TrimExcess();   //Commented because GC perf is not measured
            return queue;
        }
        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> sequence)
        {
            return new LinkedList<T>(sequence);
        }

        public const string IntMinValueAsString = "-2147483648";
        public const string IntMaxValueAsString =  "2147483647";

        public static SortedList<TKey, TValue> ToSortedList<TItem, TKey, TValue>(this IEnumerable<TItem> items, Func<TItem, TKey> getKey, Func<TItem, TValue> getValue, bool trimExcess)
        {
            var sortedList = new SortedList<TKey, TValue>();
            foreach (var item in items)
                sortedList.Add(getKey(item), getValue(item));

            if (trimExcess)
                sortedList.TrimExcess();

            return sortedList;
        }
        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TItem, TKey, TValue>(this IEnumerable<TItem> items, Func<TItem, TKey> getKey, Func<TItem, TValue> getValue)
        {
            var sortedDictionary = new SortedDictionary<TKey, TValue>();
            foreach (var item in items)
                sortedDictionary.Add(getKey(item), getValue(item));

            return sortedDictionary;
        }

        public static void AddToDictionarySet<TKeyType, TValueType, TCollectionType>(this IDictionary<TKeyType, TCollectionType> dictionary, TKeyType key, TValueType valueToAdd) where TCollectionType:ICollection<TValueType>, new()
        {
            TCollectionType existingValue;
            if (!dictionary.TryGetValue(key, out existingValue))
            {
                existingValue = new TCollectionType();
                dictionary.Add(key, existingValue);
            }

            existingValue.Add(valueToAdd);
        }

        public static string SerializeToJson<T>(DataContractJsonSerializer serializer, T objectToSerialize)
        {
            using (var buffer = new MemoryStream())
            {
                serializer.WriteObject(buffer, objectToSerialize);
                buffer.Flush();
                buffer.Position = 0;
                using (var stream = new StreamReader(buffer))
                {
                    return stream.ReadToEnd();
                }
            }
        }

        public static T DeserializeFromJson<T>(DataContractJsonSerializer serializer, string jsonToDeserialize) where T:class 
        {
            if (String.IsNullOrEmpty(jsonToDeserialize))
                return null;

            using (var buffer = new MemoryStream())
            {
                using (var stream = new StreamWriter(buffer))
                {
                    stream.Write(jsonToDeserialize);
                    stream.Flush();
                    buffer.Flush();

                    buffer.Position = 0;
                    var deserializedObject = serializer.ReadObject(buffer) as T;

                    return deserializedObject;
                }
            }          
        }

        public static readonly XmlSerializerNamespaces XmlEmptyNamespace = GetEmptyNameSpace();
        private static XmlSerializerNamespaces GetEmptyNameSpace()
        {
            var emptyNamespace = new XmlSerializerNamespaces();
            emptyNamespace.Add(String.Empty, String.Empty);
            return emptyNamespace;
        }


        /// <summary>
        /// Use carefully as there may be huge perf implication
        /// </summary>
        public static T[] AppendToArray<T>(T[] array, T valueToAppend)
        {
            if (array == null)
                array = new T[] { valueToAppend };
            else
            {
                Array.Resize(ref array, array.Length + 1);
                array[array.Length - 1] = valueToAppend;
            }

            return array;
        }

        public static string RandomIfEmpty(string valueToCheck)
        {
            return String.IsNullOrEmpty(valueToCheck) ? Guid.NewGuid().ToString() : valueToCheck;
        }
        public static string RandomIfEmpty(string valueToCheck, string alternativeValue)
        {
            return String.IsNullOrEmpty(valueToCheck) ? RandomIfEmpty(alternativeValue) : valueToCheck;
        }

        public static string RemoveChars(this string stringValue, HashSet<char> charsToRemove)
        {
            return new string(stringValue.Where(c => !charsToRemove.Contains(c)).ToArray());
        }

        /// <summary>
        /// This method is highly optimized to avoid any dynamic memory allocation of number of values are less than 2.
        /// This method is expected to be called many times when number of values are less than 2.
        /// </summary>
        public static string Concat(string delimiter, IEnumerable<string> values)
        {
            StringBuilder buffer = null;
            var firstValue = String.Empty;
            var valueCount = 0;

            foreach (var value in values)
            {
                switch (valueCount)
                {
                    case 0:
                        firstValue = value;
                        break;
                    case 1:
                        buffer = new StringBuilder(firstValue);
                        buffer.Append(delimiter);
                        buffer.Append(value);
                        break;
                    default:
                        buffer.Append(delimiter);
                        buffer.Append(value);
                        break;
                }

                valueCount++;
            }

            switch (valueCount)
            {
                case 0:
                    return firstValue;  //Empty enumerator
                case 1:
                    return firstValue;
                default:
                    return buffer.ToString();
            }
        }

        public static bool ParseBool(string value, Func<bool> getDefaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return getDefaultValue();
            else
            {
                if (string.Compare(value, Boolean.TrueString, true, CultureInfo.InvariantCulture) == 0)
                    return true;
                else if (string.Compare(value, Boolean.FalseString, true, CultureInfo.InvariantCulture) == 0)
                    return false;
                else
                {
                    int convertedInt;
                    if (int.TryParse(value, out convertedInt))
                        return convertedInt > 0;
                    else
                    {
                        if (string.Compare(value, "yes", true, CultureInfo.InvariantCulture) == 0
                            || string.Compare(value, "y", true, CultureInfo.InvariantCulture) == 0)
                            return true;
                        else if (string.Compare(value, "no", true, CultureInfo.InvariantCulture) == 0
                            || string.Compare(value, "n", true, CultureInfo.InvariantCulture) == 0)
                            return false;
                        else
                            return getDefaultValue();
                    }
                }
            }
        }

        public static bool ParseBool(string value, bool defaultValue)
        {
            return ParseBool(value, () => defaultValue);
        }

        public static bool IsNullOrEmpty<T>(ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }
        public static bool IsNullOrEmpty<T>(LinkedList<T> linkedList)
        {
            return linkedList == null || linkedList.First == null;
        }

        public static void RemoveWhere<T>(this LinkedList<T> linkedList, Func<T, bool> predicate)
        {
            var linkedListNodeToCheck = linkedList.First;
            while (linkedListNodeToCheck != null)
            {
                if (predicate(linkedListNodeToCheck.Value))
                    linkedListNodeToCheck = linkedList.RemoveAndGetNext(linkedListNodeToCheck);
                else
                    linkedListNodeToCheck = linkedListNodeToCheck.Next;
            }
        }

        public static void RemoveKeys<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IList<TKey> keysToRemove)
        {
            foreach (var key in keysToRemove)
                dictionary.Remove(key);
        }

        public static void RemoveWhere<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, bool> predicate)
        {
            var keysToRemove = dictionary.Keys.Where(predicate).ToArray();
            foreach (var keyToRemove in keysToRemove)
                dictionary.Remove(keyToRemove);
        }

        public static IEnumerable<LinkedListNode<T>> Nodes<T>(this LinkedList<T> linkedList)
        {
            var linkedListNode = linkedList.First;
            while (linkedListNode != null)
            {
                yield return linkedListNode;
                linkedListNode = linkedListNode.Next;
            }
        }

        public static IEnumerable<KeyValuePair<LinkedListNode<T>, int>> NodesAndIndex<T>(this LinkedList<T> linkedList)
        {
            var linkedListNode = linkedList.First;
            var index = 0;
            while (linkedListNode != null)
            {
                yield return new KeyValuePair<LinkedListNode<T>, int>(linkedListNode, index);
                linkedListNode = linkedListNode.Next;
                index++;
            }
        }

        public static LinkedListNode<T> RemoveAndGetNext<T>(this LinkedList<T> linkedList, LinkedListNode<T> nodeToRemove)
        {
            var tempNext = nodeToRemove.Next;
            linkedList.Remove(nodeToRemove);
            return tempNext;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue existingValue;
            if (dictionary.TryGetValue(key, out existingValue))
                return existingValue;
            else
                return defaultValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> getDefaultValue)
        {
            TValue existingValue;
            if (dictionary.TryGetValue(key, out existingValue))
                return existingValue;
            else
                return getDefaultValue != null ? getDefaultValue() : default(TValue);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            return GetValueOrDefault(dictionary, key, () => defaultValue);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return GetValueOrDefault(dictionary, key, () => default(TValue));
        }

        /// <summary>
        /// Creates a URL friendly slug from a string
        /// </summary>
        public static string ToUrlFriendlyValue(this string value)
        {
            // Repalce everything other than alphanum and _
            var cleanedValue = Regex.Replace(value, @"[\W]+", "-", RegexOptions.Compiled);
            return cleanedValue.ToLowerInvariant();
        }

        public static int ToInt(this bool booleanValue)
        {
            return booleanValue ? 1 : 0;
        }
        public static bool ToBool(this int intValue)
        {
            if (intValue == 0)
                return false;
            else if (intValue == 1)
                return true;
            else
                throw new ArgumentOutOfRangeException(String.Format("intValue is expected to be either 0 or 1 but it's {0}", intValue));
        }

        public static bool AddIfNotExist<TKey,TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value, bool updateExistingValue, IComparer<TValue> valueComparer)
        {
            TValue existingValue;
            var exist = dictionary.TryGetValue(key, out existingValue);

            if (!exist)
                dictionary.Add(key, value);
            else if (updateExistingValue)
            {
                var updateRequired = valueComparer == null || valueComparer.Compare(existingValue, value) != 0;
                if (updateRequired)
                    dictionary[key] = value;    
            }

            return exist;
        }

        public static TValueType AddOrGetFromDictionary<TKeyType, TValueType>(IDictionary<TKeyType, TValueType> dictionary, TKeyType key, Func<TKeyType, TValueType> createDictionaryValueForKey)
        {
            TValueType existingValue;

            if (!dictionary.TryGetValue(key, out existingValue))
            {
                existingValue = createDictionaryValueForKey(key);
                dictionary.Add(key, existingValue);
            }

            return existingValue;
        }

        public readonly static string[] EmptyStringArray = new string[]{};

        public static void Exchange<T>(IList<T> items, int index1, int index2)
        {
            if (index1 == index2)
                return;

            var temp = items[index1];
            items[index1] = items[index2];
            items[index2] = temp;
        }

        public static bool IsAnyNull(params object[] objects)
        {
            return objects.Any(o => o == null);
        }

        public static double Average(params double[] values)
        {
            return values.Average();
        }

        public static T[] ScalerToArray<T>(this T obj)
        {
            return new T[] {obj};
        }

        public static float Square(float number)
        {
            return number * number;
        }
        public static int Square(int number)
        {
            return number * number;
        }
        public static double Square(double number)
        {
            return number*number;
        }

        /// <summary>
        /// Walks through current call stack and returns the first frame from the top that is outside of 
        /// specified type/
        /// </summary>
        /// <param name="t">The type outside of which call stack frame is required.</param>
        /// <param name="stackFrameSearchStartIndex">The search of stack frame begins at this index. The 0 being the top most
        /// and will always point to this perticular method. The value 1 will be caller's stack frame. So usually 2 is the 
        /// starting frame outside caller (unless there are overloads). This parameter enhances performance by having to search through all 
        /// frames starting from 0 however if unsure then just pass 1.
        /// </param>
        /// <returns>MethodBase object that represents the method in call stack that was found out of type T</returns>
        /// <remarks>
        /// This function is perticularly useful when you want to get the name of the method, for example, to set in timer
        /// property or report in error log etc.
        /// </remarks>
        /// <RevisionHistory>
        /// 	<Revision Author="shitals" Date="8/25/2008 4:01 PM">Created</Revision>
        /// </RevisionHistory>
        public static StackFrame GetFirstMethodCallOutsideOfType(Type t, int stackFrameSearchStartIndex, StackTrace callStackToScan, out MethodBase userCodeMethodBase)
        {
            StackFrame userCodeStackFrame = null;   //By default we'll return null
            userCodeMethodBase = null;

            if (callStackToScan == null)
                callStackToScan = new StackTrace();    //Get current call stack

            //Loop through stack frames starting from specified search index
            for (var index = stackFrameSearchStartIndex; index < callStackToScan.FrameCount; index++)
            {
                var thisFrame = callStackToScan.GetFrame(index);
                var thisMethod = thisFrame.GetMethod();

                //Does this frame outside of specified type?
                if (thisMethod.DeclaringType != t)
                {
                    //yes return it
                    userCodeStackFrame = thisFrame;
                    userCodeMethodBase = thisMethod;
                    break;
                }
            }

            return userCodeStackFrame;
        }


        /// <summary>
        /// Converts specified base64 encoded string to byte array
        /// </summary>
        /// <param name="stringToConvert">Base64 encoded string</param>
        /// <returns></returns>
        public static byte[] Base64StringToByteArray(string stringToConvert)
        {
            return Convert.FromBase64String(stringToConvert);
        }

        public static Guid Base64StringToGuid(string valueToConvertFrom)
        {
            if (!string.IsNullOrEmpty(valueToConvertFrom))
                return new Guid(Base64StringToByteArray(valueToConvertFrom));
            else
                return Guid.Empty;
        }

        /// <summary>
        /// Converts specified byte array to base encoded string
        /// </summary>
        /// <param name="byteArrayToConvert">byte array to convert</param>
        /// <returns></returns>
        public static string ByteArrayToBase64String(byte[] byteArrayToConvert)
        {
            return Convert.ToBase64String(byteArrayToConvert);
        }

        /// <summary>
        /// Serialized entire object graph in to byte array
        /// </summary>
        /// <param name="anyObject">Object to serialize</param>
        /// <returns></returns>
        public static byte[] SerializeObject(object anyObject)
        {
            var serializationFormatter = new BinaryFormatter();
            var buffer = new MemoryStream();
            serializationFormatter.Serialize(buffer, anyObject);
            return buffer.ToArray();
        }

        /// <summary>
        /// Deserialized byte array back to original object graph
        /// </summary>
        /// <param name="serializedObjectGraph">Byte array to deserialized</param>
        /// <returns></returns>
        public static object DeserializeObject(byte[] serializedObjectGraph)
        {
            var serializationFormatter = new BinaryFormatter();
            var buffer = new MemoryStream(serializedObjectGraph);
            return serializationFormatter.Deserialize(buffer);
        }

        /// <summary>
        /// Converts specified byte array in to DataTable using the specified sceham for the data table.
        /// The byte array must have been formed by SerializeObject call on to array of DataRow.ItemArray
        /// of original data table. If schema or byte array is not specified then this call returns null.
        /// If byte array is empty but schema is specified then it returns empty data table.
        /// </summary>
        /// <param name="serializedTableData">Byte array of data table</param>
        /// <param name="tableSchema">Schema of original data table</param>
        /// <returns></returns>
        public static DataTable DeserializeDataTable(byte[] serializedTableData, string tableSchema)
        {
            DataTable table;
            if (tableSchema != null)
            {
                table = new DataTable();
                table.ReadXmlSchema(new StringReader(tableSchema));

                if (serializedTableData != null && serializedTableData.Length > 0)
                {
                    object[][] itemArrayForRows = (object[][])DeserializeObject(serializedTableData);
                    for (int rowIndex = 0; rowIndex < itemArrayForRows.Length; rowIndex++)
                    {
                        table.Rows.Add(itemArrayForRows[rowIndex]);
                    }
                }
                //else leave table empty
            }
            else
                table = null;

            return table;
        }

        public static double ParseOrDefault(string numberAsString, double defaultValue, bool useDefaultValueIfNaN)
        {
            double result;
            if (!String.IsNullOrEmpty(numberAsString))
            {
                if (!Double.TryParse(numberAsString, out result))
                    result = defaultValue;
                else if (useDefaultValueIfNaN && Double.IsNaN(result))
                    result = defaultValue;
            }
            else
                result = defaultValue;

            return result;
        }


        public static string AddToNumberAsString(string numberAsString, double numberToAdd)
        {
            string result;
            if (numberAsString == null)
                result = numberToAdd.ToString(CultureInfo.InvariantCulture);
            else
                result = (Double.Parse(numberAsString) + numberToAdd).ToString(CultureInfo.InvariantCulture);

            return result;
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source, int randomSeed)
        {
            var rnd = new Random(randomSeed);
            return source.OrderBy((item) => rnd.Next());
        }

        public static IEnumerable<T> Slice<T>(this T[] source, int start, int end)
        {
            for(var index = start; index <= end && index < source.Length && start >= 0; index++)
                yield return source[index];
        }

        public static IEnumerable<T> Slice<T>(this T[] source, int start)
        {
            for (var index = start; index < source.Length && start >= 0; index++)
                yield return source[index];
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] items)
        {
            foreach (var sourceItem in source)
                yield return sourceItem;

            foreach (var item in items)
                yield return item;
        }

        static public float Sift3Distance(string s1, string s2, int maxOffset)
        {
            if (String.IsNullOrEmpty(s1))
                return
                String.IsNullOrEmpty(s2) ? 0 : s2.Length;
            if (String.IsNullOrEmpty(s2))
                return s1.Length;
            int c = 0;
            int offset1 = 0;
            int offset2 = 0;
            int lcs = 0;
            while ((c + offset1 < s1.Length) && (c + offset2 < s2.Length))
            {
                if (s1[c + offset1] == s2[c + offset2])
                {
                    lcs++;
                }
                else
                {
                    offset1 = 0;
                    offset2 = 0;
                    for (int i = 0; i < maxOffset; i++)
                    {
                        if ((c + i < s1.Length)
                        && (s1[c + i] == s2[c]))
                        {
                            offset1 = i;
                            break;
                        }
                        if ((c + i < s2.Length)
                        && (s1[c] == s2[c + i]))
                        {
                            offset2 = i;
                            break;
                        }
                    }
                }
                c++;
            }
            return (s1.Length + s2.Length) / 2 - lcs;
        }


        public static bool DoesSetsOverlap<T>(this HashSet<T> set1, HashSet<T> set2)
        {
            if (set1.Count < set2.Count)
                return set2.Overlaps(set1);
            else
                return set1.Overlaps(set2);
        }

        //Purpose of this method is to prevent JIT to remove variables when debugging. So you can use this call instead of Debug.WriteLine.
        public static int DoNothing(object o)
        {
            return o == null ? 0 : 1;
        }
    }
}

