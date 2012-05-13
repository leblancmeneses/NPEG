#include <stdlib.h>
#include <time.h>
#include <stdio.h>
#include <assert.h>
#include "robusthaven/structures/hashmap.h"

#define start_cap (uint)71
#define nof_items (uint)(2*start_cap)
#define max_keylen (uint)30

/*
 * Creates a hashmap and inserts elements which all have to be found afterwards.
 * The hashmap capacity is intentionally chosen too low, in order to force storage in linked lists.
 */
int main(int argc, char *argv[]) {  
  char state[200];
  char keystrings[nof_items][max_keylen];
  void *dataptrs[nof_items];
  rh_hashmap_instance hashmap;
  rh_hashmap_element element;
  uint i;
  
  initstate(time(NULL), state, 200);
  
  printf("\tReached: start hashmap insertion\n");

  rh_hashmap_constructor(&hashmap, start_cap, NULL);
  for (i = 0; i < nof_items; i++) {
    uint k, keylen;

    keylen = random()%(max_keylen - 5 - 1) + 5;
    for (k = 0; k < keylen; k++) {
      /*
       * Test all admissible kinds of characters in string, since hash function may be faulty
       */
      switch (random()%3) {
      case 0:
	keystrings[i][k] = random()%('z' - 'a') + 'a';
	break;
	
      case 1:
	keystrings[i][k] = random()%('Z' - 'A') + 'A';
	break;

      case 2:
	keystrings[i][k] = random()%('9' - '0') + '0';
	break;
      }
    } /* for key chars */
    keystrings[i][k] = 0;
    dataptrs[i] = (void*)((unsigned long)random());
    element.data = dataptrs[i];
    rh_hashmap_insert(&hashmap, &element, keystrings[i]);
  } /* for items to generate & insert */
  printf("\tReached: start hashmap lookup\n");

  for (i = 0; i < nof_items; i++) {
    void *testptr;

    testptr = rh_hashmap_lookup(&hashmap, keystrings[i]);
    assert(dataptrs[i] == testptr);
    printf("\tVerified: look up of item %s.\n", keystrings[i]);
  } /* for items to look up */

  rh_hashmap_destructor(&hashmap);
  
  return 0;
} /* main */
