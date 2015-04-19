// ----------------------------------------------------------------------------------------------
// Copyright (c) Mårten Rånge.
// ----------------------------------------------------------------------------------------------
// This source code is subject to terms and conditions of the Microsoft Public License. A
// copy of the license can be found in the License.html file at the root of this distribution.
// If you cannot locate the  Microsoft Public License, please send an email to
// dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound
//  by the terms of the Microsoft Public License.
// ----------------------------------------------------------------------------------------------
// You must not remove this notice, or any other, from this software.
// ----------------------------------------------------------------------------------------------
package org.m3.hron;

import java.util.*;

class HronDocumentBuildingVisitor implements HronVisitor {
    HronLineWriter  writer                                  ;
    int             currentIndent   = 0                     ;
    StringBuilder   currentLine     = new StringBuilder ()  ;

    public HronDocumentBuildingVisitor(HronLineWriter w) {
        writer = w;
    }

    private void writeTag(int indent, char ch, String line, int beginIndex, int endIndex) {
        for (int iter = 0; iter < indent; ++iter) {
            currentLine.append('\t');
        }

        currentLine.append(ch);

        for (int iter = beginIndex; iter < endIndex; ++iter) {
            currentLine.append(line.charAt(iter));
        }

        writer.writeLine(currentLine.toString());
        currentLine.setLength(0);
    }

    private void writeLine(int indent, String line, int beginIndex, int endIndex) {
        for (int iter = 0; iter < indent; ++iter) {
            currentLine.append('\t');
        }

        for (int iter = beginIndex; iter < endIndex; ++iter) {
            currentLine.append(line.charAt(iter));
        }

        writer.writeLine(currentLine.toString());
        currentLine.setLength(0);
    }

    @Override
    public void Document_Begin() {
        //Skipped
    }
    @Override
    public void Document_End() {
        //Skipped
    }

    @Override
    public void PreProcessor(String line, int beginIndex, int endIndex) {
        writeTag(currentIndent, '!', line, beginIndex, endIndex);
    }

    @Override
    public void Empty(String line) {
        writeLine(0, line, 0, line.length());
    }

    @Override
    public void Comment(int indent, String line, int beginIndex, int endIndex) {
        writeTag(indent, '#', line, beginIndex, endIndex);
    }

    @Override
    public void Value_Begin(String line, int beginIndex, int endIndex) {
        writeTag(currentIndent, '=', line, beginIndex, endIndex);
    }
    @Override
    public void Value_Line(String line, int beginIndex, int endIndex) {
        writeLine(currentIndent + 1, line, beginIndex, endIndex);
    }
    @Override
    public void Value_End(){
        //Skipped
    }

    @Override
    public void Object_Begin(String line, int beginIndex, int endIndex) {
        writeTag(currentIndent, '@', line, beginIndex, endIndex);
        ++currentIndent;
    }
    @Override
    public void Object_End() {
        --currentIndent;
    }

    @Override
    public void Error(int lineNo, String line, String parseError) throws Exception
    {
        throw new HronException ("HRON parse exception @" + lineNo + " : " + line);
    }
}
