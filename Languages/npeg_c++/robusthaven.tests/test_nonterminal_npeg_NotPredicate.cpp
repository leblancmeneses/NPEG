#include <cassert>
#include <cstdio>
#include <cstring>
#include "robusthaven/text/Npeg.h"
#include "robusthaven/text/StringInputIterator.h"

using namespace RobustHaven::Text;

#define TESTSTRING "test"
#define OTHERSTRING "somthing else"

class _NotTest : public Npeg {
private:  
  int _expression() throw (ParsingFatalTerminalException) {
    return literal(TESTSTRING, strlen(TESTSTRING), 1);
  } 

public:
  int isMatch() throw (ParsingFatalTerminalException) {
    return notPredicate((Npeg::IsMatchPredicate)&_NotTest::_expression);
  }

public:
  _NotTest(InputIterator *iter) : Npeg(iter) {}
};

/*
 * Test that NOT never consumes anything, and that matching predicates are properly inverted.
 */
int main(int argc, char *argv[]) 
{
  const char emptystring[] = "";
  const char test1string[] = TESTSTRING"blah";
  const char not_test1string[] = OTHERSTRING"blah";
  const char test2string[] = TESTSTRING TESTSTRING"blah";
  const char middle_test2string[] = OTHERSTRING TESTSTRING TESTSTRING"blah";
  const char errmsg[] = "some kind of error";

  StringInputIterator *p_iterator;
  _NotTest *p_context;

  p_iterator = new StringInputIterator(emptystring, 0);
  p_context = new _NotTest(p_iterator);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == 0);
  printf("\tVerified: operation on empty string\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(test1string, strlen(test1string));
  p_context = new _NotTest(p_iterator);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: operation on single occurrence of string\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(not_test1string, strlen(not_test1string));
  p_context = new _NotTest(p_iterator);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == 0);
  printf("\tVerified: operation with non-occurrence of string\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(test2string, strlen(test2string));
  p_context = new _NotTest(p_iterator);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: operation with non-occurrence of string\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(test2string, strlen(test2string));
  p_context = new _NotTest(p_iterator);
  p_iterator->setIndex(strlen(TESTSTRING)/2);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == strlen(TESTSTRING)/2);
  printf("\tVerified: operation with non-occurrence of string & iterator in middle of string\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(middle_test2string, strlen(middle_test2string));
  p_context = new _NotTest(p_iterator);
  p_iterator->setIndex(strlen(OTHERSTRING));
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == strlen(OTHERSTRING));
  printf("\tVerified: operation with single occurrence of string & iterator in middle of string\n");
  delete p_iterator; delete p_context;

  return 0;
}
