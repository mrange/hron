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


using System.Text;

namespace M3.HRON.Generator.Parser
{
    static partial class ScannerExtensions
    {
        public static string Slice (this string baseString, int begin, int end)
        {
            return baseString.Substring(begin, end - begin);
        }

        public static StringBuilder AppendSlice (this StringBuilder sb, string baseString, int begin, int end)
        {
            sb.Append(baseString, begin, end - begin);
            return sb;
        }

        enum ParseLineState
        {
            NewLine     ,
            Inline      ,
            ConsumedCR  ,
        }

        public delegate bool ReadLineDelegate (string baseString, int begin, int end);

        public static void ReadLines (this string baseString, int begin, int end, ReadLineDelegate readLineDelegate)
        {
            var beginLine   = begin ;
            var endLine     = begin ;

            var state       = ParseLineState.NewLine;

            for (var iter = begin; iter < end; ++iter)
            {
                var ch = baseString[iter];

                switch (state)
                {
                    case ParseLineState.ConsumedCR:
                        if (!readLineDelegate (baseString, beginLine, endLine)) return;
                        switch (ch)
                        {
                            case '\r':
                                beginLine   = iter;
                                endLine     = iter;
                                state = ParseLineState.ConsumedCR;
                                break;
                            case '\n':
                                state = ParseLineState.NewLine;
                                break;
                            default:
                                beginLine   = iter;
                                endLine     = iter + 1;
                                state = ParseLineState.Inline;
                                break;
                        }

                        break;
                    case ParseLineState.NewLine:
                        beginLine   = iter;
                        endLine     = iter;
                        switch (ch)
                        {
                            case '\r':
                                state = ParseLineState.ConsumedCR;
                                break;
                            case '\n':
                                if (!readLineDelegate (baseString, beginLine, endLine)) return;
                                state = ParseLineState.NewLine;
                                break;
                            default:
                                state = ParseLineState.Inline;
                                ++endLine;
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
                                if (!readLineDelegate (baseString, beginLine, endLine)) return;
                                state = ParseLineState.NewLine;
                                break;
                            default:
                                ++endLine;
                                break;
                        }
                        break;
                }
            }

            switch (state)
            {
                case ParseLineState.NewLine:
                    if (!readLineDelegate (baseString, 0, 0)) return;
                    break;
                case ParseLineState.ConsumedCR:
                    if (!readLineDelegate (baseString, beginLine, endLine)) return;
                    if (!readLineDelegate (baseString, 0, 0)) return;
                    break;
                case ParseLineState.Inline:
                default:
                    if (!readLineDelegate (baseString, beginLine, endLine)) return;
                    break;
            }
        }

    }

    static partial class ScannerInterface
    {
        public enum Error
        {
            General     ,
            WrongTag    ,
            NonEmptyTag ,
        }

        public partial interface IScannerVisitor
        {
            void Document_Begin();
            void Document_End();

            void PreProcessor(string baseString, int begin, int end);

            void Empty(string baseString, int begin, int end);

            void Comment(int indent, string baseString, int begin, int end);

            void Object_Begin(string baseString, int begin, int end);
            void Object_End();

            void Value_Begin(string baseString, int begin, int end);
            void Value_Line(string baseString, int begin, int end);
            void Value_End();

            void Error(int lineNo, string baseString, int begin, int end, Error parseError);

            int ErrorCount { get; }
        }
    }

    partial class Scanner
    {
        bool m_isBuildingValue;
        int m_indention;
        int m_expectedIndention;
        int m_lineNo;
        readonly ScannerInterface.IScannerVisitor m_visitor;

        public Scanner(ScannerInterface.IScannerVisitor visitor)
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
            m_visitor.PreProcessor(BaseString, Begin + m_indention + 1, End);
        }

        partial void Partial_StateTransition__To_EndOfCommentTag()
        {
            m_visitor.Comment(m_indention, BaseString, Begin + m_indention + 1, End);
        }

        partial void Partial_StateTransition__To_EndOfEmptyTag()
        {
            if (m_isBuildingValue)
            {
                m_visitor.Value_Line(BaseString, Begin, Begin);
            }
            else
            {
                m_visitor.Empty(BaseString, Begin, End);
            }
        }

        partial void Partial_StateTransition__To_EndOfObjectTag()
        {
            PopContext();
            m_visitor.Object_Begin(BaseString, Begin + m_indention + 1, End);
            m_expectedIndention = m_indention + 1;
        }

        partial void Partial_StateTransition__To_EndOfValueTag()
        {
            PopContext();
            m_isBuildingValue = true;
            m_visitor.Value_Begin(BaseString, Begin + m_indention + 1, End);
            m_expectedIndention = m_indention + 1;
        }

        partial void Partial_StateTransition__To_EndOfValueLine()
        {
            m_visitor.Value_Line(BaseString, Begin + m_expectedIndention, End);
        }

        partial void Partial_StateTransition__To_Error()
        {
            Result = ParserResult.Error;
            m_visitor.Error(m_lineNo, BaseString, Begin, End, ScannerInterface.Error.General);
        }

        partial void Partial_StateTransition__To_WrongTagError()
        {
            Result = ParserResult.Error;
            m_visitor.Error(m_lineNo, BaseString, Begin, End, ScannerInterface.Error.WrongTag);
        }

        partial void Partial_StateTransition__To_NonEmptyTagError()
        {
            Result = ParserResult.Error;
            m_visitor.Error(m_lineNo, BaseString, Begin, End, ScannerInterface.Error.NonEmptyTag);
        }

    }
}
