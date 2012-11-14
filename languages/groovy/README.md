About This Project
==================
This project contains a groovy parser for the hron data format. Hron stands
for Human Readable Object Notation, please see the root hron project for details on the format.

The parser mimics the JsonSlurper and XmlParser usage patterns already available in groovy. It however
adds to these by also supporting custom visitors so you can hook into the hron parsing process and
do some custom processing if needed.

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
    	@author
    		=firstName
    			Bob
    		=lastName
    			Developer
    """
    
    def hron = new HronParser().parseText(hronBlob)
    
    assert hron instanceof Map 
    assert hron.welcome.title == "Welcome to HRON"
    assert hron.welcome.copy.readLines()[5] == "The Developers"
    assert hron.welcome.author instanceof Map
    assert hron.welcome.author.firstName == "Bob"

Note that indentation is significant in the hron format and that indentation is performed using TAB characters. The above example can be run 
by first building the project and then executing: 

    > groovy -cp build/libs/hron-parser-1.0.jar sample.groovy

For a few more examples of how you can use the parser, take a look at the [spock specification](https://github.com/mbjarland/hron/blob/master/languages/groovy/src/test/groovy/org/m3/hron/HronParserSpecification.groovy)
for the parser in src/test/groovy. For details on the excellent BDD framework spock, see [the spock web site](http://code.google.com/p/spock/).

Building The Project
====================
The project is configured as a gradle build project (for details on the gradle build system, see [the gradle home page](http://gradle.org)).

The only thing you need installed on your machine to build the project is java. To build, run the following:

  > ./gradlew clean build

from the root of the project tree (replace with 'gradlew.bat' for windows). This will compile and test the project
and generate a jar file in directory build/libs.


