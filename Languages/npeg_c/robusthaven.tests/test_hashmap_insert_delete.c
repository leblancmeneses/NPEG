#include <stdlib.h>
#include <time.h>
#include <stdio.h>
#include <assert.h>
#include "robusthaven/structures/hashmap.h"

#define _max_keylen (uint)30
#define start_cap (uint)91
#define nof_items (uint)(2*start_cap)
#define nof_removed (uint)(start_cap/3)

static void _generate_key(char *dstbuffer) {
  uint k, keylen;

  keylen = random()%(_max_keylen - 5 - 1) + 5;
  for (k = 0; k < keylen; k++) {
    /*
     * Test all admissible kinds of characters in string, since hash function may be faulty
     */
    switch (random()%3) {
    case 0:
      dstbuffer[k] = random()%('z' - 'a') + 'a';
      break;
	
    case 1:
      dstbuffer[k] = random()%('Z' - 'A') + 'A';
      break;

    case 2:
      dstbuffer[k] = random()%('9' - '0') + '0';
      break;
    }
  } /* for key chars */
  dstbuffer[k] = 0;
} /* _generate_key */

/*
 * Creates a hashmap only containing random ints, then a subset of the items is removed from the map.
 * All unremoved items have to be found, while for every key belonging to a removed item, we expect to 
 * get a NULL pointer. 
 * The hashmap capacity is intentionally chosen too low, in order to force storage in linked lists.
 * Multiple removal of the same item ought not lead to any malfunction.
 */
int main(int argc, char *argv[]) {
  char state[200];
  char keystrings[nof_items][_max_keylen];
  int removed_idcs[nof_removed];
  int dataitems[nof_items];
  rh_hashmap_instance hashmap;
  rh_hashmap_element element;
  uint i;
  
  initstate(time(NULL), state, 200);

  printf("\tReached: start hashmap insertion\n");
  rh_hashmap_constructor(&hashmap, start_cap, NULL);
  for (i = 0; i < nof_items; i++) { 
    _generate_key(keystrings[i]);
    dataitems[i] = (int)random();

	element.data = &dataitems[i];
    rh_hashmap_insert(&hashmap, &element, keystrings[i]);
  }

  printf("\tReached: start deletion\n");
  for (i = 0; i < nof_removed; i++) {
    removed_idcs[i] = random()%nof_items;
    rh_hashmap_remove(&hashmap, keystrings[removed_idcs[i]]);
  }

  printf("\tReached: start lookup\n");
  for (i = 0; i < nof_items; i++) {
    uint k;
    
    for (k = 0; k < nof_removed && removed_idcs[k] != i; k++);
    if (k == nof_removed) {
      assert(*(int*)rh_hashmap_lookup(&hashmap, keystrings[i]) == dataitems[i]);
      printf("\tVerified: lookup of item %s.\n", keystrings[i]);
    } else {
      assert(rh_hashmap_lookup(&hashmap, keystrings[i]) == NULL);
      printf("\tVerified: removal of item %s.\n", keystrings[i]);
    }
  } /* for all items */

  rh_hashmap_destructor(&hashmap);
  
  return 0;
} /* main */
