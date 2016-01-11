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
            this.outputSeqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTriMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSequenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tools = new System.Windows.Forms.ToolStripDropDownButton();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSequenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.contourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.strokeStyle = new System.Windows.Forms.ToolStripDropDownButton();
            this.pencilToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pen1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pen2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.crayonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ink1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ink2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.guideLineColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.exampleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSpecificFacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPanel = new System.Windows.Forms.SplitContainer();
            this.fileNameTabs = new System.Windows.Forms.TabControl();
            this.pageNumber = new System.Windows.Forms.Label();
            this.statistics = new System.Windows.Forms.Label();
            this.toolboxPanel = new System.Windows.Forms.Panel();
            this.contourLabel = new System.Windows.Forms.GroupBox();
            this.showSegApparentRidge = new System.Windows.Forms.CheckBox();
            this.showSegSuggestiveContour = new System.Windows.Forms.CheckBox();
            this.showSegSilhouette = new System.Windows.Forms.CheckBox();
            this.showSegContour = new System.Windows.Forms.CheckBox();
            this.sharpEdge = new System.Windows.Forms.CheckBox();
            this.vanishingBox = new System.Windows.Forms.GroupBox();
            this.showFaceToDraw = new System.Windows.Forms.CheckBox();
            this.showGuideLineVanlines = new System.Windows.Forms.CheckBox();
            this.showBoxVanlines = new System.Windows.Forms.CheckBox();
            this.vanishingPoint2 = new System.Windows.Forms.CheckBox();
            this.vanishingPoint1 = new System.Windows.Forms.CheckBox();
            this.vanishingLineType = new System.Windows.Forms.ComboBox();
            this.vlLabel = new System.Windows.Forms.Label();
            this.guideLineBox = new System.Windows.Forms.GroupBox();
            this.lockView = new System.Windows.Forms.CheckBox();
            this.redoSequence = new System.Windows.Forms.Button();
            this.sequencePrevButton = new System.Windows.Forms.Button();
            this.sequenceNextButton = new System.Windows.Forms.Button();
            this.nextBoxButton = new System.Windows.Forms.Button();
            this.prevBoxButton = new System.Windows.Forms.Button();
            this.glLabel = new System.Windows.Forms.Label();
            this.boxesLabel = new System.Windows.Forms.Label();
            this.adjustBox = new System.Windows.Forms.GroupBox();
            this.lineOrMesh = new System.Windows.Forms.ComboBox();
            this.meshorlineLabel = new System.Windows.Forms.Label();
            this.sketchyDegreeTrackBar = new System.Windows.Forms.TrackBar();
            this.sketcyLable = new System.Windows.Forms.Label();
            this.enableHiddenCheck = new System.Windows.Forms.CheckBox();
            this.depthType = new System.Windows.Forms.ComboBox();
            this.depthLabel = new System.Windows.Forms.Label();
            this.guidelineType = new System.Windows.Forms.ComboBox();
            this.guidelineLabel = new System.Windows.Forms.Label();
            this.strokeSizeLabel = new System.Windows.Forms.Label();
            this.strokeSizeBox = new System.Windows.Forms.ComboBox();
            this.displayBox = new System.Windows.Forms.GroupBox();
            this.showGuideLines = new System.Windows.Forms.CheckBox();
            this.showSketchyEdges = new System.Windows.Forms.CheckBox();
            this.showMesh = new System.Windows.Forms.CheckBox();
            this.showBBox = new System.Windows.Forms.CheckBox();
            this.strokeColorDialog = new System.Windows.Forms.ColorDialog();
            this.glViewer = new SketchPlatform.GLViewer();
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewPanel)).BeginInit();
            this.viewPanel.Panel1.SuspendLayout();
            this.viewPanel.Panel2.SuspendLayout();
            this.viewPanel.SuspendLayout();
            this.toolboxPanel.SuspendLayout();
            this.contourLabel.SuspendLayout();
            this.vanishingBox.SuspendLayout();
            this.guideLineBox.SuspendLayout();
            this.adjustBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sketchyDegreeTrackBar)).BeginInit();
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
            this.strokeStyle,
            this.testTools});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(866, 39);
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
            this.loadJSONFileToolStripMenuItem,
            this.outputSeqToolStripMenuItem,
            this.loadTriMeshToolStripMenuItem,
            this.exportSequenceToolStripMenuItem});
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
            this.open3D.Size = new System.Drawing.Size(160, 22);
            this.open3D.Text = "Open 3D file";
            this.open3D.Click += new System.EventHandler(this.open3D_Click);
            // 
            // import3D
            // 
            this.import3D.Name = "import3D";
            this.import3D.Size = new System.Drawing.Size(160, 22);
            this.import3D.Text = "Import 3D file";
            // 
            // saveAs3D
            // 
            this.saveAs3D.Name = "saveAs3D";
            this.saveAs3D.Size = new System.Drawing.Size(160, 22);
            this.saveAs3D.Text = "Save As 3D file";
            this.saveAs3D.Click += new System.EventHandler(this.saveAs3D_Click);
            // 
            // loadSegmentsToolStripMenuItem
            // 
            this.loadSegmentsToolStripMenuItem.Name = "loadSegmentsToolStripMenuItem";
            this.loadSegmentsToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.loadSegmentsToolStripMenuItem.Text = "Load Segments";
            this.loadSegmentsToolStripMenuItem.Click += new System.EventHandler(this.loadSegmentsToolStripMenuItem_Click);
            // 
            // loadJSONFileToolStripMenuItem
            // 
            this.loadJSONFileToolStripMenuItem.Name = "loadJSONFileToolStripMenuItem";
            this.loadJSONFileToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.loadJSONFileToolStripMenuItem.Text = "Load JSON file";
            this.loadJSONFileToolStripMenuItem.Click += new System.EventHandler(this.loadJSONFileToolStripMenuItem_Click);
            // 
            // outputSeqToolStripMenuItem
            // 
            this.outputSeqToolStripMenuItem.Name = "outputSeqToolStripMenuItem";
            this.outputSeqToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.outputSeqToolStripMenuItem.Text = "Output Box seq";
            this.outputSeqToolStripMenuItem.Click += new System.EventHandler(this.outputSeqToolStripMenuItem_Click);
            // 
            // loadTriMeshToolStripMenuItem
            // 
            this.loadTriMeshToolStripMenuItem.Name = "loadTriMeshToolStripMenuItem";
            this.loadTriMeshToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.loadTriMeshToolStripMenuItem.Text = "Load tri mesh";
            this.loadTriMeshToolStripMenuItem.Click += new System.EventHandler(this.loadTriMeshToolStripMenuItem_Click);
            // 
            // exportSequenceToolStripMenuItem
            // 
            this.exportSequenceToolStripMenuItem.Name = "exportSequenceToolStripMenuItem";
            this.exportSequenceToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.exportSequenceToolStripMenuItem.Text = "Export sequence";
            this.exportSequenceToolStripMenuItem.Click += new System.EventHandler(this.exportSequenceToolStripMenuItem_Click);
            // 
            // tools
            // 
            this.tools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.resetViewToolStripMenuItem,
            this.modelColorToolStripMenuItem,
            this.reloadViewToolStripMenuItem,
            this.autoSequenceToolStripMenuItem,
            this.saveViewToolStripMenuItem,
            this.loadViewToolStripMenuItem});
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
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // resetViewToolStripMenuItem
            // 
            this.resetViewToolStripMenuItem.Name = "resetViewToolStripMenuItem";
            this.resetViewToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.resetViewToolStripMenuItem.Text = "Reset View";
            this.resetViewToolStripMenuItem.Click += new System.EventHandler(this.resetViewToolStripMenuItem_Click);
            // 
            // modelColorToolStripMenuItem
            // 
            this.modelColorToolStripMenuItem.Name = "modelColorToolStripMenuItem";
            this.modelColorToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.modelColorToolStripMenuItem.Text = "Model Color";
            this.modelColorToolStripMenuItem.Click += new System.EventHandler(this.modelColorToolStripMenuItem_Click);
            // 
            // reloadViewToolStripMenuItem
            // 
            this.reloadViewToolStripMenuItem.Name = "reloadViewToolStripMenuItem";
            this.reloadViewToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.reloadViewToolStripMenuItem.Text = "Reload View";
            this.reloadViewToolStripMenuItem.Click += new System.EventHandler(this.reloadViewToolStripMenuItem_Click);
            // 
            // autoSequenceToolStripMenuItem
            // 
            this.autoSequenceToolStripMenuItem.Name = "autoSequenceToolStripMenuItem";
            this.autoSequenceToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.autoSequenceToolStripMenuItem.Text = "Auto sequence";
            this.autoSequenceToolStripMenuItem.Click += new System.EventHandler(this.autoSequenceToolStripMenuItem_Click);
            // 
            // saveViewToolStripMenuItem
            // 
            this.saveViewToolStripMenuItem.Name = "saveViewToolStripMenuItem";
            this.saveViewToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.saveViewToolStripMenuItem.Text = "Save view";
            this.saveViewToolStripMenuItem.Click += new System.EventHandler(this.saveViewToolStripMenuItem_Click);
            // 
            // loadViewToolStripMenuItem
            // 
            this.loadViewToolStripMenuItem.Name = "loadViewToolStripMenuItem";
            this.loadViewToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.loadViewToolStripMenuItem.Text = "Load view";
            this.loadViewToolStripMenuItem.Click += new System.EventHandler(this.loadViewToolStripMenuItem_Click);
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
            this.displayAxis,
            this.contourToolStripMenuItem});
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
            this.displayAxis.Size = new System.Drawing.Size(116, 22);
            this.displayAxis.Text = "Axis";
            this.displayAxis.Click += new System.EventHandler(this.displayAxis_Click);
            // 
            // contourToolStripMenuItem
            // 
            this.contourToolStripMenuItem.Name = "contourToolStripMenuItem";
            this.contourToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.contourToolStripMenuItem.Text = "contour";
            this.contourToolStripMenuItem.Click += new System.EventHandler(this.contourToolStripMenuItem_Click);
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
            // guideLineColorToolStripMenuItem
            // 
            this.guideLineColorToolStripMenuItem.Name = "guideLineColorToolStripMenuItem";
            this.guideLineColorToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.guideLineColorToolStripMenuItem.Text = "guide line color";
            this.guideLineColorToolStripMenuItem.Click += new System.EventHandler(this.guideLineColorToolStripMenuItem_Click);
            // 
            // testTools
            // 
            this.testTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exampleToolStripMenuItem,
            this.moveBoxToolStripMenuItem,
            this.showSpecificFacesToolStripMenuItem});
            this.testTools.Image = ((System.Drawing.Image)(resources.GetObject("testTools.Image")));
            this.testTools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.testTools.Name = "testTools";
            this.testTools.Size = new System.Drawing.Size(45, 36);
            this.testTools.Text = "Test";
            this.testTools.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // exampleToolStripMenuItem
            // 
            this.exampleToolStripMenuItem.Name = "exampleToolStripMenuItem";
            this.exampleToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.exampleToolStripMenuItem.Text = "show All guides";
            this.exampleToolStripMenuItem.Click += new System.EventHandler(this.exampleToolStripMenuItem_Click);
            // 
            // moveBoxToolStripMenuItem
            // 
            this.moveBoxToolStripMenuItem.Name = "moveBoxToolStripMenuItem";
            this.moveBoxToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.moveBoxToolStripMenuItem.Text = "move box";
            this.moveBoxToolStripMenuItem.Click += new System.EventHandler(this.moveBoxToolStripMenuItem_Click);
            // 
            // showSpecificFacesToolStripMenuItem
            // 
            this.showSpecificFacesToolStripMenuItem.Name = "showSpecificFacesToolStripMenuItem";
            this.showSpecificFacesToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.showSpecificFacesToolStripMenuItem.Text = "show specific faces";
            this.showSpecificFacesToolStripMenuItem.Click += new System.EventHandler(this.showSpecificFacesToolStripMenuItem_Click);
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
            this.viewPanel.Panel2.Controls.Add(this.pageNumber);
            this.viewPanel.Panel2.Controls.Add(this.glViewer);
            this.viewPanel.Panel2.Controls.Add(this.statistics);
            this.viewPanel.Panel2.Controls.Add(this.toolboxPanel);
            this.viewPanel.Size = new System.Drawing.Size(866, 715);
            this.viewPanel.SplitterDistance = 36;
            this.viewPanel.TabIndex = 1;
            // 
            // fileNameTabs
            // 
            this.fileNameTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameTabs.Location = new System.Drawing.Point(3, 0);
            this.fileNameTabs.Name = "fileNameTabs";
            this.fileNameTabs.SelectedIndex = 0;
            this.fileNameTabs.Size = new System.Drawing.Size(860, 35);
            this.fileNameTabs.TabIndex = 0;
            // 
            // pageNumber
            // 
            this.pageNumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pageNumber.AutoSize = true;
            this.pageNumber.BackColor = System.Drawing.Color.White;
            this.pageNumber.Font = new System.Drawing.Font("Bookman Old Style", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageNumber.ForeColor = System.Drawing.Color.Black;
            this.pageNumber.Location = new System.Drawing.Point(822, 8);
            this.pageNumber.Name = "pageNumber";
            this.pageNumber.Size = new System.Drawing.Size(32, 32);
            this.pageNumber.TabIndex = 4;
            this.pageNumber.Text = "0";
            this.pageNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.toolboxPanel.Controls.Add(this.contourLabel);
            this.toolboxPanel.Controls.Add(this.vanishingBox);
            this.toolboxPanel.Controls.Add(this.guideLineBox);
            this.toolboxPanel.Controls.Add(this.adjustBox);
            this.toolboxPanel.Controls.Add(this.displayBox);
            this.toolboxPanel.Location = new System.Drawing.Point(3, 3);
            this.toolboxPanel.Name = "toolboxPanel";
            this.toolboxPanel.Size = new System.Drawing.Size(165, 669);
            this.toolboxPanel.TabIndex = 2;
            // 
            // contourLabel
            // 
            this.contourLabel.Controls.Add(this.showSegApparentRidge);
            this.contourLabel.Controls.Add(this.showSegSuggestiveContour);
            this.contourLabel.Controls.Add(this.showSegSilhouette);
            this.contourLabel.Controls.Add(this.showSegContour);
            this.contourLabel.Controls.Add(this.sharpEdge);
            this.contourLabel.Location = new System.Drawing.Point(3, 556);
            this.contourLabel.Name = "contourLabel";
            this.contourLabel.Size = new System.Drawing.Size(156, 108);
            this.contourLabel.TabIndex = 40;
            this.contourLabel.TabStop = false;
            this.contourLabel.Text = "Contour";
            // 
            // showSegApparentRidge
            // 
            this.showSegApparentRidge.AutoSize = true;
            this.showSegApparentRidge.Location = new System.Drawing.Point(5, 65);
            this.showSegApparentRidge.Name = "showSegApparentRidge";
            this.showSegApparentRidge.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showSegApparentRidge.Size = new System.Drawing.Size(94, 17);
            this.showSegApparentRidge.TabIndex = 18;
            this.showSegApparentRidge.Text = "apparent ridge";
            this.showSegApparentRidge.UseVisualStyleBackColor = true;
            this.showSegApparentRidge.CheckedChanged += new System.EventHandler(this.showSegApparentRidge_CheckedChanged);
            // 
            // showSegSuggestiveContour
            // 
            this.showSegSuggestiveContour.AutoSize = true;
            this.showSegSuggestiveContour.Location = new System.Drawing.Point(5, 42);
            this.showSegSuggestiveContour.Name = "showSegSuggestiveContour";
            this.showSegSuggestiveContour.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showSegSuggestiveContour.Size = new System.Drawing.Size(116, 17);
            this.showSegSuggestiveContour.TabIndex = 17;
            this.showSegSuggestiveContour.Text = "suggestive contour";
            this.showSegSuggestiveContour.UseVisualStyleBackColor = true;
            this.showSegSuggestiveContour.CheckedChanged += new System.EventHandler(this.showSegSuggestiveContour_CheckedChanged);
            // 
            // showSegSilhouette
            // 
            this.showSegSilhouette.AutoSize = true;
            this.showSegSilhouette.Location = new System.Drawing.Point(79, 19);
            this.showSegSilhouette.Name = "showSegSilhouette";
            this.showSegSilhouette.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showSegSilhouette.Size = new System.Drawing.Size(71, 17);
            this.showSegSilhouette.TabIndex = 16;
            this.showSegSilhouette.Text = "silhouette";
            this.showSegSilhouette.UseVisualStyleBackColor = true;
            this.showSegSilhouette.CheckedChanged += new System.EventHandler(this.showSegSilhouette_CheckedChanged);
            // 
            // showSegContour
            // 
            this.showSegContour.AutoSize = true;
            this.showSegContour.Location = new System.Drawing.Point(5, 19);
            this.showSegContour.Name = "showSegContour";
            this.showSegContour.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showSegContour.Size = new System.Drawing.Size(62, 17);
            this.showSegContour.TabIndex = 15;
            this.showSegContour.Text = "contour";
            this.showSegContour.UseVisualStyleBackColor = true;
            this.showSegContour.CheckedChanged += new System.EventHandler(this.showSegContour_CheckedChanged);
            // 
            // sharpEdge
            // 
            this.sharpEdge.AutoSize = true;
            this.sharpEdge.Location = new System.Drawing.Point(4, 88);
            this.sharpEdge.Name = "sharpEdge";
            this.sharpEdge.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.sharpEdge.Size = new System.Drawing.Size(79, 17);
            this.sharpEdge.TabIndex = 13;
            this.sharpEdge.Text = "sharp edge";
            this.sharpEdge.UseVisualStyleBackColor = true;
            this.sharpEdge.CheckedChanged += new System.EventHandler(this.sharpEdge_CheckedChanged);
            // 
            // vanishingBox
            // 
            this.vanishingBox.Controls.Add(this.showFaceToDraw);
            this.vanishingBox.Controls.Add(this.showGuideLineVanlines);
            this.vanishingBox.Controls.Add(this.showBoxVanlines);
            this.vanishingBox.Controls.Add(this.vanishingPoint2);
            this.vanishingBox.Controls.Add(this.vanishingPoint1);
            this.vanishingBox.Controls.Add(this.vanishingLineType);
            this.vanishingBox.Controls.Add(this.vlLabel);
            this.vanishingBox.Location = new System.Drawing.Point(3, 409);
            this.vanishingBox.Name = "vanishingBox";
            this.vanishingBox.Size = new System.Drawing.Size(156, 141);
            this.vanishingBox.TabIndex = 39;
            this.vanishingBox.TabStop = false;
            this.vanishingBox.Text = "Vanishing Lines";
            // 
            // showFaceToDraw
            // 
            this.showFaceToDraw.AutoSize = true;
            this.showFaceToDraw.Checked = true;
            this.showFaceToDraw.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showFaceToDraw.Location = new System.Drawing.Point(5, 113);
            this.showFaceToDraw.Name = "showFaceToDraw";
            this.showFaceToDraw.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showFaceToDraw.Size = new System.Drawing.Size(85, 17);
            this.showFaceToDraw.TabIndex = 11;
            this.showFaceToDraw.Text = "face to draw";
            this.showFaceToDraw.UseVisualStyleBackColor = true;
            this.showFaceToDraw.CheckedChanged += new System.EventHandler(this.showFaceToDraw_CheckedChanged);
            // 
            // showGuideLineVanlines
            // 
            this.showGuideLineVanlines.AutoSize = true;
            this.showGuideLineVanlines.Location = new System.Drawing.Point(5, 90);
            this.showGuideLineVanlines.Name = "showGuideLineVanlines";
            this.showGuideLineVanlines.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showGuideLineVanlines.Size = new System.Drawing.Size(111, 17);
            this.showGuideLineVanlines.TabIndex = 10;
            this.showGuideLineVanlines.Text = "vanRay guide line";
            this.showGuideLineVanlines.UseVisualStyleBackColor = true;
            this.showGuideLineVanlines.CheckedChanged += new System.EventHandler(this.showGuideLineVanlines_CheckedChanged);
            // 
            // showBoxVanlines
            // 
            this.showBoxVanlines.AutoSize = true;
            this.showBoxVanlines.Checked = true;
            this.showBoxVanlines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showBoxVanlines.Location = new System.Drawing.Point(3, 67);
            this.showBoxVanlines.Name = "showBoxVanlines";
            this.showBoxVanlines.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.showBoxVanlines.Size = new System.Drawing.Size(83, 17);
            this.showBoxVanlines.TabIndex = 9;
            this.showBoxVanlines.Text = "vanRay box";
            this.showBoxVanlines.UseVisualStyleBackColor = true;
            this.showBoxVanlines.CheckedChanged += new System.EventHandler(this.showBoxVanlines_CheckedChanged);
            // 
            // vanishingPoint2
            // 
            this.vanishingPoint2.AutoSize = true;
            this.vanishingPoint2.Checked = true;
            this.vanishingPoint2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.vanishingPoint2.Location = new System.Drawing.Point(80, 44);
            this.vanishingPoint2.Name = "vanishingPoint2";
            this.vanishingPoint2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.vanishingPoint2.Size = new System.Drawing.Size(70, 17);
            this.vanishingPoint2.TabIndex = 8;
            this.vanishingPoint2.Text = "van ray 2";
            this.vanishingPoint2.UseVisualStyleBackColor = true;
            this.vanishingPoint2.CheckedChanged += new System.EventHandler(this.vanishingPoint2_CheckedChanged);
            // 
            // vanishingPoint1
            // 
            this.vanishingPoint1.AutoSize = true;
            this.vanishingPoint1.Checked = true;
            this.vanishingPoint1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.vanishingPoint1.Location = new System.Drawing.Point(4, 44);
            this.vanishingPoint1.Name = "vanishingPoint1";
            this.vanishingPoint1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.vanishingPoint1.Size = new System.Drawing.Size(70, 17);
            this.vanishingPoint1.TabIndex = 5;
            this.vanishingPoint1.Text = "van ray 1";
            this.vanishingPoint1.UseVisualStyleBackColor = true;
            this.vanishingPoint1.CheckedChanged += new System.EventHandler(this.vanishingPoint1_CheckedChanged);
            // 
            // vanishingLineType
            // 
            this.vanishingLineType.FormattingEnabled = true;
            this.vanishingLineType.Items.AddRange(new object[] {
            "line",
            "dashed",
            "none"});
            this.vanishingLineType.Location = new System.Drawing.Point(65, 17);
            this.vanishingLineType.Name = "vanishingLineType";
            this.vanishingLineType.Size = new System.Drawing.Size(62, 21);
            this.vanishingLineType.TabIndex = 7;
            this.vanishingLineType.Text = "line";
            this.vanishingLineType.SelectedIndexChanged += new System.EventHandler(this.vanishingLineType_SelectedIndexChanged);
            // 
            // vlLabel
            // 
            this.vlLabel.AutoSize = true;
            this.vlLabel.Location = new System.Drawing.Point(6, 20);
            this.vlLabel.Name = "vlLabel";
            this.vlLabel.Size = new System.Drawing.Size(47, 13);
            this.vlLabel.TabIndex = 6;
            this.vlLabel.Text = "van line:";
            this.vlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // guideLineBox
            // 
            this.guideLineBox.Controls.Add(this.lockView);
            this.guideLineBox.Controls.Add(this.redoSequence);
            this.guideLineBox.Controls.Add(this.sequencePrevButton);
            this.guideLineBox.Controls.Add(this.sequenceNextButton);
            this.guideLineBox.Controls.Add(this.nextBoxButton);
            this.guideLineBox.Controls.Add(this.prevBoxButton);
            this.guideLineBox.Controls.Add(this.glLabel);
            this.guideLineBox.Controls.Add(this.boxesLabel);
            this.guideLineBox.Location = new System.Drawing.Point(4, 305);
            this.guideLineBox.Name = "guideLineBox";
            this.guideLineBox.Size = new System.Drawing.Size(156, 98);
            this.guideLineBox.TabIndex = 4;
            this.guideLineBox.TabStop = false;
            this.guideLineBox.Text = "Guides";
            // 
            // lockView
            // 
            this.lockView.AutoSize = true;
            this.lockView.Location = new System.Drawing.Point(6, 77);
            this.lockView.Name = "lockView";
            this.lockView.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lockView.Size = new System.Drawing.Size(71, 17);
            this.lockView.TabIndex = 38;
            this.lockView.Text = "lock view";
            this.lockView.UseVisualStyleBackColor = true;
            this.lockView.CheckedChanged += new System.EventHandler(this.lockView_CheckedChanged);
            // 
            // redoSequence
            // 
            this.redoSequence.Image = ((System.Drawing.Image)(resources.GetObject("redoSequence.Image")));
            this.redoSequence.Location = new System.Drawing.Point(127, 45);
            this.redoSequence.Name = "redoSequence";
            this.redoSequence.Size = new System.Drawing.Size(26, 23);
            this.redoSequence.TabIndex = 37;
            this.redoSequence.UseVisualStyleBackColor = true;
            this.redoSequence.Click += new System.EventHandler(this.redoSequence_Click);
            // 
            // sequencePrevButton
            // 
            this.sequencePrevButton.Image = ((System.Drawing.Image)(resources.GetObject("sequencePrevButton.Image")));
            this.sequencePrevButton.Location = new System.Drawing.Point(54, 45);
            this.sequencePrevButton.Name = "sequencePrevButton";
            this.sequencePrevButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.sequencePrevButton.Size = new System.Drawing.Size(26, 23);
            this.sequencePrevButton.TabIndex = 36;
            this.sequencePrevButton.UseVisualStyleBackColor = true;
            this.sequencePrevButton.Click += new System.EventHandler(this.sequencePrevButton_Click);
            // 
            // sequenceNextButton
            // 
            this.sequenceNextButton.Image = ((System.Drawing.Image)(resources.GetObject("sequenceNextButton.Image")));
            this.sequenceNextButton.Location = new System.Drawing.Point(90, 45);
            this.sequenceNextButton.Name = "sequenceNextButton";
            this.sequenceNextButton.Size = new System.Drawing.Size(26, 23);
            this.sequenceNextButton.TabIndex = 35;
            this.sequenceNextButton.UseVisualStyleBackColor = true;
            this.sequenceNextButton.Click += new System.EventHandler(this.sequenceNextButton_Click);
            // 
            // nextBoxButton
            // 
            this.nextBoxButton.Image = ((System.Drawing.Image)(resources.GetObject("nextBoxButton.Image")));
            this.nextBoxButton.Location = new System.Drawing.Point(89, 18);
            this.nextBoxButton.Name = "nextBoxButton";
            this.nextBoxButton.Size = new System.Drawing.Size(26, 23);
            this.nextBoxButton.TabIndex = 34;
            this.nextBoxButton.UseVisualStyleBackColor = true;
            this.nextBoxButton.Click += new System.EventHandler(this.nextBoxButton_Click);
            // 
            // prevBoxButton
            // 
            this.prevBoxButton.Image = ((System.Drawing.Image)(resources.GetObject("prevBoxButton.Image")));
            this.prevBoxButton.Location = new System.Drawing.Point(53, 18);
            this.prevBoxButton.Name = "prevBoxButton";
            this.prevBoxButton.Size = new System.Drawing.Size(26, 23);
            this.prevBoxButton.TabIndex = 33;
            this.prevBoxButton.UseVisualStyleBackColor = true;
            this.prevBoxButton.Click += new System.EventHandler(this.prevBoxButton_Click);
            // 
            // glLabel
            // 
            this.glLabel.AutoSize = true;
            this.glLabel.Location = new System.Drawing.Point(6, 50);
            this.glLabel.Name = "glLabel";
            this.glLabel.Size = new System.Drawing.Size(36, 13);
            this.glLabel.TabIndex = 5;
            this.glLabel.Text = "guide:";
            // 
            // boxesLabel
            // 
            this.boxesLabel.AutoSize = true;
            this.boxesLabel.Location = new System.Drawing.Point(5, 23);
            this.boxesLabel.Name = "boxesLabel";
            this.boxesLabel.Size = new System.Drawing.Size(27, 13);
            this.boxesLabel.TabIndex = 0;
            this.boxesLabel.Text = "box:";
            // 
            // adjustBox
            // 
            this.adjustBox.Controls.Add(this.lineOrMesh);
            this.adjustBox.Controls.Add(this.meshorlineLabel);
            this.adjustBox.Controls.Add(this.sketchyDegreeTrackBar);
            this.adjustBox.Controls.Add(this.sketcyLable);
            this.adjustBox.Controls.Add(this.enableHiddenCheck);
            this.adjustBox.Controls.Add(this.depthType);
            this.adjustBox.Controls.Add(this.depthLabel);
            this.adjustBox.Controls.Add(this.guidelineType);
            this.adjustBox.Controls.Add(this.guidelineLabel);
            this.adjustBox.Controls.Add(this.strokeSizeLabel);
            this.adjustBox.Controls.Add(this.strokeSizeBox);
            this.adjustBox.Location = new System.Drawing.Point(3, 124);
            this.adjustBox.Name = "adjustBox";
            this.adjustBox.Size = new System.Drawing.Size(156, 175);
            this.adjustBox.TabIndex = 3;
            this.adjustBox.TabStop = false;
            this.adjustBox.Text = "Adjust";
            // 
            // lineOrMesh
            // 
            this.lineOrMesh.AllowDrop = true;
            this.lineOrMesh.FormattingEnabled = true;
            this.lineOrMesh.Items.AddRange(new object[] {
            "line",
            "mesh"});
            this.lineOrMesh.Location = new System.Drawing.Point(65, 142);
            this.lineOrMesh.Name = "lineOrMesh";
            this.lineOrMesh.Size = new System.Drawing.Size(48, 21);
            this.lineOrMesh.TabIndex = 16;
            this.lineOrMesh.Text = "lineOrMesh";
            this.lineOrMesh.SelectedIndexChanged += new System.EventHandler(this.lineOrMesh_SelectedIndexChanged);
            // 
            // meshorlineLabel
            // 
            this.meshorlineLabel.AutoSize = true;
            this.meshorlineLabel.Location = new System.Drawing.Point(2, 145);
            this.meshorlineLabel.Name = "meshorlineLabel";
            this.meshorlineLabel.Size = new System.Drawing.Size(57, 13);
            this.meshorlineLabel.TabIndex = 15;
            this.meshorlineLabel.Text = "mesOrLine";
            this.meshorlineLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // sketchyDegreeTrackBar
            // 
            this.sketchyDegreeTrackBar.Location = new System.Drawing.Point(46, 113);
            this.sketchyDegreeTrackBar.Name = "sketchyDegreeTrackBar";
            this.sketchyDegreeTrackBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.sketchyDegreeTrackBar.Size = new System.Drawing.Size(94, 45);
            this.sketchyDegreeTrackBar.TabIndex = 14;
            this.sketchyDegreeTrackBar.Value = 3;
            this.sketchyDegreeTrackBar.Scroll += new System.EventHandler(this.sketchyDegreeTrackBar_Scroll);
            // 
            // sketcyLable
            // 
            this.sketcyLable.AutoSize = true;
            this.sketcyLable.Location = new System.Drawing.Point(3, 117);
            this.sketcyLable.Name = "sketcyLable";
            this.sketcyLable.Size = new System.Drawing.Size(44, 13);
            this.sketcyLable.TabIndex = 13;
            this.sketcyLable.Text = "sketchy";
            this.sketcyLable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // enableHiddenCheck
            // 
            this.enableHiddenCheck.AutoSize = true;
            this.enableHiddenCheck.Checked = true;
            this.enableHiddenCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableHiddenCheck.Location = new System.Drawing.Point(1, 96);
            this.enableHiddenCheck.Name = "enableHiddenCheck";
            this.enableHiddenCheck.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.enableHiddenCheck.Size = new System.Drawing.Size(77, 17);
            this.enableHiddenCheck.TabIndex = 12;
            this.enableHiddenCheck.Text = "hidden line";
            this.enableHiddenCheck.UseVisualStyleBackColor = true;
            this.enableHiddenCheck.CheckedChanged += new System.EventHandler(this.enableHiddenCheck_CheckedChanged);
            // 
            // depthType
            // 
            this.depthType.FormattingEnabled = true;
            this.depthType.Items.AddRange(new object[] {
            "opacity",
            "hidden",
            "OpenGL_DetphTest",
            "none",
            "ray trace"});
            this.depthType.Location = new System.Drawing.Point(66, 71);
            this.depthType.Name = "depthType";
            this.depthType.Size = new System.Drawing.Size(62, 21);
            this.depthType.TabIndex = 5;
            this.depthType.Text = "opacity";
            this.depthType.SelectedIndexChanged += new System.EventHandler(this.depthType_SelectedIndexChanged);
            // 
            // depthLabel
            // 
            this.depthLabel.AutoSize = true;
            this.depthLabel.Location = new System.Drawing.Point(3, 74);
            this.depthLabel.Name = "depthLabel";
            this.depthLabel.Size = new System.Drawing.Size(37, 13);
            this.depthLabel.TabIndex = 4;
            this.depthLabel.Text = "depth:";
            this.depthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // guidelineType
            // 
            this.guidelineType.FormattingEnabled = true;
            this.guidelineType.Items.AddRange(new object[] {
            "random",
            "cross"});
            this.guidelineType.Location = new System.Drawing.Point(66, 47);
            this.guidelineType.Name = "guidelineType";
            this.guidelineType.Size = new System.Drawing.Size(66, 21);
            this.guidelineType.TabIndex = 3;
            this.guidelineType.Text = "random";
            this.guidelineType.SelectedIndexChanged += new System.EventHandler(this.guidelineType_SelectedIndexChanged);
            // 
            // guidelineLabel
            // 
            this.guidelineLabel.AutoSize = true;
            this.guidelineLabel.Location = new System.Drawing.Point(3, 50);
            this.guidelineLabel.Name = "guidelineLabel";
            this.guidelineLabel.Size = new System.Drawing.Size(55, 13);
            this.guidelineLabel.TabIndex = 2;
            this.guidelineLabel.Text = "guide line:";
            this.guidelineLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // strokeSizeLabel
            // 
            this.strokeSizeLabel.AutoSize = true;
            this.strokeSizeLabel.Location = new System.Drawing.Point(3, 24);
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
            "10",
            "12"});
            this.strokeSizeBox.Location = new System.Drawing.Point(66, 21);
            this.strokeSizeBox.Name = "strokeSizeBox";
            this.strokeSizeBox.Size = new System.Drawing.Size(48, 21);
            this.strokeSizeBox.TabIndex = 0;
            this.strokeSizeBox.Text = "2";
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
            this.displayBox.Size = new System.Drawing.Size(157, 114);
            this.displayBox.TabIndex = 2;
            this.displayBox.TabStop = false;
            this.displayBox.Text = "Display";
            // 
            // showGuideLines
            // 
            this.showGuideLines.AutoSize = true;
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
            this.glViewer.ColorBits = ((byte)(32));
            this.glViewer.CurrentUIMode = SketchPlatform.GLViewer.UIMode.Viewing;
            this.glViewer.DepthBits = ((byte)(16));
            this.glViewer.Location = new System.Drawing.Point(173, 3);
            this.glViewer.Name = "glViewer";
            this.glViewer.Size = new System.Drawing.Size(689, 694);
            this.glViewer.StencilBits = ((byte)(0));
            this.glViewer.TabIndex = 5;
            // 
            // Interface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(866, 754);
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
            this.contourLabel.ResumeLayout(false);
            this.contourLabel.PerformLayout();
            this.vanishingBox.ResumeLayout(false);
            this.vanishingBox.PerformLayout();
            this.guideLineBox.ResumeLayout(false);
            this.guideLineBox.PerformLayout();
            this.adjustBox.ResumeLayout(false);
            this.adjustBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sketchyDegreeTrackBar)).EndInit();
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
        private System.Windows.Forms.Label boxesLabel;
        private System.Windows.Forms.CheckBox showGuideLines;
        private System.Windows.Forms.ToolStripMenuItem guideLineColorToolStripMenuItem;
        private System.Windows.Forms.Button prevBoxButton;
        private System.Windows.Forms.Label glLabel;
        private System.Windows.Forms.Button nextBoxButton;
        private System.Windows.Forms.Button sequencePrevButton;
        private System.Windows.Forms.Button sequenceNextButton;
        private System.Windows.Forms.ToolStripMenuItem reloadViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outputSeqToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSequenceToolStripMenuItem;
        private System.Windows.Forms.ComboBox depthType;
        private System.Windows.Forms.Label depthLabel;
        private System.Windows.Forms.ComboBox vanishingLineType;
        private System.Windows.Forms.Label vlLabel;
        private System.Windows.Forms.ToolStripMenuItem saveViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadViewToolStripMenuItem;
        private System.Windows.Forms.Button redoSequence;
        private System.Windows.Forms.CheckBox lockView;
        private System.Windows.Forms.GroupBox vanishingBox;
        private System.Windows.Forms.CheckBox showGuideLineVanlines;
        private System.Windows.Forms.CheckBox showBoxVanlines;
        private System.Windows.Forms.CheckBox vanishingPoint2;
        private System.Windows.Forms.CheckBox vanishingPoint1;
        private System.Windows.Forms.CheckBox showFaceToDraw;
        private System.Windows.Forms.ToolStripMenuItem contourToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton testTools;
        private System.Windows.Forms.ToolStripMenuItem exampleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveBoxToolStripMenuItem;
        private System.Windows.Forms.GroupBox contourLabel;
        private System.Windows.Forms.CheckBox sharpEdge;
        private System.Windows.Forms.ToolStripMenuItem showSpecificFacesToolStripMenuItem;
        private System.Windows.Forms.CheckBox enableHiddenCheck;
        private System.Windows.Forms.ToolStripMenuItem loadTriMeshToolStripMenuItem;
        private System.Windows.Forms.CheckBox showSegSilhouette;
        private System.Windows.Forms.CheckBox showSegContour;
        private System.Windows.Forms.CheckBox showSegSuggestiveContour;
        private System.Windows.Forms.CheckBox showSegApparentRidge;
        private System.Windows.Forms.TrackBar sketchyDegreeTrackBar;
        private System.Windows.Forms.Label sketcyLable;
        private System.Windows.Forms.ComboBox lineOrMesh;
        private System.Windows.Forms.Label meshorlineLabel;
        private System.Windows.Forms.ToolStripMenuItem exportSequenceToolStripMenuItem;
        private System.Windows.Forms.Label pageNumber;
        private GLViewer glViewer;

	}
}

