using System;
using System.Windows.Forms;

namespace SketchPlatform
{
	static class Program
	{
		private static Interface formMain = null;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Program.formMain = new Interface();
			Application.Run(Program.formMain);
		}
		static public Interface GetFormMain()
		{
			return Program.formMain;
		}
        static public void SetPageNumber(string s)
        {
            Program.formMain.setPageNumber(s);
        }

        static public void SetPageNumberLocation(int x, int y)
        {
            Program.formMain.setPageNumberLocation(x, y);
        }

        static public void setLockState(bool isLock)
        {
            Program.formMain.setLockState(isLock);
        }

        static public void setInsetViewer(InsetViewer viewer)
        {
            Program.formMain.addInsetViewer(viewer);
        }

        static public void writeLineType(string s)
        {
            Program.formMain.setLineType(s);
        }

        static public void setLineTypeLabelLoc(int x, int y)
        {
            Program.formMain.setLineTypeLabelLoc(x,y);
        }
	}
}
