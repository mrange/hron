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

using M3.HRON.Source.Common;

namespace M3.HRON.Sample
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Log.Info("Attemping to parse HRON document");
            dynamic hron = HronDocument.ParseAsHRON();
            if (hron != null)
            {
                Log.Success("Successfully parsed HRON document");
                
                string logPath          = hron.Common.LogPath;
                string welcomeMessage   = hron.Common.WelcomeMessage;

                Log.Info(welcomeMessage);

                dynamic[] databaseConnections = hron.DataBaseConnection;

                Log.Info("Found {0} database connection(s)", databaseConnections.Length);
                foreach (var databaseConnection in databaseConnections)
                {
                    string name             = databaseConnection.Name;
                    string connectionString = databaseConnection.ConnectionString;
                    string timeOut          = databaseConnection.TimeOut;

                    string userName         = databaseConnection.User.UserName;
                    string password         = databaseConnection.User.Password;

                    Log.HighLight   ("Database connection   : {0}", name);
                    Log.Info        ("Connection string     : {0}", connectionString);
                    Log.Info        ("TimeOut               : {0}", timeOut);
                    Log.Info        ("UserName              : {0}", userName);
                    Log.Info        ("Password              : {0}", password);
                }

            }
            else
            {
                Log.Error("Failed to parse HRON document");                
            }
        }

        const string HronDocument = @"# This is an ini file using hron

# object values are started with '@'
@Common
	=LogPath
		Logs\CurrentDay
	=WelcomeMessage
		Hello there!

		String values in hron is started with '='

		Just as with Python in hron the indention is important

		Idention promotes readability but also allows hron string values 
		to be multi-line and hron has no special letters that requires escaping
		
		Letters like this causes hron no problems: &<>\'@=

		This helps readability


@DataBaseConnection
	=Name
		CustomerDB
	=ConnectionString
		Data Source=.\SQLEXPRESS;Initial Catalog=Customers
	=TimeOut
		10
	@User
		=UserName
			ATestUser
		=Password
			123
@DataBaseConnection
	=Name
		PartnerDB
	=ConnectionString
		Data Source=.\SQLEXPRESS;Initial Catalog=Partners

";
    }
}
