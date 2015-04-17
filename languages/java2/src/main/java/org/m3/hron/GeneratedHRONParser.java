

// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # template file (.tt)                                                      #
// ############################################################################





package org.m3.hron;


enum ScannerState {
    Error,
    WrongTagError,
    NonEmptyTagError,
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

enum ScannerStateTransition {
    From_Error__To_Error,
    From_WrongTagError__To_Error,
    From_NonEmptyTagError__To_Error,
    From_PreProcessing__To_PreProcessorTag,
    From_PreProcessing__To_Indention,
    From_Indention__To_EndOfEmptyTag,
    From_Indention__To_Indention,
    From_Indention__To_TagExpected,
    From_Indention__To_NoContentTagExpected,
    From_Indention__To_ValueLine,
    From_Indention__To_Error,
    From_TagExpected__To_EndOfEmptyTag,
    From_TagExpected__To_ObjectTag,
    From_TagExpected__To_ValueTag,
    From_TagExpected__To_CommentTag,
    From_TagExpected__To_EmptyTag,
    From_TagExpected__To_WrongTagError,
    From_NoContentTagExpected__To_EndOfEmptyTag,
    From_NoContentTagExpected__To_CommentTag,
    From_NoContentTagExpected__To_EmptyTag,
    From_NoContentTagExpected__To_WrongTagError,
    From_PreProcessorTag__To_EndOfPreProcessorTag,
    From_PreProcessorTag__To_PreProcessorTag,
    From_ObjectTag__To_EndOfObjectTag,
    From_ObjectTag__To_ObjectTag,
    From_ValueTag__To_EndOfValueTag,
    From_ValueTag__To_ValueTag,
    From_EmptyTag__To_EndOfEmptyTag,
    From_EmptyTag__To_EmptyTag,
    From_EmptyTag__To_NonEmptyTagError,
    From_CommentTag__To_EndOfCommentTag,
    From_CommentTag__To_CommentTag,
    From_EndOfPreProcessorTag__To_PreProcessing,
    From_EndOfObjectTag__To_Indention,
    From_EndOfEmptyTag__To_Indention,
    From_EndOfValueTag__To_Indention,
    From_EndOfCommentTag__To_Indention,
    From_ValueLine__To_EndOfValueLine,
    From_ValueLine__To_ValueLine,
    From_EndOfValueLine__To_Indention,
}

enum ScannerStateChoice {
    From_Indention__Choose_TagExpected_NoContentTagExpected_ValueLine_Error,
}

enum ScannerResult {
    Error   ,
    Continue,
    Done    ,
}

class Scanner extends BaseScanner {
    public ScannerResult  result            ;
    public ScannerState   state             ;
    public String         currentLine       ;
    public int            currentLineBegin  ;
    public int            currentLineEnd    ;
    public char           currentChar       ;

    @Override
    public void init (ScannerState initialState) {
        super.init (initialState);
        result          = ScannerResult.Continue;
        state           = initialState          ;
        currentLine     = ""                    ;
        currentLineBegin= 0                     ;
        currentLineEnd  = 0                     ;
        currentChar     = ' '                   ;
    }

    ScannerResult acceptLine (String line) {
        currentLine       = line          ;
        currentLineBegin  = 0             ;
        currentLineEnd    = line.length() ;
        currentChar       = ' '           ;

        scannerBeginLine ();

        for (int iter = currentLineBegin; iter < currentLineEnd; ++iter)
        {
            currentChar = currentLine.charAt (iter);
/*
apply:
        if (ss->result != SR_Continue)
        {
            break;
        }

        switch (ss->state)
        {
        case SS_Error:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_Error
                                ,   SS_Error
                                ,   SST_From_Error__To_Error
                                );
                    break;
                }
            break;
        case SS_WrongTagError:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Error;
                            scanner_statetransition (
                                    ss
                                ,   SS_WrongTagError
                                ,   SS_Error
                                ,   SST_From_WrongTagError__To_Error
                                );
                    break;
                }
            break;
        case SS_NonEmptyTagError:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Error;
                            scanner_statetransition (
                                    ss
                                ,   SS_NonEmptyTagError
                                ,   SS_Error
                                ,   SST_From_NonEmptyTagError__To_Error
                                );
                    break;
                }
            break;
        case SS_PreProcessing:
            switch (ss->current_char)
            {
            case '!':
                    ss->state = SS_PreProcessorTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_PreProcessing
                                ,   SS_PreProcessorTag
                                ,   SST_From_PreProcessing__To_PreProcessorTag
                                );
                    break;
            default:
                    ss->state = SS_Indention;
                            scanner_statetransition (
                                    ss
                                ,   SS_PreProcessing
                                ,   SS_Indention
                                ,   SST_From_PreProcessing__To_Indention
                                );
                    goto apply;
                    break;
                }
            break;
        case SS_Indention:
            switch (ss->current_char)
            {
            case '\t':
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_Indention
                                ,   SST_From_Indention__To_Indention
                                );
                    break;
            default:
                    scanner_statechoice (
                            ss
                        ,   SSC_From_Indention__Choose_TagExpected_NoContentTagExpected_ValueLine_Error
                        );

                switch (ss->state)
                {
                case SS_TagExpected:
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_TagExpected
                                ,   SST_From_Indention__To_TagExpected
                                );
                    break;
                case SS_NoContentTagExpected:
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_NoContentTagExpected
                                ,   SST_From_Indention__To_NoContentTagExpected
                                );
                    break;
                case SS_ValueLine:
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_ValueLine
                                ,   SST_From_Indention__To_ValueLine
                                );
                    break;
                case SS_Error:
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_Error
                                ,   SST_From_Indention__To_Error
                                );
                    break;
                    default:
                        ss->result = SR_Error;
                        break;
                    }
                    goto apply;
                    break;
                }
            break;
        case SS_TagExpected:
            switch (ss->current_char)
            {
            case '@':
                    ss->state = SS_ObjectTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_ObjectTag
                                ,   SST_From_TagExpected__To_ObjectTag
                                );
                    break;
            case '=':
                    ss->state = SS_ValueTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_ValueTag
                                ,   SST_From_TagExpected__To_ValueTag
                                );
                    break;
            case '#':
                    ss->state = SS_CommentTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_CommentTag
                                ,   SST_From_TagExpected__To_CommentTag
                                );
                    break;
            case '\t':
            case ' ':
                    ss->state = SS_EmptyTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_EmptyTag
                                ,   SST_From_TagExpected__To_EmptyTag
                                );
                    break;
            default:
                    ss->state = SS_WrongTagError;
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_WrongTagError
                                ,   SST_From_TagExpected__To_WrongTagError
                                );
                    break;
                }
            break;
        case SS_NoContentTagExpected:
            switch (ss->current_char)
            {
            case '#':
                    ss->state = SS_CommentTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_NoContentTagExpected
                                ,   SS_CommentTag
                                ,   SST_From_NoContentTagExpected__To_CommentTag
                                );
                    break;
            case '\t':
            case ' ':
                    ss->state = SS_EmptyTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_NoContentTagExpected
                                ,   SS_EmptyTag
                                ,   SST_From_NoContentTagExpected__To_EmptyTag
                                );
                    break;
            default:
                    ss->state = SS_WrongTagError;
                            scanner_statetransition (
                                    ss
                                ,   SS_NoContentTagExpected
                                ,   SS_WrongTagError
                                ,   SST_From_NoContentTagExpected__To_WrongTagError
                                );
                    break;
                }
            break;
        case SS_PreProcessorTag:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_PreProcessorTag
                                ,   SS_PreProcessorTag
                                ,   SST_From_PreProcessorTag__To_PreProcessorTag
                                );
                    break;
                }
            break;
        case SS_ObjectTag:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_ObjectTag
                                ,   SS_ObjectTag
                                ,   SST_From_ObjectTag__To_ObjectTag
                                );
                    break;
                }
            break;
        case SS_ValueTag:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_ValueTag
                                ,   SS_ValueTag
                                ,   SST_From_ValueTag__To_ValueTag
                                );
                    break;
                }
            break;
        case SS_EmptyTag:
            switch (ss->current_char)
            {
            case '\t':
            case ' ':
                            scanner_statetransition (
                                    ss
                                ,   SS_EmptyTag
                                ,   SS_EmptyTag
                                ,   SST_From_EmptyTag__To_EmptyTag
                                );
                    break;
            default:
                    ss->state = SS_NonEmptyTagError;
                            scanner_statetransition (
                                    ss
                                ,   SS_EmptyTag
                                ,   SS_NonEmptyTagError
                                ,   SST_From_EmptyTag__To_NonEmptyTagError
                                );
                    break;
                }
            break;
        case SS_CommentTag:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_CommentTag
                                ,   SS_CommentTag
                                ,   SST_From_CommentTag__To_CommentTag
                                );
                    break;
                }
            break;
        case SS_EndOfPreProcessorTag:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_PreProcessing;
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfPreProcessorTag
                                ,   SS_PreProcessing
                                ,   SST_From_EndOfPreProcessorTag__To_PreProcessing
                                );
                    goto apply;
                    break;
                }
            break;
        case SS_EndOfObjectTag:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Indention;
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfObjectTag
                                ,   SS_Indention
                                ,   SST_From_EndOfObjectTag__To_Indention
                                );
                    goto apply;
                    break;
                }
            break;
        case SS_EndOfEmptyTag:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Indention;
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfEmptyTag
                                ,   SS_Indention
                                ,   SST_From_EndOfEmptyTag__To_Indention
                                );
                    goto apply;
                    break;
                }
            break;
        case SS_EndOfValueTag:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Indention;
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfValueTag
                                ,   SS_Indention
                                ,   SST_From_EndOfValueTag__To_Indention
                                );
                    goto apply;
                    break;
                }
            break;
        case SS_EndOfCommentTag:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Indention;
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfCommentTag
                                ,   SS_Indention
                                ,   SST_From_EndOfCommentTag__To_Indention
                                );
                    goto apply;
                    break;
                }
            break;
        case SS_ValueLine:
            switch (ss->current_char)
            {
            default:
                            scanner_statetransition (
                                    ss
                                ,   SS_ValueLine
                                ,   SS_ValueLine
                                ,   SST_From_ValueLine__To_ValueLine
                                );
                    break;
                }
            break;
        case SS_EndOfValueLine:
            switch (ss->current_char)
            {
            default:
                    ss->state = SS_Indention;
                            scanner_statetransition (
                                    ss
                                ,   SS_EndOfValueLine
                                ,   SS_Indention
                                ,   SST_From_EndOfValueLine__To_Indention
                                );
                    goto apply;
                    break;
                }
            break;
        default:
            ss->result = SR_Error;
            break;
        }
    }

    if (ss->result == SR_Error)
    {
        goto end;
    }

    switch (ss->state)
    {
    case SS_Indention:
            ss->state = SS_EndOfEmptyTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_Indention
                                ,   SS_EndOfEmptyTag
                                ,   SST_From_Indention__To_EndOfEmptyTag
                                );
        break;
    case SS_TagExpected:
            ss->state = SS_EndOfEmptyTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_TagExpected
                                ,   SS_EndOfEmptyTag
                                ,   SST_From_TagExpected__To_EndOfEmptyTag
                                );
        break;
    case SS_NoContentTagExpected:
            ss->state = SS_EndOfEmptyTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_NoContentTagExpected
                                ,   SS_EndOfEmptyTag
                                ,   SST_From_NoContentTagExpected__To_EndOfEmptyTag
                                );
        break;
    case SS_PreProcessorTag:
            ss->state = SS_EndOfPreProcessorTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_PreProcessorTag
                                ,   SS_EndOfPreProcessorTag
                                ,   SST_From_PreProcessorTag__To_EndOfPreProcessorTag
                                );
        break;
    case SS_ObjectTag:
            ss->state = SS_EndOfObjectTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_ObjectTag
                                ,   SS_EndOfObjectTag
                                ,   SST_From_ObjectTag__To_EndOfObjectTag
                                );
        break;
    case SS_ValueTag:
            ss->state = SS_EndOfValueTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_ValueTag
                                ,   SS_EndOfValueTag
                                ,   SST_From_ValueTag__To_EndOfValueTag
                                );
        break;
    case SS_EmptyTag:
            ss->state = SS_EndOfEmptyTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_EmptyTag
                                ,   SS_EndOfEmptyTag
                                ,   SST_From_EmptyTag__To_EndOfEmptyTag
                                );
        break;
    case SS_CommentTag:
            ss->state = SS_EndOfCommentTag;
                            scanner_statetransition (
                                    ss
                                ,   SS_CommentTag
                                ,   SS_EndOfCommentTag
                                ,   SST_From_CommentTag__To_EndOfCommentTag
                                );
        break;
    case SS_ValueLine:
            ss->state = SS_EndOfValueLine;
                            scanner_statetransition (
                                    ss
                                ,   SS_ValueLine
                                ,   SS_EndOfValueLine
                                ,   SST_From_ValueLine__To_EndOfValueLine
                                );
        break;
    }

end:
*/      
        }

        scannerEndLine ();

        return result;
    }
}





