// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # template file (.tt)                                                      #
// ############################################################################






namespace M3.HRON.Generator.Parser
{
    enum ParserState
    {
        TagExpected,
        ObjectTag,
        ValueTag,
        CommentTag,
        ValueExpected,
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
        }

        public ParserResult AcceptCharacter (char ch)
        {
            switch (State)
            {
            case ParserState.TagExpected:
                switch (ch)
                {
                case '\t':
                    State = ParserState.TagExpected;
                    return ParserResult.Continue;
                case '@':
                    State = ParserState.ObjectTag;
                    return ParserResult.Continue;
                case '=':
                    State = ParserState.ValueTag;
                    return ParserResult.Continue;
                case '#':
                    State = ParserState.CommentTag;
                    return ParserResult.Continue;
                case ' ':
                    State = ParserState.EmptyLine;
                    return ParserResult.Continue;
                case '\r':
                    State = ParserState.EndOfEmpty;
                    return ParserResult.Continue;
                case '\n':
                    State = ParserState.EndOfEmpty;
                    return ParserResult.Continue;
                default:
                    State = ParserState.Error;
                    return ParserResult.Continue;
    
                }
                break;
            case ParserState.ObjectTag:
                switch (ch)
                {
                case '\r':
                    State = ParserState.EndOfTag;
                    return ParserResult.Continue;
                case '\n':
                    State = ParserState.EndOfTag;
                    return ParserResult.Continue;
                default:
                    State = ParserState.ObjectTag;
                    return ParserResult.Continue;
    
                }
                break;
            case ParserState.ValueTag:
                switch (ch)
                {
                case '\r':
                    State = ParserState.EndOfValue;
                    return ParserResult.Continue;
                case '\n':
                    State = ParserState.EndOfValue;
                    return ParserResult.Continue;
                default:
                    State = ParserState.ValueTag;
                    return ParserResult.Continue;
    
                }
                break;
            case ParserState.CommentTag:
                switch (ch)
                {
                case '\r':
                    State = ParserState.EndOfComment;
                    return ParserResult.Continue;
                case '\n':
                    State = ParserState.EndOfComment;
                    return ParserResult.Continue;
                default:
                    State = ParserState.CommentTag;
                    return ParserResult.Continue;
    
                }
                break;
            case ParserState.ValueExpected:
                switch (ch)
                {
                case '\t':
                    State = ParserState.ValueExpected;
                    return ParserResult.Continue;
                case '#':
                    State = ParserState.CommentTag;
                    return ParserResult.Continue;
                case '\r':
                    State = ParserState.EndOfEmpty;
                    return ParserResult.Continue;
                case '\n':
                    State = ParserState.EndOfEmpty;
                    return ParserResult.Continue;
                default:
                    State = ParserState.ValueLine;
                    return ParserResult.Continue;
    
                }
                break;
            default:
                return ParserResult.Error;
            }
        }
    }


}

