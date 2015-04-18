package org.m3.hron;

import java.io.*;

public class HRON {

    public static void parse (BufferedReader reader, HRONVisitor visitor) throws  IOException {

        visitor.Document_Begin ();

        try {

            ScannerExtension    extension   = new ScannerExtension (visitor);
            Scanner             scanner     = new Scanner (ScannerState.PreProcessorTag, extension);

            String line;

            while ((line = reader.readLine ()) != null) {
                scanner.result = ScannerResult.Continue;
                scanner.acceptLine (line);
            }

            extension.endOfDocument (scanner);

        } finally {
            visitor.Document_End ();
        }
    }
}
