using System;
using MultiChainLib;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using Gtk;

namespace VotingMultiChain
{
    public partial class VotingWindow : Gtk.Window
    {
        private MultiChainClient client;
		private Ssh sshClient;
		private ConnectDB db;
		string voterAddress;

        //Used for an error dialogue if a voter logs in and doesnt vote
        private Boolean hasVoted = false;

        // False when a disconnect causes error state in CheckIfVoteWasSent()
        // Or if the database indicates a voter has succesfully voted
        private Boolean votingProcedure = true;

		public VotingWindow(MultiChainClient importClient, Ssh importSsh, ConnectDB importDB) :
                base(Gtk.WindowType.Toplevel)
        {
            client = importClient;
			sshClient = importSsh;
			db = importDB;

			Console.WriteLine ("current logged in user: " + db.getCurrentUser());
            this.Build();
            Pango.FontDescription fd = Pango.FontDescription.FromString
                                                    ("Arial bold 15"); 
            this.votingTitle.ModifyFont(fd);
            GetCandidateName();
        }

        // Gets candidates from the database and adds them to a candidate array 
        // which is used to label each button.
        public void GetCandidateName()
        {
			Candidate[] candidates = db.getCandidates ();

			candidateOne.Label = candidates[0].getName();
			candidateTwo.Label = candidates[1].getName();
			candidateThree.Label = candidates[2].getName();
        }

        protected void OnCandidateOneClicked(object sender, EventArgs e)
        {
            startVote(1);
        }

        protected void OnCandidateTwoClicked(object sender, EventArgs e)
        {
            startVote(2);
        }

        protected void OnCandidateThreeClicked(object sender, EventArgs e)
        {
            startVote(3);
        }

        //Starts the voting procedure by creating an address for the votee and then sending that 
        //vote to a candidate, currently allows more than 1 vote 
        private void startVote(int candidateNum)
		{
			
			try
			{
                //If the voter has already successfully voted dont vote again
                if (db.getVoteStatus(db.getCurrentUser()) == ("YES"))
                {
                    //No longer need to send a vote
                    votingProcedure = false;
                }
                //Else check if an existing address is associated with the voter if so check if he has sent a vote
                else if ((db.getVoterAddress(db.getCurrentUser()) != "NULL") &&
                          db.getVoterAddress(db.getCurrentUser()) != "" &&
						  db.getVoterAddress(db.getCurrentUser()) != null) 
                {
					var task = Task.Run(async () =>
					{
                        await CheckIfVoteWasSent(db.getVoterAddress(db.getCurrentUser()));

					});
					task.Wait();
                }
                // If votingProcedure is still true voter has never voted before, 
                // give them a new address and a vote to use
                if(votingProcedure)
                {
					var task = Task.Run(async () =>
                    {
                        // Address needs recieve permissions in order to be granted one vote
                        // Send permissions are enabled automatically on the current chain
	                    await CreateAddressAsync(BlockchainPermissions.Receive);
                        await SendVoteAsync(candidateNum);
                    });
				    task.Wait();
                }
                // The voter has already voted therefore do not vote again. Only
                // show them who they have voted for
                else
                {
					//Used to store the name of the candidate the voter has voted for
					string result = "";

					// Create a task that searches the database for the name of candidate voted for
					Task task = Task.Run(async () =>
					{
						//Returns the name of the candidate that has been voted for
						result = await CheckPastVote(db.getVoterAddress(db.getCurrentUser()));

					});
					task.Wait();

					//Create a dialogue box informing the user of which candidate they have voted for
					MessageDialog md = new MessageDialog(this, DialogFlags.DestroyWithParent,
					MessageType.Info, ButtonsType.Close,
					"You have already voted for " + result);
					md.Run();
					md.Destroy();
                }
			}

            //The connection to the chain has been interrupted put the user back to initial window
			catch (Exception ex)
			{
				Console.WriteLine("******************");
				Console.WriteLine(ex);

				MessageDialog md = new MessageDialog(this, DialogFlags.DestroyWithParent,
													 MessageType.Error, ButtonsType.Close,
				"Error connecting to chain please reconnect");
				md.Run();
				md.Destroy();
                stopConnections();

				MainWindow win = new MainWindow();
				win.Show();
				this.Destroy();
			}
        }

		//Function to create a new address 
		//Also gives it permissions and a single vote which can be sent
		private async Task CreateAddressAsync(BlockchainPermissions permissions)
		{
			// Create a new address
			var newAddress = await client.GetNewAddressAsync();
			newAddress.AssertOk();
			Console.WriteLine("New issue address: " + newAddress.Result);
			voterAddress = newAddress.Result;

            // Grant new address permissions (recieve permissions)
			Console.WriteLine("Grant new address permissions so it can vote");
			var perms = await client.GrantAsync(new List<string>() { newAddress.Result }, permissions);
			Console.WriteLine(perms.RawJson);
			perms.AssertOk();

            // Create a single vote to be granted to the new address (only one vote will ever
            // be given to an address)
            Console.WriteLine("Give the new address a single vote asset");
			var moreAsset = await client.IssueMoreAsync(voterAddress, "Votes", 1);
			Console.WriteLine(moreAsset.RawJson);
			moreAsset.AssertOk();

            // Associated the voter address with the voter in the database
            db.saveVotingAddress(voterAddress, db.getCurrentUser());
		}

        //Sends the Votes asset to a single candidate as registered in the database
        private async Task SendVoteAsync(int candidateNum)
        {
            Console.WriteLine("Send Vote");

            //Get the address of the candidate to send Vote to
            string candidateAddress = db.getCandidateAddress(candidateNum);
            Console.WriteLine("Candidate Address: " + candidateAddress);

            //Send the Vote to a candidate address
            var sendAsset = await client.SendAssetFromAsync(voterAddress, candidateAddress, "Votes", 1);
            Console.WriteLine(sendAsset.RawJson);
            sendAsset.AssertOk();

            // Set has voted to true for the purpose of the GUI. Will now give now warning
            // when logging out.
            hasVoted = true;
            Console.WriteLine("Vote Succesful!");

            //Save the voting status to the user that voted
			db.saveVoteStatus("YES", db.getCurrentUser());
        }

        //Used to check if a voter has already sent a vote
        private async Task CheckIfVoteWasSent(string inAddress)
        {
			Console.WriteLine("Checking current address for votes");

            //Check current balance to ensure there is a -1 transaction which indicates a vote
            var balance = await client.ListAddressTransactions(inAddress);
            string voteCheck = "\"qty\":-1.00000000";
            bool voteSuccess = balance.RawJson.Contains(voteCheck);

            //If the current user has already voted do not allow them to vote again
            if (voteSuccess)
            {
                //Set voting procedure to false so the voter can not vote again and
                // can see who they have voted for
                votingProcedure = false;

                // Correctly set the database entry to yes to fix the voting status
				db.saveVoteStatus("YES", db.getCurrentUser());
            }
        }

        //Function used to find the name of a candidate the voter voted for.
		private async Task<string> CheckPastVote(string inAddress)
		{
			// Get a list of all previous transactions associated with the voter
			var balance = await client.ListAddressTransactions(inAddress);

            // Create an array of all candidates and their information
            Candidate[] candidates = db.getCandidates();
            String candidateName = "";

            // For each candidate check their address to see if it is associated with
            // the voters transactions. Since a voter can only have one vote we can be sure
            // only one candidate will appear if they have voted
            for (int i = 0; i< candidates.Length; i++ )
            {
                if(balance.RawJson.Contains(candidates[i].getBlockchainAddress()))
                {
                    candidateName = candidates[i].getName();
                    break;
                }
            }

            //Return the candidate name
            return candidateName;
		}

		protected void OnDeleteEvent(object o, DeleteEventArgs args)
		{
			stopConnections();
			Application.Quit();
			args.RetVal = true;
		}

		protected void stopConnections()
		{
			db.logout();
			db.closeConnection();
			sshClient.stopPort();
			sshClient.closeConnection();
		}

		protected void OnLogOutButtonClicked(object sender, EventArgs e)
		{
            //If the user has not tried to vote yet have a message dialogue pop up when logging out
            if(!hasVoted)
            {
				var dialog = new MessageDialog(this,
												DialogFlags.Modal,
												MessageType.Info,
												ButtonsType.YesNo,
												"Are you sure you want to logout?");
				dialog.Response += (object o, ResponseArgs args) =>
				{
					if (args.ResponseId == ResponseType.Yes)
					{
						db.logout();
						VotingMultiChain.LoginWindow win = new VotingMultiChain.LoginWindow(client, sshClient, db);
						win.Show();
						this.Destroy();
					}
					else if (args.ResponseId == ResponseType.No)
					{
						//Do nothing user is still on the voting screen
					}
					else if (args.ResponseId == ResponseType.DeleteEvent)
					{
						//Do nothing user is still on the voting screen
					}
					dialog.Destroy();
				};

				dialog.Run();
     
            }
            //User has already voted logging out is fine no need for message
            else
            {
				db.logout();
				VotingMultiChain.LoginWindow win = new VotingMultiChain.LoginWindow(client, sshClient, db);
				win.Show();
				this.Destroy();    
            }
		}
    }
}