
// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # text template file (.tt)                                                      #
// ############################################################################



// ############################################################################
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/SubString.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/Array.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Extensions/ParseExtensions.cs
// @@@ INCLUDE_FOUND: ../Common/Config.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Extensions/EnumParseExtensions.cs
// @@@ INCLUDE_FOUND: ../Reflection/StaticReflection.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Common/Config.cs
// @@@ INCLUDING: https://raw.github.com/mrange/T4Include/master/Reflection/StaticReflection.cs
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
namespace M3.HRON.Generator
{
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
    
    
    
    
    namespace Source.Common
    {
        using System;
        using System.Collections;
        using System.Collections.Generic;
        using System.Text;
    
    
        static class SubStringExtensions
        {
            public static void AppendSubString (this StringBuilder sb, SubString ss)
            {
                sb.Append (ss.BaseString, ss.Begin, ss.Length);
            }
    
            public static string Concatenate (this IEnumerable<SubString> values, string delimiter = null)
            {
                if (values == null)
                {
                    return "";
                }
    
                delimiter = delimiter ?? ", ";
    
                var first = true;
    
                var sb = new StringBuilder ();
                foreach (var value in values)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append (delimiter);
                    }
    
                    sb.AppendSubString (value);
                }
    
                return sb.ToString ();
            }
    
    
    
            public static SubString ToSubString (this string value, int begin = 0, int count = int.MaxValue / 2)
            {
                return new SubString (value, begin, count);
            }
    
            public static SubString ToSubString (this StringBuilder value, int begin = 0, int count = int.MaxValue / 2)
            {
                return new SubString (value.ToString (), begin, count);
            }
    
            public static SubString ToSubString (this SubString value, int begin = 0, int count = int.MaxValue / 2)
            {
                return new SubString (value, begin, count);
            }
    
            enum ParseLineState
            {
                NewLine     ,
                Inline      ,
                ConsumedCR  ,
            }
    
            public static IEnumerable<SubString> ReadLines (this string value)
            {
                return value.ToSubString ().ReadLines ();
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
                            yield return new SubString (baseString, beginLine, count);
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
                                    yield return new SubString (baseString, beginLine, count);
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
                                    yield return new SubString (baseString, beginLine, count);
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
                        yield return new SubString (baseString, 0, 0);
                        break;
                    case ParseLineState.ConsumedCR:
                        yield return new SubString (baseString, beginLine, count);
                        yield return new SubString (baseString, 0, 0);
                        break;
                    case ParseLineState.Inline:
                    default:
                        yield return new SubString (baseString, beginLine, count);
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
    
            public static readonly SubString Empty = new SubString (null, 0,0);
    
            public SubString (SubString subString, int begin, int count) : this ()
            {
                m_baseString    = subString.BaseString;
                var length      = subString.Length;
    
                begin           = Clamp (begin, 0, length);
                count           = Clamp (count, 0, length - begin);
                var end         = begin + count;
    
                m_begin         = subString.Begin + begin;
                m_end           = subString.Begin + end;
            }
    
            public SubString (string baseString, int begin, int count) : this ()
            {
                m_baseString    = baseString;
                var length      = BaseString.Length;
    
                begin           = Clamp (begin, 0, length);
                count           = Clamp (count, 0, length - begin);
                var end         = begin + count;
    
                m_begin         = begin;
                m_end           = end;
            }
    
            public static bool operator== (SubString left, SubString right)
            {
                return left.CompareTo (right) == 0;
            }
    
            public static bool operator!= (SubString left, SubString right)
            {
                return left.CompareTo (right) != 0;
            }
    
            public bool Equals (SubString other)
            {
                return CompareTo (other) == 0;
            }
    
            public override int GetHashCode  ()
            {
                if (!m_hasHashCode)
                {
                    m_hashCode = Value.GetHashCode ();
                    m_hasHashCode = true;
                }
    
                return m_hashCode;
            }
    
            IEnumerator IEnumerable.GetEnumerator ()
            {
                return GetEnumerator ();
            }
    
            public object Clone ()
            {
                return this;
            }
    
            public int CompareTo (object obj)
            {
                return obj is SubString ? CompareTo ((SubString) obj) : 1;
            }
    
    
            public override bool Equals (object obj)
            {
                return obj is SubString && Equals ((SubString) obj);
            }
    
    
            public int CompareTo (SubString other)
            {
                if (Length < other.Length)
                {
                    return -1;
                }
    
                if (Length > other.Length)
                {
                    return 1;
                }
    
                return String.Compare (
                    BaseString          ,
                    Begin               ,
                    other.BaseString    ,
                    other.Begin         ,
                    Length
                    );
            }
    
            public IEnumerator<char> GetEnumerator ()
            {
                for (var iter = Begin; iter < End; ++iter)
                {
                    yield return BaseString[iter];
                }
            }
    
            public override string ToString ()
            {
                return Value;
            }
    
            public string Value
            {
                get
                {
                    if (m_value == null)
                    {
                        m_value = BaseString.Substring (Begin, Length);
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
                        throw new IndexOutOfRangeException ("idx");
                    }
    
                    if (idx >= Length)
                    {
                        throw new IndexOutOfRangeException ("idx");
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
    
                    for (var iter = Begin; iter < End; ++iter)
                    {
                        if (!Char.IsWhiteSpace (BaseString[iter]))
                        {
                            return false;
                        }
                    }
    
                    return true;
                }
            }
    
            public bool All (Func<char,bool> test)
            {
                if (test == null)
                {
                    return true;
                }
    
                if (IsEmpty)
                {
                    return true;
                }
    
                for (var iter = Begin; iter < End; ++iter)
                {
                    if (!test (BaseString[iter]))
                    {
                        return false;
                    }
                }
    
                return true;
            }
    
            static readonly char[] s_defaultTrimChars = " \t\r\n".ToCharArray ();
    
            static bool Contains (char[] trimChars, char ch)
            {
                for (int index = 0; index < trimChars.Length; index++)
                {
                    var trimChar = trimChars[index];
    
                    if (trimChar == ch)
                    {
                        return true;
                    }
                }
    
                return false;
            }
    
            public SubString TrimStart (params char[] trimChars)
            {
                if (trimChars == null || trimChars.Length == 0)
                {
                    trimChars = s_defaultTrimChars;
                }
    
                for (var iter = Begin; iter < End; ++iter)
                {
                    var ch = BaseString[iter];
    
                    if (!Contains (trimChars, ch))
                    {
                        return new SubString (BaseString, iter, End - iter);
                    }
                }
    
                return new SubString (BaseString, Begin, 0);
            }
    
            public SubString TrimEnd (params char[] trimChars)
            {
                if (trimChars == null || trimChars.Length == 0)
                {
                    trimChars = s_defaultTrimChars;
                }
    
                for (var iter = End - 1; iter >= Begin; --iter)
                {
                    var ch = BaseString[iter];
    
                    if (!Contains (trimChars, ch))
                    {
                        return new SubString (BaseString, Begin, iter - Begin + 1);
                    }
                }
    
                return new SubString (BaseString, Begin, 0);
            }
    
            public SubString Trim (params char[] trimChars)
            {
                return TrimStart (trimChars).TrimEnd (trimChars);
            }
        }
    }
}

// ############################################################################

// ############################################################################
namespace M3.HRON.Generator
{
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
    
    namespace Source.Common
    {
        static class Array<T>
        {
            public static readonly T[] Empty = new T[0];
        }
    }
}

// ############################################################################

// ############################################################################
namespace M3.HRON.Generator
{
    
    
    
    // ############################################################################
    // #                                                                          #
    // #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
    // #                                                                          #
    // # This means that any edits to the .cs file will be lost when its          #
    // # regenerated. Changes should instead be applied to the corresponding      #
    // # template file (.tt)                                                      #
    // ############################################################################
    
    
    
    
    
    
    
    namespace Source.Extensions
    {
        using System;
        using System.Collections.Generic;
        using System.Globalization;
    
        static partial class ParseExtensions
        {
            static readonly Dictionary<Type, Func<object>> s_defaultValues = new Dictionary<Type, Func<object>> 
                {
    #if !T4INCLUDE__SUPPRESS_BOOLEAN_PARSE_EXTENSIONS
                    { typeof(Boolean)      , () => default (Boolean)},
                    { typeof(Boolean?)     , () => default (Boolean?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_CHAR_PARSE_EXTENSIONS
                    { typeof(Char)      , () => default (Char)},
                    { typeof(Char?)     , () => default (Char?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_SBYTE_PARSE_EXTENSIONS
                    { typeof(SByte)      , () => default (SByte)},
                    { typeof(SByte?)     , () => default (SByte?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT16_PARSE_EXTENSIONS
                    { typeof(Int16)      , () => default (Int16)},
                    { typeof(Int16?)     , () => default (Int16?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT32_PARSE_EXTENSIONS
                    { typeof(Int32)      , () => default (Int32)},
                    { typeof(Int32?)     , () => default (Int32?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT64_PARSE_EXTENSIONS
                    { typeof(Int64)      , () => default (Int64)},
                    { typeof(Int64?)     , () => default (Int64?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_BYTE_PARSE_EXTENSIONS
                    { typeof(Byte)      , () => default (Byte)},
                    { typeof(Byte?)     , () => default (Byte?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT16_PARSE_EXTENSIONS
                    { typeof(UInt16)      , () => default (UInt16)},
                    { typeof(UInt16?)     , () => default (UInt16?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT32_PARSE_EXTENSIONS
                    { typeof(UInt32)      , () => default (UInt32)},
                    { typeof(UInt32?)     , () => default (UInt32?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT64_PARSE_EXTENSIONS
                    { typeof(UInt64)      , () => default (UInt64)},
                    { typeof(UInt64?)     , () => default (UInt64?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_SINGLE_PARSE_EXTENSIONS
                    { typeof(Single)      , () => default (Single)},
                    { typeof(Single?)     , () => default (Single?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_DOUBLE_PARSE_EXTENSIONS
                    { typeof(Double)      , () => default (Double)},
                    { typeof(Double?)     , () => default (Double?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_DECIMAL_PARSE_EXTENSIONS
                    { typeof(Decimal)      , () => default (Decimal)},
                    { typeof(Decimal?)     , () => default (Decimal?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_TIMESPAN_PARSE_EXTENSIONS
                    { typeof(TimeSpan)      , () => default (TimeSpan)},
                    { typeof(TimeSpan?)     , () => default (TimeSpan?)},
    #endif
    #if !T4INCLUDE__SUPPRESS_DATETIME_PARSE_EXTENSIONS
                    { typeof(DateTime)      , () => default (DateTime)},
                    { typeof(DateTime?)     , () => default (DateTime?)},
    #endif
                };
            static readonly Dictionary<Type, Func<string, CultureInfo, object>> s_parsers = new Dictionary<Type, Func<string, CultureInfo, object>> 
                {
    #if !T4INCLUDE__SUPPRESS_BOOLEAN_PARSE_EXTENSIONS
                    { typeof(Boolean)  , (s, ci) => { Boolean value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Boolean?) , (s, ci) => { Boolean value; return s.TryParse(ci, out value) ? (object)(Boolean?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_CHAR_PARSE_EXTENSIONS
                    { typeof(Char)  , (s, ci) => { Char value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Char?) , (s, ci) => { Char value; return s.TryParse(ci, out value) ? (object)(Char?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_SBYTE_PARSE_EXTENSIONS
                    { typeof(SByte)  , (s, ci) => { SByte value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(SByte?) , (s, ci) => { SByte value; return s.TryParse(ci, out value) ? (object)(SByte?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT16_PARSE_EXTENSIONS
                    { typeof(Int16)  , (s, ci) => { Int16 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Int16?) , (s, ci) => { Int16 value; return s.TryParse(ci, out value) ? (object)(Int16?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT32_PARSE_EXTENSIONS
                    { typeof(Int32)  , (s, ci) => { Int32 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Int32?) , (s, ci) => { Int32 value; return s.TryParse(ci, out value) ? (object)(Int32?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_INT64_PARSE_EXTENSIONS
                    { typeof(Int64)  , (s, ci) => { Int64 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Int64?) , (s, ci) => { Int64 value; return s.TryParse(ci, out value) ? (object)(Int64?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_BYTE_PARSE_EXTENSIONS
                    { typeof(Byte)  , (s, ci) => { Byte value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Byte?) , (s, ci) => { Byte value; return s.TryParse(ci, out value) ? (object)(Byte?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT16_PARSE_EXTENSIONS
                    { typeof(UInt16)  , (s, ci) => { UInt16 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(UInt16?) , (s, ci) => { UInt16 value; return s.TryParse(ci, out value) ? (object)(UInt16?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT32_PARSE_EXTENSIONS
                    { typeof(UInt32)  , (s, ci) => { UInt32 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(UInt32?) , (s, ci) => { UInt32 value; return s.TryParse(ci, out value) ? (object)(UInt32?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_UINT64_PARSE_EXTENSIONS
                    { typeof(UInt64)  , (s, ci) => { UInt64 value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(UInt64?) , (s, ci) => { UInt64 value; return s.TryParse(ci, out value) ? (object)(UInt64?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_SINGLE_PARSE_EXTENSIONS
                    { typeof(Single)  , (s, ci) => { Single value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Single?) , (s, ci) => { Single value; return s.TryParse(ci, out value) ? (object)(Single?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_DOUBLE_PARSE_EXTENSIONS
                    { typeof(Double)  , (s, ci) => { Double value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Double?) , (s, ci) => { Double value; return s.TryParse(ci, out value) ? (object)(Double?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_DECIMAL_PARSE_EXTENSIONS
                    { typeof(Decimal)  , (s, ci) => { Decimal value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(Decimal?) , (s, ci) => { Decimal value; return s.TryParse(ci, out value) ? (object)(Decimal?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_TIMESPAN_PARSE_EXTENSIONS
                    { typeof(TimeSpan)  , (s, ci) => { TimeSpan value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(TimeSpan?) , (s, ci) => { TimeSpan value; return s.TryParse(ci, out value) ? (object)(TimeSpan?)value : null;}},
    #endif
    #if !T4INCLUDE__SUPPRESS_DATETIME_PARSE_EXTENSIONS
                    { typeof(DateTime)  , (s, ci) => { DateTime value; return s.TryParse(ci, out value) ? (object)value : null;}},
                    { typeof(DateTime?) , (s, ci) => { DateTime value; return s.TryParse(ci, out value) ? (object)(DateTime?)value : null;}},
    #endif
                };
    
            public static bool CanParse (this Type type)
            {
                if (type == null)
                {
                    return false;
                }
    
                return s_parsers.ContainsKey (type);
            }
    
            public static object GetParsedDefaultValue (this Type type)
            {
                type = type ?? typeof (object);
    
                Func<object> getValue;
    
                return s_defaultValues.TryGetValue (type, out getValue) ? getValue () : null;
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, Type type, out object value)
            {
                value = null;
                if (type == null)
                {
                    return false;
                }                
                
                Func<string, CultureInfo, object> parser;
    
                if (s_parsers.TryGetValue (type, out parser))
                {
                    value = parser (s, cultureInfo);
                }
    
                return value != null;
            }
    
            public static bool TryParse (this string s, Type type, out object value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, type, out value);
            }
    
            public static object Parse (this string s, CultureInfo cultureInfo, Type type, object defaultValue)
            {
                object value;
                return s.TryParse (cultureInfo, type, out value) ? value : defaultValue;
            }
    
            public static object Parse (this string s, Type type, object defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, type, defaultValue);
            }
    
            // Boolean (BoolLike)
    
    #if !T4INCLUDE__SUPPRESS_BOOLEAN_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Boolean value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Boolean Parse (this string s, CultureInfo cultureInfo, Boolean defaultValue)
            {
                Boolean value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Boolean Parse (this string s, Boolean defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Boolean value)
            {
                return Boolean.TryParse (s ?? "", out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_BOOLEAN_PARSE_EXTENSIONS
    
            // Char (CharLike)
    
    #if !T4INCLUDE__SUPPRESS_CHAR_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Char value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Char Parse (this string s, CultureInfo cultureInfo, Char defaultValue)
            {
                Char value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Char Parse (this string s, Char defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Char value)
            {
                return Char.TryParse (s ?? "", out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_CHAR_PARSE_EXTENSIONS
    
            // SByte (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_SBYTE_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out SByte value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static SByte Parse (this string s, CultureInfo cultureInfo, SByte defaultValue)
            {
                SByte value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static SByte Parse (this string s, SByte defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out SByte value)
            {
                return SByte.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_SBYTE_PARSE_EXTENSIONS
    
            // Int16 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_INT16_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Int16 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Int16 Parse (this string s, CultureInfo cultureInfo, Int16 defaultValue)
            {
                Int16 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Int16 Parse (this string s, Int16 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Int16 value)
            {
                return Int16.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_INT16_PARSE_EXTENSIONS
    
            // Int32 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_INT32_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Int32 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Int32 Parse (this string s, CultureInfo cultureInfo, Int32 defaultValue)
            {
                Int32 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Int32 Parse (this string s, Int32 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Int32 value)
            {
                return Int32.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_INT32_PARSE_EXTENSIONS
    
            // Int64 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_INT64_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Int64 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Int64 Parse (this string s, CultureInfo cultureInfo, Int64 defaultValue)
            {
                Int64 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Int64 Parse (this string s, Int64 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Int64 value)
            {
                return Int64.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_INT64_PARSE_EXTENSIONS
    
            // Byte (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_BYTE_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Byte value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Byte Parse (this string s, CultureInfo cultureInfo, Byte defaultValue)
            {
                Byte value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Byte Parse (this string s, Byte defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Byte value)
            {
                return Byte.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_BYTE_PARSE_EXTENSIONS
    
            // UInt16 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_UINT16_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out UInt16 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static UInt16 Parse (this string s, CultureInfo cultureInfo, UInt16 defaultValue)
            {
                UInt16 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static UInt16 Parse (this string s, UInt16 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out UInt16 value)
            {
                return UInt16.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_UINT16_PARSE_EXTENSIONS
    
            // UInt32 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_UINT32_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out UInt32 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static UInt32 Parse (this string s, CultureInfo cultureInfo, UInt32 defaultValue)
            {
                UInt32 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static UInt32 Parse (this string s, UInt32 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out UInt32 value)
            {
                return UInt32.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_UINT32_PARSE_EXTENSIONS
    
            // UInt64 (IntLike)
    
    #if !T4INCLUDE__SUPPRESS_UINT64_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out UInt64 value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static UInt64 Parse (this string s, CultureInfo cultureInfo, UInt64 defaultValue)
            {
                UInt64 value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static UInt64 Parse (this string s, UInt64 defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out UInt64 value)
            {
                return UInt64.TryParse (s ?? "", NumberStyles.Integer, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_UINT64_PARSE_EXTENSIONS
    
            // Single (FloatLike)
    
    #if !T4INCLUDE__SUPPRESS_SINGLE_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Single value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Single Parse (this string s, CultureInfo cultureInfo, Single defaultValue)
            {
                Single value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Single Parse (this string s, Single defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Single value)
            {                                                  
                return Single.TryParse (s ?? "", NumberStyles.Float, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_SINGLE_PARSE_EXTENSIONS
    
            // Double (FloatLike)
    
    #if !T4INCLUDE__SUPPRESS_DOUBLE_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Double value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Double Parse (this string s, CultureInfo cultureInfo, Double defaultValue)
            {
                Double value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Double Parse (this string s, Double defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Double value)
            {                                                  
                return Double.TryParse (s ?? "", NumberStyles.Float, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_DOUBLE_PARSE_EXTENSIONS
    
            // Decimal (FloatLike)
    
    #if !T4INCLUDE__SUPPRESS_DECIMAL_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out Decimal value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static Decimal Parse (this string s, CultureInfo cultureInfo, Decimal defaultValue)
            {
                Decimal value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static Decimal Parse (this string s, Decimal defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out Decimal value)
            {                                                  
                return Decimal.TryParse (s ?? "", NumberStyles.Float, cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_DECIMAL_PARSE_EXTENSIONS
    
            // TimeSpan (TimeSpanLike)
    
    #if !T4INCLUDE__SUPPRESS_TIMESPAN_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out TimeSpan value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static TimeSpan Parse (this string s, CultureInfo cultureInfo, TimeSpan defaultValue)
            {
                TimeSpan value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static TimeSpan Parse (this string s, TimeSpan defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out TimeSpan value)
            {                                                  
                return TimeSpan.TryParse (s ?? "", cultureInfo, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_TIMESPAN_PARSE_EXTENSIONS
    
            // DateTime (DateTimeLike)
    
    #if !T4INCLUDE__SUPPRESS_DATETIME_PARSE_EXTENSIONS
    
            public static bool TryParse (this string s, out DateTime value)
            {
                return s.TryParse (Source.Common.Config.DefaultCulture, out value);
            }
    
            public static DateTime Parse (this string s, CultureInfo cultureInfo, DateTime defaultValue)
            {
                DateTime value;
    
                return s.TryParse (cultureInfo, out value) ? value : defaultValue;
            }
    
            public static DateTime Parse (this string s, DateTime defaultValue)
            {
                return s.Parse (Source.Common.Config.DefaultCulture, defaultValue);
            }
    
            public static bool TryParse (this string s, CultureInfo cultureInfo, out DateTime value)
            {                                                  
                return DateTime.TryParse (s ?? "", cultureInfo, DateTimeStyles.AssumeLocal, out value);
            }
    
    #endif // T4INCLUDE__SUPPRESS_DATETIME_PARSE_EXTENSIONS
    
        }
    }
    
    
}

// ############################################################################

// ############################################################################
namespace M3.HRON.Generator
{
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
    
    
    
    namespace Source.Extensions
    {
        using System;
        using System.Collections.Concurrent;
        using System.Reflection;
    
        using Source.Reflection;
    
        static partial class EnumParseExtensions
        {
            enum Dummy {}
    
            sealed partial class EnumParser
            {
                public Func<string, object> ParseEnum   ;
                public Func<object>         DefaultEnum ;
            }
    
            static readonly MethodInfo s_parseEnum = StaticReflection.GetMethodInfo (() => ParseEnum<Dummy>(default (string)));
            static readonly MethodInfo s_genericParseEnum = s_parseEnum.GetGenericMethodDefinition ();
    
            static readonly MethodInfo s_defaultEnum = StaticReflection.GetMethodInfo (() => DefaultEnum<Dummy>());
            static readonly MethodInfo s_genericDefaultEnum = s_defaultEnum.GetGenericMethodDefinition ();
    
            static readonly MethodInfo s_parseNullableEnum = StaticReflection.GetMethodInfo(() => ParseNullableEnum<Dummy>(default(string)));
            static readonly MethodInfo s_genericParseNullableEnum = s_parseNullableEnum.GetGenericMethodDefinition();
    
            static readonly MethodInfo s_defaultNullableEnum = StaticReflection.GetMethodInfo(() => DefaultNullableEnum<Dummy>());
            static readonly MethodInfo s_genericDefaultNullableEnum = s_defaultNullableEnum.GetGenericMethodDefinition();
    
            static readonly ConcurrentDictionary<Type, EnumParser> s_enumParsers = new ConcurrentDictionary<Type, EnumParser>();
            static readonly Func<Type, EnumParser> s_createParser = type => CreateParser (type);
    
            static EnumParser CreateParser(Type type)
            {
                if (type.IsEnum)
                {
                    return new EnumParser
                               {
                                   ParseEnum = (Func<string, object>)Delegate.CreateDelegate(
                                                        typeof(Func<string, object>),
                                                        s_genericParseEnum.MakeGenericMethod(type)
                                                        ),
                                   DefaultEnum = (Func<object>)Delegate.CreateDelegate(
                                                       typeof(Func<object>),
                                                       s_genericDefaultEnum.MakeGenericMethod(type)
                                                       ),
                               };
    
                }
                else if (
                        type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                    && type.GetGenericArguments()[0].IsEnum
                    )
                {
                    var enumType = type.GetGenericArguments()[0];
                    return new EnumParser
                               {
                                   ParseEnum = (Func<string, object>)Delegate.CreateDelegate(
                                                        typeof(Func<string, object>),
                                                        s_genericParseNullableEnum.MakeGenericMethod(enumType)
                                                        ),
                                   DefaultEnum = (Func<object>)Delegate.CreateDelegate(
                                                       typeof(Func<object>),
                                                       s_genericDefaultNullableEnum.MakeGenericMethod(enumType)
                                                       ),
                               };
    
                }
                else
                {
                    return null;
                }
            }
    
            static object ParseEnum<TEnum>(string value)
                where TEnum : struct
            {
                TEnum result;
                return Enum.TryParse (value, true, out result)
                    ? (object)result
                    : null
                    ;
            }
    
            static object DefaultEnum<TEnum>()
                where TEnum : struct
            {
                return default (TEnum);
            }
    
            static object ParseNullableEnum<TEnum>(string value)
                where TEnum : struct
            {
                TEnum result;
                return Enum.TryParse(value, true, out result)
                    ? (object)(TEnum?)result
                    : null
                    ;
            }
    
            static object DefaultNullableEnum<TEnum>()
                where TEnum : struct
            {
                return default(TEnum?);
            }
    
            public static bool TryParseEnumValue(this string s, Type type, out object value)
            {
                value = null;
                if (string.IsNullOrEmpty (s))
                {
                    return false;
                }
    
                var enumParser = TryGetParser (type);
                if (enumParser == null)
                {
                    return false;
    
                }
                
    
                value = enumParser.ParseEnum (s);
    
                return value != null;
            }
    
            public static bool CanParseEnumValue (this Type type)
            {
                var enumParser = TryGetParser (type);
    
                return enumParser != null;
            }
    
            static EnumParser TryGetParser (Type type)
            {
                if (type == null)
                {
                    return null;
                }
    
                var enumParser = s_enumParsers.GetOrAdd (type, s_createParser);
    
                return enumParser;
            }
    
            public static object ParseEnumValue (this string s, Type type)
            {
                object value;
                return s.TryParseEnumValue (type, out value)
                    ? value
                    : null
                    ;
            }
    
            public static object GetDefaultEnumValue (this Type type)
            {
                var enumParser = TryGetParser (type);
                return enumParser != null ? enumParser.DefaultEnum () : null;
            }
    
            public static TEnum ParseEnumValue<TEnum>(this string s, TEnum defaultValue) 
                where TEnum : struct
            {
                TEnum value;
                return Enum.TryParse (s, true, out value)
                    ? value
                    : defaultValue
                    ;
            }
    
            public static TEnum? ParseEnumValue<TEnum>(this string s)
                where TEnum : struct
            {
                TEnum value;
                return Enum.TryParse(s, true, out value)
                    ? (TEnum?)value
                    : null
                    ;
            }
    
        }
    }
}

// ############################################################################

// ############################################################################
namespace M3.HRON.Generator
{
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

// ############################################################################
namespace M3.HRON.Generator
{
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
    
    namespace Source.Reflection
    {
        using System;
        using System.Linq.Expressions;
        using System.Reflection;
    
        static partial class StaticReflection
        {
            public static MethodInfo GetMethodInfo (Expression<Action> expr)
            {
                return ((MethodCallExpression)expr.Body).Method;
            }
    
            public static MemberInfo GetMemberInfo<TReturn> (Expression<Func<TReturn>> expr)
            {
                return ((MemberExpression)expr.Body).Member;
            }
    
            public static ConstructorInfo GetConstructorInfo<TReturn> (Expression<Func<TReturn>> expr)
            {
                return ((NewExpression)expr.Body).Constructor;
            }
        }
    
        static partial class StaticReflection<T>
        {
            public static MethodInfo GetMethodInfo (Expression<Action<T>> expr)
            {
                return ((MethodCallExpression)expr.Body).Method;
            }
    
            public static MemberInfo GetMemberInfo<TReturn>(Expression<Func<T, TReturn>> expr)
            {
                return ((MemberExpression)expr.Body).Member;
            }
        }
    }
}

// ############################################################################

// ############################################################################
namespace M3.HRON.Generator.Include
{
    static partial class MetaData
    {
        public const string RootPath        = @"https://raw.github.com/";
        public const string IncludeDate     = @"2015-04-19T21:57:16";

        public const string Include_0       = @"mrange/T4Include/master/Common/SubString.cs";
        public const string Include_1       = @"mrange/T4Include/master/Common/Array.cs";
        public const string Include_2       = @"mrange/T4Include/master/Extensions/ParseExtensions.cs";
        public const string Include_3       = @"mrange/T4Include/master/Extensions/EnumParseExtensions.cs";
        public const string Include_4       = @"https://raw.github.com/mrange/T4Include/master/Common/Config.cs";
        public const string Include_5       = @"https://raw.github.com/mrange/T4Include/master/Reflection/StaticReflection.cs";
    }
}
// ############################################################################


