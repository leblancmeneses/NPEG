#include <cstdio>
#include <cassert>
#include <iostream>
#include <cstring>
#include "robusthaven/text/StringInputIterator.h"
#include "robusthaven/text/PrintVisitor.h"
#include "MathematicalFormula.h"

using namespace RobustHaven::Text;
using namespace std;

#define BUFFER_SIZE 100

int main(int argc, char *argv[])
{
  const char text1[] = "(1*3+4)/5*93";
  const char text2[] = "9+(9-8)*10";  

  char buffer[BUFFER_SIZE];
  InputIterator* input; 
  MathematicalFormula* context; 
  AstNode *ast, *p_child;

  input = new StringInputIterator(text1, strlen(text1));
  context = new MathematicalFormula(input);
  assert(context->isMatch());
  cout << "\tVerified: Matching of input succeeds\n";
  ast = context->getAST();
  PrintVisitor::printAST(*ast);
  assert(ast->getToken()->getName() == "EXPRESSION");
  printf("\tVerified: The expected token name: '%s'.\n", ast->getToken()->getName().c_str());
  input->getText(buffer, ast->getToken()->getStart(), ast->getToken()->getEnd());
  assert(0 == strcmp(buffer, text1));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  assert(ast->getChildren().size() == 5);
  puts("\tVerified: Expected number of children.");
  p_child = ast->getChildren()[0];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "EXPRESSION"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "1*3+4"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[1];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "/"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
  
  p_child = ast->getChildren()[2];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "5"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[3];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "*"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[4];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "93"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  assert(ast->getChildren()[0]->getChildren().size() == 5);
  puts("\tVerified: Expected number of children.");

  p_child = ast->getChildren()[0]->getChildren()[0];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "1"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[0]->getChildren()[1];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "*"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[0]->getChildren()[2];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "3"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[0]->getChildren()[3];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "+"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[0]->getChildren()[4];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "4"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  AstNode::deleteAST(ast);
  delete context;
  delete input;

  /*
   * Example 2
   */
  input = new StringInputIterator(text2, strlen(text2));
  context = new MathematicalFormula(input);
  assert(context->isMatch());
  printf("\tVerified: The expected input was matched by parser.\n");

  ast = context->getAST();
  assert(0 == strcmp(ast->getToken()->getName().c_str(), "EXPRESSION"));
  printf("\tVerified: The expected token name: '%s'.\n", ast->getToken()->getName().c_str());
  input->getText(buffer, ast->getToken()->getStart(), ast->getToken()->getEnd());
  assert(0 == strcmp(buffer, text2));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  assert(ast->getChildren().size() == 5);
  puts("\tVerified: Expected number of children.");

  p_child = ast->getChildren()[0];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "9"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
  
  p_child = ast->getChildren()[1];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "+"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[2];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "EXPRESSION"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "9-8"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[3];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "*"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[4];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "10"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
  
  assert(ast->getChildren()[2]->getChildren().size() == 3);
  puts("\tVerified: Expected number of children.");

  p_child = ast->getChildren()[2]->getChildren()[0];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "9"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[2]->getChildren()[1];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "-"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->getChildren()[2]->getChildren()[2];
  assert(0 == strcmp(p_child->getToken()->getName().c_str(), "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->getToken()->getName().c_str());
  input->getText(buffer, p_child->getToken()->getStart(), p_child->getToken()->getEnd());
  assert(0 == strcmp(buffer, "8"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  AstNode::deleteAST(ast);
  delete context;
  delete input;

  return 0;
}
