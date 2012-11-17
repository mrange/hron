package org.m3.hron;

/**
 *
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/14/12
 * Time: 9:49 AM
 * To change this template use File | Settings | File Templates.
 */
class HronObject extends HronBase {
  boolean hasChildren = false;
  Object object;

  public HronObject(int indent) {
    super(indent);
  }

  public HronObject(Object parent, String propertyName, int indent, Object child) {
    super(parent, propertyName, indent);
    this.object = child;
  }
}
