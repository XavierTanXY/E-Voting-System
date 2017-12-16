using System;

namespace VotingMultiChain
{
	public class Person
	{
		private int id;
		private string name;
		private string username;
		private string password;

		public Person (string inUsername, string inPassword)
		{
			this.username = inUsername;
			this.password = inPassword;
		}

		public void setID(int inID)
		{
			if (inID < 0) {
				throw new System.ArgumentException ("ID can't be negative");
			} else {
				this.id = inID;
			}
		}

		public void setUsername(string inUsername)
		{
			if (inUsername.Equals (null) || inUsername.Equals ("")) {
				throw new System.ArgumentException ("User name is invalid");
			} else {
				this.username = inUsername;
			}
		}

		public void setName(string inName)
		{
			if (inName.Equals (null) || inName.Equals ("")) {
				throw new System.ArgumentException ("Name is invalid");
			} else {
				this.name = inName;
			}
		}

		public void setPassword(string inPassword)
		{
			if (inPassword.Equals (null) || inPassword.Equals ("")) {
				throw new System.ArgumentException ("Password is invalid");
			} else {
				this.password = inPassword;
			}
		}

		public int getID()
		{
			return id;
		}

		public string getUsername()
		{
			return username;
		}

		public string getName()
		{
			Console.WriteLine ("sad" + name);
			return name;
		}

		public string getPassword()
		{
			return password;
		}
	}
}

