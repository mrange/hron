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
using System;
using System.Text;
using M3.HRON.Generator.Source.Common;

namespace M3.HRON.Generator.Parser
{

    interface IVisitor
    {
        void Document_Begin();
        void Document_End();

        void Object_Begin(SubString name);
        void Object_End(SubString name);

        void Value_Begin(SubString name);
        void Value_Line(SubString name);
        void Value_End(SubString name);
    }

    partial class Scanner
    {
        bool m_isBuildingValue;
        int m_indention;
        int m_expectedIndention;
        int m_lineNo;
        string m_current;

        partial void Partial_BeginLine(string l)
        {
            m_indention = 0;
            ++m_lineNo;
            m_current = l;
        }

        partial void Partial_StateChoice(char current, ParserStateChoice choice, ParserState from, ref ParserState to)
        {
            switch (choice)
            {
                case ParserStateChoice.From_Indention__Choose_TagExpected_ValueLine_OtherValueLine_Error:

                    if (!m_isBuildingValue)
                    {
                        
                    }

                    break;
                default:
                    to = ParserState.Error;
                    break;
            }
        }

        partial void Partial_StateTransition(char current, ParserState from, ParserState to, ParserStateTransition transition, ref ParserResult result)
        {
            switch (to)
            {
                case ParserState.Error:
                default:
                    result = ParserResult.Error;
                    return;
            }
        }
    }
}
