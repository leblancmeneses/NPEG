#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <string.h>
#include "robusthaven/structures/stack.h"

int main(int argc, char *argv[]) {
  typedef struct {
    char a;
    int b;
  } teststruct_t;

#define nof_ints 3
#define nof_chars 4
#define nof_structs 5
  
  teststruct_t structdata[nof_structs];
  int intdata[nof_ints], i;
  char chardata[nof_chars];
  rh_stack_instance intstack, charstack, structstack;
  void *p_tmp;

  rh_stack_constructor(&intstack, NULL);
  rh_stack_constructor(&charstack, NULL);
  rh_stack_constructor(&structstack, NULL);


  printf("\tReached: populating stack with int datatype as data value.\n");

  for (i = 0; i < nof_ints; i++) {
    intdata[i] = i;
    rh_stack_push(&intstack, &intdata[i]);
    
    assert(intstack.capacity >= intstack.head - intstack.tail);
    printf("\tVerified: Capacity increased to make room for all pushed elements.\n");
  }

  for (i = nof_ints - 1; i >= 0; i--) {
    p_tmp = rh_stack_pop(&intstack);
    
    assert(*((int*)p_tmp) == intdata[i]);
    printf("\tVerified: Pushed elements can be properly retrieved.\n");
  }

  printf("\tReached: populating stack with char datatype as data value.\n");
  for (i = 0; i < nof_chars; i++) {
    chardata[i] = 'a' + i;
    rh_stack_push(&charstack, &chardata[i]);

    assert(charstack.capacity >= charstack.head - charstack.tail);
    printf("\tVerified: Capacity increased to make room for all pushed elements.\n");
  }

  for (i = nof_chars - 1; i >= 0; i--) {
    p_tmp = rh_stack_pop(&charstack);
    
    assert(*(char*)p_tmp == chardata[i]);
    printf("\tVerified: Pushed elements can be properly retrieved.\n");
  }

  printf("\tReached: populating stack with struct datatype as data value.\n");
  for (i = 0; i < nof_structs; i++) {
    structdata[i].a = 'b' + i;
    structdata[i].b = nof_ints + i;
    rh_stack_push(&structstack, &structdata[i]);

    assert(structstack.capacity >= structstack.head - structstack.tail);
    printf("\tVerified: Capacity increased to make room for all pushed elements.\n");
  }

  for (i = nof_structs - 1; i >= 0; i--) {
    p_tmp = rh_stack_pop(&structstack);
    
    assert(((teststruct_t*)p_tmp)->a == structdata[i].a && ((teststruct_t*)p_tmp)->b == structdata[i].b);
    printf("\tVerified: Pushed elements can be properly retrieved.\n");
  }  

  rh_stack_destructor(&intstack);
  rh_stack_destructor(&charstack);
  rh_stack_destructor(&structstack);

  return 0;
} /* main */
