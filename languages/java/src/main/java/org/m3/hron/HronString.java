package org.m3.hron;

import java.io.IOException;

/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 11:33 PM
 * To change this template use File | Settings | File Templates.
 */
class HronString extends HronBase {
  Appendable data;
  boolean hasData = false;

  public HronString(Object parent, String propertyName, int indent, Appendable data) {
    super(parent, propertyName, indent);
    this.data = data;
  }

  public void append(String str) throws IOException {
    if (data == null) return;

    if (hasData)
      data.append("\n");

    data.append(str);

    hasData = true;
  }

  public void append(char c) throws IOException {
    if (data == null) return;

    if (hasData)
      data.append("\n");

    data.append(c);

    hasData = true;
  }
}
