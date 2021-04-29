using System;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetworkController;
using SpreadsheetUtilities;
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
        /// <summary>
        /// The model behind our spreadsheet that holds our cells, 
        /// dependencies, and handles arithmetic calculations.
        /// </summary>
        private Spreadsheet s;


        /// <summary>
        /// Private integers to keep track of column and row.
        /// </summary>
        private int col; private int row;


        /// <summary> Event where we update our text boxes that show 
        /// the current cell's:
        /// Name, Value, and Contents.
        /// </summary>
        public event UpdateTextBoxes UpdateTextBoxes;


        /// <summary>
        /// Public default Spreadsheet constructor.
        /// </summary>
        public SpreadsheetController()
        {
            s = new Spreadsheet(IsValid, Normalize, "ps6");
        }


        /// <summary>
        /// This method is called whenever a new cell is selected
        /// </summary>
        /// <param name="panel">Passed in Spreadsheet Panel</param>
        public void OnSelectionChanged(SpreadsheetPanel panel)
        {
            panel.GetSelection(out int col, out int row);
            UpdateTextBoxes(GetCellName(col, row));
            this.col = col;
            this.row = row;

            // Send to server
            string json = @"{""requestType"": ""selectCell"", ""cellName"":" + @" "" " + GetCellName(col, row) + @" "" " + "}";
            //Network.commandQueue.Enqueue(json);

            if (Network.server != null)
            {
                Networking.Send(Network.server.TheSocket, json);
            }
            else
            {
                return;
            }
        }


        /// <summary>
        /// Gets cell value from Spreadsheet cell.
        /// </summary>
        /// <param name="name">Cell Name</param>
        /// <returns></returns>
        public object GetCellValue(string name)
        {
            return s.GetCellValue(name);
        }


        /// <summary>
        /// Gets cell content from Spreadsheet cell.
        /// </summary>
        /// <param name="name">Cell Name</param>
        /// <returns></returns>
        public object GetCellContents(string name)
        {
            return s.GetCellContents(name);
        }


        /// <summary>
        /// Sets the contents of the cell in the Spreadsheet.
        /// </summary>
        /// <param name="contents">Contents of the cell</param>
        /// <returns></returns>
        public IEnumerable<string> SetCellContents(string contents)
        {
            // Send to server
            string json = @"{""requestType"": ""editCell"", ""cellName"":" + @"""" +
                            GetCellName(col, row) + @"""," + @"""contents"": " +
                            @"""" + contents + @"""" + "}";

            //Network.commandQueue.Enqueue(json);
            if (Network.server != null)
                Networking.Send(Network.server.TheSocket, json);

            IEnumerable<string> res = s.SetContentsOfCell(GetCellName(col, row), contents);
            return res;

        }


        /// <summary>
        /// Saves the current spreadsheet.
        /// </summary>
        /// <param name="fileName">File to save</param>
        public void SaveSpreadsheet(string fileName)
        {
            s.Save(fileName);
        }


        /// <summary>
        /// Loads an existing spreadsheet.
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        public Spreadsheet LoadSpreadsheet(string filePath)
        {
            return s = new Spreadsheet(filePath, IsValid, Normalize, "ps6");
        }


        /// <summary>
        /// Checks if the spreadsheet has changed.
        /// </summary>
        /// <returns>True is Spreadsheet changed, false otherwise</returns>
        public bool SpreadsheetHasChanged()
        {
            return s.Changed;
        }


        /// <summary>
        /// Private helper method to use in our controller to parse and retrieve our
        /// cell name, given the cell's column and rown with account for zero-based indexing.
        /// </summary>
        /// <param name="col">Column of current cell</param>
        /// <param name="row">Row of current cell</param>
        /// <returns>Returns the current selected cell name</returns>
        private string GetCellName(int col, int row)
        {
            string columnString = "";
            decimal columnNumber = col;

            // Loops while the column number is greater than or equal to zero 
            // and gets the current cell name from the column and the row.
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




        /// <summary>
        /// Is valid delegate for the Spreadsheet constructor.
        /// </summary>
        /// <param name="s">String to check</param>
        /// <returns>True if valid cell name, else false.</returns>
        private bool IsValid(string s)
        {
            return Regex.IsMatch(s, @"^[a-zA-Z]*[0-9]+$");
        }


        /// <summary>
        /// Normalize delegate for the Spreadsheet constructor. 
        /// </summary>
        /// <param name="s">String to normalize</param>
        /// <returns>The cell name to all upper case.</returns>
        private string Normalize(string s)
        {
            return s.ToUpper();
        }

        /// <summary>
        /// This method sends an undo request to the server in the format of
        ///     - {requestType: "undo"}
        /// </summary>
        public void SendUndo()
        {

            string json = @"{""requestType"": ""undo"" }";

            if (Network.server != null)
            {
                Networking.Send(Network.server.TheSocket, json);
            }

            else
            {
                return;
            }
        }

        /// <summary>
        /// This method sends a revert request to the server in the format of
        /// - { requestType: "revertCell", cellName: "<cellname>"}
        /// </summary>
        public void SendRevert()
        {
            string json = @"{""requestType"": ""revertCell"",""cellName"":" + @"""" +
                            GetCellName(col, row) + @""" }";

            if (Network.server != null)
            {
                Networking.Send(Network.server.TheSocket, json);
            }

            else
            {
                return;
            }
        }
    }
}
