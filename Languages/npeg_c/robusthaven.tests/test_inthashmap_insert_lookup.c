#include <stdlib.h>
#include <time.h>
#include <stdio.h>
#include <assert.h>
#include "robusthaven/structures/inthashmap.h"

/*
 * Creates a hashmap and inserts elements which all have to be found afterwards.
 * The hashmap capacity is intentionally chosen too low, in order to force storage in linked lists.
 */
int main(int argc, char *argv[]) {
#define start_cap 71
#define nof_items 2*start_cap
  
  char state[200];
  int keys[nof_items];
  void *dataptrs[nof_items];
  rh_inthashmap_instance hashmap;
  uint i;
  
  initstate(time(NULL), state, 200);
  
  printf("\tReached: start hashmap insertion\n");

  rh_inthashmap_constructor(&hashmap, start_cap);
  for (i = 0; i < nof_items; i++) {
    keys[i] = (int)random();
    dataptrs[i] = (void*)(unsigned long)random();
    
    rh_inthashmap_insert(&hashmap, dataptrs[i], keys[i]);
  } /* for items to generate & insert */

  printf("\tReached: start hashmap lookup\n");
  for (i = 0; i < nof_items; i++) {
    void *testptr;

    testptr = rh_inthashmap_lookup(&hashmap, keys[i]);
    assert(dataptrs[i] == testptr);
    printf("\tVerified: lookup of item %d.\n", keys[i]);
  } /* for items to look up */

  rh_inthashmap_destructor(&hashmap);
  
  return 0;
} /* main */
