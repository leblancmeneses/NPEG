#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <assert.h>
#include "robusthaven/text/npeg_inputiterator.h"


/*
 * Unit test for the empty string limit case.
 */
int main(int argc, char *argv[]) {
  npeg_inputiterator iter;
  int i;

  srand(time(NULL));
  npeg_inputiterator_constructor(&iter, NULL, 0);
  
  for (i = 0; i < 10; i++) {
    int start, end;
    char buffer[101] = { 1, 1, 1 };

    start = rand()%100, end = rand()%100;

    assert(npeg_inputiterator_get_text(buffer, &iter, start, end) == 0);
    printf("\tVerified: iterator with empty string is handled correctly.\n");

    assert(buffer[0] == 0 && buffer[1] == 1 && buffer[2] == 1);
    printf("\tVerified: no modification of destination buffer apart from first byte.\n");
  }
  printf("\tReached: get_text works.\n");
    
  assert(npeg_inputiterator_get_current(&iter) == -1);
  assert(npeg_inputiterator_get_next(&iter) == -1);
  assert(npeg_inputiterator_get_previous(&iter) == -1);
  
  npeg_inputiterator_destructor(&iter);

  return 0;
} /* main */
