package org.m3.hron

import groovy.io.LineColumnReader

class HronParser {
  LineColumnReader reader
  /**
   * Parse a text representation of a HRON data structure
   *
   * @param text HRON text to parse
   * @return a data structure of lists and maps
   */
  public Object parseText(String text) {
    if (!text) throw new IllegalArgumentException("The HRON input text should neither be null nor empty.");

    return parse(new LineColumnReader(new StringReader(text)));
  }

  /**
   * Parse a HRON data structure from content from a reader
   *
   * @param reader reader over a HRON content
   * @return a data structure of lists and maps
   */
  public Object parse(Reader reader) {
    this.reader = (reader instanceof LineColumnReader) ? reader : new LineColumnReader(reader)

    Map<String, Object> result = [:]
    HronVisitor visitor = new MapBuildingHronVisitor(map: result)

    HronParseState state = new HronParseState()
    state.objects << new HronVisitorMarker(indent: -1, visitor: visitor)

    reader.eachLine { String line ->
      parseLine(line, state)
    }

    state.popUntilIndent(0)

    result
  }

  private void parseLine(String line, HronParseState state) {
    int indent = -1
    Character first = line?.chars?.find { char c ->
      indent++
      c != '\t'
    }

    if (first == null) return

    switch(first) {
      case '=':
        if (indent > state.currentIndent) throw new HronParseException("Invalid indent $indent at line ${reader.line}")
        if (indent < state.currentIndent) state.popUntilIndent(indent)

        state.openString line[(indent+1)..-1]
        break

      case '@':
        if (indent > state.currentIndent) throw new HronParseException("Invalid indent $indent at line ${reader.line}")
        if (indent < state.currentIndent) state.popUntilIndent(indent)

        state.openObject line[(indent+1)..-1]
        break

      default:
        //for string data
        if (state.currentString == null) throw new HronParseException("String data encountered even though no string has been opened at line ${reader.line}")
        if (indent != state.currentIndent + 1) throw new HronParseException("Invalid indent $indent at line ${reader.line}, expected ${state.currentIndent+1}")

        state.currentString.data << line[indent..-1]
    }
  }
}