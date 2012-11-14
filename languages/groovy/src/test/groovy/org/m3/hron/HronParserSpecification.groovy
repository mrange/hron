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

  String loadHronData(String fileName) {
    URL url = this.class.classLoader.getResource('testfiles')
    if (url == null)
      throw new FileNotFoundException("Unable to locate 'testfiles' folder in test classpath")

    File parent = new File(url.toURI())
    new File(parent, fileName).text
  }

  def "should handle tabs at the start of strings"() {
    given:
      def hronData = """|@bean
                        |\t=propOne
                        |\t\t\tvalue
                        |\t=propTwo
                        |\t\tfirst line
                        |\t\t\tsecond line
                        |\t\t
                        |\t\t
                        |\t=propThree
                        |\t\t
                        |\t=propFour
                        |\t\t\t
                        |\t=propFive
                        |\t\t""".stripMargin()
    when:
      def result = parser.parseText(hronData)

    then:
      result.bean instanceof Map
      result.bean.size() == 5
      result.bean.propOne == "\tvalue"
      result.bean.propTwo.readLines().size == 3
      result.bean.propTwo.readLines()[0] == "first line"
      result.bean.propTwo.readLines()[1] == "\tsecond line"
      result.bean.propTwo.readLines()[2] == ""
      result.bean.propThree == ""
      result.bean.propFour == "\t"
      result.bean.propFive == ""
  }

  /**
   * Note that we can not use the 'hron' data variable as it contains
   * line feeds and other chars which ar note allowed in method names
   * @return
   */
  @Unroll
  def "should return '#expected' from expression #expression on row #row"() {
    given:
      def result = parser.parseText(hron)

    expect:
      Eval.me("result", result, "result.${expression}") == expected

    where:
      row | hron          | expression                 | expected
      1   | "@MyBean"     | "MyBean.size()"            | 0
      2   | "@MyBean"     | "MyBean instanceof Map"    | true
      3   | "@MyBean\n"   | "MyBean.size()"            | 0
      4   | "@MyBean\n"   | "MyBean instanceof Map"    | true
      5   | "=MyProp"     | "MyProp"                   | ''
      6   | "=MyProp"     | "MyProp instanceof String" | true
      7   | "=MyProp\n"   | "MyProp"                   | ''
      8   | "=MyProp\n"   | "MyProp instanceof String" | true
  }


  def "should parse bean with no properties"() {
    given:
      def hronData = "@MyBean"

    when:
      def hron = parser.parseText(hronData)

    then:
      hron.MyBean instanceof Map
      hron.MyBean.size() == 0
  }

  def "should get correct visitor callbacks for single-object hron blob"() {
    given:
      def visitor = Mock(HronVisitor)

    when:
      parser.parseText(simpleHron, visitor)

    then:
      1 * visitor.objectPropertyVisitStarted(_, _)
      1 * visitor.objectPropertyVisitEnded(_, _, _)
      2 * visitor.stringPropertyVisitStarted(_, _)
      2 * visitor.stringPropertyVisitEnded(_, _, _)
  }


  @Unroll
  def "should return #expected from expression '#expression' for file #file"() {
    given:
      String hron = loadHronData(file)

    when:
      Map result = parser.parseText(hron) as Map

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

  def "should return correct value for multi-line string in file complex.hron"() {
    given:
      String hron = loadHronData('complex.hron')

    when:
      Map result = parser.parseText(hron) as Map
      List<String> multiLine = result.Common.WelcomeMessage.readLines()

    then:
      result instanceof Map
      multiLine.size() == 4
      multiLine[0] == "Hello there!"
      multiLine[1] == "This is a multiline script"
      multiLine[2] == ""
      multiLine[3] == "Third line"
  }

  def "should ignore comment lines for simple hron blob"() {
    given:
      String hron =   """|#######################################
                         |# I am a comment line on the root level
                         |@bean
                         |	=propOne
                         |		valueOne
                         |	######################################################
                         |	# I am a comment line at the current indentation level
                         |	=propTwo
                         |#######################################
                         |# And a comment inserted straight into the flow
                         |		valueTwo
                         |	=propThree
                         |		multi-line string value
                         |	# with a indent-level comment injected and ignored
                         |# and a root-level comment injected and ignored
                         |		second line of multi-line string value
                         |""".stripMargin()

    when:
      Map result = parser.parseText(hron) as Map

    then:
      result.bean.size() == 3
      result.bean.propOne == 'valueOne'
      result.bean.propTwo == 'valueTwo'
      result.bean.propThree.readLines().size() == 2
      result.bean.propThree.readLines()[0] == "multi-line string value"
      result.bean.propThree.readLines()[1] == "second line of multi-line string value"
  }
}
