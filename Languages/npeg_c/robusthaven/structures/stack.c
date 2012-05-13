#include <assert.h>
#include <stdlib.h>
#include <string.h>
#include "stack.h"

static const uint _start_capacity = 5;
static const double _multiplier = 2.0;

void rh_stack_constructor(rh_stack_instance* instance, stack_deletion_callback onFreeing) 
{
  instance->capacity = _start_capacity;
  instance->tail = (void**)malloc(sizeof(void*)*instance->capacity);  
  instance->head = instance->tail;
  instance->onDisposing = onFreeing;
}

void rh_stack_destructor(rh_stack_instance* instance) 
{
  void **start = instance->tail;
  int counter = rh_stack_count(instance);
	
  if(instance->onDisposing != NULL)
    {
      while(counter-- > 0)
	{
	  instance->onDisposing(start[counter]);		
	}
    }
  free(instance->tail);
  instance->capacity = 0;
  instance->tail = instance->head = NULL;
}

int rh_stack_count(const rh_stack_instance* instance) 
{
  return (instance->head - instance->tail);
} 

void rh_stack_push(rh_stack_instance* instance, void* datatoinsert) 
{
  uint nof_used;

  assert(instance->capacity > 0);

  nof_used = instance->head - instance->tail;
  if (nof_used + 1 >= instance->capacity) {
    instance->capacity = (uint)(instance->capacity*_multiplier);
    instance->tail = (void**)realloc(instance->tail, instance->capacity*sizeof(void*));
    instance->head = instance->tail + nof_used;
  } 

  *(instance->head) = datatoinsert;
  instance->head += 1;
}


void* rh_stack_pop(rh_stack_instance* instance) 
{
  void *p_data;

  /*
   * - Use the peek routine to get the top element.
   * - If the stack is not empty remove the top element.
   * - If the ratio capacity/(used elements) > multiplier, reduce the stack size by that factor.
   */
  p_data = rh_stack_peek(instance);
  if (!rh_stack_count(instance) == 0) 
  {
    uint nof_used;

    instance->head -= 1;
    nof_used = instance->head - instance->tail;
    if ((double)instance->capacity/(double)(nof_used) > _multiplier  
	&& instance->capacity > _start_capacity) {
      instance->tail = realloc(instance->tail, (uint)(instance->capacity/_multiplier)*sizeof(void*));
      instance->head = instance->tail + nof_used;
      instance->capacity = (uint)(instance->capacity/_multiplier);
    }
  }

  return p_data;
}


void* rh_stack_peek(const rh_stack_instance* instance) 
{
  void *p_data;
  
  if (rh_stack_count(instance) > 0) p_data = *(instance->head - 1);
  else p_data = NULL;

  return p_data;
}


void rh_stack_clear(rh_stack_instance* instance) 
{
  rh_stack_destructor(instance);
  rh_stack_constructor(instance, instance->onDisposing);
}
