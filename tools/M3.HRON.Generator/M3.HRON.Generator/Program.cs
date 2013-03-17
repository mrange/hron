﻿// ----------------------------------------------------------------------------------------------
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
    static class Program
    {
        static void Main(string[] args)
        {
            FunctionalityTest();
            PerformanceTest();
        }

        static void FunctionalityTest()
        {
            var hrons = Directory
                .GetFiles(@"..\..\..\..\..\reference-data", "*.hron")
                .Select(Path.GetFullPath)
                .ToArray()
                ;

            var actionLogs = hrons
                .Select(p => Path.Combine(
                    Path.GetDirectoryName(p), 
                    "test-results", 
                    "CSharp", 
                    Path.GetFileName(p) + ".actionlog"))
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
                    var dir = Path.GetDirectoryName(testCase.actionLog);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    var hronLines = ReadLines(testCase.hron);


                    using (var sw = new StreamWriter(testCase.actionLog))
                    {
                        var v = new ActionLogVisitor(sw);
                        ReadDocument(v, hronLines);
                        Log.Success("Wrote action log: {0}", Path.GetFileName(testCase.actionLog));
                    }

                }
                catch (Exception exc)
                {
                    Log.Exception("Caught exception: {0}", exc);
                }

            }

        }

        static void PerformanceTest()
        {
            var fullPath = Path.GetFullPath(@"..\..\..\..\..\reference-data\large.hron");
            //var fullPath = Path.GetFullPath(@"..\..\..\..\..\..\reference-data\helloworld.hron");
            var lines = ReadLines(fullPath);

            var v = new EmptyVisitor();

            ReadDocument(v, lines);

            const int Count = 100;
            var sw = new Stopwatch();
            Log.Info("Starting performance run...");

            sw.Start();

            for (var iter = 0; iter < Count; ++iter)
            {
                ReadDocument(v, lines);
            }

            sw.Stop();

            Log.Success("{0:#,0} lines in {1:#,0} ms", Count*lines.Length, sw.ElapsedMilliseconds);
        }

        static string[] ReadLines(string fullPath)
        {
            var lines = File
                .ReadAllLines(fullPath)
                .Take(int.MaxValue)
                .ToArray()
                ;
            return lines;
        }

        static void ReadDocument(IScannerVisitor v, string[] lines)
        {
            var scanner = new Scanner(v);
            v.Document_Begin();
            for (int index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                scanner.AcceptLine(line, 0, line.Length);
            }
            scanner.AcceptEndOfStream();
            v.Document_End();
        }
    }

    sealed class LogVisitor : IScannerVisitor
    {
        public void Document_Begin()
        {
            Log.Info("Document_Begin");
        }

        public void Document_End()
        {
            Log.Info("Document_End");
        }

        public void Empty(string baseString, int begin, int end)
        {
            Log.Info("Empty:{0}", baseString.Slice(begin, end));
        }

        public void Comment(int indent, string baseString, int begin, int end)
        {
            Log.Info("Comment:{0},{1}", indent, baseString.Slice(begin, end));
        }

        public void PreProcessor(string baseString, int begin, int end)
        {
            Log.Info("PreProcessor:{0}", baseString.Slice(begin, end));
        }

        public void Object_Begin(string baseString, int begin, int end)
        {
            Log.Info("Object_Begin:{0}", baseString.Slice(begin, end));
        }

        public void Object_End()
        {
            Log.Info("Object_End");
        }

        public void Value_Begin(string baseString, int begin, int end)
        {
            Log.Info("Value_Begin:{0}", baseString.Slice(begin, end));
        }

        public void Value_Line(string baseString, int begin, int end)
        {
            Log.Info("Value_Line:{0}", baseString.Slice(begin, end));
        }

        public void Value_End()
        {
            Log.Info("Value_End");
        }

        public void Error(int lineNo, string baseString, int begin, int end, Scanner.Error parseError)
        {
            Log.Info("Error:{0},{1},{2}", parseError, lineNo, baseString.Slice(begin, end));
        }
    }

    sealed class EmptyVisitor : IScannerVisitor
    {
        public void Document_Begin()
        {
        }

        public void Document_End()
        {
        }

        public void PreProcessor(string baseString, int begin, int end)
        {
        }

        public void Empty(string baseString, int begin, int end)
        {
        }

        public void Comment(int indent, string baseString, int begin, int end)
        {
        }

        public void Object_Begin(string baseString, int begin, int end)
        {
        }

        public void Object_End()
        {
        }

        public void Value_Begin(string baseString, int begin, int end)
        {
        }

        public void Value_Line(string baseString, int begin, int end)
        {
        }

        public void Value_End()
        {
        }

        public void Error(int lineNo, string baseString, int begin, int end, Scanner.Error parseError)
        {
        }
    }

    sealed class ActionLogVisitor : IScannerVisitor
    {
        readonly StreamWriter m_writer;

        public ActionLogVisitor(StreamWriter writer)
        {
            m_writer = writer;
        }

        public void Document_Begin()
        {
        }

        public void Document_End()
        {
        }

        public void Empty(string baseString, int begin, int end)
        {
            m_writer.WriteLine("Empty:{0}", baseString.Slice(begin, end));
        }

        public void Comment(int indent, string baseString, int begin, int end)
        {
            m_writer.WriteLine("Comment:{0},{1}", indent, baseString.Slice(begin, end));
        }

        public void PreProcessor(string baseString, int begin, int end)
        {
            m_writer.WriteLine("PreProcessor:{0}", baseString.Slice(begin, end));
        }

        public void Object_Begin(string baseString, int begin, int end)
        {
            m_writer.WriteLine("Object_Begin:{0}", baseString.Slice(begin, end));
        }

        public void Object_End()
        {
            m_writer.WriteLine("Object_End:");
        }

        public void Value_Begin(string baseString, int begin, int end)
        {
            m_writer.WriteLine("Value_Begin:{0}", baseString.Slice(begin, end));
        }

        public void Value_Line(string baseString, int begin, int end)
        {
            m_writer.WriteLine("ContentLine:{0}", baseString.Slice(begin, end));
        }

        public void Value_End()
        {
            m_writer.WriteLine("Value_End:");
        }

        public void Error(int lineNo, string baseString, int begin, int end, Scanner.Error parseError)
        {
            m_writer.WriteLine("Error:{0},{1},{2}", parseError, lineNo, baseString.Slice(begin, end));
        }
    }
}
