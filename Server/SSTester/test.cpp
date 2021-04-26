#include "pch.h"
#include "../Server/cell_value.h"
#include "../Server/cell_value.cpp"
#include "../Server/cell.h"
#include "../Server/cell.cpp"
#include "../Server/Formula.h"
#include "../Server/Formula.cpp"

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
	CellValue cell(std::string("heyo"));
	EXPECT_EQ(cell.get_content(), std::string("heyo"));
}

TEST(CellValueTest, TestGetSet)
{
	CellValue cell(std::string("heyo"));
	EXPECT_EQ(cell.get_content(), std::string("heyo"));
	cell.set_content(std::string("yo"));
	EXPECT_EQ(cell.get_content(), std::string("yo"));
}

TEST(CellValueTest, TestError)
{
	CellValue cell(std::string("FORMULA::ERROR"));
	EXPECT_EQ(cell.get_content(), std::string("FORMULA::ERROR"));
	EXPECT_TRUE(cell.is_error());
}

TEST(CellValueTest, TestError2)
{
	CellValue cell(std::string("heyo"));
	EXPECT_EQ(cell.get_content(), std::string("heyo"));
	cell.set_content(std::string("yo"));
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
	Cell cell(std::string("name"), std::string("some content"));
	EXPECT_EQ(cell.get_cell_content(), std::string("some content"));
	EXPECT_EQ(cell.get_cell_name(), std::string("name"));
	EXPECT_FALSE(cell.is_empty());
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
	Formula f(std::string("2"));
	Formula e(std::string("2"));
	double d = 2;
	EXPECT_EQ(std::to_string(d), f.to_string());
	EXPECT_TRUE(f.equals(e));
}

TEST(FormulaTests, TestConstructorPar)
{
	Formula f(std::string("(2)"));
	Formula e(std::string("(2)"));
	double d = 2;
	EXPECT_EQ("(" + std::to_string(d) + ")", f.to_string());
	EXPECT_TRUE(f.equals(e));
}

TEST(FormulaTests, TestGetVariables)
{
	Formula f(std::string("(2)"));
	EXPECT_EQ(0, f.get_variables().size());
}

TEST(FormulaTests, TestGetVariables2)
{
	Formula f(std::string("(2) + b2 + v42 + s69"));
	std::vector<std::string> expected;
	expected.push_back("b2");
	expected.push_back("v42");
	expected.push_back("s69");

	EXPECT_EQ(3, f.get_variables().size());
	EXPECT_EQ(expected, f.get_variables());
}

TEST(FormulaTests, TestFormulaError)
{
	Formula f(std::string("(2"));

	EXPECT_EQ("FORMULA::ERROR", f.to_string());
}

TEST(FormulaTests, TestEvaluate)
{
	Formula f(std::string("2 + 2"));
	double d = 4;
	EXPECT_EQ(std::to_string(d), f.evaluate().get_content());
}

TEST(FormulaTests, TestEvaluateChuncky)
{
	Formula f(std::string("(5 + 5) * 10"));
	double d = 100;
	EXPECT_EQ(std::to_string(d), f.evaluate().get_content());
}

TEST(FormulaTests, TestEvaluateChuncky2)
{
	Formula f(std::string("(5 + 5) * 10 / ( 1 + 1)"));
	double d = 50;
	EXPECT_EQ(std::to_string(d), f.evaluate().get_content());
}

#pragma endregion

//int main(int argc, char* argv[])
//{
//	testing::InitGoogleTest(&argc, argv);
//	return RUN_ALL_TESTS();
//}