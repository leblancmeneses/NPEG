#include <iostream>
#include <cassert>
#include <cstdio>
#include <cstring>
#include "robusthaven/text/Npeg.h"
#include "robusthaven/text/StringInputIterator.h"

using namespace std;
using namespace RobustHaven::Text;

#define TESTSTRING1 "test"
#define TESTSTRING2 "TEST"
#define OTHERSTRING "something else"

static const char _name1[] = "1st teststring";
static const char _name2[] = "2nd teststring";
static const char _name3[] = "1/2 sequence";
static const char _name4[] = "2 sequence unreduced";

class _CaptTest : public Npeg {
public:
  int match_expression1() throw (ParsingFatalTerminalException) {
    return literal(TESTSTRING1, strlen(TESTSTRING1), 1);
  } 

  int match_expression2() throw (ParsingFatalTerminalException) {
    return literal(TESTSTRING1, strlen(TESTSTRING1), 0);
  } 

  int capture_expression1() throw (ParsingFatalTerminalException) {
    return capturingGroup((Npeg::IsMatchPredicate)&_CaptTest::match_expression1, _name1, 0, false);
  } 

  int capture_expression2() throw (ParsingFatalTerminalException) {
    return capturingGroup((Npeg::IsMatchPredicate)&_CaptTest::match_expression2, _name2, 0, false);
  } 

  int match_expression12() throw (ParsingFatalTerminalException) {
    return sequence((Npeg::IsMatchPredicate)&_CaptTest::capture_expression1, 
		    (Npeg::IsMatchPredicate)&_CaptTest::capture_expression2);
  }

  int capture_expression3() throw (ParsingFatalTerminalException) {
    return capturingGroup((Npeg::IsMatchPredicate)&_CaptTest::match_expression12, _name3, 0, false);
  } 

  int capture_expression2_red() throw (ParsingFatalTerminalException) {
    return capturingGroup((Npeg::IsMatchPredicate)&_CaptTest::capture_expression2, _name4, 1, false);
  } 

  int forced_error() throw (ParsingFatalTerminalException) {
    return limitingRepetition(4, 3, (Npeg::IsMatchPredicate)&_CaptTest::match_expression12);
  }

  int capture_fatal() throw (ParsingFatalTerminalException) {
    return capturingGroup((Npeg::IsMatchPredicate)&_CaptTest::forced_error, _name4, 1, false);
  }

public:
  int isMatch() throw (ParsingFatalTerminalException) {
    return 1;
  }

public:
  _CaptTest(InputIterator *iterator) : Npeg(iterator) {}
};

int main(int argc, char *argv[]) {
  const char emptystring[] = "";
  const char test1string[] = TESTSTRING1"blah";
  const char test2string[] = TESTSTRING2"blah";
  const char middle_test12string[] = OTHERSTRING TESTSTRING1 TESTSTRING2"blah";
  const char errmsg[] = "some kind of error";

  StringInputIterator *p_iterator;
  _CaptTest *p_context;
  AstNode *p_ast;
  bool errorSeen;

  p_iterator = new StringInputIterator(emptystring, 0);
  p_context = new _CaptTest(p_iterator);
  assert(p_context->capture_expression1() == 0 && p_iterator->getIndex() == 0);
  assert(p_context->capture_expression2() == 0 && p_iterator->getIndex() == 0);
  delete p_iterator;
  delete p_context;
  puts("\tVerified: no change of internal state when operating on empty string");


  p_iterator = new StringInputIterator(middle_test12string, strlen(middle_test12string));
  p_context = new _CaptTest(p_iterator);
  p_iterator->setIndex(strlen(OTHERSTRING)/2);
  assert(p_context->capture_expression1() == 0 && p_iterator->getIndex() == strlen(OTHERSTRING)/2);
  assert(p_context->capture_expression2() == 0 && p_iterator->getIndex() == strlen(OTHERSTRING)/2);
  delete p_iterator;
  delete p_context;
  puts("\tVerified: no change of internal state when no occurence of expression");


  p_iterator = new StringInputIterator(test1string, strlen(test1string));
  p_context = new _CaptTest(p_iterator);
  assert(p_context->capture_expression1() == 1 && p_iterator->getIndex() == strlen(TESTSTRING1));
  p_ast = p_context->getAST();
  assert(p_ast->getToken()->getName() == _name1);
  delete p_iterator;
  delete p_context;
  AstNode::deleteAST(p_ast);
  puts("\tVerified: capturing of simple occurence of expression1");


  p_iterator = new StringInputIterator(test1string, strlen(test1string));
  p_context = new _CaptTest(p_iterator);
  errorSeen = false;
  try {
    p_context->capture_fatal();
  } catch (ParsingFatalTerminalException e) {
    errorSeen = true;
    cout << e.what() << endl;
  }
  assert(errorSeen);
  delete p_iterator;
  delete p_context;
  puts("\tVerified: Abortion on error without modification of state.");  


  p_iterator = new StringInputIterator(test2string, strlen(test2string));
  p_context = new _CaptTest(p_iterator);
  assert(p_context->capture_expression2() == 1 && p_iterator->getIndex() == strlen(TESTSTRING2));
  p_ast = p_context->getAST();
  assert(p_ast->getToken()->getName() == _name2);
  AstNode::deleteAST(p_ast);
  delete p_iterator;
  delete p_context;
  puts("\tVerified: capturing of simple occurence of expression2");


  p_iterator = new StringInputIterator(middle_test12string, strlen(middle_test12string));
  p_context = new _CaptTest(p_iterator);
  p_iterator->setIndex(strlen(OTHERSTRING));
  assert(p_context->capture_expression3() == 1 
	 && p_iterator->getIndex() == strlen(OTHERSTRING) + strlen(TESTSTRING1) + strlen(TESTSTRING2));
  p_ast = p_context->getAST();  
  assert(p_ast->getToken()->getName() == _name3 && p_ast->getChildren().size() == 2);
  AstNode::deleteAST(p_ast);
  delete p_iterator;
  delete p_context;
  puts("\tVerified: capturing of 1/2 sequence");


  p_iterator = new StringInputIterator(test2string, strlen(test2string));
  p_context = new _CaptTest(p_iterator);
  assert(p_context->capture_expression2_red() == 1 && p_iterator->getIndex() == strlen(TESTSTRING2));
  p_ast = p_context->getAST();
  assert(p_ast->getToken()->getName() == _name2 && p_ast->getChildren().size() == 0);
  AstNode::deleteAST(p_ast);
  delete p_iterator;  
  delete p_context;
  puts("\tVerified: capturing of redundant expression with reduction to single node");

  return 0;
}
