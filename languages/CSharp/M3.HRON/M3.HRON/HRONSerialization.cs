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

using System;
using System.Globalization;

namespace M3.HRON
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using Source.Common;
    using Source.Extensions;
    using Source.HRON;

    public static partial class HRONSerialization
    {
        public partial interface IVisitor
        {
            void Empty(string line);

            void Comment(int indent, string comment);

            void Value_Begin(string name);
            void Value_Line(string value);
            void Value_End(string name);

            void Object_Begin(string name);
            void Object_End(string name);

            void Error(int lineNo, string line, string parseError);
        }

        sealed partial class TranslatingVisitor : IHRONVisitor
        {
            public readonly IVisitor Visitor;
            public int ErrorCount;

            public TranslatingVisitor(IVisitor visitor)
            {
                Visitor = visitor;
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

            public void Value_End(SubString name)
            {
                Visitor.Value_End(name.Value);
            }

            public void Object_Begin(SubString name)
            {
                Visitor.Object_Begin(name.Value);
            }

            public void Object_End(SubString name)
            {
                Visitor.Object_End(name.Value);
            }

            public void Error(int lineNo, SubString line, HRONSerializer.ParseError parseError)
            {
                Visitor.Error(lineNo, line.Value, parseError.ToString());
                ++ErrorCount;
            }
        }

        static void SerializeRecursiveDictionaryImpl(IEnumerable<KeyValuePair<string, object>> dictionary, HRONWriterVisitor visitor)
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
                    visitor.Object_End(key);
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
                    visitor.Value_End(key);
                }
            }
        }

        public static string SerializeKeyValuePairs(IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            if (keyValuePairs == null)
            {
                return "";
            }
            var visitor = new HRONWriterVisitor();

            SerializeRecursiveDictionaryImpl(keyValuePairs, visitor);

            return visitor.Value;
        }

        public static bool TryParse(string input, IVisitor visitor)
        {
            if (visitor == null)
            {
                return false;
            }

            var translatingVisitor = new TranslatingVisitor(visitor);
            HRONSerializer.Parse(
                int.MaxValue,
                input.ReadLines(),
                translatingVisitor
                );
            return translatingVisitor.ErrorCount == 0;
        }

        public static bool TryParse(IEnumerable<string> input, IVisitor visitor)
        {
            if (visitor == null)
            {
                return false;
            }

            var translatingVisitor = new TranslatingVisitor(visitor);
            HRONSerializer.Parse(
                int.MaxValue,
                (input ?? Array<string>.Empty).Select(s => s.ToSubString()),
                translatingVisitor
                );
            return translatingVisitor.ErrorCount == 0;
        }

        public static bool TryParse(TextReader textReader, IVisitor visitor)
        {
            if (visitor == null)
            {
                return false;
            }

            var translatingVisitor = new TranslatingVisitor(visitor);
            HRONSerializer.Parse(
                int.MaxValue,
                textReader.ReadLines().Select(s => s.ToSubString()),
                translatingVisitor
                );
            return translatingVisitor.ErrorCount == 0;
        }

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
    }
}
