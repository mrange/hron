// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # template file (.tt)                                                      #
// ############################################################################





// ReSharper disable InconsistentNaming



namespace M3.HRON.Generator.Parser
{
    enum ParserState
    {
        Error,
        TagExpected,
        ObjectTag,
        ValueTag,
        EmptyTag,
        CommentTag,
        EndOfObjectTag,
        EndOfEmptyTag,
        EndOfValueTag,
        EndOfCommentTag,
        ValueExpected,
        CommentLine,
        ValueLine,
        EndOfEmptyLine,
        EndOfCommmentLine,
        EndOfValueLine,
    }

    enum ParserStateTransition
    {
            From_Error__To_Error,
            From_TagExpected__To_TagExpected,
            From_TagExpected__To_ObjectTag,
            From_TagExpected__To_ValueTag,
            From_TagExpected__To_CommentTag,
            From_TagExpected__To_EmptyTag,
            From_TagExpected__To_EndOfEmptyTag,
            From_TagExpected__To_Error,
            From_ObjectTag__To_EndOfObjectTag,
            From_ObjectTag__To_ObjectTag,
            From_ValueTag__To_EndOfValueTag,
            From_ValueTag__To_ValueTag,
            From_EmptyTag__To_EndOfEmptyTag,
            From_EmptyTag__To_EmptyTag,
            From_EmptyTag__To_Error,
            From_CommentTag__To_EndOfCommentTag,
            From_CommentTag__To_CommentTag,
            From_EndOfObjectTag__To_EndOfObjectTag,
            From_EndOfObjectTag__To_TagExpected,
            From_EndOfEmptyTag__To_EndOfEmptyTag,
            From_EndOfEmptyTag__To_TagExpected,
            From_EndOfValueTag__To_EndOfValueTag,
            From_EndOfValueTag__To_ValueExpected,
            From_EndOfCommentTag__To_EndOfCommentTag,
            From_EndOfCommentTag__To_TagExpected,
            From_ValueExpected__To_ValueExpected,
            From_ValueExpected__To_CommentLine,
            From_ValueExpected__To_EndOfEmptyLine,
            From_ValueExpected__To_ValueLine,
            From_CommentLine__To_EndOfCommmentLine,
            From_CommentLine__To_CommentLine,
            From_ValueLine__To_EndOfValueLine,
            From_ValueLine__To_ValueLine,
            From_EndOfEmptyLine__To_EndOfEmptyLine,
            From_EndOfEmptyLine__To_TagExpected,
            From_EndOfCommmentLine__To_EndOfCommmentLine,
            From_EndOfCommmentLine__To_ValueExpected,
            From_EndOfValueLine__To_EndOfValueLine,
            From_EndOfValueLine__To_TagExpected,
    }

    enum ParserResult
    {
        Error   ,
        Continue,
        Done    ,
    }

    sealed partial class Scanner
    {
        ParserState State = default (ParserState);

        partial void Partial_ComputeNewState (
            char                    current     ,
            ParserState             from        ,
            ParserState             to          ,
            ParserStateTransition   transition  ,
            ref ParserResult        result      ,
            ref ParserState         newState
            );
                
        public ParserResult AcceptLine (string l)
        {
            l = l ?? "";

            for (var iter = 0; iter < l.Length; ++iter)
            {
                switch (AcceptCharacter (l[iter]))
                {
                case ParserResult.Continue:
                    break;
                case ParserResult.Done:
                    return ParserResult.Done;
                case ParserResult.Error:
                default:
                    return ParserResult.Error;
                }
            }
            return ParserResult.Continue;
        }

        public ParserResult AcceptCharacter (char ch)
        {
            var result = ParserResult.Continue; 
            switch (State)
            {
            case ParserState.Error:
                switch (ch)
                {
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.Error,
                        ParserState.Error,
                        ParserStateTransition.From_Error__To_Error,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.TagExpected:
                switch (ch)
                {
                case '\t':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.TagExpected,
                        ParserState.TagExpected,
                        ParserStateTransition.From_TagExpected__To_TagExpected,
                        ref result,
                        ref State
                        );
                    break;
                case '@':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.TagExpected,
                        ParserState.ObjectTag,
                        ParserStateTransition.From_TagExpected__To_ObjectTag,
                        ref result,
                        ref State
                        );
                    break;
                case '=':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.TagExpected,
                        ParserState.ValueTag,
                        ParserStateTransition.From_TagExpected__To_ValueTag,
                        ref result,
                        ref State
                        );
                    break;
                case '#':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.TagExpected,
                        ParserState.CommentTag,
                        ParserStateTransition.From_TagExpected__To_CommentTag,
                        ref result,
                        ref State
                        );
                    break;
                case ' ':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.TagExpected,
                        ParserState.EmptyTag,
                        ParserStateTransition.From_TagExpected__To_EmptyTag,
                        ref result,
                        ref State
                        );
                    break;
                case '\r':
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.TagExpected,
                        ParserState.EndOfEmptyTag,
                        ParserStateTransition.From_TagExpected__To_EndOfEmptyTag,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.TagExpected,
                        ParserState.Error,
                        ParserStateTransition.From_TagExpected__To_Error,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.ObjectTag:
                switch (ch)
                {
                case '\r':
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.ObjectTag,
                        ParserState.EndOfObjectTag,
                        ParserStateTransition.From_ObjectTag__To_EndOfObjectTag,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.ObjectTag,
                        ParserState.ObjectTag,
                        ParserStateTransition.From_ObjectTag__To_ObjectTag,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.ValueTag:
                switch (ch)
                {
                case '\r':
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.ValueTag,
                        ParserState.EndOfValueTag,
                        ParserStateTransition.From_ValueTag__To_EndOfValueTag,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.ValueTag,
                        ParserState.ValueTag,
                        ParserStateTransition.From_ValueTag__To_ValueTag,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.EmptyTag:
                switch (ch)
                {
                case '\r':
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EmptyTag,
                        ParserState.EndOfEmptyTag,
                        ParserStateTransition.From_EmptyTag__To_EndOfEmptyTag,
                        ref result,
                        ref State
                        );
                    break;
                case '\t':
                case ' ':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EmptyTag,
                        ParserState.EmptyTag,
                        ParserStateTransition.From_EmptyTag__To_EmptyTag,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EmptyTag,
                        ParserState.Error,
                        ParserStateTransition.From_EmptyTag__To_Error,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.CommentTag:
                switch (ch)
                {
                case '\r':
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.CommentTag,
                        ParserState.EndOfCommentTag,
                        ParserStateTransition.From_CommentTag__To_EndOfCommentTag,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.CommentTag,
                        ParserState.CommentTag,
                        ParserStateTransition.From_CommentTag__To_CommentTag,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.EndOfObjectTag:
                switch (ch)
                {
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfObjectTag,
                        ParserState.EndOfObjectTag,
                        ParserStateTransition.From_EndOfObjectTag__To_EndOfObjectTag,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfObjectTag,
                        ParserState.TagExpected,
                        ParserStateTransition.From_EndOfObjectTag__To_TagExpected,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.EndOfEmptyTag:
                switch (ch)
                {
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfEmptyTag,
                        ParserState.EndOfEmptyTag,
                        ParserStateTransition.From_EndOfEmptyTag__To_EndOfEmptyTag,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfEmptyTag,
                        ParserState.TagExpected,
                        ParserStateTransition.From_EndOfEmptyTag__To_TagExpected,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.EndOfValueTag:
                switch (ch)
                {
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfValueTag,
                        ParserState.EndOfValueTag,
                        ParserStateTransition.From_EndOfValueTag__To_EndOfValueTag,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfValueTag,
                        ParserState.ValueExpected,
                        ParserStateTransition.From_EndOfValueTag__To_ValueExpected,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.EndOfCommentTag:
                switch (ch)
                {
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfCommentTag,
                        ParserState.EndOfCommentTag,
                        ParserStateTransition.From_EndOfCommentTag__To_EndOfCommentTag,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfCommentTag,
                        ParserState.TagExpected,
                        ParserStateTransition.From_EndOfCommentTag__To_TagExpected,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.ValueExpected:
                switch (ch)
                {
                case '\t':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.ValueExpected,
                        ParserState.ValueExpected,
                        ParserStateTransition.From_ValueExpected__To_ValueExpected,
                        ref result,
                        ref State
                        );
                    break;
                case '#':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.ValueExpected,
                        ParserState.CommentLine,
                        ParserStateTransition.From_ValueExpected__To_CommentLine,
                        ref result,
                        ref State
                        );
                    break;
                case '\r':
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.ValueExpected,
                        ParserState.EndOfEmptyLine,
                        ParserStateTransition.From_ValueExpected__To_EndOfEmptyLine,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.ValueExpected,
                        ParserState.ValueLine,
                        ParserStateTransition.From_ValueExpected__To_ValueLine,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.CommentLine:
                switch (ch)
                {
                case '\r':
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.CommentLine,
                        ParserState.EndOfCommmentLine,
                        ParserStateTransition.From_CommentLine__To_EndOfCommmentLine,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.CommentLine,
                        ParserState.CommentLine,
                        ParserStateTransition.From_CommentLine__To_CommentLine,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.ValueLine:
                switch (ch)
                {
                case '\r':
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.ValueLine,
                        ParserState.EndOfValueLine,
                        ParserStateTransition.From_ValueLine__To_EndOfValueLine,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.ValueLine,
                        ParserState.ValueLine,
                        ParserStateTransition.From_ValueLine__To_ValueLine,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.EndOfEmptyLine:
                switch (ch)
                {
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfEmptyLine,
                        ParserState.EndOfEmptyLine,
                        ParserStateTransition.From_EndOfEmptyLine__To_EndOfEmptyLine,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfEmptyLine,
                        ParserState.TagExpected,
                        ParserStateTransition.From_EndOfEmptyLine__To_TagExpected,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.EndOfCommmentLine:
                switch (ch)
                {
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfCommmentLine,
                        ParserState.EndOfCommmentLine,
                        ParserStateTransition.From_EndOfCommmentLine__To_EndOfCommmentLine,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfCommmentLine,
                        ParserState.ValueExpected,
                        ParserStateTransition.From_EndOfCommmentLine__To_ValueExpected,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            case ParserState.EndOfValueLine:
                switch (ch)
                {
                case '\n':
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfValueLine,
                        ParserState.EndOfValueLine,
                        ParserStateTransition.From_EndOfValueLine__To_EndOfValueLine,
                        ref result,
                        ref State
                        );
                    break;
                default:
                    Partial_ComputeNewState (
                        ch,
                        ParserState.EndOfValueLine,
                        ParserState.TagExpected,
                        ParserStateTransition.From_EndOfValueLine__To_TagExpected,
                        ref result,
                        ref State
                        );
                    break;
    
                }
                break;
            default:
                result = ParserResult.Error;
                break;
            }

            return result;
        }

    }


}

