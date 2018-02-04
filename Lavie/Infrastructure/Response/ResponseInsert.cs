using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lavie.Infrastructure
{
    public class ResponseInsert
    {
        private readonly Func<int, string> _getValue;
        private readonly ResponseInsertMode _mode;
        private readonly Func<string, IEnumerable<string>> _selectorFunction;
        private readonly Func<string, IEnumerable<string>, ResponseInsertMode, Func<int, string>, string> _insertValueFunction;

        public ResponseInsert(Func<int, string> getValue, ResponseInsertMode mode, string selector)
            : this(getValue, mode, GenerateSelectorFunction(selector))
        {
        }

        public ResponseInsert(Func<int, string> getValue, ResponseInsertMode mode, string selector, Func<string, IEnumerable<string>, ResponseInsertMode, Func<int, string>, string> insertValueFunction)
            : this(getValue, mode, GenerateSelectorFunction(selector), insertValueFunction)
        {
        }

        public ResponseInsert(Func<int, string> getValue, ResponseInsertMode mode, Func<string, IEnumerable<string>> selectorFunction)
            : this(getValue, mode, selectorFunction, DefaultInsertValue)
        {
        }

        public ResponseInsert(Func<int, string> getValue, ResponseInsertMode mode, Func<string, IEnumerable<string>> selectorFunction, Func<string, IEnumerable<string>, ResponseInsertMode, Func<int, string>, string> insertValueFunction)
        {
            this._getValue = getValue;
            this._mode = mode;
            this._selectorFunction = selectorFunction;
            this._insertValueFunction = insertValueFunction;
        }

        public void Apply(ref string doc, ref bool modifiedDoc)
        {
            IEnumerable<string> elements = _selectorFunction(doc);

            if (elements == null || elements.Count() <= 0) return;

            doc = _insertValueFunction(doc, elements, _mode, _getValue);

            modifiedDoc = true;
        }

        private static string InsertValueOnElements(string doc, IEnumerable<string> elements, Func<string, int, string> getElementReplacement)
        {
            int index = 0;

            foreach (string element in elements)
            {
                doc = doc.Replace(element, getElementReplacement(element, index));

                index++;
            }

            return doc;
        }

        private static string DefaultInsertValue(string doc, IEnumerable<string> elements, ResponseInsertMode mode, Func<int, string> getValue)
        {
            switch (mode)
            {
                case ResponseInsertMode.ReplaceWith:
                    return InsertValueOnElements(doc, elements, (e, i) => ReplaceWith(getValue(i)));
                case ResponseInsertMode.InsertBefore:
                    return InsertValueOnElements(doc, elements, (e, i) => InsertBefore(e, getValue(i)));
                case ResponseInsertMode.InsertAfter:
                    return InsertValueOnElements(doc, elements, (e, i) => InsertAfter(e, getValue(i)));
                case ResponseInsertMode.AppendTo:
                    return InsertValueOnElements(doc, elements, (e, i) => AppendTo(e, getValue(i)));
                case ResponseInsertMode.PrependTo:
                    return InsertValueOnElements(doc, elements, (e, i) => PrependTo(e, getValue(i)));
                case ResponseInsertMode.Wrap:
                    return InsertValueOnElements(doc, elements, (e, i) => Wrap(e, getValue(i)));
                case ResponseInsertMode.Remove:
                    return InsertValueOnElements(doc, elements, (e, i) => Remove(e));
            }

            return doc;
        }

        private static string ReplaceWith(string value)
        {
            return value;
        }

        private static string InsertBefore(string element, string value)
        {
            return value + element;
        }

        private static string InsertAfter(string element, string value)
        {
            return element + value;
        }

        private static readonly Regex _closingTagRegex = new Regex(@"(</[^<>]+>)$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static string AppendTo(string element, string value)
        {
            return _closingTagRegex.Replace(element, new MatchEvaluator(m => value + m.Value));
        }

        private static readonly Regex _openingTagRegex = new Regex(@"^(<[^<>]+>)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static string PrependTo(string element, string value)
        {
            return _openingTagRegex.Replace(element, new MatchEvaluator(m => m.Value + value));
        }

        private static string Wrap(string element, string value)
        {
            return string.Format(value, element);
        }

        private static string Remove(string element)
        {
            return String.Empty;
        }

        private static readonly Dictionary<string, Regex> _compiledSelectorRegexes = new Dictionary<string, Regex>();
        private static readonly Regex _whitespaceRegex = new Regex(@"\s+", RegexOptions.Singleline);
        private static readonly Regex _compactSelectorRegex = new Regex(@"\s*([>+~,=!^$():\[\]])\s*", RegexOptions.Singleline);
        //TODO: (nheskew) add filters and attributes
        private static readonly Regex _simpleSelectorRegex = new Regex(@"^(?<tag>\w+)?(?:(?:\.(?<class>[a-z][\w-_]*))|(?:#(?<id>[a-z][\w-_.:]*)))*$", RegexOptions.Singleline);
        private static readonly Regex _voidElementNameRegex = new Regex("^(?:br|img|input)$", RegexOptions.IgnoreCase);
        private static Func<string, IEnumerable<string>> GenerateSelectorFunction(string cssSelector)
        {
            if (!_compiledSelectorRegexes.ContainsKey(cssSelector))
            {
                lock(string.Intern(cssSelector))
                {
                    if (!_compiledSelectorRegexes.ContainsKey(cssSelector))
                    {
                        StringBuilder pattern = new StringBuilder();

                        string[] selectors = _compactSelectorRegex.Replace(cssSelector.Trim(), "$1").Split(',');
                        foreach (string selector in selectors)
                        {
                            //TODO: (nheskew) other hierarchy selectors (>, + and ~)
                            if (selector.Contains(">") || selector.Contains("+") || selector.Contains("~"))
                                throw new InvalidOperationException("Child and sibling selectors are not yet implemented :|");

                            string selectorPattern = constructAncestorDescendantPattern(selector);

                            pattern.AppendFormat("{1}(?:{0})", selectorPattern, pattern.Length > 0 ? "|" : String.Empty);
                        }

                        _compiledSelectorRegexes.Add(
                            cssSelector,
                            new Regex(pattern.ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline)
                            );
                    }
                }
            }

            return doc => FindElements(doc, _compiledSelectorRegexes[cssSelector]);
        }

        private static string constructAncestorDescendantPattern(string selector)
        {
            StringBuilder ancestorDescendantPattern = new StringBuilder();

            string[] selectorParts = _whitespaceRegex.Split(selector);
            foreach (string selectorPart in selectorParts)
                ancestorDescendantPattern.AppendFormat(
                    selectorPart == selectorParts.First()
                        ? "(?<={0}"
                        : ".*?{0}",
                    selectorPart == selectorParts.Last()
                        ? string.Format(")(?<element>{0})", ConstructNodePatternFormat(selectorPart))
                        : ConstructNodePatternFormat(selectorPart)
                    );

            return ancestorDescendantPattern.ToString();
        }

        //TODO: (nheskew) hook up filters and attributes
        private static string ConstructNodePatternFormat(string selector)
        {
            StringBuilder nodePatternFormat = new StringBuilder("<");
            string nodeName = @"\w+";

            MatchCollection selectorPartMatches = _simpleSelectorRegex.Matches(selector);
            foreach (Match selectorPartMatch in selectorPartMatches)
            {
                if (selectorPartMatch.Groups["tag"].Success)
                    nodeName = selectorPartMatch.Groups["tag"].Value;

                nodePatternFormat.Append(nodeName);

                if (selectorPartMatch.Groups["id"].Success)
                    nodePatternFormat.Append(ConstructNodeIDPattern(selectorPartMatch.Groups["id"].Captures));

                if (selectorPartMatch.Groups["class"].Success)
                    nodePatternFormat.Append(ConstructNodeClassPattern(selectorPartMatch.Groups["class"].Captures));
            }

            nodePatternFormat.Append("[^>]*/?>");

            return nodePatternFormat.ToString();
        }

        private static string ConstructNodeIDPattern(CaptureCollection idCaptureCollection)
        {
            return string.Format("\\s+id=[\"'](?:{0})[\"']", idCaptureCollection[0].Value);
        }

        //TODO: (nheskew) look for multiple class names on the element and deal with the edges of the class name a _lot_ better
        private static string ConstructNodeClassPattern(CaptureCollection classNameCaptureCollection)
        {
            return string.Format("\\s+class=[\"'](?:[^\"']*\\s+)?{0}(?:\\s+[^\"']*)?[\"']", classNameCaptureCollection[0].Value);
        }

        private static IEnumerable<string> FindElements(string doc, Regex selectorRegex)
        {
            List<string> elements = new List<string>(5);

            MatchCollection matches = selectorRegex.Matches(doc);

            foreach (Match match in matches)
                if (match.Groups["element"].Success)
                    foreach (Group group in match.Groups["element"].Captures)
                        elements.Add(
                            group.Value.EndsWith("/>") || _voidElementNameRegex.IsMatch(group.Value)
                                ? group.Value
                                : FindElementWithContents(doc, group)
                            );

            return elements;
        }

        private static readonly Regex _findElementRegex = new Regex(@"(</?(?<tag>\w+)[^>]*/?>)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static string FindElementWithContents(string doc, Group startTagGroup)
        {
            string elementStartToDocEnd = doc.Substring(startTagGroup.Index);

            int tagCount = 0;
            int endClosingTagIndex = 0;
            string tagName = String.Empty;

            MatchCollection tagMatches = _findElementRegex.Matches(elementStartToDocEnd);
            foreach (Match tagMatch in tagMatches)
            {
                string tag = tagMatch.Value;
                if (tagMatch == tagMatches[0])
                    tagName = tagMatch.Groups["tag"].Value;

                if (tagName == tagMatch.Groups["tag"].Value && !tag.EndsWith("/>"))
                {
                    if (tag.StartsWith("</"))
                        --tagCount;
                    else
                        ++tagCount;
                }

                endClosingTagIndex = tagMatch.Index + tagMatch.Length;

                if (tagCount == 0)
                    break;
            }

            return elementStartToDocEnd.Substring(0, endClosingTagIndex);
        }
    }
}
