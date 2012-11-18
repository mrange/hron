namespace M3.HRON.Reference

open System
open System.Diagnostics

type ParserState    = {input : string; pos : int; indent : int;}

type ParserResult<'a>  =
    |   Success of 'a*ParserState
    |   Failure of string*ParserState

type Parser<'a>     = ParserState -> ParserResult<'a>

module Parser =
    
    let parse (p : Parser<'a>) s    =
        let ps = {input = s; pos = 0; indent = 0}
        p ps

    let advance     ps      = {ps with pos = ps.pos + 1}
    let is_not_eos  ps      = ps.pos < ps.input.Length
    let is_eos      ps      = ps.pos >= ps.input.Length
    let is_success  pr      =   match pr with
                                    |   Success(_,_)    -> true
                                    |   _               -> false   

    let p_success v         = (fun ps -> Success(v, ps))
    
    let p_failure v         = (fun ps -> Failure(v, ps))

    let current_pos ps      = 
        let center  = 0
        let width   = 16
        let start = max (ps.pos - center) 0
        let stop = min (ps.pos + width + (start - (ps.pos - center))) ps.input.Length  
        ps.input.Substring(start, stop - start).Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t")

    let p_debug p           = (fun ps -> 
//        if System.Diagnostics.Debugger.IsAttached then System.Diagnostics.Debugger.Break ()
        if (is_eos ps) then printf "DEBUG: EOS"
        else (printf "DEBUG: pos:%d indent:%d %s\r\n" ps.pos ps.indent (current_pos ps))        
        p ps
        )

    let p_eos               = (fun ps -> if is_eos ps then Success((), ps) else Failure("Expected EOS", ps))

    let p_any_char      = (fun ps -> 
        if is_not_eos ps
            then Success (ps.input.[ps.pos], advance ps)
            else Failure ("EOS", ps)
        )

    let p_satisy test e = (fun ps -> 
        if is_not_eos ps
            then
                if (test ps.input.[ps.pos]) 
                    then Success (ps.input.[ps.pos], advance ps)
                    else Failure (e, ps)
            else Failure ("EOS", ps)
        )

    let p_map (p : Parser<'a>) m = (fun ps ->
        match p ps with
            |   Success(v, ps') ->  Success(m v, ps')
            |   Failure(e, _)   ->  Failure(e,ps)
        )

    let p_many (p : Parser<'a>)  = 
        let rec build ps = 
            match p ps with
                |   Success (v, ps')    ->  
                    let (vs, ps'') = build ps'
                    (v::vs, ps'')
                |   Failure (_, _)      ->
                    ([], ps)
        (fun ps ->
            let (vs, ps) = build ps 
            Success (vs, ps)
        )

    let p_opt (p : Parser<'a>)  = (fun ps ->
        match p ps with
            |   Success (v, ps')    ->  Success (Some v, ps')
            |   Failure (_, _)      ->  Success (None, ps)
        )
        
    let p_except (p : Parser<'a>) (pe : Parser<'b>) = (fun ps ->
        match pe ps with
            |   Success (_, _)  ->  Failure ("Unexpected", ps)
            |   Failure (_, _)  ->  p ps
        )

    let p_choose (parsers : Parser<'a> list) = 
        let rec pick_first parsers' ps =
            match parsers' with
                |   p::parsers''    -> 
                    match p ps with
                        |   Success(v, ps') ->  Success(v, ps')
                        |   _               ->  pick_first parsers'' ps
                |   _               -> Failure ("No match", ps) 
        (fun ps ->
            pick_first parsers ps
        )

    let p_combine (pl : Parser<'a>) (pr : Parser<'b>) = (fun ps ->
        match pl ps with
            |   Success (l, ps')    ->  
                match pr ps' with
                    |   Success (r, ps'')   ->  Success((l,r), ps'')
                    |   Failure (e, _)      ->  Failure(e, ps)
            |   Failure (e, _)      ->  Failure (e, ps)
        )

    let p_keep_left (pl : Parser<'a>) (pr : Parser<'b>) = 
        let combiner = p_combine pl pr
        (fun ps ->
        match combiner ps  with
            |   Success ((l,_), ps')    ->  Success (l, ps')
            |   Failure (e, _)          ->  Failure (e, ps)
        )

    let p_keep_right (pl : Parser<'a>) (pr : Parser<'b>) = 
        let combiner = p_combine pl pr
        (fun ps ->
        match combiner ps  with
            |   Success ((_,r), ps')    ->  Success (r, ps')
            |   Failure (e, _)          ->  Failure (e, ps)
        )

    let p_indent= (fun ps ->
        Success ((), {ps with indent = ps.indent + 1})
        )

    let p_dedent= (fun ps ->
        Success ((), {ps with indent = ps.indent - 1})
        )

    let (>>)    = p_combine
    let (.>>)   = p_keep_left
    let (>>.)   = p_keep_right          
    let (>>?)   = p_map
    let (>>!)   = p_except

    let p_char ch   = (p_satisy (fun c -> c = ch) ("Expected: " + ch.ToString())) >>? (fun ch -> ()) 

    let p_cr        = p_choose [p_char '\r'; p_eos;]
    let p_ln        = p_choose [p_char '\n'; p_eos;]
    let p_tab       = p_char '\t'

    let p_eol = (fun ps ->
        match p_cr ps with
            |   Success (_, ps')            -> 
                match p_ln ps' with
                    |   Success (_, ps'')   -> Success ((), ps'')
                    |   Failure (_, _)      -> Success ((), ps')
            |   Failure (_, _)              -> p_ln ps        
        )

    let p_indention = 
        let rec consume_indention ps current_indent= 
            if current_indent = ps.indent then Some ps
            elif (is_not_eos ps) && ps.input.[ps.pos] = '\t' then consume_indention (advance ps) (current_indent + 1)
            else None
        (fun ps ->
            match consume_indention ps 0 with
                |   Some ps'    ->  Success ((), ps')
                |   _           ->  Failure("Expected indent", ps)
        )     


