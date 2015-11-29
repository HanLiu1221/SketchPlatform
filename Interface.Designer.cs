namespace GraphicsPlatform
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
            this.renderOption = new System.Windows.Forms.ToolStripDropDownButton();
            this.vertexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.faceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectElement = new System.Windows.Forms.ToolStripDropDownButton();
            this.vertexSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.faceSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPanel = new System.Windows.Forms.SplitContainer();
            this.fileNameTabs = new System.Windows.Forms.TabControl();
            this.tools = new System.Windows.Forms.ToolStripDropDownButton();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.display = new System.Windows.Forms.ToolStripDropDownButton();
            this.displayAxis = new System.Windows.Forms.ToolStripMenuItem();
            this.glViewer = new GraphicsPlatform.GLViewer();
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewPanel)).BeginInit();
            this.viewPanel.Panel1.SuspendLayout();
            this.viewPanel.Panel2.SuspendLayout();
            this.viewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ModelFile,
            this.tools,
            this.renderOption,
            this.selectElement,
            this.display});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(784, 39);
            this.menu.TabIndex = 0;
            this.menu.Text = "toolStrip1";
            // 
            // ModelFile
            // 
            this.ModelFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.open3D,
            this.import3D,
            this.saveAs3D});
            this.ModelFile.Image = ((System.Drawing.Image)(resources.GetObject("ModelFile.Image")));
            this.ModelFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ModelFile.Name = "ModelFile";
            this.ModelFile.Size = new System.Drawing.Size(45, 36);
            this.ModelFile.Text = "File";
            this.ModelFile.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // open3D
            // 
            this.open3D.Name = "open3D";
            this.open3D.Size = new System.Drawing.Size(150, 22);
            this.open3D.Text = "Open 3D file";
            this.open3D.Click += new System.EventHandler(this.open3D_Click);
            // 
            // import3D
            // 
            this.import3D.Name = "import3D";
            this.import3D.Size = new System.Drawing.Size(150, 22);
            this.import3D.Text = "Import 3D file";
            // 
            // saveAs3D
            // 
            this.saveAs3D.Name = "saveAs3D";
            this.saveAs3D.Size = new System.Drawing.Size(150, 22);
            this.saveAs3D.Text = "Save As 3D file";
            // 
            // renderOption
            // 
            this.renderOption.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vertexToolStripMenuItem,
            this.wireFrameToolStripMenuItem,
            this.faceToolStripMenuItem});
            this.renderOption.Image = ((System.Drawing.Image)(resources.GetObject("renderOption.Image")));
            this.renderOption.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.renderOption.Name = "renderOption";
            this.renderOption.Size = new System.Drawing.Size(57, 36);
            this.renderOption.Text = "Render";
            this.renderOption.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // vertexToolStripMenuItem
            // 
            this.vertexToolStripMenuItem.Name = "vertexToolStripMenuItem";
            this.vertexToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.vertexToolStripMenuItem.Text = "Vertex";
            this.vertexToolStripMenuItem.Click += new System.EventHandler(this.pointToolStripMenuItem_Click);
            // 
            // wireFrameToolStripMenuItem
            // 
            this.wireFrameToolStripMenuItem.Name = "wireFrameToolStripMenuItem";
            this.wireFrameToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.wireFrameToolStripMenuItem.Text = "WireFrame";
            this.wireFrameToolStripMenuItem.Click += new System.EventHandler(this.wireFrameToolStripMenuItem_Click);
            // 
            // faceToolStripMenuItem
            // 
            this.faceToolStripMenuItem.Checked = true;
            this.faceToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.faceToolStripMenuItem.Name = "faceToolStripMenuItem";
            this.faceToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.faceToolStripMenuItem.Text = "Face";
            this.faceToolStripMenuItem.Click += new System.EventHandler(this.faceToolStripMenuItem_Click);
            // 
            // selectElement
            // 
            this.selectElement.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vertexSelection,
            this.edgeSelection,
            this.faceSelection});
            this.selectElement.Image = ((System.Drawing.Image)(resources.GetObject("selectElement.Image")));
            this.selectElement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.selectElement.Name = "selectElement";
            this.selectElement.Size = new System.Drawing.Size(51, 36);
            this.selectElement.Text = "Select";
            this.selectElement.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // vertexSelection
            // 
            this.vertexSelection.Name = "vertexSelection";
            this.vertexSelection.Size = new System.Drawing.Size(105, 22);
            this.vertexSelection.Text = "vertex";
            // 
            // edgeSelection
            // 
            this.edgeSelection.Name = "edgeSelection";
            this.edgeSelection.Size = new System.Drawing.Size(105, 22);
            this.edgeSelection.Text = "edge";
            // 
            // faceSelection
            // 
            this.faceSelection.Name = "faceSelection";
            this.faceSelection.Size = new System.Drawing.Size(105, 22);
            this.faceSelection.Text = "face";
            // 
            // viewPanel
            // 
            this.viewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewPanel.Location = new System.Drawing.Point(0, 39);
            this.viewPanel.Name = "viewPanel";
            this.viewPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // viewPanel.Panel1
            // 
            this.viewPanel.Panel1.Controls.Add(this.fileNameTabs);
            // 
            // viewPanel.Panel2
            // 
            this.viewPanel.Panel2.Controls.Add(this.glViewer);
            this.viewPanel.Size = new System.Drawing.Size(784, 523);
            this.viewPanel.SplitterDistance = 34;
            this.viewPanel.TabIndex = 1;
            // 
            // fileNameTabs
            // 
            this.fileNameTabs.Location = new System.Drawing.Point(0, 0);
            this.fileNameTabs.Name = "fileNameTabs";
            this.fileNameTabs.SelectedIndex = 0;
            this.fileNameTabs.Size = new System.Drawing.Size(783, 31);
            this.fileNameTabs.TabIndex = 0;
            // 
            // tools
            // 
            this.tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.resetViewToolStripMenuItem});
            this.tools.Image = ((System.Drawing.Image)(resources.GetObject("tools.Image")));
            this.tools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(49, 36);
            this.tools.Text = "Tools";
            this.tools.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // resetViewToolStripMenuItem
            // 
            this.resetViewToolStripMenuItem.Name = "resetViewToolStripMenuItem";
            this.resetViewToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.resetViewToolStripMenuItem.Text = "Reset View";
            this.resetViewToolStripMenuItem.Click += new System.EventHandler(this.resetViewToolStripMenuItem_Click);
            // 
            // display
            // 
            this.display.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayAxis});
            this.display.Image = ((System.Drawing.Image)(resources.GetObject("display.Image")));
            this.display.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.display.Name = "display";
            this.display.Size = new System.Drawing.Size(58, 36);
            this.display.Text = "Display";
            this.display.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // displayAxis
            // 
            this.displayAxis.Name = "displayAxis";
            this.displayAxis.Size = new System.Drawing.Size(152, 22);
            this.displayAxis.Text = "Axis";
            this.displayAxis.Click += new System.EventHandler(this.displayAxis_Click);
            // 
            // glViewer
            // 
            this.glViewer.AccumBits = ((byte)(0));
            this.glViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glViewer.AutoCheckErrors = false;
            this.glViewer.AutoFinish = false;
            this.glViewer.AutoMakeCurrent = true;
            this.glViewer.AutoSwapBuffers = true;
            this.glViewer.BackColor = System.Drawing.Color.Black;
            this.glViewer.ColorBits = ((byte)(32));
            this.glViewer.CurrentUIMode = GraphicsPlatform.GLViewer.UIMode.Viewing;
            this.glViewer.DepthBits = ((byte)(16));
            this.glViewer.Location = new System.Drawing.Point(3, 3);
            this.glViewer.Name = "glViewer";
            this.glViewer.Size = new System.Drawing.Size(778, 479);
            this.glViewer.StencilBits = ((byte)(0));
            this.glViewer.TabIndex = 0;
            // 
            // Interface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.viewPanel);
            this.Controls.Add(this.menu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Interface";
            this.Text = "CGPlatform";
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.viewPanel.Panel1.ResumeLayout(false);
            this.viewPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.viewPanel)).EndInit();
            this.viewPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion


		private System.Windows.Forms.ToolStrip menu;
		private System.Windows.Forms.ToolStripDropDownButton ModelFile;
		private System.Windows.Forms.ToolStripMenuItem open3D;
		private System.Windows.Forms.ToolStripMenuItem import3D;
        private System.Windows.Forms.ToolStripMenuItem saveAs3D;
        private System.Windows.Forms.ToolStripDropDownButton selectElement;
        private System.Windows.Forms.ToolStripMenuItem vertexSelection;
        private System.Windows.Forms.ToolStripMenuItem edgeSelection;
        private System.Windows.Forms.ToolStripMenuItem faceSelection;
        private System.Windows.Forms.SplitContainer viewPanel;
        private System.Windows.Forms.TabControl fileNameTabs;
        private GLViewer glViewer;
        private System.Windows.Forms.ToolStripDropDownButton renderOption;
        private System.Windows.Forms.ToolStripMenuItem vertexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wireFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem faceToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton tools;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton display;
        private System.Windows.Forms.ToolStripMenuItem displayAxis;

	}
}

