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
  HronVisitor objectPropertyVisitStarted(String propertyName) {
    map[propertyName] = new MapBuildingHronVisitor()
  }

  @Override
  void objectPropertyVisitEnded() {
    //do nothing for now
  }

  @Override
  Appendable stringPropertyVisitStarted(String propertyName) {
    new StringBuilder()
  }

  @Override
  void stringPropertyVisitEnded(String propertyName, Appendable propertyValue) {
    //we want Strings in our result, not string builders
    map[propertyName] = propertyValue.toString()
  }
}
