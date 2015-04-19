C# HRON parser
==============

The C# HRON parser is intended to be the goto HRON parser for .NET and Mono.

Priority for the C# parser are:
  1. Correctness
  2. Usability
  3. Performance
  4. Support parsing of large HRON files

Correctness
-----------

The C# HRON parser is compared with the [reference parser](https://github.com/mrange/hron/tree/master/languages/fsharp) 
using so called action logs. The action logs are produced from the 
[reference data](https://github.com/mrange/hron/tree/master/reference-data) and compared using the 
[parser validator](https://github.com/mrange/hron/tree/master/tools/ParserValidator).

The reference data is complex and covers many cases and should ensure the core parser is of good quality.

The mapping of HRON to objects are simiplied by choosing a data structure that closely match the HRON format
minimizing risks for bugs that arise from trying to map two fundamentally incompatible structures (such as mapping
XML to a C# class).

Usability
---------

The C# HRON parser provides low-level API using visitors that allows developers to write their own serializers.

As HRON is a fundamentally schemaless and dynamic format it matches well to C# dynamic and in our opinion
should be the preferred way to consume HRON.

    string hron = File.ReadAllText();
    dynamic hron = hron.ParseAsHRON();

    string logPath          = hron.Common.LogPath;
    string welcomeMessage   = hron.Common.WelcomeMessage;

    dynamic[] databaseConnections = hron.DataBaseConnection;

    foreach (var databaseConnection in databaseConnections)
    {
        string name             = databaseConnection.Name;
        string connectionString = databaseConnection.ConnectionString;
        int    timeOut          = databaseConnection.TimeOut;

        string userName         = databaseConnection.User.UserName;
        string password         = databaseConnection.User.Password;

        UserType userType       = databaseConnection.User.Type;
    }

The HRON document

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
            
            Letters like this causes hron no problems: &<>\'@=

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
            =Type
                SuperUser
    @DataBaseConnection
        =Name
            PartnerDB
        =ConnectionString
            Data Source=.\SQLEXPRESS;Initial Catalog=Partners

Performance
-----------

The C# HRON parser should have good performance and with blank visitors are able to read
roughly ~20,000,000 rows/sec from memory. This roughly translates to ~15 cycles per 
character which I find acceptable considering the example document couldn't fit in the 
L2 cache.

Large file suppport
-------------------

Dynamic objects are a great way to consume HRON document but if one have huge HRON documents
(such as log files) that may be too slow or memory consuming. This is also a reason to use the 
visitor based parsers as they leave it to the developer how an HRON document should be translated
into objects.

License
------------------------------
This library is released under Microsoft Public License (Ms-PL).
