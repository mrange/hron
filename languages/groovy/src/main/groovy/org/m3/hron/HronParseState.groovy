package org.m3.hron

/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 9:40 PM
 * To change this template use File | Settings | File Templates.
 */
class HronParseState {
  Stack<HronVisitorMarker> objects = []

  HronStringProperty currentString

  HronVisitorMarker getCurrentObject() {
    objects.peek()
  }

  int getCurrentIndent() {
    currentObject.indent + 1
  }

  HronVisitor getCurrentVisitor() {
    currentObject.visitor
  }

  void openString(String propertyName) {
    closeString()

    Appendable data = currentVisitor.stringPropertyVisitStarted(propertyName)
    currentString = new HronStringProperty(propertyName: propertyName, data: data)
  }

  void closeString() {
    if (currentString == null) return

    currentVisitor.stringPropertyVisitEnded(currentString.propertyName, currentString.data)
    currentString = null
  }

  void openObject(String objectName) {
    closeString()
    HronVisitor newVisitor = currentVisitor.objectPropertyVisitStarted(objectName)

    objects << new HronVisitorMarker(indent: currentIndent, visitor: newVisitor)
  }

  void popUntilIndent(int indent) {
    closeString()
    while (indent < currentIndent) {
      currentVisitor.objectPropertyVisitEnded()
      objects.pop()
    }
  }
  }
