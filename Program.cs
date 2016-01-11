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
	}
}
