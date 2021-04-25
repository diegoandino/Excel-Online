using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SS;
namespace SpreadsheetModel
{
    public class ServerSpreadsheet
    {
        private Spreadsheet s;

       
        private string ver = "1";

        /// <summary>
        /// Constructor for the Server spreadsheet
        /// </summary>
        public ServerSpreadsheet()
        {
            s = new Spreadsheet(c => Regex.IsMatch(c, @"^[a-zA-Z]*[0-9]+$"), c => c.ToUpper(), ver);
        }

        public void EditCellToSpreadsheet(string name, string contents)
        {
            s.SetContentsOfCell(name, contents);
        }

        public Spreadsheet GetSpreadsheet()
        {
            return s;
        }
    }
}
