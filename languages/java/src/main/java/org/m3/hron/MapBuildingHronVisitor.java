package org.m3.hron;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

/**
 *
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 9:01 PM
 * To change this template use File | Settings | File Templates.
 */
class MapBuildingHronVisitor implements HronVisitor {
  Map<String, Object> map;
  Map<Object, String> lastInserted = new LinkedHashMap<Object, String>();

  public MapBuildingHronVisitor(Map<String, Object> backing) {
    this.map = backing;
  }

  private Object insertOrArrayify(Object parent, Object child, String propertyName) {
    @SuppressWarnings("unchecked")
    Map<String, Object> parentMap = (parent == null) ? map : (Map<String, Object>) parent;

    if (propertyName.equals("")) {
      String lastKey = lastInserted.get(parent);
      Object last = parentMap.get(lastKey);
      if (last instanceof List) {
        //noinspection unchecked
        ((List) last).add(child);
      } else {
        List<Object> newList = new ArrayList<Object>();
        newList.add(last);
        newList.add(child);
        parentMap.put(lastKey, newList);
      }
    } else {
      parentMap.put(propertyName, child);
      lastInserted.put(parent, propertyName);
    }

    return child;
  }

  @Override
  public Object objectPropertyVisitStarted(Object parent, String propertyName) {
    return insertOrArrayify(parent, new LinkedHashMap<String, Object>(), propertyName);
  }


  @Override
  public void objectPropertyVisitEnded(Object parent, String propertyName, Object child) {
    lastInserted.remove(child);
  }

  @Override
  public Appendable stringPropertyVisitStarted(Object parent, String propertyName) {
    return new StringBuilder();
  }

  @Override
  public void stringPropertyVisitEnded(Object parent, String propertyName, Appendable propertyValue) {
    insertOrArrayify(parent, propertyValue.toString(), propertyName);
  }

  @Override
  public void error(Object parent, long line, int column, HronParseException error) {
    //do nothing for now
  }
}
