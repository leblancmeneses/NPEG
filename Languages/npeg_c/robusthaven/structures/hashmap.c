#include <stdlib.h>
#include <string.h>
#include "hashmap.h"

void rh_hashmap_constructor(rh_hashmap_instance *instance, const uint initial_size, void (*onFreeing)(rh_hashmap_element*)) {
  uint i;

  instance->onFreeing = onFreeing;
  instance->capacity = initial_size;
  instance->baseptr = malloc(instance->capacity*sizeof(rh_hashmap_element));  

  for (i = 0; i < instance->capacity; i++) {
    instance->baseptr[i].active = 0;
  }
}


/*
 * Recursive linked list deletion routine
 */
static void _del_elem(rh_hashmap_instance *instance, rh_hashmap_element *elem) 
{
	if (elem->next != NULL)
	{ 
		_del_elem(instance, elem->next);
	}
	
	
	if(instance->onFreeing != NULL)
	{
		instance->onFreeing(elem);
	}
	free(elem);
}

void rh_hashmap_destructor(rh_hashmap_instance *instance) {
	uint i;

	for (i = 0; i < instance->capacity; i++) 
	{
		if (instance->baseptr[i].active)
		{
			if (instance->baseptr[i].next != NULL) 
			{
				_del_elem(instance, instance->baseptr[i].next);
			}
			instance->baseptr[i].next = NULL;

			if(instance->onFreeing != NULL) {
			  instance->onFreeing(&instance->baseptr[i]);
			}
		} 
	}

	instance->capacity = 0;
	free(instance->baseptr);
}

static uint _string_to_integer(const char skey[]) {
  uint ikey, i;
  const char *p_char;

  ikey = 0;
  for (p_char = skey, i = 0; *p_char != 0; p_char++, i++) {
    ikey = ikey + ((uint)(*p_char - 'a') << i);
  }

  return ikey;
}

void rh_hashmap_insert(rh_hashmap_instance *instance, rh_hashmap_element* element, char key[]) 
{
  uint ikey, idx;

  /*
   * - Compute array index.
   * - Check if array element is free.
   * - If yes: store in array, if not: store in linked list.
   */
  ikey = _string_to_integer(key);
  idx = ikey%instance->capacity;
  if (instance->baseptr[idx].active) 
  {
    rh_hashmap_element *curr_elem;
    rh_hashmap_element *new_elem;
    
    /*
     * Have to store in linked list 
     */
    new_elem = malloc(sizeof(rh_hashmap_element));
    new_elem->key = key;
    new_elem->data = element->data;
    new_elem->next = NULL;

    curr_elem = &instance->baseptr[idx];
    while (curr_elem->next != NULL) curr_elem = curr_elem->next;
    
    curr_elem->next = new_elem;
  }
  else
  {
    /*
     * Element is empty, can insert in array.
     */
    instance->baseptr[idx].active = 1;
    instance->baseptr[idx].next = NULL;
    instance->baseptr[idx].data = element->data;
    instance->baseptr[idx].key = key;
  }
}

void rh_hashmap_remove(rh_hashmap_instance *instance, char key[]) {
  uint idx;

  /*
   * - Compute array index.
   * - If stored in array:
   * -- If there was no key collision: mark element as inactive.
   * -- Else: Move first linked list element into the array.
   * - If stored in linked list: update linked list.
   */
  idx = _string_to_integer(key)%instance->capacity;
  if (instance->baseptr[idx].active) 
  {
    if (!strcmp(instance->baseptr[idx].key, key)) 
    {
      /*
       * stored in array
       */
      if (instance->baseptr[idx].next == NULL) 
      {
	      instance->baseptr[idx].active = 0;
      }
      else
      {
	rh_hashmap_element *child_elem;
		
	/*
	 * has child elements
	 */
	if( instance->onFreeing != NULL)
	  {
	    instance->onFreeing(&instance->baseptr[idx]);
	  }

	child_elem = instance->baseptr[idx].next;
	instance->baseptr[idx].next = child_elem->next;
	instance->baseptr[idx].key = child_elem->key;
	instance->baseptr[idx].data = child_elem->data;	
	free(child_elem);
      } 
    }
    else 
    {
		rh_hashmap_element *curr_elem, *del_elem;
		
		curr_elem = &instance->baseptr[idx];
		while (curr_elem->next != NULL && strcmp(curr_elem->next->key, key))
		{ 
			curr_elem = curr_elem->next; 
		}
		
      if (curr_elem->next == NULL)
      { 
		return;
      }
      else 
      {
		del_elem = curr_elem->next;
		curr_elem->next = del_elem->next;
		if( instance->onFreeing != NULL)
		{
			instance->onFreeing(del_elem);
		}
		free(del_elem);
      } /* if found in linked list */
    } /* if in linked list */
  } 
  else 
  {
	  return;
  }
}

void* rh_hashmap_lookup(rh_hashmap_instance *instance, const char key[]) {
  uint idx;

  idx = _string_to_integer(key)%instance->capacity;  
  if (instance->baseptr[idx].active) {
    rh_hashmap_element *curr_elem;
    
    curr_elem = &instance->baseptr[idx];
    while (curr_elem != NULL && strcmp(curr_elem->key, key)) curr_elem = curr_elem->next;
    if (curr_elem != NULL) return curr_elem->data;
    else return NULL;
  } else return NULL;
}

int rh_hashmap_contains_key(rh_hashmap_instance *instance, const char key[]) {
  return (rh_hashmap_lookup(instance, key) != NULL);
}

void rh_hashmap_dummy_on_free(rh_hashmap_element* node) {
}

void rh_hashmap_clear(rh_hashmap_instance *instance)
{
	rh_hashmap_destructor(instance);	
	rh_hashmap_constructor(instance, 1, instance->onFreeing);
}
