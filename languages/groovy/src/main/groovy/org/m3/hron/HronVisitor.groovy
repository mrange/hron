package org.m3.hron

/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 8:19 PM
 * To change this template use File | Settings | File Templates.
 */
public interface HronVisitor {
  HronVisitor objectPropertyVisitStarted(String propertyName)
  void objectPropertyVisitEnded()

  Appendable stringPropertyVisitStarted(String propertyName)
  void stringPropertyVisitEnded(String propertyName, Appendable property)


}