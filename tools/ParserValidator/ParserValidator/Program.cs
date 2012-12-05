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

// ReSharper disable InconsistentNaming

using System.IO;
using ParserValidator.Source.Common;

namespace ParserValidator
{
    using ParserValidator.Source.ConsoleApp;

    partial class Program
    {
        static void Main(string[] args)
        {
            Runner.Run(args);
        }
    }

    namespace Source.ConsoleApp
    {
        partial class Runner
        {
            static partial void Partial_Run(string[] args, dynamic config)
            {
                var referenceDataPath = Path.GetFullPath(@"..\..\..\..\..\reference-data");
                var testResultsPath = Path.Combine(referenceDataPath, "test-results");

                Log.Info("{0} : {1}", referenceDataPath, Directory.Exists(referenceDataPath));
                Log.Info("{0} : {1}", testResultsPath, Directory.Exists(testResultsPath));


            }
        }
    }
}
