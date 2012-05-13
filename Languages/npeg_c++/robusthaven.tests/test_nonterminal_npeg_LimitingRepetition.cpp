#include <iostream>
#include <cassert>
#include <cstdio>
#include <cstring>
#include "robusthaven/text/Npeg.h"
#include "robusthaven/text/StringInputIterator.h"

using namespace RobustHaven::Text;
using namespace std;

#define TESTSTRING "test"
#define OTHERSTRING "somthing else"

class _RepTest : public Npeg {
  int m_min, m_max;

private:
  int _expression() {
    return literal(TESTSTRING, strlen(TESTSTRING), 1);
  } 

  int _infiniteExpression() {
    return notPredicate((Npeg::IsMatchPredicate)&_RepTest::_expression);
  }

public:
  void setBounds(int min, int max) {
    m_min = min, m_max = max;
  }

  int isMatch() throw (ParsingFatalTerminalException) {
    return limitingRepetition(m_min, m_max, (Npeg::IsMatchPredicate)&_RepTest::_expression);
  }

  int bad() throw (ParsingFatalTerminalException) {
    return limitingRepetition(m_min, m_max, (Npeg::IsMatchPredicate)&_RepTest::_infiniteExpression);
  }

public:
  _RepTest(InputIterator *iterator) : Npeg(iterator) {}
};

int main(int argc, char *argv[]) 
{
  const char emptystring[] = "";
  const char otherstring[] = "blah";
  const char test1string[] = TESTSTRING"Tes";
  const char test3string[] = TESTSTRING TESTSTRING TESTSTRING"Tes";
  const char middle_test2string[] = OTHERSTRING TESTSTRING TESTSTRING"blah";
  const char errmsg[] = "some kind of error";

  StringInputIterator *p_iterator;
  _RepTest *p_context;
  bool errorSeen;
	
  p_iterator = new StringInputIterator(emptystring, 0);
  p_context = new _RepTest(p_iterator);  
  p_context->setBounds(0, 10);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == 0);
  p_context->setBounds(1, 10);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of empty string.\n");
  delete p_context; 
  delete p_iterator; 

  p_iterator = new StringInputIterator(otherstring, strlen(otherstring));
  p_context = new _RepTest(p_iterator);
  p_context->setBounds(0, 10);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == 0);
  p_context->setBounds(1, 10);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of 0 occurrence in non-empty string.\n");	
  delete p_context; 
  delete p_iterator; 	

  p_iterator = new StringInputIterator(otherstring, strlen(otherstring));
  p_context = new _RepTest(p_iterator);
  try {
    errorSeen = false;
    p_context->setBounds(0, -1);
    p_context->bad();
  } catch (InfiniteLoopException &i) {
    errorSeen = true;
  }
  assert(errorSeen);
  printf("\tVerified: Detection of infinite loops.\n");	
  delete p_context; 
  delete p_iterator; 	

  p_iterator = new StringInputIterator(test1string, strlen(test1string));
  p_context = new _RepTest(p_iterator);
  p_context->setBounds(0, -1);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == strlen(TESTSTRING));
  p_iterator->setIndex(0);
  p_context->setBounds(1, 10);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == strlen(TESTSTRING));
  p_iterator->setIndex(0);
  p_context->setBounds(2, -1);
  assert(p_context->isMatch() == 0 && p_iterator->getIndex() == 0);
  printf("\tVerified: Handling of single occurrence.\n");	
  delete p_context; 
  delete p_iterator; 	

  p_iterator = new StringInputIterator(test3string, strlen(test3string));
  p_context = new _RepTest(p_iterator);
  p_context->setBounds(0, 1);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == strlen(TESTSTRING));
  p_iterator->setIndex(0);
  p_context->setBounds(1, -1);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == 3*strlen(TESTSTRING));
  p_iterator->setIndex(0);
  p_context->setBounds(2, 2);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == 2*strlen(TESTSTRING));
  p_iterator->setIndex(0);
  p_context->setBounds(2, 3);
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == 3*strlen(TESTSTRING));
  printf("\tVerified: Handling of tripple occurrence.\n");	
  delete p_context; 
  delete p_iterator; 

  p_iterator = new StringInputIterator(middle_test2string, strlen(middle_test2string));
  p_context = new _RepTest(p_iterator);
  p_iterator->setIndex(strlen(OTHERSTRING));
  p_context->setBounds(0, 1);
  assert(p_context->isMatch() == 1 
	 && p_iterator->getIndex() == strlen(OTHERSTRING) + strlen(TESTSTRING));
  p_iterator->setIndex(strlen(OTHERSTRING));
  p_context->setBounds(1, -1);
  assert(p_context->isMatch() == 1 	 
	 && p_iterator->getIndex() == strlen(OTHERSTRING) + 2*strlen(TESTSTRING));
  p_iterator->setIndex(strlen(OTHERSTRING));
  p_context->setBounds(2, 2);
  assert(p_context->isMatch() == 1
	 && p_iterator->getIndex() == strlen(OTHERSTRING) + 2*strlen(TESTSTRING));
  p_iterator->setIndex(strlen(OTHERSTRING));
  p_context->setBounds(2, 3);
  assert(p_context->isMatch() == 1 
	 && p_iterator->getIndex() == strlen(OTHERSTRING) + 2*strlen(TESTSTRING));
  printf("\tVerified: Handling of double occurrence at center of string.\n");	

  p_iterator->setIndex(strlen(OTHERSTRING));  
  errorSeen = false;
  try {
    p_context->setBounds(2, 1);    
    p_context->isMatch();
  } catch (ParsingFatalTerminalException e) {
    cout << e.what() << endl;
    errorSeen = true;
  }
  assert(true);
  printf("\tVerified: Handling of non-sensical repetition limits.\n");	
  delete p_context; 
  delete p_iterator; 

  return 0;
}
