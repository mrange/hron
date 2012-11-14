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


  def "should get visitor callbacks for single-object hron blob"() {
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
      //result."$expression" == expected

    where:
      file           | expression                          || expected
      'simple.hron'  | 'bean'                              || [propOne: 'valueOne', propTwo: 'valueTwo']
      'simple.hron'  | 'bean.propOne'                      || 'valueOne'
      'complex.hron' | 'DataBaseConnection'                || [Name: 'CustomerDB', TimeOut: '10']
      'complex.hron' | 'grandParent.parent.child.prop1'    || 'val1'
  }
}
