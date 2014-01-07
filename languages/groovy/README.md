
About This Project
==================
This project contains a groovy parser for the hron data format. Hron stands
for Human Readable Object Notation, please see the [hron root project](https://github.com/mrange/hron) for 
details on the format.

**Please note**: The groovy implementation of this project is geared 
towards correctness and compactness of implementation. It can be seen as a 
reference implementation and the performance aspects of the parser have 
to a large extent been ignored. For a more performant version, please
refer to the [java version](https://github.com/mrange/hron/tree/master/languages/java)
of the parser. 

The parser mimics the [JsonSlurper](http://groovy.codehaus.org/gapi/groovy/json/JsonSlurper.html)
and [XmlParser](http://groovy.codehaus.org/api/groovy/util/XmlParser.html) usage patterns already 
available in groovy. It however adds to these by also supporting custom visitors which gives you the option to hook into the hron parsing process and
do some custom processing when needed.

Following is a typical usage of the parser from groovy:

    import org.m3.hron.HronParser

    def hronBlob = """\
    @welcome
        =title
            Welcome to HRON
        =copy
            HRON is a new data format
            which is easy to read and
            supports multi line strings.
            
            Best,
            The Developers
        @authors
            =firstName
                Bob
            =lastName
                Developer
        @
            =firstName
                Steve
            =lastName
                Stevensson
    """
        
    def hron = new HronParser().parseText(hronBlob)
        
    assert hron instanceof Map 
    assert hron.welcome.title == "Welcome to HRON"
    assert hron.welcome.copy.readLines()[5] == "The Developers"
    // Note, the groovy parser currently does not handle the hron 
    // list notation correctly. The commented out assertions should 
    // work, but do not. 
    // assert hron.welcome.authors instanceof List
    assert hron.welcome.authors instanceof Map
    // assert hron.welcome.authors[0].firstName == "Bob"
    assert hron.welcome.authors.firstName == "Bob"

    println "Hron sample successfully executed!"

Note that indentation is significant in the hron format and that indentation is performed using either TAB characters. The above example can be run by (first build the project jar file and 
then execute a sample groovy script against it): 

    > gradlew clean build 
    > groovy -cp build/libs/hron-parser-groovy-1.0.jar src/samples/sample.groovy
    Hron sample successfully executed!

For a few more examples of how you can use the parser, take a look at the [spock specification](https://github.com/mbjarland/hron/blob/master/languages/groovy/src/test/groovy/org/m3/hron/HronParserSpecification.groovy)
for the parser in src/test/groovy. For details on the excellent BDD framework spock, see [the spock web site](http://code.google.com/p/spock/).

Project Maintainer
==================
The groovy and java hron implementations are maintained by (Matias Bjarland)[https://github.com/mbjarland]
For any questions or suggestions, feel free to contact me at mbjarland@gmail.com. 

Pull requests are welcome

Building The Project
====================
The project is configured as a gradle build project (for details on the gradle build system, see [the gradle home page](http://gradle.org)).

The only thing you need installed on your machine to build the project is java. To build, run the following:

  > gradlew clean build

from the root of the project tree (replace with 'gradlew.bat' for windows). This will compile and test the project
and generate a jar file in directory build/libs.


TODO
====
* Add groovy builder for hron documents
* Add serializer 
