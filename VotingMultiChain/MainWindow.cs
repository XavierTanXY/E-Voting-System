using System;
using Gtk;
using MultiChainLib;
using System.Threading;
using System.Threading.Tasks;
using System.Net; //for IPAddress.Loopback
using System.Collections.Generic; //for List usage
using Renci.SshNet; //from SSH.NET package

namespace VotingMultiChain
{

	public partial class MainWindow : Gtk.Window
	{
	    //Allow all functions to make use of the MultiChainClient client
	    private MultiChainClient client;
		private Ssh sshClient = null;
		private ConnectDB db;

	    private Boolean clientConnected = false;

		//STATION DETAILS
		private bool mainStation = false;
		private const string MAIN_STATION_IP = "192.168.1.10";
		//MULTICHAIN CLIENT DETAILS
		private const int RPC_PORT = 7322;
		private const string RPC_USERNAME = "blockchainrpc";
		private const string RPC_PASSWORD = "blockchain5";
		private const string CHAIN_NAME = "blockchain";


	    public MainWindow() : base(Gtk.WindowType.Toplevel)
	    {
	        Build();
	        Pango.FontDescription fd = Pango.FontDescription.FromString("Arial bold 15");
	        this.labelTitle.ModifyFont(fd);
	        this.portEntry.WidthChars = 7;
		}

	    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	    {
			stopConnections ();
	        Application.Quit();
	        a.RetVal = true;
	    }

	    protected void OnClientConnectClicked(object sender, EventArgs e)
	    {
	       
			// Call task to connect to client blockchain
			try
			{
				isMainStation(); //Check whether this machine is the main station
				if (!mainStation) //Only Voting Stations require SSH connection to access database in Main Station
				   connectToSsh();
				connectToDB();

	            var task = Task.Run(async () =>
	            {
	               await connectToChain();
	            });

				task.Wait();  
			}
			catch (Exception ex)
			{
				Console.WriteLine("******************");
				Console.WriteLine(ex);

				MessageDialog md = new MessageDialog(this, DialogFlags.DestroyWithParent,
	                                                 MessageType.Error, ButtonsType.Close,
	            "Could not connect to chain, please ensure you have entered the correct details.");
				md.Run();
				md.Destroy();
			}

	        //Open login screen and close this if client connected successfully
			if (clientConnected)
			{
				VotingMultiChain.LoginWindow win = new VotingMultiChain.LoginWindow(client, sshClient, db);
				win.Show();
				this.Destroy();
			}
	    }

		//Stops all connections when the program exits to prevent connection running 
		protected void stopConnections()
		{
			db.closeConnection ();
			if (!mainStation) //Main Station does not use SSH! Only Voting Station needs to clean up SSH
			{
				sshClient.stopPort ();
				sshClient.closeConnection ();
			}
		}

		//Connects to ssh
		protected void connectToSsh()
		{
			// parameter:ip, port, main machine name, ssh password (can leave it empty now, need to fix), path to ssh keys - "/home/current_machine_name/.ssh/id_rsa", passphrase
			sshClient = new Ssh(MAIN_STATION_IP, 2299, "coccp5", "", "/home/coccp5/.ssh/id_rsa", "blockchain5");
			sshClient.openConnection ();
			sshClient.portForwarding ();

			Console.WriteLine ("Connected to Ssh!");
		}

		//Connects to database if ssh is successfully connected
		protected void connectToDB()
		{
			Console.WriteLine ("Connected to Database!");
			db = ConnectDB.createInstance();
			if (!mainStation)
			{
				ForwardedPortLocal port = sshClient.getPort ();

				//change it when needed
				//database name, username for loggin to database, password for loggin to database
				//you can add table or change database if you wish
				db.setConnection (port.BoundHost, port.BoundPort, "eVoting", "blockchain5", "blockchain5");
			}
			else
				db.setConnection ("localhost", 3306, "eVoting", "blockchain5", "blockchain5");
			db.openConnection ();

		}

		internal async Task connectToChain()
		{
	        //blockchain data user entry uncomment in production build
	        /**
	        String ipAddress = ipEntry.Text;
	        int portNum = Convert.ToInt32(portEntry.Text);
	        String rpcUserName = rpcUserEntry.Text;
	        String rpcPass = rpcPasswordentry.Text;
	        String chainName = chainNameEntry.Text;
	        client = new MultiChainClient(ipAddress, portNum, false, rpcUserName, rpcPass, chainName);
	        **/

	        //Fast client data entry here for development comment or change as needed
			client = new MultiChainClient(MAIN_STATION_IP, RPC_PORT, false, RPC_USERNAME, RPC_PASSWORD, CHAIN_NAME);

	        //Print out chain name to console to be sure we are connected
	        var info = await client.GetInfoAsync();
	        info.AssertOk();
	        Console.WriteLine("ChainName: {0}", info.Result.ChainName);
	        Console.WriteLine();
	        clientConnected = true;	  
		}

		public void isMainStation()
		{
			string hostName = Dns.GetHostName (); //get my hostname
			IPHostEntry myHost = Dns.GetHostEntry (hostName); //get my host IP to resolve
			string IPAddress = myHost.AddressList [0].ToString (); //get my IP address
			mainStation = (IPAddress == MAIN_STATION_IP); //true = Main Station, false = Voting Station
		}
	}
}
