#include "pch.h"
#include "../Server/cell_value.h"
#include "../Server/cell_value.cpp"
#include "../Server/cell.h"
#include "../Server/cell.cpp"
#include "../Server/Formula.h"
#include "../Server/Formula.cpp"
#include "../Server/dependency_graph.h"
#include "../Server/dependency_graph.cpp"
#include "../Server/changes_stack.h"
#include "../Server/changes_stack.cpp"
#include "../Server/Spreadsheet.h"
#include "../Server/Spreadsheet.cpp"

#include <string>

#pragma region CellValue Tests

TEST(CellValueTest, TestDefaultConstructor) 
{
	CellValue cell;
	EXPECT_EQ( cell.get_content(), std::string("")  );
	EXPECT_TRUE(cell.is_empty());
}

TEST(CellValueTest, TestConstructor)
{
	std::string s("heyo");
	CellValue cell(s);
	EXPECT_EQ(cell.get_content(), std::string("heyo"));
}

TEST(CellValueTest, TestGetSet)
{
	std::string s("heyo");
	CellValue cell(s);
	EXPECT_EQ(cell.get_content(), std::string("heyo"));
	std::string s2("yo");
	cell.set_content(s2);
	EXPECT_EQ(cell.get_content(), std::string("yo"));
}

TEST(CellValueTest, TestError)
{
	std::string s("FORMULA::ERROR");
	CellValue cell(s);
	EXPECT_EQ(cell.get_content(), std::string("FORMULA::ERROR"));
	EXPECT_TRUE(cell.is_error());
}

TEST(CellValueTest, TestError2)
{
	std::string s("heyo");
	CellValue cell(s);
	EXPECT_EQ(cell.get_content(), std::string("heyo"));
	std::string s2("yo");
	cell.set_content(s2);
	EXPECT_EQ(cell.get_content(), std::string("yo"));
	cell.set_error();
	EXPECT_TRUE(cell.is_error());
}

#pragma endregion

#pragma region Cell Tests

TEST(CellTest, TestDefaultConstructor)
{
	Cell cell;
	EXPECT_EQ(cell.get_cell_content(), std::string(""));
	EXPECT_EQ(cell.get_cell_name(), std::string(""));
	EXPECT_TRUE(cell.is_empty());
}

TEST(CellTest, TestConstructor2)
{
	std::string name("name");
	std::string content("some content");
	Cell cell(name, content);
	EXPECT_EQ(cell.get_cell_content(), std::string("some content"));
	EXPECT_EQ(cell.get_cell_name(), std::string("name"));
	EXPECT_FALSE(cell.is_empty());
}

TEST(CellTest, TestRevert)
{
	std::string name("name");
	std::string content("some content");
	Cell cell(name, content);
	
	cell.set_cell_content(name);
	EXPECT_EQ(cell.get_cell_content(), name);

	EXPECT_EQ(cell.revert(), content);
	EXPECT_EQ(cell.get_cell_content(), content);

	std::string empty("");
	EXPECT_EQ(cell.revert(), empty);
	EXPECT_EQ(cell.get_cell_content(), empty);
}

#pragma endregion

#pragma region Formula Tests

TEST(FormulaTests, TestDefaultConstructor)
{
	Formula f;
	Formula e;
	EXPECT_EQ(std::string(""), f.to_string());
	EXPECT_TRUE(f.equals(e));
}

TEST(FormulaTests, TestConstructor2)
{
	std::string s("2");
	Formula f(s);
	Formula e(s);
	double d = 2;
	EXPECT_EQ(std::to_string(d), f.to_string());
	EXPECT_TRUE(f.equals(e));
}

TEST(FormulaTests, TestConstructorPar)
{
	std::string s("(2)");
	Formula f(s);
	Formula e(s);
	double d = 2;
	EXPECT_EQ("(" + std::to_string(d) + ")", f.to_string());
	EXPECT_TRUE(f.equals(e));
}

TEST(FormulaTests, TestGetVariables)
{
	std::string s("(2)");
	Formula f(s);
	EXPECT_EQ(0, f.get_variables().size());
}

TEST(FormulaTests, TestGetVariables2)
{
	std::string s("(2) + b2 + v42 + s69");
	Formula f(s);
	std::vector<std::string> expected;
	expected.push_back("b2");
	expected.push_back("v42");
	expected.push_back("s69");

	EXPECT_EQ(3, f.get_variables().size());
	EXPECT_EQ(expected, f.get_variables());
}

TEST(FormulaTests, TestFormulaError)
{
	std::string s("(2");
	Formula f(s);

	EXPECT_EQ("FORMULA::ERROR", f.to_string());
}

TEST(FormulaTests, TestEvaluate)
{
	std::string s("2 + 2");
	Formula f(s);
	double d = 4;
	EXPECT_EQ(std::to_string(d), f.evaluate().get_content());
}

TEST(FormulaTests, TestEvaluateChuncky)
{
	std::string s("(5 + 5) * 10");
	Formula f(s);
	double d = 100;
	EXPECT_EQ(std::to_string(d), f.evaluate().get_content());
}

TEST(FormulaTests, TestEvaluateChuncky2)
{
	std::string s("(5 + 5) * 10 / ( 1 + 1)");
	Formula f(s);
	double d = 50;
	EXPECT_EQ(std::to_string(d), f.evaluate().get_content());
}

#pragma endregion

#pragma region DependencyGraphTests

TEST(DependencyGraphTests, TestConstructor)
{
	DependencyGraph t;
	EXPECT_EQ(0, t.get_size());
}

TEST(DependencyGraphTests, SimpleEmptyRemoveTest)
{
	DependencyGraph t;

	std::string x("x");
	std::string y("y");

	t.add_dependency(x, y);
	EXPECT_EQ(1, t.get_size());
	t.remove_dependency(x, y);
	EXPECT_EQ(0, t.get_size());
}

TEST(DependencyGraphTests, TestRemove)
{
	DependencyGraph dg;

	std::string a("a");
	std::string b("b");
	std::string c("c");
	std::string d("d");
	std::string e("e");
	std::string f("f");
	std::string g("g");
	std::string h("h");

	dg.add_dependency(a, b);
	dg.add_dependency(a, c);
	dg.add_dependency(a, d);
	dg.add_dependency(a, e);
	dg.add_dependency(a, f);
	dg.add_dependency(a, g);
	dg.add_dependency(a, h);
	
	EXPECT_EQ(7, dg.get_size());
	
	dg.remove_dependency(a, c);
	dg.remove_dependency(a, e);

	EXPECT_EQ(5, dg.get_size());

}

TEST(DependencyGraphTests, SimpleReplaceTest)
{
	DependencyGraph t;

	std::string x("x");
	std::string y("y");

	t.add_dependency(x, y);
	EXPECT_EQ(1, t.get_size());
	t.remove_dependency(x, y);

	std::vector<std::string> vec;
	t.replace_dependents(x, vec);
}

TEST(DependencyGraphTests, StaticTest)
{
	DependencyGraph t1;
	DependencyGraph t2;

	std::string x("x");
	std::string y("y");

	t1.add_dependency(x, y);
	EXPECT_EQ(1, t1.get_size());
	EXPECT_EQ(0, t2.get_size());
}

TEST(DependencyGraphTests, SizeTest)
{
	DependencyGraph t;

	std::string a("a");
	std::string b("b");
	std::string c("c");
	std::string d("d");

	t.add_dependency(a, b);
	t.add_dependency(a, c);
	t.add_dependency(c, b);
	t.add_dependency(b, d);
	EXPECT_EQ(4, t.get_size());
}

//
//TEST(DependencyGraphTests, StressTest)
//{
//	// Dependency graph
//	DependencyGraph t;
//
//	// A bunch of strings to use
//	const int SIZE = 200;
//	std::string letters[SIZE];
//	for (int i = 0; i < SIZE; i++)
//	{
//		//letters[i] = ("" + (char)('a' + i));
//		letters[i] = std::string("a") + std::to_string(i);
//	}
//
//	// The correct answers
//	std::unordered_set<std::string> dents[SIZE];
//	std::unordered_set<std::string> dees[SIZE];
//	for (int i = 0; i < SIZE; i++)
//	{
//		//dents[i] = new HashSet<string>();
//		
//		dents[i] = std::unordered_set<std::string>();
//		dees[i] = std::unordered_set<std::string>();
//	}
//
//	// Add a bunch of dependencies
//	for (int i = 0; i < SIZE; i++)
//	{
//		for (int j = i + 1; j < SIZE; j++)
//		{
//			t.add_dependency(letters[i], letters[j]);
//			dents[i].insert(letters[j]);
//			dees[j].insert(letters[i]);
//		}
//	}
//
//	// Remove a bunch of dependencies
//	for (int i = 0; i < SIZE; i++)
//	{
//		for (int j = i + 4; j < SIZE; j += 4)
//		{
//			t.remove_dependency(letters[i], letters[j]);
//			dents[i].erase(letters[j]);
//			dees[j].erase(letters[i]);
//		}
//	}
//
//	// Add some back
//	for (int i = 0; i < SIZE; i++)
//	{
//		for (int j = i + 1; j < SIZE; j += 2)
//		{
//			t.add_dependency(letters[i], letters[j]);
//			dents[i].insert(letters[j]);
//			dees[j].insert(letters[i]);
//		}
//	}
//
//	// Remove some more
//	for (int i = 0; i < SIZE; i += 2)
//	{
//		for (int j = i + 3; j < SIZE; j += 3)
//		{
//			t.remove_dependency(letters[i], letters[j]);
//			dents[i].erase(letters[j]);
//			dees[j].erase(letters[i]);
//		}
//	}
//
//	// Make sure everything is right
//	for (int i = 0; i < SIZE; i++)
//	{
//		std::unordered_set<std::string> set1;
//		
//
//		for (std::string s : t.get_dependees(letters[i]))
//		{
//			set1.insert(s);
//		}
//
//		std::unordered_set<std::string> set2;
//		for (std::string s : t.get_dependents(letters[i]))
//		{
//			set2.insert(s);
//		}
//
//		
//		EXPECT_TRUE(dees[i] == set1);
//		EXPECT_TRUE(dents[i] == set2);
//	}
//}

TEST(DependencyGraphTests, TestIndexer)
{
	DependencyGraph dg;

	std::string a("a");
	std::string b("b");
	std::string c("c");
	std::string d("d");
	std::string e("e");

	dg.add_dependency(b, a);
	dg.add_dependency(c, a);
	dg.add_dependency(d, a);
	dg.add_dependency(e, a);

	std::string s("a");
	EXPECT_EQ(dg[s], 4);
}


TEST(DependencyGraphTests, TestIndexerButWithMoreStuff)
{
	DependencyGraph dg;

	std::string a("a");
	std::string b("b");
	std::string c("c");
	std::string d("d");
	std::string e("e");
	std::string f("f");
	std::string g("g");
	std::string h("h");
	std::string i("i");

	dg.add_dependency(b, a);
	dg.add_dependency(c, a);
	dg.add_dependency(d, a);
	dg.add_dependency(e, a);
	dg.add_dependency(f, b);
	dg.add_dependency(g, b);
	dg.add_dependency(h, b);
	dg.add_dependency(i, b);

	
	EXPECT_TRUE(dg[a] == 4);
	EXPECT_TRUE(dg[b] == 4);
}


#pragma endregion

#pragma region StackTests

TEST(StackTests, TestConstructor)
{
	changes_stack stack;
	EXPECT_EQ(0, stack.size());
}

TEST(StackTests, TestPush)
{
	changes_stack stack;
	std::string s("heyo");
	stack.push(s);
	std::string s2("heyo");
	EXPECT_EQ(1, stack.size());
	EXPECT_EQ(s2, stack.peek());
	EXPECT_EQ(s2, stack.pop());
	EXPECT_EQ(0, stack.size());
	EXPECT_TRUE(stack.is_empty());
}

TEST(StackTests, MaxSize)
{
	changes_stack stack;

	for (int i = 0; i < 100; i++)
	{
		std::string s(std::to_string(i));
		stack.push(s);
	}
	
	EXPECT_EQ(100, stack.size());

	std::string s("heyo");
	stack.push(s);
	std::string s2("heyo");

	EXPECT_EQ(100, stack.size());
	EXPECT_EQ(s2, stack.pop());

	stack.clear();
	EXPECT_EQ(0, stack.size());
	EXPECT_TRUE(stack.is_empty());
}

TEST(StackTests, EmptyPeek)
{
	changes_stack stack;
	stack.peek();
	EXPECT_EQ(0, stack.size());
}

TEST(StackTests, EmptyPop)
{
	changes_stack stack;
	std::string s = stack.pop();
	EXPECT_EQ(s, "");
	EXPECT_EQ(0, stack.size());
}

TEST(StackTests, PushPopOrder)
{
	changes_stack stack;
	for (int i = 0; i < 10; i++)
	{
		std::string s(std::to_string(i));
		stack.push(s);
	}

	for (int i = 9; i > 0; i--)
	{
		EXPECT_EQ(std::to_string(i), stack.pop());
	}
}

#pragma endregion

#pragma region SpreadsheetTests

TEST(SpreadsheetTests, Constructor1)
{
	Spreadsheet sp;
	std::string s("spwedsweet UwU");
	EXPECT_EQ(s, sp.get_spreadsheet_name());

	std::vector<std::string> vec;
	EXPECT_EQ(vec, sp.get_nonempty_cells());
}

TEST(SpreadsheetTests, Constructor2)
{
	std::string uwu("UwU");
	Spreadsheet sp(uwu);

	std::string s(uwu);
	EXPECT_EQ(s, sp.get_spreadsheet_name());

	std::vector<std::string> vec;
	EXPECT_EQ(vec, sp.get_nonempty_cells());
}

TEST(SpreadsheetTests, SetCellStr1)
{
	std::string uwu("UwU");
	Spreadsheet sp(uwu);

	std::string name1("a1");
	std::string content1("Tarik is not a weeb");
	sp.set_contents_of_cell(name1, content1);

	std::string name2("a2");
	std::string content2("Tarik is is a weeb");
	sp.set_contents_of_cell(name2, content2);

	std::vector<std::string> vec;
	vec.push_back(name1);
	vec.push_back(name2);

	EXPECT_EQ(vec, sp.get_nonempty_cells());
}

TEST(SpreadsheetTests, SetCellContents1)
{
	std::string uwu("UwU");
	Spreadsheet sp(uwu);

	std::string name1("a1");
	std::string content1("Tarik is not a weeb");
	sp.set_contents_of_cell(name1, content1);

	std::string name2("a2");
	std::string content2("Tarik is is a weeb");
	sp.set_contents_of_cell(name2, content2);

	std::vector<std::string> vec;
	vec.push_back(name1);
	vec.push_back(name2);

	EXPECT_EQ(vec, sp.get_nonempty_cells());
	EXPECT_EQ(content1, sp.get_cell_contents(name1));
	EXPECT_EQ(content2, sp.get_cell_contents(name2));
}

#pragma endregion

