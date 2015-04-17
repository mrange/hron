package org.m3.hron;

class BaseScanner {
    public int            expectedIndent  ;
    public int            indention       ;
    public int            lineNo          ;
    public int            isBuildingValue ;

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
        ScannerState choice
        ) {

    }

    protected void scannerStateTransition (
            ScannerState            from
        ,   ScannerState            to
        ,   ScannerStateTransition  transition
        ) {

    }
}

