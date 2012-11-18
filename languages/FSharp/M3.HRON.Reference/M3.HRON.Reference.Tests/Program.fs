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

[<EntryPoint>]
let main argv = 
    let input = File.ReadAllText(@"..\..\..\..\..\..\reference-data\helloworld.hron")     
    let result = Parser.parse HRONParser.p_hron input
    match result with 
        |   Success (doc, ps)   ->  Console.WriteLine("Success")
        |   Failure (_,_)       ->  Console.WriteLine("Failure")
    0 
