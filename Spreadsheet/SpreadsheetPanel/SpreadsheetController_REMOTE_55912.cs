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
    /// Our delegate to update our current cell's Textboxes
    /// </summary>
    /// <param name="name"></param>
    public delegate void UpdateTextBoxes(string cellName);

    

    /// <summary>
    /// This class represents a controller for the Spreadsheet Panel.
    /// </summary>
    public class SpreadsheetController
    {

        

        /// <summary> Event where we update our text boxes that show 
        /// the current cell's:
        /// Name, Value, and Contents.
        /// </summary>
        public event UpdateTextBoxes UpdateTextBoxes;

        /// <summary>
        /// This method is called whenever a new cell is selected
        /// </summary>
        /// <param name="panel"></param>
        public void OnSelectionChanged(SpreadsheetPanel panel)
        {
            panel.GetSelection(out int col, out int row);
            UpdateTextBoxes(GetCellName(col, row));
            
        }

        /// <summary>
        /// This method is called whenever our user enters contents into
        /// our CellContents box.  That (String) contents is then added to our
        /// spreadsheet (model) with the appropriate cell name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateContentsOfCell(object sender, EventArgs e)
        {
            // Update Cell contents here
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
