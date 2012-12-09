

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
        PreProcessing,
        Indention,
        TagExpected,
        NoContentTagExpected,
        PreProcessorTag,
        ObjectTag,
        ValueTag,
        EmptyTag,
        CommentTag,
        EndOfPreProcessorTag,
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
        Done    ,
    }


    sealed partial class Scanner
    {
        ParserState State = default (ParserState);
        const char EndOfStream = (char)0;

        partial void Partial_BeginLine ();
        partial void Partial_EndLine ();

        partial void Partial_StateChoice__From_Indention__Choose_TagExpected_NoContentTagExpected_ValueLine_Error ();

        partial void Partial_StateTransition__From_Error ();

        partial void Partial_StateTransition__To_Error ();

        partial void Partial_StateTransition__From_Error__To_Error ();
        partial void Partial_StateTransition__From_PreProcessing ();

        partial void Partial_StateTransition__To_PreProcessing ();

        partial void Partial_StateTransition__From_PreProcessing__To_PreProcessorTag ();
        partial void Partial_StateTransition__From_PreProcessing__To_Indention ();
        partial void Partial_StateTransition__From_Indention ();

        partial void Partial_StateTransition__To_Indention ();

        partial void Partial_StateTransition__From_Indention__To_EndOfEmptyTag ();
        partial void Partial_StateTransition__From_Indention__To_Indention ();
        partial void Partial_StateTransition__From_Indention__To_TagExpected ();
        partial void Partial_StateTransition__From_Indention__To_NoContentTagExpected ();
        partial void Partial_StateTransition__From_Indention__To_ValueLine ();
        partial void Partial_StateTransition__From_Indention__To_Error ();
        partial void Partial_StateTransition__From_TagExpected ();

        partial void Partial_StateTransition__To_TagExpected ();

        partial void Partial_StateTransition__From_TagExpected__To_EndOfEmptyTag ();
        partial void Partial_StateTransition__From_TagExpected__To_ObjectTag ();
        partial void Partial_StateTransition__From_TagExpected__To_ValueTag ();
        partial void Partial_StateTransition__From_TagExpected__To_CommentTag ();
        partial void Partial_StateTransition__From_TagExpected__To_EmptyTag ();
        partial void Partial_StateTransition__From_TagExpected__To_Error ();
        partial void Partial_StateTransition__From_NoContentTagExpected ();

        partial void Partial_StateTransition__To_NoContentTagExpected ();

        partial void Partial_StateTransition__From_NoContentTagExpected__To_EndOfEmptyTag ();
        partial void Partial_StateTransition__From_NoContentTagExpected__To_CommentTag ();
        partial void Partial_StateTransition__From_NoContentTagExpected__To_EmptyTag ();
        partial void Partial_StateTransition__From_NoContentTagExpected__To_Error ();
        partial void Partial_StateTransition__From_PreProcessorTag ();

        partial void Partial_StateTransition__To_PreProcessorTag ();

        partial void Partial_StateTransition__From_PreProcessorTag__To_EndOfPreProcessorTag ();
        partial void Partial_StateTransition__From_PreProcessorTag__To_PreProcessorTag ();
        partial void Partial_StateTransition__From_ObjectTag ();

        partial void Partial_StateTransition__To_ObjectTag ();

        partial void Partial_StateTransition__From_ObjectTag__To_EndOfObjectTag ();
        partial void Partial_StateTransition__From_ObjectTag__To_ObjectTag ();
        partial void Partial_StateTransition__From_ValueTag ();

        partial void Partial_StateTransition__To_ValueTag ();

        partial void Partial_StateTransition__From_ValueTag__To_EndOfValueTag ();
        partial void Partial_StateTransition__From_ValueTag__To_ValueTag ();
        partial void Partial_StateTransition__From_EmptyTag ();

        partial void Partial_StateTransition__To_EmptyTag ();

        partial void Partial_StateTransition__From_EmptyTag__To_EndOfEmptyTag ();
        partial void Partial_StateTransition__From_EmptyTag__To_EmptyTag ();
        partial void Partial_StateTransition__From_EmptyTag__To_Error ();
        partial void Partial_StateTransition__From_CommentTag ();

        partial void Partial_StateTransition__To_CommentTag ();

        partial void Partial_StateTransition__From_CommentTag__To_EndOfCommentTag ();
        partial void Partial_StateTransition__From_CommentTag__To_CommentTag ();
        partial void Partial_StateTransition__From_EndOfPreProcessorTag ();

        partial void Partial_StateTransition__To_EndOfPreProcessorTag ();

        partial void Partial_StateTransition__From_EndOfPreProcessorTag__To_PreProcessing ();
        partial void Partial_StateTransition__From_EndOfObjectTag ();

        partial void Partial_StateTransition__To_EndOfObjectTag ();

        partial void Partial_StateTransition__From_EndOfObjectTag__To_Indention ();
        partial void Partial_StateTransition__From_EndOfEmptyTag ();

        partial void Partial_StateTransition__To_EndOfEmptyTag ();

        partial void Partial_StateTransition__From_EndOfEmptyTag__To_Indention ();
        partial void Partial_StateTransition__From_EndOfValueTag ();

        partial void Partial_StateTransition__To_EndOfValueTag ();

        partial void Partial_StateTransition__From_EndOfValueTag__To_Indention ();
        partial void Partial_StateTransition__From_EndOfCommentTag ();

        partial void Partial_StateTransition__To_EndOfCommentTag ();

        partial void Partial_StateTransition__From_EndOfCommentTag__To_Indention ();
        partial void Partial_StateTransition__From_ValueLine ();

        partial void Partial_StateTransition__To_ValueLine ();

        partial void Partial_StateTransition__From_ValueLine__To_EndOfValueLine ();
        partial void Partial_StateTransition__From_ValueLine__To_ValueLine ();
        partial void Partial_StateTransition__From_EndOfValueLine ();

        partial void Partial_StateTransition__To_EndOfValueLine ();

        partial void Partial_StateTransition__From_EndOfValueLine__To_Indention ();

        partial void Partial_AcceptEndOfStream ();
                
        ParserResult Result;
        SubString    CurrentLine;
        char         CurrentCharacter;

        public void AcceptEndOfStream ()
        {
            Partial_AcceptEndOfStream ();
        }

        public bool AcceptLine (SubString ss)
        {
            Result = ParserResult.Continue;
            CurrentLine = ss;

            Partial_BeginLine ();

            var bs = ss.BaseString;
            var begin = ss.Begin;
            var end = ss.End;

            for (var iter = begin; iter < end; ++iter)
            {
                CurrentCharacter = bs[iter];
apply:
                if (Result != ParserResult.Continue)
                {
                    break;
                }

                switch (State)
                {
                case ParserState.Error:
                    switch (CurrentCharacter)
                    {
                    default:
                            Partial_StateTransition__From_Error ();
                            Partial_StateTransition__From_Error__To_Error ();
                            Partial_StateTransition__To_Error ();
                        break;
    
                    }
                    break;
                case ParserState.PreProcessing:
                    switch (CurrentCharacter)
                    {
                    case '!':
                        State = ParserState.PreProcessorTag; 
                            Partial_StateTransition__From_PreProcessing ();
                            Partial_StateTransition__From_PreProcessing__To_PreProcessorTag ();
                            Partial_StateTransition__To_PreProcessorTag ();
                        break;
                    default:
                        State = ParserState.Indention; 
                            Partial_StateTransition__From_PreProcessing ();
                            Partial_StateTransition__From_PreProcessing__To_Indention ();
                            Partial_StateTransition__To_Indention ();
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.Indention:
                    switch (CurrentCharacter)
                    {
                    case '\t':
                            Partial_StateTransition__From_Indention ();
                            Partial_StateTransition__From_Indention__To_Indention ();
                            Partial_StateTransition__To_Indention ();
                        break;
                    default:
                    Partial_StateChoice__From_Indention__Choose_TagExpected_NoContentTagExpected_ValueLine_Error ();

                        switch (State)
                        {
                        case ParserState.TagExpected:
                            Partial_StateTransition__From_Indention ();
                            Partial_StateTransition__From_Indention__To_TagExpected ();
                            Partial_StateTransition__To_TagExpected ();
                            break;
                        case ParserState.NoContentTagExpected:
                            Partial_StateTransition__From_Indention ();
                            Partial_StateTransition__From_Indention__To_NoContentTagExpected ();
                            Partial_StateTransition__To_NoContentTagExpected ();
                            break;
                        case ParserState.ValueLine:
                            Partial_StateTransition__From_Indention ();
                            Partial_StateTransition__From_Indention__To_ValueLine ();
                            Partial_StateTransition__To_ValueLine ();
                            break;
                        case ParserState.Error:
                            Partial_StateTransition__From_Indention ();
                            Partial_StateTransition__From_Indention__To_Error ();
                            Partial_StateTransition__To_Error ();
                            break;
                        default:
                            Result = ParserResult.Error;
                            break;
                    }
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.TagExpected:
                    switch (CurrentCharacter)
                    {
                    case '@':
                        State = ParserState.ObjectTag; 
                            Partial_StateTransition__From_TagExpected ();
                            Partial_StateTransition__From_TagExpected__To_ObjectTag ();
                            Partial_StateTransition__To_ObjectTag ();
                        break;
                    case '=':
                        State = ParserState.ValueTag; 
                            Partial_StateTransition__From_TagExpected ();
                            Partial_StateTransition__From_TagExpected__To_ValueTag ();
                            Partial_StateTransition__To_ValueTag ();
                        break;
                    case '#':
                        State = ParserState.CommentTag; 
                            Partial_StateTransition__From_TagExpected ();
                            Partial_StateTransition__From_TagExpected__To_CommentTag ();
                            Partial_StateTransition__To_CommentTag ();
                        break;
                    case '\t':
                    case ' ':
                        State = ParserState.EmptyTag; 
                            Partial_StateTransition__From_TagExpected ();
                            Partial_StateTransition__From_TagExpected__To_EmptyTag ();
                            Partial_StateTransition__To_EmptyTag ();
                        break;
                    default:
                        State = ParserState.Error; 
                            Partial_StateTransition__From_TagExpected ();
                            Partial_StateTransition__From_TagExpected__To_Error ();
                            Partial_StateTransition__To_Error ();
                        break;
    
                    }
                    break;
                case ParserState.NoContentTagExpected:
                    switch (CurrentCharacter)
                    {
                    case '#':
                        State = ParserState.CommentTag; 
                            Partial_StateTransition__From_NoContentTagExpected ();
                            Partial_StateTransition__From_NoContentTagExpected__To_CommentTag ();
                            Partial_StateTransition__To_CommentTag ();
                        break;
                    case '\t':
                    case ' ':
                        State = ParserState.EmptyTag; 
                            Partial_StateTransition__From_NoContentTagExpected ();
                            Partial_StateTransition__From_NoContentTagExpected__To_EmptyTag ();
                            Partial_StateTransition__To_EmptyTag ();
                        break;
                    default:
                        State = ParserState.Error; 
                            Partial_StateTransition__From_NoContentTagExpected ();
                            Partial_StateTransition__From_NoContentTagExpected__To_Error ();
                            Partial_StateTransition__To_Error ();
                        break;
    
                    }
                    break;
                case ParserState.PreProcessorTag:
                    switch (CurrentCharacter)
                    {
                    default:
                            Partial_StateTransition__From_PreProcessorTag ();
                            Partial_StateTransition__From_PreProcessorTag__To_PreProcessorTag ();
                            Partial_StateTransition__To_PreProcessorTag ();
                        break;
    
                    }
                    break;
                case ParserState.ObjectTag:
                    switch (CurrentCharacter)
                    {
                    default:
                            Partial_StateTransition__From_ObjectTag ();
                            Partial_StateTransition__From_ObjectTag__To_ObjectTag ();
                            Partial_StateTransition__To_ObjectTag ();
                        break;
    
                    }
                    break;
                case ParserState.ValueTag:
                    switch (CurrentCharacter)
                    {
                    default:
                            Partial_StateTransition__From_ValueTag ();
                            Partial_StateTransition__From_ValueTag__To_ValueTag ();
                            Partial_StateTransition__To_ValueTag ();
                        break;
    
                    }
                    break;
                case ParserState.EmptyTag:
                    switch (CurrentCharacter)
                    {
                    case '\t':
                    case ' ':
                            Partial_StateTransition__From_EmptyTag ();
                            Partial_StateTransition__From_EmptyTag__To_EmptyTag ();
                            Partial_StateTransition__To_EmptyTag ();
                        break;
                    default:
                        State = ParserState.Error; 
                            Partial_StateTransition__From_EmptyTag ();
                            Partial_StateTransition__From_EmptyTag__To_Error ();
                            Partial_StateTransition__To_Error ();
                        break;
    
                    }
                    break;
                case ParserState.CommentTag:
                    switch (CurrentCharacter)
                    {
                    default:
                            Partial_StateTransition__From_CommentTag ();
                            Partial_StateTransition__From_CommentTag__To_CommentTag ();
                            Partial_StateTransition__To_CommentTag ();
                        break;
    
                    }
                    break;
                case ParserState.EndOfPreProcessorTag:
                    switch (CurrentCharacter)
                    {
                    default:
                        State = ParserState.PreProcessing; 
                            Partial_StateTransition__From_EndOfPreProcessorTag ();
                            Partial_StateTransition__From_EndOfPreProcessorTag__To_PreProcessing ();
                            Partial_StateTransition__To_PreProcessing ();
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.EndOfObjectTag:
                    switch (CurrentCharacter)
                    {
                    default:
                        State = ParserState.Indention; 
                            Partial_StateTransition__From_EndOfObjectTag ();
                            Partial_StateTransition__From_EndOfObjectTag__To_Indention ();
                            Partial_StateTransition__To_Indention ();
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.EndOfEmptyTag:
                    switch (CurrentCharacter)
                    {
                    default:
                        State = ParserState.Indention; 
                            Partial_StateTransition__From_EndOfEmptyTag ();
                            Partial_StateTransition__From_EndOfEmptyTag__To_Indention ();
                            Partial_StateTransition__To_Indention ();
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.EndOfValueTag:
                    switch (CurrentCharacter)
                    {
                    default:
                        State = ParserState.Indention; 
                            Partial_StateTransition__From_EndOfValueTag ();
                            Partial_StateTransition__From_EndOfValueTag__To_Indention ();
                            Partial_StateTransition__To_Indention ();
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.EndOfCommentTag:
                    switch (CurrentCharacter)
                    {
                    default:
                        State = ParserState.Indention; 
                            Partial_StateTransition__From_EndOfCommentTag ();
                            Partial_StateTransition__From_EndOfCommentTag__To_Indention ();
                            Partial_StateTransition__To_Indention ();
                        goto apply;
                        break;
    
                    }
                    break;
                case ParserState.ValueLine:
                    switch (CurrentCharacter)
                    {
                    default:
                            Partial_StateTransition__From_ValueLine ();
                            Partial_StateTransition__From_ValueLine__To_ValueLine ();
                            Partial_StateTransition__To_ValueLine ();
                        break;
    
                    }
                    break;
                case ParserState.EndOfValueLine:
                    switch (CurrentCharacter)
                    {
                    default:
                        State = ParserState.Indention; 
                            Partial_StateTransition__From_EndOfValueLine ();
                            Partial_StateTransition__From_EndOfValueLine__To_Indention ();
                            Partial_StateTransition__To_Indention ();
                        goto apply;
                        break;
    
                    }
                    break;
                default:
                    Result = ParserResult.Error;
                    break;
                }
            }

            if (Result == ParserResult.Error)
            {
                return false;
            }

            // EndOfLine
            CurrentCharacter = EndOfStream;

            {
                switch (State)
                {
                case ParserState.Indention:
                    State = ParserState.EndOfEmptyTag; 
                            Partial_StateTransition__From_Indention ();
                            Partial_StateTransition__From_Indention__To_EndOfEmptyTag ();
                            Partial_StateTransition__To_EndOfEmptyTag ();
                    break;
                case ParserState.TagExpected:
                    State = ParserState.EndOfEmptyTag; 
                            Partial_StateTransition__From_TagExpected ();
                            Partial_StateTransition__From_TagExpected__To_EndOfEmptyTag ();
                            Partial_StateTransition__To_EndOfEmptyTag ();
                    break;
                case ParserState.NoContentTagExpected:
                    State = ParserState.EndOfEmptyTag; 
                            Partial_StateTransition__From_NoContentTagExpected ();
                            Partial_StateTransition__From_NoContentTagExpected__To_EndOfEmptyTag ();
                            Partial_StateTransition__To_EndOfEmptyTag ();
                    break;
                case ParserState.PreProcessorTag:
                    State = ParserState.EndOfPreProcessorTag; 
                            Partial_StateTransition__From_PreProcessorTag ();
                            Partial_StateTransition__From_PreProcessorTag__To_EndOfPreProcessorTag ();
                            Partial_StateTransition__To_EndOfPreProcessorTag ();
                    break;
                case ParserState.ObjectTag:
                    State = ParserState.EndOfObjectTag; 
                            Partial_StateTransition__From_ObjectTag ();
                            Partial_StateTransition__From_ObjectTag__To_EndOfObjectTag ();
                            Partial_StateTransition__To_EndOfObjectTag ();
                    break;
                case ParserState.ValueTag:
                    State = ParserState.EndOfValueTag; 
                            Partial_StateTransition__From_ValueTag ();
                            Partial_StateTransition__From_ValueTag__To_EndOfValueTag ();
                            Partial_StateTransition__To_EndOfValueTag ();
                    break;
                case ParserState.EmptyTag:
                    State = ParserState.EndOfEmptyTag; 
                            Partial_StateTransition__From_EmptyTag ();
                            Partial_StateTransition__From_EmptyTag__To_EndOfEmptyTag ();
                            Partial_StateTransition__To_EndOfEmptyTag ();
                    break;
                case ParserState.CommentTag:
                    State = ParserState.EndOfCommentTag; 
                            Partial_StateTransition__From_CommentTag ();
                            Partial_StateTransition__From_CommentTag__To_EndOfCommentTag ();
                            Partial_StateTransition__To_EndOfCommentTag ();
                    break;
                case ParserState.ValueLine:
                    State = ParserState.EndOfValueLine; 
                            Partial_StateTransition__From_ValueLine ();
                            Partial_StateTransition__From_ValueLine__To_EndOfValueLine ();
                            Partial_StateTransition__To_EndOfValueLine ();
                    break;
                }
            }

            Partial_EndLine ();

            return Result != ParserResult.Error;
        }
    }
}





