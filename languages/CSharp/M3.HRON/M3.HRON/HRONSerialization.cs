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
    using Source.Common;
    using Source.HRON;

    public static partial class HRONSerializationExtensions
    {
        public static dynamic ParseAsHRON(this string input, object defaultValue = null)
        {
            object dynamicValue;
            return HRONSerialization.TryParseAsDynamic(input, out dynamicValue) 
                ? dynamicValue 
                : defaultValue
                ;
        }
    }

    public static partial class HRONSerialization
    {
        public static bool TryParseAsDynamic (string input, out object dynamicValue)
        {
            HRONDynamicParseError[] errors;
            return HRONSerializer.TryParseDynamic(
                0,
                input.ReadLines(),
                out dynamicValue,
                out errors
                );
        }
    }
}
