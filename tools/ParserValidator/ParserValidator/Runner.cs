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

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ParserValidator.Source.Common;
using ParserValidator.Source.Extensions;
using ParserValidator.Source.HRON;

namespace ParserValidator.Source.Common
{
}

namespace ParserValidator.Source.ConsoleApp
{
    sealed partial class ReferenceData
    {
        public string ReferenceDataPath ;
        public string ActionLogPath     ;

        public string ReferenceDataName
        {
            get { return Path.GetFileName(ReferenceDataPath ?? ""); }
        }

        public string ActionLogName
        {
            get { return Path.GetFileName(ActionLogPath ?? ""); }
        }
    }

    sealed partial class TestResultActionLog
    {
        public string ActionLogPath;
        public string ActionLogName
        {
            get { return Path.GetFileName(ActionLogPath ?? ""); }
        }
    }

    sealed partial class TestResult
    {
        public string                   Path                ;
        public string                   Name
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(Path); }
        }

        public TestRunConfiguration     Configuration       ;
        public TestResultActionLog[]    ActionLogs          ;
    }

    enum TestRunOption
    {
        ValidateAll         ,
        ValidateContent     ,
        ValidateExistence   ,
        Ignore              ,
    }

    sealed partial class TestRunConfiguration
    {
        public TestRunOption PreProcessor   ;

        public TestRunOption Object_Begin   ;
        public TestRunOption Object_End     ;

        public TestRunOption Value_Begin    ;
        public TestRunOption Value_End      ;

        public TestRunOption Comment        ;
        public TestRunOption Empty          ;

        public TestRunOption ContentLine    ;
        public TestRunOption EmptyLine      ;
        public TestRunOption CommentLine    ;


    }

    partial class Runner
    {

        static readonly Regex s_actionLogEntry = new Regex(
            @"^(?<entry>\w+)\:(?<data>.*)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
            );

        static partial void Partial_Run(string[] args, dynamic config)
        {
            try
            {
                var referenceDataPath = Path.GetFullPath(@"..\..\..\..\..\reference-data");
                var testResultsPath = Path.Combine(referenceDataPath, "test-results");

                Log.HighLight("ParserValidator starting...");

                Log.Info("Looking for reference data and matching action logs here: {0}", referenceDataPath);

                var referenceData = GetReferenceData(referenceDataPath).ToArray();
                if (referenceData.Length == 0)
                {
                    Log.Warning("No reference data found, terminating");
                    return;
                }

                Log.Success("Found #{0} reference data files", referenceData.Length);

                Log.Info("Looking for test results here: {0}", testResultsPath);

                var testResults = GetTestResults(testResultsPath).ToArray();

                if (testResults.Length == 0)
                {
                    Log.Warning("No test results found, terminating");
                    return;
                }

                Log.Success("Found #{0} test results", testResults.Length);

                Log.HighLight("Validation step is starting...");


                var referenceActionLogs = referenceData.ToDictionary(
                    rd => rd.ActionLogName,
                    StringComparer.OrdinalIgnoreCase
                    );

                foreach (var testResult in testResults)
                {
                    ProcessTestResult(testResult, referenceActionLogs);
                }
            }
            finally
            {
                Log.HighLight("Validation has completed");
                var isSuccess = Log.SuccessCount > 0 && Log.ErrorCount == 0; 
                Log.Success("#{0} Successes", Log.SuccessCount);
                Log.Warning("#{0} Warnings", Log.WarningCount);
                Log.Error("#{0} Errors", Log.ErrorCount);

                if (isSuccess)
                {
                    Log.Success("Overall it was a SUCCESS");
                }
                else
                {
                    Log.Error("Overall it was a FAILURE");
                }
            }
        }

        static void ProcessTestResult(TestResult testResult, Dictionary<string, ReferenceData> referenceActionLogs)
        {
            Log.Info("Processing test result: {0}", testResult.Name);

            foreach (var testResultActionLog in testResult.ActionLogs)
            {
                Log.Info("Processing test result action log: {0}", testResultActionLog.ActionLogName);
                var referenceDataActionLog = referenceActionLogs
                    .Lookup(testResultActionLog.ActionLogName)
                    ;

                if (referenceDataActionLog == null)
                {
                    Log.Warning("No matching reference action log found: {0}", testResultActionLog.ActionLogName);
                    continue;
                }

                try
                {
                    var referenceLines = File.ReadAllLines(referenceDataActionLog.ActionLogPath);
                    var resultLines = File.ReadAllLines(testResultActionLog.ActionLogPath);

                    var referenceLineNo = 0;
                    var resultLineNo = 0;

                    while (referenceLineNo < referenceLines.Length && resultLineNo < resultLines.Length)
                    {
                        try
                        {
                            var referenceLine = referenceLines[referenceLineNo];
                            var resultLine = resultLines[resultLineNo];

                            var matchReferenceLine = s_actionLogEntry.Match(referenceLine);
                            var matchResultLine = s_actionLogEntry.Match(resultLine);
                        }
                        finally
                        {
                            ++referenceLineNo;
                            ++resultLineNo;
                        }
                    }

                    Log.Success("Success");
                }
                catch (ExitCodeException)
                {
                    throw;
                }
                catch (Exception exc)
                {
                    Log.Warning("Error while processing result action log: {0}", exc.Message);
                }
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
                TestResult result = null;
                try
                {
                    result = GetTestResult(testResult);
                }
                catch (ExitCodeException)
                {
                    throw;
                }
                catch (Exception exc)
                {
                    Log.Warning("Failed to build testresults from: {0}, reason: {1}", testResultsPath, exc.Message);
                }

                if (result == null)
                {
                    continue;
                }

                var testResultFiles = result.ActionLogs ?? Array<TestResultActionLog>.Empty;
                if (testResultFiles.Length == 0)
                {
                    continue;
                }

                yield return result;
            }
        }

        static TestResult GetTestResult(string testResultPath)
        {
            testResultPath = testResultPath ?? "";

            var result = new TestResult
                             {
                                 Path = testResultPath                      ,
                                 Configuration = new TestRunConfiguration() ,
                             };

            Log.Info ("Processing test result in: {0}", result.Name);

            var configurationFilePath = Path.Combine(testResultPath, "parser_validator_config.hron");
            var configurationFileName = Path.GetFileName (configurationFilePath);
            if (File.Exists(configurationFilePath))
            {
                Log.Info("Reading config file: {0}", configurationFileName);
                using (var configStream = new StreamReader(configurationFilePath))
                {
                    dynamic config;
                    HRONDynamicParseError[] errors;
                    var parseResult = HRONSerializer.TryParseDynamic(
                        int.MaxValue,
                        configStream.ReadLines().Select (s => s.ToSubString ()),
                        out config,
                        out errors
                        );

                    if (!parseResult)
                    {
                        Log.Error("Failed to read config file: {0}", configurationFileName);
                        return result;
                    }

                    result.Configuration.PreProcessor   = config.PreProcessor   ;
                    result.Configuration.Object_Begin   = config.Object_Begin   ;
                    result.Configuration.Object_End     = config.Object_End     ;
                    result.Configuration.Value_Begin    = config.Value_Begin    ;
                    result.Configuration.Value_End      = config.Value_End      ;
                    result.Configuration.Comment        = config.Comment        ;
                    result.Configuration.Empty          = config.Empty          ;
                    result.Configuration.ContentLine    = config.ContentLine    ; 
                    result.Configuration.EmptyLine      = config.EmptyLine      ;
                    result.Configuration.CommentLine    = config.CommentLine    ;
                }
            }

            result.ActionLogs = Directory
                .EnumerateFiles(testResultPath, "*.actionlog")
                .Select(p => new TestResultActionLog {ActionLogPath = p})
                .ToArray()
                ;

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