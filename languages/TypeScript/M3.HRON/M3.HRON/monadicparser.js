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
var MonadicParser;
(function (MonadicParser) {
    var Snapshot = (function () {
        function Snapshot() {
        }
        return Snapshot;
    })();
    MonadicParser.Snapshot = Snapshot;

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
            return this.position >= this.text.length;
        };

        ParserState.prototype.advance = function (satisfy) {
            var begin = this.position;
            var end = this.text.length;

            var i = 0;
            var pos = begin;

            for (; pos < end && satisfy(this.text.charCodeAt(pos), i); ++pos, ++i) {
            }

            this.position = pos;

            return this.text.substring(begin, pos);
        };

        ParserState.prototype.skipAdvance = function (satisfy) {
            var begin = this.position;
            var end = this.text.length;

            var i = 0;
            var pos = begin;

            for (; pos < end && satisfy(this.text.charCodeAt(pos), i); ++pos, ++i) {
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
    MonadicParser.ParserState = ParserState;

    var ParseResult = (function () {
        function ParseResult() {
        }
        return ParseResult;
    })();
    MonadicParser.ParseResult = ParseResult;

    var Parser = (function () {
        function Parser(p) {
            this.parse = p;
        }
        Parser.prototype.combine = function (pOther) {
            var _this = this;
            return parser(function (ps) {
                var snapshot = ps.snapshot();

                var pResult = _this.parse(ps);

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
            var _this = this;
            return parser(function (ps) {
                var snapshot = ps.snapshot();

                var pResult = _this.parse(ps);

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
            var _this = this;
            return parser(function (ps) {
                var snapshot = ps.snapshot();

                var pResult = _this.parse(ps);

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

        Parser.prototype.except = function (pExcept) {
            var _this = this;
            return parser(function (ps) {
                var snapshot = ps.snapshot();

                var pExceptResult = pExcept.parse(ps);

                if (pExceptResult.success) {
                    ps.restore(snapshot);
                    return ps.fail();
                }

                var pResult = _this.parse(ps);

                if (!pResult.success) {
                    return ps.fail();
                }

                return ps.succeed(pResult.value);
            });
        };

        Parser.prototype.opt = function () {
            var _this = this;
            return parser(function (ps) {
                var pResult = _this.parse(ps);

                if (!pResult.success) {
                    return ps.succeed(null);
                }

                return ps.succeed(pResult.value);
            });
        };

        Parser.prototype.transform = function (transform) {
            var _this = this;
            return parser(function (ps) {
                var pResult = _this.parse(ps);

                if (!pResult.success) {
                    return ps.fail();
                }

                return ps.succeed(transform(pResult.value));
            });
        };
        return Parser;
    })();
    MonadicParser.Parser = Parser;

    function parser(p) {
        return new Parser(p);
    }
    MonadicParser.parser = parser;

    function parse(p, s) {
        var ps = new ParserState(s);
        return p.parse(ps);
    }
    MonadicParser.parse = parse;

    function success(value) {
        return parser(function (ps) {
            return ps.succeed(value);
        });
    }
    MonadicParser.success = success;

    function fail() {
        return parser(function (ps) {
            return ps.fail();
        });
    }
    MonadicParser.fail = fail;

    function indent() {
        return parser(function (ps) {
            ps.increaseIndent();
            return ps.succeed(undefined);
        });
    }
    MonadicParser.indent = indent;

    function dedent() {
        return parser(function (ps) {
            if (!ps.decreaseIndent()) {
                return ps.fail();
            }
            return ps.succeed(undefined);
        });
    }
    MonadicParser.dedent = dedent;

    function anyChar() {
        return parser(function (ps) {
            if (ps.isEOS()) {
                ps.fail();
            }

            var ch = ps.text.charCodeAt(ps.position);

            ++ps.position;

            return ps.succeed(ch);
        });
    }
    MonadicParser.anyChar = anyChar;

    function EOS() {
        return parser(function (ps) {
            if (!ps.isEOS()) {
                return ps.fail();
            }

            return ps.succeed(undefined);
        });
    }
    MonadicParser.EOS = EOS;

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
    MonadicParser.EOL = EOL;

    function satisfy(satisfy) {
        return parser(function (ps) {
            if (ps.isEOS()) {
                ps.fail();
            }

            var ch = ps.text.charCodeAt(ps.position);

            if (!satisfy(ch, 0)) {
                ps.fail();
            }

            ++ps.position;

            return ps.succeed(ch);
        });
    }
    MonadicParser.satisfy = satisfy;

    function satisfyMany(satisfy) {
        return parser(function (ps) {
            return ps.succeed(ps.advance(satisfy));
        });
    }
    MonadicParser.satisfyMany = satisfyMany;

    function skipSatisfyMany(satisfy) {
        return parser(function (ps) {
            return ps.succeed(ps.skipAdvance(satisfy));
        });
    }
    MonadicParser.skipSatisfyMany = skipSatisfyMany;

    function satisyWhitespace(ch, pos) {
        switch (ch) {
            case 0x09:
            case 0x0A:
            case 0x0D:
            case 0x20:
                return true;
            default:
                return false;
        }
    }
    MonadicParser.satisyWhitespace = satisyWhitespace;

    function satisyTab(ch, pos) {
        return ch === 0x09;
    }
    MonadicParser.satisyTab = satisyTab;

    function skipString(str) {
        return parser(function (ps) {
            var snapshot = ps.snapshot();

            var ss = str;

            var result = ps.skipAdvance(function (s, pos) {
                return pos < ss.length && ss.charCodeAt(pos) === s;
            });

            if (result === ss.length) {
                return ps.succeed(undefined);
            } else {
                ps.restore(snapshot);
                return ps.fail();
            }
        });
    }
    MonadicParser.skipString = skipString;

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
    MonadicParser.many = many;

    function manyString(p) {
        return parser(function (ps) {
            var result = "";

            var pResult;

            while ((pResult = p.parse(ps)).success) {
                result += String.fromCharCode(pResult.value);
            }

            return ps.succeed(result);
        });
    }
    MonadicParser.manyString = manyString;

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
    MonadicParser.choice = choice;

    function anyIndention() {
        return skipSatisfyMany(satisyWhitespace);
    }
    MonadicParser.anyIndention = anyIndention;

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
    MonadicParser.indention = indention;

    function circular() {
        return parser(null);
    }
    MonadicParser.circular = circular;
})(MonadicParser || (MonadicParser = {}));
//# sourceMappingURL=monadicparser.js.map
