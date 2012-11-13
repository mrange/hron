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


lexcial structure 
-----------------
1. Comments in hron begin with "#" and continue to the end of the line. 
2. Containing only blanks, tabs, and comments are called blank lines. Outside a string they have no effect on idention and are ignored. When expecting a string they count as an empty line
3. Tabs immediately at the beginnings of lines are significant. The indention level is the number of tabs at the beginning of line. 
4. If one line is indented more than the preceding non-blank line, it is taken to be preceded by an INDENT token. It is an error for the first non-blank line in a file to be indented. 
5. If one line is indented less than the preceding non-blank line, it is taken to be preceded by enough DEDENT tokens to match all unmatched INDENT tokens introduced by preceding more-indented lines. The end of a file is preceded by enough DEDENT tokens to match all unmatched INDENT tokens. 

EBNF 
----

```ebnf

string ::= <any source character except NEWLINE>

value ::= "=" string NEWLINE INDENT (string NEWLINE)* DEDENT

object ::= "@" string NEWLINE INDENT (member NEWLINE)* DEDENT

member ::= value | object

hron ::= member*

```
