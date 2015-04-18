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

public class Hron {

    public static void parse (BufferedReader reader, HronVisitor visitor) throws  IOException {

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
