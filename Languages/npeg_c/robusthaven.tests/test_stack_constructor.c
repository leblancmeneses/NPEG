#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <string.h>
#include "robusthaven/structures/stack.h"

/*
 * Tests the constructor/destructors of the stack module
 */
int main(int argc, char *argv[]) {
  rh_stack_instance stack;

  rh_stack_constructor(&stack, NULL);
  assert(stack.head == stack.tail);
  printf("\tVerified: Correct pointer assignment.\n");

  assert(rh_stack_count(&stack) == 0);
  printf("\tVerified: intial number of items.\n");
  rh_stack_destructor(&stack);
 
  return 0;
} /* main */
