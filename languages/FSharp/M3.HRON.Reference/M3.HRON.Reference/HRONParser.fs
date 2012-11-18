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
open Parser

type Member         =
    |   Value   of string*string list
    |   Object  of string*Member list
    |   Other


type HRON           = Member list

module HRONParser =

    let p_value_tag     = p_char '='
    let p_object_tag    = p_char '@'
    let p_comment_tag   = p_char '#'

    let p_empty_string  = p_many (p_whitespace >>! p_eol) >>? (fun cs -> ()) 
    let p_string        = p_many (p_any_char >>! p_eol) >>? (fun cs -> new string(List.toArray cs))
    let p_comment_string= p_empty_string 
                            >>. p_comment_tag 
                            >>. p_string 

    let invalid_line                = "<<INVALID_LINE>>"
    let is_valid_line (line: string)= Object.ReferenceEquals(line, invalid_line) = false
    let is_valid_member m           = match m with
                                        | Other ->  false
                                        | _     ->  true
                                 

    let p_empty_line    = p_empty_string 
                            .>> p_eol 
                            >>? (fun cs -> "")
    let p_comment_line  = p_comment_string
                            .>> p_eol 
                            >>? (fun cs -> invalid_line)
    let p_nonempty_line = p_indention 
                            >>. p_string 
                            .>> p_eol

    let p_value_line    = p_choose [
                            p_nonempty_line ; 
                            p_comment_line  ; 
                            p_empty_line    ;
                            ]
    let p_value_lines   = p_many (p_value_line >>! p_eos)
                            >>? (fun vs -> vs |> List.filter is_valid_line)

    let p_value         = p_indention 
                            >>. p_value_tag 
                            >>. p_string 
                            .>> p_eol 
                            .>> p_indent 
                            >>  p_value_lines  
                            .>> p_dedent >>? Value

    let p_empty         = p_empty_string 
                            .>> p_eol 
                            >>? (fun cs -> Other)

    let p_comment       = p_comment_string
                            .>> p_eol 
                            >>? (fun cs -> Other)

    let rec p_members ps= (p_many (p_member >>! p_eos) 
                            >>? (fun ms -> ms |> List.filter is_valid_member)) ps
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

