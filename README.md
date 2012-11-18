hron
====

hron - human readable object notation

hron sample
-----------

```hron

# This is an ini file using hron

# object values are started with '@'
@Common
	=LogPath
		Logs\CurrentDay
	=WelcomeMessage
		Hello there!

		String values in hron is started with '='

		Just as with Python in hron the indention is important

		Idention promotes readability but also allows hron string values 
		to be multi-line and hron has no special letters that requires escaping
		
		Letters like this causes hron no problems: &<>\"'@=

		This helps readability
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
		Data Source=.\SQLEXPRESS;Initial Catalog=Partners

```


hron grammar (EBNF)
-------------------

1. The symbol "::=" serves the same purpose as colon in Bison. 
2. Unquoted parentheses group. 
3. A trailing unquoted asterisk (*) indicates 0 or more repetitions. 
4. A trailing unquoted plus indicates 1 or more repetitions. 
5. Unquoted square braces indicate an optional phrase. 
6. "p EXCEPT q" means that parser succeeds if p succeeds and q fails

EBNF 
----

```ebnf

anychar         ::= <parses any character>
whitespace      ::= <parses any whitespace character>
eos             ::= <parses END OF STREAM>
eol             ::= <parses END OF LINE (END OF STREAM counts as END OF LINE)>
indention       ::= <parses the current indention>
indent          ::= <increases indention by one>
dedent          ::= <decreases indention by one>

empty_string    ::= (whitespace EXCEPT eol)*
string          ::= (anychar EXCEPT eol)*
comment_string  ::= empty_string "#" string

empty_line      ::= empty_string eol
comment_line    ::= comment_string eol
nonempty_line   ::= indention string eol
value_line      ::= nonempty_line | comment_line | empty_line
value_lines     ::= (value_line EXCEPT eos)*

value           ::= indention "=" eol indent values_lines dedent
empty           ::= empty_string eol
comment         ::= comment_string eol
object          ::= indention "@" eold indent members dedent
member          ::= value | object | comment | empty
members         ::= (member EXCEPT eos)* 

hron ::= members

```
