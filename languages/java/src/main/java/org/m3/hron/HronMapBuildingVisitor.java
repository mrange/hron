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

class HronMapBuildingVisitor implements HronVisitor {
    ArrayList<LinkedHashMap<String, Object>>    objectStack     = new ArrayList<LinkedHashMap<String, Object>>();
    ArrayList<String>                           propertyStack   = new ArrayList<String>();
    StringBuilder                               currentValue    = new StringBuilder();

    public HronMapBuildingVisitor(LinkedHashMap<String, Object> root) {
        objectStack.add(root);
    }

    private void setProperty(Object value) {
        String name = propertyStack.get(propertyStack.size() - 1);
        propertyStack.remove(propertyStack.size() - 1);
        LinkedHashMap<String, Object> object = objectStack.get(objectStack.size() - 1);
        object.put(name, value);    // TODO: Arrayify
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
        //Skipped
    }

    @Override
    public void Empty(String line) {
        //Skipped
    }

    @Override
    public void Comment(int indent, String line, int beginIndex, int endIndex) {
        //Skipped
    }

    @Override
    public void Value_Begin(String line, int beginIndex, int endIndex) {
        propertyStack.add(line.substring(beginIndex, endIndex));
        currentValue.setLength(0);
    }
    @Override
    public void Value_Line(String line, int beginIndex, int endIndex) {
        for (int iter = beginIndex; iter < endIndex; ++iter) {
            currentValue.append(line.charAt(iter));
        }
        currentValue.append('\n');
    }
    @Override
    public void Value_End(){
        setProperty(currentValue.toString());
    }

    @Override
    public void Object_Begin(String line, int beginIndex, int endIndex) {
        propertyStack.add(line.substring(beginIndex, endIndex));
        objectStack.add(new LinkedHashMap<String, Object>());
    }
    @Override
    public void Object_End() {
        LinkedHashMap<String, Object> object = objectStack.get(objectStack.size() - 1);
        objectStack.remove(objectStack.size() - 1);
        setProperty(object);
    }

    @Override
    public void Error(int lineNo, String line, String parseError) throws Exception
    {
        throw new HronException ("HRON parse exception @" + lineNo + " : " + line);
    }
}
