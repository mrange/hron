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
import java.util.*;

public class Main {
    public static void main(String[] args) throws Exception {
        Main instance = new Main();
        instance.run();
    }

    void run() throws Exception {
        performanceTest();
        semanticTest();
    }

    private void semanticTest() throws Exception {
        Path referenceDataPath  = FileSystems.getDefault().getPath("..", "..", "reference-data");
        Path resultsPath        = FileSystems.getDefault().getPath("..", "..", "reference-data", "test-results", "Java");

        System.out.format("Producing action logs...\n");
        try (DirectoryStream<Path> stream = Files.newDirectoryStream(referenceDataPath, p -> p.toString().endsWith(".hron"))) {
            for (Path hronPath : stream) {

                Path hronFileNamePath       = hronPath.getFileName();
                String hronFileName         = hronFileNamePath.toString();
                String actionLogFileName    = hronFileName + ".actionlog";
                Path actionLogPath          = resultsPath.resolve(actionLogFileName);
                String actionLogFilePath    = actionLogPath.toString();

                System.out.format("Processing: %s\n", hronFileName);
                try (BufferedReader reader = Files.newBufferedReader(hronPath)) {
                    System.out.format("  Writing action log: %s\n", actionLogFilePath);
                    try (PrintStream writer = new PrintStream(actionLogFilePath)) {

                        HronVisitor visitor = new HronVisitor() {
                            public void Document_Begin() {
                            }
                            public void Document_End() {
                            }

                            public void PreProcessor(String line, int beginIndex, int endIndex) {
                                writer.format("PreProcessor:%s\n", line.substring(beginIndex, endIndex));
                            }

                            public void Empty(String line) {
                                writer.format("Empty:%s\n", line);
                            }

                            public void Comment(int indent, String line, int beginIndex, int endIndex) {
                                writer.format ("Comment:%d,%s\n", indent, line.substring(beginIndex, endIndex));
                            }

                            public void Value_Begin(String line, int beginIndex, int endIndex) {
                                writer.format("Value_Begin:%s\n", line.substring(beginIndex, endIndex));
                            }
                            public void Value_Line(String line, int beginIndex, int endIndex) {
                                writer.format("ContentLine:%s\n", line.substring(beginIndex, endIndex));
                            }
                            public void Value_End() {
                                writer.format("Value_End:\n");
                            }

                            public void Object_Begin(String line, int beginIndex, int endIndex) {
                                writer.format("Object_Begin:%s\n", line.substring(beginIndex, endIndex));
                            }
                            public void Object_End() {
                                writer.format("Object_End:\n");
                            }

                            public void Error(int lineNo, String line, String parseError) {
                                writer.format("Error:%s\n", line);
                            }
                        };

                        Hron.parse(() -> reader.readLine(), visitor);
                    }
                }
            }
        }
    }

    private void performanceTest() throws Exception {
        System.out.format("Performance tests...\n");
        Path largePath  = FileSystems.getDefault().getPath("..", "..", "reference-data", "large.hron");
        try (BufferedReader reader = Files.newBufferedReader(largePath)) {
            ArrayList<String> lines = new ArrayList<String>();
            String line = null;
            while ((line = reader.readLine()) != null) {
                lines.add(line);
            }

            HronVisitor visitor = new HronVisitor() {
                public void Document_Begin() {
                }
                public void Document_End() {
                }

                public void PreProcessor(String line, int beginIndex, int endIndex) {
                }

                public void Empty(String line) {
                }

                public void Comment(int indent, String line, int beginIndex, int endIndex) {
                }

                public void Value_Begin(String line, int beginIndex, int endIndex) {
                }
                public void Value_Line(String line, int beginIndex, int endIndex) {
                }
                public void Value_End() {
                }

                public void Object_Begin(String line, int beginIndex, int endIndex) {
                }
                public void Object_End() {
                }

                public void Error(int lineNo, String line, String parseError) throws Exception {
                    throw new Exception ("HRON parse error");
                }
            };

            final int outers = 100;

            long then = System.nanoTime ();

            for (int outer = 0; outer < outers; ++outer) {
                HronLineReader innerReader = new HronLineReader() {
                    private int lineNo = 0;
                    private int size = lines.size();
                    public String readLine() throws Exception {
                        if (lineNo < size)
                        {
                            return lines.get(lineNo++);
                        }
                        else
                        {
                            return null;
                        }
                    }
                };

                Hron.parse(innerReader, visitor);
            }

            long now = System.nanoTime ();

            long elapsed = (now - then) / 1000000;

            System.out.format("  %d lines read in %d ms\n", lines.size()*outers, elapsed);
        }
    }
}
