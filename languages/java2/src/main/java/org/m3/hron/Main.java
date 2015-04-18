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

import java.io.*;
import java.nio.file.*;

public class Main {
    public static void main (String[] args) throws IOException {
        Main instance = new Main ();
        instance.run();
    }

    void run() throws IOException {

        HRONVisitor visitor = new HRONVisitor() {
            public void Document_Begin () {
                System.out.format ("Document_Begin:\n");
            }
            public void Document_End () {
                System.out.format ("Document_End:\n");
            }

            public void PreProcessor (String line, int beginIndex, int endIndex) {
                System.out.format ("PreProcessor:%s\n", line.substring (beginIndex, endIndex));
            }

            public void Empty (String line) {
                System.out.format ("Empty:%s\n", line);
            }

            public void Comment (int indent, String line, int beginIndex, int endIndex) {
                System.out.format ("Comment:%i,%s\n", indent, line.substring (beginIndex, endIndex));
            }

            public void Value_Begin (String line, int beginIndex, int endIndex) {
                System.out.format ("Value_Begin:%s\n", line.substring (beginIndex, endIndex));
            }
            public void Value_Line (String line, int beginIndex, int endIndex) {
                System.out.format ("ContentLine:%s\n", line.substring (beginIndex, endIndex));
            }
            public void Value_End () {
                System.out.format ("Value_End:\n");
            }

            public void Object_Begin (String line, int beginIndex, int endIndex) {
                System.out.format ("Object_Begin:%s\n", line.substring (beginIndex, endIndex));
            }
            public void Object_End () {
                System.out.format ("Object_End:\n");
            }

            public void Error (int lineNo, String line, String parseError) {
                System.out.format ("Error:%s\n", line);
            }
        };


        Path path = FileSystems.getDefault ().getPath ("..", "..", "reference-data");

        try (DirectoryStream<Path> stream = Files.newDirectoryStream(path, p -> p.toString ().endsWith ("helloworld.hron"))) {
            for (Path file: stream) {
                System.out.format("Parsing %s\n", file.getFileName ());
                try (BufferedReader reader = Files.newBufferedReader (file)) {
                    HRON.parse (reader, visitor);
                }
            }
    }
  }
}
