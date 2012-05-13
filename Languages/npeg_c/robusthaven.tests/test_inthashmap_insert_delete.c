#include <stdlib.h>
#include <time.h>
#include <stdio.h>
#include <assert.h>
#include "robusthaven/structures/inthashmap.h"

/*
 * Creates a hashmap only containing random ints, then a subset of the items is removed from the map.
 * All unremoved items have to be found, while for every key belonging to a removed item, we expect to 
 * get a NULL pointer. 
 * The hashmap capacity is intentionally chosen too low, in order to force storage in linked lists.
 * Multiple removal of the same item ought not lead to any malfunction.
 */
int main(int argc, char *argv[]) {
#define start_cap 91
#define nof_items 2*start_cap
#define nof_removed start_cap/3

  char state[200];
  int keys[nof_items];
  int removed_idcs[nof_removed];
  int dataitems[nof_items];
  rh_inthashmap_instance hashmap;
  uint i;
  
  initstate(time(NULL), state, 200);

  printf("\tReached: start hashmap insertion\n");
  rh_inthashmap_constructor(&hashmap, start_cap);
  for (i = 0; i < nof_items; i++) { 
    keys[i] = (int)random();
    dataitems[i] = (int)random();

    rh_inthashmap_insert(&hashmap, &dataitems[i], keys[i]);
  }

  printf("\tReached: start deletion\n");
  for (i = 0; i < nof_removed; i++) {
    removed_idcs[i] = random()%nof_items;
    rh_inthashmap_remove(&hashmap, keys[removed_idcs[i]]);
  }

  printf("\tReached: start lookup\n");
  for (i = 0; i < nof_items; i++) {
    uint k;
    
    for (k = 0; k < nof_removed && removed_idcs[k] != i; k++);
    if (k == nof_removed) {
      assert(*(int*)rh_inthashmap_lookup(&hashmap, keys[i]) == dataitems[i]);
      printf("\tVerified: lookup of item w/ key %d.\n", keys[i]);
    } else {
      assert(rh_inthashmap_lookup(&hashmap, keys[i]) == NULL);
      printf("\tVerified: removal of item w/ key %d.\n", keys[i]);
    }
  } /* for all items */

  rh_inthashmap_destructor(&hashmap);
  
  return 0;
} /* main */
