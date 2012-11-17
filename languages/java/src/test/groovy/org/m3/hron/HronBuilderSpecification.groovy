package org.m3.hron

import spock.lang.Specification
import spock.lang.Shared
import spock.lang.Unroll
import spock.lang.Ignore

/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/14/12
 * Time: 7:00 PM
 * To change this template use File | Settings | File Templates.
 */
class HronBuilderSpecification extends Specification {

  def setup() {
    parser = new HronParser()
  }

  def cleanup() {
  }

  def setupSpec() {
  }

  def cleanupSpec() {
  }


  @Ignore
  def "should get correct visitor callbacks for single-object hron blob"() {
    given:
      def builder //= Mock(HronVisitor)

    when:
      builder 2
      parser.parseText(simpleHron, visitor)

    then:
      1 * visitor.objectPropertyVisitStarted(_, _)
      1 * visitor.objectPropertyVisitEnded(_, _, _)
      2 * visitor.stringPropertyVisitStarted(_, _)
      2 * visitor.stringPropertyVisitEnded(_, _, _)
  }
}
