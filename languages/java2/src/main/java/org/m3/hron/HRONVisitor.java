package org.m3.hron;

public interface HRONVisitor {
    void Document_Begin();
    void Document_End();

    void PreProcessor(String line);

    void Empty(String line);

    void Comment(int indent, String comment);

    void Value_Begin(String name);
    void Value_Line(String value);
    void Value_End();

    void Object_Begin(String name);
    void Object_End();

    void Error(int lineNo, String line, String parseError);
}