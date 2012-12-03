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

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using M3.HRON.Generator.Parser;
using M3.HRON.Generator.Source.Common;

namespace M3.HRON.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var fullPath = Path.GetFullPath(@"..\..\..\..\..\..\reference-data\large.hron");
            var lines = File.ReadAllLines(fullPath).Select (s => s.ToSubString()).ToArray();

            var v = new EmptyVisitor();
            ReadDocument(v, lines);


            const int Count = 40;
            var sw = new Stopwatch();
            sw.Start();

            for (var iter = 0; iter < Count; ++iter)
            {
                ReadDocument(v, lines);
            }

            sw.Stop();

            Console.WriteLine ("{0:#,0} lines in {1:#,0} ms", Count * lines.Length, sw.ElapsedMilliseconds);
        }

        static void ReadDocument(IVisitor v, SubString[] lines)
        {
            var scanner = new Scanner(v);
            v.Document_Begin();
            for (int index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                scanner.AcceptLine(line);
            }
            scanner.AcceptEndOfStream();
            v.Document_End();
        }
    }

    sealed class ConsoleVisitor : IVisitor
    {
        public void Document_Begin()
        {
            Log.Info("Document_Begin");
        }

        public void Document_End()
        {
            Log.Info("Document_End");
        }

        public void Object_Begin(SubString name)
        {
            Log.Info("Object_Begin: {0}", name);
        }

        public void Object_End()
        {
            Log.Info("Object_End");
        }

        public void Value_Begin(SubString name)
        {
            Log.Info("Value_Begin: {0}", name);
        }

        public void Value_Line(SubString name)
        {
            Log.Info("Value_Line: {0}", name);
        }

        public void Value_End()
        {
            Log.Info("Value_End");
        }
    }

    sealed class EmptyVisitor : IVisitor
    {
        public void Document_Begin()
        {
        }

        public void Document_End()
        {
        }

        public void Object_Begin(SubString name)
        {
        }

        public void Object_End()
        {
        }

        public void Value_Begin(SubString name)
        {
        }

        public void Value_Line(SubString name)
        {
        }

        public void Value_End()
        {
        }
    }
}
