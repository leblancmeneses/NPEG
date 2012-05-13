#include <cassert>
#include <cstdio>
#include <cstring>
#include "robusthaven/text/Npeg.h"
#include "robusthaven/text/StringInputIterator.h"

using namespace RobustHaven::Text;

#define TESTSTRING1 "test1"
#define TESTSTRING2 "test__2"
#define OTHERSTRING "something else"

class _SeqTest : public Npeg {
private:
  int _expression1() throw (ParsingFatalTerminalException) {
    return literal(TESTSTRING1, strlen(TESTSTRING1), 1);
  }

  int _expression2() throw (ParsingFatalTerminalException) {
    return literal(TESTSTRING2, strlen(TESTSTRING2), 1);
  }

public:
  int isMatch() throw (ParsingFatalTerminalException) {
    return sequence((Npeg::IsMatchPredicate)&_SeqTest::_expression1, 
		    (Npeg::IsMatchPredicate)&_SeqTest::_expression2);
  }

  int isMatch2() throw (ParsingFatalTerminalException) {
    return sequence((Npeg::IsMatchPredicate)&_SeqTest::_expression2, 
		    (Npeg::IsMatchPredicate)&_SeqTest::_expression1);
  }

public:
  _SeqTest(InputIterator *iterator) : Npeg(iterator) {}
};

/*
 * - Check that there's no partial consumption in case of mismatch.
 * - Check that there's no undesired multiple matching of repeated occurences.
 * - Check that the order of the expression is heeded.
 */
int main(int argc, char *argv[]) 
{
  const char emptystring[] = "";
  const char test1string1[] = TESTSTRING1"blah";
  const char test1string2[] = TESTSTRING2"blah";
  const char test1string1_1string2[] = TESTSTRING1 TESTSTRING2"blah";
  const char middle_test1string2_1string1[] = OTHERSTRING TESTSTRING1 TESTSTRING2"blah";
  const char errmsg[] = "some kind of error";

  StringInputIterator *p_iterator;
  _SeqTest *p_context;

  p_iterator = new StringInputIterator(emptystring, 0);
  p_context = new _SeqTest(p_iterator);
  assert(p_context->isMatch2() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of empty string.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(test1string1, strlen(test1string1));
  p_context = new _SeqTest(p_iterator);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  assert(p_context->isMatch2() == 0 && p_iterator->getIndex() == 0);
  delete p_iterator; delete p_context;
  p_iterator = new StringInputIterator(test1string2, strlen(test1string2));
  p_context = new _SeqTest(p_iterator);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  assert(p_context->isMatch2() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of partial sequence.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(test1string1_1string2, strlen(test1string1_1string2));
  p_context = new _SeqTest(p_iterator);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == strlen(TESTSTRING1) + strlen(TESTSTRING2));
  p_iterator->setIndex(0);
  assert(p_context->isMatch2() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of full sequence.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(middle_test1string2_1string1, 
				       strlen(middle_test1string2_1string1));
  p_context = new _SeqTest(p_iterator);
  p_iterator->setIndex(strlen(OTHERSTRING));
  assert(p_context->isMatch() == 1
	 && p_iterator->getIndex() == strlen(OTHERSTRING) + strlen(TESTSTRING1) + strlen(TESTSTRING2));
  p_iterator->setIndex(strlen(OTHERSTRING));
  assert(p_context->isMatch2() == 0 && p_iterator->getIndex() == strlen(OTHERSTRING));
  printf("\tVerified: Handling of nonoccurence of full sequence at random position.\n");
  delete p_iterator; delete p_context;  

  return 0;
}
