package org.m3.hron

/**
 * Created with IntelliJ IDEA.
 * User: mbjarland
 * Date: 11/13/12
 * Time: 11:33 PM
 * To change this template use File | Settings | File Templates.
 */
class HronString extends HronBase {
  Appendable data
  boolean hasData = false

  def leftShift(String str) {
    if (data == null) return

    if (hasData) data << "\n"

    data << str

    hasData = true
  }
}
