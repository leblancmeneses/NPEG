#include <stdlib.h>
#include <time.h>
#include <stdio.h>
#include <assert.h>
#include "robusthaven/structures/hashmap.h"

#define _max_keylen (uint)30

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
 * Creates 3 hashmaps with different data types and inserts elements which all have to be 
 * found afterwards.
 * The hashmap capacity is intentionally chosen too low, in order to force storage in linked lists.
 */
int main(int argc, char *argv[]) {
  typedef struct teststruct_t {
    int a;
    char b;
  } teststruct_t;

#define start_cap_int (uint)71
#define start_cap_char (uint)91
#define start_cap_struct (uint)113
#define nof_items_int (uint)(2*start_cap_int)
#define nof_items_char (uint)(2*start_cap_char)
#define nof_items_struct (uint)(2*start_cap_struct)
  
  char state[200];
  char intkeystrings[nof_items_int][_max_keylen];
  int intdata[nof_items_int];
  char charkeystrings[nof_items_char][_max_keylen];
  char chardata[nof_items_char];
  char structkeystrings[nof_items_struct][_max_keylen];
  teststruct_t structdata[nof_items_struct];
  rh_hashmap_instance hashmap_int, hashmap_char, hashmap_struct;
  rh_hashmap_element element;
  uint i;  

  initstate(time(NULL), state, 200);

  rh_hashmap_constructor(&hashmap_int, start_cap_int, NULL);
  rh_hashmap_constructor(&hashmap_char, start_cap_char, NULL);
  rh_hashmap_constructor(&hashmap_struct, start_cap_struct, NULL);

  printf("\tReached: population of int datatype hashmap insertion\n");
  for (i = 0; i < nof_items_int; i++) {
    _generate_key(intkeystrings[i]);
    intdata[i] = (int)random();

	element.data =  &intdata[i];
    rh_hashmap_insert(&hashmap_int, &element, intkeystrings[i]);
  }  

  for (i = 0; i < nof_items_int; i++) {
    int *testptr;

    testptr = (int*)rh_hashmap_lookup(&hashmap_int, intkeystrings[i]);
    assert(intdata[i] == *testptr);
    printf("\tVerified: lookup of item %s.\n", intkeystrings[i]);
  } /* for int items to look up */
  rh_hashmap_destructor(&hashmap_int);





  printf("\tReached: population of char datatype hashmap insertion\n");
  for (i = 0; i < nof_items_char; i++) {
    _generate_key(charkeystrings[i]);
    chardata[i] = (char)(random()%('z' - 'a') + 'a');

	element.data = &chardata[i];
    rh_hashmap_insert(&hashmap_char, &element, charkeystrings[i]);
  }  

  for (i = 0; i < nof_items_char; i++) {
    char *testptr;

    testptr = (char*)rh_hashmap_lookup(&hashmap_char, charkeystrings[i]);
    assert(chardata[i] == *testptr);
    printf("\tVerified: lookup of char item %s.\n", charkeystrings[i]);
  } /* for char items to look up */
  rh_hashmap_destructor(&hashmap_char);




  printf("\tReached: population of struct datatype hashmap insertion\n");
  for (i = 0; i < nof_items_struct; i++) {
    _generate_key(structkeystrings[i]);
    structdata[i].a = (char)(random()%('z' - 'a') + 'a');
    structdata[i].b = (int)random();

	element.data =  &structdata[i];
    rh_hashmap_insert(&hashmap_struct, &element, structkeystrings[i]);
  }  

  for (i = 0; i < nof_items_struct; i++) {
    teststruct_t *testptr;

    testptr = (teststruct_t*)rh_hashmap_lookup(&hashmap_struct, structkeystrings[i]);
    assert(structdata[i].a == testptr->a && structdata[i].b == testptr->b);
    printf("\tVerified: lookup of struct item %s.\n", structkeystrings[i]);
  } /* for char items to look up */
  rh_hashmap_destructor(&hashmap_struct);
  
  return 0;
} /* main */
