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

using System.Text;
using M3.HRON.Generator.Parser;
using M3.HRON.Generator.Source.Common;

namespace M3.HRON
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Globalization;
    using System.Linq;

    public partial interface IVisitor
    {
        void Document_Begin();
        void Document_End();

        void PreProcessor(string line);

        void Empty(string line);

        void Comment(int indent, string comment);

        void Value_Begin(string name);
        void Value_Line(string value);
        void Value_End();

        void Object_Begin(string name);
        void Object_End();

        void Error(int lineNo, string line, string parseError);
    }

    sealed partial class TranslatingVisitor : M3.HRON.Generator.Parser.IVisitor
    {
        public readonly IVisitor Visitor;
        public int ErrorCount;

        public TranslatingVisitor(IVisitor visitor)
        {
            Visitor = visitor;
        }

        public void Document_Begin()
        {
            Visitor.Document_Begin();
        }

        public void Document_End()
        {
            Visitor.Document_End();
        }

        public void PreProcessor(SubString line)
        {
            Visitor.PreProcessor(line.Value);
        }

        public void Empty(SubString line)
        {
            Visitor.Empty(line.Value);
        }

        public void Comment(int indent, SubString comment)
        {
            Visitor.Comment(indent, comment.Value);
        }

        public void Value_Begin(SubString name)
        {
            Visitor.Value_Begin(name.Value);
        }

        public void Value_Line(SubString value)
        {
            Visitor.Value_Line(value.Value);
        }

        public void Value_End()
        {
            Visitor.Value_End();
        }

        public void Object_Begin(SubString name)
        {
            Visitor.Object_Begin(name.Value);
        }

        public void Object_End()
        {
            Visitor.Object_End();
        }

        public void Error(int lineNo, SubString line, Scanner.Error parseError)
        {
            Visitor.Error(lineNo, line.Value, parseError.ToString());
            ++ErrorCount;
        }
    }

    sealed partial class WritingVisitor : M3.HRON.Generator.Parser.IVisitor
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

        public void PreProcessor(SubString line)
        {
            StringBuilder.Append('!');
            StringBuilder.AppendSubString(line);
            StringBuilder.AppendLine();
        }

        public void Empty(SubString line)
        {
            StringBuilder.AppendSubString(line);
            StringBuilder.AppendLine();
        }

        public void Comment(int indent, SubString comment)
        {
            StringBuilder.Append('\t', indent);
            StringBuilder.Append('"');
            StringBuilder.AppendSubString(comment);
            StringBuilder.AppendLine();
        }

        public void Value_Begin(SubString name)
        {
            StringBuilder.Append('\t', m_indent);
            StringBuilder.Append('=');
            StringBuilder.AppendSubString(name);
            StringBuilder.AppendLine();
            ++m_indent;
        }

        public void Value_Line(SubString value)
        {
            StringBuilder.Append('\t', m_indent);
            StringBuilder.AppendSubString(value);
            StringBuilder.AppendLine();
        }

        public void Value_End()
        {
            --m_indent;
        }

        public void Object_Begin(SubString name)
        {
            StringBuilder.Append('\t', m_indent);
            StringBuilder.Append('@');
            StringBuilder.AppendSubString(name);
            StringBuilder.AppendLine();
            ++m_indent;
        }

        public void Object_End()
        {
            --m_indent;
        }

        public void Error(int lineNo, SubString line, Scanner.Error parseError)
        {
            StringBuilder.AppendFormat("# ERROR - {0}({1}) : {2}", parseError, lineNo, line);
            StringBuilder.AppendLine();
            ++ErrorCount;
        }
    }

    public static partial class HRONSerialization
    {
        static void SerializeRecursiveDictionaryImpl(IEnumerable<KeyValuePair<string, object>> dictionary, WritingVisitor visitor)
        {
            foreach (var kv in dictionary)
            {
                var key = kv.Key.ToSubString();
                var innerDictionary = kv.Value as IEnumerable<KeyValuePair<string, object>>;
                if (innerDictionary != null)
                {
                    visitor.Object_Begin(key);
                    SerializeRecursiveDictionaryImpl(
                        innerDictionary,
                        visitor
                        );
                    visitor.Object_End();
                }
                else
                {
                    visitor.Value_Begin(key);
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
                            visitor.Value_Line(line);
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
            var visitor = new WritingVisitor();

            SerializeRecursiveDictionaryImpl(keyValuePairs, visitor);

            return visitor.StringBuilder.ToString();
        }

        static bool TryParse<T>(T input, IVisitor visitor, Action<T, Scanner> action)
        {
            if (visitor == null)
            {
                return false;
            }

            var translatingVisitor = new TranslatingVisitor(visitor);

            translatingVisitor.Document_Begin();

            try
            {
                var scanner = new Scanner(translatingVisitor);

                action(input, scanner);

                scanner.AcceptEndOfStream();
            }
            finally
            {
                translatingVisitor.Document_End();
            }

            return translatingVisitor.ErrorCount == 0;
            
        }

        public static bool TryParse(string input, IVisitor visitor)
        {
            return TryParse(
                input,
                visitor,
                (i,s) =>
                    {
                        foreach (var line in i.ReadLines())
                        {
                            s.AcceptLine(line);
                        }
                    });
        }

        public static bool TryParse(IEnumerable<string> input, IVisitor visitor)
        {
            return TryParse(
                input,
                visitor,
                (i, s) =>
                {
                        i = i ?? Array<string>.Empty;
                        foreach (var line in i)
                        {
                            s.AcceptLine(line.ToSubString());
                        }
                    });
        }

        public static bool TryParse(TextReader textReader, IVisitor visitor)
        {
            return TryParse(
                textReader,
                visitor,
                (i, s) =>
                    {
                        string line;
                        while ((line = textReader.ReadLine()) != null)
                        {
                            s.AcceptLine(line.ToSubString());
                        }
                    });
        }
/*
        public static bool TryParseAsDynamic(string input, out object dynamicValue)
        {
            HRONDynamicParseError[] errors;
            return HRONSerializer.TryParseDynamic(
                int.MaxValue,
                input.ReadLines(),
                out dynamicValue,
                out errors
                );
        }

        public static bool TryParseAsDynamic(IEnumerable<string> input, out object dynamicValue)
        {
            HRONDynamicParseError[] errors;
            return HRONSerializer.TryParseDynamic(
                int.MaxValue,
                (input ?? Array<string>.Empty).Select(s => s.ToSubString()),
                out dynamicValue,
                out errors
                );
        }

        public static bool TryParseAsDynamic(TextReader textReader, out object dynamicValue)
        {
            HRONDynamicParseError[] errors;
            return HRONSerializer.TryParseDynamic(
                int.MaxValue,
                textReader.ReadLines().Select(s => s.ToSubString()),
                out dynamicValue,
                out errors
                );
        }
*/
    }
}
