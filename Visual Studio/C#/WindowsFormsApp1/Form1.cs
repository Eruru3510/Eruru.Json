using System;
using System.Windows.Forms;
using Eruru.Json;

namespace WindowsFormsApp1 {

	public partial class Form1 : Form {

		public Form1 () {
			InitializeComponent ();
		}

		private void Form1_Load (object sender, EventArgs e) {
			Parse ();
		}

		private void TextBox_Input_TextChanged (object sender, EventArgs e) {
			Parse ();
		}

		private void Form1_Resize (object sender, EventArgs e) {
			TextBox_Input.Width = ClientSize.Width / 2 - 10;
			TextBox_Input.Height = ClientSize.Height - 10;
			TextBox_Output.Left = TextBox_Input.Width + 10;
			TextBox_Output.Width = TextBox_Input.Width;
			TextBox_Output.Height = TextBox_Input.Height;
		}

		void Parse () {
			try {
				TextBox_Output.Text = JsonValue.Parse (TextBox_Input.Text).Serialize (false);
			} catch (Exception exception) {
				TextBox_Output.Text = exception.ToString ();
			}
		}

	}

}