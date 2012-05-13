#include <time.h>
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include "robusthaven/structures/stackstack.h"

/*
 * - Verifies that elements when pushed are stored in the right stack.
 * - Verfies that no elements are lost.
 */
int main(int argc, char *argv[]) {
  const int nof_stacks = 5;
  const int max_nof_stackelems = 10;
  const int nof_stackelems[] = {3, 6, 2, 7, 8};

  int stackelems[nof_stacks][max_nof_stackelems];
  rh_stackstack_instance stackstack;
  void *item;
  int i, k;
  
  /*
   * - Create random ints in 2D array.
   * - Push elements on stack, according to which line of the random int array they were taken from.
   * - Pop the elements from the stack and check that the association between stacks and elements
   * was preserved (Attention order is reversed!).
   */
  srand(time(NULL));
  rh_stackstack_constructor(&stackstack, NULL);
  
  for (i = 0; i < nof_stacks; i++) for (k = 0; k < nof_stackelems[i]; k++) {
      stackelems[i][k] = rand();
    }

  for (i = 0; i < nof_stacks; i++) {
    rh_stackstack_push_empty_stack(&stackstack);
    for (k = 0; k < nof_stackelems[i]; k++) {
      item = &stackelems[i][k];
      rh_stackstack_push_on_top(&stackstack, item);
    }
    
    item = rh_stack_peek(&stackstack.basestack);
    assert(rh_stack_count((rh_stack_instance*)item) == nof_stackelems[i]);
    printf("\tVerified: number of elements in top-most stack matches expectation\n");
  }
  printf("Reached: end of pushing\n");

  for (i = nof_stacks - 1; i >= 0; i--) {
    for (k = nof_stackelems[i] - 1; k >= 0; k--) {
      rh_stack_instance *p_topstack;

      item = rh_stack_peek(&stackstack.basestack);
      p_topstack = (rh_stack_instance*)item;
      item = rh_stack_peek(p_topstack);
      assert(*(int*)item == stackelems[i][k]);
      printf("\tVerified: content of top of stack via rh_stack routines\n");

      item = rh_stackstack_pop_from_top(&stackstack);
      assert(*(int*)item == stackelems[i][k]);
      printf("\tVerified: popped content is the expected\n");
    }
    rh_stackstack_dispose_tos(&stackstack);
  }

  rh_stackstack_destructor(&stackstack);

  return 0;
} 
