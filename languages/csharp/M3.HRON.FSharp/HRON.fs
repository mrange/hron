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

namespace M3.HRON.FSharp

open System.Collections.Generic

open M3.HRON

[<Measure>]
type HRONElementKey

type HRONQueryError =
  | NotFoundError
  | IndexOutOfRangeError of int*int
  | MemberNotFoundError  of string

type HRONQueryPathPart =
  | IndexQueryPart  of int
  | MemberQueryPart of string

type HRONQueryPath = HRONQueryPathPart list

type HRONElement =
  | StringElement of string
  | ObjectElement of (int<HRONElementKey>*HRONElement) []

and HRONDocument(names : Dictionary<string, int<HRONElementKey>>, element : HRONElement) =

  member x.GetKey (nm : string) : int<HRONElementKey> =
    let mutable id = -1<HRONElementKey>
    if names.TryGetValue (nm, & id) then
      id
    else
      -1<HRONElementKey>

  member x.Element = element

  member x.Query = ElementQuery ([],x,element)

and HRONQuery =
  | ErrorQuery    of HRONQueryPath*HRONDocument*HRONQueryError
  | ElementQuery  of HRONQueryPath*HRONDocument*HRONElement
  | ElementsQuery of HRONQueryPath*HRONDocument*HRONElement []

  member x.AsString =
    let inline str path error = "ERROR"
    let inline get path (document : HRONDocument) element =
      match element with
      | StringElement s -> s
      | ObjectElement elements -> "[]"
    match x with
    | ErrorQuery (path,_,error) -> str path error
    | ElementQuery (path, document, element) -> get path document element
    | ElementsQuery (path, document, elements) when elements.Length = 0 -> ""
    | ElementsQuery (path, document, elements) ->
      let element = elements.[0]
      get path document element

  static member ( ? ) (x : HRONQuery, nm : string) : HRONQuery =
    let get path (document : HRONDocument) element =
      let key = document.GetKey nm
      match element with
      | StringElement _ ->
        ErrorQuery (path, document, MemberNotFoundError nm)
      | ObjectElement elements ->
        let result = ResizeArray<_>()
        for k,e in elements do
          if k = key then
            result.Add e
        match result.Count with
        | 0 -> ErrorQuery (path, document, MemberNotFoundError nm)
        | 1 -> ElementQuery (MemberQueryPart nm::path, document, result.[0])
        | _ -> ElementsQuery (MemberQueryPart nm::path, document, result.ToArray ())
    match x with
    | ErrorQuery _ -> x
    | ElementQuery (path, document, element) -> get path document element
    | ElementsQuery (path, document, elements) when elements.Length = 0 ->
      ErrorQuery (path, document, MemberNotFoundError nm)
    | ElementsQuery (path, document, elements) ->
      let element = elements.[0]
      get path document element

  member x.Item (idx : int) : HRONQuery = 
    match x with
    | ErrorQuery _ -> x
    | ElementQuery (path, document, element) ->
      if idx > -1 && idx < 1 then
        ElementQuery (IndexQueryPart idx::path, document, element)
      else
        ErrorQuery (path, document, IndexOutOfRangeError (idx, 1))
    | ElementsQuery (path, document, elements) ->
      if idx > -1 && idx < elements.Length then
        ElementQuery (IndexQueryPart idx::path, document, elements.[idx])
      else
        ErrorQuery (path, document, IndexOutOfRangeError (idx, elements.Length))


module HRON =

  module Details =

    type HRONVisitor() = 

      let names = Dictionary<string, int<HRONElementKey>> ()

      let key (nm : string) : int<HRONElementKey> =
        let mutable id = -1<HRONElementKey>
        if names.TryGetValue (nm, & id) then
          id
        else
          id <- names.Count*1<HRONElementKey>
          names.Add (nm, id)
          id

      let ss (baseString : string) (beginIndex : int) (endIndex : int) : string =
        baseString.Substring(beginIndex, endIndex - beginIndex)

      let mutable keys    = [-1<HRONElementKey>]
      let mutable context = [ResizeArray<int<HRONElementKey>*HRONElement> ()]
      let value           = System.Text.StringBuilder ()
      let errors          = ResizeArray<int*string*string> ()
      interface IHRONVisitor with
          member x.Document_Begin() = 
            // Ignored
            ()
          member x.Document_End() = 
            // Ignored
            ()
          member x.PreProcessor(baseString : string, beginIndex : int, endIndex : int) = 
            // Ignored
            ()
          member x.Empty(baseString : string, beginIndex : int, endIndex : int) = 
            // Ignored
            ()
          member x.Comment(indent : int, baseString : string, beginIndex : int, endIndex : int) =
            // Ignored
            ()
          member x.Value_Begin(baseString : string, beginIndex : int, endIndex : int) =
            x.PushValue <| ss baseString beginIndex endIndex
          member x.Value_Line(baseString : string, beginIndex : int, endIndex : int) =
            ignore <| value.Append (baseString, beginIndex, endIndex - beginIndex)
            ignore <| value.AppendLine ()
          member x.Value_End() =
            let kv = x.PopValue ()
            context.Head.Add kv
          member x.Object_Begin(baseString : string, beginIndex : int, endIndex : int) =
            x.PushObject <| ss baseString beginIndex endIndex
          member x.Object_End() = 
            let kv = x.PopObject ()
            context.Head.Add kv
          member x.Error(lineNo : int, parseError : string, baseString : string, beginIndex : int, endIndex : int) =
            errors.Add(lineNo, parseError, ss baseString beginIndex endIndex)

      member x.PushValue (name : string) : unit =
        keys <- key name::keys
        ignore<| value.Clear ()

      member x.PopValue () : int<HRONElementKey>*HRONElement =
        let key = keys.Head
        keys <- keys.Tail
        key, StringElement (value.ToString ())

      member x.PushObject (name : string) : unit =
        keys <- key name::keys
        context <- (ResizeArray<_>())::context
        ()

      member x.PopObject () = 
        let key = keys.Head
        keys <- keys.Tail
        let head = context.Head
        context <- context.Tail
        key, ObjectElement (head.ToArray ())

      member x.CreateDocument () =
        let head = context.Head
        HRONDocument(names, ObjectElement (head.ToArray()))

  let parse (hron : string) : HRONDocument option = 
    let v = Details.HRONVisitor ()
    if HRONSerialization.TryParse (hron, v) then
      Some <| v.CreateDocument ()
    else
      None
