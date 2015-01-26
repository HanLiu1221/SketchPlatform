namespace MyPlatform
{
	partial class Interface
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Interface));
			this.menu = new System.Windows.Forms.ToolStrip();
			this.ModelFile = new System.Windows.Forms.ToolStripDropDownButton();
			this.open3D = new System.Windows.Forms.ToolStripMenuItem();
			this.import3D = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAs3D = new System.Windows.Forms.ToolStripMenuItem();
			this.ImageFile = new System.Windows.Forms.ToolStripDropDownButton();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menu.SuspendLayout();
			this.SuspendLayout();
			// 
			// menu
			// 
			this.menu.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ModelFile,
            this.ImageFile});
			this.menu.Location = new System.Drawing.Point(0, 0);
			this.menu.Name = "menu";
			this.menu.Size = new System.Drawing.Size(784, 39);
			this.menu.TabIndex = 0;
			this.menu.Text = "toolStrip1";
			// 
			// ModelFile
			// 
			this.ModelFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ModelFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.open3D,
            this.import3D,
            this.saveAs3D});
			this.ModelFile.Image = ((System.Drawing.Image)(resources.GetObject("ModelFile.Image")));
			this.ModelFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ModelFile.Name = "ModelFile";
			this.ModelFile.Size = new System.Drawing.Size(45, 36);
			this.ModelFile.Text = "toolStripDropDownButton1";
			// 
			// open3D
			// 
			this.open3D.Name = "open3D";
			this.open3D.Size = new System.Drawing.Size(152, 22);
			this.open3D.Text = "Open";
			// 
			// import3D
			// 
			this.import3D.Name = "import3D";
			this.import3D.Size = new System.Drawing.Size(152, 22);
			this.import3D.Text = "Import";
			// 
			// saveAs3D
			// 
			this.saveAs3D.Name = "saveAs3D";
			this.saveAs3D.Size = new System.Drawing.Size(152, 22);
			this.saveAs3D.Text = "Save As";
			// 
			// ImageFile
			// 
			this.ImageFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ImageFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
			this.ImageFile.Image = ((System.Drawing.Image)(resources.GetObject("ImageFile.Image")));
			this.ImageFile.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ImageFile.Name = "ImageFile";
			this.ImageFile.Size = new System.Drawing.Size(45, 36);
			this.ImageFile.Text = "toolStripDropDownButton1";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.openToolStripMenuItem.Text = "Open";
			// 
			// Interface
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.menu);
			this.Name = "Interface";
			this.Text = "MyPlatform";
			this.menu.ResumeLayout(false);
			this.menu.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip menu;
		private System.Windows.Forms.ToolStripDropDownButton ModelFile;
		private System.Windows.Forms.ToolStripMenuItem open3D;
		private System.Windows.Forms.ToolStripMenuItem import3D;
		private System.Windows.Forms.ToolStripMenuItem saveAs3D;
		private System.Windows.Forms.ToolStripDropDownButton ImageFile;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;

	}
}

