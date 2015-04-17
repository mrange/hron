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

class BaseScanner {
    public int          expectedIndent  ;
    public int          indention       ;
    public int          lineNo          ;
    public int          isBuildingValue ;

    public HRONVisitor  visitor         ;

    public void init (ScannerState initialState) {
        expectedIndent  = 0;
        indention       = 0;
        lineNo          = 0;
        isBuildingValue = 0;
    }

    protected void scannerBeginLine () {

    }

    protected void scannerEndLine () {

    }

    protected void scannerStateChoice (
        ScannerStateChoice choice
        ) {

    }

    protected void scannerStateTransition (
            ScannerState            from
        ,   ScannerState            to
        ,   ScannerStateTransition  transition
        ) {

    }
}

