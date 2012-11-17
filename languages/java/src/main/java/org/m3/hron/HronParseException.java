package org.m3.hron;

import java.util.List;

/**
 *
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 8:43 PM
 * To change this template use File | Settings | File Templates.
 */
class HronParseException extends RuntimeException {
  long row;
  long column;
  List<String> errorContext;

  HronParseException(long row, long column, List<String> errorContext, String message) {
    super(message);

    this.row = row;
    this.column = column;
    this.errorContext = errorContext;
  }

  public String times(char c, long len) {
    StringBuilder sb = new StringBuilder();

    for (int i=0; i<len; i++) {
      sb.append(c);
    }

    return sb.toString();
  }

  public String padLeft(String s, int len) {
    StringBuilder sb = new StringBuilder(s);
    while (sb.length() < len) {
      sb.insert(0, ' ');
    }

    return sb.toString();
  }

  public String getMessage() {
    StringBuilder result = new StringBuilder();
    result.append(super.getMessage());
    result.append(" at [");
    result.append(row);
    result.append(", ");
    result.append(column);
    result.append("] \n");

    int r = 0;
    for (String line : errorContext) {
      result.append(padLeft((row - 2 + r) + " |", 3));
      result.append(line.replaceAll("\t", " "));
      result.append('\n');

    }

    result.append(times('-', column + 3));
    result.append('^');

    return result.toString();
  }
}
