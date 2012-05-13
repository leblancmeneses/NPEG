#include <assert.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <time.h>
#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"

/*
 * - Test that consumption of an empty string results in failure.
 * - Test that consumption of a random string works until the end of the string is reached.
 */
int main(int argc, char *argv[]) 
{
#define randstringlen 10
  const char errmsg[] = "some kind of error";

  char emptystring[] = "";
  char randomstring[randstringlen];
  npeg_inputiterator iterator;
  rh_stack_instance disable_back_reference, errors;
  rh_list_instance warnings;
  rh_stackstack_instance ast_stack;
  rh_hashmap_instance lookup;
  npeg_error error;
  npeg_context context = {.disableBackReferenceStack = &disable_back_reference,
			  .sandbox = &ast_stack,
			  .backReferenceLookup = &lookup,
			  .warnings = &warnings,
			  .errors = &errors};
  int i;

  npeg_constructor(&context, NULL);	
  srand(time(NULL));

  npeg_inputiterator_constructor(&iterator, emptystring, 0);
  assert(npeg_AnyCharacter(&iterator, &context) == 0);
  npeg_inputiterator_destructor(&iterator);
  printf("\tVerified: handling of empty string\n");

  for (i = 0; i < randstringlen; i++) {
    randomstring[i] = rand()%('z' - 'a') + 'a';
  }
  
  npeg_inputiterator_constructor(&iterator, randomstring, randstringlen);
  for (i = 0; i < randstringlen; i++) 
  {
    assert(npeg_inputiterator_get_current(&iterator) == randomstring[i]);
    assert(npeg_AnyCharacter(&iterator, &context) == 1);
  }

  assert(npeg_AnyCharacter(&iterator, &context) == 0);
  printf("\tVerified: handling of end of string\n");
  
  npeg_inputiterator_destructor(&iterator);
  printf("\tVerified: consumption of random string\n");

  npeg_inputiterator_constructor(&iterator, randomstring, randstringlen);
  error.message = (char*)errmsg, error.iteratorIndex = 0;
  rh_stack_push(context.errors, &error);
  assert(npeg_AnyCharacter(&iterator, &context) == 0 && iterator.index == 0);
  rh_stack_pop(context.errors);
  puts("\tVerified: Abortion on error without modification of state.");  
  npeg_inputiterator_destructor(&iterator);

  npeg_destructor(&context);
  	
  return 0;
}
