namespace SketchPlatform
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
            this.loadSegmentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadJSONFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tools = new System.Windows.Forms.ToolStripDropDownButton();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderOption = new System.Windows.Forms.ToolStripDropDownButton();
            this.vertexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireFrameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.faceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectElement = new System.Windows.Forms.ToolStripDropDownButton();
            this.vertexSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.faceSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.display = new System.Windows.Forms.ToolStripDropDownButton();
            this.displayAxis = new System.Windows.Forms.ToolStripMenuItem();
            this.strokeStyle = new System.Windows.Forms.ToolStripDropDownButton();
            this.pencilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pen1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pen2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crayonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ink1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ink2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPanel = new System.Windows.Forms.SplitContainer();
            this.fileNameTabs = new System.Windows.Forms.TabControl();
            this.statistics = new System.Windows.Forms.Label();
            this.toolboxPanel = new System.Windows.Forms.Panel();
            this.guideLineBox = new System.Windows.Forms.GroupBox();
            this.nextBoxButton = new System.Windows.Forms.Button();
            this.prevBoxButton = new System.Windows.Forms.Button();
            this.boxesLabel = new System.Windows.Forms.Label();
            this.adjustBox = new System.Windows.Forms.GroupBox();
            this.guidelineType = new System.Windows.Forms.ComboBox();
            this.guidelineLabel = new System.Windows.Forms.Label();
            this.strokeSizeLabel = new System.Windows.Forms.Label();
            this.strokeSizeBox = new System.Windows.Forms.ComboBox();
            this.displayBox = new System.Windows.Forms.GroupBox();
            this.showGuideLines = new System.Windows.Forms.CheckBox();
            this.showSketchyEdges = new System.Windows.Forms.CheckBox();
            this.showMesh = new System.Windows.Forms.CheckBox();
            this.showBBox = new System.Windows.Forms.CheckBox();
            this.glViewer = new SketchPlatform.GLViewer();
            this.strokeColorDialog = new System.Windows.Forms.ColorDialog();
            this.guideLineColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewPanel)).BeginInit();
            this.viewPanel.Panel1.SuspendLayout();
            this.viewPanel.Panel2.SuspendLayout();
            this.viewPanel.SuspendLayout();
            this.toolboxPanel.SuspendLayout();
            this.guideLineBox.SuspendLayout();
            this.adjustBox.SuspendLayout();
            this.displayBox.SuspendLayout();
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
            this.display,
            this.strokeStyle});
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
            this.saveAs3D,
            this.loadSegmentsToolStripMenuItem,
            this.loadJSONFileToolStripMenuItem});
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
            this.open3D.Size = new System.Drawing.Size(155, 22);
            this.open3D.Text = "Open 3D file";
            this.open3D.Click += new System.EventHandler(this.open3D_Click);
            // 
            // import3D
            // 
            this.import3D.Name = "import3D";
            this.import3D.Size = new System.Drawing.Size(155, 22);
            this.import3D.Text = "Import 3D file";
            // 
            // saveAs3D
            // 
            this.saveAs3D.Name = "saveAs3D";
            this.saveAs3D.Size = new System.Drawing.Size(155, 22);
            this.saveAs3D.Text = "Save As 3D file";
            // 
            // loadSegmentsToolStripMenuItem
            // 
            this.loadSegmentsToolStripMenuItem.Name = "loadSegmentsToolStripMenuItem";
            this.loadSegmentsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.loadSegmentsToolStripMenuItem.Text = "Load Segments";
            this.loadSegmentsToolStripMenuItem.Click += new System.EventHandler(this.loadSegmentsToolStripMenuItem_Click);
            // 
            // loadJSONFileToolStripMenuItem
            // 
            this.loadJSONFileToolStripMenuItem.Name = "loadJSONFileToolStripMenuItem";
            this.loadJSONFileToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.loadJSONFileToolStripMenuItem.Text = "Load JSON file";
            this.loadJSONFileToolStripMenuItem.Click += new System.EventHandler(this.loadJSONFileToolStripMenuItem_Click);
            // 
            // tools
            // 
            this.tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.resetViewToolStripMenuItem,
            this.modelColorToolStripMenuItem});
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
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // resetViewToolStripMenuItem
            // 
            this.resetViewToolStripMenuItem.Name = "resetViewToolStripMenuItem";
            this.resetViewToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.resetViewToolStripMenuItem.Text = "Reset View";
            this.resetViewToolStripMenuItem.Click += new System.EventHandler(this.resetViewToolStripMenuItem_Click);
            // 
            // modelColorToolStripMenuItem
            // 
            this.modelColorToolStripMenuItem.Name = "modelColorToolStripMenuItem";
            this.modelColorToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.modelColorToolStripMenuItem.Text = "Model Color";
            this.modelColorToolStripMenuItem.Click += new System.EventHandler(this.modelColorToolStripMenuItem_Click);
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
            this.vertexSelection.Click += new System.EventHandler(this.vertexSelection_Click);
            // 
            // edgeSelection
            // 
            this.edgeSelection.Name = "edgeSelection";
            this.edgeSelection.Size = new System.Drawing.Size(105, 22);
            this.edgeSelection.Text = "edge";
            this.edgeSelection.Click += new System.EventHandler(this.edgeSelection_Click);
            // 
            // faceSelection
            // 
            this.faceSelection.Name = "faceSelection";
            this.faceSelection.Size = new System.Drawing.Size(105, 22);
            this.faceSelection.Text = "face";
            this.faceSelection.Click += new System.EventHandler(this.faceSelection_Click);
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
            this.displayAxis.Size = new System.Drawing.Size(95, 22);
            this.displayAxis.Text = "Axis";
            this.displayAxis.Click += new System.EventHandler(this.displayAxis_Click);
            // 
            // strokeStyle
            // 
            this.strokeStyle.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pencilToolStripMenuItem,
            this.pen1ToolStripMenuItem,
            this.pen2ToolStripMenuItem,
            this.crayonToolStripMenuItem,
            this.ink1ToolStripMenuItem,
            this.ink2ToolStripMenuItem,
            this.edgeColorToolStripMenuItem,
            this.guideLineColorToolStripMenuItem});
            this.strokeStyle.Image = ((System.Drawing.Image)(resources.GetObject("strokeStyle.Image")));
            this.strokeStyle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.strokeStyle.Name = "strokeStyle";
            this.strokeStyle.Size = new System.Drawing.Size(53, 36);
            this.strokeStyle.Text = "Stroke";
            this.strokeStyle.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // pencilToolStripMenuItem
            // 
            this.pencilToolStripMenuItem.Name = "pencilToolStripMenuItem";
            this.pencilToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.pencilToolStripMenuItem.Text = "pencil";
            this.pencilToolStripMenuItem.Click += new System.EventHandler(this.pencilToolStripMenuItem_Click);
            // 
            // pen1ToolStripMenuItem
            // 
            this.pen1ToolStripMenuItem.Name = "pen1ToolStripMenuItem";
            this.pen1ToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.pen1ToolStripMenuItem.Text = "pen-1";
            this.pen1ToolStripMenuItem.Click += new System.EventHandler(this.pen1ToolStripMenuItem_Click);
            // 
            // pen2ToolStripMenuItem
            // 
            this.pen2ToolStripMenuItem.Name = "pen2ToolStripMenuItem";
            this.pen2ToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.pen2ToolStripMenuItem.Text = "pen-2";
            this.pen2ToolStripMenuItem.Click += new System.EventHandler(this.pen2ToolStripMenuItem_Click);
            // 
            // crayonToolStripMenuItem
            // 
            this.crayonToolStripMenuItem.Name = "crayonToolStripMenuItem";
            this.crayonToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.crayonToolStripMenuItem.Text = "crayon";
            this.crayonToolStripMenuItem.Click += new System.EventHandler(this.crayonToolStripMenuItem_Click);
            // 
            // ink1ToolStripMenuItem
            // 
            this.ink1ToolStripMenuItem.Name = "ink1ToolStripMenuItem";
            this.ink1ToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.ink1ToolStripMenuItem.Text = "ink-1";
            this.ink1ToolStripMenuItem.Click += new System.EventHandler(this.ink1ToolStripMenuItem_Click);
            // 
            // ink2ToolStripMenuItem
            // 
            this.ink2ToolStripMenuItem.Name = "ink2ToolStripMenuItem";
            this.ink2ToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.ink2ToolStripMenuItem.Text = "ink-2";
            this.ink2ToolStripMenuItem.Click += new System.EventHandler(this.ink2ToolStripMenuItem_Click);
            // 
            // edgeColorToolStripMenuItem
            // 
            this.edgeColorToolStripMenuItem.Name = "edgeColorToolStripMenuItem";
            this.edgeColorToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.edgeColorToolStripMenuItem.Text = "edge color";
            this.edgeColorToolStripMenuItem.Click += new System.EventHandler(this.edgeColorToolStripMenuItem_Click);
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
            this.viewPanel.Panel2.Controls.Add(this.statistics);
            this.viewPanel.Panel2.Controls.Add(this.toolboxPanel);
            this.viewPanel.Panel2.Controls.Add(this.glViewer);
            this.viewPanel.Size = new System.Drawing.Size(784, 523);
            this.viewPanel.SplitterDistance = 29;
            this.viewPanel.TabIndex = 1;
            // 
            // fileNameTabs
            // 
            this.fileNameTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameTabs.Location = new System.Drawing.Point(3, 0);
            this.fileNameTabs.Name = "fileNameTabs";
            this.fileNameTabs.SelectedIndex = 0;
            this.fileNameTabs.Size = new System.Drawing.Size(778, 35);
            this.fileNameTabs.TabIndex = 0;
            // 
            // statistics
            // 
            this.statistics.AutoSize = true;
            this.statistics.Location = new System.Drawing.Point(180, 8);
            this.statistics.Name = "statistics";
            this.statistics.Size = new System.Drawing.Size(0, 13);
            this.statistics.TabIndex = 1;
            // 
            // toolboxPanel
            // 
            this.toolboxPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.toolboxPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolboxPanel.Controls.Add(this.guideLineBox);
            this.toolboxPanel.Controls.Add(this.adjustBox);
            this.toolboxPanel.Controls.Add(this.displayBox);
            this.toolboxPanel.Location = new System.Drawing.Point(3, 3);
            this.toolboxPanel.Name = "toolboxPanel";
            this.toolboxPanel.Size = new System.Drawing.Size(165, 484);
            this.toolboxPanel.TabIndex = 2;
            // 
            // guideLineBox
            // 
            this.guideLineBox.Controls.Add(this.nextBoxButton);
            this.guideLineBox.Controls.Add(this.prevBoxButton);
            this.guideLineBox.Controls.Add(this.boxesLabel);
            this.guideLineBox.Location = new System.Drawing.Point(3, 223);
            this.guideLineBox.Name = "guideLineBox";
            this.guideLineBox.Size = new System.Drawing.Size(156, 91);
            this.guideLineBox.TabIndex = 4;
            this.guideLineBox.TabStop = false;
            this.guideLineBox.Text = "Guides";
            // 
            // nextBoxButton
            // 
            this.nextBoxButton.Location = new System.Drawing.Point(81, 25);
            this.nextBoxButton.Name = "nextBoxButton";
            this.nextBoxButton.Size = new System.Drawing.Size(39, 24);
            this.nextBoxButton.TabIndex = 2;
            this.nextBoxButton.Text = "next";
            this.nextBoxButton.UseVisualStyleBackColor = true;
            this.nextBoxButton.Click += new System.EventHandler(this.nextBoxButton_Click);
            // 
            // prevBoxButton
            // 
            this.prevBoxButton.Location = new System.Drawing.Point(36, 25);
            this.prevBoxButton.Name = "prevBoxButton";
            this.prevBoxButton.Size = new System.Drawing.Size(39, 24);
            this.prevBoxButton.TabIndex = 1;
            this.prevBoxButton.Text = "prev";
            this.prevBoxButton.UseVisualStyleBackColor = true;
            this.prevBoxButton.Click += new System.EventHandler(this.prevBoxButton_Click);
            // 
            // boxesLabel
            // 
            this.boxesLabel.AutoSize = true;
            this.boxesLabel.Location = new System.Drawing.Point(3, 30);
            this.boxesLabel.Name = "boxesLabel";
            this.boxesLabel.Size = new System.Drawing.Size(27, 13);
            this.boxesLabel.TabIndex = 0;
            this.boxesLabel.Text = "box:";
            // 
            // adjustBox
            // 
            this.adjustBox.Controls.Add(this.guidelineType);
            this.adjustBox.Controls.Add(this.guidelineLabel);
            this.adjustBox.Controls.Add(this.strokeSizeLabel);
            this.adjustBox.Controls.Add(this.strokeSizeBox);
            this.adjustBox.Location = new System.Drawing.Point(3, 123);
            this.adjustBox.Name = "adjustBox";
            this.adjustBox.Size = new System.Drawing.Size(156, 91);
            this.adjustBox.TabIndex = 3;
            this.adjustBox.TabStop = false;
            this.adjustBox.Text = "Adjust";
            // 
            // guidelineType
            // 
            this.guidelineType.FormattingEnabled = true;
            this.guidelineType.Items.AddRange(new object[] {
            "cross",
            "random"});
            this.guidelineType.Location = new System.Drawing.Point(66, 55);
            this.guidelineType.Name = "guidelineType";
            this.guidelineType.Size = new System.Drawing.Size(66, 21);
            this.guidelineType.TabIndex = 3;
            this.guidelineType.Text = "cross";
            this.guidelineType.SelectedIndexChanged += new System.EventHandler(this.guidelineType_SelectedIndexChanged);
            // 
            // guidelineLabel
            // 
            this.guidelineLabel.AutoSize = true;
            this.guidelineLabel.Location = new System.Drawing.Point(3, 55);
            this.guidelineLabel.Name = "guidelineLabel";
            this.guidelineLabel.Size = new System.Drawing.Size(52, 13);
            this.guidelineLabel.TabIndex = 2;
            this.guidelineLabel.Text = "guide line";
            this.guidelineLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // strokeSizeLabel
            // 
            this.strokeSizeLabel.AutoSize = true;
            this.strokeSizeLabel.Location = new System.Drawing.Point(3, 27);
            this.strokeSizeLabel.Name = "strokeSizeLabel";
            this.strokeSizeLabel.Size = new System.Drawing.Size(60, 13);
            this.strokeSizeLabel.TabIndex = 1;
            this.strokeSizeLabel.Text = "stroke size:";
            this.strokeSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // strokeSizeBox
            // 
            this.strokeSizeBox.AllowDrop = true;
            this.strokeSizeBox.FormattingEnabled = true;
            this.strokeSizeBox.Items.AddRange(new object[] {
            "2",
            "4",
            "6",
            "8",
            "10"});
            this.strokeSizeBox.Location = new System.Drawing.Point(66, 24);
            this.strokeSizeBox.Name = "strokeSizeBox";
            this.strokeSizeBox.Size = new System.Drawing.Size(48, 21);
            this.strokeSizeBox.TabIndex = 0;
            this.strokeSizeBox.Text = "2";
            //this.strokeSizeBox.SelectedIndexChanged += new System.EventHandler(this.strokeSizeBox_SelectedIndexChanged);
            this.strokeSizeBox.TextChanged += new System.EventHandler(this.strokeSizeBox_TextChanged);
            // 
            // displayBox
            // 
            this.displayBox.Controls.Add(this.showGuideLines);
            this.displayBox.Controls.Add(this.showSketchyEdges);
            this.displayBox.Controls.Add(this.showMesh);
            this.displayBox.Controls.Add(this.showBBox);
            this.displayBox.Location = new System.Drawing.Point(3, 4);
            this.displayBox.Name = "displayBox";
            this.displayBox.Size = new System.Drawing.Size(157, 112);
            this.displayBox.TabIndex = 2;
            this.displayBox.TabStop = false;
            this.displayBox.Text = "Display";
            // 
            // showGuideLines
            // 
            this.showGuideLines.AutoSize = true;
            this.showGuideLines.Checked = true;
            this.showGuideLines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGuideLines.Location = new System.Drawing.Point(6, 88);
            this.showGuideLines.Name = "showGuideLines";
            this.showGuideLines.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showGuideLines.Size = new System.Drawing.Size(76, 17);
            this.showGuideLines.TabIndex = 4;
            this.showGuideLines.Text = "guide lines";
            this.showGuideLines.UseVisualStyleBackColor = true;
            this.showGuideLines.CheckedChanged += new System.EventHandler(this.showGuideLines_CheckedChanged);
            // 
            // showSketchyEdges
            // 
            this.showSketchyEdges.AutoSize = true;
            this.showSketchyEdges.Checked = true;
            this.showSketchyEdges.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showSketchyEdges.Location = new System.Drawing.Point(6, 65);
            this.showSketchyEdges.Name = "showSketchyEdges";
            this.showSketchyEdges.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showSketchyEdges.Size = new System.Drawing.Size(95, 17);
            this.showSketchyEdges.TabIndex = 3;
            this.showSketchyEdges.Text = "sketchy edges";
            this.showSketchyEdges.UseVisualStyleBackColor = true;
            this.showSketchyEdges.CheckedChanged += new System.EventHandler(this.showSketchyEdges_CheckedChanged);
            // 
            // showMesh
            // 
            this.showMesh.AutoSize = true;
            this.showMesh.Location = new System.Drawing.Point(6, 42);
            this.showMesh.Name = "showMesh";
            this.showMesh.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showMesh.Size = new System.Drawing.Size(51, 17);
            this.showMesh.TabIndex = 2;
            this.showMesh.Text = "mesh";
            this.showMesh.UseVisualStyleBackColor = true;
            this.showMesh.CheckedChanged += new System.EventHandler(this.showMesh_CheckedChanged);
            // 
            // showBBox
            // 
            this.showBBox.AutoSize = true;
            this.showBBox.Checked = true;
            this.showBBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showBBox.Location = new System.Drawing.Point(6, 19);
            this.showBBox.Name = "showBBox";
            this.showBBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showBBox.Size = new System.Drawing.Size(90, 17);
            this.showBBox.TabIndex = 1;
            this.showBBox.Text = "bounding box";
            this.showBBox.UseVisualStyleBackColor = true;
            this.showBBox.CheckedChanged += new System.EventHandler(this.showBBox_CheckedChanged);
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
            this.glViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.glViewer.ColorBits = ((byte)(32));
            this.glViewer.CurrentUIMode = SketchPlatform.GLViewer.UIMode.Viewing;
            this.glViewer.DepthBits = ((byte)(16));
            this.glViewer.Location = new System.Drawing.Point(174, 3);
            this.glViewer.Name = "glViewer";
            this.glViewer.Size = new System.Drawing.Size(607, 484);
            this.glViewer.StencilBits = ((byte)(0));
            this.glViewer.TabIndex = 0;
            // 
            // guideLineColorToolStripMenuItem
            // 
            this.guideLineColorToolStripMenuItem.Name = "guideLineColorToolStripMenuItem";
            this.guideLineColorToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.guideLineColorToolStripMenuItem.Text = "guide line color";
            this.guideLineColorToolStripMenuItem.Click += new System.EventHandler(this.guideLineColorToolStripMenuItem_Click);
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
            this.Text = "Sketching";
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.viewPanel.Panel1.ResumeLayout(false);
            this.viewPanel.Panel2.ResumeLayout(false);
            this.viewPanel.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewPanel)).EndInit();
            this.viewPanel.ResumeLayout(false);
            this.toolboxPanel.ResumeLayout(false);
            this.guideLineBox.ResumeLayout(false);
            this.guideLineBox.PerformLayout();
            this.adjustBox.ResumeLayout(false);
            this.adjustBox.PerformLayout();
            this.displayBox.ResumeLayout(false);
            this.displayBox.PerformLayout();
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
        private System.Windows.Forms.ToolStripDropDownButton renderOption;
        private System.Windows.Forms.ToolStripMenuItem vertexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wireFrameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem faceToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton tools;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton display;
        private System.Windows.Forms.ToolStripMenuItem displayAxis;
        private System.Windows.Forms.ToolStripMenuItem modelColorToolStripMenuItem;
        private System.Windows.Forms.Label statistics;
        private System.Windows.Forms.ToolStripMenuItem loadSegmentsToolStripMenuItem;
        private System.Windows.Forms.Panel toolboxPanel;
        private GLViewer glViewer;
        private System.Windows.Forms.CheckBox showBBox;
        private System.Windows.Forms.GroupBox displayBox;
        private System.Windows.Forms.CheckBox showMesh;
        private System.Windows.Forms.ToolStripDropDownButton strokeStyle;
        private System.Windows.Forms.ToolStripMenuItem pencilToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pen1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pen2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem crayonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ink1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ink2ToolStripMenuItem;
        private System.Windows.Forms.CheckBox showSketchyEdges;
        private System.Windows.Forms.GroupBox adjustBox;
        private System.Windows.Forms.ComboBox strokeSizeBox;
        private System.Windows.Forms.Label strokeSizeLabel;
        private System.Windows.Forms.ToolStripMenuItem edgeColorToolStripMenuItem;
        private System.Windows.Forms.ColorDialog strokeColorDialog;
        private System.Windows.Forms.ToolStripMenuItem loadJSONFileToolStripMenuItem;
        private System.Windows.Forms.ComboBox guidelineType;
        private System.Windows.Forms.Label guidelineLabel;
        private System.Windows.Forms.GroupBox guideLineBox;
        private System.Windows.Forms.Button nextBoxButton;
        private System.Windows.Forms.Button prevBoxButton;
        private System.Windows.Forms.Label boxesLabel;
        private System.Windows.Forms.CheckBox showGuideLines;
        private System.Windows.Forms.ToolStripMenuItem guideLineColorToolStripMenuItem;

	}
}

