package org.m3.hron


import spock.lang.Shared
import spock.lang.Unroll
import spock.lang.Specification
import spock.lang.IgnoreRest

/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 11:48 PM
 * To change this template use File | Settings | File Templates.
 */
class HronParserSpecification  extends Specification  {
  @Shared HronParser parser
  @Shared String simpleHron =  """|@bean
                                  |\t=propOne
                                  |\t\tvalueOne
                                  |\t=propTwo
                                  |\t\tvalueTwo""".stripMargin()

  def setup() {
    parser = new HronParser()
  }

  def cleanup() {
  }

  def setupSpec() {
  }

  def cleanupSpec() {
  }

  File getHronFile(String fileName) {
    URL url = this.class.classLoader.getResource('testfiles')
    if (url == null)
      throw new FileNotFoundException("Unable to locate 'testfiles' folder in test classpath")

    File parent = new File(url.toURI())

    new File(parent, fileName)
  }

  String loadHronData(String fileName) {
    getHronFile(fileName).text
  }

  @Unroll
  def "should return #expected from expression '#expression' for file #file"() {
    given:
      File hronFile = getHronFile(file)

    when:
      Map result = parser.parseFile(hronFile) as Map

    then:
      result instanceof Map
      Eval.me("result", result, "result.${expression}") == expected

    where:
      file           | expression                          || expected
      'simple.hron'  | 'bean'                              || [propOne: 'valueOne', propTwo: 'valueTwo']
      'simple.hron'  | 'bean.propOne'                      || 'valueOne'
      'complex.hron' | 'DataBaseConnection'                || [Name: 'CustomerDB', TimeOut: '10']
      'complex.hron' | 'grandParent.parent.child.prop1'    || 'val1'
  }
}
