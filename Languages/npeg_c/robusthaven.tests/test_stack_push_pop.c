#include <time.h>
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <string.h>
#include "robusthaven/structures/stack.h"

int main(int argc, char *argv[]) {
  char state[200];
  int i, nof_elems;
  rh_stack_instance stack;
  void **test_elements, *temp_element;  

  /*
   * - Make an array of nof_elems (random) elements.
   * - Push all nof_elems elements (forward traversal)
   * - Traverse array backwards, test that array element and popped stack element match.
   * - Always verify that the stack capacity to used element ratio never exceeds 2
   */
  srand(time(NULL));
  initstate(time(NULL), state, 200);

  rh_stack_constructor(&stack, NULL);

  nof_elems = rand()%135 + 15;
  printf("\tReached: random number of stack size generation.\n");
  test_elements = (void**)malloc(sizeof(void*)*nof_elems);

  for (i = 0; i < nof_elems; i++) {
    test_elements[i] = (void*)random();
    rh_stack_push(&stack, test_elements[i]);
    
    assert(stack.capacity >= stack.head - stack.tail);
    printf("\tVerified: capacity is larger (growing) than stack.head - stack.tail.\n");
  }
  printf("\tReached: end of pushing rh_stack_node(s).\n");


  for (i = nof_elems - 1; i >= 0; i--) {
    temp_element = rh_stack_peek(&stack);
    assert(test_elements[i] == temp_element);

    temp_element = NULL;
    temp_element = rh_stack_pop(&stack);
    assert(test_elements[i] == temp_element);

    /* 
     * This is NO black box testing, numerical values are highly implementation specific -> update
     * when changing stack.o
     */
    assert((double)stack.capacity/(stack.head - stack.tail) <= 2.0 || stack.capacity <= 5);
    printf("\tVerified: Correct readjustment of size of underlying array.\n");
  }
  printf("\tReached: end of poping rh_stack_node(s).\n");


  assert(rh_stack_count(&stack) == 0);
  printf("\tVerified: count after equally pushing and popping of data elements is zero.\n");
  assert(stack.capacity > 0);
  printf("\tVerified: capacity is always greater than zero when stack is empty.\n");

  free(test_elements);
  rh_stack_destructor(&stack);

  return 0;
} /* main */
