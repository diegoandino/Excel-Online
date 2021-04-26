#include "pch.h"
#include "CppUnitTest.h"
#include "../Server/Formula.h"
#include "../Server/dependency_graph.h"
#include "../Server/cell.h"
#include "../Server/cell_value.h"
#include "../Server/Spreadsheet.h"

//#include <string>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace Tester
{
	TEST_CLASS(Tester)
	{
	public:

		TEST_METHOD(TestCellValueConstruct1)
		{
			/*int my_cell;
			Assert::AreEqual(true, true);*/
			//Cell cell = Cell();
			//Spreadsheet* sp = new Spreadsheet(std::string(""));
			//CellValue value = CellValue();
			Cell cell;// = Cell();
		}
	};
}
