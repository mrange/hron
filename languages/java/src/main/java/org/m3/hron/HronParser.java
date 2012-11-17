package org.m3.hron;

import groovy.io.LineColumnReader;

import java.io.*;
import java.nio.Buffer;
import java.nio.MappedByteBuffer;
import java.nio.channels.FileChannel;
import java.util.*;

/**
 *
 FileInputStream f = new FileInputStream( name );
 FileChannel ch = f.getChannel( );
 MappedByteBuffer mb = ch.map( ch.MapMode.READ_ONLY,
 0L, ch.size( ) );
 byte[] barray = new byte[SIZE];
 long checkSum = 0L;
 int nGet;
 while( mb.hasRemaining( ) )
 {
 nGet = Math.min( mb.remaining( ), SIZE );
 mb.get( barray, 0, nGet );
 for ( int i=0; i<nGet; i++ )
 checkSum += barray[i];
 }
 */
class HronParser {
  private LineColumnReader reader;
  private long row = 1;
  private int column = 0;

  public Map<String, Object> parseFile(File file) throws IOException {
    Map<String, Object> result = new LinkedHashMap<String, Object>();
    HronVisitor visitor = new MapBuildingHronVisitor(result);

    parseFile(file, visitor);

    return result;
  }

  public void parseFile(File file, HronVisitor visitor) throws IOException {
    int SIZE = 4*1024;
    
    FileInputStream stream = new FileInputStream(file);
    FileChannel channel = stream.getChannel();

    MappedByteBuffer mappedBuffer = channel.map( FileChannel.MapMode.READ_ONLY, 0L, channel.size( ) );
    HronParseState state = new HronParseState(visitor);
    state.open();

    byte[] buffer = new byte[SIZE];
    long charsRemaining = file.length();

    int indent = 0;
    boolean pivotFound = false;
    char pivot = 0;
    char[] line = new char[64*1024];

    while( charsRemaining > 0 ) {
      int numChars = (charsRemaining < SIZE) ? (int) charsRemaining : SIZE;
      mappedBuffer.get(buffer, 0, numChars);

      for ( int i=0; i<numChars; i++ ) {
        char c = (char) (buffer[i] & 0xFF);
        line[column++] = c;
        charsRemaining--;

        if (pivotFound) {
          if (c != '\n') continue;

          switch(pivot) {
            case '#':
              break;

            case '=':
              if (indent < state.currentIndent)
                state.closeUntilIndent(indent);
              if (column == indent + 1 && !state.currentObject.hasChildren) fail("Array syntax without context encountered");

              state.openString(new String(line, indent+1, column-indent-2));
              break;

            case '@':
              if (indent < state.currentIndent)
                state.closeUntilIndent(indent);
              if (column == indent + 1 && !state.arrayIsOk()) fail("Array syntax without context encountered");

              state.openObject(new String(line, indent+1, column-indent-2));
              break;

            case '\t':
              if (state.currentString == null) fail("String data encountered even though no string has been opened");

              state.currentString.append(new String(line, indent+1, column-indent-2));
              break;
          }

          indent = 0;
          pivotFound = false;
          row++;
          column = 0;

          continue;
        }

        if (c == '\n') {
          indent = 0;
          pivotFound = false;
          row++;
          column = 0;
          continue;
        }

        if (c == '#') {
          pivotFound = true;
          pivot = c;

        } else if (column == state.currentIndent+1) {
          if (c != '@' && c != '=' && c != '\t') fail("Invalid character '" + c + "' encountered");
          pivotFound = true;
          pivot = c;

        } else if (c != '\t') {  //we find a non-tab before we get to the current indent -> assume pivot
          if (c != '@' && c != '=') fail("Invalid character '" + c + "' encountered");
          pivotFound = true;
          pivot = c;
        }

        indent = column-1 ;
      }
    }

    state.close();
  }


  List<String> getErrorContext() {
    return new ArrayList<String>();
  }

  private void fail(String message) throws HronParseException {
    List<String> errorContext = getErrorContext();
    throw new HronParseException(row, column, errorContext, message);
  }
}