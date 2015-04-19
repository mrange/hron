package org.m3.hron

/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 8:19 PM
 * To change this template use File | Settings | File Templates.
 */
public interface HronVisitor {
  Object objectPropertyVisitStarted(Object parent, String propertyName)
  void objectPropertyVisitEnded(Object parent, String propertyName, Object child)

  Appendable stringPropertyVisitStarted(Object parent, String propertyName)
  void stringPropertyVisitEnded(Object parent, String propertyName, Appendable property)

  void error(Object parent, long line, int column, HronParseException error)
}