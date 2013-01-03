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

namespace M3.HRON.Sample
{
    using System.Dynamic;
    using System.Collections.Generic;

    using Source.Common;

    enum UserType
    {
        User        ,
        SuperUser   ,
    }

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
                    int    timeOut          = databaseConnection.TimeOut;

                    string userName         = databaseConnection.User.UserName;
                    string password         = databaseConnection.User.Password;

                    UserType userType       = databaseConnection.User.Type;

                    Log.HighLight   ("Database connection   : {0}", name);
                    Log.Info        ("Connection string     : {0}", connectionString);
                    Log.Info        ("TimeOut               : {0}", timeOut);
                    Log.Info        ("UserName              : {0}", userName);
                    Log.Info        ("Password              : {0}", password);
                    Log.Info        ("UserType              : {0}", userType);
                }

            }
            else
            {
                Log.Error("Failed to parse HRON document");                
            }

            {
                dynamic billg       = new ExpandoObject();
                billg.Type          = "Person"  ;
                billg.FirstName     = "Bill"    ;
                billg.LastName      = "Gates"   ;
                billg.YearOfBirth   = 1955      ;

                dynamic melindag        = new ExpandoObject();
                melindag.Type           = "Person"  ;
                melindag.FirstName      = "Melinda" ;
                melindag.LastName       = "Gates"   ;
                melindag.YearOfBirth    = 1964;

                billg.Spouse            = melindag;


                IDictionary<string, object> dictionary = billg;
                // Note: Currently the serializer can't handle a recursive 
                // structure and will simply get stuck
                Log.Success(dictionary.SerializeKeyValuePairsAsHRON());
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
		=Type
			SuperUser
@DataBaseConnection
	=Name
		PartnerDB
	=ConnectionString
		Data Source=.\SQLEXPRESS;Initial Catalog=Partners

";
    }
}
