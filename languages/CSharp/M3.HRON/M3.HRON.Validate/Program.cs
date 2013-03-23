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

namespace M3.HRON.Validate
{
    using System;
    using System.IO;
    using System.Linq;
    using M3.HRON.Validate.Source.Common;
    using M3.HRON.Generator.Parser;


    using Source.ConsoleApp;

    static class Program
    {
        static void Main(string[] args)
        {
            Runner.Run(args);
        }
    }

    namespace Source.ConsoleApp
    {
        static partial class Runner
        {
            public static string Slice (this string baseString, int begin, int end)
            {
                return baseString.Substring(begin, end - begin);
            }

            static partial void Partial_Run(string[] args, dynamic config)
            {
                Log.Info("Looking for reference data...");
                var hrons = Directory
                    .GetFiles(@"..\..\..\..\..\..\reference-data", "*.hron")
                    .Select(Path.GetFullPath)
                    .ToArray()
                    ;
                Log.Success("Found {0} reference data files", hrons.Length);

                Log.Info("Looking for reference action log files...");
                var actionLogs = hrons
                    .Select(p => Path.Combine(
                        Path.GetDirectoryName(p),
                        "test-results",
                        "CSharp",
                        Path.GetFileName(p) + ".actionlog"))
                    .ToArray()
                    ;
                Log.Success("Found {0} reference action log files", actionLogs.Length);

                var testCases = hrons
                    .Zip(actionLogs, (hron, actionLog) => new { hron, actionLog })
                    .ToArray()
                    ;

                Log.Info("Processing {0} test cases...", testCases.Length);
                foreach (var testCase in testCases)
                {
                    try
                    {
                        var dir = Path.GetDirectoryName(testCase.actionLog) ?? ".";
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        var hronLines = ReadLines(testCase.hron);


                        using (var sw = new StreamWriter(testCase.actionLog))
                        {
                            var v = new ActionLogVisitor(sw);
                            HRONSerialization.TryParse(hronLines, v);
                            Log.Success("Wrote action log: {0}", Path.GetFileName(testCase.actionLog));
                        }

                    }
                    catch (Exception exc)
                    {
                        Log.Exception("Caught exception: {0}", exc);
                    }

                }
                Log.Success("Processing of {0} test cases done", testCases.Length);
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
        }

        sealed class ActionLogVisitor : IHRONVisitor
        {
            readonly StreamWriter m_writer;
            int m_errorCount;

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

            public void Error(int lineNo, string baseString, int begin, int end, ScannerInterface.Error parseError)
            {
                m_writer.WriteLine("Error:{0},{1},{2}", parseError, lineNo, baseString.Slice(begin, end));
                ++m_errorCount;
            }

            public int ErrorCount
            {
                get { return m_errorCount; }
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
        }

    }
}

