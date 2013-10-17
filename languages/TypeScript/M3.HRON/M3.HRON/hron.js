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
    var Snapshot = (function () {
        function Snapshot() {
        }
        return Snapshot;
    })();

    var ParserState = (function () {
        function ParserState(s) {
            this.text = s || "";
            this.position = 0;
            this.indent = 0;
        }
        ParserState.prototype.snapshot = function () {
            return { position: this.position, indent: this.indent };
        };

        ParserState.prototype.increaseIndent = function () {
            ++this.indent;
        };

        ParserState.prototype.decreaseIndent = function () {
            if (this.indent < 1) {
                return false;
            }

            --this.position;

            return true;
        };

        ParserState.prototype.restore = function (snapshot) {
            this.position = snapshot.position;
            this.indent = snapshot.indent;
        };

        ParserState.prototype.isEOS = function () {
            return this.position < this.text.length;
        };

        ParserState.prototype.advance = function (satisfy) {
            var begin = this.position;
            var end = this.text.length;

            var pos = begin;

            for (; pos < end && satisfy(this.text.charCodeAt(pos), pos); ++pos) {
            }

            this.position = pos;

            return this.text.substring(begin, pos);
        };

        ParserState.prototype.skipAdvance = function (satisfy) {
            var begin = this.position;
            var end = this.text.length;

            var pos = begin;

            for (; pos < end && satisfy(this.text.charCodeAt(pos), pos); ++pos) {
            }

            this.position = pos;

            return pos - begin;
        };

        ParserState.prototype.succeed = function (value) {
            return { state: this, success: true, value: value };
        };

        ParserState.prototype.fail = function () {
            return { state: this, success: false, value: undefined };
        };
        return ParserState;
    })();

    var ParseResult = (function () {
        function ParseResult() {
        }
        return ParseResult;
    })();

    var Parser = (function () {
        function Parser(p) {
            this.parse = p;
        }
        Parser.prototype.combine = function (pOther) {
            return parser(function (ps) {
                var snapshot = ps.snapshot();

                var pResult = this.parse(ps);

                if (!pResult.success) {
                    return ps.fail();
                }

                var pOtherResult = pOther.parse(ps);

                if (!pOtherResult.success) {
                    ps.restore(snapshot);
                    return ps.fail();
                }

                var result = { v0: pResult.value, v1: pOtherResult.value };

                return ps.succeed(result);
            });
        };

        Parser.prototype.keepLeft = function (pOther) {
            return parser(function (ps) {
                var snapshot = ps.snapshot();

                var pResult = this.parse(ps);

                if (!pResult.success) {
                    return ps.fail();
                }

                var pOtherResult = pOther.parse(ps);

                if (!pOtherResult.success) {
                    ps.restore(snapshot);
                    return ps.fail();
                }

                return ps.succeed(pResult.value);
            });
        };

        Parser.prototype.keepRight = function (pOther) {
            return parser(function (ps) {
                var snapshot = ps.snapshot();

                var pResult = this.parse(ps);

                if (!pResult.success) {
                    return ps.fail();
                }

                var pOtherResult = pOther.parse(ps);

                if (!pOtherResult.success) {
                    ps.restore(snapshot);
                    return ps.fail();
                }

                return ps.succeed(pOtherResult.value);
            });
        };

        Parser.prototype.except = function (pOther) {
            return parser(function (ps) {
                var snapshot = ps.snapshot();

                var pResult = this.parse(ps);

                if (!pResult.success) {
                    return ps.fail();
                }

                var pOtherResult = pOther.parse(ps);

                if (pOtherResult.success) {
                    ps.restore(snapshot);
                    return ps.fail();
                }

                return ps.succeed(pResult.value);
            });
        };

        Parser.prototype.opt = function () {
            return parser(function (ps) {
                var pResult = this.parse(ps);

                if (!pResult.success) {
                    return ps.succeed(null);
                }

                return ps.succeed(pResult.value);
            });
        };

        Parser.prototype.transform = function (transform) {
            return parser(function (ps) {
                var pResult = this.parse(ps);

                if (!pResult.success) {
                    return ps.succeed(null);
                }

                return ps.succeed(transform(pResult.value));
            });
        };
        return Parser;
    })();

    function parser(p) {
        return new Parser(p);
    }

    function parse(p, s) {
        var ps = new ParserState(s);
        return p.parse(ps);
    }

    function success(value) {
        return parser(function (ps) {
            return ps.succeed(value);
        });
    }

    function fail() {
        return parser(function (ps) {
            return ps.fail();
        });
    }

    function indent() {
        return parser(function (ps) {
            ps.increaseIndent();
            return ps.succeed(undefined);
        });
    }

    function dedent() {
        return parser(function (ps) {
            if (!ps.decreaseIndent()) {
                return ps.fail();
            }
            return ps.succeed(undefined);
        });
    }

    function anyChar() {
        return parser(function (ps) {
            if (ps.isEOS()) {
                ps.fail();
            }

            var ch = ps.text[ps.position];

            ++ps.position;

            return ps.succeed(ch);
        });
    }

    function EOS() {
        return parser(function (ps) {
            if (!ps.isEOS()) {
                ps.fail();
            }

            return ps.succeed(undefined);
        });
    }

    function EOL() {
        return parser(function (ps) {
            if (ps.isEOS()) {
                return ps.succeed(undefined);
            }

            if (ps.text[ps.position] === "\n") {
                ++ps.position;

                return ps.succeed(undefined);
            }

            if (ps.text[ps.position] === "\r") {
                ++ps.position;

                if (!ps.isEOS() && ps.text[ps.position] === "\n") {
                    ++ps.position;

                    return ps.succeed(undefined);
                }

                return ps.succeed(undefined);
            }

            return ps.fail();
        });
    }

    function satisfy(satisfy) {
        return parser(function (ps) {
            if (ps.isEOS()) {
                ps.fail();
            }

            var ch = ps.text[ps.position];

            if (!satisfy(ch, 0)) {
                ps.fail();
            }

            ++ps.position;

            return ps.succeed(ch);
        });
    }

    function satisfyMany(satisfy) {
        return parser(function (ps) {
            return ps.succeed(ps.advance(satisfy));
        });
    }

    function skipSatisfyMany(satisfy) {
        return parser(function (ps) {
            return ps.succeed(ps.skipAdvance(satisfy));
        });
    }

    function satisyWhitespace(str, pos) {
        return str === " " || str === "\t" || str === "\r" || str === "\n";
    }

    function satisyTab(str, pos) {
        return str === "\t";
    }

    function skipString(str) {
        return parser(function (ps) {
            var snapshot = ps.snapshot();

            var ss = str;

            var result = ps.skipAdvance(function (s, pos) {
                return pos < ss.length && ss.charCodeAt(pos) === s.charCodeAt(pos);
            });

            if (result = ss.length) {
                return ps.succeed(undefined);
            } else {
                ps.restore(snapshot);
                return ps.fail();
            }
        });
    }

    function many(p) {
        return parser(function (ps) {
            var result = [];

            var pResult;

            while ((pResult = p.parse(ps)).success) {
                result.push(pResult.value);
            }

            return ps.succeed(result);
        });
    }

    function manyString(p) {
        return parser(function (ps) {
            var result = "";

            var pResult;

            while ((pResult = p.parse(ps)).success) {
                result += pResult.value;
            }

            return ps.succeed(result);
        });
    }

    function choice() {
        var choices = [];
        for (var _i = 0; _i < (arguments.length - 0); _i++) {
            choices[_i] = arguments[_i + 0];
        }
        return parser(function (ps) {
            for (var iter = 0; iter < choices.length; ++iter) {
                var p = choices[iter];

                var pResult = p.parse(ps);

                if (pResult.success) {
                    return ps.succeed(pResult.value);
                }
            }

            return ps.fail();
        });
    }

    function anyIndention() {
        return skipSatisfyMany(satisyWhitespace);
    }

    function indention() {
        return parser(function (ps) {
            var snapshot = ps.snapshot();

            var tabs = ps.skipAdvance(satisyTab);

            if (tabs !== ps.indent) {
                ps.restore(snapshot);
                return ps.fail();
            }

            return ps.succeed(tabs);
        });
    }

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

    var HRONEmpty = (function () {
        function HRONEmpty() {
        }
        HRONEmpty.prototype.apply = function (visitor) {
            visitor.VisitEmpty();
        };
        return HRONEmpty;
    })();

    var HRONComment = (function () {
        function HRONComment(line) {
            this.line = line;
        }
        HRONComment.prototype.apply = function (visitor) {
            visitor.VisitComment(this.line);
        };
        return HRONComment;
    })();

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

    // Defining HRON grammar
    function whitespace() {
        return satisfy(satisyWhitespace);
    }

    function emptyString() {
        return manyString(whitespace().except(EOL()));
    }

    function string_() {
        return manyString(anyChar().except(EOL()));
    }

    function commentString() {
        return;
        anyIndention().keepLeft(skipString("#")).keepRight(string_());
    }

    function preprocessor() {
        return;
        skipString("!").keepRight(string_()).keepLeft(EOL());
    }

    function preprocessors() {
        return many(preprocessor());
    }

    function emptyLine() {
        return emptyLine().keepLeft(EOL());
    }

    function commentLine() {
        return commentString().keepLeft(EOL());
    }

    function nonEmptyLine() {
        return;
        indention().keepRight(string_()).keepLeft(EOL());
    }

    function valueLine() {
        return choice(nonEmptyLine(), commentLine(), emptyLine());
    }

    function valueLines() {
        return many(valueLine().except(EOL()));
    }

    function value() {
        var pname = indention().keepRight(skipString("=").keepRight(string_().keepLeft(EOL())));
        var plines = indent().keepRight(valueLines().keepLeft(dedent()));

        return;
        pname.combine(plines).transform(function (c) {
            return new HRONValue(c.v0, c.v1);
        });
    }

    function empty() {
        return;
        emptyString().keepLeft(EOL()).transform(function (c) {
            return new HRONEmpty();
        });
    }

    function comment() {
        return;
        commentString().keepLeft(EOL()).transform(function (c) {
            return new HRONComment(c);
        });
    }

    function object() {
        var pname = indention().keepRight(skipString("@").keepRight(string_().keepLeft(EOL())));
        var pobjects = indent().keepRight(members().keepLeft(dedent()));
        return;
        pname.combine(pobjects).transform(function (c) {
            return new HRONObject(c.v0, c.v1);
        });
    }

    function member() {
        return choice(value(), object(), comment(), empty());
    }

    function members() {
        return many(member().except(EOS()));
    }
})(HRON || (HRON = {}));
//# sourceMappingURL=hron.js.map
