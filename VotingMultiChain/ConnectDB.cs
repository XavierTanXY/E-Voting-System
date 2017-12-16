using System;
using Gtk;
using System.Data;
using System.Timers;
using System.Threading;
using MySql.Data.MySqlClient;

namespace VotingMultiChain
{
	public class ConnectDB
	{
		private static ConnectDB dbInstance = null;
		private MySqlConnectionStringBuilder conString = null;
		private MySqlConnection connection = null;
		private bool validConnection = false;

		private string currentUsername;
		private string currentUserType;

		public const string BOTH_INCORRECT = "login details";
		public const string CORRECT_DETAILS = "correct details";
		public const string ALREADY_LOGGED_IN = "logged in";

		public const string VOTER = "Voter";
		public const string ADMIN = "Administrator";
		public const string NONE = "None";

		//Variables for login
		private int incrementNum;
		private System.Timers.Timer aTimer;
		private Thread newThread;


		private ConnectDB()
		{
			//when creating a database connection, set current login username to default
			currentUsername = "No user is logged in";
			currentUserType = NONE;

			incrementNum = 0;
			aTimer = new System.Timers.Timer();


		}

		public static ConnectDB createInstance() //Singleton pattern. Purpose: Ensure one machine can only have one database instance
		{
			if (dbInstance == null)
				dbInstance = new ConnectDB (); 
			else
				Console.WriteLine ("You already had a database instance!");
			return dbInstance;
		}

		public void setConnection(string hostName, uint port, string dbName, string dbUser, string passwd)
		{
			if (conString == null)
			{
				//Create a proper mysql connection info using MySqlConnectionStringBuilder
				conString = new MySqlConnectionStringBuilder ();
				conString.Server = hostName;
				conString.Port = port;
				conString.Database = dbName;
				conString.UserID = dbUser;
				conString.Password = passwd;
				//Build up mysql connection
				connection = new MySqlConnection (conString.ToString ());
			}
			else
				Console.WriteLine ("There's already existing connection setup!");
		}

		public void openConnection()
		{
			if (connection != null)
			{
				try
				{
					connection.Open ();
					validConnection = true;
				}
				catch (MySqlException ex)
				{
					switch (ex.Number)
					{
					case 0:
						Console.WriteLine ("Cannot connect to server.");
						break;
					case 1045:
						Console.WriteLine ("Invalid username or password.");
						break;
					default:
						Console.WriteLine (ex.Message);
						break;
					}
					connection = null;
				}
			}
			else
				Console.WriteLine ("Connection info has not been set up!");
		}

		public void closeConnection()
		{
			if (connection != null) {
				try
				{
					connection.Close ();
				}
				catch (MySqlException ex)
				{
					Console.WriteLine (ex.Message);
				}
			}
			else
				Console.WriteLine ("There's no connection to begin with!");
		}
			
		//This method can be modified to get more details about current user
		public string getCurrentUser()
		{
			return currentUsername;
		}

		//A new thread that runs every seconds when user logins
		public void newThreadMethod()
		{
			aTimer.Elapsed+=new ElapsedEventHandler(OnTimedEvent);
			aTimer.Interval=1000;
			aTimer.Enabled=true;
		}

		/* Set current user name to logged in username,
		 * Login by changing the login value to 'Y' in the database
		 * */
		public void login(string userName, string userType) 
		{
			currentUsername = userName;
			currentUserType = userType;

			newThread = new Thread (newThreadMethod);
			newThread.Start ();
		}

		/* Remove the attached current logged user name to logout
		 * Logout by changing the login value to 'N' in the database
		 * */
		public void logout()
		{
			string command = "";
			incrementNum = 0;

			if (currentUserType.Equals (VOTER)) {

				//Stop the thread when a user logouts
				command = $"call incrementVoterLogin('{currentUsername}', {incrementNum});";

			} else if (currentUserType.Equals (ADMIN)) {
				
				//command = $"call loginAdmin('N','{currentUsername}');";
				command = $"call incrementAdminLogin('{currentUsername}', {incrementNum});";

			}

			aTimer.Enabled = false;
			newThread.Abort();

			executeCommand (command);

			currentUsername = "No user is logged in";
			currentUserType = NONE;
		}

		//The thread will run this method every 1 sec
		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			incrementLogin ();
		}

		//Increment current user loginNum
		public void incrementLogin()
		{
			incrementNum = incrementNum + 1;
			string command = "";

			if (currentUserType.Equals (VOTER)) {
				command = $"call incrementVoterLogin('{currentUsername}', {incrementNum});";
			} else if (currentUserType.Equals (ADMIN)) {
				command = $"call incrementAdminLogin('{currentUsername}', {incrementNum});";
			}

			executeCommand (command);
		}

		//Check against the database using username and password whether this user exists
		public string userExists(Person person, string userType) 
		{
			string retVal = BOTH_INCORRECT;
			string command = "";

			string username = person.getUsername ();
			string password = person.getPassword ();

			/* Check against voter table if the user logins as voter or 
			 * check against admin table if ther user logins as admin
			 */

			if (userType.Equals ("Voter")) {

				//Check voter exists procedure
				command = $"call checkVoter('{username}','{password}');";

				if ( executeCommand (command) ) {

					/* After checking the correct username and password then 
					* check whether the voter has already logged in
					* */
					if (isVoterLoggedIn (username)) {
						retVal = ALREADY_LOGGED_IN;
					} else {
						retVal = CORRECT_DETAILS;
					}


				}


			} else if (userType.Equals ("Administrator")) {

				command = $"call checkAdmin('{username}','{password}');";

				if ( executeCommand (command) ) {

					if (isAdminLoggedIn (username)) {
						retVal = ALREADY_LOGGED_IN;
					} else {
						retVal = CORRECT_DETAILS;
					}
				}

			}


			return retVal;
		}

		/* compare initial and later loginNum to check whether it is logged in */
		public bool compareLoginNum(string command)
		{
			bool retVal = false;
			Console.WriteLine ("why");
			MySqlCommand cmd = connection.CreateCommand();
			cmd.CommandText = command;

			MySqlDataReader reader = cmd.ExecuteReader();

			reader.Read();

			//Get initial value of loginNum
			int firstLoginNum = reader.GetInt32 ("loginNum");
			reader.Close ();

			Console.WriteLine ("Logging in ..." + firstLoginNum);

			//Suspense thread to 3 sec to compare later value
			Thread.Sleep (3000);

			MySqlDataReader reader2 = cmd.ExecuteReader();

			reader2.Read();

			int secondLoginNum = reader2.GetInt32 ("loginNum");
			reader2.Close();

			/* If the value if not the same, which means it is already logged in
			 * The reason is because the user's loginNum is increment over time.
			 * If not the same, which means the user forgot to logout, then
			 * reset the login state and login.
			 * */

			if (secondLoginNum != firstLoginNum) {

				retVal = true;

			} else if (secondLoginNum == firstLoginNum) {

				if (currentUserType.Equals (VOTER)) {
					string logoutCommand = $"call incrementVoterLogin('{currentUsername}', 0);";
					executeCommand (logoutCommand);
				} else if (currentUserType.Equals (ADMIN)) {
					string logoutCommand = $"call incrementAdminLogin('{currentUsername}', 0);";
					executeCommand (logoutCommand);
				}


				retVal = false;

			}

			return retVal;
		}

		/* A method which checks whether a voter has already logged in on other station 
		 * before logging them in. 
		 * */
		public bool isVoterLoggedIn(string userName) 
		{
			string command = $"call getVoter('{userName}');";
			return compareLoginNum (command);
		}

		/* Same method as above, but for admin
		 * */
		public bool isAdminLoggedIn(string userName) 
		{
			string command = $"call getAdmin('{userName}');";
			return compareLoginNum (command);
		}

		//This method get candidates from database
		public Candidate[] getCandidates() 
		{
			string command = $"call getAllCandidates();";
			MySqlCommand cmd = connection.CreateCommand ();
			cmd.CommandText = command;

			MySqlDataReader reader = cmd.ExecuteReader();

			Candidate[] candidates = new Candidate[3];

			int i = 0;
			while(reader.Read())
			{
				String name = reader.GetString("name");
				String blockchainAddress = reader.GetString ("blockchainAddress");
				int id = reader.GetInt32 ("id");

				Candidate candidate = new Candidate (id, name, blockchainAddress);

				candidates [i] = candidate;
				i++;
			}

			reader.Close();

			return candidates;
		}

        //Get the canndidate address from database in order to send votes to them
        public string getCandidateAddress(int id)
        {
            string candidateAddress = "";

			string command = $"call getAllCandidates();";
			MySqlCommand cmd = connection.CreateCommand();
			cmd.CommandText = command;

			MySqlDataReader reader = cmd.ExecuteReader();

			int i = 1;
			while (reader.Read())
			{
                if(id == i)
                {
					candidateAddress = reader.GetString("blockchainAddress");
                }
				i++;
			}

			reader.Close();

            return candidateAddress;
        }


        //Save the address of the voter into the database (should be hidden for anonymity)
		public void saveVotingAddress(string votingAddress, string userName)
		{
			string command = $"call saveVotingAddress('{votingAddress}','{userName}');";
			MySqlCommand cmd = connection.CreateCommand();
			cmd.CommandText = command;
			cmd.ExecuteNonQuery();
		}

		//Save the vote status of a voter into db
		public void saveVoteStatus(string voteStatus, string userName)
		{
			string command = $"call saveVoteStatus('{voteStatus}','{userName}');";
			MySqlCommand cmd = connection.CreateCommand();
			cmd.CommandText = command;
			cmd.ExecuteNonQuery();
		}

		//Get the canndidate address from database in order to send votes to them
        public string getVoteStatus(string userName)
		{
			string voteStatus = "";

			string command = $"call getVoter('{userName}');";
			MySqlCommand cmd = connection.CreateCommand();
			cmd.CommandText = command;

			MySqlDataReader reader = cmd.ExecuteReader();

            reader.Read();

			if (reader["vote"] != DBNull.Value)
			{
				voteStatus = reader.GetString("vote");
			}
			else
			{
                voteStatus = null;
			}

			reader.Close();

			return voteStatus;
		}

		//Get the canndidate address from database in order to send votes to them
		public string getVoterAddress(string userName)
		{
			string voteAddress = "";

			string command = $"call getVoter('{userName}');";
			MySqlCommand cmd = connection.CreateCommand();
			cmd.CommandText = command;

			MySqlDataReader reader = cmd.ExecuteReader();

			reader.Read();


			if (reader["votingAddress"] != DBNull.Value)
			{
				voteAddress = reader.GetString("votingAddress");
			}
			else
			{
				voteAddress = null;
			}


			reader.Close();

			return voteAddress;
		}

		//Execute database commands if the data exists
		public bool executeCommand(string command)
		{
			bool status = false;
			if (validConnection)
			{
				MySqlCommand cmd = connection.CreateCommand ();
				cmd.CommandText = command;
				try
				{
					//cmd.ExecuteNonQuery ();
					//these lines below are for testing purpose
					MySqlDataReader reader = cmd.ExecuteReader();

					//if data exists reader reads data, else reader return empty set
					if( reader.HasRows ) 
					{
						status = true;
					} 
					else 
					{
						status = false;
					}

					reader.Close();

				}
				catch (MySqlException ex)
				{
					Console.WriteLine (ex.Message);
				}
			}
			else
				Console.WriteLine ("Connection is not valid yet");

			return status;
		}
	}
}

