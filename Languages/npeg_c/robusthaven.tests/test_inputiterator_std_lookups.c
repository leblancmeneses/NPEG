#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <assert.h>
#include "robusthaven/text/npeg_inputiterator.h"

/*
 * Unit test for the look ahead/behind & previous, next & current routines
 */
int main(int argc, char *argv[]) {
#define strlen (uint)12

  char string[strlen+1];
  npeg_inputiterator iter;
  uint i;

  srand(time(NULL));

  for (i = 0; i < strlen; i++) {
    if (rand()%10 < 2) string[i] = 0;
    else string[i] = rand()%('z' - 'a') + 'a';
  }
  string[i] = 0;
  npeg_inputiterator_constructor(&iter, string, strlen);
  assert(iter.length == strlen);

  assert(npeg_inputiterator_get_previous(&iter) == -1 && iter.index == 0);
  assert(npeg_inputiterator_get_current(&iter) == string[0] && iter.index == 0);
  assert(npeg_inputiterator_get_next(&iter) == string[1] && iter.index == 1);
  printf("\tVerified: start of string OK.\n");

  for (i = 1; i < strlen - 1; i++) {
    assert(npeg_inputiterator_get_next(&iter) == string[i+1]);
  }
  assert(npeg_inputiterator_get_next(&iter) == -1 && iter.index == strlen);
  printf("\tVerified: next works.\n");
  
  for (i = 0; i < strlen; i++) {
    assert(npeg_inputiterator_get_previous(&iter) == string[strlen-i-1]);
  }
  assert(npeg_inputiterator_get_previous(&iter) == -1 && iter.index == 0);
  printf("\tVerified: previous works.\n");
  

  npeg_inputiterator_destructor(&iter);

  return 0;
} /* main */
