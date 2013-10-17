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
var HRON;
(function (HRON) {
    // HRON (Human Readable Object Notation): https://github.com/mrange/hron
    // Defining HRON AST
    var mp = MonadicParser;

    var HRONValue = (function () {
        function HRONValue(name, lines) {
            this.name = name;
            this.lines = lines;
        }
        HRONValue.prototype.apply = function (visitor) {
            visitor.VisitValue(this.name, this.lines);
        };
        return HRONValue;
    })();
    HRON.HRONValue = HRONValue;

    var HRONEmpty = (function () {
        function HRONEmpty() {
        }
        HRONEmpty.prototype.apply = function (visitor) {
            visitor.VisitEmpty();
        };
        return HRONEmpty;
    })();
    HRON.HRONEmpty = HRONEmpty;

    var HRONComment = (function () {
        function HRONComment(line) {
            this.line = line;
        }
        HRONComment.prototype.apply = function (visitor) {
            visitor.VisitComment(this.line);
        };
        return HRONComment;
    })();
    HRON.HRONComment = HRONComment;

    var HRONObject = (function () {
        function HRONObject(name, members) {
            this.name = name;
            this.members = members;
        }
        HRONObject.prototype.apply = function (visitor) {
            visitor.VisitObject(this.name, this.members);
        };
        return HRONObject;
    })();
    HRON.HRONObject = HRONObject;

    var HRONDocument = (function () {
        function HRONDocument(preprocessors, members) {
            this.preprocessors = preprocessors;
            this.members = members;
        }
        HRONDocument.prototype.apply = function (visitor) {
            visitor.VisitDocument(this.preprocessors, this.members);
        };
        return HRONDocument;
    })();
    HRON.HRONDocument = HRONDocument;

    // Defining HRON grammar
    // The grammar can be found here: https://github.com/mrange/hron
    function whitespace() {
        return mp.satisfy(mp.satisyWhitespace);
    }

    function emptyString() {
        return mp.manyString(whitespace().except(mp.EOL()));
    }

    function string_() {
        return mp.manyString(mp.anyChar().except(mp.EOL()));
    }

    function commentString() {
        return mp.anyIndention().keepLeft(mp.skipString("#")).keepRight(string_());
    }

    function preprocessor() {
        return mp.skipString("!").keepRight(string_()).keepLeft(mp.EOL());
    }

    function preprocessors() {
        return mp.many(preprocessor());
    }

    function emptyLine() {
        return emptyLine().keepLeft(mp.EOL());
    }

    function commentLine() {
        return commentString().keepLeft(mp.EOL());
    }

    function nonEmptyLine() {
        return mp.indention().keepRight(string_()).keepLeft(mp.EOL());
    }

    function valueLine() {
        return mp.choice(nonEmptyLine(), commentLine(), emptyLine());
    }

    function valueLines() {
        return mp.many(valueLine().except(mp.EOL()));
    }

    function value() {
        var pname = mp.indention().keepRight(mp.skipString("=").keepRight(string_().keepLeft(mp.EOL())));
        var plines = mp.indent().keepRight(valueLines().keepLeft(mp.dedent()));

        return pname.combine(plines).transform(function (c) {
            return new HRONValue(c.v0, c.v1);
        });
    }

    function empty() {
        return emptyString().keepLeft(mp.EOL()).transform(function (c) {
            return new HRONEmpty();
        });
    }

    function comment() {
        return commentString().keepLeft(mp.EOL()).transform(function (c) {
            return new HRONComment(c);
        });
    }

    function object() {
        var pname = mp.indention().keepRight(mp.skipString("@").keepRight(string_().keepLeft(mp.EOL())));
        var pobjects = mp.indent().keepRight(members().keepLeft(mp.dedent()));
        return pname.combine(pobjects).transform(function (c) {
            return new HRONObject(c.v0, c.v1);
        });
    }

    function member() {
        return mp.choice(value(), object(), comment(), empty());
    }

    function members() {
        return mp.many(member().except(mp.EOS()));
    }

    function hron() {
        return preprocessors().combine(members()).transform(function (c) {
            return new HRONDocument(c.v0, c.v1);
        });
    }

    var parserForHRON = hron();

    function parseHron(s) {
        var result = mp.parse(parserForHRON, s);
        if (result.success) {
            return { doc: result.value, stop: result.state.position };
        } else {
            return { doc: null, stop: result.state.position };
        }
    }
    HRON.parseHron = parseHron;
})(HRON || (HRON = {}));
//# sourceMappingURL=hron.js.map
