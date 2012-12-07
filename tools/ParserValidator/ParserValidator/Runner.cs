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

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ParserValidator.Source.Common;
using ParserValidator.Source.Extensions;
using ParserValidator.Source.HRON;

namespace ParserValidator.Source.ConsoleApp
{
    partial class ReferenceData
    {
        public string ReferenceDataPath ;
        public string ActionLogPath     ;
    }

    partial class TestResult
    {
        public string   Name;
        public dynamic  Configuration;
        public string[] TestResultFiles;
    }

    partial class Runner
    {
        static partial void Partial_Run(string[] args, dynamic config)
        {
            var referenceDataPath = Path.GetFullPath(@"..\..\..\..\..\reference-data");
            var testResultsPath = Path.Combine(referenceDataPath, "test-results");

            Log.HighLight ("ParserValidator starting...");

            Log.Info("Looking for reference data and matching action logs here: {0}", referenceDataPath);

            var referenceDatas = GetReferenceData(referenceDataPath).ToArray ();
            if (referenceDatas.Length == 0)
            {
                Log.Warning("No reference data found, terminating");
                return;
            }

            Log.Success("Found #{0} reference data files", referenceDatas.Length);

            Log.Info("Looking for test results here: {0}", testResultsPath);

            var testResults = GetTestResults(testResultsPath).ToArray ();

            if (testResults.Length == 0)
            {
                Log.Warning("No test results found, terminating");
                return;
            }

            Log.Success("Found #{0} test results", testResults.Length);

            Log.HighLight ("Validation step is starting...");

            foreach (var testResult in testResults)
            {
                Log.Info("Processing test result: {0}", testResult.Name);

            }

        }

        static IEnumerable<TestResult> GetTestResults(string testResultsPath)
        {
            testResultsPath = testResultsPath ?? "";
            if (!Directory.Exists(testResultsPath))
            {
                Log.Warning("Test results directory doesn't exists");
                yield break;                
            }

            foreach (var testResult in Directory.EnumerateDirectories(testResultsPath))
            {
                var result = GetTestResult(testResult);
                var testResultFiles = result.TestResultFiles ?? Array<string>.Empty;
                if (testResultFiles.Length > 0)
                {
                    yield return result;
                }
            }
        }

        static TestResult GetTestResult(string testResultPath)
        {
            testResultPath = testResultPath ?? "";

            var result = new TestResult();

            result.Name = Path.GetFileNameWithoutExtension(testResultPath);

            Log.Info ("Processing test result in: {0}", result.Name);

            var configurationFilePath = Path.Combine(testResultPath, "testresult.config");
            var configurationFileName = Path.GetFileName (configurationFilePath);
            if (File.Exists(configurationFilePath))
            {
                Log.Info("Reading config file: {0}", configurationFileName);
                using (var configStream = new StreamReader(configurationFilePath))
                {
                    HRONDynamicParseError[] errors;
                    var parseResult = HRONSerializer.TryParseDynamic(
                        int.MaxValue,
                        configStream.ReadLines().Select (s => s.ToSubString ()),
                        out result.Configuration,
                        out errors
                        );

                    if (!parseResult)
                    {
                        Log.Error("Failed to read config file: {0}", configurationFileName);
                        return result;
                    }

                }
            }
            else
            {
                result.Configuration = HRONObject.Empty;
            }

            result.TestResultFiles = Directory.GetFiles(testResultPath, "*.actionlog");

            return result;
        }

        static IEnumerable<ReferenceData> GetReferenceData(string referenceDataPath)
        {
            referenceDataPath = referenceDataPath ?? "";
            if (!Directory.Exists(referenceDataPath))
            {
                Log.Warning("Reference data directory doesn't exists");
                yield break;
            }

            foreach (var referenceData in Directory.EnumerateFiles(referenceDataPath, "*.hron"))
            {
                var actionLog = referenceData + ".actionlog";
                if (File.Exists(actionLog))
                {
                    yield return
                        new ReferenceData
                            {
                                ReferenceDataPath = referenceData,
                                ActionLogPath = actionLog,
                            };
                }
                else
                {
                    Log.Warning (
                        "Couldn't find matching action log for reference data file: {0}",
                        Path.GetFileName (referenceData)
                        );
                }
            }
            
        }
    }
}