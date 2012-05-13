#include <assert.h>
#include <stdio.h>
#include <string.h>
#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"

#define TESTSTRING1 "test"
#define TESTSTRING2 "TEST"
#define OTHERSTRING "somthing else"

static int _sub_expression1(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_Literal(iterator, context, TESTSTRING1, strlen(TESTSTRING1), 1);
} 

static int _sub_expression2(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_Literal(iterator, context, TESTSTRING1, strlen(TESTSTRING1), 0);
} 

static int _expression1(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_AndPredicate(iterator, context, _sub_expression1) 
    && _sub_expression2(iterator, context);
}

static int _expression2(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_AndPredicate(iterator, context, _sub_expression1) 
    && _sub_expression2(iterator, context);
}

/*
 * With all tests, also test commutativity.
 * - Does AND handle the empty string properly?
 * - Does AND only consume the string once?
 * - Does and consume nothing if one predicate is false?
 * - Does AND still work properly in the middle of a string?
 */
int main(int argc, char *argv[]) 
{
  const char emptystring[] = "";
  const char test1string[] = TESTSTRING1"blah";
  const char not_test1string[] = TESTSTRING2"blah";
  const char test2string[] = TESTSTRING1 TESTSTRING1 TESTSTRING2"blah";
  const char middle_test2string[] = OTHERSTRING TESTSTRING1 TESTSTRING1 TESTSTRING2"blah";
  const char errmsg[] = "some kind of error";

  npeg_inputiterator iterator;
  rh_stack_instance disable_back_reference, errors;
  rh_list_instance warnings;
  rh_stackstack_instance ast_stack;
  rh_hashmap_instance lookup;
  npeg_context context;
  npeg_error error;
  
  context.disableBackReferenceStack = &disable_back_reference;
  context.sandbox = &ast_stack;
  context.backReferenceLookup = &lookup;
  context.warnings = &warnings; 
  context.errors = &errors;

  npeg_constructor(&context, NULL);
  
  npeg_inputiterator_constructor(&iterator, emptystring, 0);
  assert(_expression1(&iterator, &context) == 0 && iterator.index == 0);
  assert(_expression2(&iterator, &context) == 0 && iterator.index == 0);
  printf("\tVerified: Handling of empty string.\n");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, test1string, strlen(test1string));
  assert(_expression1(&iterator, &context) == 1 && iterator.index == strlen(TESTSTRING1));
  iterator.index = 0;
  assert(_expression2(&iterator, &context) == 1 && iterator.index == strlen(TESTSTRING1));
  printf("\tVerified: Handling of single occurence string.\n");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, test1string, strlen(test1string));
  error.message = (char*)errmsg, error.iteratorIndex = 0;
  rh_stack_push(context.errors, &error);
  assert(_expression1(&iterator, &context) == 0 && iterator.index == 0);
  rh_stack_pop(context.errors);
  printf("\tVerified: Abortion on error without modification of state.\n");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, not_test1string, strlen(not_test1string));
  assert(_expression1(&iterator, &context) == 0 && iterator.index == 0);
  assert(_expression2(&iterator, &context) == 0 && iterator.index == 0);
  printf("\tVerified: Handling of single occurence string.\n");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, test2string, strlen(test2string));
  assert(_expression1(&iterator, &context) == 1 && iterator.index == strlen(TESTSTRING1));
  iterator.index = 0;
  assert(_expression2(&iterator, &context) == 1 && iterator.index == strlen(TESTSTRING1));
  printf("\tVerified: Handling of double occurence string.\n");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, middle_test2string, strlen(middle_test2string));
  iterator.index = strlen(OTHERSTRING);
  assert(_expression1(&iterator, &context) == 1 && iterator.index 
	 == strlen(OTHERSTRING) + strlen(TESTSTRING1));
  iterator.index = strlen(OTHERSTRING);
  assert(_expression2(&iterator, &context) == 1 && iterator.index 
	 == strlen(OTHERSTRING) + strlen(TESTSTRING1));
  printf("\tVerified: Handling of double occurence at center of string.\n");
  npeg_inputiterator_destructor(&iterator);

  npeg_destructor(&context);

  return 0;
}
