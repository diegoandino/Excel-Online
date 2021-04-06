using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{

    /// <summary>
    /// This Class represents the beginning internals of our Spreadsheet program.
    /// It utilizes a Dependency graph to keep track of cell dependencies, as well
    /// as using a Formula class to evaluate formulas.
    /// @Author Tarik Vu u0759288 
    /// 9/25/20
    ///  
    /// EDIT: As of 9/29/20 we are on the PS5 Branch! - Tarik Vu
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// Our backing data structure (dictionary) that will contain all of our cells
        /// using their cell name as their key.
        /// </summary>
        private IDictionary<string, Cell> Cells;

        /// <summary>
        /// Our Dependency graph that keeps track of each Cell's Dependency, also used in finding 
        /// circular exceptions.
        /// </summary>
        private DependencyGraph depGraph;

        /// <summary>
        /// Indicates whether or not this SS has changed from either being saved or a new 
        /// if a new cell was added.
        /// </summary>
        public override bool Changed { get; protected set; }


        /// <summary>
        /// Constructor that takes in no arguements to construct an empty spreadsheet.
        /// Using base() we can have lamda's to determine the default returns for our delegates.
        /// In a zero argument construction we are to have isValid always be true, normalize to always
        /// return the string as it was, and our version to be set to the string "default".
        /// 
        /// Validity delegate:  Used to check if a variable name is valid.
        /// Normalize delegate: Used to Standardize (normalize) variable names.
        /// Version:            The spreadsheet's version.
        /// </summary>
        public Spreadsheet()
            : base(v => true, n => n, "default")
        {
            Cells = new Dictionary<string, Cell>();
            depGraph = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Constructor that takes in three arguements to construct an empty spreadsheet.
        /// 
        /// Validity delegate:  Used to check if a variable name is valid.
        /// Normalize delegate: Used to Standardize (normalize) variable names.
        /// Version:            The spreadsheet's version.
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            Cells = new Dictionary<string, Cell>();
            depGraph = new DependencyGraph();
            Changed = false;
        }

        /// <summary>
        /// Constructor that takes in four arguements to construct a spreadsheet.
        /// 
        /// Path:               String representing path to a file after being saved.
        /// Validity delegate:  Used to check if a variable name is valid.
        /// Normalize delegate: Used to Standardize (normalize) variable names.
        /// Version:           The spreadsheet's version.
        /// 
        /// This constructor utilizes XMLDemo's ReadXML method provided by professor kopta.
        /// </summary>
        public Spreadsheet(string path, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            Cells = new Dictionary<string, Cell>();
            depGraph = new DependencyGraph();

            // Useing path to get our desired SS
            string filename = path;
            string cellName = "";
            try
            {
                // Create an XmlReader 
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":

                                    if (!(this.Version.Equals(reader["version"])))
                                        throw new SpreadsheetReadWriteException("Incorrect version");
                                    break; // no more direct info to read for Spreadsheet

                                case "cell":
                                    break; // no more direct info to read on Cell

                                case "name":
                                    reader.Read();
                                    cellName = reader.Value; // Grab cell name
                                    break;

                                case "contents":
                                    reader.Read(); // reader value is now cell contents.
                                    SetContentsOfCell(cellName, reader.Value);
                                    break;
                            }
                        }
                        else // If it's not a start element, it's probably an end element
                        {
                        }
                    }
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Error loading in spreadsheet " + path);
            }

            Changed = false;
        }



        public override object GetCellContents(string name)
        {
            // Normalize our cell name
            string cellName = Normalize(name);

            // Check for name 
            CheckCellName(cellName);

            // Cell doesn't Exist
            if (!Cells.ContainsKey(cellName))
                return "";

            Cell temp = Cells[cellName];
            return temp.GetContents();
        }

        public override object GetCellValue(string name)
        {
            // Normalize our cell name
            string cellName = Normalize(name);

            // If name is null or invalid.
            CheckCellName(cellName);
            if (Cells.ContainsKey(cellName))
            {
                Cell temp = Cells[cellName];


                return temp.GetValue();
            }

            return "";

        }


        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            // Check if we have any cells to return
            if (Cells.Values.Count > 0)
                foreach (Cell c in Cells.Values)
                    if (!c.isEmpty)
                        yield return c.name;

            // Return empty
            yield break;
        }

        public override string GetSavedVersion(string filename)
        {
            try
            {
                // Create an XmlReader 
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    return reader["version"];   // Returning Version                                                            
                            }
                        }
                        else // If it's not a start element, it's probably an end element
                        {
                        }
                    }
                }
                // the case for spreadsheet never happened, there was a bad XML file:
                throw new SpreadsheetReadWriteException("unable to find version for. " + filename);
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Error loading in spreadsheet " + filename + ". Unable" +
                    "to retrive version");
            }
        }

        /// <summary>
        /// This Save method is laid out very similarly to the XML demo provided with an added try - catch
        /// per method specifications.
        /// </summary>
        /// <param name="filename">Name of the file.</param>
        public override void Save(string filename)
        {
            // Setting up XML reader
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            try
            {
                // Create an XmlWriter 
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {

                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");

                    // Version (attribute) of this spreadhsheet
                    writer.WriteAttributeString("version", this.Version);

                    // write the Cells themselves
                    foreach (Cell c in Cells.Values)
                        c.WriteXml(writer);

                    writer.WriteEndElement(); // Ends the spreadsheet block
                    writer.WriteEndDocument();

                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Error saving SS");
            }

            Changed = false;
        }

        protected override IList<string> SetCellContents(string name, double number)
        {
            // Adding as a new cell
            if (!(Cells.ContainsKey(name)))
                Cells.Add(name, new Cell(name, number));

            // overwriting cell 
            else
                Cells[name] = new Cell(name, number);

            // Recalculate dependents
            RecalculateDependents(name);

            List<string> toReturn = GetCellsToRecalculate(name).ToList();
            Changed = true;

            return toReturn;

        }

        protected override IList<string> SetCellContents(string name, string text)
        {

            // Add new empty cell if needed
            if (!(Cells.ContainsKey(name)))
                Cells.Add(name, new Cell(name, text));

            // overwriting a cell with an empty string removes it (piazza @670)
            else if (text.Equals(""))
                Cells.Remove(name);

            // overwriting cell 
            else
                Cells[name] = new Cell(name, text);

            List<string> toReturn = GetCellsToRecalculate(name).ToList();

            // Cells that depended on Cells[name] will now be formula error after recalc
            RecalculateDependents(name);

            Changed = true;

            return toReturn;
        }

        protected override IList<string> SetCellContents(string name, Formula formula)
        {

            foreach (string var in formula.GetVariables())
            {

                if (!IsValid(var))
                    throw new FormulaFormatException("invalid variable " + var);

            }

            // Try Catch in case of Circular Exception
            try
            {
                // Adding dependencies for the variables within formula to name
                foreach (string var in formula.GetVariables())
                    depGraph.AddDependency(var, name);

                // Our list to return as well as check for CircularException
                List<string> toReturn = GetCellsToRecalculate(name).ToList();

                // Adding as a new Cell and grabbing it's value.
                object value;
                try
                {
                    value = formula.Evaluate(lookup);
                }
                catch (ArgumentException)
                {
                    value = new FormulaError();
                }

                if (!(Cells.ContainsKey(name)))
                    Cells.Add(name, new Cell(name, formula, value));

                // Overwriting the Cell to a formula
                else
                    Cells[name] = new Cell(name, formula, value);

                // Recalculate dependents
                RecalculateDependents(name);

                Changed = true;
                return toReturn;
            }

            // Resetting dependencies SS is unchanged.
            catch (CircularException)
            {
                foreach (string var in formula.GetVariables())
                    depGraph.RemoveDependency(var, name);

                // Then we throw the Exception.
                throw new CircularException();
            }

        }

        public override IList<string> SetContentsOfCell(string name, string content)
        {
            // Normalize our cell name
            string cellName = Normalize(name);

            // Name and content check here, removed from other add methods.
            CheckCellName(cellName);
            if (content is null)
                throw new ArgumentNullException();

            // If content parses as double set contents to double.
            double doubleContent;
            bool isDouble = double.TryParse(content, out doubleContent);
            if (isDouble)
                return SetCellContents(cellName, doubleContent);

            // Formula case needs a string after '=', otherwise just the string "="
            if (content.StartsWith("="))
            {
                string remainder = content.Substring(1, content.Length - 1);

                // Normalize the variables in our formula
                remainder = Normalize(remainder);

                // FormulaFormatException would be thrown here:
                Formula f = new Formula(remainder);
                return SetCellContents(cellName, f);
            }

            // Setting as string:
            return SetCellContents(cellName, content);
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            // Check for any Dependents to begin with
            if (depGraph.HasDependents(name))
            {
                return depGraph.GetDependents(name);
            }
            // Otherwise we return a new empty list
            return new List<string>();
        }


        // Private helper methods for Spreadsheet

        /// <summary>
        /// Private helper method that uses our dictionary "Cells" in order to 
        /// lookup a cell's value for evaluation.
        /// This method will throw an arguement exception if:
        /// - A Cell is empty (value is empty string)
        /// - A cell's value is a string / formula error
        /// This in turn will cause an evaluation in the Formula Class of this cell
        /// to result in a return of a FormulaError.
        /// </summary>
        /// <param name="name">Cell Name</param>
        /// <returns>Cell's double value.</returns>
        private double lookup(string name)
        {
            // Looking up a cell that hasnt been added yet, add an empty one.
            if (!Cells.ContainsKey(name))
                Cells.Add(name, new Cell(name, ""));

            Cell cell = Cells[name];

            object value = cell.GetValue();

            // We have a string or FormulaError as the value of our cell.
            if (value is string || value is FormulaError)
                throw new ArgumentException();

            // Else we know our cell value is a double and we can return it as such.
            return (double)value;
        }

        /// <summary>
        /// Private helper method to recalculate the dependents of 
        /// this Cell[name].  This method is called if "name" has dependents that
        /// depend on it's value. And is called in the case where the contents of this
        /// cell were either a double or formula.
        /// </summary>
        /// <param name="name">Name of cell that has dependents</param>
        private void RecalculateDependents(string name)
        {
            // Updating the values
            foreach (string s in GetCellsToRecalculate(name))
            {
                // Check if that cell was removed already, if so skip and just recalc dependents.
                if (Cells.ContainsKey(name))
                {
                    object value;
                    Cell cur = Cells[s];
                    Formula formula = cur.formula;
                    try
                    {
                        // Only recalculating if needed
                        if (!(formula is null))
                        {
                            // Try to evaluate the dependent
                            value = formula.Evaluate(lookup);

                            // Adding the cell back in with its new value.
                            Cells[s] = new Cell(s, formula, value);
                        }
                    }

                    // Dependent (still) couldnt evaluate, give it an Error.
                    catch (ArgumentException)
                    {
                        Cells[s] = new Cell(s, formula, new FormulaError());
                    }
                }
            }
        }

        /// <summary>
        /// UPDATED FOR PS5 DEFINITION
        /// Private helper method called whenever a cell is added.
        /// that check's the Cells Name for validity.
        /// 
        /// Variables for a Spreadsheet are valid if and only if they are one or more letters followed
        /// by one or more digits (numbers) this must now be enforced by the spreadsheet.
        /// 
        /// Throws InvalidNameException when a cell name condition is not met.
        /// </summary>
        /// <param name="name">Cell name to check</param>
        private void CheckCellName(string name)
        {
            // Checking Cell name validity with our delegate:
            if (IsValid(name))
            {

                // Null Check 
                if (name is null || name.Equals(""))
                    throw new InvalidNameException();

                // Start of variable must be letter 
                if (!(Char.IsLetter(name[0])))
                    throw new InvalidNameException();

                // First Letter must be followed by letters or nums
                if (name.Length < 2)
                    throw new InvalidNameException();

                // Regex to determine remaining string Letters followed by numbers
                string remaining = name.Substring(1, name.Length - 1);

                string pattern = "^[a-zA-Z]*[0-9]+$";
                if (!Regex.IsMatch(remaining, pattern))
                    throw new InvalidNameException();
            }
            else
            {
                throw new InvalidNameException();
            }
        }

        /// <summary>
        /// Our Cell Class that will be used in Spreadsheet.  Every time a new cell is created
        /// it's contents are defaulted to an empty string.  
        ///
        /// A Cell Also contains a contents and a value.
        /// Contents can be: String, double, or Formula.
        /// If contents are empty string "" the cell is considered empty.       
        /// Value can be: String, double, or FormulaError.
        /// 
        /// If Contents = String, Value is that string.
        /// If Contents = double, Value is that double.
        /// If Contents = Formula, Value is double or Formula Error as reported by
        /// the evaluate method of Formula Class.
        /// 
        /// our private doubles and strings are used to represent our 
        /// different contents.
        /// 
        /// Values are to be implemented during ps5
        /// value: This private object is to be used to either hold a double
        /// or a Formula error.  It is created when a cell is created using the formula 
        /// constructor.  
        /// </summary>
        private class Cell
        {
            /// <summary> Double Contents of cell (if any)  </summary>
            private double conDouble;

            /// <summary> String Contents of cell (if any) </summary>
            private string conString;

            /// <summary> Formula Contents of cell (if any) </summary>
            public Formula formula { get; private set; }

            // ADDED FOR PS5
            /// <summary> Formula Value to represent double or Formula error </summary>
            private object value;


            /// <summary> The name of this cell, Made immutable with private set. </summary>
            public string name { get; private set; }

            /// <summary> Dictates whether or not this cell is empty. </summary>
            public bool isEmpty { get; private set; }

            /// <summary> Creating a new cell with a name, and it's input double.</summary>
            public Cell(string name, double number)
            {
                this.name = name;
                this.conDouble = number;
                isEmpty = false;
            }

            /// <summary> Creating a new cell with a name, and it's input String.</summary>
            public Cell(string name, string text)
            {
                this.name = name;
                this.conString = text;

                // If the text was an empty string, the cell is empty.
                if (text.Equals(""))
                    isEmpty = true;
                else
                    isEmpty = false;
            }

            /// <summary>
            /// Creates a Cell with the given name, formula, and value.
            /// Value is determined when the cell is first added and can either be
            /// a double, or a FormulaError.
            /// </summary>
            /// <param name="name">Cell's Name</param>
            /// <param name="formula">Cell's Formula</param>
            /// <param name="value">Either a double or formula error.</param>
            public Cell(string name, Formula formula, object value)
            {
                this.name = name;
                this.formula = formula;
                this.value = value;
                isEmpty = false;
            }

            /// <summary>
            /// Returns the contents of this cell by examining which variable was instantiated
            /// using the appropriate constructor.  
            /// </summary>
            /// <returns>Returns the contents of this cell (string, double, or Formula)</returns>
            public object GetContents()
            {
                // Contents were string
                if (!(conString is null))
                    return conString;

                // Contents were Formula
                if (!(formula is null))
                    return formula;

                // Contents were double
                return conDouble;
            }

            /// <summary>
            /// Returns the Value of this cell by examining which variable was instantiated
            /// using the appropriate constructor.  
            /// In the case that the Contents of this cell were a formula, we are to evaluate said
            /// formula to return either a double or a formula error.
            /// </summary>
            /// <returns>Returns the contents of this cell (string, double, or Formula)</returns>
            public object GetValue()
            {
                // Contents were string
                if (!(conString is null))
                    return conString;

                // Contents were Formula return value (double or FormulaError)
                if (!(formula is null))
                    return value;

                // Contents were double
                return conDouble;
            }

            /// <summary>
            /// This method was inspired by the XML Demo provided by Prof. Kopta.  
            /// It is used with an XMLWriter when saving a spreadsheet.  And writes down 
            /// the data of this specific cell including the cell's name and contents.  The
            /// value is not recorded to save on space and is simply reconstructed when loading
            /// in a spreadsheet.
            /// When writing a Formula contents, we prepend "=" as per the save method's specifications.
            /// </summary>
            /// <param name="writer">XML writer used to record this given cell.</param>
            internal void WriteXml(XmlWriter writer)
            {
                writer.WriteStartElement("cell");
                // We use a shortcut to write an element with a single string
                writer.WriteElementString("name", name);

                // Determining the appropriate contents
                string contents;

                // Contents were string
                if (!(conString is null))
                    contents = conString;

                // Contents were Formula 
                else if (!(formula is null))
                    contents = formula.ToString().Insert(0, "=");

                // Contents were double
                else
                    contents = conDouble.ToString();

                writer.WriteElementString("contents", contents);
                writer.WriteEndElement(); // Ends the Cell
            }
        }
    }
}