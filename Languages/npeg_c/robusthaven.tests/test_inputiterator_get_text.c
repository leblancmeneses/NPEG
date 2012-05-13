#include <string.h>
#include <stdio.h>
#include <time.h>
#include <stdlib.h>
#include <assert.h>
#include "robusthaven/text/npeg_inputiterator.h"

/*
 * Unit test for the get_text routine
 */
int main(int argc, char *argv[]) {
#define strlen (uint)10

  char string[strlen+1], buffer[strlen+1];
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

  assert(npeg_inputiterator_get_text(buffer, &iter, 0, strlen) == strlen);
  assert(buffer[strlen] == 0);
  assert(memcmp(buffer, string, strlen) == 0);
  printf("\tVerified: copying works.\n");

  assert(npeg_inputiterator_get_text(buffer, &iter, 2, 5) == 3);
  assert(buffer[3] == 0);
  assert(buffer[0] == string[2] && buffer[1] == string[3] && buffer[2] == string[4]);
  printf("\tVerified: substring works.\n");

  assert(npeg_inputiterator_get_text(buffer, &iter, 2, 1) == 0);
  assert(buffer[strlen] == 0);
  printf("\tVerified: start > end works.\n");

  assert(npeg_inputiterator_get_text(buffer, &iter, -2, 1) == 1);
  assert(buffer[1] == 0);
  assert(buffer[0] == string[0]);
  printf("\tVerified: start < 0 works.\n");

  assert(npeg_inputiterator_get_text(buffer, &iter, strlen - 1, strlen + 1) == 1);
  assert(buffer[1] == 0);
  assert(buffer[0] == string[strlen-1]);
  printf("\tVerified: end > strlen works.\n");
  
  npeg_inputiterator_destructor(&iter);

  return 0;
} /* main */
