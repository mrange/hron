open System
open System.IO
open M3.HRON.Reference

[<EntryPoint>]
let main argv = 
    let input = File.ReadAllText(@"..\..\..\..\..\..\reference-data\simple.hron")     
    let result = Parser.parse HRONParser.p_hron input
    match result with 
        |   Success (doc, ps)   ->  Console.WriteLine("Success")
        |   Failure (_,_)       ->  Console.WriteLine("Failure")
    0 
