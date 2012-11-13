package org.m3.hron


import spock.lang.Shared
import spock.lang.Unroll
import spock.lang.Specification


/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 11:48 PM
 * To change this template use File | Settings | File Templates.
 */
class HronParserSpecification  extends Specification  {
  @Shared HronParser parser

  def setup() {
    parser = new HronParser()
  }

  def cleanup() {

  }

  def setupSpec() {

  }

  def cleanupSpec() {
  }

  String loadTestFile(String fileName) {
    URL url = this.class.classLoader.getResource('testfiles')
    if (url == null)
      throw new FileNotFoundException("Unable to locate 'testfiles' folder in test classpath")

    File parent = new File(url.toURI())
    new File(parent, fileName).text
  }

  @Unroll
  def "should return #result from expression #expression for file #file"() {
    given:
      String hron = loadTestFile(file)

    when:
      Map result = parser.parseText(hron) as Map


    then:
      result instanceof Map

    where:
      file          | expression  | result
      'simple.hron' |
  }
}
