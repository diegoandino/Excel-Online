Entry Date #1: October 7th, 2020.
-----------------------------------------------------
Authors: Tarik Vu (u0759288) and Diego Andino (u1075562)

Resources: 
	- Microsoft: 
		- OpenFileDialog Class: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.openfiledialog?view=netcore-3.1
		- SaveFileDialog Class: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.savefiledialog?view=netcore-3.1
		- PS6 Skeleton Demo: https://github.com/uofu-cs3500-fall20/Examples/tree/master/PS6Skeleton

Design Decisions: 
	- Managed to keep the access to the rows and columns of the Spreadsheet O(1) by utilizing a Dictionary<K, V> where key is of 
	  type string, that holds the cell name, and value of type integer array that holds two values: { column, row }. Every time a 
	  new cell is added to the Spreadsheet, it also gets mapped with a name, column and a row so it can be accessed at any point
	  for updating and recalculating. 

	- Followed the MVC pattern by adding a SpreadsheetController class to be able to separate concerns of the functionality of the 
	  Spreadsheet. Our main backing Spreadsheet object is held in the SpreadsheetController class as well as all of the methods it 
	  uses. 

	- Getting the cell names correctly by using zero-based indexing was done by using a private helper method called GetCellName(col, row)
	  that returns the cell name of the current selected row and column (e.g GetCellName(0, 0) will be A1). It is used to set the contents 
	  of the cell by passing in the current row and column. 

Challenges of PS6:
	- Parsing:  We had a rather difficult time discussing how we would efficiently  parse cell names in order to retrieve their appropriate rows and columns.

	- Recalculating cells with formula errors was difficult and took us a couple of extra hours until we decided to take a break and look over ps5 grading tests.  Since Tarik's code passed more
	  of the grading tests we decided to swap out our ps5 code.


Entry Date #2: October 8th, 2020.
-----------------------------------------------------
How to use the Spreadsheet:
	- To ADD contents to a cell, select any cell by clicking on it and type in the contents of the cell (it can be a formula, a string or a value) and hitting enter.
	- To ADD a formula, you can click on a cell and begin the formula with the "=" character and then type in an operation (e.g =A1 + 2 or =10 * 10).
	- To DELETE contents from a cell, click on the cell and just delete the contents by backspacing or selecting all of the contents then hitting enter.
	- To OPEN an existing Spreadsheet file (of type ".sprd"), go into the File menu on the top left and select "Open". It will prompt you to look for the existing Spreadsheet file.
	- To SAVE a Spreadsheet, go into the File menu on the top left and click "Save", it will prompt you with a window to choose a location to save your file into.
	
	Extra Features:
		- To PRINT a Spreadsheet, go into the File menu on the top left and click "Print". It will print the current open Spreadsheet directly to your connected printer.
		- Resource for printing: https://docs.microsoft.com/en-us/dotnet/api/system.drawing.printing.printdocument?redirectedfrom=MSDN&view=dotnet-plat-ext-3.1
		
		- Added a live clock to the Spreadsheet using the Timer object.
		- Resource for live clock: https://stackoverflow.com/questions/17832021/constantly-update-current-time-display-on-a-windows-form
