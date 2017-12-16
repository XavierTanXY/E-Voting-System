using System;
using System.Data;
using MySql.Data.MySqlClient;

public class Test
{
	public static void Main(string[] args)
	{
		//Login details for database
		string connectionString =
			"Server=localhost;" +
				"Database=E_Voting;" +
				"User ID=root;" +
				"Password=soccp5;" +
				"Pooling=false";
		
		//Establish a connection to database
		IDbConnection dbcon;
		dbcon = new MySqlConnection(connectionString);
		dbcon.Open();
		IDbCommand dbcmd = dbcon.CreateCommand();

		/**
			SQL command
			For testing database models purpose, 
			I have inserted a user and set the password using sha1 function into database.
			So that the password will not be visible to others.
			Then from a c# application, I create this sql command to validate that user with sha1 fuction.
			It outputs the correct user's details. 
			This method can be used later for logging in whether in Voting or Main Station 

		*/
		string sql =
			"SELECT * " 
			+ "FROM Voter " + "WHERE id = 1 and password = sha1('password')";
		dbcmd.CommandText = sql;
		
		//Create a reader to read from database when the password matches.
		IDataReader reader = dbcmd.ExecuteReader();
		
		while(reader.Read()) {
			string FirstName = (string) reader["firstname"];
			string LastName = (string) reader["lastname"];
			string password = (string) reader["password"];
			int userID = (int) reader["id"];
			string voted = (string) reader["voted"];	

			//The default value for this field is NULL on database.
			string votingAddress = "Null";
			//If this value from db is not null, then set the value
			if (!Convert.IsDBNull (reader ["votingAddress"])) {
				votingAddress = (string)reader ["votingAddress"];
			}

			//Outputs results
			Console.WriteLine("ID: " + userID + ", Name: " + FirstName + " " + LastName + ", voted: " + voted + ", voting address: " + votingAddress );
		}

		// clean up
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbcon.Close();
		dbcon = null;
	}
}
