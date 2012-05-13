#ifndef ROBUSTHAVEN_STRUCTURES_STACKSTACK_H
#define ROBUSTHAVEN_STRUCTURES_STACKSTACK_H
#include "stack.h"

typedef struct rh_stackstack_instance {
  rh_stack_instance basestack;
  stack_deletion_callback stack_deletion_callback;
} rh_stackstack_instance;

/*
 * Creates a new, empty stack of stacks.
 * The "destructor_callback" is the callback that is applied to the elements of the element stacks,
 * i.e. the callback responsible for freeing up the payload memeory if necessary.
 */
void rh_stackstack_constructor(rh_stackstack_instance *instance, 
			       stack_deletion_callback destructor_callback);

void rh_stackstack_destructor(rh_stackstack_instance* instance);

void rh_stackstack_push_empty_stack(rh_stackstack_instance* instance);

/*
 * Pushes an element onto the top-most stack.
 */
void rh_stackstack_push_on_top(rh_stackstack_instance* instance, void* datatoinsert);

/*
 * Pops an element from the top-most stack.
 */
void* rh_stackstack_pop_from_top(rh_stackstack_instance* instance);

/*
 * Returns the TOS of the top-most stack.
 */
void* rh_stackstack_peek_at_top(rh_stackstack_instance* instance);

/*
 * The popped stack resides in dynamically allocated memory.
 * Free it with "free" after it has been destroyed.
 */
rh_stack_instance* rh_stackstack_pop_stack(rh_stackstack_instance* instance);

/*
 * The peeked stack resides in memory managed by the stack of stacks, so all manipulation will have
 * external effects!
 */
rh_stack_instance* rh_stackstack_peek_stack(rh_stackstack_instance* instance);

/*
 * Removes the top-most item from the stack of stacks and disposes of it.
 */
void rh_stackstack_dispose_tos(rh_stackstack_instance* instance);

/*
 * Counts the number of stacks in the stack of stacks.
 */
int rh_stackstack_count(rh_stackstack_instance* instance);

#endif
