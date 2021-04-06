using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SS;

namespace SS
{
    /// <summary>
    /// This class represents a controller for the Spreadsheet Panel.
    /// </summary>
    public class SpreadsheetController
    {

        public void OnSelectionChanged(SpreadsheetPanel panel)
        {
            panel.GetSelection(out int col, out int row);
            panel.SetValue(col, row, GetCellName(col, row));
        }


        /// <summary>
        /// Priavate helper method to in our controller to parse and retrieve our
        /// cell name, given the cell's column and rown with account for zero-based indexing.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private string GetCellName(int col, int row)
        {
            string columnString = "";
            decimal columnNumber = col;
            
            while (columnNumber >= 0)
            {
                decimal currentLetterNumber = (columnNumber) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);

                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;

                row++;

                if (columnNumber > 26 || row > 99)
                    throw new ArgumentOutOfRangeException();
            }

            return columnString + row;
        }
    }
}
