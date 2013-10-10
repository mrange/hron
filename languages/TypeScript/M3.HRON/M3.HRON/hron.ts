module HRON {

    class Snapshot {
        position    : number
        indent      : number
    }

    class ParserState {
        text    : string
        position: number
        indent  : number

        snapshot() : Snapshot {
            return { position : this.position, indent : this.indent }
        }

        increaseIndent() : void {
            ++this.indent
        }

        decreaseIndent() : boolean {
            if (this.indent < 1) {
                return false
            }

            --this.position

            return true
        }

        restore(snapshot : Snapshot) {
            this.position   = snapshot.position
            this.indent     = snapshot.indent
        }

        isEOS() : boolean {
            return this.position < this.text.length
        }

        advance (satisfy : (string, number) => boolean) : string {
            var begin = this.position
            var end = this.text.length

            var pos = begin;

            for (; pos < end && satisfy(this.text.charAt(pos), pos); ++pos) {
            }

            this.position = pos

            return this.text.substring(begin, pos);
        }

        skipAdvance (satisfy : (string, number) => boolean) : number {
            var begin = this.position
            var end = this.text.length

            var pos = begin;

            for (; pos < end && satisfy(this.text.charAt(pos), pos); ++pos) {
            }

            this.position = pos

            return pos - begin
        }

        succeed<T>(value : T) : ParseResult<T> {
            return {state : this, success : true , value : value}
        }

        fail<T>() : ParseResult<T> {
            return {state : this, success : false , value : undefined}
        }

    }

    class ParseResult<T> {
        state   : ParserState
        success : boolean
        value   : T
    }

    interface Parser<T> {
        (ps : ParserState) : ParseResult<T>
    }

    function success<T>(value : T) : Parser<T> {
        return function (ps : ParserState) { return ps.succeed(value) }
    }

    function fail<T>() : Parser<T> {
        return function (ps : ParserState) { return ps.fail<T>() }
    }

   
    function indent() : Parser<void> {
        return function (ps : ParserState) { 
            ps.increaseIndent()
            return ps.succeed<void>(undefined)
        }
    }

    function dedent() : Parser<void> {
        return function (ps : ParserState) { 
            if (!ps.decreaseIndent()) {
                return ps.fail<void>()
            }
            return ps.succeed<void>(undefined)
        }
    }

    function anyChar() : Parser<string> {
        return function (ps : ParserState) { 
            if (ps.isEOS()) {
                ps.fail<string>()
            }

            var ch = ps.text[ps.position]

            ++ps.position

            return ps.succeed(ch)
        }
    }

    function EOS() : Parser<void> {
        return function (ps : ParserState) { 
            if (!ps.isEOS()) {
                ps.fail<void>()
            }

            return ps.succeed<void>(undefined)
        }
    }

    function EOL() : Parser<void> {
        return function (ps : ParserState) { 
            if (ps.isEOS()) {
                return ps.succeed<void>(undefined)
            }

            if (ps.text[ps.position] === "\n") {
                ++ps.position

                return ps.succeed<void>(undefined)
            }

            if (ps.text[ps.position] === "\r") {
                ++ps.position

                if (!ps.isEOS() && ps.text[ps.position] === "\n") {
                    ++ps.position

                    return ps.succeed<void>(undefined)
                }

                return ps.succeed<void>(undefined)
            }

            return ps.fail<void>()
        }
    }

    function satisfy(satisfy : (string, number) => boolean) : Parser<string> {
        return function (ps : ParserState) { 
            if (ps.isEOS()) {
                ps.fail<string>()
            }

            var ch = ps.text[ps.position]

            if (!satisfy(ch,0)) {
                ps.fail<string>()
            }

            ++ps.position

            return ps.succeed(ch)
        }
    }

    function satisfyMany(satisfy : (string, number) => boolean) : Parser<string> {
        return function (ps : ParserState) { return ps.succeed(ps.advance(satisfy)) }
    }

    function skipSatisfyMany(satisfy : (string, number) => boolean) : Parser<number> {
        return function (ps : ParserState) { return ps.succeed(ps.skipAdvance(satisfy)) }
    }

    function satisyWhitespace(str : string, pos : number) {
        return str === " " || str === "\t" || str === "\r" || str === "\n" 
    }

    function satisyTab(str : string, pos : number) {
        return str === "\t"
    }


    function skipString(str : string) : Parser<void> {
        return function (ps : ParserState) { 
            var snapshot = ps.snapshot()

            var ss = str

            var result = ps.skipAdvance(function (s, pos) {
                return pos < ss.length && ss.charAt(pos) === s.charAt(pos)
                })

            if (result = ss.length) {
                return ps.succeed<void>(undefined)
            } else {
                ps.restore(snapshot)
                return ps.fail<void>()
            }
        }
    }

    function combine<T0, T1>(p0 : Parser<T0>, p1 : Parser<T1>) : Parser<{v0 : T0; v1 : T1}> {
        return function (ps : ParserState) { 
            var snapshot = ps.snapshot()

            var p0Result = p0(ps)

            if (!p0Result.success) {
                return ps.fail<{v0 : T0; v1 : T1}>()
            }

            var p1Result = p1(ps)

            if (!p1Result.success) {
                ps.restore(snapshot)
                return ps.fail<{v0 : T0; v1 : T1}>()
            }

            var result = {v0 : p0Result.value, v1 : p1Result.value}

            return ps.succeed(result)
        }
    }

    function keepLeft<T0, T1>(p0 : Parser<T0>, p1 : Parser<T1>) : Parser<T0> {
        return function (ps : ParserState) { 
            var snapshot = ps.snapshot()

            var p0Result = p0(ps)

            if (!p0Result.success) {
                return ps.fail<T0>()
            }

            var p1Result = p1(ps)

            if (!p1Result.success) {
                ps.restore(snapshot)
                return ps.fail<T0>()
            }

            return ps.succeed(p0Result.value)
        }
    }

    function keepRight<T0, T1>(p0 : Parser<T0>, p1 : Parser<T1>) : Parser<T1> {
        return function (ps : ParserState) { 
            var snapshot = ps.snapshot()

            var p0Result = p0(ps)

            if (!p0Result.success) {
                return ps.fail<T1>()
            }

            var p1Result = p1(ps)

            if (!p1Result.success) {
                ps.restore(snapshot)
                return ps.fail<T1>()
            }

            return ps.succeed(p1Result.value)
        }
    }

    function except<T0,T1>(p0 : Parser<T0>, p1 : Parser<T1>) : Parser<T0> {
        return function (ps : ParserState) { 
            var snapshot = ps.snapshot()

            var p0Result = p0(ps)

            if (!p0Result.success) {
                return ps.fail<T0>()
            }

            var p1Result = p1(ps)

            if (p1Result.success) {
                ps.restore(snapshot)
                return ps.fail<T0>()
            }

            return ps.succeed(p0Result.value)
        }
    }

    function many<T>(p : Parser<T>) : Parser<T[]> {
        return function (ps : ParserState) { 

            var result : T[] = []

            var pResult : ParseResult<T>

            while((pResult = p(ps)).success) {
                result.push(pResult.value)
            }

            return ps.succeed(result)
        }
    }

    function manyString(p : Parser<string>) : Parser<string> {
        return function (ps : ParserState) { 

            var result = ""

            var pResult : ParseResult<string>

            while((pResult = p(ps)).success) {
                result += pResult.value
            }

            return ps.succeed(result)
        }
    }

    function opt<T>(p : Parser<T>) : Parser<T> {
        return function (ps : ParserState) { 

            var pResult = p(ps)

            if (!pResult.success) {
                return ps.succeed<T>(null)
            }

            return ps.succeed(pResult.value)
        }
    }

    function choice<T>(... choices : Parser<T>[]) : Parser<T> {
        return function (ps : ParserState) { 

            for (var iter = 0; iter < choices.length; ++iter) {
                var parser = choices[iter]

                var pResult = parser(ps)

                if (pResult.success) {
                    return ps.succeed(pResult.value)
                }

            }

            return ps.fail<T>()
        }
    }

    function anyIndention() : Parser<number> {
        return skipSatisfyMany(satisyWhitespace)
    }

    function indention() : Parser<number> {
        return function (ps : ParserState) { 
            var snapshot = ps.snapshot()

            var tabs = ps.skipAdvance(satisyTab)

            if (tabs !== ps.indent) {
                ps.restore(snapshot)
                return ps.fail<number>()
            }

            return ps.succeed(tabs)
        }
    }

    // Defining HRON AST

    // Defining HRON grammar
    function whitespace() : Parser<string> {
        return satisfy(satisyWhitespace)
    }

    function emptyString() : Parser<string> {
        return manyString(except(whitespace(), EOL()))
    }

    function string_() : Parser<string> {
        return manyString(except(anyChar(), EOL()))
    }

    function commentString() : Parser<string> {
        return keepRight(keepLeft(anyIndention(), skipString("#")), string_())
    }

    function preprocessor() : Parser<string> {
        return keepLeft(keepRight(skipString("!"), string_()), EOL())
    }

    function preprocessors() : Parser<string[]> {
        return many(preprocessor())
    }

    function emptyLine() : Parser<string> {
        return keepLeft(emptyLine(), EOL())
    }

    function commentLine() : Parser<string> {
        return keepLeft(commentString(), EOL())
    }

    function nonEmptyLine() : Parser<string> {
        return keepLeft(keepRight(indention(), string_()), EOL())
    }

    function valueLine() : Parser<string> {
        return choice(nonEmptyLine(), commentLine(), emptyLine())
    }

    function valueLines() : Parser<string[]> {
        return many(except(valueLine(), EOL()))
    }

    function value() : Parser<string[]> {
        return many(except(valueLine(), EOL()))
    }

}
