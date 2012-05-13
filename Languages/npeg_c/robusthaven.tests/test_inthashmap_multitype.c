#include <stdlib.h>
#include <time.h>
#include <stdio.h>
#include <assert.h>
#include "robusthaven/structures/inthashmap.h"

static const uint _max_keylen = 30;

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

#define start_cap_int 71
#define start_cap_char 91
#define start_cap_struct 113
#define nof_items_int 2*start_cap_int
#define nof_items_char 2*start_cap_char
#define nof_items_struct 2*start_cap_struct
  
  char state[200];
  int intkeys[nof_items_int];
  int intdata[nof_items_int];
  int charkeys[nof_items_char];
  char chardata[nof_items_char];
  int structkeys[nof_items_struct];
  teststruct_t structdata[nof_items_struct];
  rh_inthashmap_instance hashmap_int, hashmap_char, hashmap_struct;
  uint i;
  
  initstate(time(NULL), state, 200);

  rh_inthashmap_constructor(&hashmap_int, start_cap_int);
  rh_inthashmap_constructor(&hashmap_char, start_cap_char);
  rh_inthashmap_constructor(&hashmap_struct, start_cap_struct);

  printf("\tReached: population of int datatype hashmap insertion\n");
  for (i = 0; i < nof_items_int; i++) {
    intkeys[i] = (int)random();
    intdata[i] = (int)random();

    rh_inthashmap_insert(&hashmap_int, &intdata[i], intkeys[i]);
  }  

  for (i = 0; i < nof_items_int; i++) {
    int *testptr;

    testptr = (int*)rh_inthashmap_lookup(&hashmap_int, intkeys[i]);
    assert(intdata[i] == *testptr);
    printf("\tVerified: int lookup of item %d.\n", intkeys[i]);
  } /* for int items to look up */
  rh_inthashmap_destructor(&hashmap_int);

  printf("\tReached: population of char datatype hashmap insertion\n");
  for (i = 0; i < nof_items_char; i++) {
    charkeys[i] = (int)random();
    chardata[i] = (char)(random()%('z' - 'a') + 'a');

    rh_inthashmap_insert(&hashmap_char, &chardata[i], charkeys[i]);
  }  

  for (i = 0; i < nof_items_char; i++) {
    char *testptr;

    testptr = (char*)rh_inthashmap_lookup(&hashmap_char, charkeys[i]);
    assert(chardata[i] == *testptr);
    printf("\tVerified: char hashmap lookup of item %d.\n", charkeys[i]);
  } /* for char items to look up */
  rh_inthashmap_destructor(&hashmap_char);

  printf("\tReached: population of struct datatype hashmap insertion\n");
  for (i = 0; i < nof_items_struct; i++) {
    structkeys[i] = (int)random();
    structdata[i].a = (char)(random()%('z' - 'a') + 'a');
    structdata[i].b = (int)random();

    rh_inthashmap_insert(&hashmap_struct, &structdata[i], structkeys[i]);
  }  

  for (i = 0; i < nof_items_struct; i++) {
    teststruct_t *testptr;

    testptr = (teststruct_t*)rh_inthashmap_lookup(&hashmap_struct, structkeys[i]);
    assert(structdata[i].a == testptr->a && structdata[i].b == testptr->b);
    printf("\tVerified: struct hashmap lookup of item %d.\n", structkeys[i]);
  } /* for char items to look up */
  rh_inthashmap_destructor(&hashmap_struct);
  
  return 0;
} /* main */
