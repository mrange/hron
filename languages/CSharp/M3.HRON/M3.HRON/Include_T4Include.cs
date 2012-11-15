
// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # text template file (.tt)                                                      #
// ############################################################################



// ############################################################################
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Hron/HRONSerializer.cs
// @@@ INCLUDE_FOUND: ../Common/Array.cs
// @@@ INCLUDE_FOUND: ../Common/Config.cs
// @@@ INCLUDE_FOUND: ../Common/SubString.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Hron/HRONDynamicObjectSerializer.cs
// @@@ INCLUDE_FOUND: HRONSerializer.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/Array.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/Config.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/SubString.cs
// @@@ SKIPPING (Already seen): https://raw.github.com/mrange/T4Include/master/Hron/HRONSerializer.cs
// ############################################################################
// Certains directives such as #define and // Resharper comments has to be 
// moved to top in order to work properly    
// ############################################################################
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantCaseLabel
// ############################################################################

// ############################################################################
namespace M3.HRON
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    
    
    namespace Source.HRON
    {
        using System.Collections.Generic;
        using System.Text;
        using Source.Common;
    
        partial interface IHRONVisitor
        {
            void Empty (SubString line);
    
            void Comment(int indent, SubString comment);
    
            void Value_Begin(SubString name);
            void Value_Line(SubString value);
            void Value_End(SubString name);
    
            void Object_Begin(SubString name);
            void Object_End(SubString name);
    
            void Error(int lineNo, SubString line, HRONSerializer.ParseError parseError);
        }
    
        abstract partial class BaseHRONWriterVisitor : IHRONVisitor
        {
            readonly StringBuilder m_sb = new StringBuilder();
            int m_indent;
            
            protected abstract void WriteLine (string line);
            protected abstract void WriteLine (StringBuilder line);
    
            public void Empty (SubString line)
            {
                m_sb.Clear();
                m_sb.Append(line);
                WriteLine(m_sb);
            }
    
            public void Comment(int indent, SubString comment)
            {
                m_sb.Clear();
                m_sb.Append('\t', indent);
                m_sb.Append('#');
                m_sb.Append(comment);
                WriteLine(m_sb);
            }
    
            public void Value_Begin(SubString name)
            {
                m_sb.Clear();
                m_sb.Append('\t', m_indent);
                m_sb.Append('=');
                m_sb.Append(name);
                ++m_indent;
                WriteLine(m_sb);
            }
    
            public void Value_Line(SubString value)
            {
                m_sb.Clear();
                m_sb.Append('\t', m_indent);
                m_sb.Append(value);
                WriteLine(m_sb);
            }
    
            public void Value_End(SubString name)
            {
                --m_indent;
            }
    
            public void Object_Begin(SubString name)
            {
                m_sb.Clear();
                m_sb.Append('\t', m_indent);
                m_sb.Append('@');
                m_sb.Append(name);
                WriteLine(m_sb);
                ++m_indent;
            }
    
            public void Object_End(SubString name)
            {
                --m_indent;
            }
    
            public void Error(int lineNo, SubString line, HRONSerializer.ParseError parseError)
            {
                m_sb.Clear();
                m_sb.AppendFormat(Config.DefaultCulture, "# Error at line {0}: {1}", lineNo, parseError);
                WriteLine(m_sb);
            }
    
        }
    
        sealed partial class HRONWriterVisitor : BaseHRONWriterVisitor
        {
            readonly StringBuilder m_sb = new StringBuilder(128);
    
            public string Value
            {
                get
                {
                    return m_sb.ToString();                
                }
            }
    
            protected override void WriteLine(string line)
            {
                m_sb.AppendLine(line);
            }
    
            protected override void WriteLine(StringBuilder line)
            {
                var count = line.Length;
                for (var iter = 0; iter < count; ++iter)
                {
                    m_sb.Append(line[iter]);
                }
                m_sb.AppendLine();
            }
        }
    
        static partial class HRONSerializer
        {
            enum ParseState
            {
                ExpectingTag    ,
                ExpectingValue  ,
            }
    
            public enum ParseError
            {
                IndentIncreasedMoreThanExpected ,
                TagIsNotCorrectlyFormatted      ,
            }
    
            public static void Parse (
                int maxErrorCount,
                IEnumerable<SubString> lines,
                IHRONVisitor visitor
                )
            {
                if (visitor == null)
                {
                    return;
                }
    
                var errorCount = 0;
    
                lines = lines ?? Array<SubString>.Empty;
    
                var state = ParseState.ExpectingTag;
                var expectedIndent = 0;
                var lineNo = 0;
                var context = new Stack<SubString>();
    
                foreach (var line in lines)
                {
                    ++lineNo;
    
                    var currentIndent = 0;
                    var lineLength = line.Length;
    
                    for (var iter = 0; iter < lineLength; ++iter)
                    {
                        var ch = line[iter];
                        if (ch == '\t')
                        {
                            ++currentIndent;
                        }
                        else
                        {
                            iter = lineLength;
                        }
                    }
    
                    if (currentIndent > expectedIndent)
                    {
                        visitor.Error(lineNo, line, ParseError.IndentIncreasedMoreThanExpected);
                        if (++errorCount > 0)
                        {
                            return;
                        }
                        continue;
                    }
    
                    bool isComment;
                    switch (state)
                    {
                        case ParseState.ExpectingTag:
                            isComment = currentIndent < lineLength
                                && line[currentIndent] == '#'
                                ;
                            break;
                        case ParseState.ExpectingValue:
                        default:
                            isComment = currentIndent < expectedIndent
                                && currentIndent < lineLength
                                && line[currentIndent] == '#'
                                ;
                            break;
                    }
    
                    var isWhiteSpace = line.ToSubString(currentIndent).IsWhiteSpace;
    
                    if (isComment)
                    {
                        visitor.Comment(currentIndent, line.ToSubString(currentIndent + 1));
                    }
                    else if (isWhiteSpace && currentIndent < expectedIndent)
                    {
                        switch (state)
                        {
                            case ParseState.ExpectingValue:
                                visitor.Value_Line(SubString.Empty);
                                break;
                            case ParseState.ExpectingTag:
                            default:
                                visitor.Empty(line);
                                break;
                        }
                    }
                    else if (isWhiteSpace)
                    {
                        switch (state)
                        {
                            case ParseState.ExpectingValue:
                                visitor.Value_Line(line.ToSubString(expectedIndent));
                                break;
                            case ParseState.ExpectingTag:
                            default:
                                visitor.Empty(line);
                                break;
                        }
                    }
                    else
                    {
                        if (currentIndent < expectedIndent)
                        {
                            switch (state)
                            {
                                case ParseState.ExpectingTag:
                                    for (var iter = currentIndent; iter < expectedIndent; ++iter)
                                    {
                                        visitor.Object_End(context.Peek());
                                        context.Pop();
                                    }
                                    break;
                                case ParseState.ExpectingValue:
                                default:
                                    visitor.Value_End(context.Peek());
                                    // Popping the value name
                                    context.Pop();
                                    for (var iter = currentIndent + 1; iter < expectedIndent; ++iter)
                                    {
                                        visitor.Object_End(context.Peek());
                                        context.Pop();
                                    }
                                    break;
                            }
    
                            expectedIndent = currentIndent;
                            state = ParseState.ExpectingTag;
                        }
    
                        switch (state)
                        {
                            case ParseState.ExpectingTag:
                                if (currentIndent < lineLength)
                                {
                                    var first = line[currentIndent];
                                    switch (first)
                                    {
                                        case '@':
                                            state = ParseState.ExpectingTag;
                                            ++expectedIndent;
                                            context.Push(line.ToSubString(currentIndent + 1));
                                            visitor.Object_Begin(context.Peek());
                                            break;
                                        case '=':
                                            state = ParseState.ExpectingValue;
                                            ++expectedIndent;
                                            context.Push(line.ToSubString(currentIndent + 1));
                                            visitor.Value_Begin(context.Peek());
                                            break;
                                        default:
                                            visitor.Error(lineNo, line, ParseError.TagIsNotCorrectlyFormatted);
                                            if (++errorCount > 0)
                                            {
                                                return;
                                            }
                                            break;
                                    }
                                }
                                break;
                            case ParseState.ExpectingValue:
                                visitor.Value_Line(line.ToSubString(expectedIndent));
                                break;
                        }
                    }
                }
    
                switch (state)
                {
                    case ParseState.ExpectingTag:
                        for (var iter = 0; iter < expectedIndent; ++iter)
                        {
                            visitor.Object_End(context.Peek());
                            context.Pop();
                        }
                        break;
                    case ParseState.ExpectingValue:
                    default:
                        visitor.Value_End(context.Peek());
                        // Popping the value name
                        context.Pop();
                        for (var iter = 0 + 1; iter < expectedIndent; ++iter)
                        {
                            visitor.Object_End(context.Peek());
                            context.Pop();
                        }
                        break;
                }
    
            }
    
        }
    
    }
}

// ############################################################################
namespace M3.HRON
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    
    
    
    namespace Source.HRON
    {
        using System.Collections.Generic;
        using System.Dynamic;
        using System.Linq;
        using System.Text;
    
        using Source.Common;
    
        static partial class HronExtensions
        {
            public static IHRONEntity FirstOrEmpty (this IEnumerable<IHRONEntity> entities)
            {
                if (entities == null)
                {
                    return HRONValue.Empty;
                }
    
                return entities.FirstOrDefault() ?? HRONValue.Empty;
            }
        }
    
        partial class HRONDynamicParseError
        {
            public readonly int LineNo;
            public readonly string Line;
            public readonly HRONSerializer.ParseError ParseError;
    
            public HRONDynamicParseError(int lineNo, string line, HRONSerializer.ParseError parseError)
            {
                LineNo = lineNo;
                Line = line;
                ParseError = parseError;
            }
        }
    
        partial interface IHRONEntity
        {
            HRONSerializer.DynamicType GetDynamicType();
    
            IEnumerable<string>      GetMemberNames();
            IEnumerable<IHRONEntity> GetMember(string name);
            string GetValue();
    
            void Apply(SubString name, IHRONVisitor visitor);
            void ToString(StringBuilder sb);
        }
    
        sealed partial class HRONDynamicMembers : DynamicObject
        {
            readonly IHRONEntity[] m_entities;
    
            public HRONDynamicMembers(IEnumerable<IHRONEntity> entities)
            {
                m_entities = (entities ?? Array<IHRONEntity>.Empty).ToArray ();
            }
    
            public int GetCount ()
            {
                return m_entities.Length;
            }
    
            public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
            {
                if (indexes.Length == 1 && indexes[0] is int)
                {
                    var index = (int)indexes[0];
                    if (index < 0)
                    {
                        result = HRONValue.Empty;
                        return true;
                    }
    
                    if (index >= m_entities.Length)
                    {
                        result = HRONValue.Empty;
                        return true;
                    }
    
                    result = m_entities[index];
                    return true;
                }
    
                return base.TryGetIndex(binder, indexes, out result);
            }
    
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var entity = m_entities.FirstOrEmpty();
    
                var dynamicObject = entity as DynamicObject;
                if (dynamicObject != null)
                {
                    return dynamicObject.TryGetMember(binder, out result);
                }
    
                return base.TryGetMember(binder, out result);
            }
    
            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                if (binder.ReturnType == typeof(string))
                {
                    result = m_entities.FirstOrEmpty().GetValue ();
                    return true;
                }
                else if (binder.ReturnType == typeof(string[]))
                {
                    result = m_entities.Select(e => e.GetValue()).ToArray();
                    return true;
                }
                else if (binder.ReturnType ==typeof(object[]))
                {
                    result = m_entities;
                    return true;
                }
                return base.TryConvert(binder, out result);
            }
        }
    
        abstract partial class BaseHRONEntity : DynamicObject, IHRONEntity
        {
            public abstract HRONSerializer.DynamicType GetDynamicType();
            public abstract IEnumerable<string> GetMemberNames();
            public abstract IEnumerable<IHRONEntity> GetMember(string name);
            public abstract string GetValue();
    
            public abstract void Apply(SubString name, IHRONVisitor visitor);
            public abstract void ToString(StringBuilder sb);
    
            public override string ToString()
            {
                var sb = new StringBuilder(128);
                ToString(sb);
                return sb.ToString();
            }
    
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = new HRONDynamicMembers(GetMember(binder.Name));
                return true;
            }
    
            public override bool TryConvert(ConvertBinder binder, out object result)
            {
                if (binder.ReturnType == typeof(string))
                {
                    result = GetValue();
                    return true;
                }
                return base.TryConvert(binder, out result);
            }
    
        }
    
        sealed partial class HRONObject : BaseHRONEntity
        {
            public partial struct Member
            {
                readonly string m_name;
                readonly IHRONEntity m_value;
    
                public Member(string name, IHRONEntity value)
                    : this()
                {
                    m_name = name;
                    m_value = value;
                }
    
                public string Name
                {
                    get { return m_name ?? ""; }
                }
    
                public IHRONEntity Value
                {
                    get { return m_value ?? HRONValue.Empty; }
                }
    
                public override string ToString()
                {
                    return Name + " : " + Value;
                }
            }
    
            ILookup<string, IHRONEntity> m_lookup;
            readonly Member[] m_members;
    
            public HRONObject(Member[] members)
            {
                m_members = members ?? Array<Member>.Empty;
            }
    
            ILookup<string, IHRONEntity> GetLookup()
            {
                if (m_lookup == null)
                {
                    m_lookup = m_members.ToLookup(p => p.Name, p => p.Value);
                }
    
                return m_lookup;
            }
    
            public override HRONSerializer.DynamicType GetDynamicType()
            {
                return HRONSerializer.DynamicType.Object;
            }
    
            public override IEnumerable<string> GetMemberNames()
            {
                for (int index = 0; index < m_members.Length; index++)
                {
                    var pair = m_members[index];
                    yield return pair.Name;
                }
            }
    
            public override IEnumerable<IHRONEntity> GetMember(string name)
            {
                return GetLookup()[name ?? ""]; 
            }
    
            public override string GetValue()
            {
                return "";
            }
    
            public void Visit (IHRONVisitor visitor)
            {
                if (visitor == null)
                {
                    return;
                }
    
                for (var index = 0; index < m_members.Length; index++)
                {
                    var pair = m_members[index];
                    var innerName = pair.Name.ToSubString();
                    pair.Value.Apply(innerName, visitor);
                }
            }
    
            public override void Apply(SubString name, IHRONVisitor visitor)
            {
                if (visitor == null)
                {
                    return;
                }
    
                visitor.Object_Begin(name);
                for (var index = 0; index < m_members.Length; index++)
                {
                    var pair = m_members[index];
                    var innerName = pair.Name.ToSubString();
                    pair.Value.Apply(innerName, visitor);
                }
                visitor.Object_End(name);
            }
    
            public override void ToString(StringBuilder sb)
            {
                sb.Append("{Object");
                for (var index = 0; index < m_members.Length; index++)
                {
                    var pair = m_members[index];
                    sb.Append(", '");
                    sb.Append(pair.Name);
                    sb.Append("' : ");
                    pair.Value.ToString(sb);
                }
                sb.Append('}');
            }
        }
    
        sealed partial class HRONValue : BaseHRONEntity
        {
            readonly string m_value;
            public static readonly IHRONEntity Empty = new HRONValue("");
    
            public HRONValue(string value)
            {
                m_value = value ?? "";
            }
    
            public override HRONSerializer.DynamicType GetDynamicType()
            {
                return HRONSerializer.DynamicType.Value;
            }
    
            public override IEnumerable<string> GetMemberNames()
            {
                return Array<string>.Empty;
            }
    
            public override IEnumerable<IHRONEntity> GetMember(string name)
            {
                return Array<IHRONEntity>.Empty;
            }
    
            public override string GetValue()
            {
                return m_value;
            }
    
            public override void Apply(SubString name, IHRONVisitor visitor)
            {
                if (visitor == null)
                {
                    return;
                }
    
                visitor.Value_Begin(name);
                foreach (var line in m_value.ReadLines())
                {
                    visitor.Value_Line(line);
                }
                visitor.Value_End(name);
            }
    
            public override void ToString(StringBuilder sb)
            {
                sb.Append('"');
                sb.Append(m_value);
                sb.Append('"');
            }
        }
    
        sealed partial class HRONDynamicBuilderVisitor : IHRONVisitor
        {
            public struct Item
            {
                public readonly SubString Name;
                public readonly List<HRONObject.Member> Pairs;
    
                public Item(SubString name)
                    : this()
                {
                    Name = name;
                    Pairs = new List<HRONObject.Member>();
                }
            }
    
            readonly Stack<Item> m_stack = new Stack<Item>();
            readonly StringBuilder m_value = new StringBuilder(128);
            bool m_firstLine = true;
    
            public readonly List<HRONDynamicParseError> Errors = new List<HRONDynamicParseError>();
            public readonly int MaxErrors;
    
            public HRONDynamicBuilderVisitor(int maxErrors)
            {
                MaxErrors = maxErrors;
                m_stack.Push(new Item("Root".ToSubString()));
            }
    
            public Item Top
            {
                get { return m_stack.Peek(); }
            }
    
            public void Empty(SubString line)
            {
            }
    
            public void Comment(int indent, SubString comment)
            {
            }
    
            public void Value_Begin(SubString name)
            {
                m_value.Clear();
                m_firstLine = true;
            }
    
            public void Value_Line(SubString value)
            {
                if (m_firstLine)
                {
                    m_firstLine = false;
                }
                else
                {
                    m_value.AppendLine();
                }
                m_value.Append(value);
            }
    
            public void Value_End(SubString name)
            {
                AddEntity(name.Value, new HRONValue(m_value.ToString()));
            }
    
            public void Object_Begin(SubString name)
            {
                m_stack.Push(new Item(name));
            }
    
            public void Object_End(SubString name)
            {
                var pop = m_stack.Pop();
                AddEntity(pop.Name.Value, new HRONObject(pop.Pairs.ToArray()));
            }
    
            void AddEntity(string name, IHRONEntity entity)
            {
                Top.Pairs.Add(new HRONObject.Member(name, entity));
            }
    
            public void Error(int lineNo, SubString line, HRONSerializer.ParseError parseError)
            {
                Errors.Add(new HRONDynamicParseError(lineNo, line.Value, parseError));
            }
        }
    
    
        static partial class HRONSerializer
        {
            public enum DynamicType
            {
                Value,
                Object,
            }
    
            public static void VisitDynamic (
                HRONObject hronObject,
                IHRONVisitor visitor
                )
            {
                if (hronObject == null)
                {
                    return;
                }
    
                hronObject.Visit(visitor);
            }
    
            public static string DynamicAsString (
                HRONObject hronObject
                )
            {
                if (hronObject == null)
                {
                    return "";
                }
    
                var v = new HRONWriterVisitor();
                VisitDynamic(hronObject, v);
                return v.Value;
            }
    
            public static bool TryParseDynamic(
                int maxErrorCount,
                IEnumerable<SubString> lines,
                out dynamic dyn,
                out HRONDynamicParseError[] errors
                )
            {
                HRONObject hronObject;
    
                if (!TryParseDynamic(maxErrorCount, lines, out hronObject, out errors))
                {
                    dyn = null;
                    return false;
                }
    
                dyn = hronObject;
                return true;
            }
    
            public static bool TryParseDynamic(
                int maxErrorCount,
                IEnumerable<SubString> lines,
                out HRONObject hronObject,
                out HRONDynamicParseError[] errors
                )
            {
                hronObject = null;
                errors = Array<HRONDynamicParseError>.Empty;
    
                var visitor = new HRONDynamicBuilderVisitor(maxErrorCount);
    
                Parse(maxErrorCount, lines, visitor);
    
                if (visitor.Errors.Count > 0)
                {
                    errors = visitor.Errors.ToArray();
                    return false;
                }
    
                hronObject = new HRONObject(visitor.Top.Pairs.ToArray());
    
                return true;
            }
    
        }
    }
}

// ############################################################################
namespace M3.HRON
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    namespace Source.Common
    {
        static class Array<T>
        {
            public static readonly T[] Empty = new T[0];
        }
    }
}

// ############################################################################
namespace M3.HRON
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    namespace Source.Common
    {
        using System.Globalization;
    
        sealed partial class InitConfig
        {
            public CultureInfo DefaultCulture = CultureInfo.InvariantCulture;
        }
    
        static partial class Config
        {
            static partial void Partial_Constructed(ref InitConfig initConfig);
    
            public readonly static CultureInfo DefaultCulture;
    
            static Config ()
            {
                var initConfig = new InitConfig();
    
                Partial_Constructed (ref initConfig);
    
                initConfig = initConfig ?? new InitConfig();
    
                DefaultCulture = initConfig.DefaultCulture;
            }
        }
    }
}

// ############################################################################
namespace M3.HRON
{
    // ----------------------------------------------------------------------------------------------
    // Copyright (c) M�rten R�nge.
    // ----------------------------------------------------------------------------------------------
    // This source code is subject to terms and conditions of the Microsoft Public License. A 
    // copy of the license can be found in the License.html file at the root of this distribution. 
    // If you cannot locate the  Microsoft Public License, please send an email to 
    // dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
    //  by the terms of the Microsoft Public License.
    // ----------------------------------------------------------------------------------------------
    // You must not remove this notice, or any other, from this software.
    // ----------------------------------------------------------------------------------------------
    
    
    namespace Source.Common
    {
        using System;
        using System.Collections;
        using System.Collections.Generic;
        using System.Text;
    
    
        static class SubStringExtensions
        {
            public static void Append (this StringBuilder sb, SubString ss)
            {
                sb.Append(ss.BaseString, ss.Begin, ss.Length);
            }
    
            public static void AppendLine(this StringBuilder sb, SubString ss)
            {
                sb.Append(ss.BaseString, ss.Begin, ss.Length);
                sb.AppendLine();
            }
    
            public static SubString ToSubString (this string value, int begin = 0, int count = int.MaxValue / 2)
            {
                return new SubString(value, begin, count);
            }
    
            public static SubString ToSubString(this StringBuilder value, int begin = 0, int count = int.MaxValue / 2)
            {
                return new SubString(value.ToString(), begin, count);
            }
    
            public static SubString ToSubString(this SubString value, int begin = 0, int count = int.MaxValue / 2)
            {
                return new SubString(value, begin, count);
            }
    
            enum ParseLineState
            {
                NewLine     ,
                Inline      ,
                ConsumedCR  ,
            }
    
            public static IEnumerable<SubString> ReadLines(this string value)
            {
                return value.ToSubString().ReadLines();
            }
    
            public static IEnumerable<SubString> ReadLines (this SubString subString)
            {
                var baseString = subString.BaseString;
                var begin = subString.Begin;
                var end = subString.End;
    
                var beginLine   = begin ;
                var count       = 0     ;
    
                var state       = ParseLineState.NewLine;
    
                for (var iter = begin; iter < end; ++iter)
                {
                    var ch = baseString[iter];
    
                    switch (state)
                    {
                        case ParseLineState.ConsumedCR:
                            yield return new SubString(baseString, beginLine, count);
                            switch (ch)
                            {
                                case '\r':
                                    beginLine = iter;
                                    count = 0;
                                    state = ParseLineState.ConsumedCR;
                                    break;
                                case '\n':
                                    state = ParseLineState.NewLine;
                                    break;
                                default:
                                    beginLine = iter;
                                    count = 1;
                                    state = ParseLineState.Inline;
                                    break;
                            }
    
                            break;
                        case ParseLineState.NewLine:
                            beginLine   = iter;
                            count       = 0;
                            switch (ch)
                            {
                                case '\r':
                                    state = ParseLineState.ConsumedCR;
                                    break;
                                case '\n':
                                    yield return new SubString(baseString, beginLine, count);
                                    state = ParseLineState.NewLine;
                                    break;
                                default:
                                    state = ParseLineState.Inline;
                                    ++count;
                                    break;
                            }
                            break;
                        case ParseLineState.Inline:
                        default:
                            switch (ch)
                            {
                                case '\r':
                                    state = ParseLineState.ConsumedCR;
                                    break;
                                case '\n':
                                    yield return new SubString(baseString, beginLine, count);
                                    state = ParseLineState.NewLine;
                                    break;
                                default:
                                    ++count;
                                    break;
                            }
                            break;
                    }
                }
    
                switch (state)
                {
                    case ParseLineState.NewLine:
                        break;
                    case ParseLineState.ConsumedCR:
                    case ParseLineState.Inline:
                    default:
                        yield return new SubString(baseString, beginLine, count);
                        break;
                }
            }
    
        }
    
        struct SubString 
            :   IComparable
            ,   ICloneable
            ,   IComparable<SubString>
            ,   IEnumerable<char>
            ,   IEquatable<SubString>
        {
            readonly string m_baseString;
            readonly int m_begin;
            readonly int m_end;
    
            string m_value;
            int m_hashCode;
            bool m_hasHashCode;
    
            static int Clamp (int v, int l, int r)
            {
                if (v < l)
                {
                    return l;
                }
    
                if (r < v)
                {
                    return r;
                }
    
                return v;
            }
    
            public static readonly SubString Empty = new SubString(null, 0,0);
    
            public SubString(SubString subString, int begin, int count) : this()
            {
                m_baseString = subString.BaseString;
                var length = subString.Length;
    
                begin = Clamp(begin, 0, length);
                count = Clamp(count, 0, length - begin);
    
                var end = begin + count;
    
                m_begin = subString.Begin + begin;
                m_end = subString.Begin + end;
            }
    
            public SubString(string baseString, int begin, int count) : this()
            {
                m_baseString = baseString;
                var length = BaseString.Length;
    
                begin = Clamp(begin, 0, length);
                count = Clamp(count, 0, length - begin);
    
                var end = begin + count;
    
                m_begin = begin;
                m_end = end;
            }
    
            public bool Equals(SubString other)
            {
                return CompareTo(other) == 0;
            }
    
            public override int GetHashCode()
            {
                if (!m_hasHashCode)
                {
                    m_hashCode = Value.GetHashCode();
                    m_hasHashCode = true;
                }
    
                return m_hashCode;
            }
    
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
    
            public object Clone()
            {
                return this;
            }
    
            public int CompareTo(object obj)
            {
                return obj is SubString ? CompareTo((SubString) obj) : 1;
            }
    
    
            public override bool Equals(object obj)
            {
                return obj is SubString && Equals((SubString) obj);
            }
    
    
            public int CompareTo(SubString other)
            {
                return String.Compare(
                    BaseString,
                    Begin,
                    other.BaseString,
                    other.Begin,
                    Math.Min(Length, other.Length)
                    );
            }
    
            public IEnumerator<char> GetEnumerator()
            {
                for (var iter = Begin; iter < End; ++iter)
                {
                    yield return BaseString[iter];
                }
            }
    
            public override string ToString()
            {
                return Value;
            }
    
            public string Value
            {
                get
                {
                    if (m_value == null)
                    {
                        m_value = BaseString.Substring(Begin, Length);
                    }
                    return m_value;
                }
            }
    
            public string BaseString
            {
                get { return m_baseString ?? ""; }
            }
    
            public int Begin
            {
                get { return m_begin; }
            }
    
            public int End
            {
                get { return m_end; }
            }
    
            public char this[int idx]
            {
                get
                {
                    if (idx < 0)
                    {
                        throw new IndexOutOfRangeException("idx");
                    }
    
                    if (idx >= Length)
                    {
                        throw new IndexOutOfRangeException("idx");
                    }
    
                    return BaseString[idx + Begin];
                }
            }
    
            public int Length
            {
                get { return End - Begin; }
            }
    
            public bool IsEmpty
            {
                get { return Length == 0; }
            }
    
            public bool IsWhiteSpace
            {
                get
                {
                    if (IsEmpty)
                    {
                        return true;
                    }
    
                    for(var iter = Begin; iter < End; ++iter)
                    {
                        if (!Char.IsWhiteSpace(BaseString[iter]))
                        {
                            return false;
                        }
                    }
    
                    return true;
                }
            }
    
        }
    }
}
// ############################################################################

// ############################################################################
namespace M3.HRON.Include
{
    static partial class MetaData
    {
        public const string RootPath        = @"https://raw.github.com/";
        public const string IncludeDate     = @"2012-11-13T22:07:00";

        public const string Include_0       = @"mrange/T4Include/master/Hron/HRONSerializer.cs";
        public const string Include_1       = @"mrange/T4Include/master/Hron/HRONDynamicObjectSerializer.cs";
        public const string Include_2       = @"https://raw.github.com/mrange/T4Include/master/Common/Array.cs";
        public const string Include_3       = @"https://raw.github.com/mrange/T4Include/master/Common/Config.cs";
        public const string Include_4       = @"https://raw.github.com/mrange/T4Include/master/Common/SubString.cs";
    }
}
// ############################################################################

