package org.m3.hron

import groovy.transform.InheritConstructors

/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 8:43 PM
 * To change this template use File | Settings | File Templates.
 */
@InheritConstructors
class HronParseException extends Exception {
  int row
  int column

  HronParseException(int row, int column, String message) {
    super(message)
    this.row = row
    this.column = column
  }

  String getMessage() {
    super.getMessage() + " at [$row, $column]"
  }
}
