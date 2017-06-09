using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Silicups.Core
{
    public abstract class Template
    {
        protected string templateTag;

        private StreamWriter outputWriter;
        private Dictionary<string, List<string>> codeLines;

        public Template(string templateTag)
        {
            this.templateTag = templateTag;
        }

        public void GenerateOutputStream(Stream templateStream, Stream outputStream)
        {
            this.outputWriter = new StreamWriter(outputStream);

            templateStream.Seek(0, System.IO.SeekOrigin.Begin);
            var templateReader = new StreamReader(templateStream);

            TemplateEngine.ParseTemplate(
                templateReader,
                outputWriter,
                templateTag,
                SectionArgumentsGetter,
                (codeLines) => { this.codeLines = codeLines; GenerateContent(); }
            );

            outputWriter.Flush();
            outputWriter.Dispose();
            outputWriter = null;
        }

        protected void WriteSection(string sectionName, params object[] args)
        {
            TemplateEngine.WriteSection(outputWriter, codeLines, sectionName, args);
        }

        protected abstract IEnumerable<string> SectionArgumentsGetter(string sectionName);
        protected abstract void GenerateContent();
    }

    public static class TemplateEngine
    {
        public static void ParseTemplate(StreamReader templateReader, StreamWriter outputWriter, string templateTag,
            Func<string, IEnumerable<string>> sectionArgumentsGetter,
            Action<Dictionary<string, List<string>>> codeProcessor)
        {
            string beginTag = "BEGIN:" + templateTag;
            string endTag = "END:" + templateTag;
            string sectionTag = templateTag + ":";

            string line;
            bool inCode = false;
            string sectionName = "BEGIN";
            Dictionary<string, List<string>> codeLines = null;
            while ((line = templateReader.ReadLine()) != null)
            {
                if (inCode)
                {
                    if (line.Contains(endTag))
                    {
                        codeProcessor(codeLines);
                        codeLines = null;
                        inCode = false;
                    }
                    else if (line.Contains(sectionTag))
                    {
                        string part = line.Substring(line.IndexOf(sectionTag) + sectionTag.Length);
                        int spaceIndex = part.IndexOf(" ");
                        if (spaceIndex == 0)
                        { continue; }
                        else if (spaceIndex > 0)
                        { part = part.Substring(0, spaceIndex); }
                        sectionName = part;
                    }
                    else
                    {
                        // line parsing: {argName} -> {0}
                        // 1. {argName} -> \a0\b; \a and \b are unlike characters in any template source
                        int i = 0;
                        string parsedLine = line;
                        foreach (string key in sectionArgumentsGetter(sectionName))
                        { parsedLine = parsedLine.Replace('{' + key + '}', '\a' + (i++).ToString() + '\b'); }
                        // 2. other { and } -> {{ and }}
                        parsedLine = parsedLine.Replace("{", "{{").Replace("}", "}}");
                        // 3. \a -> {, \b -> }
                        parsedLine = parsedLine.Replace('\a', '{').Replace('\b', '}');
                        codeLines.GetOrAdd(sectionName, (key) => new List<string>()).Add(parsedLine);
                    }
                }
                else
                {
                    if (line.Contains(beginTag))
                    {
                        if (line.Contains(endTag))
                        { continue; }
                        codeLines = new Dictionary<string, List<string>>();
                        inCode = true;
                    }
                    else
                    { outputWriter.WriteLine(line); }
                }
            }
        }

        private static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            { return value; }
            value = valueFactory(key);
            dictionary.Add(key, value);
            return value;
        }

        public static void WriteSection(StreamWriter writer, Dictionary<string, List<string>> codeLines, string sectionName, params object[] args)
        {
            if (codeLines.ContainsKey(sectionName))
            {
                foreach (string line in codeLines[sectionName])
                {
                    writer.WriteLine(String.Format(line, args));
                }
            }
        }
    }
}
