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

namespace M3.HRON.Reference

open System
open System.Diagnostics
open System.Text

open Parser

type ValueLine      =
    |   EmptyLine   of string
    |   CommentLine of int*string
    |   ContentLine of string          

type Member         =
    |   Empty   of string
    |   Value   of string*ValueLine list
    |   Object  of string*Member list
    |   Comment of int*string


type HRON           = Member list

module HRONParser =

    let p_value_tag     = p_char '='
    let p_object_tag    = p_char '@'
    let p_comment_tag   = p_char '#'

    let p_empty_string  = p_many (p_whitespace >>! p_eol) >>? (fun cs -> new string(List.toArray cs)) 
    let p_string        = p_many (p_any_char >>! p_eol) >>? (fun cs -> new string(List.toArray cs))
    let p_any_indention = p_many p_tab >>? (fun cs -> List.length cs)
    let p_comment_string= p_any_indention
                            .>> p_comment_tag 
                            >>  p_string 


    let p_empty_line    = p_empty_string 
                            .>> p_eol 
                            >>? EmptyLine
    let p_comment_line  = p_comment_string
                            .>> p_eol 
                            >>? CommentLine
    let p_nonempty_line = p_indention 
                            >>. p_string 
                            .>> p_eol
                            >>? ContentLine

    let p_value_line    = p_choose [
                            p_nonempty_line ; 
                            p_comment_line  ; 
                            p_empty_line    ;
                            ]
    let p_value_lines   = p_many (p_value_line >>! p_eos)

    let p_value         = p_indention 
                            >>. p_value_tag 
                            >>. p_string 
                            .>> p_eol 
                            .>> p_indent 
                            >>  p_value_lines  
                            .>> p_dedent >>? Value

    let p_empty         = p_empty_string 
                            .>> p_eol 
                            >>? Empty

    let p_comment       = p_comment_string
                            .>> p_eol 
                            >>? Comment

    let rec p_members ps= (p_many (p_member >>! p_eos)) ps 
    and p_object ps     = (p_indention 
                            >>. p_object_tag 
                            >>. p_string 
                            .>> p_eol 
                            .>> p_indent 
                            >> p_members  
                            .>> p_dedent >>? Object
                            ) ps

    and p_member ps     = (p_choose [
                            p_value     ; 
                            p_object    ; 
                            p_comment   ; 
                            p_empty     ;
                            ]) ps

    let p_hron  = p_members

    let to_string hron  =
        let value_to_string (sb : StringBuilder) i v =
            match v with
                |   CommentLine (i, ln) -> sb.Append('\t', i).Append('#').Append(ln).AppendLine()
                |   EmptyLine   ws      -> sb.Append(ws).AppendLine()
                |   ContentLine ln      -> sb.Append('\t', i).Append(ln).AppendLine()
        let rec member_to_string (sb : StringBuilder) i m =
            match m with
                |   Comment (i, ln)     -> sb.Append('\t', i).Append('#').Append(ln).AppendLine()
                |   Empty   ws          -> sb.Append(ws).AppendLine()
                |   Object  (nm, ms)    ->
                    ignore (sb.Append('\t', i).Append('@').Append(nm).AppendLine())
                    for m' in ms do
                       ignore (member_to_string sb (i + 1) m')
                    sb
                |   Value (nm, vs)      ->                         
                    ignore (sb.Append('\t', i).Append('=').Append(nm).AppendLine())
                    for v' in vs do
                       ignore (value_to_string sb (i + 1) v')
                    sb
        let sb = new StringBuilder(128)
        for m' in hron do
            ignore (member_to_string sb 0 m')
        sb.ToString ()