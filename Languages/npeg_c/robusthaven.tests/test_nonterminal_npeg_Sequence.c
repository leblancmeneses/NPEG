#include <assert.h>
#include <stdio.h>
#include <string.h>
#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"

#define TESTSTRING1 "test1"
#define TESTSTRING2 "test__2"
#define OTHERSTRING "something else"

static int _expression1(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_Literal(iterator, context, TESTSTRING1, strlen(TESTSTRING1), 1);
}

static int _expression2(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_Literal(iterator, context, TESTSTRING2, strlen(TESTSTRING2), 1);
}

/*
 * - Check that there's no partial consumption in case of mismatch.
 * - Check that there's no undesired multiple matching of repeated occurences.
 * - Check that the order of the expression is heeded.
 */
int main(int argc, char *argv[]) 
{
  const char emptystring[] = "";
  const char test1string1[] = TESTSTRING1"blah";
  const char test1string2[] = TESTSTRING2"blah";
  const char test1string1_1string2[] = TESTSTRING1 TESTSTRING2"blah";
  const char middle_test1string2_1string1[] = OTHERSTRING TESTSTRING1 TESTSTRING2"blah";
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
  assert(npeg_Sequence(&iterator, &context, _expression1, _expression2) == 0 
	 && iterator.index == 0);
  assert(npeg_Sequence(&iterator, &context, _expression2, _expression1) == 0 
	 && iterator.index == 0);
  printf("\tVerified: Handling of empty string.\n");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, test1string1, strlen(test1string1));
  assert(npeg_Sequence(&iterator, &context, _expression1, _expression2) == 0 
	 && iterator.index == 0);
  assert(npeg_Sequence(&iterator, &context, _expression2, _expression1) == 0 
	 && iterator.index == 0);
  npeg_inputiterator_constructor(&iterator, test1string2, strlen(test1string2));
  assert(npeg_Sequence(&iterator, &context, _expression1, _expression2) == 0 
	 && iterator.index == 0);
  assert(npeg_Sequence(&iterator, &context, _expression2, _expression1) == 0 
	 && iterator.index == 0);
  printf("\tVerified: Handling of partial sequence.\n");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, test1string1_1string2, strlen(test1string1_1string2));
  assert(npeg_Sequence(&iterator, &context, _expression1, _expression2) == 1 
	 && iterator.index == strlen(TESTSTRING1) + strlen(TESTSTRING2));
  iterator.index = 0;
  assert(npeg_Sequence(&iterator, &context, _expression2, _expression1) == 0 
	 && iterator.index == 0);
  printf("\tVerified: Handling of full sequence.\n");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, test1string1_1string2, strlen(test1string1_1string2));
  error.message = (char*)errmsg, error.iteratorIndex = 0;
  rh_stack_push(context.errors, &error);
  assert(npeg_Sequence(&iterator, &context, _expression1, _expression2) == 0
	 && iterator.index == 0);
  rh_stack_pop(context.errors);
  puts("\tVerified: Abortion on error without modification of state.");  
  npeg_inputiterator_destructor(&iterator);	

  npeg_inputiterator_constructor(&iterator, middle_test1string2_1string1, 
				 strlen(middle_test1string2_1string1));
  iterator.index = strlen(OTHERSTRING);
  assert(npeg_Sequence(&iterator, &context, _expression1, _expression2) == 1
	 && iterator.index == strlen(OTHERSTRING) + strlen(TESTSTRING1) + strlen(TESTSTRING2));
  iterator.index = strlen(OTHERSTRING);
  assert(npeg_Sequence(&iterator, &context, _expression2, _expression1) == 0
	 && iterator.index == strlen(OTHERSTRING));
  printf("\tVerified: Handling of nonoccurence of full sequence at random position.\n");
  npeg_inputiterator_destructor(&iterator);  

  npeg_destructor(&context);

  return 0;
}
