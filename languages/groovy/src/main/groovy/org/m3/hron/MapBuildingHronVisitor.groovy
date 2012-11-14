package org.m3.hron

/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 9:01 PM
 * To change this template use File | Settings | File Templates.
 */
class MapBuildingHronVisitor implements HronVisitor {
  Map<String, Object> map = [:]

  private MapBuildingHronVisitor checkParent(HronVisitor parent) {
    if (!(parent instanceof MapBuildingHronVisitor))
      throw new HronParseException("Received invalid visitor class ${parent.class.name}, expeceted ${this.class.name}")

    parent as MapBuildingHronVisitor
  }

  @Override
  Object objectPropertyVisitStarted(Object parent, String propertyName) {
    Map<String, Object> child = [:]

    Map parentMap = (parent == null) ? map : parent as Map
    parentMap[propertyName] = child

    child
  }

  @Override
  void objectPropertyVisitEnded(Object parent, String propertyName, Object child) {
    //do nothing for now
  }

  @Override
  Appendable stringPropertyVisitStarted(Object parent, String propertyName) {
    new StringBuilder()
  }

  @Override
  void stringPropertyVisitEnded(Object parent, String propertyName, Appendable propertyValue) {
    Map parentMap = (parent == null) ? map : parent as Map
    parentMap[propertyName] = propertyValue.toString()
  }
}
