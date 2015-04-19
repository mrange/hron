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
  List<String> errorContext

  HronParseException(row, column, errorContext, String message) {
    super(message)

    this.row = row
    this.column = column
    this.errorContext = errorContext
  }

  String getMessage() {
    StringBuilder result = new StringBuilder()
    result << "${super.message} at [$row, $column] \n"
    errorContext.eachWithIndex { line, i ->
      result << "${row - 2 + i} |".padLeft(3)
      result << line.replaceAll(/\t/, / /)
      result << '\n'
    }
    result << "${'-' * (column+3)}^"

    result
  }
}
