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

open UnitTest

open M3.HRON.FSharp

open HRON

let simple = """
# This is an ini file using hron

# object values are started with '@'
@Greeting
	=Title
		Hello World from hron!
	=WelcomeMessage
		Hello there!

		String values in hron are started with '='

		Just as in Python, indentation is significant in hron

		Indentation promotes readability but also allows hron string values 
		to be multi-line and relieves them from the need for escaping. 

		Let us say that again, there exists _no_ character escaping in hron. 

		Letters like this are fine in an hron string: &<>\"'@=

		This helps readability!
@DataBaseConnection
	=Name
		CustomerDB
	=ConnectionString
		Data Source=.\SQLEXPRESS;Initial Catalog=Customers
	=TimeOut
		10
	@User
		=UserName
			ATestUser
		=Password
			123
@DataBaseConnection
	=Name
		PartnerDB
	=ConnectionString
		Data Source=.\SQLEXPRESS;Initial Catalog=Partner
	=TimeOut
		30
	@User
		=UserName
			AnotherTestUser
		=Password
			12345"""

let expect_hstring (expected : string) (actual : HRONQuery) = expect_eq expected actual.AsString

[<Test>]
let ``Basic HRON tests`` () : unit = 
  let checkConnection (connection : HRONQuery) dbName connectionString timeOut user pwd =
    expect {
      do! expect_hstring  dbName            (connection ? Name            )
      do! expect_hstring  connectionString  (connection ? ConnectionString)
      do! expect_hstring  timeOut           (connection ? TimeOut         )

      let u = connection ? User
      do! expect_hstring user (u ? UserName)
      do! expect_hstring pwd  (u ? Password)
    }

  expect {
    let! hron = assert_some <| parse simple
    let query = hron.Query

    let greeting = query ? Greeting ? Title
    do! expect_hstring "Hello World from hron!" greeting

    let conns = query ? DataBaseConnection
    do! checkConnection conns     "CustomerDB"  @"Data Source=.\SQLEXPRESS;Initial Catalog=Customers" "10" "ATestUser"        "123"
    do! checkConnection conns.[0] "CustomerDB"  @"Data Source=.\SQLEXPRESS;Initial Catalog=Customers" "10" "ATestUser"        "123"
    do! checkConnection conns.[1] "PartnerDB"   @"Data Source=.\SQLEXPRESS;Initial Catalog=Partner"   "30" "AnotherTestUser"  "12345"

  } |> run


[<EntryPoint>]
let main argv = 
  try
    runTests ()
  with
  | e -> 
    errorf "EXCEPTION: %s" e.Message

  if errors > 0 then
    errorf "%d errors detected" errors
    999
  else
    success "All tests passed"
    0
