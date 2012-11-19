hron
====

hron - human readable object notation

XML and JSON (and others) aims to be human-readable language independent data interchange 
formats. However they are not without flaws:

XML
---
1. XML author has to take care avoiding letters such as &<> and encode them properly. 
   This hurts readability and writeability.

JSON
----
1. JSON struggles with multi-line texts.
2. JSON number values are depending on platform unserialized as double, long or decimal. 
   This makes the number value less useful as depending on how it's unserialized information 
   might be lost.
3. (JSON lacks strong deserializers on the .NET platform, there are many attempts but
   none feels as strong as the XMLDOM did already in 1998) 

hron
----

hron means human readable objection notation

1. we put human readable in the acronym in order to remind us why we started thinking about 
   hron in the first place.
2. hron supports multi-line texts
3. hron doesn't require you to escape "special" characters
4. In hron indention is important (like python) in order to eliminate end tags and 
   to improve readability
5. hron has objects and values, objects consists of members of others objects or values.
   values are always text values 

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
any_indention   ::= <parses any indention>
indention       ::= <parses the current indention>
indent          ::= <increases indention by one>
dedent          ::= <decreases indention by one>

empty_string    ::= (whitespace EXCEPT eol)*
string          ::= (anychar EXCEPT eol)*
comment_string  ::= any_indention "#" string

empty_line      ::= empty_string eol
comment_line    ::= comment_string eol
nonempty_line   ::= indention string eol
value_line      ::= nonempty_line | comment_line | empty_line
value_lines     ::= (value_line EXCEPT eos)*

value           ::= indention "=" string eol indent values_lines dedent
empty           ::= empty_string eol
comment         ::= comment_string eol
object          ::= indention "@" string eol indent members dedent
member          ::= value | object | comment | empty
members         ::= (member EXCEPT eos)* 

hron ::= members

```
