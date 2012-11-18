package org.m3.hron;

import java.io.*;
import java.nio.CharBuffer;
import java.nio.MappedByteBuffer;
import java.nio.channels.FileChannel;
import java.nio.charset.Charset;
import java.nio.charset.CharsetDecoder;
import java.util.*;

/**
 *
 */
class HronParser {
  public Map<String, Object> parseText(String text) throws IOException {
    Map<String, Object> result = new LinkedHashMap<String, Object>();
    HronVisitor visitor = new MapBuildingHronVisitor(result);

    parseText(text, visitor);

    return result;
  }

  public void parseText(String text, HronVisitor visitor) throws IOException {
    CharBuffer charBuffer = CharBuffer.wrap(text);
    parseInternal(charBuffer, visitor);
  }

  public Map<String, Object> parseFile(File file) throws IOException {
    Map<String, Object> result = new LinkedHashMap<String, Object>();
    HronVisitor visitor = new MapBuildingHronVisitor(result);

    parseFile(file, visitor);

    return result;
  }

  public void parseFile(File file, HronVisitor visitor) throws IOException {
    FileInputStream stream = new FileInputStream(file);
    FileChannel channel = stream.getChannel();

    MappedByteBuffer mappedBuffer = channel.map( FileChannel.MapMode.READ_ONLY, 0L, channel.size( ) );

    String encoding = getFileEncoding(file);
    Charset charset = Charset.forName(encoding);
    CharsetDecoder decoder = charset.newDecoder();
    CharBuffer charBuffer = decoder.decode(mappedBuffer);

    parseInternal(charBuffer, visitor);
  }

  private void parseInternal(CharBuffer charBuffer, HronVisitor visitor) throws IOException {
    int SIZE = 4*1024;
    
    HronParseState state = new HronParseState(visitor);
    state.open();

    char[] buffer = new char[SIZE];
    long charsRemaining = charBuffer.remaining();

    int indent = 0;
    boolean pivotFound = false;
    char pivot = 0;
    char[] line = new char[64*1024];
    long row = 1;
    int column = 0;

    while( charsRemaining > 0 ) {
      int numChars = (charsRemaining < SIZE) ? (int) charsRemaining : SIZE;
      charBuffer.get(buffer, 0, numChars);

      for ( int i=0; i<numChars; i++, column++ ) {
        char c = buffer[i];
        line[column] = c;
        charsRemaining--;

        if (pivotFound) {
          if (c != '\n' && charsRemaining > 0) continue;
          //if (pivot == '#') continue;

          int add = (charsRemaining == 0 && c != '\n') ? 1 : 0;
          String rest = new String(line, indent+1, column-indent-1 + add);

          switch(pivot) {
            case '#':
              break;

            case '=':
              if (indent < state.currentIndent) state.closeUntilIndent(indent);
              if (column == indent + 1 && !state.currentObject.hasChildren) fail(row, column-1, "Array syntax without context encountered");

              state.openString(rest);
              break;

            case '@':
              if (indent < state.currentIndent) state.closeUntilIndent(indent);
              if (column == indent + 1 && !state.currentObject.hasChildren) fail(row, column-1, "Array syntax without context encountered");

              state.openObject(rest);
              break;

            case '\t':
              if (state.currentString == null) fail(row, column, "String data encountered even though no string has been opened");

              state.currentString.append(rest);
              break;
          }

          indent = 0;
          pivotFound = false;
          row++;
          column = -1;

          continue;
        }

        if (c == '\n') {
          indent = 0;
          pivotFound = false;
          row++;
          column = -1;
          continue;
        }

        if (c == '#') {
          pivotFound = true;
          pivot = c;

        } else if (c != '\t') {  //we find a non-tab before we get to the current indent -> assume pivot
          if (c != '@' && c != '=') fail(row, column, "Invalid character '" + c + "' encountered");
          pivotFound = true;
          pivot = c;

        } else if (column == state.currentIndent) {
          if (c != '@' && c != '=' && c != '\t') fail(row, column, "Invalid character '" + c + "' encountered");
          pivotFound = true;
          pivot = c;

        }

        indent = column;
      }
    }

    state.close();
  }


  List<String> getErrorContext() {
    return new ArrayList<String>();
  }

  private void fail(long row, int column, String message) throws HronParseException {
    List<String> errorContext = getErrorContext();
    throw new HronParseException(row, column+1, errorContext, message);
  }


  /***
   * <p>Determines the encoding of the specified file. Returns "UTF-16BE" or "UTF-16LE" if a
   * Byte Order Mark (BOM) is found,</p>
   *
   * <p>>If a UTF8 BOM is found an encoding of "UTF-8" is returned. Otherwise the default encoding is returned.</p>
   *
   * @param file file to check
   * @return "UTF-8", "UTF-16BE", "UTF-16LE", or default encoding.
   */
  private String getFileEncoding(File file) {
    String encoding = System.getProperty("file.encoding");

    BufferedReader bufferedReader = null;

    try {
      // In order to read files with non-default encoding, specify an encoding in the FileInputStream constructor.
      bufferedReader = new BufferedReader(new InputStreamReader(new FileInputStream(file)));

      char buffer[] = new char[3];
      int length = bufferedReader.read(buffer);

      if (length >= 2) {
        if ((buffer[0] == (char) 0xff && buffer[1] == (char) 0xfe)) { /* UTF-16, little endian */
          encoding = "UTF-16LE";
        } else if (buffer[0] == (char) 0xfe && buffer[1] == (char) 0xff) { /* UTF-16, big endian */
          encoding = "UTF-16BE";
        }
      }
      if (length >= 3) {
        if (buffer[0] == (char) 0xef && buffer[1] == (char) 0xbb && buffer[2] == (char) 0xbf) /* UTF-8 */  {
          encoding = "UTF-8";
        }
      }
    } catch (IOException ex) {

    } finally {
      if (bufferedReader != null) {
        try {
          bufferedReader.close();
        } catch (IOException ex) {
        }
      }
    }

    return encoding;
  }
}