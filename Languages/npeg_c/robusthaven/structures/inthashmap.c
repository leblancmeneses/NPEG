#include <stdlib.h>
#include <string.h>
#include "inthashmap.h"

void rh_inthashmap_constructor(rh_inthashmap_instance *instance, const uint initial_size) {
  uint i;

  instance->capacity = initial_size;
  instance->baseptr = malloc(instance->capacity*sizeof(rh_inthashmap_element));  

  for (i = 0; i < instance->capacity; i++) {
    instance->baseptr[i].active = 0;
  }
}

void rh_inthashmap_insert(rh_inthashmap_instance *instance, void *data, const int key) {
  uint ikey, idx;

  /*
   * - Compute array index.
   * - Check if array element is free.
   * - If yes: store in array, if not: store in linked list.
   */
  ikey = (uint)key;
  idx = ikey%instance->capacity;
  if (instance->baseptr[idx].active) {
    rh_inthashmap_element *curr_elem;
    rh_inthashmap_element *new_elem;
    
    /*
     * Have to store in linked list 
     */
    new_elem = malloc(sizeof(rh_inthashmap_element));
    new_elem->key = key;
    new_elem->data = data;
    new_elem->next = NULL;

    curr_elem = &instance->baseptr[idx];
    while (curr_elem->next != NULL) curr_elem = curr_elem->next;
    
    curr_elem->next = new_elem;
  } else {
    /*
     * Element is empty, can insert in array.
     */
    instance->baseptr[idx].active = 1;
    instance->baseptr[idx].next = NULL;
    instance->baseptr[idx].data = data;
    instance->baseptr[idx].key = key;
  }
}

void rh_inthashmap_remove(rh_inthashmap_instance *instance, const int key) {
  uint idx;

  /*
   * - Compute array index.
   * - If stored in array:
   * -- If there was no key collision: mark element as inactive.
   * -- Else: Move first linked list element into the array.
   * - If stored in linked list: update linked list.
   */
  idx = ((uint)key)%instance->capacity;
  if (instance->baseptr[idx].active) {
    if (instance->baseptr[idx].key == key) {
      /*
       * stored in array
       */
      if (instance->baseptr[idx].next == NULL) instance->baseptr[idx].active = 0;
      else {
	rh_inthashmap_element *child_elem;

	/*
	 * has child elements
	 */
	child_elem = instance->baseptr[idx].next;
	instance->baseptr[idx].next = child_elem->next;
	instance->baseptr[idx].key = child_elem->key;
	instance->baseptr[idx].data = child_elem->data;
	
	free(child_elem);
      } 
    } else {
      rh_inthashmap_element *curr_elem, *del_elem;

      curr_elem = &instance->baseptr[idx];
      while (curr_elem->next != NULL && curr_elem->next->key != key) curr_elem = curr_elem->next; 
      if (curr_elem->next == NULL) return;
      else {
	del_elem = curr_elem->next;
	curr_elem->next = del_elem->next;
	free(del_elem);
      } /* if found in linked list */
    } /* if in linked list */
  } else return;
}

void* rh_inthashmap_lookup(rh_inthashmap_instance *instance, const int key) {
  uint idx;

  idx = (uint)key%instance->capacity;  
  if (instance->baseptr[idx].active) {
    rh_inthashmap_element *curr_elem;
    
    curr_elem = &instance->baseptr[idx];
    while (curr_elem != NULL && curr_elem->key != key) curr_elem = curr_elem->next;
    if (curr_elem != NULL) return curr_elem->data;
    else return NULL;
  } else return NULL;
}

static void _del_intelem(rh_inthashmap_element *elem) {
  if (elem->next != NULL) _del_intelem(elem->next);
  free(elem);
}

void rh_inthashmap_destructor(rh_inthashmap_instance *instance) {
  uint i;

  for (i = 0; i < instance->capacity; i++) if (instance->baseptr[i].active) {
      if (instance->baseptr[i].next != NULL) _del_intelem(instance->baseptr[i].next);
      instance->baseptr[i].next = NULL;
    } 

  instance->capacity = 0;
  free(instance->baseptr);
}

int rh_inthashmap_contains_key(rh_inthashmap_instance *instance, const int key) {
  return (rh_inthashmap_lookup(instance, key) != NULL);
}


void rh_inthashmap_clear(rh_inthashmap_instance *instance)
{
	rh_inthashmap_destructor(instance);	
	rh_inthashmap_constructor(instance, 1);
}
