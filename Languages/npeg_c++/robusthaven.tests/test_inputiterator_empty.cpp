#include <iostream>
#include <ctime>
#include <cstdlib>
#include <cassert>

#include "robusthaven/text/StringInputIterator.h"

using namespace std;
using namespace RobustHaven::Text;

/*
 * Unit test for the empty string limit case.
 */
int main(int argc, char *argv[]) {
  StringInputIterator iter("", 0);
  int i;

  srand(time(NULL));
  
  for (i = 0; i < 10; i++) {
    int start, end;
    char buffer[101] = { 1, 1, 1 };

    start = rand()%100, end = rand()%100;

    assert(iter.getText(buffer, start, end) == 0);
    cout << "\tVerified: iterator with empty string is handled correctly.\n";

    assert(buffer[0] == 0 && buffer[1] == 1 && buffer[2] == 1);
    cout << "\tVerified: no modification of destination buffer apart from first byte.\n";
  }
  cout << "\tReached: get_text works.\n";
    
  assert(iter.getCurrent() == -1);
  assert(iter.getNext() == -1);
  
  return 0;
} /* main */
