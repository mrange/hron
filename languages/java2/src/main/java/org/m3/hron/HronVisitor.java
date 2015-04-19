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

public interface HronVisitor {
    void Document_Begin ();
    void Document_End ();

    void PreProcessor (String line, int beginIndex, int endIndex);

    void Empty (String line);

    void Comment (int indent, String line, int beginIndex, int endIndex);

    void Value_Begin (String line, int beginIndex, int endIndex);
    void Value_Line (String line, int beginIndex, int endIndex);
    void Value_End ();

    void Object_Begin (String line, int beginIndex, int endIndex);
    void Object_End ();

    void Error (int lineNo, String line, String parseError);
}