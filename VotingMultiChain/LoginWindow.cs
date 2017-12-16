using System;
using Gtk;
using MultiChainLib;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

namespace VotingMultiChain
{
	public partial class LoginWindow : Gtk.Window
	{
		private MultiChainClient client;
		private Ssh sshClient;
		private ConnectDB db;

		private string userType;

		public const string BOTH_INCORRECT = "login details";
		public const string CORRECT_DETAILS = "correct details";
		public const string ALREADY_LOGGED_IN = "logged in";

		public const string MAIN_STATION = "Main Station";
		public const string VOTING_STATION = "Voting Station";

		public const string VOTER = "Voter";
		public const string ADMIN = "Administrator";

		private string station;


		public LoginWindow(MultiChainClient importClient, Ssh importSsh, ConnectDB importDB) :
		base(Gtk.WindowType.Toplevel)
		{

			client = importClient;
			sshClient = importSsh;
			db = importDB;

			this.Build();
			Pango.FontDescription fd = Pango.FontDescription.FromString("Arial bold 15");
			this.titleLabel.ModifyFont(fd);

			this.passwordEntry.WidthChars = 20;
			this.userEntry.WidthChars = 20;

			setStation ();
		}


		/* EDITED: sshClient will be uninitialised if it's from Main Station
		 * Otherwise, it is an voting station.
		 * */
		private void setStation()
		{
			bool mainStation = false; //let the default be Voting Station
			if (sshClient == null)
				mainStation = true;

			if (mainStation)
			{
				station = MAIN_STATION;
				this.Title = MAIN_STATION;
			}
			else
			{
				station = VOTING_STATION;
				this.Title = VOTING_STATION;
			}
		}

		//If login is valid go to either voting or main station screen
		protected void OnLoginButtonClicked(object sender, EventArgs e)
		{
			userType = userSelectionCombo.ActiveText;

			//First check the login details is correct
			if (loginIsValid (userEntry.Text, passwordEntry.Text, userType)) {

				db.login (userEntry.Text, userType);

				if (userType.Equals (VOTER)) 
				{
					//Voter voter = new Voter (userEntry.Text, passwordEntry.Text);
					//db.login (voter.getName());


					VotingMultiChain.VotingWindow win = new VotingMultiChain.VotingWindow (client, sshClient, db);
					win.Show ();
					this.Destroy ();
				} 
				else if (userType.Equals (ADMIN)) 
				{
					//Administrator admin = new Administrator(userEntry.Text, passwordEntry.Text);
					//db.login (admin.getName ());

					VotingMultiChain.MainStationWindow win = new VotingMultiChain.MainStationWindow (client, sshClient, db);
					win.Show ();
					this.Destroy ();
				}
			} 
		}

		//Checks database to validate login credentials
		private Boolean loginIsValid(String userName, String password, String userType)
		{
			Boolean valid = false;

			//Does not allow admin to login to voting station
			if (userType.Equals (ADMIN) && station.Equals (VOTING_STATION)) {

				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
					MessageType.Error, ButtonsType.Close,
					"Administrator is not allowed to login to voting station");
				md.Run ();
				md.Destroy ();

			} else if (userType.Equals (VOTER) && station.Equals (MAIN_STATION)) {

				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
					MessageType.Error, ButtonsType.Close,
					"Voter is not allowed to login to main station");
				md.Run ();
				md.Destroy ();

			} else {
				Person person = new Person (userName, password);

				string validation = db.userExists (person, userType); 

				//If all details are correct, return true
				if (validation == CORRECT_DETAILS) {
					valid = true;
				} else if (validation == BOTH_INCORRECT) {

					/*Check for different login cases: 
				* both user name and password are incorrect
				* Then output appropriate message to user
				* */

					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
						MessageType.Error, ButtonsType.Close,
						"Please ensure that you have entered the correct login details.");
					md.Run ();
					md.Destroy ();


				} else if (validation == ALREADY_LOGGED_IN) {

					MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
						MessageType.Error, ButtonsType.Close,
						"This user is already logged in on other station.");
					md.Run ();
					md.Destroy ();
				}
			}



			return valid;
		}

		protected void OnDeleteEvent(object o, DeleteEventArgs args)
		{
			stopConnections ();
			Application.Quit();
			args.RetVal = true;
		}

		//Stops all connections when the program exits to prevent connection running
		protected void stopConnections()
		{
			db.closeConnection ();
			if (station.Equals (VOTING_STATION))
			{
				sshClient.stopPort ();
				sshClient.closeConnection ();
			}
		}
	}
}
