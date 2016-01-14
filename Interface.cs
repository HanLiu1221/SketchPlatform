using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace SketchPlatform
{
	public partial class Interface : Form
	{
		public Interface()
		{
			InitializeComponent();
            this.glViewer.Init();
		}

        /*********Var**********/
        private void open3D_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "3D model (*.obj; *.off; *.ply)|*.obj; *.off; *.ply|All Files(*.*)|*.*";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = dialog.FileName;
                // load mesh
                this.glViewer.loadMesh(filename);
                // set tab page
                TabPage tp = new TabPage(Path.GetFileName(filename));
                this.fileNameTabs.TabPages.Add(tp);
                this.fileNameTabs.SelectedTab = tp;
                this.glViewer.setTabIndex(this.fileNameTabs.TabCount);
                int[] meshStats = this.glViewer.getMeshStatistics();
                this.statistics.Text = "# number of vertex: " + meshStats[0] + "\n"
                    + "# number of guideLines: " + meshStats[1] + "\n"
                    + "# number of faces: " + meshStats[2] + "\n";
            }
            this.glViewer.Refresh();
        }


        private void pointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.vertexToolStripMenuItem.Checked = !this.vertexToolStripMenuItem.Checked;
            this.glViewer.setRenderOption(1);
        }

        private void wireFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.wireFrameToolStripMenuItem.Checked = !this.wireFrameToolStripMenuItem.Checked;
            this.glViewer.setRenderOption(2);
        }

        private void faceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.faceToolStripMenuItem.Checked = !this.faceToolStripMenuItem.Checked;
            this.glViewer.setRenderOption(3);
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.setUIMode(1);
        }

        private void resetViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.resetView();
        }

        private void displayAxis_Click(object sender, EventArgs e)
        {
            this.displayAxis.Checked = !this.displayAxis.Checked;
            this.glViewer.displayAxes();
            this.glViewer.Refresh();
        }

        private void modelColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = GLViewer.ModelColor;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GLViewer.ModelColor = colorDialog.Color;
                this.glViewer.setSegmentColor(colorDialog.Color);
                this.glViewer.Refresh();
            }
        }

        private void vertexSelection_Click(object sender, EventArgs e)
        {
            this.glViewer.setUIMode(2);
        }

        private void edgeSelection_Click(object sender, EventArgs e)
        {
            this.glViewer.setUIMode(3);
        }

        private void faceSelection_Click(object sender, EventArgs e)
        {
            this.glViewer.setUIMode(4);
        }

        private void loadSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = "D:\\Projects\\sketchingTutorial\\SketchPlatform\\Data\\old\\segments";
            //dialog.SelectedPath = "D:\\Projects\\sketchingTutorial\\CGPlatform\\Data";
            //if (dialog.ShowDialog(this) == DialogResult.OK)
            //{
                string folderName = dialog.SelectedPath;
                this.glViewer.loadSegments(folderName);
                int[] segStats = this.glViewer.getSegmentStatistics();
                this.statistics.Text = "# number of segments: " + segStats[0] + "\n";
            //}
            this.glViewer.Refresh();
        }

        private void showBBox_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showBoundingbox = this.showBBox.Checked;
            this.glViewer.Refresh();
        }

        private void showMesh_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showMesh = this.showMesh.Checked;
            this.glViewer.Refresh();
        }

        private void showSketchyEdges_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showSketchyEdges = this.showSketchyEdges.Checked;
            this.glViewer.Refresh();
        }

        private void showGuideLines_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showGuideLines = this.showGuideLines.Checked;
            this.glViewer.Refresh();
        }

        private void pencilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.setStrokeStyle(0);
            this.glViewer.Refresh();
        }

        private void pen1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.setStrokeStyle(1);
            this.glViewer.Refresh();
        }

        private void pen2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.setStrokeStyle(2);
            this.glViewer.Refresh();
        }

        private void crayonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.setStrokeStyle(3);
            this.glViewer.Refresh();
        }

        private void ink1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.setStrokeStyle(4);
            this.glViewer.Refresh();
        }

        private void ink2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.setStrokeStyle(5);
            this.glViewer.Refresh();
        }

        //private void strokeSizeBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (this.strokeSizeBox.Text == null || this.strokeSizeBox.Text.Length == 0)
        //        return;
        //    int size = Int32.Parse(this.strokeSizeBox.Text);
        //    this.glViewer.setStrokeSize(size);
        //    this.glViewer.Refresh();
        //}

        private void strokeSizeBox_TextChanged(object sender, EventArgs e)
        {
            if (this.strokeSizeBox.Text == null || this.strokeSizeBox.Text.Length == 0)
                return;
            double size = double.Parse(this.strokeSizeBox.Text);
            this.glViewer.setStrokeSize(size);
            this.glViewer.Refresh();
        }

        private void edgeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.strokeColorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.glViewer.setSektchyEdgesColor(this.strokeColorDialog.Color);
                this.glViewer.Refresh();
            }
        }

        private void guideLineColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.strokeColorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.glViewer.setGuideLineColor(this.strokeColorDialog.Color);
                this.glViewer.Refresh();
            }
        }

        private void strokeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.strokeColorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.glViewer.setStrokeColor(this.strokeColorDialog.Color);
                this.glViewer.Refresh();
            }
        }

        private void loadJSONFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSOn file (*.json)|*.json;|All Files(*.*)|*.*";
            dialog.CheckFileExists = true;
            dialog.FileName = "D:\\Projects\\SketchingTutorial\\SketchPlatform\\Data\\json\\phone_output\\drawing_sequence.json";
            //dialog.FileName = "D:\\Projects\\SketchingTutorial\\SketchPlatform\\Data\\json\\mixer_output\\drawing_sequence.json";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = dialog.FileName;
                this.glViewer.loadJSONFile(filename);
                this.glViewer.Refresh();
            }
        }


        private void guidelineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.guidelineType.Text == null || this.guidelineType.Text.Length == 0)
                return;
            int idx = this.guidelineType.SelectedIndex;
            this.glViewer.setGuideLineStyle(idx);
            this.glViewer.Refresh();
        }

        private void prevBoxButton_Click(object sender, EventArgs e)
        {
            this.glViewer.prevBox();
            this.glViewer.Refresh();
        }

        private void nextBoxButton_Click(object sender, EventArgs e)
        {
            this.glViewer.nextBox();
            this.glViewer.Refresh();
        }

        private void sequencePrevButton_Click(object sender, EventArgs e)
        {
            string pageStr = this.glViewer.prevSequence();
            this.pageNumber.Text = pageStr;
            this.lockView.Checked = this.glViewer.lockView;
            this.glViewer.Refresh();
        }

        private void sequenceNextButton_Click(object sender, EventArgs e)
        {
            string pageStr = this.glViewer.nextSequence();
            this.pageNumber.Text = pageStr;
            this.lockView.Checked = this.glViewer.lockView;
            this.glViewer.Refresh();
        }

        private void redoSequence_Click(object sender, EventArgs e)
        {
            this.glViewer.redoSequence();
            //this.pageNumber.Text = "0";
            this.lockView.Checked = this.glViewer.lockView;
            this.glViewer.Refresh();
        }

        public void setPageNumber(string s)
        {
            this.pageNumber.Text = s;
            this.Refresh();
        }

        public void setPageNumberLocation(int x, int y)
        {
            this.pageNumber.Location = new Point(x, y);
            //this.Refresh();
        }

        public void setLockState(bool isLock)
        {
            this.lockView.Checked = isLock;
            this.Refresh();
        }

        private void reloadViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.reloadView();
            this.glViewer.Refresh();
        }

        private void outputSeqToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.outputBoxSequence();
        }

        private void autoSequenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.AutoSequence();
        }

        private void depthType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.glViewer.setDepthType(this.depthType.SelectedIndex);
            this.glViewer.Refresh();
        }


        private void vanishingLineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.vanishingLineType.SelectedIndex < 2)
            {
                this.glViewer.showVanishingLines = true;
                this.glViewer.setVanishingLineDrawType(this.vanishingLineType.SelectedIndex);
            }
            else
            {
                this.glViewer.showVanishingLines = false;
            }
            this.glViewer.Refresh();
        }

        private void saveViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "MAT file (*.mat)|*.mat;|All Files(*.*)|*.*";
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = dialog.FileName;
                this.glViewer.writeModelViewMatrix(filename);
            }
        }

        private void loadViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "MAT file (*.mat)|*.mat;|All Files(*.*)|*.*";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = dialog.FileName;
                this.glViewer.readModelModelViewMatrix(filename);
                this.glViewer.Refresh();
            }
        }

        private void saveAs3D_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "3D model (*.obj; *.off; *.ply)|*.obj; *.off; *.ply|All Files(*.*)|*.*";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = dialog.FileName;
            }
        }

        private void lockView_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.lockView = this.lockView.Checked;
            this.Refresh();
        }

        private void exampleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // test 1
            this.glViewer.inExample = !this.glViewer.inExample;
            // test 2
            //this.glViewer.showAllFaceToDraw = !this.glViewer.showAllFaceToDraw;
            //if (this.glViewer.showAllFaceToDraw)
            //{
            //    this.glViewer.activateAllGuidelines();
            //}
            //else
            //{
            //    this.glViewer.deActivateAllGuidelines();
            //}
            // test 3
            this.glViewer.twoBoxExample();
            this.glViewer.Refresh();
        }

        private void showSpecificFacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.glViewer.activateSpecificEdges();
            this.glViewer.inExample = !this.glViewer.inExample;
            this.glViewer.setSpecificFace();
            this.glViewer.Refresh();
        }

        private void vanishingPoint1_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showVanishingRay1 = this.vanishingPoint1.Checked;
            this.glViewer.Refresh();
        }

        private void vanishingPoint2_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showVanishingRay2 = this.vanishingPoint2.Checked;
            this.glViewer.Refresh();
        }

        private void showBoxVanlines_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showBoxVanishingLine = this.showBoxVanlines.Checked;
            this.glViewer.Refresh();
        }

        private void showGuideLineVanlines_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showGuideLineVanishingLine = this.showGuideLineVanlines.Checked;
            this.glViewer.Refresh();
        }

        private void showFaceToDraw_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showFaceToDraw = this.showFaceToDraw.Checked;
            this.glViewer.Refresh();
        }

        private void contourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.computeContours();
            this.glViewer.Refresh();
        }

        private void moveBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.moveBox();
            this.glViewer.Refresh();
        }

        private void sharpEdge_CheckedChanged(object sender, EventArgs e)
        {
            //this.glViewer.showSharpEdge = this.sharpEdge.Checked;
            this.glViewer.showDrawnStroke = this.sharpEdge.Checked;
            this.glViewer.Refresh();
        }

        private void enableHiddenCheck_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.enableHiddencheck = this.enableHiddenCheck.Checked;
            if (!this.glViewer.enableHiddencheck)
            {
                this.glViewer.recoverHiddenlines();
            }
            this.glViewer.Refresh();
        }

        private void loadTriMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "3D model (*.obj; *.off; *.ply)|*.obj; *.off; *.ply|All Files(*.*)|*.*";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = dialog.FileName;
                // load mesh
                this.glViewer.loadTriMesh(filename);
                // set tab page
                TabPage tp = new TabPage(Path.GetFileName(filename));
                this.fileNameTabs.TabPages.Add(tp);
                this.fileNameTabs.SelectedTab = tp;
            }
            this.glViewer.Refresh();
        }

        private void showSegContour_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showSegContour = this.showSegContour.Checked;
            this.glViewer.computeContours();
            this.glViewer.Refresh();
        }

        private void showSegSilhouette_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showSegSilhouette = this.showSegSilhouette.Checked;
            this.glViewer.computeContours();
            this.glViewer.Refresh();
        }

        private void showSegSuggestiveContour_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showSegSuggestiveContour = this.showSegSuggestiveContour.Checked;
            this.glViewer.computeContours();
            this.glViewer.Refresh();
        }

        private void showSegApparentRidge_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showSegApparentRidge = this.showSegApparentRidge.Checked;
            this.glViewer.computeContours();
            this.glViewer.Refresh();
        }

        private void sketchyDegreeTrackBar_Scroll(object sender, EventArgs e)
        {
            
            double rate = (double)this.sketchyDegreeTrackBar.Value / this.sketchyDegreeTrackBar.Maximum;
            rate /= 3;
            this.glViewer.setStrokeSketchyRate(rate);
            this.glViewer.Refresh();
        }

        private void lineOrMesh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lineOrMesh.SelectedIndex == 0)
            {
                this.glViewer.showLineOrMesh = true;

            }
            else
            {
                this.glViewer.showLineOrMesh = false;
            }
            this.glViewer.Refresh();
        }

        private void exportSequenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.exportSequenceDiv();
        }

        private void showSegbundary_CheckedChanged(object sender, EventArgs e)
        {
            this.glViewer.showSegBoundary = this.showSegbundary.Checked;
            this.glViewer.computeContours();
            this.glViewer.Refresh();
        }

        private void sketchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.setUIMode(5);
        }

        private void saveSketchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "sketch file (*.sketch)|*.sketch|All Files(*.*)|*.*";
            //if (dialog.ShowDialog(this) == DialogResult.OK)
            //{
            //    string filename = dialog.FileName;
            //    this.glViewer.saveSketches(filename);
            //}
            this.glViewer.saveSketches("");
        }

        private void eraserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.setUIMode(6);
        }

        private void clearAllStrokes_Click(object sender, EventArgs e)
        {
            this.glViewer.clearAllStrokes();
            this.glViewer.Refresh();
        }

        private void loadSketchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "sketch file (*.sketch)|*.sketch|All Files(*.*)|*.*";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string filename = dialog.FileName;
                this.glViewer.loadSketches(filename);
                this.glViewer.Refresh();
            }
        }

        private void screenCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.glViewer.captureScreen(this.glViewer.getSequenceNumber());
        }

        private void moveCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.glViewer.setUIMode(7);
            //this.glViewer.zoonIn = !this.glViewer.zoonIn;
            this.glViewer.Refresh();
        }

        //########## inset view ##########//

        public void addInsetViewer(InsetViewer viewer)
        {
            this.viewPanel.Panel2.Controls.Add(viewer);
            viewer.BringToFront();
        }

        public void setLineType(string s)
        {
            this.lineTypeLabel.Text = s;
            this.Refresh();
        }

        public void setLineTypeLabelLoc(int x, int y)
        {
            this.lineTypeLabel.Location = new Point(x, y);
        }

	}
}
