open test

open M3.HRON.FSharp

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
			12356
"""

[<Test>]
let ``Basic HRON tests`` () : unit = 
  expect {
    let! hron = expect_some <| HRON.parse simple
    let databaseConnections = hron.Query ? DataBaseConnection
    let customerConnection = databaseConnections.[0]
    let name = customerConnection ? Name
    let! _ = expect_eq "CustomerDB" name.AsString
    let partnerConnection = databaseConnections.[1]
    return ()
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
