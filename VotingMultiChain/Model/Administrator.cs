using System;

namespace VotingMultiChain
{
	public class Administrator:Person
	{
		 
		public Administrator (string username, string password):base(username, password)
		{
		}

		public void setID(int inID)
		{
			base.setID (inID);
		}

		public void setUsername(string inUsername)
		{
			base.setUsername (inUsername);
		}

		public void setName(string inName)
		{
			base.setName (inName);
		}

		public void setPassword(string inPassword)
		{
			base.setPassword (inPassword);
		}

		public int getID()
		{
			return base.getID ();
		}

		public string getUsername()
		{
			return base.getUsername ();
		}

		public string getName()
		{
			return base.getName ();
		}

		public string getPassword()
		{
			return base.getPassword();
		}
	}
}

