#ifndef ROBUSTHAVEN_STRUCTURES_HASHMAP_H
#define ROBUSTHAVEN_STRUCTURES_HASHMAP_H

#include "../types/types.h"

/*
 * Hybrid hashmap element:
 * Basic hashmap is an array, in case of a hash key collisions elements with the same 
 * hash key are arranged in a linked list.
 */
typedef struct rh_hashmap_element {
  struct rh_hashmap_element *next;
  int active;
  
  /*
   * Attention neither full key strings, nor the data are copied upon insertion: 
   * DO NOT CLEAR THEIR MEMORY.
   * Also please note: data is not a const pointer, in order to simplify manipulation of the 
   * content through the client, however, the content is never manipulated by the hashmap.
   */
  void *data;  
  char *key;
} rh_hashmap_element;

typedef struct rh_hashmap_instance {
  rh_hashmap_element *baseptr;
  uint capacity;
  void (*onFreeing)(rh_hashmap_element*);
} rh_hashmap_instance;

/*
 * Inserts an element into a initialized hashmap.
 */
void rh_hashmap_insert(rh_hashmap_instance *instance, rh_hashmap_element* element, char key[]);

/*
 * Deletes an element from a hashmap.
 * If the key is not found, nothing is deleted and nothing happens.
 */
void rh_hashmap_remove(rh_hashmap_instance *instance, char key[]);

/*
 * Initializes a hashmap.
 * This does not force the use of a prime number for the capacity, but whenever possible try to pass one.
 */
void rh_hashmap_constructor(rh_hashmap_instance *instance, const uint initial_size, 
			    void (*onFreeing)(rh_hashmap_element*));

/*
 * Frees all memory under the control of a hashmap instance (i.e. no data, nor key fields are freed).
 */
void rh_hashmap_destructor(rh_hashmap_instance *instance);

/*
 * Returns the data associated with "key", NULL if key is not found.
 */
void* rh_hashmap_lookup(rh_hashmap_instance *instance, const char key[]);

/*
 * Boolean operation that tells whether a key exists in the given hashmap.
 */
int rh_hashmap_contains_key(rh_hashmap_instance *instance, const char key[]);

/*
 * Frees all memory under the control of a hashmap instance and begins a new memory allocated version.
 */
void rh_hashmap_clear(rh_hashmap_instance *instance);

/*
 * Dummy on free callback that does nothing, in case the data memory has to preserved.
 */
void rh_hashmap_dummy_on_free(rh_hashmap_element* node);

#endif
