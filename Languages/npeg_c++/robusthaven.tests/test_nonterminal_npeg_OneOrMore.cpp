#include <cassert>
#include <cstdio>
#include <cstring>
#include "robusthaven/text/Npeg.h"
#include "robusthaven/text/StringInputIterator.h"

using namespace RobustHaven::Text;

#define TESTSTRING "test"
#define OTHERSTRING "somthing else"

class _OneTest : public Npeg {
private:
  int _expression() throw (ParsingFatalTerminalException) {
    return literal(TESTSTRING, strlen(TESTSTRING), 1);
  } 

  int _infiniteExpression() {
    return notPredicate((Npeg::IsMatchPredicate)&_OneTest::_expression);
  }

public:
  int isMatch() throw (ParsingFatalTerminalException) {
    return Npeg::oneOrMore((Npeg::IsMatchPredicate)&_OneTest::_expression);
  }

  int bad() throw (ParsingFatalTerminalException) {
    return oneOrMore((Npeg::IsMatchPredicate)&_OneTest::_infiniteExpression);
  }

public:
  _OneTest(InputIterator *iterator) : Npeg(iterator) {}
};

int main(int argc, char *argv[]) 
{
  const char emptystring[] = "";
  const char otherstring[] = "blah";
  const char test1string[] = TESTSTRING"Tes";
  const char test2string[] = TESTSTRING TESTSTRING"Tes";
  const char middle_test2string[] = OTHERSTRING TESTSTRING TESTSTRING"blah";
  const char errmsg[] = "some kind of error";

  StringInputIterator *p_iterator;
  _OneTest *p_context;
  bool errorSeen;

  p_iterator = new StringInputIterator(emptystring, 0);
  p_context = new _OneTest(p_iterator);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of empty string.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(otherstring, strlen(otherstring));
  p_context = new _OneTest(p_iterator);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of 0 occurrence in non-empty string.\n");	
  delete p_iterator; delete p_context;	

  p_iterator = new StringInputIterator(otherstring, strlen(otherstring));
  p_context = new _OneTest(p_iterator);
  try {
    errorSeen = false;
    p_context->bad();
  } catch (InfiniteLoopException &i) {
    errorSeen = true;
  }
  assert(errorSeen);
  printf("\tVerified: Detection of infinite loops.\n");	
  delete p_context; 
  delete p_iterator; 	

  p_iterator = new StringInputIterator(test1string, strlen(test1string));
  p_context = new _OneTest(p_iterator);
  assert(p_context->isMatch() == 1 
	 && p_iterator->getIndex() == strlen(TESTSTRING));
  printf("\tVerified: Handling of single occurrence.\n");	
  delete p_iterator; delete p_context;	

  p_iterator = new StringInputIterator(test2string, strlen(test2string));
  p_context = new _OneTest(p_iterator);
  assert(p_context->isMatch() == 1 
	 && p_iterator->getIndex() == 2*strlen(TESTSTRING));
  printf("\tVerified: Handling of double occurrence.\n");	
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(middle_test2string, strlen(middle_test2string));
  p_context = new _OneTest(p_iterator);
  p_iterator->setIndex(strlen(OTHERSTRING));
  assert(p_context->isMatch() == 1 
	 && p_iterator->getIndex() == 2*strlen(TESTSTRING) + strlen(OTHERSTRING));
  printf("\tVerified: Handling of double occurrence at string center.\n");	
  delete p_iterator; delete p_context;

  return 0;
}
