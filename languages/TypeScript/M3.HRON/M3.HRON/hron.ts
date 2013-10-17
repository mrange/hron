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

module HRON {

    class Snapshot {
        position    : number
        indent      : number
    }

    class ParserState {
        text    : string
        position: number
        indent  : number

        constructor (s : string) {
            this.text       = s || ""
            this.position   = 0
            this.indent     = 0
        }

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

            for (; pos < end && satisfy(this.text.charCodeAt(pos), pos); ++pos) {
            }

            this.position = pos

            return this.text.substring(begin, pos);
        }

        skipAdvance (satisfy : (string, number) => boolean) : number {
            var begin = this.position
            var end = this.text.length

            var pos = begin;

            for (; pos < end && satisfy(this.text.charCodeAt(pos), pos); ++pos) {
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

    class Parser<T> {
        parse       :   (ps : ParserState) => ParseResult<T>

        constructor (p : (ps : ParserState) => ParseResult<T>) {
            this.parse = p
        }

        combine<TOther>(pOther : Parser<TOther>) : Parser<{v0 : T; v1 : TOther}> {
            return parser (function (ps : ParserState) { 
                var snapshot = ps.snapshot()

                var pResult = this.parse(ps)

                if (!pResult.success) {
                    return ps.fail<{v0 : T; v1 : TOther}>()
                }

                var pOtherResult = pOther.parse(ps)

                if (!pOtherResult.success) {
                    ps.restore(snapshot)
                    return ps.fail<{v0 : T; v1 : TOther}>()
                }

                var result = {v0 : pResult.value, v1 : pOtherResult.value}

                return ps.succeed(result)
            })
        }

        keepLeft<TOther>(pOther : Parser<TOther>) : Parser<T> {
            return parser (function (ps : ParserState) { 
                var snapshot = ps.snapshot()

                var pResult = this.parse(ps)

                if (!pResult.success) {
                    return ps.fail<T>()
                }

                var pOtherResult = pOther.parse(ps)

                if (!pOtherResult.success) {
                    ps.restore(snapshot)
                    return ps.fail<T>()
                }

                return ps.succeed(pResult.value)
            })
        }

        keepRight<TOther>(pOther : Parser<TOther>) : Parser<TOther> {
            return parser (function (ps : ParserState) { 
                var snapshot = ps.snapshot()

                var pResult = this.parse(ps)

                if (!pResult.success) {
                    return ps.fail<TOther>()
                }

                var pOtherResult = pOther.parse(ps)

                if (!pOtherResult.success) {
                    ps.restore(snapshot)
                    return ps.fail<TOther>()
                }

                return ps.succeed(pOtherResult.value)
            })
        }

        except<TOther>(pOther : Parser<TOther>) : Parser<T> {
            return parser (function (ps : ParserState) { 
                var snapshot = ps.snapshot()

                var pResult = this.parse(ps)

                if (!pResult.success) {
                    return ps.fail<T>()
                }

                var pOtherResult = pOther.parse(ps)

                if (pOtherResult.success) {
                    ps.restore(snapshot)
                    return ps.fail<T>()
                }

                return ps.succeed(pResult.value)
            })
        }

        opt() : Parser<T> {
            return parser (function (ps : ParserState) { 

                var pResult = this.parse(ps)

                if (!pResult.success) {
                    return ps.succeed<T>(null)
                }

                return ps.succeed(pResult.value)
            })
        }

        transform<TTo>(transform : (T) => TTo) : Parser<TTo> {
            return parser (function (ps : ParserState) { 

                var pResult = this.parse(ps)

                if (!pResult.success) {
                    return ps.succeed<TTo>(null)
                }

                return ps.succeed(transform(pResult.value))
            })
        }
    }

    function parser<T> (p : (ps : ParserState) => ParseResult<T>) {
        return new Parser<T> (p)
    }

    function parse<T>(p : Parser<T>, s : string) : ParseResult<T> {
        var ps = new ParserState(s)
        return p.parse(ps)
    }

    function success<T>(value : T) : Parser<T> {
        return parser (function (ps : ParserState) { return ps.succeed(value) })
    }

    function fail<T>() : Parser<T> {
        return parser (function (ps : ParserState) { return ps.fail<T>() })
    }

   
    function indent() : Parser<void> {
        return parser (function (ps : ParserState) { 
            ps.increaseIndent()
            return ps.succeed<void>(undefined)
        })
    }

    function dedent() : Parser<void> {
        return parser (function (ps : ParserState) { 
            if (!ps.decreaseIndent()) {
                return ps.fail<void>()
            }
            return ps.succeed<void>(undefined)
        })
    }

    function anyChar() : Parser<string> {
        return parser (function (ps : ParserState) { 
            if (ps.isEOS()) {
                ps.fail<string>()
            }

            var ch = ps.text[ps.position]

            ++ps.position

            return ps.succeed(ch)
        })
    }

    function EOS() : Parser<void> {
        return parser (function (ps : ParserState) { 
            if (!ps.isEOS()) {
                ps.fail<void>()
            }

            return ps.succeed<void>(undefined)
        })
    }

    function EOL() : Parser<void> {
        return parser (function (ps : ParserState) { 
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
        })
    }

    function satisfy(satisfy : (string, number) => boolean) : Parser<string> {
        return parser (function (ps : ParserState) { 
            if (ps.isEOS()) {
                ps.fail<string>()
            }

            var ch = ps.text[ps.position]

            if (!satisfy(ch,0)) {
                ps.fail<string>()
            }

            ++ps.position

            return ps.succeed(ch)
        })
    }

    function satisfyMany(satisfy : (string, number) => boolean) : Parser<string> {
        return parser (function (ps : ParserState) { return ps.succeed(ps.advance(satisfy)) })
    }

    function skipSatisfyMany(satisfy : (string, number) => boolean) : Parser<number> {
        return parser (function (ps : ParserState) { return ps.succeed(ps.skipAdvance(satisfy)) })
    }

    function satisyWhitespace(str : string, pos : number) {
        return str === " " || str === "\t" || str === "\r" || str === "\n" 
    }

    function satisyTab(str : string, pos : number) {
        return str === "\t"
    }

    function skipString(str : string) : Parser<void> {
        return parser (function (ps : ParserState) { 
            var snapshot = ps.snapshot()

            var ss = str

            var result = ps.skipAdvance(function (s, pos) {
                return pos < ss.length && ss.charCodeAt(pos) === s.charCodeAt(pos)
                })

            if (result = ss.length) {
                return ps.succeed<void>(undefined)
            } else {
                ps.restore(snapshot)
                return ps.fail<void>()
            }
        })
    }

    function many<T>(p : Parser<T>) : Parser<T[]> {
        return parser (function (ps : ParserState) { 

            var result : T[] = []

            var pResult : ParseResult<T>

            while((pResult = p.parse(ps)).success) {
                result.push(pResult.value)
            }

            return ps.succeed(result)
        })
    }

    function manyString(p : Parser<string>) : Parser<string> {
        return parser (function (ps : ParserState) { 

            var result = ""

            var pResult : ParseResult<string>

            while((pResult = p.parse(ps)).success) {
                result += pResult.value
            }

            return ps.succeed(result)
        })
    }

    function choice<T>(... choices : Parser<T>[]) : Parser<T> {
        return parser (function (ps : ParserState) { 

            for (var iter = 0; iter < choices.length; ++iter) {
                var p = choices[iter]

                var pResult = p.parse(ps)

                if (pResult.success) {
                    return ps.succeed(pResult.value)
                }

            }

            return ps.fail<T>()
        })
    }

    function anyIndention() : Parser<number> {
        return skipSatisfyMany(satisyWhitespace)
    }

    function indention() : Parser<number> {
        return parser (function (ps : ParserState) { 
            var snapshot = ps.snapshot()

            var tabs = ps.skipAdvance(satisyTab)

            if (tabs !== ps.indent) {
                ps.restore(snapshot)
                return ps.fail<number>()
            }

            return ps.succeed(tabs)
        })
    }

    // Defining HRON AST

    interface HRONVisitor {
        VisitDocument(preprocessors : string[], members : HRON[]) : void;
        VisitValue(name : string, lines : string[]) : void;
        VisitEmpty() : void;
        VisitComment(lin : string) : void;
        VisitObject(name : string, members : HRON[]) : void;
    }

    interface HRON {
        apply(visitor : HRONVisitor) : void;
    }

    class HRONValue implements HRON {
        name            : string
        lines           : string[]

        constructor (
            name            : string    ,
            lines           : string[]
            ) {
            this.name   = name
            this.lines  = lines
        }


        apply(visitor : HRONVisitor) : void {
            visitor.VisitValue(this.name, this.lines)
        }
    }

    class HRONEmpty implements HRON {

        apply(visitor : HRONVisitor) : void {
            visitor.VisitEmpty()
        }
    }

    class HRONComment implements HRON {
        line            : string

        constructor (
            line           : string
            ) {
            this.line   = line
        }


        apply(visitor : HRONVisitor) : void {
            visitor.VisitComment(this.line)
        }
    }

    class HRONObject implements HRON {
        name            : string
        members         : HRON[]

        constructor (
            name            : string,
            members         : HRON[]
            ) {
            this.name   = name
            this.members= members
        }

        apply(visitor : HRONVisitor) : void {
            visitor.VisitObject(this.name, this.members)
        }
    }

    class HRONDocument implements HRON {
        preprocessors   : string[]
        members         : HRON[]

        constructor (
            preprocessors   : string[]  ,
            members         : HRON[]
            ) {
            this.preprocessors  = preprocessors
            this.members        = members
        }

        apply(visitor : HRONVisitor) : void {
            visitor.VisitDocument(this.preprocessors, this.members)
        }
    }

    // Defining HRON grammar

    function whitespace() : Parser<string> {
        return satisfy(satisyWhitespace)
    }

    function emptyString() : Parser<string> {
        return manyString(whitespace().except(EOL()))
    }

    function string_() : Parser<string> {
        return manyString(anyChar().except(EOL()))
    }

    function commentString() : Parser<string> {
        return 
            anyIndention()
                .keepLeft(skipString("#"))
                .keepRight(string_())
    }

    function preprocessor() : Parser<string> {
        return 
            skipString("!")
                .keepRight(string_())
                .keepLeft(EOL())
    }

    function preprocessors() : Parser<string[]> {
        return many(preprocessor())
    }

    function emptyLine() : Parser<string> {
        return emptyLine().keepLeft(EOL())
    }

    function commentLine() : Parser<string> {
        return commentString().keepLeft(EOL())
    }

    function nonEmptyLine() : Parser<string> {
        return 
            indention()
                .keepRight(string_())
                .keepLeft(EOL())
    }

    function valueLine() : Parser<string> {
        return choice(
            nonEmptyLine()  , 
            commentLine()   , 
            emptyLine()
            )
    }

    function valueLines() : Parser<string[]> {
        return many(valueLine().except(EOL()))
    }

    function value() : Parser<HRON> {
        var pname = indention().keepRight(skipString("=").keepRight(string_().keepLeft(EOL())))
        var plines = indent().keepRight(valueLines().keepLeft(dedent()))

        return
            pname
                .combine(plines)
                .transform(function (c : {v0 : string; v1 : string[]}) {return new HRONValue(c.v0, c.v1)})
    }

    function empty() : Parser<HRON> {
        return 
            emptyString()
                .keepLeft(EOL())
                .transform(function (c : string) {return new HRONEmpty()})
    }

    function comment() : Parser<HRON> {
        return 
            commentString()
                .keepLeft(EOL())
                .transform(function (c : string) {return new HRONComment(c)})
    }

    function object() : Parser<HRON> {
        var pname = indention().keepRight(skipString("@").keepRight(string_().keepLeft(EOL())))
        var pobjects = indent().keepRight(members().keepLeft(dedent()))
        return
            pname
                .combine(pobjects)
                .transform(function (c : {v0 : string; v1 : HRON[]}) {return new HRONObject(c.v0, c.v1)})
    }

    function member() : Parser<HRON> {
        return choice (value(), object(), comment(), empty ())
    }

    function members() : Parser<HRON[]> {
        return many(member().except(EOS()))
    }

}
