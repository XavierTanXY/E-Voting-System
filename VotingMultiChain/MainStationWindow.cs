using System;
using System.Globalization; //timezone
using MultiChainLib;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using Gtk;

namespace VotingMultiChain
{
	public partial class MainStationWindow : Gtk.Window
	{
		private MultiChainClient client;
		private Ssh sshClient;
		private ConnectDB db;
		private int currentNumVoters = 0; //should not exceed NUM_VOTERS
		private const int NUM_VOTERS = 49; //look at the database for this one. Currently only 49 records in database

		public MainStationWindow(MultiChainClient importClient, Ssh importSsh, ConnectDB importDB) :
		base(Gtk.WindowType.Toplevel)
		{
			client = importClient;
			sshClient = importSsh;
			db = importDB;

			Console.WriteLine ("current logged in user: " + db.getCurrentUser());
			this.Build();
		}

		private void votesTally()
		{
			//Record the timestamp when the votes tallying begins. This will clarify the total votes at certain time.
			DateTime localDate = DateTime.Now;
			CultureInfo culture = new CultureInfo ("en-US");
			string currentTime = localDate.ToString (culture);

			//get the list of candidates from database
			Candidate[] candidates = db.getCandidates();
			int length = candidates.Length; //we need to know how many candidates are there to match with our other arrays
			string[] votes = new string[length];

			//Adjusting the alignment to display candidates name and votes
			candidateNames.Text = "\nCandidates:\n";
			candidateResults.Text = "\nTotal Votes:\n";
			double totalVotes = 0.0; //default value, this will be used to calculate percentage vote
			for (int ii = 0; ii < length; ii++) {

				//get the candidate's blockchain address to retrieve the total votes
				string address = candidates [ii].getBlockchainAddress ();
				Task task = Task.Run (async () => {
					//Return the total votes of each candidate
					var candidateVote = await getTotalVotes (address);
					votes [ii] = candidateVote;
					totalVotes += Convert.ToDouble (votes [ii]);
				});
				task.Wait ();

				//Now, onto displaying the output
				candidateNames.Text += candidates [ii].getName () + "\n";
				candidateResults.Text += votes [ii] + "\n";
			}

			//Now onto displaying percentage vote
			percentageResults.Text = "\nTotal Votes (%):\n";
			for (int jj = 0; jj < length; jj++) {

				//calculate percentage vote
				double percentageVote = Convert.ToDouble (votes [jj]) / totalVotes * 100;
				double roundedPercentageVote = Math.Round (percentageVote, 2);
				percentageResults.Text += roundedPercentageVote + "%\n";
			}

			//display timestamp
			timeLabel.Text = currentTime;

			//count the number of voters that have voted
			currentNumVoters = (int) totalVotes;
		}

		private async Task<string> getTotalVotes(string inAddress)
		{
			var balance = await client.GetAddressBalancesAsync (inAddress, 1, false); //get JSON RPC Response
			var res = balance.Result; //parse Json RPC Response to List<AddressBalance>
			string result = res[0].Qty.ToString(); //get the details of total votes as string as the return value

			return result;
		}
			
		protected void OnDeleteEvent(object o, DeleteEventArgs args)
		{
			stopConnections ();
			Application.Quit();
			args.RetVal = true;
		}

		protected void OnLogOutButtonClicked(object sender, EventArgs e)
		{
			db.logout();
			VotingMultiChain.LoginWindow win = new VotingMultiChain.LoginWindow(client, sshClient, db);
			win.Show();
			this.Destroy();
		}

		protected void stopConnections()
		{
			db.logout();
			db.closeConnection ();
		}
			
		protected void OnCountingVoteClicked (object sender, EventArgs e)
		{
			if (currentNumVoters <= NUM_VOTERS) {
				currentNumVoters = 0; //reset
				votesTally ();
			} else {
				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Close,
					                   "All voters have voted.");
				md.Run ();
				md.Destroy ();
			}
		}

		protected void OnAuditButtonClicked (object sender, EventArgs e)
		{
			auditTransaction();
		}

		private void auditTransaction()
		{
		}
	}
}