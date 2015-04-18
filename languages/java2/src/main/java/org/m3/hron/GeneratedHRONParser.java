


// ---------------------------------------------------------------------------------------------- 
// Copyright (c) M?rten R?nge. 
// ---------------------------------------------------------------------------------------------- 
// This source code is subject to terms and conditions of the Microsoft Public License. A 
// copy of the license can be found in the License.html file at the root of this distribution. 
// If you cannot locate the  Microsoft Public License, please send an email to 
// dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
//  by the terms of the Microsoft Public License. 
// ---------------------------------------------------------------------------------------------- 
// You must not remove this notice, or any other, from this software. 
// ---------------------------------------------------------------------------------------------- 

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

class Scanner {
    public ScannerResult    result            ;
    public ScannerState     state             ;
    public String           currentLine       ;
    public int              currentLineBegin  ;
    public int              currentLineEnd    ;
    public char             currentChar       ;
    public ScannerExtension extension         ;

    public Scanner (ScannerState initialState, ScannerExtension ext) {
        result          = ScannerResult.Continue;
        state           = initialState          ;
        currentLine     = ""                    ;
        currentLineBegin= 0                     ;
        currentLineEnd  = 0                     ;
        currentChar     = ' '                   ;
        extension       = ext                   ;
    }

    ScannerResult acceptLine (String line) {
        currentLine       = line          ;
        currentLineBegin  = 0             ;
        currentLineEnd    = line.length() ;
        currentChar       = ' '           ;

        ScannerExtension.scannerBeginLine (this);

        for (int iter = currentLineBegin; iter < currentLineEnd && result == ScannerResult.Continue; ++iter) {
            currentChar = currentLine.charAt (iter);

            while (apply ()) { }
        }

        applyEndOfLine ();

        ScannerExtension.scannerEndLine (this);

        return result;
    }

    boolean apply () {
        switch (state) {
        case Error:
            switch (currentChar) {
            default:
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.Error
                        ,   ScannerState.Error
                        ,   ScannerStateTransition.From_Error__To_Error
                        );
                    return false;
                }
        case WrongTagError:
            switch (currentChar) {
            default:
                    state = ScannerState.Error;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.WrongTagError
                        ,   ScannerState.Error
                        ,   ScannerStateTransition.From_WrongTagError__To_Error
                        );
                    return false;
                }
        case NonEmptyTagError:
            switch (currentChar) {
            default:
                    state = ScannerState.Error;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.NonEmptyTagError
                        ,   ScannerState.Error
                        ,   ScannerStateTransition.From_NonEmptyTagError__To_Error
                        );
                    return false;
                }
        case PreProcessing:
            switch (currentChar) {
            case '!':
                    state = ScannerState.PreProcessorTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.PreProcessing
                        ,   ScannerState.PreProcessorTag
                        ,   ScannerStateTransition.From_PreProcessing__To_PreProcessorTag
                        );
                    return false;
            default:
                    state = ScannerState.Indention;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.PreProcessing
                        ,   ScannerState.Indention
                        ,   ScannerStateTransition.From_PreProcessing__To_Indention
                        );
                    return true;
                }
        case Indention:
            switch (currentChar) {
            case '\t':
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.Indention
                        ,   ScannerState.Indention
                        ,   ScannerStateTransition.From_Indention__To_Indention
                        );
                    return false;
            default:
                    ScannerExtension.scannerStateChoice (
                            this
                        ,   ScannerStateChoice.From_Indention__Choose_TagExpected_NoContentTagExpected_ValueLine_Error
                        );

                switch (state) {
                case TagExpected:
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.Indention
                        ,   ScannerState.TagExpected
                        ,   ScannerStateTransition.From_Indention__To_TagExpected
                        );
                        break;
                case NoContentTagExpected:
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.Indention
                        ,   ScannerState.NoContentTagExpected
                        ,   ScannerStateTransition.From_Indention__To_NoContentTagExpected
                        );
                        break;
                case ValueLine:
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.Indention
                        ,   ScannerState.ValueLine
                        ,   ScannerStateTransition.From_Indention__To_ValueLine
                        );
                        break;
                case Error:
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.Indention
                        ,   ScannerState.Error
                        ,   ScannerStateTransition.From_Indention__To_Error
                        );
                        break;
                    default:
                        result = ScannerResult.Error;
                        break;
                    }
                    return true;
                }
        case TagExpected:
            switch (currentChar) {
            case '@':
                    state = ScannerState.ObjectTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.TagExpected
                        ,   ScannerState.ObjectTag
                        ,   ScannerStateTransition.From_TagExpected__To_ObjectTag
                        );
                    return false;
            case '=':
                    state = ScannerState.ValueTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.TagExpected
                        ,   ScannerState.ValueTag
                        ,   ScannerStateTransition.From_TagExpected__To_ValueTag
                        );
                    return false;
            case '#':
                    state = ScannerState.CommentTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.TagExpected
                        ,   ScannerState.CommentTag
                        ,   ScannerStateTransition.From_TagExpected__To_CommentTag
                        );
                    return false;
            case '\t':
            case ' ':
                    state = ScannerState.EmptyTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.TagExpected
                        ,   ScannerState.EmptyTag
                        ,   ScannerStateTransition.From_TagExpected__To_EmptyTag
                        );
                    return false;
            default:
                    state = ScannerState.WrongTagError;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.TagExpected
                        ,   ScannerState.WrongTagError
                        ,   ScannerStateTransition.From_TagExpected__To_WrongTagError
                        );
                    return false;
                }
        case NoContentTagExpected:
            switch (currentChar) {
            case '#':
                    state = ScannerState.CommentTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.NoContentTagExpected
                        ,   ScannerState.CommentTag
                        ,   ScannerStateTransition.From_NoContentTagExpected__To_CommentTag
                        );
                    return false;
            case '\t':
            case ' ':
                    state = ScannerState.EmptyTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.NoContentTagExpected
                        ,   ScannerState.EmptyTag
                        ,   ScannerStateTransition.From_NoContentTagExpected__To_EmptyTag
                        );
                    return false;
            default:
                    state = ScannerState.WrongTagError;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.NoContentTagExpected
                        ,   ScannerState.WrongTagError
                        ,   ScannerStateTransition.From_NoContentTagExpected__To_WrongTagError
                        );
                    return false;
                }
        case PreProcessorTag:
            switch (currentChar) {
            default:
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.PreProcessorTag
                        ,   ScannerState.PreProcessorTag
                        ,   ScannerStateTransition.From_PreProcessorTag__To_PreProcessorTag
                        );
                    return false;
                }
        case ObjectTag:
            switch (currentChar) {
            default:
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.ObjectTag
                        ,   ScannerState.ObjectTag
                        ,   ScannerStateTransition.From_ObjectTag__To_ObjectTag
                        );
                    return false;
                }
        case ValueTag:
            switch (currentChar) {
            default:
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.ValueTag
                        ,   ScannerState.ValueTag
                        ,   ScannerStateTransition.From_ValueTag__To_ValueTag
                        );
                    return false;
                }
        case EmptyTag:
            switch (currentChar) {
            case '\t':
            case ' ':
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.EmptyTag
                        ,   ScannerState.EmptyTag
                        ,   ScannerStateTransition.From_EmptyTag__To_EmptyTag
                        );
                    return false;
            default:
                    state = ScannerState.NonEmptyTagError;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.EmptyTag
                        ,   ScannerState.NonEmptyTagError
                        ,   ScannerStateTransition.From_EmptyTag__To_NonEmptyTagError
                        );
                    return false;
                }
        case CommentTag:
            switch (currentChar) {
            default:
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.CommentTag
                        ,   ScannerState.CommentTag
                        ,   ScannerStateTransition.From_CommentTag__To_CommentTag
                        );
                    return false;
                }
        case EndOfPreProcessorTag:
            switch (currentChar) {
            default:
                    state = ScannerState.PreProcessing;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.EndOfPreProcessorTag
                        ,   ScannerState.PreProcessing
                        ,   ScannerStateTransition.From_EndOfPreProcessorTag__To_PreProcessing
                        );
                    return true;
                }
        case EndOfObjectTag:
            switch (currentChar) {
            default:
                    state = ScannerState.Indention;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.EndOfObjectTag
                        ,   ScannerState.Indention
                        ,   ScannerStateTransition.From_EndOfObjectTag__To_Indention
                        );
                    return true;
                }
        case EndOfEmptyTag:
            switch (currentChar) {
            default:
                    state = ScannerState.Indention;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.EndOfEmptyTag
                        ,   ScannerState.Indention
                        ,   ScannerStateTransition.From_EndOfEmptyTag__To_Indention
                        );
                    return true;
                }
        case EndOfValueTag:
            switch (currentChar) {
            default:
                    state = ScannerState.Indention;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.EndOfValueTag
                        ,   ScannerState.Indention
                        ,   ScannerStateTransition.From_EndOfValueTag__To_Indention
                        );
                    return true;
                }
        case EndOfCommentTag:
            switch (currentChar) {
            default:
                    state = ScannerState.Indention;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.EndOfCommentTag
                        ,   ScannerState.Indention
                        ,   ScannerStateTransition.From_EndOfCommentTag__To_Indention
                        );
                    return true;
                }
        case ValueLine:
            switch (currentChar) {
            default:
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.ValueLine
                        ,   ScannerState.ValueLine
                        ,   ScannerStateTransition.From_ValueLine__To_ValueLine
                        );
                    return false;
                }
        case EndOfValueLine:
            switch (currentChar) {
            default:
                    state = ScannerState.Indention;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.EndOfValueLine
                        ,   ScannerState.Indention
                        ,   ScannerStateTransition.From_EndOfValueLine__To_Indention
                        );
                    return true;
                }
        default:
            result = ScannerResult.Error;
            return false;
        }
    }

    void applyEndOfLine () {
        if (result == ScannerResult.Error) {
            return;
        }

        switch (state) {
        case Indention:
            state = ScannerState.EndOfEmptyTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.Indention
                        ,   ScannerState.EndOfEmptyTag
                        ,   ScannerStateTransition.From_Indention__To_EndOfEmptyTag
                        );
            break;
        case TagExpected:
            state = ScannerState.EndOfEmptyTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.TagExpected
                        ,   ScannerState.EndOfEmptyTag
                        ,   ScannerStateTransition.From_TagExpected__To_EndOfEmptyTag
                        );
            break;
        case NoContentTagExpected:
            state = ScannerState.EndOfEmptyTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.NoContentTagExpected
                        ,   ScannerState.EndOfEmptyTag
                        ,   ScannerStateTransition.From_NoContentTagExpected__To_EndOfEmptyTag
                        );
            break;
        case PreProcessorTag:
            state = ScannerState.EndOfPreProcessorTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.PreProcessorTag
                        ,   ScannerState.EndOfPreProcessorTag
                        ,   ScannerStateTransition.From_PreProcessorTag__To_EndOfPreProcessorTag
                        );
            break;
        case ObjectTag:
            state = ScannerState.EndOfObjectTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.ObjectTag
                        ,   ScannerState.EndOfObjectTag
                        ,   ScannerStateTransition.From_ObjectTag__To_EndOfObjectTag
                        );
            break;
        case ValueTag:
            state = ScannerState.EndOfValueTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.ValueTag
                        ,   ScannerState.EndOfValueTag
                        ,   ScannerStateTransition.From_ValueTag__To_EndOfValueTag
                        );
            break;
        case EmptyTag:
            state = ScannerState.EndOfEmptyTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.EmptyTag
                        ,   ScannerState.EndOfEmptyTag
                        ,   ScannerStateTransition.From_EmptyTag__To_EndOfEmptyTag
                        );
            break;
        case CommentTag:
            state = ScannerState.EndOfCommentTag;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.CommentTag
                        ,   ScannerState.EndOfCommentTag
                        ,   ScannerStateTransition.From_CommentTag__To_EndOfCommentTag
                        );
            break;
        case ValueLine:
            state = ScannerState.EndOfValueLine;
                    ScannerExtension.scannerStateTransition (
                            this
                        ,   ScannerState.ValueLine
                        ,   ScannerState.EndOfValueLine
                        ,   ScannerStateTransition.From_ValueLine__To_EndOfValueLine
                        );
            break;
        }

    }
}





