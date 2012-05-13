#include <stdlib.h>
#include <assert.h>
#include "stackstack.h"

static void onfree_stackstack_callback(void *node) {
  rh_stack_destructor((rh_stack_instance*)node);
  free(node);
} 

void rh_stackstack_constructor(rh_stackstack_instance *instance, 
			       stack_deletion_callback destructor_cleaner) {
  rh_stack_constructor(&instance->basestack, onfree_stackstack_callback);
  instance->stack_deletion_callback = destructor_cleaner;
} 

void rh_stackstack_destructor(rh_stackstack_instance* instance) {
  rh_stack_destructor(&instance->basestack);
} 

void rh_stackstack_push_empty_stack(rh_stackstack_instance* instance) {
  rh_stack_instance *p_new_stack;
  
  p_new_stack = (rh_stack_instance*)malloc(sizeof(rh_stack_instance));
  rh_stack_constructor(p_new_stack, instance->stack_deletion_callback);
  
  rh_stack_push(&instance->basestack, (void*)p_new_stack);
} 

void rh_stackstack_push_on_top(rh_stackstack_instance* instance, void* datatoinsert) {
  void *top_stack;

  assert(rh_stack_count(&instance->basestack) > 0);
  top_stack = rh_stack_peek(&instance->basestack);
  rh_stack_push((rh_stack_instance*)top_stack, datatoinsert);
} 

void* rh_stackstack_pop_from_top(rh_stackstack_instance* instance) {
  void *top_stack;
  
  if (rh_stack_count(&instance->basestack) <= 0) return NULL;
  else {
    top_stack = rh_stack_peek(&instance->basestack);

    return rh_stack_pop((rh_stack_instance*)top_stack);  
  }
} 

rh_stack_instance* rh_stackstack_pop_stack(rh_stackstack_instance* instance) {
  void *top_stack;

  if (rh_stack_count(&instance->basestack) <= 0) return NULL;
  else {
    top_stack = rh_stack_pop(&instance->basestack);
  
    return (rh_stack_instance*)top_stack;
  }
} 

rh_stack_instance* rh_stackstack_peek_stack(rh_stackstack_instance* instance) {
  void *top_stack;

  if (rh_stack_count(&instance->basestack) <= 0) return NULL;
  else {
    top_stack = rh_stack_peek(&instance->basestack);
  
    return (rh_stack_instance*)top_stack;
  }
} 

int rh_stackstack_count(rh_stackstack_instance* instance) {
  return rh_stack_count(&instance->basestack);
}

void rh_stackstack_dispose_tos(rh_stackstack_instance* instance) {
  rh_stack_instance *p_tos;

  p_tos = rh_stackstack_pop_stack(instance);
  rh_stack_destructor(p_tos);
  free(p_tos);
} 

void* rh_stackstack_peek_at_top(rh_stackstack_instance* instance) {
  rh_stack_instance *p_tos;

  if (rh_stack_count(&instance->basestack) <= 0) return NULL;
  else {
    p_tos = (rh_stack_instance*)rh_stack_peek(&instance->basestack);

    return rh_stack_peek(p_tos);  
  }
} 
