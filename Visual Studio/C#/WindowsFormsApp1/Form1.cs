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

		private void textBox1_TextChanged (object sender, EventArgs e) {
			Parse ();
		}

		void Parse () {
			try {
				textBox2.Text = JsonValue.Parse (textBox1.Text).Serialize (false);
			} catch (Exception exception) {
				textBox2.Text = exception.ToString ();
			}
		}

	}

}