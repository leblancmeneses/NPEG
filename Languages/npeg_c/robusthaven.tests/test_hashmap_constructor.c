#include <stdlib.h>
#include <time.h>
#include <stdio.h>
#include <assert.h>
#include "robusthaven/structures/hashmap.h"

/*
 * Runs some tests on a newly created hashmap:
 * - Test correct capacity
 * - Test inactivity of all fields
 */
int main(int argc, char *argv[]) {
  rh_hashmap_instance hashmap;
  uint i, start_cap;

  srand(time(NULL));
  start_cap = rand()%31;
  
  rh_hashmap_constructor(&hashmap, start_cap, NULL);
  assert(hashmap.capacity == start_cap);
  printf("\tVerified: hashmap constructor allocates memory specified by constructor argument.\n");

  for (i = 0; i < hashmap.capacity; i++) {
    assert(!hashmap.baseptr[i].active);
  }
  printf("\tVerified: all buckets are marked inactive.\n");

  rh_hashmap_destructor(&hashmap);

  return 0;
} /* main */
