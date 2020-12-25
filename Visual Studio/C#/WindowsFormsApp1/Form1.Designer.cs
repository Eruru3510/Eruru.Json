namespace WindowsFormsApp1 {
	partial class Form1 {
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose (bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent () {
			this.TextBox_Input = new System.Windows.Forms.TextBox();
			this.TextBox_Output = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// TextBox_Input
			// 
			this.TextBox_Input.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextBox_Input.Font = new System.Drawing.Font("Consolas", 11F);
			this.TextBox_Input.Location = new System.Drawing.Point(5, 5);
			this.TextBox_Input.MaxLength = 2147483647;
			this.TextBox_Input.Multiline = true;
			this.TextBox_Input.Name = "TextBox_Input";
			this.TextBox_Input.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TextBox_Input.Size = new System.Drawing.Size(620, 640);
			this.TextBox_Input.TabIndex = 0;
			this.TextBox_Input.WordWrap = false;
			this.TextBox_Input.TextChanged += new System.EventHandler(this.TextBox_Input_TextChanged);
			// 
			// TextBox_Output
			// 
			this.TextBox_Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextBox_Output.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.TextBox_Output.Location = new System.Drawing.Point(630, 5);
			this.TextBox_Output.MaxLength = 2147483647;
			this.TextBox_Output.Multiline = true;
			this.TextBox_Output.Name = "TextBox_Output";
			this.TextBox_Output.ReadOnly = true;
			this.TextBox_Output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TextBox_Output.Size = new System.Drawing.Size(620, 640);
			this.TextBox_Output.TabIndex = 1;
			this.TextBox_Output.WordWrap = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1254, 651);
			this.Controls.Add(this.TextBox_Output);
			this.Controls.Add(this.TextBox_Input);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Resize += new System.EventHandler(this.Form1_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox TextBox_Input;
		private System.Windows.Forms.TextBox TextBox_Output;
	}
}

