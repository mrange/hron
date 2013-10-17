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
    var whitespace = mp.satisfy(mp.satisyWhitespace);

    var emptyString = mp.manyString(whitespace.except(mp.EOL()));

    var string_ = mp.manyString(mp.anyChar().except(mp.EOL()));

    var commentString = mp.anyIndention().keepLeft(mp.skipString("#")).keepRight(string_);

    var preprocessor = mp.skipString("!").keepRight(string_).keepLeft(mp.EOL());

    var preprocessors = mp.many(preprocessor);

    var emptyLine = emptyString.keepLeft(mp.EOL());

    var commentLine = commentString.keepLeft(mp.EOL());

    var nonEmptyLine = mp.indention().keepRight(string_).keepLeft(mp.EOL());

    var valueLine = mp.choice(nonEmptyLine, commentLine, emptyLine);

    var valueLines = mp.many(valueLine.except(mp.EOL()));

    var value = (function () {
        var pname = mp.indention().keepRight(mp.skipString("=").keepRight(string_.keepLeft(mp.EOL())));
        var plines = mp.indent().keepRight(valueLines.keepLeft(mp.dedent()));

        return pname.combine(plines).transform(function (c) {
            return new HRONValue(c.v0, c.v1);
        });
    })();

    var empty = emptyString.keepLeft(mp.EOL()).transform(function (c) {
        return new HRONEmpty();
    });

    var comment = commentString.keepLeft(mp.EOL()).transform(function (c) {
        return new HRONComment(c);
    });

    // object uses members and members uses object implicitly
    // circular() is used to break circular references
    var members = mp.circular();

    var object = (function () {
        var pname = mp.indention().keepRight(mp.skipString("@").keepRight(string_.keepLeft(mp.EOL())));
        var pobjects = mp.indent().keepRight(members.keepLeft(mp.dedent()));
        return pname.combine(pobjects).transform(function (c) {
            return new HRONObject(c.v0, c.v1);
        });
    })();

    var member = mp.choice(value, object, comment, empty);

    var membersImpl = mp.many(member.except(mp.EOS()));

    var hron = (function () {
        // Sets up the circular dependency
        members.parse = membersImpl.parse;

        return preprocessors.combine(members).transform(function (c) {
            return new HRONDocument(c.v0, c.v1);
        });
    })();

    function parseHron(s) {
        var result = mp.parse(hron, s);
        if (result.success) {
            return { doc: result.value, stop: result.state.position };
        } else {
            return { doc: null, stop: result.state.position };
        }
    }
    HRON.parseHron = parseHron;
})(HRON || (HRON = {}));
//# sourceMappingURL=hron.js.map
