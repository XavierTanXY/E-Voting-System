using System;
using MultiChainLib;
namespace VotingMultiChain
{
    public partial class Dialog : Gtk.Dialog
    {
        public Dialog(MultiChainClient client)
        {

			this.Build();
        }
    }
}
