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
        Indention,
        TagExpected,
        ObjectTag,
        ValueTag,
        EmptyTag,
        CommentTag,
        EndOfObjectTag,
        EndOfEmptyTag,
        EndOfValueTag,
        EndOfCommentTag,
        ValueLine,
        OtherValueLine,
        CommentLine,
        EmptyLine,
        EndOfValueLine,
        EndOfCommentLine,
        EndOfEmptyLine,
    }

    enum ParserStateTransition
    {
            From_Error__To_Error,
            From_Indention__To_Indention,
            From_Indention__To_TagExpected,
            From_Indention__To_ValueLine,
            From_Indention__To_OtherValueLine,
            From_Indention__To_Error,
            From_TagExpected__To_EndOfEmptyTag,
            From_TagExpected__To_ObjectTag,
            From_TagExpected__To_ValueTag,
            From_TagExpected__To_CommentTag,
            From_TagExpected__To_EmptyTag,
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
            From_EndOfObjectTag__To_Indention,
            From_EndOfEmptyTag__To_Indention,
            From_EndOfValueTag__To_Indention,
            From_EndOfCommentTag__To_Indention,
            From_ValueLine__To_EndOfValueLine,
            From_ValueLine__To_ValueLine,
            From_OtherValueLine__To_EndOfEmptyLine,
            From_OtherValueLine__To_CommentLine,
            From_OtherValueLine__To_EmptyLine,
            From_OtherValueLine__To_Error,
            From_CommentLine__To_EndOfCommentLine,
            From_CommentLine__To_CommentLine,
            From_EmptyLine__To_EndOfEmptyLine,
            From_EmptyLine__To_EmptyLine,
            From_EmptyLine__To_Error,
            From_EndOfValueLine__To_Indention,
            From_EndOfCommentLine__To_Indention,
            From_EndOfEmptyLine__To_Indention,
    }

    enum ParserStateChoice
    {
            From_Indention__Choose_TagExpected_ValueLine_OtherValueLine_Error,
    }

    enum ParserResult
    {
        Error   ,
        Continue,
    }

    sealed partial class Scanner
    {
        ParserState State = default (ParserState);

        partial void Partial_BeginLine (string l);
        partial void Partial_EndLine (string l);

        partial void Partial_StateChoice (
            char                    current     ,
            ParserStateChoice       choice      ,
            ParserState             from        ,
            ref ParserState         to
            );

        partial void Partial_StateTransition (
            char                    current     ,
            ParserState             from        ,
            ParserState             to          ,
            ParserStateTransition   transition  ,
            ref ParserResult        result      
            );
                
        public ParserResult AcceptLine (string l)
        {
            l = l ?? "";

            var result = ParserResult.Continue;
            var length = l.Length;

            Partial_BeginLine (l);

            for (var iter = 0; (iter < length) & (result == ParserResult.Continue); ++iter)
            {
                result = AcceptCharacter (l[iter]);
            }

            AcceptEndOfLine ();

            Partial_EndLine (l);

            return result;
        }

        const char EndOfStream = (char)0;

        ParserResult AcceptEndOfLine ()
        {
            var next = default (ParserState);
            var result = ParserResult.Continue; 
            switch (State)
            {
            case ParserState.TagExpected:
                next = ParserState.EndOfEmptyTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.TagExpected,
                    next,
                    ParserStateTransition.From_TagExpected__To_EndOfEmptyTag,
                    ref result
                    );

                break;
            case ParserState.ObjectTag:
                next = ParserState.EndOfObjectTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.ObjectTag,
                    next,
                    ParserStateTransition.From_ObjectTag__To_EndOfObjectTag,
                    ref result
                    );

                break;
            case ParserState.ValueTag:
                next = ParserState.EndOfValueTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.ValueTag,
                    next,
                    ParserStateTransition.From_ValueTag__To_EndOfValueTag,
                    ref result
                    );

                break;
            case ParserState.EmptyTag:
                next = ParserState.EndOfEmptyTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.EmptyTag,
                    next,
                    ParserStateTransition.From_EmptyTag__To_EndOfEmptyTag,
                    ref result
                    );

                break;
            case ParserState.CommentTag:
                next = ParserState.EndOfCommentTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.CommentTag,
                    next,
                    ParserStateTransition.From_CommentTag__To_EndOfCommentTag,
                    ref result
                    );

                break;
            case ParserState.ValueLine:
                next = ParserState.EndOfValueLine; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.ValueLine,
                    next,
                    ParserStateTransition.From_ValueLine__To_EndOfValueLine,
                    ref result
                    );

                break;
            case ParserState.OtherValueLine:
                next = ParserState.EndOfEmptyLine; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.OtherValueLine,
                    next,
                    ParserStateTransition.From_OtherValueLine__To_EndOfEmptyLine,
                    ref result
                    );

                break;
            case ParserState.CommentLine:
                next = ParserState.EndOfCommentLine; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.CommentLine,
                    next,
                    ParserStateTransition.From_CommentLine__To_EndOfCommentLine,
                    ref result
                    );

                break;
            case ParserState.EmptyLine:
                next = ParserState.EndOfEmptyLine; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.EmptyLine,
                    next,
                    ParserStateTransition.From_EmptyLine__To_EndOfEmptyLine,
                    ref result
                    );

                break;
            }

            State = next;

            return result;
        }

        ParserResult AcceptCharacter (char ch)
        {
            var next = default (ParserState);
            var result = ParserResult.Continue; 
            switch (State)
            {
            case ParserState.Error:
                switch (ch)
                {
                default:
                    next = ParserState.Error; 
                    Partial_StateTransition (
                        ch,
                        ParserState.Error,
                        next,
                        ParserStateTransition.From_Error__To_Error,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.Indention:
                switch (ch)
                {
                case '\t':
                    next = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.Indention,
                        next,
                        ParserStateTransition.From_Indention__To_Indention,
                        ref result
                        );
                    break;
                default:
                    Partial_StateChoice (
                        EndOfStream,
                        ParserStateChoice.From_Indention__Choose_TagExpected_ValueLine_OtherValueLine_Error,
                        ParserState.Indention,
                        ref next
                        );

                    switch (next)
                    {
                    case ParserState.TagExpected:
                        Partial_StateTransition (
                            EndOfStream,
                            ParserState.Indention,
                            next,
                            ParserStateTransition.From_Indention__To_TagExpected,
                            ref result
                            );
                        break;
                    case ParserState.ValueLine:
                        Partial_StateTransition (
                            EndOfStream,
                            ParserState.Indention,
                            next,
                            ParserStateTransition.From_Indention__To_ValueLine,
                            ref result
                            );
                        break;
                    case ParserState.OtherValueLine:
                        Partial_StateTransition (
                            EndOfStream,
                            ParserState.Indention,
                            next,
                            ParserStateTransition.From_Indention__To_OtherValueLine,
                            ref result
                            );
                        break;
                    case ParserState.Error:
                        Partial_StateTransition (
                            EndOfStream,
                            ParserState.Indention,
                            next,
                            ParserStateTransition.From_Indention__To_Error,
                            ref result
                            );
                        break;
                    default:
                        result = ParserResult.Error;
                        break;
                }
                    if (result == ParserResult.Continue)
                    {
                        AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.TagExpected:
                switch (ch)
                {
                case '@':
                    next = ParserState.ObjectTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.TagExpected,
                        next,
                        ParserStateTransition.From_TagExpected__To_ObjectTag,
                        ref result
                        );
                    break;
                case '=':
                    next = ParserState.ValueTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.TagExpected,
                        next,
                        ParserStateTransition.From_TagExpected__To_ValueTag,
                        ref result
                        );
                    break;
                case '#':
                    next = ParserState.CommentTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.TagExpected,
                        next,
                        ParserStateTransition.From_TagExpected__To_CommentTag,
                        ref result
                        );
                    break;
                case '\t':
                case ' ':
                    next = ParserState.EmptyTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.TagExpected,
                        next,
                        ParserStateTransition.From_TagExpected__To_EmptyTag,
                        ref result
                        );
                    break;
                default:
                    next = ParserState.Error; 
                    Partial_StateTransition (
                        ch,
                        ParserState.TagExpected,
                        next,
                        ParserStateTransition.From_TagExpected__To_Error,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.ObjectTag:
                switch (ch)
                {
                default:
                    next = ParserState.ObjectTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.ObjectTag,
                        next,
                        ParserStateTransition.From_ObjectTag__To_ObjectTag,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.ValueTag:
                switch (ch)
                {
                default:
                    next = ParserState.ValueTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.ValueTag,
                        next,
                        ParserStateTransition.From_ValueTag__To_ValueTag,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.EmptyTag:
                switch (ch)
                {
                case '\t':
                case ' ':
                    next = ParserState.EmptyTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EmptyTag,
                        next,
                        ParserStateTransition.From_EmptyTag__To_EmptyTag,
                        ref result
                        );
                    break;
                default:
                    next = ParserState.Error; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EmptyTag,
                        next,
                        ParserStateTransition.From_EmptyTag__To_Error,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.CommentTag:
                switch (ch)
                {
                default:
                    next = ParserState.CommentTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.CommentTag,
                        next,
                        ParserStateTransition.From_CommentTag__To_CommentTag,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.EndOfObjectTag:
                switch (ch)
                {
                default:
                    next = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfObjectTag,
                        next,
                        ParserStateTransition.From_EndOfObjectTag__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.EndOfEmptyTag:
                switch (ch)
                {
                default:
                    next = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfEmptyTag,
                        next,
                        ParserStateTransition.From_EndOfEmptyTag__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.EndOfValueTag:
                switch (ch)
                {
                default:
                    next = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfValueTag,
                        next,
                        ParserStateTransition.From_EndOfValueTag__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.EndOfCommentTag:
                switch (ch)
                {
                default:
                    next = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfCommentTag,
                        next,
                        ParserStateTransition.From_EndOfCommentTag__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.ValueLine:
                switch (ch)
                {
                default:
                    next = ParserState.ValueLine; 
                    Partial_StateTransition (
                        ch,
                        ParserState.ValueLine,
                        next,
                        ParserStateTransition.From_ValueLine__To_ValueLine,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.OtherValueLine:
                switch (ch)
                {
                case '#':
                    next = ParserState.CommentLine; 
                    Partial_StateTransition (
                        ch,
                        ParserState.OtherValueLine,
                        next,
                        ParserStateTransition.From_OtherValueLine__To_CommentLine,
                        ref result
                        );
                    break;
                case ' ':
                    next = ParserState.EmptyLine; 
                    Partial_StateTransition (
                        ch,
                        ParserState.OtherValueLine,
                        next,
                        ParserStateTransition.From_OtherValueLine__To_EmptyLine,
                        ref result
                        );
                    break;
                default:
                    next = ParserState.Error; 
                    Partial_StateTransition (
                        ch,
                        ParserState.OtherValueLine,
                        next,
                        ParserStateTransition.From_OtherValueLine__To_Error,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.CommentLine:
                switch (ch)
                {
                default:
                    next = ParserState.CommentLine; 
                    Partial_StateTransition (
                        ch,
                        ParserState.CommentLine,
                        next,
                        ParserStateTransition.From_CommentLine__To_CommentLine,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.EmptyLine:
                switch (ch)
                {
                case '\t':
                case ' ':
                    next = ParserState.EmptyLine; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EmptyLine,
                        next,
                        ParserStateTransition.From_EmptyLine__To_EmptyLine,
                        ref result
                        );
                    break;
                default:
                    next = ParserState.Error; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EmptyLine,
                        next,
                        ParserStateTransition.From_EmptyLine__To_Error,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.EndOfValueLine:
                switch (ch)
                {
                default:
                    next = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfValueLine,
                        next,
                        ParserStateTransition.From_EndOfValueLine__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.EndOfCommentLine:
                switch (ch)
                {
                default:
                    next = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfCommentLine,
                        next,
                        ParserStateTransition.From_EndOfCommentLine__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.EndOfEmptyLine:
                switch (ch)
                {
                default:
                    next = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfEmptyLine,
                        next,
                        ParserStateTransition.From_EndOfEmptyLine__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            default:
                result = ParserResult.Error;
                break;
            }

            State = next;

            return result;
        }

    }


}

