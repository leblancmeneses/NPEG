#ifndef ROBUSTHAVEN_STRUCTURES_STACK_H
#define ROBUSTHAVEN_STRUCTURES_STACK_H

#include "../types/types.h"

typedef void (*stack_deletion_callback)(void*);

typedef struct rh_stack_instance
{
  void **head, **tail;
  uint capacity;
  stack_deletion_callback onDisposing;
} rh_stack_instance;


// Creates an empty stack structure
// onDisposing is called only when destructor is called explicitly or implicitly by clear();
// if user pops an element out the user is responsible for managing unmanaged memory at that time.
void rh_stack_constructor(rh_stack_instance* instance, stack_deletion_callback onDisposing);


// Deletes all memory allocated for the stack, except the rh_stack_instance object
// if user *data in rh_stack_node contains unmanaged memory the user is encouraged 
// to pass a onDisposing callback to properly clean out custom *data being destroyed.
void rh_stack_destructor(rh_stack_instance* instance);


// a new node will be created (malloc) and linked in the stack.
// argument nodetoinsert are copied to the newly allocated memory.
// Control of the memory (deallocation) of nodetoinsert is left to the user.
// Attention: If NDEBUG is not defined and the stack has 0 capacity, calling this routine will lead
// to an assertion failure.
void rh_stack_push(rh_stack_instance* instance, void* datatoinsert);


// Returns a pointer to the payload data in the top of stack element.
// If the stack is empty, a NULL value is returned. 
// The user is responsible for the management of the memory region to which the returned pointer points.
void* rh_stack_pop(rh_stack_instance* instance);

// Returns a pointer to the payload data in the top of stack element (this ought not to be freed as 
// subsequent calls of peek or pop on the TOS might lead to access violations).
// If the stack is empty, a NULL value is returned. 
void* rh_stack_peek(const rh_stack_instance* instance);


// Counts the number of elements on the stack.
int rh_stack_count(const rh_stack_instance* instance);


// Empties the stack and resets the capacity to the initial value.
void rh_stack_clear(rh_stack_instance* instance);

#endif
