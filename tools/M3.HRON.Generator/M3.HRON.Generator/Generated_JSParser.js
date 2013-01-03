

// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # template file (.tt)                                                      #
// ############################################################################






function hronScanner(ps, initialState) {
    this.SR_Error       = 0;
    this.SR_Continue    = 1;
    this.SR_Done        = 2;

    this.SS_Error = 0;
    this.SS_WrongTagError = 1;
    this.SS_NonEmptyTagError = 2;
    this.SS_PreProcessing = 3;
    this.SS_Indention = 4;
    this.SS_TagExpected = 5;
    this.SS_NoContentTagExpected = 6;
    this.SS_PreProcessorTag = 7;
    this.SS_ObjectTag = 8;
    this.SS_ValueTag = 9;
    this.SS_EmptyTag = 10;
    this.SS_CommentTag = 11;
    this.SS_EndOfPreProcessorTag = 12;
    this.SS_EndOfObjectTag = 13;
    this.SS_EndOfEmptyTag = 14;
    this.SS_EndOfValueTag = 15;
    this.SS_EndOfCommentTag = 16;
    this.SS_ValueLine = 17;
    this.SS_EndOfValueLine = 18;
    this.result         = this.SR_Continue;
    this.state          = this[initialState];
    this.currentLine    = "";
    this.currentChar    = "";
    this.parserState    = ps;

    this.acceptLine     = function(line) {
        if (!line) line = "";

        this.currentLine = line;
        this.currentChar = line[0];

        var begin = 0;
        var end = line.length;

        if (this.parserState.lineBegin) this.parserState.lineBegin(this);

        for (var iter = begin; iter < end; ++iter) {
            this.currentChar = this.currentLine[iter];

            switch (this.state)
            {
            }
        }

        if (this.parserState.lineEnd) this.parserState.lineEnd(this);
    };
}





