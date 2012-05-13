#include <cstdio>
#include <cassert>
#include <iostream>
#include <cstring>
#include "robusthaven/text/StringInputIterator.h"
#include "SimpleXml.h"

using namespace RobustHaven::Text;

int main(int argc, char *argv[])
{
#define BUFFER_SIZE 100

  const char* stream = "<h1>hello</h1><h2>hello</h2>";

  InputIterator* input = new StringInputIterator(stream, strlen(stream));
  SimpleXml* parser = new SimpleXml(input);
  char buffer[BUFFER_SIZE];
  AstNode *ast, *p_child;

  assert(parser->isMatch());
  printf("\tVerified: The expected input was matched by parser.\n");

  ast = parser->getAST();
  assert(0 == strcmp(ast->getToken()->getName().c_str(), "Expression"));
  printf("\tVerified: The expected token name: %s.\n", ast->getToken()->getName().c_str());

  input->getText(buffer, ast->getToken()->getStart(), ast->getToken()->getEnd());
  assert(0 == strcmp(buffer, stream));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  assert(ast->getChildren().size() == 6);
  puts("\tVerified: Expected number of children.");

  p_child = ast->getChildren()[0];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "START_TAG"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "<h1>"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[1];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "Body"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "hello"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[2];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "END_TAG"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "</h1>"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[3];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "START_TAG"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "<h2>"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[4];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "Body"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "hello"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[5];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "END_TAG"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "</h2>"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  AstNode::deleteAST(ast);
  delete parser;
  delete input;

  return 0;
}
