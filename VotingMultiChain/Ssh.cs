using System;
using System.Net; //for IPAddress.Loopback
using System.Collections.Generic; //for List usage
using Renci.SshNet; //from SSH.NET package

namespace VotingMultiChain
{
	public class Ssh
	{
		private ConnectionInfo conInfo = null;
		private SshClient client = null;
		private ForwardedPortLocal port = null;
		private bool connect = false; //default: has no connection
		private string ipAddress;

		public Ssh (string ipAddr, int port, string userName, string passwd, string keyFilePath, string passphrase)
		{
			setConnection (ipAddr, port, userName, passwd, keyFilePath, passphrase); //set up connection info
			client = new SshClient (conInfo); //build up connection based on the previous method
			ipAddress = ipAddr;
		}

		private void setConnection(string ipAddr, int port, string userName, string passwd, string keyFilePath, string passphrase)
		{
			//Set the private key file
			var keyFile = new PrivateKeyFile(keyFilePath, passphrase);
			var keyFiles = new[] { keyFile };
			//Set the authentication methods
			var methods = new List<AuthenticationMethod>();
			methods.Add(new PasswordAuthenticationMethod(userName, passwd)); //User-password based authentication
			methods.Add(new PrivateKeyAuthenticationMethod(userName, keyFiles)); //key-based authentication

			conInfo = new ConnectionInfo (ipAddr, port, userName, methods.ToArray());
		}

		public void openConnection()
		{
			if (client != null)
			{
				client.Connect ();
				connect = true;
			}
			else
				Console.WriteLine ("You cannot start a connection without setting up the client!");
		}

		public void closeConnection()
		{
			if (connect)
			{
				client.Disconnect ();
				client.Dispose ();
			}
			else
				Console.WriteLine ("No connection to be closed!");
		}

		//ssh tunnel for database remote connection
		public void portForwarding()
		{
			if (connect)
			{
				port = new ForwardedPortLocal (IPAddress.Loopback.ToString(), 4479, "127.0.0.1", 3306);
				client.AddForwardedPort (port);
				port.Start ();
			}
			else
				Console.WriteLine ("Client can't be reached!");
		}

		//added a method to retrieve port
		public ForwardedPortLocal getPort()
		{
			return port;
		}

		//added to stop the port
		public void stopPort()
		{
			port.Stop ();
		}

		//Return the ip of the current machine
		public string getCurrentMachineIP()
		{
			string name = Dns.GetHostName ();

			string ip = Dns.GetHostEntry (name).AddressList [0].ToString ();

			return ip;
		}

		//Check if the ip is the same
		public bool equalIP(string ipAddr)
		{
			bool equal = false;
			Console.WriteLine ("Ssh machine IP: " + this.ipAddress);
			if (ipAddr.Equals (this.ipAddress)) 
			{
				equal = true;
			}

			return equal;
		}

		/*This method below is for testing purpose only
		public void executeCommand(string command)
		{
			if (connect)
			{
				using (var cmd = client.CreateCommand (command))
				{
					cmd.Execute ();
					Console.WriteLine ("Command> " + cmd.CommandText);
					Console.WriteLine ("Result:\n{0}", cmd.Result);
				}
			}
			else
				Console.WriteLine ("You have no connection!");
		}*/
	}
}
