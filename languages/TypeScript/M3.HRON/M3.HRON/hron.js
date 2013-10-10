var HRON;
(function (HRON) {
    var Snapshot = (function () {
        function Snapshot() {
        }
        return Snapshot;
    })();

    var ParserState = (function () {
        function ParserState() {
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

            for (; pos < end && satisfy(this.text.charAt(pos), pos); ++pos) {
            }

            this.position = pos;

            return this.text.substring(begin, pos);
        };

        ParserState.prototype.skipAdvance = function (satisfy) {
            var begin = this.position;
            var end = this.text.length;

            var pos = begin;

            for (; pos < end && satisfy(this.text.charAt(pos), pos); ++pos) {
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

    function success(value) {
        return function (ps) {
            return ps.succeed(value);
        };
    }

    function fail() {
        return function (ps) {
            return ps.fail();
        };
    }

    function indent() {
        return function (ps) {
            ps.increaseIndent();
            return ps.succeed(undefined);
        };
    }

    function dedent() {
        return function (ps) {
            if (!ps.decreaseIndent()) {
                return ps.fail();
            }
            return ps.succeed(undefined);
        };
    }

    function anyChar() {
        return function (ps) {
            if (ps.isEOS()) {
                ps.fail();
            }

            var ch = ps.text[ps.position];

            ++ps.position;

            return ps.succeed(ch);
        };
    }

    function EOS() {
        return function (ps) {
            if (!ps.isEOS()) {
                ps.fail();
            }

            return ps.succeed(undefined);
        };
    }

    function EOL() {
        return function (ps) {
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
        };
    }

    function satisfy(satisfy) {
        return function (ps) {
            if (ps.isEOS()) {
                ps.fail();
            }

            var ch = ps.text[ps.position];

            if (!satisfy(ch, 0)) {
                ps.fail();
            }

            ++ps.position;

            return ps.succeed(ch);
        };
    }

    function satisfyMany(satisfy) {
        return function (ps) {
            return ps.succeed(ps.advance(satisfy));
        };
    }

    function skipSatisfyMany(satisfy) {
        return function (ps) {
            return ps.succeed(ps.skipAdvance(satisfy));
        };
    }

    function satisyWhitespace(str, pos) {
        return str === " " || str === "\t" || str === "\r" || str === "\n";
    }

    function satisyTab(str, pos) {
        return str === "\t";
    }

    function skipString(str) {
        return function (ps) {
            var snapshot = ps.snapshot();

            var ss = str;

            var result = ps.skipAdvance(function (s, pos) {
                return pos < ss.length && ss.charAt(pos) === s.charAt(pos);
            });

            if (result = ss.length) {
                return ps.succeed(undefined);
            } else {
                ps.restore(snapshot);
                return ps.fail();
            }
        };
    }

    function combine(p0, p1) {
        return function (ps) {
            var snapshot = ps.snapshot();

            var p0Result = p0(ps);

            if (!p0Result.success) {
                return ps.fail();
            }

            var p1Result = p1(ps);

            if (!p1Result.success) {
                ps.restore(snapshot);
                return ps.fail();
            }

            var result = { v0: p0Result.value, v1: p1Result.value };

            return ps.succeed(result);
        };
    }

    function keepLeft(p0, p1) {
        return function (ps) {
            var snapshot = ps.snapshot();

            var p0Result = p0(ps);

            if (!p0Result.success) {
                return ps.fail();
            }

            var p1Result = p1(ps);

            if (!p1Result.success) {
                ps.restore(snapshot);
                return ps.fail();
            }

            return ps.succeed(p0Result.value);
        };
    }

    function keepRight(p0, p1) {
        return function (ps) {
            var snapshot = ps.snapshot();

            var p0Result = p0(ps);

            if (!p0Result.success) {
                return ps.fail();
            }

            var p1Result = p1(ps);

            if (!p1Result.success) {
                ps.restore(snapshot);
                return ps.fail();
            }

            return ps.succeed(p1Result.value);
        };
    }

    function except(p0, p1) {
        return function (ps) {
            var snapshot = ps.snapshot();

            var p0Result = p0(ps);

            if (!p0Result.success) {
                return ps.fail();
            }

            var p1Result = p1(ps);

            if (p1Result.success) {
                ps.restore(snapshot);
                return ps.fail();
            }

            return ps.succeed(p0Result.value);
        };
    }

    function many(p) {
        return function (ps) {
            var result = [];

            var pResult;

            while ((pResult = p(ps)).success) {
                result.push(pResult.value);
            }

            return ps.succeed(result);
        };
    }

    function manyString(p) {
        return function (ps) {
            var result = "";

            var pResult;

            while ((pResult = p(ps)).success) {
                result += pResult.value;
            }

            return ps.succeed(result);
        };
    }

    function opt(p) {
        return function (ps) {
            var pResult = p(ps);

            if (!pResult.success) {
                return ps.succeed(null);
            }

            return ps.succeed(pResult.value);
        };
    }

    function choice() {
        var choices = [];
        for (var _i = 0; _i < (arguments.length - 0); _i++) {
            choices[_i] = arguments[_i + 0];
        }
        return function (ps) {
            for (var iter = 0; iter < choices.length; ++iter) {
                var parser = choices[iter];

                var pResult = parser(ps);

                if (pResult.success) {
                    return ps.succeed(pResult.value);
                }
            }

            return ps.fail();
        };
    }

    function anyIndention() {
        return skipSatisfyMany(satisyWhitespace);
    }

    function indention() {
        return function (ps) {
            var snapshot = ps.snapshot();

            var tabs = ps.skipAdvance(satisyTab);

            if (tabs !== ps.indent) {
                ps.restore(snapshot);
                return ps.fail();
            }

            return ps.succeed(tabs);
        };
    }

    function whitespace() {
        return satisfy(satisyWhitespace);
    }

    function emptyString() {
        return manyString(except(whitespace(), EOL()));
    }

    function string_() {
        return manyString(except(anyChar(), EOL()));
    }

    function commentString() {
        return keepRight(keepLeft(anyIndention(), skipString("#")), string_());
    }
})(HRON || (HRON = {}));
//# sourceMappingURL=hron.js.map
