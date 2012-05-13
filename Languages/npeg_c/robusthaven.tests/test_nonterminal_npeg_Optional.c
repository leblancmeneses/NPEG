#include <assert.h>
#include <stdio.h>
#include <string.h>
#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"

#define TESTSTRING "test"
#define OTHERSTRING "somthing else"

static int _expression(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_Literal(iterator, context, TESTSTRING, strlen(TESTSTRING), 1);
} 

int main(int argc, char *argv[]) 
{
  const char emptystring[] = "";
  const char otherstring[] = "blah";
  const char test1string[] = TESTSTRING"Tes";
  const char test2string[] = TESTSTRING TESTSTRING"Tes";
  const char middle_test2string[] = OTHERSTRING TESTSTRING TESTSTRING"blah";
  const char errmsg[] = "some kind of error";

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

  npeg_constructor(&context, NULL);

  npeg_inputiterator_constructor(&iterator, emptystring, 0);
  assert(npeg_Optional(&iterator, &context, _expression) == 1 && iterator.index == 0);
  printf("\tVerified: Handling of empty string.\n");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, otherstring, strlen(otherstring));
  assert(npeg_Optional(&iterator, &context, _expression) == 1 && iterator.index == 0);
  printf("\tVerified: Handling of 0 occurrence in non-empty string.\n");	
  npeg_inputiterator_destructor(&iterator);	

  npeg_inputiterator_constructor(&iterator, test1string, strlen(test1string));
  assert(npeg_Optional(&iterator, &context, _expression) == 1 
	 && iterator.index == strlen(TESTSTRING));
  printf("\tVerified: Handling of single occurrence.\n");	
  npeg_inputiterator_destructor(&iterator);	

  npeg_inputiterator_constructor(&iterator, test1string, strlen(test1string));
  error.message = (char*)errmsg, error.iteratorIndex = 0;
  rh_stack_push(context.errors, &error);
  assert(npeg_Optional(&iterator, &context, _expression) == 0 && iterator.index == 0);
  rh_stack_pop(context.errors);
  puts("\tVerified: Abortion on error without modification of state.");  
  npeg_inputiterator_destructor(&iterator);	

  npeg_inputiterator_constructor(&iterator, test2string, strlen(test2string));
  assert(npeg_Optional(&iterator, &context, _expression) == 1 
	 && iterator.index == strlen(TESTSTRING));
  printf("\tVerified: Single consumption of double occurrence.\n");	
  npeg_inputiterator_destructor(&iterator);		

  npeg_inputiterator_constructor(&iterator, middle_test2string, strlen(middle_test2string));
  iterator.index = strlen(OTHERSTRING);  
  assert(npeg_Optional(&iterator, &context, _expression) == 1 
	 && iterator.index == strlen(TESTSTRING) + strlen(OTHERSTRING));
  printf("\tVerified: Single consumption of double occurrence at string center.\n");	
  npeg_inputiterator_destructor(&iterator);		  

  npeg_destructor(&context);

  return 0;
}
