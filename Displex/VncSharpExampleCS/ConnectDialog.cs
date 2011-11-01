using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace VncSharp
{
	/// <summary>
	/// A simple GUI Form for obtaining a VNC Host.
	/// </summary>
	public class ConnectDialog : Form
	{
		Button btnOk;
		Button btnCancel;
		TextBox txtHost;

		Container components = null;

		private ConnectDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Gets the Host entered by the user.
		/// </summary>
		public string Host {
			get {
				return txtHost.Text;
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtHost = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(144, 8);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(64, 23);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(144, 40);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(64, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			// 
			// txtHost
			// 
			this.txtHost.Location = new System.Drawing.Point(16, 16);
			this.txtHost.Name = "txtHost";
			this.txtHost.Size = new System.Drawing.Size(112, 20);
			this.txtHost.TabIndex = 0;
			this.txtHost.Text = "";
			// 
			// ConnectDialog
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(216, 73);
			this.Controls.Add(this.txtHost);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConnectDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "VNC Host";
			this.ResumeLayout(false);

		}
		#endregion
		
		/// <summary>
		/// Creates an instance of ConnectDialog and uses it to obtain the name of the VNC Host.
		/// </summary>
		/// <returns>Returns the VNC Host name entered by the user, or null if he/she clicked Cancel.</returns>
		public static string GetVncHost()
		{
			using(ConnectDialog dialog = new ConnectDialog()) {
				if (dialog.ShowDialog() == DialogResult.OK) {
					return dialog.Host;
				} else {
					// If the user clicks Cancel, return null and not the empty string.
					return null;
				}
			}
		}
	}
}