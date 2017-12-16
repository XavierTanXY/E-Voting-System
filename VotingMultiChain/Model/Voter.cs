using System;

namespace VotingMultiChain
{
	public class Voter: Person
	{
		private string votingAddress;
		private bool vote;
		private int loginAttempt;
		
		public Voter (string username, string password):base(username, password)
		{
			
		}

		public void setVotingAddress(string inAddress)
		{
			votingAddress = inAddress;
		}

		public void setVote(bool inVoted)
		{	
			vote = inVoted;
		}

		public void setLoginAttempt(int inAttempt)
		{
			if (inAttempt < 0) {
				throw new System.ArgumentException ("User name is invalid");
			} else {
				loginAttempt = inAttempt;
			}
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

		public string getVotingAddress()
		{
			return votingAddress;
		}

		public bool getVoted()
		{
			return vote;
		}

		public int getLoginAttempt()
		{
			return loginAttempt;
		}
	}
}

