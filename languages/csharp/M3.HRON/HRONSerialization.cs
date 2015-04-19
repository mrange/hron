// ----------------------------------------------------------------------------------------------
// Copyright (c) Mårten Rånge.
// ----------------------------------------------------------------------------------------------
// This source code is subject to terms and conditions of the Microsoft Public License. A 
// copy of the license can be found in the License.html file at the root of this distribution. 
// If you cannot locate the  Microsoft Public License, please send an email to 
// dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
//  by the terms of the Microsoft Public License.
// ----------------------------------------------------------------------------------------------
// You must not remove this notice, or any other, from this software.
// ----------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

namespace M3.HRON
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Globalization;
    using System.Text;

    using M3.HRON.Generator.Parser;
    using M3.HRON.Generator.Source.Common;
    using M3.HRON.Generator.Source.Extensions;

    public partial interface IHRONVisitor
    {
        void Document_Begin();
        void Document_End();

        void PreProcessor(string baseString, int beginIndex, int endIndex);

        void Empty(string baseString, int beginIndex, int endIndex);

        void Comment(int indent, string baseString, int beginIndex, int endIndex);

        void Value_Begin(string baseString, int beginIndex, int endIndex);
        void Value_Line(string baseString, int beginIndex, int endIndex);
        void Value_End();

        void Object_Begin(string baseString, int beginIndex, int endIndex);
        void Object_End();

        void Error(int lineNo, string parseError, string baseString, int beginIndex, int endIndex);
    }

    public sealed partial class HRONWritingVisitor : IHRONVisitor
    {
        int m_indent;
        public int ErrorCount;
        public readonly StringBuilder StringBuilder = new StringBuilder();

        public void Document_Begin()
        {
        }

        public void Document_End()
        {
        }

        void Append(int indent, char tag, string baseString, int beginIndex, int endIndex)
        {
            StringBuilder.Append('\t', indent);
            StringBuilder.Append(tag);
            StringBuilder.Append(baseString, beginIndex, endIndex - beginIndex);
            StringBuilder.AppendLine();
        }

        public void PreProcessor(string baseString, int beginIndex, int endIndex)
        {
          Append(m_indent, '!', baseString, beginIndex, endIndex);
        }

        public void Empty(string baseString, int beginIndex, int endIndex)
        {
            StringBuilder.Append(baseString, beginIndex, endIndex - beginIndex);
            StringBuilder.AppendLine();
        }

        public void Comment(int indent, string baseString, int beginIndex, int endIndex)
        {
            Append(m_indent, '#', baseString, beginIndex, endIndex);
        }

        public void Value_Begin(string baseString, int beginIndex, int endIndex)
        {
            Append(m_indent, '=', baseString, beginIndex, endIndex);
            ++m_indent;
        }

        public void Value_Line(string baseString, int beginIndex, int endIndex)
        {
            StringBuilder.Append('\t', m_indent);
            StringBuilder.Append(baseString, beginIndex, endIndex - beginIndex);
            StringBuilder.AppendLine();
        }

        public void Value_End()
        {
            --m_indent;
        }

        public void Object_Begin(string baseString, int beginIndex, int endIndex)
        {
            Append(m_indent, '@', baseString, beginIndex, endIndex);
            ++m_indent;
        }

        public void Object_End()
        {
            --m_indent;
        }

        public void Error(int lineNo, string parseError, string baseString, int beginIndex, int endIndex)
        {
            StringBuilder.AppendFormat("# ERROR - {0}({1}) : {2}", parseError, lineNo, baseString.Substring (beginIndex, endIndex - beginIndex));
            StringBuilder.AppendLine();
            ++ErrorCount;
        }
    }

    public static partial class HRONSerialization
    {
        static void SerializeRecursiveDictionaryImpl(IEnumerable<KeyValuePair<string, object>> dictionary, HRONWritingVisitor visitor)
        {
            foreach (var kv in dictionary)
            {
                var key = kv.Key;
                var innerDictionary = kv.Value as IEnumerable<KeyValuePair<string, object>>;
                if (innerDictionary != null)
                {
                    visitor.Object_Begin(key, 0, key.Length);
                    SerializeRecursiveDictionaryImpl(
                        innerDictionary,
                        visitor
                        );
                    visitor.Object_End();
                }
                else
                {
                    visitor.Value_Begin(key, 0, key.Length);
                    var value = kv.Value;
                    if (value != null)
                    {
                        var formattable = value as IFormattable;
                        var valueAsString = formattable != null 
                            ? formattable.ToString("", CultureInfo.InvariantCulture) 
                            : value.ToString()
                            ;
                        var lines = valueAsString.ReadLines();
                        foreach (var line in lines)
                        {
                            visitor.Value_Line(line.BaseString, line.Begin, line.Length);
                        }
                    }
                    visitor.Value_End();
                }
            }
        }

        public static string SerializeKeyValuePairs(IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            if (keyValuePairs == null)
            {
                return "";
            }
            var visitor = new HRONWritingVisitor();

            SerializeRecursiveDictionaryImpl(keyValuePairs, visitor);

            return visitor.StringBuilder.ToString();
        }

        static bool TryParse<T>(T input, IHRONVisitor visitor, Action<T, Scanner> action)
        {
            if (visitor == null)
            {
                return false;
            }

            Parse(input, visitor, action);

            // TODO:
            return true;
        }

        static void Parse<T>(T input, IHRONVisitor visitor, Action<T, Scanner> action)
        {
            visitor.Document_Begin();

            try
            {
                var scanner = new Scanner(visitor);

                action(input, scanner);

                scanner.AcceptEndOfStream();
            }
            finally
            {
                visitor.Document_End();
            }
        }

        public static bool TryParse(string input, IHRONVisitor visitor)
        {
            return TryParse(
                input,
                visitor,
                (i,s) =>
                    {
                        foreach (var line in i.ReadLines())
                        {
                            s.AcceptLine(line.BaseString, line.Begin, line.End);
                        }
                    });
        }

        public static bool TryParse(IEnumerable<string> input, IHRONVisitor visitor)
        {
            return TryParse(
                input,
                visitor,
                (i, s) =>
                {
                        i = i ?? Array<string>.Empty;
                        foreach (var line in i)
                        {
                            s.AcceptLine(line, 0, line.Length);
                        }
                    });
        }

        public static bool TryParse(TextReader textReader, IHRONVisitor visitor)
        {
            return TryParse(
                textReader.ReadLines(), 
                visitor
                );
        }
    }
}
