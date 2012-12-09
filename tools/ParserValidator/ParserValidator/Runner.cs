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
// ReSharper disable PartialTypeWithSinglePart

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ParserValidator.Source.Common;
using ParserValidator.Source.Extensions;
using ParserValidator.Source.HRON;
using ParserValidator.Source.Reflection;

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
        public bool          EmptyContentLinesAndEmptyLinesAreConsideredEquivalent  ;
        public bool          CommentsAndCommentLinesAreConsideredEquivalent         ;
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
        static readonly ClassDescriptor s_classDescriptor = typeof (TestRunConfiguration).GetClassDescriptor();

        static int MaxErrors = 10;

        static partial void Partial_Run(string[] args, dynamic config)
        {
            {
                int maxErrors = config.ParserValidator.MaxErrors;
                if (maxErrors > 0)
                {
                    MaxErrors = maxErrors;
                }
            }
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
                    ProcessActionLog(referenceDataActionLog, testResult, testResultActionLog);
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

        sealed partial class ActionLog
        {
            static readonly Regex s_actionLogEntry = new Regex(
                @"^(?<tag>\w+)\:(?<data>.*)$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant
                );

            public readonly string      Name        ;
            public readonly string[]    Lines       ;
            public int                  LineNo      ;

            public int                  ErrorCount  ;

            public ActionLog(string name, string[] lines)
            {
                Name  = name ?? "NoName"; 
                Lines = lines ?? Array<string>.Empty;
                
            }

            public bool Advance()
            {
                if (HasContent)
                {
                    ++LineNo;
                }

                return HasContent;
            }

            public bool HasContent
            {
                get { return LineNo > -1 && LineNo < Lines.Length; }
            }

            public string Current
            {
                get { return HasContent ? Lines[LineNo] : ""; }
            }

            public Match GetMatch()
            {
                return s_actionLogEntry.Match(Current);
            }

            public bool ReportFailure(string tag, string failure, bool skipLine)
            {
                ++ErrorCount;
                Log.Error(
                    "{0}@{1} - {2} - {3}{4}",
                    Name,
                    LineNo + 1, 
                    tag,
                    failure ?? "Unknown",
                    skipLine ? "- Skipping line" : ""
                    );
                if (skipLine)
                {
                    return Advance();
                }
                else
                {
                    return true;
                }
            }

            public bool ReportAllIsGood(string tag)
            {
                return Advance();
            }
        }

        sealed partial class ValidateState
        {
            public readonly ActionLog   Reference   ;
            public readonly ActionLog   Result      ;

            public ValidateState(string[] referenceLines, string[] resultLines)
            {
                Reference   = new ActionLog ("Reference", referenceLines);
                Result      = new ActionLog ("Result", resultLines);
            }

            public bool ReportAllIsGood(string tag)
            {
                return Reference.ReportAllIsGood(tag) & Result.ReportAllIsGood(tag);
            }

            public bool ReportFailure(string referenceTag, string resultTag, string validationFailure, bool skipReferenceLine, bool skipResultLine)
            {
                return Reference.ReportFailure(referenceTag, validationFailure, skipReferenceLine) & Result.ReportFailure(resultTag, validationFailure, skipResultLine);
            }

            public bool HasContent
            {
                get { return Reference.HasContent && Result.HasContent; }
            }

            public int ErrorCount
            {
                get { return Math.Max(Reference.ErrorCount, Result.ErrorCount); }
            }

        }

        static readonly Regex s_commentContent = new Regex(
            @"^(?<indention>\d+),(?<content>.*)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
            );
        static void ProcessActionLog(
            ReferenceData referenceDataActionLog,
            TestResult testResult,
            TestResultActionLog testResultActionLog
            )
        {
            var config = s_classDescriptor
                .Members
                .Where(m => m.HasPublicGetter && m.MemberType == typeof (TestRunOption))
                .ToDictionary(m => m.Name, m => (TestRunOption)m.Getter(testResult.Configuration))
                ;

            var referenceLines = File.ReadAllLines(referenceDataActionLog.ActionLogPath);
            var resultLines = File.ReadAllLines(testResultActionLog.ActionLogPath);

            var state = new ValidateState (referenceLines, resultLines);

            while (state.HasContent && state.ErrorCount < MaxErrors)
            {
                var matchReferenceLine  = state.Reference.GetMatch();
                var matchResultLine     = state.Result.GetMatch();

                if (!matchReferenceLine.Success)
                {
                    state.Reference.ReportFailure("UnknownTag", "Invalid formatted line", skipLine: true);
                }

                if (!matchResultLine.Success)
                {
                    state.Result.ReportFailure("UnknownTag", "Invalid formatted line", skipLine: true);
                }

                var referenceTag = matchReferenceLine.Groups["tag"].Value;
                var referenceData = matchReferenceLine.Groups["data"].Value;

                var resultTag = matchResultLine.Groups["tag"].Value;
                var resultData = matchResultLine.Groups["data"].Value;

                var isIdentical = IsIdentical(testResult.Configuration, referenceTag, resultTag, referenceData, resultData); 

                if (isIdentical)
                {
                    state.ReportAllIsGood(referenceTag);
                    continue;
                }

                var referenceOption = config.Lookup(referenceTag);

                var hasIdenticalTag = 
                        referenceTag == resultTag
                    ||  (
                            testResult.Configuration.EmptyContentLinesAndEmptyLinesAreConsideredEquivalent 
                            && IsComment(referenceTag)
                            && IsComment(resultTag)
                        );

                switch (referenceOption)
                {
                    case TestRunOption.ValidateExistence:
                        if (hasIdenticalTag)
                        {
                            state.ReportAllIsGood(referenceTag);
                            continue;
                        }
                        else
                        {
                            state.ReportFailure(referenceTag, resultTag, "Validation failure (missing tag)", skipReferenceLine: true, skipResultLine: false);
                            continue;
                        }
                    case TestRunOption.ValidateContent:
                        if (hasIdenticalTag)
                        {
                            if (IsComment(referenceTag))
                            {
                                var referenceCommentContent = GetCommentContent(referenceData);
                                var resultCommentContent = GetCommentContent(resultData);

                                if (referenceCommentContent.Item2 != resultCommentContent.Item2)
                                {
                                    state.ReportFailure(referenceTag, resultTag, "Validation failure (content)", skipReferenceLine: true, skipResultLine: true);
                                }
                                else
                                {
                                    state.ReportAllIsGood(referenceTag);
                                }

                                continue;
                            }
                            else
                            {
                                if (referenceData != resultData)
                                {
                                    state.ReportFailure(referenceTag, resultTag, "Validation failure (content)", skipReferenceLine: true, skipResultLine: true);
                                    continue;
                                }
                                else
                                {
                                    state.ReportAllIsGood(referenceTag);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            state.ReportFailure(referenceTag, resultTag, "Validation failure (missing tag)", skipReferenceLine: true, skipResultLine: false);
                            continue;
                        }
                    case TestRunOption.Ignore:
                        if (hasIdenticalTag)
                        {
                            state.ReportAllIsGood(referenceTag);
                            continue;
                        }
                        else
                        {
                            state.Reference.ReportAllIsGood(referenceTag);
                            continue;
                        }
                        continue;
                    case TestRunOption.ValidateAll:
                    default:
                        state.ReportFailure(referenceTag, resultTag, "Validation failure (mismatch)", skipReferenceLine: true, skipResultLine:true);
                        continue;
                }
            }
        }

        static Tuple<int, string> GetCommentContent(string content)
        {
            content = content ?? "";

            var match = s_commentContent.Match(content);
            return match.Success
                       ? Tuple.Create(
                           match.Groups["indention"].Value.Parse(0),
                           match.Groups["content"].Value)
                       : Tuple.Create(
                           0,
                           content);
        }

        static bool IsComment(string tag)
        {
            switch (tag)
            {
                case "Comment":
                case "CommentLine":
                    return true;
                default:
                    return false;
            }
        }

        static bool IsIdentical(TestRunConfiguration config, string referenceTag, string resultTag, string referenceData, string resultData)
        {
            if (referenceTag == resultTag && referenceData == resultData)
            {
                return true;
            }

            var result = false;

            if (config.EmptyContentLinesAndEmptyLinesAreConsideredEquivalent)
            {
                result |= IsEmpty(referenceTag, referenceData) && IsEmpty(resultTag, resultData);
            }

            return result;
        }

        static bool IsEmpty(string tag, string data)
        {
            switch (tag)
            {
                case "EmptyLine":
                case "Empty":
                    return true;
                case "ContentLine":
                    return string.IsNullOrWhiteSpace(data);
                default:
                    return false;
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

                    result.Configuration.EmptyContentLinesAndEmptyLinesAreConsideredEquivalent  = config.EmptyContentLinesAndEmptyLinesAreConsideredEquivalent  ;
                    result.Configuration.CommentsAndCommentLinesAreConsideredEquivalent         = config.CommentsAndCommentLinesAreConsideredEquivalent         ;
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