#include <cassert>
#include <cstdio>
#include <cstring>
#include <cstdlib>
#include <ctime>
#include "robusthaven/text/StringInputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;

class _AnyTest : public Npeg {
public:
  int isMatch(void) throw (ParsingFatalTerminalException) {
    return anyCharacter();
  }

public:
  _AnyTest(InputIterator *p_iter) : Npeg(p_iter) {}
};

/*
 * - Test that consumption of an empty string results in failure.
 * - Test that consumption of a random string works until the end of the string is reached.
 */
int main(int argc, char *argv[]) 
{
  const int randstringlen = 10;
  const char errmsg[] = "some kind of error";

  char emptystring[] = "";
  char randomstring[randstringlen];
  StringInputIterator *p_iterator;
  Npeg *p_context;
  int i;

  srand(time(NULL));

  p_iterator = new StringInputIterator(emptystring, 0);
  p_context = new _AnyTest(p_iterator);
  assert(p_context->isMatch() == 0);
  delete p_iterator; delete p_context;
  printf("\tVerified: handling of empty string\n");

  for (i = 0; i < randstringlen; i++) {
    randomstring[i] = rand()%('z' - 'a') + 'a';
  }
  
  p_iterator = new StringInputIterator(randomstring, randstringlen);
  p_context = new _AnyTest(p_iterator);
  for (i = 0; i < randstringlen; i++) 
  {
    assert(p_iterator->getCurrent() == randomstring[i]);
    assert(p_context->isMatch() == 1);
  }

  assert(p_context->isMatch() == 0);
  printf("\tVerified: handling of end of string\n");
  
  delete p_iterator; delete p_context;
  printf("\tVerified: consumption of random string\n");

  return 0;
}
