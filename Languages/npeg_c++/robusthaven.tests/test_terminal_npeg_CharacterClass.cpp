#include <iostream>
#include <cassert>
#include <cstdio>
#include <cstring>
#include "robusthaven/text/StringInputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;
using namespace std;

class _CharTest : public Npeg {
private:
  const char *m_classStr;
  int m_len;

public:
  void setClass(const char classStr[], const int len) {
    m_classStr = classStr;
    m_len = len;
  }

  int isMatch(void) throw (ParsingFatalTerminalException) {
    return characterClass(m_classStr, m_len);
  }

public:
  _CharTest(InputIterator *p_iter) : Npeg(p_iter) {}
};

int main(int argc, char *argv[]) 
{
  char input[2] = {'\0','\0'};
  int i = 0;			
  StringInputIterator *p_iterator;
  _CharTest *p_context;
  bool errorSeen;
			
  printf("\tReached: ensure arguments are validated\n");
	
  errorSeen = false;
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  try {
    p_context->setClass("", 0);
    p_context->isMatch();    
  } catch (ParsingFatalTerminalException e) {
    cout << e.what() << endl;
    assert(0 == strcmp(e.what(), "CharacterClass definition must be a minimum of 3 characters [expression]"));	
    printf("\tVerified: expected exception message.  '%s'.\n", e.what());
    errorSeen = true;
  }
  assert(errorSeen);
  printf("\tVerified: empty character class leads to error.\n");
  assert(0 == p_context->getWarnings().size());	
  printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
  assert(0 == p_iterator->getIndex());
  printf("\tVerified: On no match iterator should not consume character.\n");
  delete p_iterator; delete p_context;



  printf("\n");		
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  try {
    errorSeen = false;
    p_context->setClass("aaa", 3);
    p_context->isMatch();
    printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
  } catch (ParsingFatalTerminalException e) {
    assert(0 == strcmp(e.what(), "CharacterClass definition must start with ["));	
    printf("\tVerified: expected exception message.  '%s'.\n", e.what());
    errorSeen = true;
  }
  assert(errorSeen);
  printf("\tVerified: invalid character class leads to error.\n");  
  assert(0 == p_context->getWarnings().size());	
  printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
  assert(0 == p_iterator->getIndex());
  printf("\tVerified: On no match iterator should not consume character.\n");
  delete p_iterator; delete p_context;




  printf("\n");
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  try {
    errorSeen = false;
    p_context->setClass("[aa", 3);
    p_context->isMatch();
    printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
  } catch (ParsingFatalTerminalException e) {
    assert(0 == strcmp(e.what(), "CharacterClass definition must end with ]"));	
    printf("\tVerified: expected exception message.  '%s'.\n", e.what());
    errorSeen = true;
  }
  assert(errorSeen);
  printf("\tVerified: invalid character class leads to error.\n");  
  assert(0 == p_context->getWarnings().size());	
  printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
  assert(0 == p_iterator->getIndex());
  printf("\tVerified: On no match iterator should not consume character.\n");
  delete p_iterator; delete p_context;

	
  printf("\n");
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  try {
    errorSeen = false;
    p_context->setClass("[\\]", 3);
    p_context->isMatch();
    printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
  } catch (ParsingFatalTerminalException e) {
    assert(0 == strcmp(e.what(), "CharacterClass definition requires user to escape "
		       "'\\' given location in expression. User must escape by specifying '\\\\'"));	
    printf("\tVerified: expected exception message.  '%s'.\n", e.what());
    errorSeen = true;
  }
  assert(errorSeen);
  printf("\tVerified: invalid character class leads to error.\n");  
  assert(0 == p_context->getWarnings().size());	
  printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
  assert(0 == p_iterator->getIndex());
  printf("\tVerified: On no match iterator should not consume character.\n");
  delete p_iterator; delete p_context;

	
  printf("\n");
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  try {
    errorSeen = false;
    p_context->setClass("[-]", 3);
    p_context->isMatch();
    printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
  } catch (ParsingFatalTerminalException e) {
    assert(0 == strcmp(e.what(), "CharacterClass definition requires user to escape '-' given location in expression. User must escape by specifying '\\-'"));	
    printf("\tVerified: expected exception message.  '%s'.\n", e.what());
    errorSeen = true;
  }
  assert(errorSeen);
  printf("\tVerified: invalid character class leads to error.\n");  
  assert(0 == p_context->getWarnings().size());	
  printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
  assert(0 == p_iterator->getIndex());
  printf("\tVerified: On no match iterator should not consume character.\n");
  delete p_iterator; delete p_context;
	
  printf("\n");
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  try {
    errorSeen = false;
    p_context->setClass("[a-]", 4);
    p_context->isMatch();
    printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
  } catch (ParsingFatalTerminalException e) {
    assert(0 == strcmp(e.what(), "CharacterClass definition requires user to escape '-' given location in expression. User must escape by specifying '\\-'"));	
    printf("\tVerified: expected exception message.  '%s'.\n", e.what());
    errorSeen = true;
  }
  assert(errorSeen);
  printf("\tVerified: invalid character class leads to error.\n");  
  assert(0 == p_context->getWarnings().size());	
  printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
  assert(0 == p_iterator->getIndex());
  printf("\tVerified: On no match iterator should not consume character.\n");
  delete p_iterator; delete p_context;
	
	
  printf("\n");
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  try {
    errorSeen = false;
    p_context->setClass("[\\L]", 4);
    p_context->isMatch();
    printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
  } catch (ParsingFatalTerminalException e) {
    assert(0 == strcmp(e.what(), "CharacterClass definition contains an invalid escape sequence. Accepted sequences: \\\\, \\s, \\S, \\d, \\D, \\w, \\W"));	
    printf("\tVerified: expected exception message.  '%s'.\n", e.what());
    errorSeen = true;
  }
  assert(errorSeen);
  printf("\tVerified: invalid character class leads to error.\n");  
  assert(0 == p_context->getWarnings().size());	
  printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
  assert(0 == p_iterator->getIndex());
  printf("\tVerified: On no match iterator should not consume character.\n");
  delete p_iterator; delete p_context;
	
	
	
  printf("\n");
  printf("\tReached: confirming postive character group.\n");
  printf("\n");
	


  printf("\tReached: validate simple character ranges.\n");
  for(i = 0; i <= 9; i++)
    {
      *input = '0' + i;
      p_iterator = new StringInputIterator(input, 2);
      p_context = new _CharTest(p_iterator);
      p_context->setClass("[0-9]", 5);      
      assert(1 == p_context->isMatch());
      printf("\tVerified: [0-9] match with %c as input.\n", *input);
      assert(1 == p_iterator->getIndex());
      printf("\tVerified: On match iterator should consume character.\n");
      delete p_iterator; delete p_context;
    }
	
  printf("\n");
  printf("\tReached: validate grouped character ranges.\n");
  for(i = 0; i < 26; i++)
    {
      *input = 'a' + i;
      p_iterator = new StringInputIterator(input, 2);
      p_context = new _CharTest(p_iterator);
      p_context->setClass("[A-Z0-9a-z]", 11);      
      assert(1 == p_context->isMatch());
      printf("\tVerified: [A-Z0-9a-z] match with %c as input.\n", *input);
      assert(1 == p_iterator->getIndex());
      printf("\tVerified: On match iterator should consume character.\n");
      delete p_iterator; delete p_context;
		
      *input = 'A' + i;
      p_iterator = new StringInputIterator(input, 2);
      p_context = new _CharTest(p_iterator);
      p_context->setClass("[A-Z0-9a-z]", 11);      
      assert(1 == p_context->isMatch());
      printf("\tVerified: [A-Z0-9a-z] match with %c as input.\n", *input);
      assert(1 == p_iterator->getIndex());
      printf("\tVerified: On match iterator should consume character.\n");
      delete p_iterator; delete p_context;
		
      *input = '0' + i%10;
      p_iterator = new StringInputIterator(input, 2);
      p_context = new _CharTest(p_iterator);
      p_context->setClass("[A-Z0-9a-z]", 11);      
      assert(1 == p_context->isMatch());
      printf("\tVerified: [A-Z0-9a-z] match with %c as input.\n", *input);
      assert(1 == p_iterator->getIndex());
      printf("\tVerified: On match iterator should consume character.\n");
      delete p_iterator; delete p_context;
    }


	
  printf("\n");
  printf("\tReached: interpreted simple escape sequence.\n");
  for(i = 0; i <= 9; i++)
    {
      *input = '0' + i;
      p_iterator = new StringInputIterator(input, 2);
      p_context = new _CharTest(p_iterator);
      p_context->setClass("[\\d]", 4);      
      assert(1 == p_context->isMatch());
      printf("\tVerified: \\d match with %d as input.\n", i);
      assert(1 == p_iterator->getIndex());
      printf("\tVerified: On match iterator should consume character.\n");
      delete p_iterator; delete p_context;
    }
	
	
  printf("\n");
  printf("\tReached: interpreted groupped escape sequence.\n");

  //"[\\d\\D\\s\\S\\w\\W]"
  for(i = 0; i <= 9; i++)
    {
      *input = '0' + i;
      p_iterator = new StringInputIterator(input, 2);
      p_context = new _CharTest(p_iterator);
      p_context->setClass("[\\d\\s]", 6);      
      assert(1 == p_context->isMatch());
      printf("\tVerified: [\\d\\s] match with %d as input.\n", i);
      assert(1 == p_iterator->getIndex());
      printf("\tVerified: On match iterator should consume character.\n");
      delete p_iterator; delete p_context;
		
      p_iterator = new StringInputIterator(input, 2);
      p_context = new _CharTest(p_iterator);
      p_context->setClass("[\\s\\w]", 6);      
      assert(1 == p_context->isMatch());
      printf("\tVerified: [\\s\\w] match with %d as input.\n", i);
      assert(1 == p_iterator->getIndex());
      printf("\tVerified: On match iterator should consume character.\n");
      delete p_iterator; delete p_context;
    }

  printf("\n");
  *input = ' ';
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  p_context->setClass("[\\d\\s]", 6);      
  assert(1 == p_context->isMatch());
  printf("\tVerified: [\\d\\s] match with ' ' as input.\n");
  assert(1 == p_iterator->getIndex());
  printf("\tVerified: On match iterator should consume character.\n");
  delete p_iterator; delete p_context;

  *input = '\n';
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  p_context->setClass("[\\d\\s]", 6);      
  assert(1 == p_context->isMatch());
  printf("\tVerified: [\\d\\s] match with \\n as input.\n");
  assert(1 == p_iterator->getIndex());
  printf("\tVerified: On match iterator should consume character.\n");
  delete p_iterator; delete p_context;
	
  *input = '\f';
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  p_context->setClass("[\\d\\s]", 6);      
  assert(1 == p_context->isMatch());
  printf("\tVerified: [\\d\\s] match with \\f as input.\n");
  assert(1 == p_iterator->getIndex());
  printf("\tVerified: On match iterator should consume character.\n");
  delete p_iterator; delete p_context;
	
  *input = '\r';
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  p_context->setClass("[\\d\\s]", 6);      
  assert(1 == p_context->isMatch());
  printf("\tVerified: [\\d\\s] match with \\r as input.\n");
  assert(1 == p_iterator->getIndex());
  printf("\tVerified: On match iterator should consume character.\n");
  delete p_iterator; delete p_context;
	
  *input = '\t';
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  p_context->setClass("[\\d\\s]", 6);      
  assert(1 == p_context->isMatch());
  printf("\tVerified: [\\d\\s] match with \\t as input.\n");
  assert(1 == p_iterator->getIndex());
  printf("\tVerified: On match iterator should consume character.\n");
  delete p_iterator; delete p_context;
	
  *input = '\v';
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  p_context->setClass("[\\d\\s]", 6);      
  assert(1 == p_context->isMatch());
  printf("\tVerified: [\\d\\s] match with \\v as input.\n");
  assert(1 == p_iterator->getIndex());
  printf("\tVerified: On match iterator should consume character.\n");
  delete p_iterator; delete p_context;
	







  printf("\n");
  printf("\tReached: confirming negative character group.\n");
  printf("\n");
	
  printf("\tReached: confirming single negative character group.\n");
  *input = 'b';
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  p_context->setClass("[^a]", 4);      
  assert(1 == p_context->isMatch());
  printf("\tVerified: [^a] matches input b.\n");
  assert(1 == p_iterator->getIndex());
  printf("\tVerified: On match iterator should consume character.\n");
  delete p_iterator; delete p_context;	
	
  *input = 'a';
  p_iterator = new StringInputIterator(input, 2);
  p_context = new _CharTest(p_iterator);
  p_context->setClass("[^a]", 4);      
  assert(0 == p_context->isMatch());
  printf("\tVerified: [^a] does matches input a.\n");
  assert(0 == p_iterator->getIndex());
  printf("\tVerified: On not match character is not consummed by iterator.\n");
  delete p_iterator; delete p_context;


  printf("\tReached: confirming escaped negative character group.\n");
  for(i = 0; i < 26; i++)
    {
      *input =  'a' + i;
      p_iterator = new StringInputIterator(input, 2);
      p_context = new _CharTest(p_iterator);
      p_context->setClass("[^\\W]", 5);      
      assert(1 == p_context->isMatch());
      printf("\tVerified: [^\\W] which means [\\w] match with %c as input.\n", *input);
      assert(1 == p_iterator->getIndex());
      printf("\tVerified: On match iterator should consume character.\n");
      delete p_iterator; delete p_context;

      p_iterator = new StringInputIterator(input, 2);
      p_context = new _CharTest(p_iterator);
      p_context->setClass("[^\\S]", 5);      
      assert(0 == p_context->isMatch());
      printf("\tVerified: [^\\S] which translates to postive character group [\\s] will not match with %c as input.\n", *input);
      assert(0 == p_iterator->getIndex());
      printf("\tVerified: On no match character is not consummed by iterator.\n");
      delete p_iterator; delete p_context;
    }

	
  return 0;
}
