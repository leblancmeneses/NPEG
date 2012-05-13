#include <cassert>
#include <cstdio>
#include <cstring>

#include "robusthaven/text/StringInputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;

#define TESTSTRING1 "test"
#define TESTSTRING2 "TEST"
#define OTHERSTRING "somthing else"

class _AndTest : public Npeg {
public:
  int sub_expression1() throw (ParsingFatalTerminalException) {
    return literal(TESTSTRING1, strlen(TESTSTRING1), 1);
  } 

  int sub_expression2() throw (ParsingFatalTerminalException) {
    return literal(TESTSTRING1, strlen(TESTSTRING1), 0);
  } 

  int expression1() throw (ParsingFatalTerminalException) {
    return andPredicate((Npeg::IsMatchPredicate)&_AndTest::sub_expression1) && sub_expression2();
  }

  int expression2() throw (ParsingFatalTerminalException) {
    return andPredicate((Npeg::IsMatchPredicate)&_AndTest::sub_expression2) && sub_expression1();
  }

  int isMatch() throw (ParsingFatalTerminalException) {
    return 1;
  }

public:
  _AndTest(InputIterator *iterator) : Npeg(iterator) {}
};

/*
 * With all tests, also test commutativity.
 * - Does AND handle the empty string properly?
 * - Does AND only consume the string once?
 * - Does and consume nothing if one predicate is false?
 * - Does AND still work properly in the middle of a string?
 */
int main(int argc, char *argv[]) 
{
  const char emptystring[] = "";
  const char test1string[] = TESTSTRING1"blah";
  const char not_test1string[] = TESTSTRING2"blah";
  const char test2string[] = TESTSTRING1 TESTSTRING1 TESTSTRING2"blah";
  const char middle_test2string[] = OTHERSTRING TESTSTRING1 TESTSTRING1 TESTSTRING2"blah";
  const char errmsg[] = "some kind of error";

  StringInputIterator *p_iterator;
  _AndTest *p_context;
  
  p_iterator = new StringInputIterator(emptystring, 0);  
  p_context = new _AndTest(p_iterator);
  assert(p_context->expression1() == 0 && p_iterator->getIndex() == 0);
  assert(p_context->expression2() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of empty string.\n");
  delete p_context;
  delete p_iterator;  

  p_iterator = new StringInputIterator(test1string, strlen(test1string));
  p_context = new _AndTest(p_iterator);
  assert(p_context->expression1() == 1 && p_iterator->getIndex() == strlen(TESTSTRING1));
  p_iterator->setIndex(0);
  assert(p_context->expression2() == 1 && p_iterator->getIndex() == strlen(TESTSTRING1));
  printf("\tVerified: Handling of single occurence string.\n");
  delete p_context;
  delete p_iterator;

  p_iterator = new StringInputIterator(not_test1string, strlen(not_test1string));
  p_context = new _AndTest(p_iterator);
  assert(p_context->expression1() == 0 && p_iterator->getIndex() == 0);
  assert(p_context->expression2() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of single occurence string.\n");
  delete p_context;
  delete p_iterator;

  p_iterator = new StringInputIterator(test2string, strlen(test2string));
  p_context = new _AndTest(p_iterator);
  assert(p_context->expression1() == 1 && p_iterator->getIndex() == strlen(TESTSTRING1));
  p_iterator->setIndex(0);
  assert(p_context->expression2() == 1 && p_iterator->getIndex() == strlen(TESTSTRING1));
  printf("\tVerified: Handling of double occurence string.\n");
  delete p_context;
  delete p_iterator;

  p_iterator = new StringInputIterator(middle_test2string, strlen(middle_test2string));
  p_context = new _AndTest(p_iterator);
  p_iterator->setIndex(strlen(OTHERSTRING));
  assert(p_context->expression1() == 1 
	 && p_iterator->getIndex() == strlen(OTHERSTRING) + strlen(TESTSTRING1));
  p_iterator->setIndex(strlen(OTHERSTRING));
  assert(p_context->expression2() == 1 
	 && p_iterator->getIndex() == strlen(OTHERSTRING) + strlen(TESTSTRING1));
  printf("\tVerified: Handling of double occurence at center of string.\n");
  delete p_context;
  delete p_iterator;

  return 0;
}
