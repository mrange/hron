

// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # template file (.tt)                                                      #
// ############################################################################





#pragma warning disable 0162
// ReSharper disable CheckNamespace
// ReSharper disable CSharpWarnings::CS0162
// ReSharper disable HeuristicUnreachableCode
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart


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

    enum ParserResult
    {
        Error   ,
        Continue,
    }


    sealed partial class Scanner
    {
        ParserState State = default (ParserState);
        const char EndOfStream = (char)0;

        partial void Partial_BeginLine (SubString ss);
        partial void Partial_EndLine ();

        partial void Partial_StateChoice__From_Indention__Choose_TagExpected_ValueLine_Error (
            char                    current     ,
            ref ParserState         to
            );

        partial void Partial_StateTransition__From_Error (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_Error (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_Error__To_Error (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_Indention (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_Indention (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_Indention__To_EndOfEmptyTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_Indention__To_Indention (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_Indention__To_TagExpected (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_Indention__To_ValueLine (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_Indention__To_Error (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_TagExpected (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_TagExpected (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_TagExpected__To_EndOfEmptyTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_TagExpected__To_ObjectTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_TagExpected__To_ValueTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_TagExpected__To_CommentTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_TagExpected__To_EmptyTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_TagExpected__To_Error (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_ObjectTag (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_ObjectTag (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_ObjectTag__To_EndOfObjectTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_ObjectTag__To_ObjectTag (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_ValueTag (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_ValueTag (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_ValueTag__To_EndOfValueTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_ValueTag__To_ValueTag (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_EmptyTag (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_EmptyTag (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_EmptyTag__To_EndOfEmptyTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_EmptyTag__To_EmptyTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_EmptyTag__To_Error (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_CommentTag (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_CommentTag (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_CommentTag__To_EndOfCommentTag (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_CommentTag__To_CommentTag (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_EndOfObjectTag (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_EndOfObjectTag (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_EndOfObjectTag__To_Indention (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_EndOfEmptyTag (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_EndOfEmptyTag (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_EndOfEmptyTag__To_Indention (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_EndOfValueTag (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_EndOfValueTag (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_EndOfValueTag__To_Indention (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_EndOfCommentTag (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_EndOfCommentTag (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_EndOfCommentTag__To_Indention (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_ValueLine (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_ValueLine (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_ValueLine__To_EndOfValueLine (
                char                    current     ,
                ref ParserResult        result      
                );
            partial void Partial_StateTransition__From_ValueLine__To_ValueLine (
                char                    current     ,
                ref ParserResult        result      
                );
        partial void Partial_StateTransition__From_EndOfValueLine (
            char                    current     ,
            ref ParserResult        result      
            );

        partial void Partial_StateTransition__To_EndOfValueLine (
            char                    current     ,
            ref ParserResult        result      
            );

            partial void Partial_StateTransition__From_EndOfValueLine__To_Indention (
                char                    current     ,
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
            char ch;

            for (var iter = begin; iter < end; ++iter)
            {
                ch = bs[iter];
apply:
                if (result != ParserResult.Continue)
                {
                    return result;
                }

                switch (State)
                {
                case ParserState.Error:
                    switch (ch)
                    {
                    default:
                            Partial_StateTransition__From_Error (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_Error__To_Error (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_Error (
                                ch,
                                ref result
                                );
                        break;
    
                    }
                    break;
                case ParserState.Indention:
                    switch (ch)
                    {
                    case '\t':
                            Partial_StateTransition__From_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_Indention__To_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_Indention (
                                ch,
                                ref result
                                );
                        break;
                    default:
                    Partial_StateChoice__From_Indention__Choose_TagExpected_ValueLine_Error (
                        ch,
                        ref State
                        );

                        switch (State)
                        {
                        case ParserState.TagExpected:
                            Partial_StateTransition__From_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_Indention__To_TagExpected (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_TagExpected (
                                ch,
                                ref result
                                );
                            break;
                        case ParserState.ValueLine:
                            Partial_StateTransition__From_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_Indention__To_ValueLine (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_ValueLine (
                                ch,
                                ref result
                                );
                            break;
                        case ParserState.Error:
                            Partial_StateTransition__From_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_Indention__To_Error (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_Error (
                                ch,
                                ref result
                                );
                            break;
                        default:
                            result = ParserResult.Error;
                            break;
                    }
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.TagExpected:
                    switch (ch)
                    {
                    case '@':
                        State = ParserState.ObjectTag; 
                            Partial_StateTransition__From_TagExpected (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_TagExpected__To_ObjectTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_ObjectTag (
                                ch,
                                ref result
                                );
                        break;
                    case '=':
                        State = ParserState.ValueTag; 
                            Partial_StateTransition__From_TagExpected (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_TagExpected__To_ValueTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_ValueTag (
                                ch,
                                ref result
                                );
                        break;
                    case '#':
                        State = ParserState.CommentTag; 
                            Partial_StateTransition__From_TagExpected (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_TagExpected__To_CommentTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_CommentTag (
                                ch,
                                ref result
                                );
                        break;
                    case '\t':
                    case ' ':
                        State = ParserState.EmptyTag; 
                            Partial_StateTransition__From_TagExpected (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_TagExpected__To_EmptyTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_EmptyTag (
                                ch,
                                ref result
                                );
                        break;
                    default:
                        State = ParserState.Error; 
                            Partial_StateTransition__From_TagExpected (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_TagExpected__To_Error (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_Error (
                                ch,
                                ref result
                                );
                        break;
    
                    }
                    break;
                case ParserState.ObjectTag:
                    switch (ch)
                    {
                    default:
                            Partial_StateTransition__From_ObjectTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_ObjectTag__To_ObjectTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_ObjectTag (
                                ch,
                                ref result
                                );
                        break;
    
                    }
                    break;
                case ParserState.ValueTag:
                    switch (ch)
                    {
                    default:
                            Partial_StateTransition__From_ValueTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_ValueTag__To_ValueTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_ValueTag (
                                ch,
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
                            Partial_StateTransition__From_EmptyTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_EmptyTag__To_EmptyTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_EmptyTag (
                                ch,
                                ref result
                                );
                        break;
                    default:
                        State = ParserState.Error; 
                            Partial_StateTransition__From_EmptyTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_EmptyTag__To_Error (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_Error (
                                ch,
                                ref result
                                );
                        break;
    
                    }
                    break;
                case ParserState.CommentTag:
                    switch (ch)
                    {
                    default:
                            Partial_StateTransition__From_CommentTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_CommentTag__To_CommentTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_CommentTag (
                                ch,
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
                            Partial_StateTransition__From_EndOfObjectTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_EndOfObjectTag__To_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_Indention (
                                ch,
                                ref result
                                );
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.EndOfEmptyTag:
                    switch (ch)
                    {
                    default:
                        State = ParserState.Indention; 
                            Partial_StateTransition__From_EndOfEmptyTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_EndOfEmptyTag__To_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_Indention (
                                ch,
                                ref result
                                );
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.EndOfValueTag:
                    switch (ch)
                    {
                    default:
                        State = ParserState.Indention; 
                            Partial_StateTransition__From_EndOfValueTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_EndOfValueTag__To_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_Indention (
                                ch,
                                ref result
                                );
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.EndOfCommentTag:
                    switch (ch)
                    {
                    default:
                        State = ParserState.Indention; 
                            Partial_StateTransition__From_EndOfCommentTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_EndOfCommentTag__To_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_Indention (
                                ch,
                                ref result
                                );
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.ValueLine:
                    switch (ch)
                    {
                    default:
                            Partial_StateTransition__From_ValueLine (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_ValueLine__To_ValueLine (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_ValueLine (
                                ch,
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
                            Partial_StateTransition__From_EndOfValueLine (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_EndOfValueLine__To_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_Indention (
                                ch,
                                ref result
                                );
                        goto apply;
                        break;
    
                    }
                    break;
                default:
                    result = ParserResult.Error;
                    break;
                }
            }

            if (result != ParserResult.Continue)
            {
                return result;
            }

            // EndOfLine
            ch = EndOfStream;

            {
                switch (State)
                {
                case ParserState.Indention:
                    State = ParserState.EndOfEmptyTag; 
                            Partial_StateTransition__From_Indention (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_Indention__To_EndOfEmptyTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_EndOfEmptyTag (
                                ch,
                                ref result
                                );
                    break;
                case ParserState.TagExpected:
                    State = ParserState.EndOfEmptyTag; 
                            Partial_StateTransition__From_TagExpected (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_TagExpected__To_EndOfEmptyTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_EndOfEmptyTag (
                                ch,
                                ref result
                                );
                    break;
                case ParserState.ObjectTag:
                    State = ParserState.EndOfObjectTag; 
                            Partial_StateTransition__From_ObjectTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_ObjectTag__To_EndOfObjectTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_EndOfObjectTag (
                                ch,
                                ref result
                                );
                    break;
                case ParserState.ValueTag:
                    State = ParserState.EndOfValueTag; 
                            Partial_StateTransition__From_ValueTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_ValueTag__To_EndOfValueTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_EndOfValueTag (
                                ch,
                                ref result
                                );
                    break;
                case ParserState.EmptyTag:
                    State = ParserState.EndOfEmptyTag; 
                            Partial_StateTransition__From_EmptyTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_EmptyTag__To_EndOfEmptyTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_EndOfEmptyTag (
                                ch,
                                ref result
                                );
                    break;
                case ParserState.CommentTag:
                    State = ParserState.EndOfCommentTag; 
                            Partial_StateTransition__From_CommentTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_CommentTag__To_EndOfCommentTag (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_EndOfCommentTag (
                                ch,
                                ref result
                                );
                    break;
                case ParserState.ValueLine:
                    State = ParserState.EndOfValueLine; 
                            Partial_StateTransition__From_ValueLine (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__From_ValueLine__To_EndOfValueLine (
                                ch,
                                ref result
                                );
                            Partial_StateTransition__To_EndOfValueLine (
                                ch,
                                ref result
                                );
                    break;
                }
            }

            Partial_EndLine ();

            return result;
        }
    }
}





