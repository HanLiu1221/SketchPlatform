﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace GraphicsPlatform
{
	public partial class Interface : Form
	{
		public Interface()
		{
			InitializeComponent();
		}

        private void open3D_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "3D model (*.obj; *.off)|*.obj; *.off|All Files(*.*)|*.*";
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
                int[] stats = this.glViewer.getStatistics();
                this.statistics.Text = "# number of vertex: " + stats[0] + "\n"
                    + "# number of edges: " + stats[1] + "\n"
                    + "# number of faces: " + stats[2] + "\n";
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
	}
}
