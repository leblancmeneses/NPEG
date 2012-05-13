#include <stdlib.h>
#include <time.h>
#include <stdio.h>
#include <assert.h>
#include "robusthaven/structures/inthashmap.h"

/*
 * Runs some tests on a newly created hashmap:
 * - Test correct capacity
 * - Test inactivity of all fields
 */
int main(int argc, char *argv[]) {
  rh_inthashmap_instance hashmap;
  uint i, start_cap;

  srand(time(NULL));
  start_cap = rand()%31;
  
  rh_inthashmap_constructor(&hashmap, start_cap);
  assert(hashmap.capacity == start_cap);
  printf("\tVerified: hashmap constructor allocates memory specified by constructor argument.\n");

  for (i = 0; i < hashmap.capacity; i++) {
    assert(!hashmap.baseptr[i].active);
  }
  printf("\tVerified: all buckets are marked inactive.\n");

  rh_inthashmap_destructor(&hashmap);

  return 0;
} /* main */
