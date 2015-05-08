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

module UnitTest

open Microsoft.FSharp.Core

open System
open System.Reflection

let mutable errors  = 0

let print (cc : ConsoleColor) (prefix : string) (msg : string) : unit =
  let old = Console.ForegroundColor
  try
    Console.ForegroundColor <- cc
    Console.Write prefix
    Console.WriteLine msg
  finally
    Console.ForegroundColor <- old

let error   msg   = 
  errors <- errors + 1
  print ConsoleColor.Red    "ERROR    : "  msg
let info      msg = print ConsoleColor.Gray   "INFO     : " msg
let warning   msg = print ConsoleColor.Yellow "WARNING  : " msg
let success   msg = print ConsoleColor.Green  "SUCCESS  : " msg
let highlight msg = print ConsoleColor.White  "HIGHLIGHT: " msg

let errorf      fmt = Printf.kprintf error      fmt
let infof       fmt = Printf.kprintf info       fmt
let warningf    fmt = Printf.kprintf warning    fmt
let successf    fmt = Printf.kprintf success    fmt
let highlightf  fmt = Printf.kprintf highlight  fmt

type Expectation<'T> = Expectation of (('T-> unit) -> unit)

let expect_eq (expected : 'T) (actual : 'T) : Expectation<unit> =
  Expectation <| fun a ->
    if expected = actual then
      a ()
    else
      errorf "EQ: %A=%A" expected actual
      a ()

let assert_some (actual : 'T option) : Expectation<'T> =
  Expectation <| fun a ->
    match actual with
    | Some v -> a v
    | _ ->
      error "SOME: None"

module ExpectationMonad =

  let Delay (ft : unit -> Expectation<'T>) : Expectation<'T> =
    ft ()

  let Return v : Expectation<'T> =
    Expectation <| fun a ->
      a v

  let Bind (t : Expectation<'T>) (fu : 'T -> Expectation<'U>) : Expectation<'U> =
    Expectation <| fun uv ->
      let (Expectation tt) = t

      tt (fun vt -> 
        let (Expectation u) = fu vt
        u uv)

  type ExpectationBuilder() =
    member x.Return v     = Return v
    member x.Bind (t,fu)  = Bind t fu
    member x.Delay ft     = Delay ft

let expect = ExpectationMonad.ExpectationBuilder ()

let (>>=) f s = ExpectationMonad.Bind f s

let run (e : Expectation<'T>) : unit =
  let (Expectation ee) = e
  ee (fun _ -> ())


[<AttributeUsage(AttributeTargets.Method)>]
[<AllowNullLiteral>]
type TestAttribute() =
  inherit Attribute()

let runTests () =
  let assembly = Assembly.GetExecutingAssembly ()
  let types = assembly.GetTypes ()

  let methods = 
    types
    |> Seq.collect  (fun t -> t.GetMethods (BindingFlags.Static ||| BindingFlags.Public))
    |> Seq.filter   (fun m -> m.GetCustomAttribute<TestAttribute>() <> null)
    |> Seq.filter   (fun m -> m.GetParameters().Length = 0)
    |> Seq.filter   (fun m -> m.ReturnType = typeof<Void>)
    |> Seq.sortBy   (fun m -> m.Name)
    |> Seq.toArray

  highlightf "Found %d tests" methods.Length

  for meth in methods do
    infof "Running test: %s" meth.Name

    try
      ignore <| meth.Invoke (null, [||])
    with
    | :? TargetInvocationException as e ->
      errorf "  threw exception: %s" e.InnerException.Message
    | e ->
      errorf "  threw exception: %s" e.Message


