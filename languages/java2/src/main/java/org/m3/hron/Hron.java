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
import java.io.*;

public class Hron {

    public static void parse(HronLineReader reader, HronVisitor visitor) throws Exception {

        visitor.Document_Begin();

        try {

            ScannerExtension    extension   = new ScannerExtension(visitor);
            Scanner             scanner     = new Scanner(ScannerState.PreProcessing, extension);

            String line;

            while ((line = reader.readLine()) != null) {
                scanner.result = ScannerResult.Continue;
                scanner.acceptLine(line);
            }

            extension.endOfDocument(scanner);

        } finally {
            visitor.Document_End();
        }
    }

    public static Map<String, Object> parseAsMap(HronLineReader reader) throws Exception {

        LinkedHashMap<String, Object> result = new LinkedHashMap<String, Object>();

        parse(reader, new HronMapBuildingVisitor(result));

        return result;
    }

    private static void WriteFromMapImpl(Map<String, Object> map, HronVisitor visitor) throws Exception {

        if (map == null) {
            return;
        }

        Set<String> keySet = map.keySet();

        // TODO: Support arrayified values

        for (String key : keySet)
        {
            Object value = map.get(key);
            if (value == null) {
                continue;
            } else if (value instanceof String) {
                try (StringReader reader = new StringReader((String)value)) {
                    try (BufferedReader bufferedReader = new BufferedReader(reader)) {
                        visitor.Value_Begin(key, 0, key.length());
                        String line;
                        while ((line = bufferedReader.readLine()) != null) {
                            visitor.Value_Line(line, 0, line.length());
                        }
                        visitor.Value_End();
                    }
                }
            } else if (value instanceof Map) {
                visitor.Object_Begin(key, 0, key.length());
                WriteFromMapImpl((Map)value, visitor);
                visitor.Object_End();
            }
        }
    }

    public static void writeFromMap(Map<String, Object> map, HronLineWriter writer) throws Exception {

        HronDocumentBuildingVisitor visitor = new HronDocumentBuildingVisitor(writer);

        visitor.Document_Begin();

        try {

            WriteFromMapImpl (map, visitor);

        } finally {
            visitor.Document_End();
        }

    }

}
