#include "Formula.h"

Formula::Formula(std::string& formula)
{

    //initialization

    //keep track of the paretheses count
    int rightParCount = 0;
    int leftParCount = 0;
    //track follow rules
    bool followsOpeningPar = false;
    bool followsOperator = false;

    tokens = get_tokens(formula);

    //first checks

    if (tokens.size() == 0)
    {
        std::cout << "Formula is empty" << std::endl;
        return;
    }

    if (!on_par_follow_rule(tokens[0]))
    {
        std::cout << "Invalid token at the beginning of the expression" << std::endl;
        return;
    }

    //Following rules and balanced parantheses

    for (int i = 0; i < tokens.size(); i++)
    {
        //this way tokens can be verified
        //in case there is no isValid method passed
        if (!token_is_valid(tokens[i]))
        {
            std::cout << "Invalid token at the beginning of the expression" << std::endl;
            return;
        }

        //Open parenthesis/Operator Following Rule
        //checks the token after an opening parentheses & operators
        if (followsOpeningPar || followsOperator)
        {
            if (!op_par_follow_rule(tokens[i]))
            {
                std::cout << "Invalid token at the beginning of the expression" << std::endl;
                return;
            }

            followsOpeningPar = false;
            followsOperator = false;

        }

        //Extra Following Rule
        // this considers that if the token is not an opening parentheses or an operator
        // then it must be an extra token
        else
        {
            if (!(is_operator(tokens[i]) || tokens[i] == ")") && i != 0)
            {
                std::cout << "a token is not valid" << std::endl;
                return;
            }
        }

        //keeps track of the parentheses' count
        if (tokens[i] == "(")
        {
            leftParCount++;
            followsOpeningPar = true;
        }

        //checks for balanced parentheses
        else if (tokens[i] == ")")
        {
            rightParCount++;
            if (rightParCount > leftParCount)
            {
                std::cout << "\')\' misssing " << std::endl;
                return;
            }
        }
        else if (is_operator(tokens[i]))
        {
            followsOperator = true;
        }

        //adds a normalized double to the the tokens array
        std::string::size_type sz;
        try
        {
            double d = std::stod(tokens[i], &sz);
            tokens[i] = std::to_string(d);
        }
        catch (const std::exception&)
        {
            if (is_variable(tokens[i]))
            {
                if (!is_valid(tokens[i]))
                {
                    std::cout << "a token is not valid" << std::endl;
                    return;
                }
            }
        }


    }
}
