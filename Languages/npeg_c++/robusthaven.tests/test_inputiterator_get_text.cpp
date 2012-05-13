#include <cstring>
#include <iostream>
#include <ctime>
#include <cstdlib>
#include <cassert>

#include "robusthaven/text/StringInputIterator.h"

using namespace std;
using namespace RobustHaven::Text;

/*
 * Unit test for the get_text routine
 */
int main(int argc, char *argv[]) {
  const uint strlen = 10;

  char string[strlen+1], buffer[strlen+1];
  InputIterator *p_iter;
  uint i;

  srand(time(NULL));

  for (i = 0; i < strlen; i++) {
    if (rand()%10 < 2) string[i] = 0;
    else string[i] = rand()%('z' - 'a') + 'a';
  }

  p_iter = new StringInputIterator(string, strlen);
  assert(p_iter->getLength() == strlen);

  assert(p_iter->getText(buffer, 0, strlen) == strlen);
  assert(buffer[strlen] == 0);
  assert(memcmp(buffer, string, strlen) == 0);
  cout << "\tVerified: copying works.\n";

  assert(p_iter->getText(buffer, 2, 5) == 3);
  assert(buffer[3] == 0);
  assert(buffer[0] == string[2] && buffer[1] == string[3] && buffer[2] == string[4]);
  cout << "\tVerified: substring works.\n";

  assert(p_iter->getText(buffer, 2, 1) == 0);
  assert(buffer[strlen] == 0);
  cout << "\tVerified: start > end works.\n";

  assert(p_iter->getText(buffer, strlen - 1, strlen + 1) == 1);
  assert(buffer[1] == 0);
  assert(buffer[0] == string[strlen-1]);
  cout << "\tVerified: end > strlen works.\n";
  
  delete (StringInputIterator*)p_iter;

  return 0;
} /* main */
