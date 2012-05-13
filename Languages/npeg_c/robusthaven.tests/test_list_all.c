#include <time.h>
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <string.h>
#include "robusthaven/structures/list.h"

#define _nof_test_items 150
static int _g_nof_visited_items = 0;
static int _g_visited_items[_nof_test_items];

static void _destructor_callback(void *item) {
  _g_visited_items[_g_nof_visited_items] = *(int*)item;
  _g_nof_visited_items += 1;
}

/*
 * Since the list module is very small, I rolled all tests into one test application:
 * - Check that the list is initialized with non-zero capacity & zero items.
 * - Check that items can be added (beyond the initial capacity).
 * - Check that the order in which items are stored is the same in which they were added.
 */
int main(int argc, char *argv[]) {
  rh_list_instance list;
  int testitems[_nof_test_items], i;

  srand(time(NULL));

  rh_list_constructor(&list, _destructor_callback);
  assert(list.capacity > 0 && rh_list_count(&list) == 0);
  puts("\tVerified: list was initialized properly.");
  
  for (i = 0; i < _nof_test_items; i++) {
    testitems[i] = rand();
  }

  for (i = 0; i < _nof_test_items; i++) {
    rh_list_add(&list, &testitems[i]);
  }
  assert(rh_list_count(&list) == _nof_test_items);
  puts("\tVerified: a great number of items can be put in the list, without access violations");
  
  for (i = 0; i < _nof_test_items; i++) {
    assert(*(int*)rh_list_get_item(&list, i) == testitems[i]);
    printf("\tVerified: item %d was found in the list, and it was in the right place.\n", testitems[i]);
  }

  rh_list_destructor(&list);

  for (i = 0; i < _nof_test_items; i++) {
    assert(_g_visited_items[i] == testitems[i]);
    printf("\tVerified: item %d was visited by destructor callback.\n", testitems[i]);
  }

  return 0;
} 
