using System;

namespace VotingMultiChain
{
	public class Candidate
	{
		private int id;
		private string name;
		private string blockchainAddress;

		public Candidate (int id, string name, string blockchainAddress)
		{
			this.id = id;
			this.name = name;
			this.blockchainAddress = blockchainAddress;
		}

		public string getName()
		{
			return name;
		}

        public string getBlockchainAddress()
        {
            return blockchainAddress;
        }

	}
}

