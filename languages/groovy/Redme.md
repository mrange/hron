About This Project
==================
This project contains a groovy parser for the hron data format. Hron stands
for Human Readable Data Format, please see the root hron project for details on the format.

The parser mimics the JsonSlurper and XmlParser usage patterns already available in groovy. It however
adds to these by also supporting custom visitors so you can hook into the hron parsing process and
do some custom processing if needed.

Following is a typical usage of the parser from groovy:

    def hron = """\
    @greet
      =title
        Weldome to HRON
      =copy
        HRON is a new data format which
        which is easy to read and
        supports multi line strings.

        Best,
        The Developers"""

    def hron = new HronParser().parseText(hron)

    assert hron.title == "Welcome to HRON"
    assert hron.copy.readLines()[5] == "The Developers"

For a few more examples of how you can use the parser, take a look at the spock specifications
in src/test/groovy (see [the spock web site](http://code.google.com/p/spock/) for details on spock).

Building The Project
====================
The project is configured as a gradle build project (see [the gradle home page](http://gradle.org) for details).

The only thing you need installed on your machine to build the project is java. To build, run the following:

  > ./gradlew clean build

from the root of the project tree (replace with 'gradlew.bat' for windows). This will compile and test the project
and generate a jar file in directory build/libs.

