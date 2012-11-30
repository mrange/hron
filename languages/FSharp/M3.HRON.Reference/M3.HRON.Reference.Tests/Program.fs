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

open System
open System.IO
open M3.HRON.Reference

let min_empty_string = 0
let max_empty_string = 10

let min_string = 4
let max_string = 10

let min_member = 5
let max_member = 10

let min_indent = 0
let max_indent = 4

let min_value_lines = 0
let max_value_lines = 5

let max_level = 5

let build_empty_char (random : Random)=
    let c = random.Next(0,2)
    match c with    
        |   0 -> '\t'
        |   1 -> ' '
let build_empty_string (random : Random)=
    let c = random.Next(min_empty_string, max_empty_string)
    new string ([for i in 0..c -> build_empty_char random] |> List.toArray)

let build_char (random : Random)=
    let c = random.Next(0,25)
    (char)(c + 65)

let build_string (random : Random)=
    let c = random.Next(min_string, max_string)
    new string ([for i in 0..c -> build_char random] |> List.toArray)

let build_value_line (random : Random)=
    let c = random.Next(0, 10)
    match c with
        |   0           -> CommentLine  (random.Next(min_indent, max_indent), build_string random)
        |   _ when c < 4-> EmptyLine    (build_empty_string random)
        |   _           -> ContentLine  (build_string random)

let build_value_lines (random : Random)=
    let c = random.Next(min_value_lines, max_value_lines)
    [|for i in 0..c -> build_value_line random|]

let rec build_member l (random : Random)=
    let c = random.Next(0, 10)
    match c with
        |   0           -> Empty    (build_empty_string random)
        |   1           -> Comment  (random.Next(min_indent, max_indent), build_string random)
        |   _ when c < 5-> Value    (build_string random, build_value_lines random)
        |   _           -> Object   (build_string random, build_members l random)
and build_members l (random : Random)= 
    let c = random.Next(min_member, max_member)
    if l > 0
    then [|for i in 0..c -> build_member (l - 1) random|]
    else [||]

let build_random() =
    let random = new Random (19740531)

    let x = build_members max_level random
    let y = HRONParser.to_string ([||], x)
    File.WriteAllText ("random.hron", y)

[<EntryPoint>]
let main argv = 
//    build_random()

    let path  = Path.GetFullPath(@"..\..\..\..\..\..\reference-data\random.hron")
    let input = File.ReadAllText(path)
    printf "Loaded: %s\r\n" path     
    let result = Parser.parse HRONParser.p_hron input
    match result with 
        |   Success (doc, ps)   ->  
            let output = HRONParser.to_string doc
            let l1              = input.Length
            let l2              = output.Length
            let lm              = min l1 l2
            let mutable first   = -1
            for i in 0..(lm - 1) do
                if first = -1 && input.[i] <> output.[i] 
                    then first <- i
            if first = -1 && l1 = l2
            then printf "Successfully read, parsed and serialized\r\n"
            elif first = -1
            then printf "Length mismatch %d, %d\r\n" l1 l2
            else printf "First mismatch@ %d\r\n" first 
        |   Failure (_,_)       ->  printf "Failed to parse\r\n"
    
    0 
