namespace M3.HRON.Reference

open System
open System.Diagnostics
open Parser

type Member         =
    |   Value   of string*string list
    |   Object  of string*Member list


type HRON           = Member list

module HRONParser =

    let p_value_tag     = p_char '='
    let p_object_tag    = p_char '@'

    let p_string        = p_many (p_any_char >>! p_eol) >>? (fun cs -> new string(List.toArray cs))

    let p_value_line    = p_indention >>. p_string .>> p_eol
    let p_value         = p_indention >>. p_value_tag >>. p_string .>> p_eol .>> p_indent >> (p_many p_value_line) .>> p_dedent >>? Value

    let rec p_object ps = (p_indention >>. p_object_tag >>. p_string .>> p_eol .>> p_indent >> (p_many p_member) .>> p_dedent >>? Object) ps
    and p_member ps     = (p_choose [p_value; p_object]) ps

    let p_hron  = p_many p_member

