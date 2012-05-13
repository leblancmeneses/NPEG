#include <time.h>
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include "robusthaven/structures/stackstack.h"

#define _nof_stacks 5
#define _max_nof_stackelems 10
static const int _nof_stackelems[] = {3, 6, 2, 7, 8};

static int _g_last_visited_stack, _g_last_visited_element;
static int _g_visited[_nof_stacks][_max_nof_stackelems];

static void _onfree_callback(void *node) {
  if (_g_last_visited_element == 0) {
    _g_last_visited_stack -= 1;
    _g_last_visited_element = _nof_stackelems[_g_last_visited_stack] - 1;
  } else _g_last_visited_element -= 1;
  
  _g_visited[_g_last_visited_stack][_g_last_visited_element] = *(int*)node;
} 

/*
 * - Verifies that the destructor really visits all elements upon destruction.
 */
int main(int argc, char *argv[]) {
  int stackelems[_nof_stacks][_max_nof_stackelems];
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
  rh_stackstack_constructor(&stackstack, _onfree_callback);
  
  for (i = 0; i < _nof_stacks; i++) for (k = 0; k < _nof_stackelems[i]; k++) {
      stackelems[i][k] = rand();
    }

  for (i = 0; i < _nof_stacks; i++) {
    rh_stackstack_push_empty_stack(&stackstack);
    for (k = 0; k < _nof_stackelems[i]; k++) {
      item = &stackelems[i][k];
      rh_stackstack_push_on_top(&stackstack, item);
    }
  }
  printf("Reached: end of pushing\n");

  _g_last_visited_stack = _nof_stacks - 1;
  _g_last_visited_element = _nof_stackelems[_g_last_visited_stack];
  rh_stackstack_destructor(&stackstack);
  
  for (i = 0; i < _nof_stacks; i++) for (k = 0; k < _nof_stackelems[i]; k++) {
      assert(_g_visited[i][k] == stackelems[i][k]);
      puts("\tVerified: element was visited by destructor");
    }

  return 0;
}
