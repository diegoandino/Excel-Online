using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;

namespace SS
{
    /// <summary>
    /// This class handles multiple Spreadsheet instances.
    /// </summary>
    class SpreadsheetFormContext : ApplicationContext
    {
        /// <summary>
        /// Keeps count of the number of open forms.
        /// </summary>
        private int FormCount = 0;


        /// <summary>
        /// Singleton SpreadsheetForm
        /// </summary>
        private static SpreadsheetFormContext FormContext;


        /// <summary>
        /// Default constructor.
        /// </summary>
        private SpreadsheetFormContext()
        {
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Form form)
        {
            // One more form is running
            FormCount++;

            // When this form closes, we want to find out
            form.FormClosed += (o, e) => { if (--FormCount <= 0) ExitThread(); };

            // Run the form
            form.Show();
        }


        /// <summary>
        /// Returns the one DemoApplicationContext.
        /// </summary>
        public static SpreadsheetFormContext GetFormContext()
        {
            if (FormContext == null)
            {
                FormContext = new SpreadsheetFormContext();
            }
            return FormContext;
        }
    }


    /// <summary>
    /// Main Program.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Starting App Context and start the app inside it. 
            SpreadsheetFormContext appContext = SpreadsheetFormContext.GetFormContext();
            appContext.RunForm(new SpreadsheetForm());

            Application.Run(appContext);
        }
    }
}
