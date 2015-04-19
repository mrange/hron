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

let write_value_line (output : StreamWriter) (vl: ValueLine) =
    match vl with
        |   EmptyLine   s       -> output.WriteLine("EmptyLine:{0}", s)
        |   CommentLine (i,s)   -> output.WriteLine("CommentLine:{0},{1}", i, s)
        |   ContentLine s       -> output.WriteLine("ContentLine:{0}", s)

let rec write_member (output : StreamWriter) (m: Member) =
    match m with
        |   Empty s             ->  output.WriteLine("Empty:{0}", s)
        |   Comment (i,s)       ->  output.WriteLine("Comment:{0},{1}", i, s)
        |   Object (name,ms')   ->  output.WriteLine("Object_Begin:{0}", name)
                                    for m' in ms' do
                                        write_member output m'
                                    output.WriteLine("Object_End:{0}", name)
        |   Value (name, vl')   ->  output.WriteLine("Value_Begin:{0}", name)
                                    for v' in vl' do
                                        write_value_line output v'
                                    output.WriteLine("Value_End:{0}", name)


let write_hron (output : StreamWriter) (hron : HRON) =
    let (pps, ms) = hron
    for pp in pps do
        output.WriteLine("PreProcessor:{0}", pp)
    for m in ms do
        write_member output m

let build_action_log (hron : string) (action_log : string)=
    let input = File.ReadAllText(hron)
    use output = new StreamWriter(action_log)
    let result = Parser.parse HRONParser.p_hron input
    match result with
        |   Success (doc, ps)   -> write_hron output doc
        |   Failure (msg, _, _) -> failwithf "Parse failure: %s" msg
[<EntryPoint>]
let main argv =
    let hrons = Directory.GetFiles (@"..\..\..\..\..\..\reference-data", "*.hron")
                    |> Array.map Path.GetFullPath
    for hron in hrons do
        let action_log = hron + ".actionlog"
        printfn "Build action log for: %s" hron
        try
            build_action_log hron action_log
        with
            |   exc -> printfn "Failed to build action log: %s" exc.Message

    0 // return an integer exit code
