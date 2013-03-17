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

type ParserState    = {input : string; pos : int; indent : int;}

type ParserResult<'a>  =
    |   Success of 'a*ParserState
    |   Failure of string*ParserState*ParserState

type Parser<'a>     = ParserState -> ParserResult<'a>

module Parser =
    
    let parse (p : Parser<'a>) s    =
        let ps = {input = s; pos = 0; indent = 0}
        p ps

    let advance     ps      = {ps with pos = ps.pos + 1}
    let move_to     ps pos  = {ps with pos = pos}
    let is_not_eos  ps      = ps.pos < ps.input.Length
    let is_eos      ps      = ps.pos >= ps.input.Length
    let is_success  pr      =   match pr with
                                    |   Success(_,_)    -> true
                                    |   _               -> false   

    let p_success v         = (fun ps -> Success(v, ps))
    
    let p_failure v         = (fun ps -> Failure(v, ps, ps))

    let current_pos ps      = 
        let center  = 0
        let width   = 32
        let start = max (ps.pos - center) 0
        let stop = min (ps.pos + width + (start - (ps.pos - center))) ps.input.Length  
        ps.input.Substring(start, stop - start).Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t")

    let p_debug p           = (fun ps -> 
//        if System.Diagnostics.Debugger.IsAttached then System.Diagnostics.Debugger.Break ()
        let r = p ps
        let is_success = 
            match r with 
                | Success _ -> "Yes"
                | _ -> "No"
        if (is_eos ps) then printf "DEBUG: EOS\r\n"
        else (printf "DEBUG: success:%s pos:%d indent:%d %s\r\n" is_success ps.pos ps.indent (current_pos ps))        
        r
        )

    let p_eos               = (fun ps -> if is_eos ps then Success((), ps) else Failure("Expected EOS", ps, ps))

    let p_any_char      = (fun ps -> 
        if is_not_eos ps
            then Success (ps.input.[ps.pos], advance ps)
            else Failure ("EOS", ps, ps)
        )

    let p_satisy test e = (fun ps -> 
        if is_not_eos ps
            then
                if (test ps.input.[ps.pos]) 
                    then Success (ps.input.[ps.pos], advance ps)
                    else Failure (e, ps, ps)
            else  Failure ("EOS", ps, ps)
        )

    let p_satisy_many test = (fun ps ->         
        let mutable p = ps.pos
        let e = ps.input.Length

        while (p < e) && (test (p - ps.pos) ps.input.[p]) do
            p <- p + 1

        Success (ps.input.Substring(ps.pos, p - ps.pos), move_to ps p)
        )

    let p_satisy_fixed test fix e = (fun ps ->         
        let mutable p = ps.pos
        let ee = fix + ps.pos

        if ee > ps.input.Length then
            Failure (e, ps, ps)
        else
            while (p < ee) && (test (p - ps.pos) ps.input.[p]) do
                p <- p + 1

            if p = ee then
                Success (ps.input.Substring(ps.pos, p - ps.pos), move_to ps p)
            else
                Failure (e, ps, move_to ps p)
        )

    let p_value (p : Parser<'a>) v = (fun ps ->
        match p ps with
            |   Success(_, ps') ->  Success(v, ps')
            |   Failure(e, _, ps')->  Failure(e, ps, ps')
        )

    let p_map (p : Parser<'a>) m = (fun ps ->
        match p ps with
            |   Success(v, ps')     ->  Success(m v, ps')
            |   Failure(e, _, ps')  ->  Failure(e,ps, ps')
        )

    let p_many (p : Parser<'a>)  = 
        (fun ps ->
            let mutable is_looking  = true
            let mutable ps'         = ps
            let         result      = new ResizeArray<'a> ()
            while (is_looking) do
                let pr = p ps'
                match pr with
                    |   Success (v, ps'')   ->
                        result.Add(v)
                        ps'     <- ps''
                    |   _       ->
                        is_looking <- false

            Success (result.ToArray (), ps')
        )

    let p_sep (p : Parser<'a>) (psep : Parser<'b>)= 
        (fun ps ->
            let mutable is_first    = true
            let mutable is_looking  = true
            let mutable ps'         = ps
            let         result      = new ResizeArray<'a> ()
            while (is_looking) do
                if is_first then
                    is_first <- false
                else
                    match psep ps' with
                        |   Success(_,ps'') ->
                            ps' <- ps''
                        |   _               ->
                            is_looking <- false

                if is_looking then                    
                    let pr = p ps'
                    match pr with
                        |   Success (v, ps'')   ->
                            result.Add(v)
                            ps'     <- ps''
                        |   _       ->
                            is_looking <- false

            Success (result.ToArray (), ps')
        )

    let p_opt (p : Parser<'a>)  = (fun ps ->
        match p ps with
            |   Success (v, ps')    ->  Success (Some v, ps')
            |   Failure _           ->  Success (None, ps)
        )
        
    let p_except (p : Parser<'a>) (pe : Parser<'b>) = (fun ps ->
        match pe ps with
            |   Success (_, ps')->  Failure ("Unexpected", ps, ps')
            |   Failure _       ->  p ps
        )

    let p_choose (parsers : Parser<'a> list) = 
        (fun ps ->
            let mutable parsers'    = parsers
            let mutable ps'         = ps
            let mutable result      = None
            while parsers'.Length > 0 do
                let pr = parsers'.Head ps'
                match pr with
                    |   Success (v, ps'') ->
                        parsers'    <- []
                        ps'         <- ps''
                        result      <- Some v
                    |   _   ->
                        parsers' <- parsers'.Tail

            match result with
                |   Some x  -> Success (x, ps')
                |   _       -> Failure ("No match found", ps, ps')                         
        )

    let p_combine (pl : Parser<'a>) (pr : Parser<'b>) = (fun ps ->
        match pl ps with
            |   Success (l, ps')    ->  
                match pr ps' with
                    |   Success (r, ps'')       ->  Success((l,r), ps'')
                    |   Failure (e, _, ps'')    ->  Failure(e, ps, ps'')
            |   Failure (e, _, ps'')            ->  Failure (e, ps, ps'')
        )

    let p_keep_left (pl : Parser<'a>) (pr : Parser<'b>) = 
        let combiner = p_combine pl pr
        (fun ps ->
        match combiner ps  with
            |   Success ((l,_), ps')    ->  Success (l, ps')
            |   Failure (e, _, ps')     ->  Failure (e, ps, ps')
        )

    let p_keep_right (pl : Parser<'a>) (pr : Parser<'b>) = 
        let combiner = p_combine pl pr
        (fun ps ->
        match combiner ps  with
            |   Success ((_,r), ps')    ->  Success (r, ps')
            |   Failure (e, _, ps')     ->  Failure (e, ps, ps')
        )

    let (>>)    = p_combine
    let (.>>)   = p_keep_left
    let (>>.)   = p_keep_right          
    let (>>?)   = p_map
    let (>>??)  = p_value
    let (>>!)   = p_except

    let p_between (pp : Parser<'a>) (pr : Parser<'b>) (pe : Parser<'c>) = pp >>. pr .>> pe

    let p_indent= (fun ps ->
        Success ((), {ps with indent = ps.indent + 1})
        )

    let p_dedent= (fun ps ->
        Success ((), {ps with indent = ps.indent - 1})
        )

    let p_char ch               = (p_satisy (fun c -> c = ch) ("Expected: " + ch.ToString())) >>? (fun ch -> ()) 
    let p_string (str : string) = (p_satisy_fixed (fun i c -> (i < str.Length) && (str.[i] = c))) str.Length ("Expected:" + str) >>? (fun str -> ()) 

    let p_whitespace            = p_satisy Char.IsWhiteSpace "Expected whitespace"
    let p_whitespaces           = p_satisy_many (fun _ c -> Char.IsWhiteSpace(c))

    let p_cr                    = p_choose [p_char '\r'; p_eos;]
    let p_ln                    = p_choose [p_char '\n'; p_eos;]
    let p_tab                   = p_char '\t'

    let p_eol = (fun ps ->
        match p_cr ps with
            |   Success (_, ps')            -> 
                match p_ln ps' with
                    |   Success (_, ps'')   -> Success ((), ps'')
                    |   Failure _           -> Success ((), ps')
            |   Failure _                   -> p_ln ps        
        )

    let p_indention = 
        (fun ps ->
            let s = ps.pos 
            let e = min ps.input.Length (ps.pos + ps.indent)
            let mutable r = true
            for x in s..(e - 1) do
                r <- (ps.input.[x] = '\t') && r 
            
            if r 
            then    Success ((), move_to ps (ps.pos + ps.indent))
            else    Failure("Expected indent", ps , move_to ps (ps.pos + ps.indent))    
        )     


