#ifndef ROBUSTHAVEN_STRUCTURES_INTHASHMAP_H
#define ROBUSTHAVEN_STRUCTURES_INTHASHMAP_H

#include "../types/types.h"

/*********************************************************************************************************/
/*
 * Hybrid hashmap element:
 * Basic hashmap is an array, in case of a hash key collisions elements with the same 
 * hash key are arranged in a linked list.
 */
typedef struct rh_inthashmap_element {
  struct rh_inthashmap_element *next;
  int active;  
  void *data;  
  int key;
} rh_inthashmap_element;

typedef struct rh_inthashmap_instance {
  rh_inthashmap_element *baseptr;
  uint capacity;
} rh_inthashmap_instance;

/*
 * Inserts an element into a initialized hashmap.
 */
void rh_inthashmap_insert(rh_inthashmap_instance *instance, void *data, const int key);

/*
 * Deletes an element from a hashmap.
 * If the key is not found, nothing is deleted and nothing happens.
 */
void rh_inthashmap_remove(rh_inthashmap_instance *instance, const int key);

/*
 * Initializes a hashmap.
 * This does not force the use of a prime number for the capacity, but whenever possible try to pass one.
 */
void rh_inthashmap_constructor(rh_inthashmap_instance *instance, const uint initial_size);

/*
 * Frees all memory under the control of a hashmap instance (i.e. no data, nor key fields are freed).
 */
void rh_inthashmap_destructor(rh_inthashmap_instance *instance);

/*
 * Returns the data associated with "key", NULL if key is not found.
 */
void* rh_inthashmap_lookup(rh_inthashmap_instance *instance, const int key);

/*
 * Boolean operation that tells whether a key exists in the given hashmap.
 */
int rh_inthashmap_contains_key(rh_inthashmap_instance *instance, const int key);

/*
 * Frees all memory under the control of a hashmap instance and begins a new memory allocated version.
 */
void rh_inthashmap_clear(rh_inthashmap_instance *instance);

#endif
