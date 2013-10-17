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
        visitDocument(preprocessors : string[], members : HRON[]) : void;
        visitValue(name : string, lines : string[]) : void;
        visitEmpty() : void;
        visitComment(comment : string) : void;
        visitObject(name : string, members : HRON[]) : void;
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
            visitor.visitValue(this.name, this.lines)
        }
    }

    export class HRONEmpty implements HRON {

        apply(visitor : HRONVisitor) : void {
            visitor.visitEmpty()
        }
    }

    export class HRONComment implements HRON {
        comment         : string

        constructor (
            comment     : string
            ) {
            this.comment= comment
        }


        apply(visitor : HRONVisitor) : void {
            visitor.visitComment(this.comment)
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
            visitor.visitObject(this.name, this.members)
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
            visitor.visitDocument(this.preprocessors, this.members)
        }
    }

    export class HRONSerializer implements HRONVisitor {
        public hron = new mp.StringBuilder()

        indent = 0 

        visitDocument(preprocessors : string[], members : HRON[]) : void {
            for (var iter = 0; iter < preprocessors.length; ++iter) {
                var preprocessor = preprocessors[0]
                this
                    .hron
                    .append("!")
                    .append(preprocessor)
                    .newLine()
            }

            for (var iter = 0; iter < members.length; ++iter) {
                var member = members[iter]
                member.apply(this);
            }
        }

        visitValue(name : string, lines : string[]) : void {
            this
                .hron
                .indent(this.indent)
                .append("=")
                .append(name)
                .newLine()

            for (var iter = 0; iter < lines.length; ++iter) {
                var line = lines[iter]

                this
                    .hron
                    .indent(this.indent + 1)
                    .append(line)
                    .newLine()
            }

        }

        visitEmpty() : void {
            this
                .hron
                .newLine()
        }

        visitComment(comment : string) : void {
            this
                .hron
                .append("#")
                .append(comment)
                .newLine()
        }

        visitObject(name : string, members : HRON[]) : void {

            this
                .hron
                .indent(this.indent)
                .append("@")
                .append(name)
                .newLine()

            ++this.indent

            for (var iter = 0; iter < members.length; ++iter) {
                var member = members[iter]
                member.apply(this);
            }

            --this.indent
        }

    }

    // Defining HRON grammar
    // The grammar can be found here: https://github.com/mrange/hron

    var whitespace = mp.satisfy(mp.satisyWhitespace)

    var emptyString = mp.manyString(whitespace.except(mp.EOL()))

    var string_ = mp.manyString(mp.anyChar().except(mp.EOL()))

    var commentString = mp.anyIndention()
                .keepLeft(mp.skipString("#"))
                .keepRight(string_)

    var preprocessor = mp.skipString("!")
                .keepRight(string_)
                .keepLeft(mp.EOL())

    var preprocessors = mp.many(preprocessor)

    var emptyLine = emptyString.keepLeft(mp.EOL())

    var commentLine = commentString.keepLeft(mp.EOL())

    var nonEmptyLine = mp.indention()
                .keepRight(string_)
                .keepLeft(mp.EOL())

    var valueLine = mp.choice(
            nonEmptyLine    , 
            commentLine     , 
            emptyLine
            )

    var valueLines = mp.many(valueLine.except(mp.EOL()))

    var value : mp.Parser<HRON> = () => {
        var pname = mp.indention().keepRight(mp.skipString("=").keepRight(string_.keepLeft(mp.EOL())))
        var plines = mp.indent().keepRight(valueLines.keepLeft(mp.dedent()))

        return pname
                .combine(plines)
                .transform((c : {v0 : string; v1 : string[]}) => {return new HRONValue(c.v0, c.v1)})
        }()

    var empty : mp.Parser<HRON> = emptyString
            .keepLeft(mp.EOL())
            .transform((c : string) => {return new HRONEmpty()})

    var comment : mp.Parser<HRON> = commentString
            .keepLeft(mp.EOL())
            .transform((c : string) => {return new HRONComment(c)})

    // object uses members and members uses object implicitly
    // circular() is used to break circular references
    var members = mp.circular<HRON[]>()

    var object : mp.Parser<HRON> = () => {
        var pname = mp.indention().keepRight(mp.skipString("@").keepRight(string_.keepLeft(mp.EOL())))
        var pobjects = mp.indent().keepRight(members.keepLeft(mp.dedent()))
        return pname
                .combine(pobjects)
                .transform((c : {v0 : string; v1 : HRON[]}) => {return new HRONObject(c.v0, c.v1)})
        }()

    var member = mp.choice (value.log("value"), object.log("object"), comment.log("comment"), empty.log("empty"))

    var membersImpl = mp.many(member.except(mp.EOS()))

    var hron = () => {
        // Sets up the circular dependency
        members.parse = membersImpl.parse

        return preprocessors.combine(members)
            .transform((c : {v0 : string[]; v1 : HRON[]}) => {return new HRONDocument(c.v0, c.v1)})
        }()

    export function parseHron(s : string) : {doc : HRONDocument; stop : number} {
        var result = mp.parse(hron, s)
        if (result.success) {
            return {doc : result.value, stop : result.state.position}
        } else {
            return {doc : null, stop : result.state.position}
        }
    }

    export function writeHRON(doc : HRONDocument) : string {
        var v = new HRONSerializer()
        doc.apply(v)
        return v.hron.toString()
    }


}
