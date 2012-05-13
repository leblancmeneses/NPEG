#include <cstdio>
#include <cassert>
#include <iostream>
#include <cstring>
#include "robusthaven/text/StringInputIterator.h"
#include "NpegNode.h"

using namespace RobustHaven::Text;
using namespace std;

#define BUFFER_SIZE 100

int main(int argc, char *argv[])
{
  const char* input1 = "NpEg";
  const char* input2 = ".NET Parsing Expression Grammar";
  const char* input3 = "NET Parsing Expression Grammar";

  char buffer[BUFFER_SIZE];
  InputIterator* input;
  NpegNode* parser;
  AstNode *ast;
  
  input = new StringInputIterator(input1, strlen(input1));
  parser = new NpegNode(input);

  assert(parser->isMatch());
  printf("\tVerified: The expected input was matched by parser.\n");
	
  ast = parser->getAST();
  assert(0 == strcmp(ast->getToken()->getName().c_str(), "NpegNode"));
  printf("\tVerified: The expected token name: '%s'.\n", ast->getToken()->getName().c_str());
	
  input->getText(buffer, ast->getToken()->getStart(), ast->getToken()->getEnd());
  assert(0 == strcmp(buffer, "NpEg"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  AstNode::deleteAST(ast);
  delete parser;
  delete input;


  input = new StringInputIterator(input2, 31);
  parser = new NpegNode(input);
	
  printf("\tReached: parsetree(&iterator, &context) for input2.\n");
  assert(parser->isMatch());
  printf("\tVerified: The expected input was matched by parser.\n");
	
  ast = parser->getAST();
  assert(0 == strcmp(ast->getToken()->getName().c_str(), "NpegNode"));
  printf("\tVerified: The expected token name: '%s'.\n", ast->getToken()->getName().c_str());
	
  input->getText(buffer, ast->getToken()->getStart(), ast->getToken()->getEnd());
  assert(0 == strcmp(buffer, ".NET Parsing Expression Grammar"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  AstNode::deleteAST(ast);
  delete parser;
  delete input;


  input = new StringInputIterator(input3, strlen(input3));
  parser = new NpegNode(input);
  
  printf("\tReached: parsetree(&iterator, &context) for input3.\n");
  assert(!parser->isMatch());
  printf("\tVerified: The expected input would NOT be matched by parser.\n");
	
  ast = parser->getAST();
  assert(ast == NULL);
  printf("\tVerified: Retrieving an ast when not a match; npeg_get_ast would return null.\n");

  delete parser;
  delete input;

  return 0;
}
