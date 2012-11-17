package org.m3.hron;

import java.util.Stack;

/**
 *
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 9:40 PM
 * To change this template use File | Settings | File Templates.
 */
class HronParseState {
  Stack<HronObject> objects = new Stack<HronObject>();
  HronObject currentObject;
  HronString currentString;
  int currentIndent;
  HronVisitor visitor;

  public HronParseState(HronVisitor visitor) {
    this.visitor = visitor;
  }

  void open() {
    currentIndent = 0;
    currentObject = new HronObject(-1);
    objects.push(currentObject);
  }

  void close() {
    closeUntilIndent(0);
  }

  boolean arrayIsOk() {
    return currentObject.hasChildren;
  }

  void openString(String propertyName) {
    closeString();

    Appendable data = visitor.stringPropertyVisitStarted(currentObject.object, propertyName);

    currentObject.hasChildren = true;
    currentString = new HronString(currentObject.object, propertyName, currentIndent, data);
  }

  void closeString() {
    if (currentString == null) return;

    visitor.stringPropertyVisitEnded(currentObject.object, currentString.propertyName, currentString.data);
    currentString = null;
  }

  void openObject(String objectName) {
    closeString();

    Object parent = (currentObject != null ? currentObject.object : null);
    Object child = visitor.objectPropertyVisitStarted(parent, objectName);

    if (currentObject != null) currentObject.hasChildren = true;

    currentObject = new HronObject(parent, objectName, currentIndent, child);
    currentIndent = currentIndent + 1;
    objects.push(currentObject);
  }

  void closeUntilIndent(int indent) {
    closeString();
    while (indent < currentIndent) {
      visitor.objectPropertyVisitEnded(currentObject.parent, currentObject.propertyName, currentObject.object);

      objects.pop();
      if (objects.empty()) throw new RuntimeException("Invalid object stack state, internal error");

      currentObject = objects.peek();
      currentIndent = currentObject.indent + 1;
    }
  }

}
