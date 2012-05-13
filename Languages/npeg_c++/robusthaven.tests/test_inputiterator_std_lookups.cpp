#include <sstream>
#include <cstring>
#include <iostream>
#include <ctime>
#include <cstdlib>
#include <cassert>

#include "robusthaven/text/StringInputIterator.h"

using namespace std;
using namespace RobustHaven::Text;

/*
 * Unit test for the look ahead/behind & previous, next & current routines
 */
int main(int argc, char *argv[]) {
  const uint strlen = 12;

  char string[strlen+1];
  InputIterator *p_iter;
  uint i;

  srand(time(NULL));

  for (i = 0; i < strlen; i++) {
    if (rand()%10 < 2) string[i] = 0;
    else string[i] = rand()%('z' - 'a') + 'a';
  }
  p_iter = new StringInputIterator(string, strlen);
  assert(p_iter->getLength() == strlen);

  assert(p_iter->getIndex() == 0);
  assert(p_iter->getCurrent() == string[0] && p_iter->getIndex() == 0);
  assert(p_iter->getNext() == string[1] && p_iter->getIndex() == 1);
  cout <<"\tVerified: start of string OK.\n";

  for (i = 1; i < strlen - 1; i++) {
    assert(p_iter->getNext() == string[i+1]);
  }
  assert(p_iter->getNext() == -1 && p_iter->getIndex() == strlen);
  cout <<"\tVerified: next works.\n";

  delete p_iter;

  return 0;
} /* main */
