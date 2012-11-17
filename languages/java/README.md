NOTE
====
This project is in its infancy. It will stabilize within the next few days but as of now, take anything here (including this readme) 
with a grain of salt. 

About This Project
==================
This project contains a pure java parser for the hron data format. Hron stands
for Human Readable Object Notation, please see the root hron project for details on the format.

The parser mimics the JsonSlurper and XmlParser usage patterns from groovy. It however
adds to these by also supporting custom visitors so you can hook into the hron parsing process and
do some custom processing if needed.

Following is a typical usage of the parser from java:

    import org.m3.hron.HronParser;
    
    File hronFile = new File('src/samples/sample.hron');
    Map<String, Object> hron = new HronParser().parseFile(hronFile)
    
    assert hron.get("welcome").get("title").equals("Welcome to HRON");
    
    println "Hron sample successfully executed!"

Note that indentation is significant in the hron format and that indentation is performed using TAB characters. The above example can be run 
by (first build the project jar file and then execute a sample groovy script against it): 

    > gradlew clean build 
    > java -jar build/libs/hron-parser-java-1.0.jar
    Hron sample successfully executed!

For a few more examples of how you can use the parser, take a look at the [spock specification](https://github.com/mbjarland/hron/blob/master/languages/groovy/src/test/groovy/org/m3/hron/HronParserSpecification.groovy)
for the parser in src/test/groovy. For details on the excellent BDD framework spock, see [the spock web site](http://code.google.com/p/spock/).

Building The Project
====================
The project is configured as a gradle build project (for details on the gradle build system, see [the gradle home page](http://gradle.org)).

The only thing you need installed on your machine to build the project is java. To build, run the following:

  > gradlew clean build

from the root of the project tree (replace with 'gradlew.bat' for windows). This will compile and test the project
and generate a jar file in directory build/libs.


TODO
====
* reimplement the tests
* rewrite this readme to match java (it is transplanted from groovy)
* implement parse methods with string and reader arguments


