using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Net.Http.Headers;
using System.Windows.Forms;
using NetworkController;

namespace SS
{

    /// <summary>
    /// This class is the View of our Spreadsheet program.  It will utilize our model (Spreadsheet) 
    /// through event handlers where our model will handle cell data as well as dependenceis and arithmetic
    /// calculations.  
    /// @Authors Diego Andino & Tarik Vu
    /// 
    /// Ver 1.1 (4/11/2021)
    /// - Modified to initiate connections to server
    /// 
    /// </summary>
    /// Testing push for networking
    public partial class SpreadsheetForm : Form
    {
        /// <summary> Private Controller to manage Spreadsheet.</summary>
        private SpreadsheetController controller;

        /// <summary> Private helper dictionary to store cells and their index
        /// by rows and columns. </summary>
        private Dictionary<string, int[]> cells;

        /// <summary> The name of our default cell. </summary>
        private static readonly string DefaultCell = "A1";

        /// <summary>  Bitmap to use for printing feature. </summary>
        private Bitmap memoryImage;

        private List<string> listOfSpreadsheet;
        /// <summary>
        /// Public SpreadsheetForm constructor.
        /// </summary>
        public SpreadsheetForm()
        {
            controller = new SpreadsheetController();
            cells = new Dictionary<string, int[]>();

            // Connection based code:
            Network.ConnectionError += ShowConnectionError;
            Network.Connected += HandleConnected;
            Network.SpreadSheetsArrived += PickASpreadSheet;
            Network.UpdateArrived += UpdateContentsOfCell;

            InitializeComponent();

            // Disable cells until connection is made
            MainPanel.Enabled = false;
            CellContentsBox.Enabled = false;

        }


        /// <summary>
        /// This method is called upon successful connection from the Spreadsheet client to the server.
        /// This method takes in an array of spreadsheet names, and sorts them out into a combo box. 
        /// The user will be able to select a spreadsheet from the combo box to request for the server to
        /// send, or send back to the server a request to create a new spreadsheet.
        /// </summary>
        /// <param name="Spreadsheets"></param>
        private void PickASpreadSheet(string[] Spreadsheets)
        {
            listOfSpreadsheet = new List<string>(Spreadsheets);
            this.Invoke(new MethodInvoker(
                () =>
                {
                    // Create the popup:
                    Form prompt = new Form();
                    prompt.Size = new Size(300, 90);
                    prompt.FormBorderStyle = FormBorderStyle.FixedSingle;

                    // 2 buttons: One to request making a spreadsheet, one to confirm selection
                    Button confirm_SS = new Button();
                    Button new_SS = new Button();
                    confirm_SS.Location = new System.Drawing.Point(175, 0);
                    new_SS.Location = new System.Drawing.Point(175, 25);
                    confirm_SS.Text = "Confirm";
                    new_SS.Text = "New";

                    // Add availible spreadsheets into a combobox
                    ComboBox selections = new ComboBox();
                    selections.SelectedText = "--Select--";
                    for (int i = 0; i < Spreadsheets.Length; i++) // rename "test" to "spreadsheets" s
                        if(Spreadsheets[i] != "")
                            selections.Items.Add(Spreadsheets[i]);
                    

                    // Reposition & hook up buttons:
                    new_SS.Click += RequestNew_SS;
                    new_SS.Click += (sender, e) => ClosePrompt(prompt, sender, e);

                    // If no selection was made, "" is sent to Reqest_SS, else we send the selection
                    confirm_SS.Click += (sender, e) => Request_SS(selections.SelectedItem == null ? "" : selections.SelectedItem.ToString(), sender, e);

                    // Closes prompt when a selection was made
                    confirm_SS.Click += (sender, e) => ClosePrompt(prompt, sender, e);
                    new_SS.Click += (sender, e) => ClosePrompt(prompt, sender, e);

                    // Adding controls:                  
                    prompt.Controls.Add(confirm_SS);
                    prompt.Controls.Add(new_SS);
                    prompt.Controls.Add(selections);
                    prompt.Text = "Pick a spreadsheet";

                    prompt.ShowDialog();
                }));
        }

        /// <summary>
        /// This method is invoked when the user requests a new Spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestNew_SS(object s, EventArgs evn)
        {
            // Create the popup:
            Form prompt = new Form();
            prompt.Text = "New Spreadsheet name";
            prompt.Size = new Size(350, 63);
            prompt.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Textbox for inputting a new spreadsheet name
            TextBox t = new TextBox();
            t.Size = new Size(170, 30);

            // Buttons to confirm / cancel 
            Button confirmButton = new Button();
            Button cancelButton = new Button();
            confirmButton.Location = new System.Drawing.Point(175, 0);
            cancelButton.Location = new System.Drawing.Point(250, 0);
            confirmButton.Text = "Confirm";
            cancelButton.Text = "Cancel";

            // Events for buttons:
            cancelButton.Click += (sender, e) => ClosePrompt(prompt, sender, e);
            //cancelButton.Click += (sender, e) => SetCanShowSpreadSheets();
            confirmButton.Click += (sender, e) => ClosePrompt(prompt, sender, e);
            confirmButton.Click += (sender, e) => Request_SS(t.Text, sender, e);

            // Add to the prompt:
            prompt.Controls.Add(t);
            prompt.Controls.Add(confirmButton);
            prompt.Controls.Add(cancelButton);
            prompt.ShowDialog();

        }

        /// <summary>
        /// This method is invoked when a user asks for an existing spreadsheet.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Request_SS(string selection, object sender, EventArgs e)
        {
            
            if (selection.Equals("") || selection.Equals("\n")/* || listOfSpreadsheet.Contains(selection)*/)
            {
                MessageBox.Show("Selection must be non-empty, no newline, and unique");

                EnableConnectInputFields();
                ConnectButton.Text = "Connect";

                return;
            }

            DisableConnectInputFields();
            MessageBox.Show(selection + " Was chosen");

            // Send the name of the spreadsheet to server:
            Network.spreadsheetNameQueue.Enqueue(selection);
        }


        /// <summary>
        /// Method to close prompt after a selection is made to request the server
        /// to send a new spreadsheet or to send and existing one.-
        /// </summary>
        private void ClosePrompt(Form prompt, object sender, EventArgs e)
        {
            //EnableConnectInputFields();
            ConnectButton.Text = "Connect";

            prompt.Close();
        }


        /// <summary>
        /// Upon connection, we setup our spreadsheet.
        /// </summary>
        private void HandleConnected()
        {
            this.Invoke(new MethodInvoker(
                () =>
                {
                    MessageBox.Show("Connected");
                    MainPanel.Enabled = true;
                    CellContentsBox.Enabled = true;
                }));
        }


        /// <summary>
        /// Method used to report any errors that occured.
        /// </summary>
        /// <param name="err">Error to be reported</param>
        private void ShowConnectionError(string error)
        {
            MessageBox.Show(error);

            // Re-enable the controlls so the user can reconnect
            this.Invoke(new MethodInvoker(
                () =>
                {
                    ConnectButton.Text = "Connect";

                    EnableConnectInputFields();
                }));
        }


        /// <summary>
        /// This Method is called after InitializeComponent is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpreadsheetForm_Load(object sender, EventArgs e)
        {
            // Event handling for Selection changed (update textboxes)
            MainPanel.SelectionChanged += controller.OnSelectionChanged;
            controller.UpdateTextBoxes += UpdateTextBoxes;

            // Hooking up Enter button for CellContents
            Button enter = new Button();
            AcceptButton = enter;

            // Hitting enter will send an update request to the server
            enter.Click += SendUpdateCellRequest; 

            // Event handling for when Spreadsheet closes
            this.FormClosing += ClosingWindow;

            // Our Default starting cell A1
            UpdateTextBoxes(DefaultCell);
            MainPanel.Focus();

            // Hook up new Controls (connect button)
            ConnectButton.Click += ConnectClick;

            // convienence for connecting to a local server
            ServerTextBox.Text = "localhost";
        }


        /// <summary>
        /// Upon pressing enter, The Client will send to the server a request to change a specified cell
        /// Here we pass the contents SpreadsheetController.  Our Spreadsheet controller will then handle parsing the
        /// cell name of the requested edit.
        /// </summary>
        private void SendUpdateCellRequest(object sender, EventArgs e)
        {
            controller.SendUpdateRequest(CellContentsBox.Text);
        }

        /// <summary>
        /// This method is called when our user clicks on the connect button 
        /// and tries to connect to a server. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectClick(object sender, EventArgs e)
        {
            // User must input a name
            if (UserNameTextBox.Text.Length == 0)
            {
                MessageBox.Show("Name Cannot Be Empty");
                return;
            }

            // Disable controls to try to connect to Server
            ConnectButton.Enabled = false;
            UserNameTextBox.Enabled = false;
            ServerTextBox.Enabled = false;

            // Try to connect to Server
            try
            {
                Network.Connect(ServerTextBox.Text, UserNameTextBox.Text);
                DisableConnectInputFields();
                ConnectButton.Text = "Connected!";
            }

            // Couldn't connect
            catch
            {
                MessageBox.Show("Connecting to server failed.");
                return;
            }

        }


        /// <summary>
        /// Private method in our view (listener) to update our TextBoxes
        /// showing information about our current cell whenever a new
        /// box in our spreadsheet is selected.
        /// </summary>
        /// <param name="s">name of the cell we are cuurently on.</param>
        private void UpdateTextBoxes(string cellName)
        {
            CellNameBox.Text = cellName;

            CellValueBox.Clear();
            CellContentsBox.Clear();

            try
            {
                CellValueBox.Text = controller.GetCellValue(cellName).ToString();

                // Updating CellContentsBox with either a formula or another content.
                if (controller.GetCellContents(cellName) is Formula)
                    CellContentsBox.Text = controller.GetCellContents(cellName).ToString().Insert(0, "=");

                else
                    CellContentsBox.Text = controller.GetCellContents(cellName).ToString();
            }

            // Cell has not been added to spreadsheet yet, no contents or value to be found.
            catch (ArgumentException)
            {

            }
        }

        /// <summary>
        /// disables the UserNameTextBox, ServerTextBox and ConnectButton
        /// </summary>
        private void DisableConnectInputFields()
        {
            UserNameTextBox.Enabled = false;
            ServerTextBox.Enabled = false;
            ConnectButton.Enabled = false;
        }

        /// <summary>
        /// enables the UserNameTextBox, ServerTextBox and ConnectButton
        /// </summary>
        private void EnableConnectInputFields()
        {
            UserNameTextBox.Enabled = true;
            ServerTextBox.Enabled = true;
            ConnectButton.Enabled = true;
        }

        /// <summary>
        /// This method is called whenever our user enters contents into
        /// our CellContents box.  That (String) contents is then added to our
        /// spreadsheet (model) with the appropriate cell name.
        /// 
        /// This method is called when the user presses ENTER on their keyboard.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateContentsOfCell(string cellName, string cellContents)
        {
            try
            {
                // Send edit request to server
                //IEnumerable<string> CellsToRecalculate = controller.SetCellContents(cellContents);

                IEnumerable<string> CellsToRecalculate = controller.GetCellsToRecalc(cellName);

                // Compute edit request after server approves
                MainPanel.GetSelection(out int col, out int row);

                // Check if new value is a Formula Error.
                if (controller.GetCellValue(CellNameBox.Text) is FormulaError)
                {
                    if (!cells.ContainsKey(CellNameBox.Text))
                        cells.Add(CellNameBox.Text, new int[] { col, row });

                    // Set as Formula Error.
                    MainPanel.SetValue(col, row, CellContentsBox.Text);

                    // Recalculate and set Value box to current value.
                    RecalculateCells(CellsToRecalculate);
                    CellValueBox.Text = controller.GetCellValue(CellNameBox.Text).ToString();
                }

                // Update cells normally.
                else
                {
                    if (!cells.ContainsKey(CellNameBox.Text))
                        cells.Add(CellNameBox.Text, new int[] { col, row });

                    RecalculateCells(CellsToRecalculate);
                    CellValueBox.Text = controller.GetCellValue(CellNameBox.Text).ToString();
                }
            }

            // Create pop-up
            catch (FormulaFormatException)
            {
                MessageBox.Show("Invalid Formula At Cell: " + CellNameBox.Text);
            }

            catch (CircularException)
            {
                MessageBox.Show("Circular Exception At Cell: " + CellNameBox.Text);
            }
        }


        /// <summary>
        /// Private helper method to recalculate cells.
        /// </summary>
        /// <param name="CellsToRecalculate">Input of cells to recalculate</param>
        private void RecalculateCells(IEnumerable<string> CellsToRecalculate)
        {
            foreach (string cell in CellsToRecalculate)
            {
                MainPanel.SetValue(cells[cell][0], cells[cell][1], controller.GetCellValue(cell).ToString());
            }
        }


        /// <summary>
        /// Creates a new blank spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetFormContext.GetFormContext().RunForm(new SpreadsheetForm());
        }


        /// <summary>
        /// Saves current spreadsheet.
        /// This method is based off of the Microsoft C# documentation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "All files (*.*)|*.*|Spreadsheet Files| *.sprd";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {
                    // Code to write the stream goes here.
                    myStream.Close();

                    controller.SaveSpreadsheet(saveFileDialog.FileName);
                }
            }
        }


        /// <summary>
        /// Opens an existing spreadsheet.
        /// This method is based off of the Microsoft C# documentation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "All files (*.*)|*.*|Spreadsheet Files| *.sprd";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }


            // Safety feature when loading spreadsheet
            try
            {
                // Have the file path, overwrite current spreadsheet.
                Spreadsheet newSpreadsheet = controller.LoadSpreadsheet(filePath);

                MainPanel.Clear();
                cells.Clear();

                // Updating cell dictionary and setting the new spreadsheet cells.
                foreach (string cell in newSpreadsheet.GetNamesOfAllNonemptyCells())
                {
                    CellNameToColAndRow(out int col, out int row, cell);
                    cells.Add(cell, new int[] { col, row });

                    MainPanel.SetValue(cells[cell][0], cells[cell][1], controller.GetCellValue(cell).ToString());
                }
            }

            catch (SpreadsheetReadWriteException err)
            {
                MessageBox.Show("Could Not Load Spreadsheet \nError: " + err, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // Resetting to our default cell
            CellNameBox.Text = DefaultCell;
            CellValueBox.Text = controller.GetCellValue(DefaultCell).ToString();
            CellContentsBox.Text = controller.GetCellContents(DefaultCell).ToString();
            MainPanel.SetSelection(0, 0);
        }


        /// <summary>
        /// Private helper method to parse cell names when loading in a spreadsheet.
        /// </summary>
        /// <param name="col">collumn of the cell</param>
        /// <param name="row">row of the cell</param>
        /// <param name="name">name of the cell</param>
        private void CellNameToColAndRow(out int col, out int row, string name)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char firstLetter = name[0];

            col = alphabet.IndexOf(firstLetter);
            row = int.Parse(name.Substring(1, name.Length - 1)) - 1;
        }


        /// <summary>
        /// Closes the current spreadsheet.
        /// 
        /// This method has a safety feature that prompts the user
        /// if the user is closing an unsaved spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (controller.SpreadsheetHasChanged())
            {
                string message = "Current Spreadsheet has unsaved contents. \nWould you like to save?";

                if (MessageBox.Show(message, "Close Application", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    saveToolStripMenuItem_Click(sender, e);

                else
                    Close();
            }

            Close();
        }


        /// <summary>
        /// Helper method for closing window using standard windows close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosingWindow(object sender, FormClosingEventArgs e)
        {
            if (controller.SpreadsheetHasChanged())
            {
                string message = "Current Spreadsheet has unsaved contents. \nWould you like to save?";

                if (MessageBox.Show(message, "Close Application", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    saveToolStripMenuItem_Click(sender, e);

                else
                    e.Cancel = false;
            }
        }


        /// <summary>
        /// Opens README.md (.txt) as Help window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Directory.GetCurrentDirectory();
            string newPath = Path.GetFullPath(Path.Combine(path, @"..\..\..\README.txt"));

            Label label = new Label();
            label.Text = File.ReadAllText(newPath);
            label.Size = new Size(700, 700)
;
            Form form = new Form();
            form.Size = new Size(750, 750);
            form.Controls.Add(label);
            form.ShowDialog();
        }


        /// <summary>
        /// Extra feature to print the spreadsheet.
        /// 
        /// Taken from the Microsoft documentation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to print this spreadsheet?", "Print Spreadsheet", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CaptureScreen();
                printDocument.Print();
                printDocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);
            }
        }


        /// <summary>
        /// Helper method for printing the spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(memoryImage, 0, 0);
        }


        /// <summary>
        /// Helper method for capturing the screen information.
        /// </summary>
        private void CaptureScreen()
        {
            Graphics myGraphics = this.CreateGraphics();
            Size s = this.Size;
            memoryImage = new Bitmap(s.Width, s.Height, myGraphics);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            memoryGraphics.CopyFromScreen(this.Location.X, this.Location.Y, 0, 0, s);
        }


        /// <summary>
        /// Updates the timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            TimeLabel.Text = DateTime.Now.ToLongTimeString();
        }


        /// <summary>
        /// When clicked, our client sends a request to the server to undo the most recent change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoButton_Click(object sender, EventArgs e)
        {
            controller.SendUndo();
        }

        /// <summary>
        /// When clicked, our client sends a request to the server to revert a specific cell. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RevertButton_Click(object sender, EventArgs e)
        {
            controller.SendRevert();
        }
    }
}