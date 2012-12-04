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
using System.Collections.Generic;
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
            FunctionalityTest();
            PerformanceTest();
        }

        static void FunctionalityTest()
        {
            var hrons = Directory
                .GetFiles(@"..\..\..\..\..\..\reference-data", "*.hron")
                .Select(Path.GetFullPath)
                .ToArray()
                ;

            var actionLogs = hrons
                .Select(p => p + ".actionlog")
                .ToArray()
                ;

            var testCases = hrons
                .Zip(actionLogs, (hron, actionLog) => new {hron, actionLog})
                .ToArray()
                ;

            foreach (var testCase in testCases)
            {
                try
                {
                    var hronLines = ReadLines(testCase.hron);
                    var actionLogLines = ReadLines(testCase.actionLog);
                }
                catch (Exception exc)
                {
                    Log.Exception("Caught exception: {0}", exc);
                }

            }

        }

        static void PerformanceTest()
        {
            var fullPath = Path.GetFullPath(@"..\..\..\..\..\..\reference-data\large.hron");
            //var fullPath = Path.GetFullPath(@"..\..\..\..\..\..\reference-data\helloworld.hron");
            var lines = ReadLines(fullPath);

            var v = new EmptyVisitor();

            ReadDocument(v, lines);

            const int Count = 100;
            var sw = new Stopwatch();
            sw.Start();

            for (var iter = 0; iter < Count; ++iter)
            {
                ReadDocument(v, lines);
            }

            sw.Stop();

            Log.Info("{0:#,0} lines in {1:#,0} ms", Count*lines.Length, sw.ElapsedMilliseconds);
        }

        static SubString[] ReadLines(string fullPath)
        {
            var lines = File
                .ReadAllLines(fullPath)
                .Take(int.MaxValue)
                .Select(s => s.ToSubString())
                .ToArray()
                ;
            return lines;
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

    sealed class LogVisitor : IVisitor
    {
        public void Document_Begin()
        {
            Log.Info("Document_Begin");
        }

        public void Document_End()
        {
            Log.Info("Document_End");
        }

        public void Comment(SubString comment)
        {
            Log.Info("Comment: {0}", comment);
        }

        public void PreProcessor(SubString preprocessor)
        {
            Log.Info("PreProcessor: {0}", preprocessor);
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

        public void Comment(SubString comment)
        {
            
        }

        public void PreProcessor(SubString preprocessor)
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

    sealed class ActionLogVisitor : IVisitor
    {
        public List<SubString> ActionLog = new List<SubString>();

        public void Document_Begin()
        {
        }

        public void Document_End()
        {
        }

        public void Comment(SubString comment)
        {

        }

        public void PreProcessor(SubString preprocessor)
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
