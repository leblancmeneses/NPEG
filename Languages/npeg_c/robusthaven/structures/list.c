#include <stdlib.h>
#include "list.h"

static const int _start_capacity = 5;
static const double _expansion_multiplier = 2.0;

void rh_list_constructor(rh_list_instance *instance, rh_list_ondestruct_callback elementdestructor) {
  instance->capacity = _start_capacity;
  instance->elementdestructor = elementdestructor;
  instance->tail = (instance->head = (void**)malloc(sizeof(void*)*_start_capacity));  
}

void rh_list_destructor(rh_list_instance *instance) {
  uint i;

  if (instance->elementdestructor) for (i = 0; i < rh_list_count(instance); i++) {
      instance->elementdestructor(instance->head[i]);
    }
    
  free(instance->head);
  instance->capacity = 0;
  instance->head = instance->tail = NULL;
}

uint rh_list_count(rh_list_instance *instance) {
  return instance->tail - instance->head;
} 

void rh_list_add(rh_list_instance *instance, void *item) {
  const uint old_nof_items = rh_list_count(instance);

  if (old_nof_items + 1 >= instance->capacity) {    
    instance->capacity *= _expansion_multiplier;
    instance->head = (void**)realloc(instance->head, sizeof(void*)*instance->capacity);
    instance->tail = instance->head + old_nof_items;
  }

  *instance->tail = item;
  instance->tail += 1;
}

void* rh_list_get_item(rh_list_instance *instance, const uint index) {
  if (index < rh_list_count(instance)) return instance->head[index];
  else return NULL;
} 

void rh_list_clear(rh_list_instance* instance) {
  rh_list_destructor(instance);
  rh_list_constructor(instance, instance->elementdestructor);
}
