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
package org.m3.hron;

class ScannerExtension {
    public int          expectedIndent  ;
    public int          indention       ;
    public int          lineNo          ;
    public boolean      isBuildingValue ;

    public HronVisitor  visitor         ;

    public ScannerExtension(HronVisitor vis) {
        expectedIndent  = 0     ;
        indention       = 0     ;
        lineNo          = 0     ;
        isBuildingValue = false ;
        visitor         = vis   ;
    }

    public static void popContext(Scanner scanner) {
        if (scanner.extension.isBuildingValue && scanner.extension.indention < scanner.extension.expectedIndent) {
            --scanner.extension.expectedIndent;
            scanner.extension.visitor.Value_End();
            scanner.extension.isBuildingValue = false;
        }

        while (scanner.extension.indention < scanner.extension.expectedIndent) {
            --scanner.extension.expectedIndent;
            scanner.extension.visitor.Object_End();
        }
    }

    public static void endOfDocument(Scanner scanner) {
        scanner.extension.indention = 0;
        popContext(scanner);
    }

    public static void scannerBeginLine(Scanner scanner) {
        scanner.extension.indention = 0;
        ++scanner.extension.lineNo;

        switch (scanner.state) {
        case PreProcessing:
        case PreProcessorTag:
        case EndOfPreProcessorTag:
            scanner.state = ScannerState.PreProcessing;
            break;
        default:
            scanner.state = ScannerState.Indention;
            break;
        }
    }

    public static void scannerEndLine(Scanner scanner) {
    }

    public static void scannerStateChoice(
            Scanner             scanner
        ,   ScannerStateChoice  choice
        ) {
        switch (choice) {
        case From_Indention__Choose_TagExpected_NoContentTagExpected_ValueLine_Error:
            if (scanner.extension.isBuildingValue)
            {
                scanner.state = scanner.extension.expectedIndent > scanner.extension.indention
                    ? ScannerState.TagExpected
                    : ScannerState.ValueLine
                    ;
            }
            else
            {
                scanner.state = scanner.extension.expectedIndent < scanner.extension.indention
                    ? ScannerState.NoContentTagExpected
                    : ScannerState.TagExpected
                    ;
            }
            break;
        default:
            break;
        }
    }

    public static void scannerStateTransition(
            Scanner                 scanner
        ,   ScannerState            from
        ,   ScannerState            to
        ,   ScannerStateTransition  transition
        ) throws Exception {

        switch (transition) {
        case From_Indention__To_Indention:
            ++scanner.extension.indention;
            break;
        default:
            break;
        }

        switch (to)
        {
        case PreProcessorTag:
        case EmptyTag:
        case CommentTag:
        case ValueTag:
        case ValueLine:
        case ObjectTag:
            scanner.result = ScannerResult.Done;
            break;
        case EndOfValueLine:
            scanner.extension.visitor.Value_Line(scanner.currentLine, scanner.extension.expectedIndent, scanner.currentLineEnd);
            break;
        case EndOfPreProcessorTag:
            scanner.extension.visitor.PreProcessor(scanner.currentLine, scanner.extension.indention + 1, scanner.currentLineEnd);
            break;
        case EndOfCommentTag:
            scanner.extension.visitor.Comment(scanner.extension.indention, scanner.currentLine, scanner.extension.indention + 1, scanner.currentLineEnd);
            break;
        case EndOfEmptyTag:
            if (scanner.extension.isBuildingValue) {
                scanner.extension.visitor.Value_Line("", 0, 0);
            } else {
                scanner.extension.visitor.Empty(scanner.currentLine);
            }
            break;
        case EndOfObjectTag:
            popContext(scanner);
            scanner.extension.visitor.Object_Begin(scanner.currentLine, scanner.extension.indention + 1, scanner.currentLineEnd);
            scanner.extension.expectedIndent = scanner.extension.indention + 1;
            break;
        case EndOfValueTag:
            popContext(scanner);
            scanner.extension.isBuildingValue = true;
            scanner.extension.visitor.Value_Begin(scanner.currentLine, scanner.extension.indention + 1, scanner.currentLineEnd);
            scanner.extension.expectedIndent = scanner.extension.indention + 1;
            break;
        case Error:
            scanner.result = ScannerResult.Error;
            scanner.extension.visitor.Error(scanner.extension.lineNo, scanner.currentLine, "General");
            break;
        case WrongTagError:
            scanner.result = ScannerResult.Error;
            scanner.extension.visitor.Error(scanner.extension.lineNo, scanner.currentLine, "WrongTag");
            break;
        case NonEmptyTagError:
            scanner.result = ScannerResult.Error;
            scanner.extension.visitor.Error(scanner.extension.lineNo, scanner.currentLine, "NonEmptyTag");
            break;
        default:
            break;
        }
    }
}

