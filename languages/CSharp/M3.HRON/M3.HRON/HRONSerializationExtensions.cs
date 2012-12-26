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
    using System.IO;

    public static partial class HRONSerializationExtensions
    {
/*
        public static dynamic ParseAsHRON(this string input, object defaultValue = null)
        {
            object dynamicValue;
            return HRONSerialization.TryParseAsDynamic(input, out dynamicValue) 
                       ? dynamicValue 
                       : defaultValue
                ;
        }

        public static dynamic ParseAsHRON(this IEnumerable<string> input, object defaultValue = null)
        {
            object dynamicValue;
            return HRONSerialization.TryParseAsDynamic(input, out dynamicValue)
                       ? dynamicValue
                       : defaultValue
                ;
        }

        public static dynamic ParseAsHRON(this TextReader textReader, object defaultValue = null)
        {
            object dynamicValue;
            return HRONSerialization.TryParseAsDynamic(textReader, out dynamicValue)
                       ? dynamicValue
                       : defaultValue
                ;
        }

        public static string SerializeKeyValuePairsAsHRON(this IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            return HRONSerialization.SerializeKeyValuePairs(keyValuePairs);
        }
*/
    }
}