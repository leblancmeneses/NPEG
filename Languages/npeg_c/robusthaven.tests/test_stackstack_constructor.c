#include <assert.h>
#include <stdio.h>
#include "robusthaven/structures/stackstack.h"

static void _dummy(void *node) {
}

int main(int argc, char *argv[]) {
  rh_stackstack_instance stackstack;
  
  rh_stackstack_constructor(&stackstack, _dummy);
  assert(rh_stackstack_count(&stackstack) == 0 && rh_stack_count(&stackstack.basestack) == 0);
  assert(stackstack.stack_deletion_callback == _dummy);
  printf("\tVerfied: assignment of all critical fields by constructor.\n");
  rh_stackstack_destructor(&stackstack);
  printf("\tVerfied: memory integrity after destroying empty stack.\n");

  return 0;
} 
