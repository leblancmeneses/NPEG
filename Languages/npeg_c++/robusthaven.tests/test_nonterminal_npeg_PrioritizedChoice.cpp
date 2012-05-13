#include <cassert>
#include <cstdio>
#include <cstring>
#include "robusthaven/text/Npeg.h"
#include "robusthaven/text/StringInputIterator.h"

using namespace RobustHaven::Text;

#define TESTSTRING1 "test"
#define TESTSTRING2 "test with extension"
#define OTHERSTRING "somthing else"

class _ChoiceTest : public Npeg {
private:
  int _expression1() throw (ParsingFatalTerminalException) {
    return literal(TESTSTRING1, strlen(TESTSTRING1), 1);
  }

  int _expression2() throw (ParsingFatalTerminalException) {
    return literal(TESTSTRING2, strlen(TESTSTRING2), 1);
  }

public:
  int isMatch() throw (ParsingFatalTerminalException) {
    return prioritizedChoice((Npeg::IsMatchPredicate)&_ChoiceTest::_expression1, 
			     (Npeg::IsMatchPredicate)&_ChoiceTest::_expression2);
  }

  int isMatch2() throw (ParsingFatalTerminalException) {
    return prioritizedChoice((Npeg::IsMatchPredicate)&_ChoiceTest::_expression2, 
			     (Npeg::IsMatchPredicate)&_ChoiceTest::_expression1);
  }

public:
  _ChoiceTest(InputIterator *iterator) : Npeg(iterator) {}
};

/*
 * - Check that the left exression is really given the priority over the right one.
 * - Check operation on various string inputs (empty, multiple occurrences, ...)
 */
int main(int argc, char *argv[]) 
{
  const char emptystring[] = "";
  const char test1string1[] = TESTSTRING1"blah";
  const char test1string2[] = TESTSTRING2"blah";
  const char test1string1_1string2[] = TESTSTRING1 TESTSTRING2"blah";
  const char middle_test1string2[] = OTHERSTRING TESTSTRING2"blah";
  const char errmsg[] = "some kind of error";
  
  StringInputIterator *p_iterator;
  _ChoiceTest *p_context;

  p_iterator = new StringInputIterator(emptystring, 0);
  p_context = new _ChoiceTest(p_iterator);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  assert(p_context->isMatch2() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of empty string.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(middle_test1string2, strlen(middle_test1string2));
  p_context = new _ChoiceTest(p_iterator);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  p_iterator->setIndex(0);
  assert(p_context->isMatch2() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of nonoccurence of in non-empty string.\n");
  delete p_iterator; delete p_context;  

  p_iterator = new StringInputIterator(test1string1, strlen(test1string1));
  p_context = new _ChoiceTest(p_iterator);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == strlen(TESTSTRING1));
  printf("\tVerified: Handling of single occurence of short string.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(test1string2, strlen(test1string2));
  p_context = new _ChoiceTest(p_iterator);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == strlen(TESTSTRING1));
  p_iterator->setIndex(0);
  assert(p_context->isMatch2() == 1 && p_iterator->getIndex() == strlen(TESTSTRING2));
  printf("\tVerified: Handling of single occurence of long string.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(test1string1_1string2, strlen(test1string1_1string2));
  p_context = new _ChoiceTest(p_iterator);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == strlen(TESTSTRING1));
  printf("\tVerified: Handling of single occurence of short string before long string.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(middle_test1string2, strlen(middle_test1string2));
  p_context = new _ChoiceTest(p_iterator);
  p_iterator->setIndex(strlen(OTHERSTRING));
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == strlen(OTHERSTRING) + strlen(TESTSTRING1));
  p_iterator->setIndex(strlen(OTHERSTRING));
  assert(p_context->isMatch2() == 1 && p_iterator->getIndex() == strlen(OTHERSTRING) + strlen(TESTSTRING2));
  printf("\tVerified: Handling of single occurence of short string.\n");
  delete p_iterator; delete p_context;  

  return 0;
}
