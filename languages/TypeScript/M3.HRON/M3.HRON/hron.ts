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
    // HRON (Human Readable Object Notation): https://github.com/mrange/hron

    // Defining HRON AST

    import mp               = MonadicParser

    export interface HRONVisitor {
        VisitDocument(preprocessors : string[], members : HRON[]) : void;
        VisitValue(name : string, lines : string[]) : void;
        VisitEmpty() : void;
        VisitComment(lin : string) : void;
        VisitObject(name : string, members : HRON[]) : void;
    }

    export interface HRON {
        apply(visitor : HRONVisitor) : void;
    }

    export class HRONValue implements HRON {
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

    export class HRONEmpty implements HRON {

        apply(visitor : HRONVisitor) : void {
            visitor.VisitEmpty()
        }
    }

    export class HRONComment implements HRON {
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

    export class HRONObject implements HRON {
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

    export class HRONDocument implements HRON {
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
    // The grammar can be found here: https://github.com/mrange/hron

    function whitespace() : mp.Parser<string> {
        return mp.satisfy(mp.satisyWhitespace)
    }

    function emptyString() : mp.Parser<string> {
        return mp.manyString(whitespace().except(mp.EOL()))
    }

    function string_() : mp.Parser<string> {
        return mp.manyString(mp.anyChar().except(mp.EOL()))
    }

    function commentString() : mp.Parser<string> {
        return mp.anyIndention()
                .keepLeft(mp.skipString("#"))
                .keepRight(string_())
    }

    function preprocessor() : mp.Parser<string> {
        return mp.skipString("!")
                .keepRight(string_())
                .keepLeft(mp.EOL())
    }

    function preprocessors() : mp.Parser<string[]> {
        return mp.many(preprocessor())
    }

    function emptyLine() : mp.Parser<string> {
        return emptyLine().keepLeft(mp.EOL())
    }

    function commentLine() : mp.Parser<string> {
        return commentString().keepLeft(mp.EOL())
    }

    function nonEmptyLine() : mp.Parser<string> {
        return mp.indention()
                .keepRight(string_())
                .keepLeft(mp.EOL())
    }

    function valueLine() : mp.Parser<string> {
        return mp.choice(
            nonEmptyLine()  , 
            commentLine()   , 
            emptyLine()
            )
    }

    function valueLines() : mp.Parser<string[]> {
        return mp.many(valueLine().except(mp.EOL()))
    }

    function value() : mp.Parser<HRON> {
        var pname = mp.indention().keepRight(mp.skipString("=").keepRight(string_().keepLeft(mp.EOL())))
        var plines = mp.indent().keepRight(valueLines().keepLeft(mp.dedent()))

        return pname
                .combine(plines)
                .transform(function (c : {v0 : string; v1 : string[]}) {return new HRONValue(c.v0, c.v1)})
    }

    function empty() : mp.Parser<HRON> {
        return emptyString()
                .keepLeft(mp.EOL())
                .transform(function (c : string) {return new HRONEmpty()})
    }

    function comment() : mp.Parser<HRON> {
        return commentString()
                .keepLeft(mp.EOL())
                .transform(function (c : string) {return new HRONComment(c)})
    }

    function object() : mp.Parser<HRON> {
        var pname = mp.indention().keepRight(mp.skipString("@").keepRight(string_().keepLeft(mp.EOL())))
        var pobjects = mp.indent().keepRight(members().keepLeft(mp.dedent()))
        return pname
                .combine(pobjects)
                .transform(function (c : {v0 : string; v1 : HRON[]}) {return new HRONObject(c.v0, c.v1)})
    }

    function member() : mp.Parser<HRON> {
        return mp.choice (value(), object(), comment(), empty ())
    }

    function members() : mp.Parser<HRON[]> {
        return mp.many(member().except(mp.EOS()))
    }

    function hron() : mp.Parser<HRONDocument> {
        return preprocessors().combine(members())
            .transform(function (c : {v0 : string[]; v1 : HRON[]}) {return new HRONDocument(c.v0, c.v1)})
    }

    var parserForHRON = hron()

    export function parseHron(s : string) : {doc : HRONDocument; stop : number} {
        var result = mp.parse(parserForHRON, s)
        if (result.success) {
            return {doc : result.value, stop : result.state.position}
        } else {
            return {doc : null, stop : result.state.position}
        }
    }

}
