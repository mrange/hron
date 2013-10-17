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

module MonadicParser {

    export class Snapshot {
        position    : number
        indent      : number
    }

    export class ParserState {
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

    export class ParseResult<T> {
        state   : ParserState
        success : boolean
        value   : T
    }

    export class Parser<T> {
        parse       :   (ps : ParserState) => ParseResult<T>

        constructor (p : (ps : ParserState) => ParseResult<T>) {
            this.parse = p
        }

        combine<TOther>(pOther : Parser<TOther>) : Parser<{v0 : T; v1 : TOther}> {
            return parser<{v0 : T; v1 : TOther}> ((ps : ParserState) => { 
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
            return parser ((ps : ParserState) => { 
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
            return parser ((ps : ParserState) => { 
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
            return parser ((ps : ParserState) => { 
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
            return parser ((ps : ParserState) => { 

                var pResult = this.parse(ps)

                if (!pResult.success) {
                    return ps.succeed<T>(null)
                }

                return ps.succeed(pResult.value)
            })
        }

        transform<TTo>(transform : (T) => TTo) : Parser<TTo> {
            return parser ((ps : ParserState) => { 

                var pResult = this.parse(ps)

                if (!pResult.success) {
                    return ps.succeed<TTo>(null)
                }

                return ps.succeed(transform(pResult.value))
            })
        }
    }

    export function parser<T> (p : (ps : ParserState) => ParseResult<T>) {
        return new Parser<T> (p)
    }

    export function parse<T>(p : Parser<T>, s : string) : ParseResult<T> {
        var ps = new ParserState(s)
        return p.parse(ps)
    }

    export function success<T>(value : T) : Parser<T> {
        return parser ((ps : ParserState) => { return ps.succeed(value) })
    }

    export function fail<T>() : Parser<T> {
        return parser ((ps : ParserState) => { return ps.fail<T>() })
    }

   
    export function indent() : Parser<void> {
        return parser ((ps : ParserState) => { 
            ps.increaseIndent()
            return ps.succeed<void>(undefined)
        })
    }

    export function dedent() : Parser<void> {
        return parser ((ps : ParserState) => { 
            if (!ps.decreaseIndent()) {
                return ps.fail<void>()
            }
            return ps.succeed<void>(undefined)
        })
    }

    export function anyChar() : Parser<string> {
        return parser ((ps : ParserState) => { 
            if (ps.isEOS()) {
                ps.fail<string>()
            }

            var ch = ps.text[ps.position]

            ++ps.position

            return ps.succeed(ch)
        })
    }

    export function EOS() : Parser<void> {
        return parser ((ps : ParserState) => { 
            if (!ps.isEOS()) {
                ps.fail<void>()
            }

            return ps.succeed<void>(undefined)
        })
    }

    export function EOL() : Parser<void> {
        return parser ((ps : ParserState) => { 
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

    export function satisfy(satisfy : (string, number) => boolean) : Parser<string> {
        return parser ((ps : ParserState) => { 
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

    export function satisfyMany(satisfy : (string, number) => boolean) : Parser<string> {
        return parser ((ps : ParserState) => { return ps.succeed(ps.advance(satisfy)) })
    }

    export function skipSatisfyMany(satisfy : (string, number) => boolean) : Parser<number> {
        return parser ((ps : ParserState) => { return ps.succeed(ps.skipAdvance(satisfy)) })
    }

    export function satisyWhitespace(str : string, pos : number) {
        return str === " " || str === "\t" || str === "\r" || str === "\n" 
    }

    export function satisyTab(str : string, pos : number) {
        return str === "\t"
    }

    export function skipString(str : string) : Parser<void> {
        return parser ((ps : ParserState) => { 
            var snapshot = ps.snapshot()

            var ss = str

            var result = ps.skipAdvance((s, pos) => {
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

    export function many<T>(p : Parser<T>) : Parser<T[]> {
        return parser ((ps : ParserState) => { 

            var result : T[] = []

            var pResult : ParseResult<T>

            while((pResult = p.parse(ps)).success) {
                result.push(pResult.value)
            }

            return ps.succeed(result)
        })
    }

    export function manyString(p : Parser<string>) : Parser<string> {
        return parser ((ps : ParserState) => { 

            var result = ""

            var pResult : ParseResult<string>

            while((pResult = p.parse(ps)).success) {
                result += pResult.value
            }

            return ps.succeed(result)
        })
    }

    export function choice<T>(... choices : Parser<T>[]) : Parser<T> {
        return parser ((ps : ParserState) => { 

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

    export function anyIndention() : Parser<number> {
        return skipSatisfyMany(satisyWhitespace)
    }

    export function indention() : Parser<number> {
        return parser ((ps : ParserState) => { 
            var snapshot = ps.snapshot()

            var tabs = ps.skipAdvance(satisyTab)

            if (tabs !== ps.indent) {
                ps.restore(snapshot)
                return ps.fail<number>()
            }

            return ps.succeed(tabs)
        })
    }
}
