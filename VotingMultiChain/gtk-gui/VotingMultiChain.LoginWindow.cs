
// This file has been generated by the GUI designer. Do not modify.
namespace VotingMultiChain
{
	public partial class LoginWindow
	{
		private global::Gtk.VBox vbox1;
		
		private global::Gtk.Label titleLabel;
		
		private global::Gtk.VBox vbox2;
		
		private global::Gtk.HPaned hpaned1;
		
		private global::Gtk.HBox hbox3;
		
		private global::Gtk.Label ComboxLabel;
		
		private global::Gtk.ComboBox userSelectionCombo;
		
		private global::Gtk.VBox vbox3;
		
		private global::Gtk.HBox hbox6;
		
		private global::Gtk.Label userLabel;
		
		private global::Gtk.Entry userEntry;
		
		private global::Gtk.HBox hbox5;
		
		private global::Gtk.Label passwordLabel;
		
		private global::Gtk.Entry passwordEntry;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Button loginButton;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget VotingMultiChain.LoginWindow
			this.Name = "VotingMultiChain.LoginWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Voting Log In");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			this.BorderWidth = ((uint)(10));
			this.Resizable = false;
			this.DefaultWidth = 550;
			this.DefaultHeight = 550;
			// Container child VotingMultiChain.LoginWindow.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.titleLabel = new global::Gtk.Label ();
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Enter login details");
			this.vbox1.Add (this.titleLabel);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.titleLabel]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hpaned1 = new global::Gtk.HPaned ();
			this.hpaned1.CanFocus = true;
			this.hpaned1.Name = "hpaned1";
			this.hpaned1.Position = 10;
			this.vbox2.Add (this.hpaned1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hpaned1]));
			w2.Position = 0;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.ComboxLabel = new global::Gtk.Label ();
			this.ComboxLabel.Name = "ComboxLabel";
			this.ComboxLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("               User Type");
			this.hbox3.Add (this.ComboxLabel);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.ComboxLabel]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.userSelectionCombo = global::Gtk.ComboBox.NewText ();
			this.userSelectionCombo.AppendText (global::Mono.Unix.Catalog.GetString ("Voter"));
			this.userSelectionCombo.AppendText (global::Mono.Unix.Catalog.GetString ("Administrator"));
			this.userSelectionCombo.Name = "userSelectionCombo";
			this.userSelectionCombo.Active = 0;
			this.hbox3.Add (this.userSelectionCombo);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.userSelectionCombo]));
			w4.Position = 1;
			w4.Padding = ((uint)(30));
			this.vbox2.Add (this.hbox3);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox3]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			this.vbox1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.vbox2]));
			w6.Position = 1;
			// Container child vbox1.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox6 = new global::Gtk.HBox ();
			this.hbox6.Name = "hbox6";
			this.hbox6.Spacing = 6;
			// Container child hbox6.Gtk.Box+BoxChild
			this.userLabel = new global::Gtk.Label ();
			this.userLabel.Name = "userLabel";
			this.userLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("             User Name");
			this.hbox6.Add (this.userLabel);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.userLabel]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child hbox6.Gtk.Box+BoxChild
			this.userEntry = new global::Gtk.Entry ();
			this.userEntry.CanFocus = true;
			this.userEntry.Name = "userEntry";
			this.userEntry.IsEditable = true;
			this.userEntry.InvisibleChar = '•';
			this.hbox6.Add (this.userEntry);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox6 [this.userEntry]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Padding = ((uint)(25));
			this.vbox3.Add (this.hbox6);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox6]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox5 = new global::Gtk.HBox ();
			this.hbox5.Name = "hbox5";
			this.hbox5.Spacing = 6;
			// Container child hbox5.Gtk.Box+BoxChild
			this.passwordLabel = new global::Gtk.Label ();
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("               Password");
			this.hbox5.Add (this.passwordLabel);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.passwordLabel]));
			w10.Position = 0;
			w10.Expand = false;
			w10.Fill = false;
			// Container child hbox5.Gtk.Box+BoxChild
			this.passwordEntry = new global::Gtk.Entry ();
			this.passwordEntry.CanFocus = true;
			this.passwordEntry.Name = "passwordEntry";
			this.passwordEntry.IsEditable = true;
			this.passwordEntry.Visibility = false;
			this.passwordEntry.InvisibleChar = '•';
			this.hbox5.Add (this.passwordEntry);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.passwordEntry]));
			w11.Position = 1;
			w11.Fill = false;
			w11.Padding = ((uint)(25));
			this.vbox3.Add (this.hbox5);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox5]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			this.vbox3.Add (this.hbox1);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox1]));
			w13.Position = 2;
			this.vbox1.Add (this.vbox3);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.vbox3]));
			w14.Position = 2;
			// Container child vbox1.Gtk.Box+BoxChild
			this.loginButton = new global::Gtk.Button ();
			this.loginButton.CanFocus = true;
			this.loginButton.Name = "loginButton";
			this.loginButton.UseUnderline = true;
			this.loginButton.BorderWidth = ((uint)(2));
			this.loginButton.Label = global::Mono.Unix.Catalog.GetString ("Log In");
			this.vbox1.Add (this.loginButton);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.loginButton]));
			w15.Position = 3;
			w15.Expand = false;
			w15.Fill = false;
			w15.Padding = ((uint)(10));
			this.Add (this.vbox1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Show ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.loginButton.Clicked += new global::System.EventHandler (this.OnLoginButtonClicked);
		}
	}
}