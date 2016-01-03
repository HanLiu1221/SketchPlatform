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
            dialog.SelectedPath = "D:\\Projects\\sketchingTutorial\\SketchPlatform\\Data\\segments";
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
        //    this.glViewer.setStrokeStylePerSeg(size);
        //    this.glViewer.Refresh();
        //}

        private void strokeSizeBox_TextChanged(object sender, EventArgs e)
        {
            if (this.strokeSizeBox.Text == null || this.strokeSizeBox.Text.Length == 0)
                return;
            int size = Int32.Parse(this.strokeSizeBox.Text);
            this.glViewer.setStrokeStylePerSeg(size);
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
            this.glViewer.prevSequence();
            this.glViewer.Refresh();
        }

        private void sequenceNextButton_Click(object sender, EventArgs e)
        {
            this.glViewer.nextSequence();
            this.glViewer.Refresh();
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
            if (this.vanishingLineType.SelectedIndex > 0)
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

        private void redoSequence_Click(object sender, EventArgs e)
        {
            this.glViewer.redoSequence();
            this.glViewer.Refresh();
        }
	}
}
