﻿// ----------------------------------------------------------------------------------------------
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


namespace M3.HRON.Generator.Parser
{
    using M3.HRON.Generator.Source.Common;

    interface IScannerVisitor
    {
        void Document_Begin();
        void Document_End();

        void PreProcessor(SubString preProcessor);

        void Empty(SubString line);

        void Comment(int indent, SubString comment);

        void Object_Begin(SubString name);
        void Object_End();

        void Value_Begin(SubString name);
        void Value_Line(SubString content);
        void Value_End();

        void Error(int lineNo, SubString line, Scanner.Error parseError);
    }

    partial class Scanner
    {
        public enum Error
        {
            General     ,
            WrongTag    ,
            NonEmptyTag ,
        }

        bool m_isBuildingValue;
        int m_indention;
        int m_expectedIndention;
        int m_lineNo;
        readonly IScannerVisitor m_visitor;
        static readonly SubString s_empty = new SubString();

        public Scanner(IScannerVisitor visitor)
        {
            m_visitor = visitor;
            State = ParserState.PreProcessing;
        }

        partial void Partial_AcceptEndOfStream()
        {
            m_indention = 0;
            PopContext();
        }

        partial void Partial_BeginLine()
        {
            switch (State)
            {
                case ParserState.PreProcessing:
                case ParserState.PreProcessorTag:
                case ParserState.EndOfPreProcessorTag:
                    State = ParserState.PreProcessing;
                    break;
                default:
                    State = ParserState.Indention;
                    break;
            }

            m_indention = 0;
            ++m_lineNo;
        }

        partial void Partial_StateChoice__From_Indention__Choose_TagExpected_NoContentTagExpected_ValueLine_Error()
        {
            if (m_isBuildingValue)
            {
                State = m_expectedIndention > m_indention
                    ? ParserState.TagExpected
                    : ParserState.ValueLine
                    ;
            }
            else
            {
                State = m_expectedIndention < m_indention
                    ? ParserState.NoContentTagExpected
                    : ParserState.TagExpected
                    ;
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

        partial void Partial_StateTransition__From_Indention__To_Indention()
        {
            ++m_indention;
        }

        partial void Partial_StateTransition__To_PreProcessorTag()
        {
            Result = ParserResult.Done;
        }

        partial void Partial_StateTransition__To_EmptyTag()
        {
            Result = ParserResult.Done;            
        }

        partial void Partial_StateTransition__To_CommentTag()
        {
            Result = ParserResult.Done;
        }

        partial void Partial_StateTransition__To_ValueTag()
        {
            Result = ParserResult.Done;
        }

        partial void Partial_StateTransition__To_ValueLine()
        {
            Result = ParserResult.Done;
        }

        partial void Partial_StateTransition__To_ObjectTag()
        {
            Result = ParserResult.Done;
        }

        partial void Partial_StateTransition__To_EndOfPreProcessorTag()
        {
            m_visitor.PreProcessor(CurrentLine.ToSubString(m_indention + 1));
        }

        partial void Partial_StateTransition__To_EndOfCommentTag()
        {
            m_visitor.Comment(m_indention, CurrentLine.ToSubString(m_indention + 1));
        }

        partial void Partial_StateTransition__To_EndOfEmptyTag()
        {
            if (m_isBuildingValue)
            {
                m_visitor.Value_Line(s_empty);
            }
            else
            {
                m_visitor.Empty(CurrentLine);
            }
        }

        partial void Partial_StateTransition__To_EndOfObjectTag()
        {
            PopContext();
            m_visitor.Object_Begin(CurrentLine.ToSubString(m_indention + 1));
            m_expectedIndention = m_indention + 1;
        }

        partial void Partial_StateTransition__To_EndOfValueTag()
        {
            PopContext();
            m_isBuildingValue = true;
            m_visitor.Value_Begin(CurrentLine.ToSubString(m_indention + 1));
            m_expectedIndention = m_indention + 1;
        }

        partial void Partial_StateTransition__To_EndOfValueLine()
        {
            m_visitor.Value_Line(CurrentLine.ToSubString(m_expectedIndention));
        }

        partial void Partial_StateTransition__To_Error()
        {
            Result = ParserResult.Error;
            m_visitor.Error(m_lineNo, CurrentLine, Error.General);
        }

        partial void Partial_StateTransition__To_WrongTagError()
        {
            Result = ParserResult.Error;
            m_visitor.Error(m_lineNo, CurrentLine, Error.WrongTag);
        }

        partial void Partial_StateTransition__To_NonEmptyTagError()
        {
            Result = ParserResult.Error;
            m_visitor.Error(m_lineNo, CurrentLine, Error.NonEmptyTag);
        }

    }
}
