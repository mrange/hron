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
    using M3.HRON.Generator.Source.Common;

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
        EndOfValueLine,
    }

    enum ParserStateTransition
    {
            From_Error__To_Error,
            From_Indention__To_EndOfEmptyTag,
            From_Indention__To_Indention,
            From_Indention__To_TagExpected,
            From_Indention__To_ValueLine,
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
            From_EndOfValueLine__To_Indention,
    }

    enum ParserStateChoice
    {
            From_Indention__Choose_TagExpected_ValueLine_Error,
    }

    enum ParserResult
    {
        Error   ,
        Continue,
    }

    sealed partial class Scanner
    {
        ParserState State = default (ParserState);

        partial void Partial_BeginLine (SubString ss);
        partial void Partial_EndLine ();

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

        partial void Partial_AcceptEndOfStream ();
                
        public ParserResult AcceptEndOfStream ()
        {
            Partial_AcceptEndOfStream ();
            return ParserResult.Continue;             
        }

        public ParserResult AcceptLine (SubString ss)
        {
            var result = ParserResult.Continue;

            Partial_BeginLine (ss);

            var bs = ss.BaseString;
            var begin = ss.Begin;
            var end = ss.End;

            for (var iter = begin; (iter < end) & (result == ParserResult.Continue); ++iter)
            {
                result = AcceptCharacter (bs[iter]);
            }

            AcceptEndOfLine ();

            Partial_EndLine ();

            return result;
        }

        const char EndOfStream = (char)0;

        ParserResult AcceptEndOfLine ()
        {
            var result = ParserResult.Continue; 
            switch (State)
            {
            case ParserState.Indention:
                State = ParserState.EndOfEmptyTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.Indention,
                    ParserState.EndOfEmptyTag,
                    ParserStateTransition.From_Indention__To_EndOfEmptyTag,
                    ref result
                    );

                break;
            case ParserState.TagExpected:
                State = ParserState.EndOfEmptyTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.TagExpected,
                    ParserState.EndOfEmptyTag,
                    ParserStateTransition.From_TagExpected__To_EndOfEmptyTag,
                    ref result
                    );

                break;
            case ParserState.ObjectTag:
                State = ParserState.EndOfObjectTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.ObjectTag,
                    ParserState.EndOfObjectTag,
                    ParserStateTransition.From_ObjectTag__To_EndOfObjectTag,
                    ref result
                    );

                break;
            case ParserState.ValueTag:
                State = ParserState.EndOfValueTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.ValueTag,
                    ParserState.EndOfValueTag,
                    ParserStateTransition.From_ValueTag__To_EndOfValueTag,
                    ref result
                    );

                break;
            case ParserState.EmptyTag:
                State = ParserState.EndOfEmptyTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.EmptyTag,
                    ParserState.EndOfEmptyTag,
                    ParserStateTransition.From_EmptyTag__To_EndOfEmptyTag,
                    ref result
                    );

                break;
            case ParserState.CommentTag:
                State = ParserState.EndOfCommentTag; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.CommentTag,
                    ParserState.EndOfCommentTag,
                    ParserStateTransition.From_CommentTag__To_EndOfCommentTag,
                    ref result
                    );

                break;
            case ParserState.ValueLine:
                State = ParserState.EndOfValueLine; 
                Partial_StateTransition (
                    EndOfStream,
                    ParserState.ValueLine,
                    ParserState.EndOfValueLine,
                    ParserStateTransition.From_ValueLine__To_EndOfValueLine,
                    ref result
                    );

                break;
            }

            return result;
        }

        ParserResult AcceptCharacter (char ch)
        {
            var result = ParserResult.Continue; 
            switch (State)
            {
            case ParserState.Error:
                switch (ch)
                {
                default:
                    State = ParserState.Error; 
                    Partial_StateTransition (
                        ch,
                        ParserState.Error,
                        ParserState.Error,
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
                    State = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.Indention,
                        ParserState.Indention,
                        ParserStateTransition.From_Indention__To_Indention,
                        ref result
                        );
                    break;
                default:
                    Partial_StateChoice (
                        ch,
                        ParserStateChoice.From_Indention__Choose_TagExpected_ValueLine_Error,
                        ParserState.Indention,
                        ref State
                        );

                    switch (State)
                    {
                    case ParserState.TagExpected:
                        Partial_StateTransition (
                            ch,
                            ParserState.Indention,
                            State,
                            ParserStateTransition.From_Indention__To_TagExpected,
                            ref result
                            );
                        break;
                    case ParserState.ValueLine:
                        Partial_StateTransition (
                            ch,
                            ParserState.Indention,
                            State,
                            ParserStateTransition.From_Indention__To_ValueLine,
                            ref result
                            );
                        break;
                    case ParserState.Error:
                        Partial_StateTransition (
                            ch,
                            ParserState.Indention,
                            State,
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
                        result = AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.TagExpected:
                switch (ch)
                {
                case '@':
                    State = ParserState.ObjectTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.TagExpected,
                        ParserState.ObjectTag,
                        ParserStateTransition.From_TagExpected__To_ObjectTag,
                        ref result
                        );
                    break;
                case '=':
                    State = ParserState.ValueTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.TagExpected,
                        ParserState.ValueTag,
                        ParserStateTransition.From_TagExpected__To_ValueTag,
                        ref result
                        );
                    break;
                case '#':
                    State = ParserState.CommentTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.TagExpected,
                        ParserState.CommentTag,
                        ParserStateTransition.From_TagExpected__To_CommentTag,
                        ref result
                        );
                    break;
                case '\t':
                case ' ':
                    State = ParserState.EmptyTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.TagExpected,
                        ParserState.EmptyTag,
                        ParserStateTransition.From_TagExpected__To_EmptyTag,
                        ref result
                        );
                    break;
                default:
                    State = ParserState.Error; 
                    Partial_StateTransition (
                        ch,
                        ParserState.TagExpected,
                        ParserState.Error,
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
                    State = ParserState.ObjectTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.ObjectTag,
                        ParserState.ObjectTag,
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
                    State = ParserState.ValueTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.ValueTag,
                        ParserState.ValueTag,
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
                    State = ParserState.EmptyTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EmptyTag,
                        ParserState.EmptyTag,
                        ParserStateTransition.From_EmptyTag__To_EmptyTag,
                        ref result
                        );
                    break;
                default:
                    State = ParserState.Error; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EmptyTag,
                        ParserState.Error,
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
                    State = ParserState.CommentTag; 
                    Partial_StateTransition (
                        ch,
                        ParserState.CommentTag,
                        ParserState.CommentTag,
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
                    State = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfObjectTag,
                        ParserState.Indention,
                        ParserStateTransition.From_EndOfObjectTag__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        result = AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.EndOfEmptyTag:
                switch (ch)
                {
                default:
                    State = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfEmptyTag,
                        ParserState.Indention,
                        ParserStateTransition.From_EndOfEmptyTag__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        result = AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.EndOfValueTag:
                switch (ch)
                {
                default:
                    State = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfValueTag,
                        ParserState.Indention,
                        ParserStateTransition.From_EndOfValueTag__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        result = AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.EndOfCommentTag:
                switch (ch)
                {
                default:
                    State = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfCommentTag,
                        ParserState.Indention,
                        ParserStateTransition.From_EndOfCommentTag__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        result = AcceptCharacter (ch);    
                    }   
                    break;
    
                }
                break;
            case ParserState.ValueLine:
                switch (ch)
                {
                default:
                    State = ParserState.ValueLine; 
                    Partial_StateTransition (
                        ch,
                        ParserState.ValueLine,
                        ParserState.ValueLine,
                        ParserStateTransition.From_ValueLine__To_ValueLine,
                        ref result
                        );
                    break;
    
                }
                break;
            case ParserState.EndOfValueLine:
                switch (ch)
                {
                default:
                    State = ParserState.Indention; 
                    Partial_StateTransition (
                        ch,
                        ParserState.EndOfValueLine,
                        ParserState.Indention,
                        ParserStateTransition.From_EndOfValueLine__To_Indention,
                        ref result
                        );
                    if (result == ParserResult.Continue)
                    {
                        result = AcceptCharacter (ch);    
                    }   
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

