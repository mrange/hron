package org.m3.hron;

/**
 *
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/14/12
 * Time: 9:50 AM
 * To change this template use File | Settings | File Templates.
 */
class HronBase {
  Object parent;
  String propertyName;
  int indent;

  public HronBase(int indent) {
    this.indent = indent;
  }

  public HronBase(Object parent, String propertyName, int indent) {
    this.parent = parent;
    this.propertyName = propertyName;
    this.indent = indent;
  }
}
