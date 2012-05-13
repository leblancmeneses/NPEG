#include <cstdio>
#include <cassert>
#include <iostream>
#include <cstring>
#include "robusthaven/text/StringInputIterator.h"
#include "PhoneNumber.h"

using namespace RobustHaven::Text;
using namespace std;

#define BUFFER_SIZE 100

int main(int argc, char *argv[])
{
	const char* stream = "123-456-7890";

	char buffer[BUFFER_SIZE];
	InputIterator* input = new StringInputIterator(stream, strlen(stream));
	PhoneNumber* parser = new PhoneNumber(input);
	AstNode *ast, *p_child;

	assert(parser->isMatch());
	printf("\tVerified: The expected input was matched by parser.\n");
	
	ast = parser->getAST();
	assert(0 == strcmp(ast->getToken()->getName().c_str(), "PhoneNumber"));
	printf("\tVerified: The expected token name: %s.\n", ast->getToken()->getName().c_str());
	
	input->getText(buffer, ast->getToken()->getStart(), ast->getToken()->getEnd());
	assert(0 == strcmp(buffer, stream));
	printf("\tVerified: The expected matched string: '%s'.\n", buffer);
  
	assert(ast->getChildren().size() == 3);
	puts("\tVerified: Expected number of children.");

	p_child = ast->getChildren()[0];
	assert(0 == strcmp(p_child->getToken()->getName().c_str(), "ThreeDigitCode"));
	printf("\tVerified: The expected token name: %s.\n", ast->getToken()->getName().c_str());
	input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
	assert(0 == strcmp(buffer, "123"));
	printf("\tVerified: The expected matched string of 1st child: '%s'.\n", buffer);
  
	p_child = ast->getChildren()[1];
	assert(0 == strcmp(p_child->getToken()->getName().c_str(), "ThreeDigitCode"));
	printf("\tVerified: The expected token name: %s.\n", ast->getToken()->getName().c_str());
	input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
	assert(0 == strcmp(buffer, "456"));
	printf("\tVerified: The expected matched string of 2nd child: '%s'.\n", buffer);


	p_child = ast->getChildren()[2];
	assert(0 == strcmp(p_child->getToken()->getName().c_str(), "FourDigitCode"));
	printf("\tVerified: The expected token name: %s.\n", ast->getToken()->getName().c_str());
	input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
	assert(0 == strcmp(buffer, "7890"));
	printf("\tVerified: The expected matched string of 3rd child: '%s'.\n", buffer);

	AstNode::deleteAST(ast);
	delete parser;
	delete input;


	return 0;
}
