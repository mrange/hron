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

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

using M3.HRON.Generator.Source.Common;

namespace M3.HRON.Generator.Parser
{

    interface IVisitor
    {
        void Document_Begin();
        void Document_End();

        void Object_Begin(SubString name);
        void Object_End();

        void Value_Begin(SubString name);
        void Value_Line(SubString name);
        void Value_End();
    }

    partial class Scanner
    {

        bool m_isBuildingValue;
        int m_indention;
        int m_expectedIndention;
        int m_lineNo;
        SubString m_current;
        readonly IVisitor m_visitor;
        static readonly SubString s_empty = new SubString();

        public Scanner(IVisitor visitor)
        {
            m_visitor = visitor;
            State = ParserState.Indention;
        }

        partial void Partial_AcceptEndOfStream()
        {
            m_indention = 0;
            PopContext();
        }

        partial void Partial_BeginLine(SubString ss)
        {
            State = ParserState.Indention;
            m_indention = 0;
            ++m_lineNo;
            m_current = ss;
        }

        partial void Partial_StateChoice(char current, ParserStateChoice choice, ParserState from, ref ParserState to)
        {
            switch (choice)
            {
                case ParserStateChoice.From_Indention__Choose_TagExpected_ValueLine_Error:
                    if (m_isBuildingValue)
                    {
                        to = m_expectedIndention > m_indention 
                            ? ParserState.TagExpected
                            : ParserState.ValueLine
                            ;
                    }
                    else 
                    {
                        to = m_expectedIndention < m_indention 
                            ? ParserState.Error 
                            : ParserState.TagExpected
                            ;
                    }

                    break;
                default:
                    to = ParserState.Error;
                    break;
            }
        }

        void PopContext()
        {
            if (m_isBuildingValue && m_indention < m_expectedIndention)
            {
                --m_expectedIndention;
                m_visitor.Value_End();
                m_isBuildingValue = false;
            }

            while (m_indention < m_expectedIndention)
            {
                --m_expectedIndention;
                m_visitor.Object_End();
            }

        }

        partial void Partial_StateTransition(char current, ParserState from, ParserState to, ParserStateTransition transition, ref ParserResult result)
        {
            switch (to)
            {
                case ParserState.Indention:
                    if (from == ParserState.Indention)
                    {
                        ++m_indention;
                    }
                    return;
                case ParserState.EndOfCommentTag:
                    return;
                case ParserState.EndOfEmptyTag:
                    if (m_isBuildingValue)
                    {
                        m_visitor.Value_Line(s_empty);
                    }
                    return;
                case ParserState.EndOfObjectTag:
                    PopContext();
                    m_visitor.Object_Begin(m_current.ToSubString(m_expectedIndention + 1));
                    m_expectedIndention = m_indention + 1;
                    break;
                case ParserState.EndOfValueTag:
                    PopContext();
                    m_isBuildingValue = true;
                    m_visitor.Value_Begin(m_current.ToSubString(m_expectedIndention + 1));
                    m_expectedIndention = m_indention + 1;
                    break;
                case ParserState.EndOfValueLine:
                    m_visitor.Value_Line(m_current.ToSubString(m_expectedIndention));
                    break;
                case ParserState.Error:
                    result = ParserResult.Error;
                    return;
                default:
                    return;
            }
        }
    }
}
