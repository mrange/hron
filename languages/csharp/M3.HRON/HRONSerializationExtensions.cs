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



namespace M3.HRON
{
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using M3.HRON.Generator.Source.Common;

    public static partial class HRONSerializationExtensions
    {
        internal static IHRONEntity FirstOrEmpty(this IEnumerable<IHRONEntity> entities)
        {
            if (entities == null)
            {
                return HRONValue.Empty;
            }

            return entities.FirstOrDefault() ?? HRONValue.Empty;
        }

        internal static IEnumerable<string> ReadLines(this TextReader tr)
        {
            if (tr == null)
            {
                yield break;
            }

            string line;
            while ((line = tr.ReadLine()) != null)
            {
                yield return line;
            }
        }

        public static dynamic ParseAsHRON(this string input, object defaultValue = null)
        {
            HRONObject dynamicValue;
            HRONDynamicParseError[] errors;
            return HRONSerialization.TryParseAsDynamic(input.ReadLines(), out dynamicValue, out errors) 
                       ? dynamicValue 
                       : defaultValue
                ;
        }

        public static dynamic ParseAsHRON(this IEnumerable<string> input, object defaultValue = null)
        {
            HRONObject dynamicValue;
            HRONDynamicParseError[] errors;
            return HRONSerialization.TryParseAsDynamic(input, out dynamicValue, out errors)
                       ? dynamicValue
                       : defaultValue
                ;
        }

        public static dynamic ParseAsHRON(this TextReader textReader, object defaultValue = null)
        {
            HRONObject dynamicValue;
            HRONDynamicParseError[] errors;
            return HRONSerialization.TryParseAsDynamic(textReader.ReadLines(), out dynamicValue, out errors)
                       ? dynamicValue
                       : defaultValue
                ;
        }

        public static string SerializeKeyValuePairsAsHRON(this IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            return HRONSerialization.SerializeKeyValuePairs(keyValuePairs);
        }
    }
}