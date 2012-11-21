hron
====

hron - Human Readable Object Notation

XML and JSON (and others) aim to be human-readable, language independent, data interchange 
formats. However they are not without flaws:

XML
---
1. The XML author has to take care avoiding letters such as &<> and make sure to encode them properly. 
   This hurts readability and writeability.

JSON
----
1. JSON struggles with multi-line texts.
2. JSON number values are, depending on platform, unserialized as double, long or decimal. 
   This makes JSON number values less useful as, depending on how the value is unserialized,
	 information may be lost. We have personally encountered this in a financial application 
   with disastrous end results.
3. JSON lacks strong deserializers on the .NET platform. There are many attempts but
   none feel as strong as the XMLDOM did already in 1998. 

hron
----

hron stands for human readable objection notation:

1. we put human readable in the acronym to remind us why we started thinking about 
   hron in the first place.
2. hron supports multi-line texts
3. hron doesn't require you to escape "special" characters
4. In hron, we made indention significant (as in python) in order to eliminate end tags and 
   to improve readability
5. hron has objects and values. Objects consist of other objects or values.
   Values are always text values. 
6. There is no special array type. Arrays are created by first adding a named object or value 
   and then just adding another one witout a name. This creates a two element array.

hron sample
-----------

```hron

# This is an ini file using hron

# object values are started with '@'
@Greeting
	=Title
		Hello World from hron!
	=WelcomeMessage
		Hello there!

		String values in hron are started with '='

		Just as in Python, indentation is significant in hron

		Idention promotes readability but also allows hron string values 
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

# As we don't 'name' the below object, this will implicitly create an array out of the 
# above DataBaseConnection and the below object. Like in real life, adding an apple 
# next to an existing apple does not create some new concept of "array of apples", 
# two apples next to each other implicitly constitute a "collection of apples" 
# without the need for any special sauce (pun intended). 
@
	=Name
		PartnerDB
	=ConnectionString
		Data Source=.\SQLEXPRESS;Initial Catalog=Partners

```

The above would (in groovy/java lingo, replace Map with dictionary for .Net) parse to a map with
two keys: 'Greeting' and 'DataBaseConnection' where the value for key Greeting is a map with two string 
values and the value for key DataBaseConnection is a list which contains two map objects. For a good examlpe
of what is an is not valid, take a look at 
[the parser unit tests](https://github.com/mbjarland/hron/blob/master/languages/groovy/src/test/groovy/org/m3/hron/HronParserSpecification.groovy)
, some which are very descriptive. 

Is There a Parser for Language X?
---------------------------------
Possibly! Check in the [languages sub directory](https://github.com/mrange/hron/tree/master/languages).

We are busy implementing parsers in various languages, some as reference implementations with little 
concern for the parser performance (i.e. the groovy parser), others with specific performance metrics 
in mind (like the java one). Shortly upcoming parsers include scala and c++. 

If you can not find a parser in your language, fork the repo and write one! We welcome all contributions. 
We would especially welcome an implementation in javascript and any insane implementations in long forgotten 
or obscure languages, because...well because we like computer languages. 

We are also working on a standardized set of test files, both positive and negative, which will
constitute a smoke test for new parsers. 

hron grammar (EBNF)
-------------------
Rules for the grammar notation: 

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

preprocessor    ::= "!" string eol
preprocessors   ::= preprocessor*

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

hron ::= preprocessors members

```

Who Are We?
-----------
Just a few guys who got tired of name spaces and entity escaping and figured the world 
would be a better place with a simple, human readable data interchange format. 

Current contributors: 

#### Mårten Rånge
hron language specification, BNF, and implementations in CSharp and FSharp. 

marten.range@gmail.com

#### Matias Bjarland
Contributions to hron language specification, implementations in groovy and java.

mbjarland@gmail.com

#### Mattias Karlsson
Together with Mårten responsible for spawning the idea for hron. Contributions to language 
specification, invaluable feedback. 


