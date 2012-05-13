#ifndef ROBUSTHAVEN_STRUCTURES_LIST_H
#define ROBUSTHAVEN_STRUCTURES_LIST_H

#include "../types/types.h"

typedef void (*rh_list_ondestruct_callback)(void*);

// Since this is for storing warnings, items will likely never be removed, thus an array backend
// is more suitable than a linked list approach.
typedef struct rh_list_instance {
  // Head is a pointer to the first item in the list, tail points behind the one that was added last.
  void **head, **tail;
  uint capacity;
  // The destructor is applied to all active buckets upon list destruction.
  rh_list_ondestruct_callback elementdestructor;
} rh_list_instance;

// Initializes a list structure.
void rh_list_constructor(rh_list_instance *instance, rh_list_ondestruct_callback elementdestructor);

// Frees up all memory allocated for a list.
void rh_list_destructor(rh_list_instance *instance);

// Appends an element to the end of the list.
void rh_list_add(rh_list_instance *instance, void *item);

// Counts the number of elements that are stored in the list.
uint rh_list_count(rh_list_instance *instance);

// Retrieves the element with the given index (older elements have lower indices).
// Valid indices go from 0 to rh_list_count(instance) - 1, NULL is returned if that range is exceeded.
void* rh_list_get_item(rh_list_instance *instance, const uint index);

/*
 * Restarts the list with the same destructor, but removes all stored items.
 */
void rh_list_clear(rh_list_instance* instance);
#endif
