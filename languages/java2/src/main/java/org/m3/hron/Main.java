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

        Path referenceDataPath  = FileSystems.getDefault ().getPath ("..", "..", "reference-data");
        Path resultsPath        = FileSystems.getDefault ().getPath ("..", "..", "reference-data", "test-results", "Java");

        try (DirectoryStream<Path> stream = Files.newDirectoryStream(referenceDataPath, p -> p.toString ().endsWith (".hron"))) {
            for (Path hronPath : stream) {

                Path hronFileNamePath       = hronPath.getFileName ();
                String hronFileName         = hronFileNamePath.toString ();
                String actionLogFileName    = hronFileName + ".actionlog";
                Path actionLogPath          = resultsPath.resolve (actionLogFileName);
                String actionLogFilePath    = actionLogPath.toString ();

                System.out.format("Processing: %s\n", hronFileName);
                try (BufferedReader reader = Files.newBufferedReader (hronPath)) {
                    System.out.format("  Writing action log: %s\n", actionLogFilePath);
                    try (PrintStream writer = new PrintStream (actionLogFilePath)) {

                        HronVisitor visitor = new HronVisitor() {
                            public void Document_Begin () {
                            }
                            public void Document_End () {
                            }

                            public void PreProcessor (String line, int beginIndex, int endIndex) {
                                writer.format ("PreProcessor:%s\n", line.substring (beginIndex, endIndex));
                            }

                            public void Empty (String line) {
                                writer.format ("Empty:%s\n", line);
                            }

                            public void Comment (int indent, String line, int beginIndex, int endIndex) {
                                writer.format ("Comment:%d,%s\n", indent, line.substring (beginIndex, endIndex));
                            }

                            public void Value_Begin (String line, int beginIndex, int endIndex) {
                                writer.format ("Value_Begin:%s\n", line.substring (beginIndex, endIndex));
                            }
                            public void Value_Line (String line, int beginIndex, int endIndex) {
                                writer.format ("ContentLine:%s\n", line.substring (beginIndex, endIndex));
                            }
                            public void Value_End () {
                                writer.format ("Value_End:\n");
                            }

                            public void Object_Begin (String line, int beginIndex, int endIndex) {
                                writer.format ("Object_Begin:%s\n", line.substring (beginIndex, endIndex));
                            }
                            public void Object_End () {
                                writer.format ("Object_End:\n");
                            }

                            public void Error (int lineNo, String line, String parseError) {
                                writer.format ("Error:%s\n", line);
                            }
                        };

                        Hron.parse (reader, visitor);
                    }
                }
            }
        }
    }
}
