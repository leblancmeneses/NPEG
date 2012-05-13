#include <assert.h>
#include <stdio.h>
#include <string.h>
#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"

#define TESTSTRING1 "test"
#define TESTSTRING2 "TEST"
#define OTHERSTRING "something else"

static int _match_expression1(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_Literal(iterator, context, TESTSTRING1, strlen(TESTSTRING1), 1);
} 

static int _match_expression2(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_Literal(iterator, context, TESTSTRING1, strlen(TESTSTRING1), 0);
} 

static const char _name1[] = "1st teststring";

static int _capture_expression1(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_CapturingGroup(iterator,  context, _match_expression1, _name1, 0, 0);
} 

static const char _name2[] = "2nd teststring";

static int _capture_expression2(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_CapturingGroup(iterator,  context, _match_expression2, _name2, 0, 0);
} 

static int _match_expression12(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_Sequence(iterator,  context, _capture_expression1, _capture_expression2);
}

static const char _name3[] = "1/2 sequence";

static int _capture_expression3(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_CapturingGroup(iterator,  context, _match_expression12, _name3, 0, 0);
} 

static const char _name4[] = "2 sequence unreduced";

static int _capture_expression2_red(npeg_inputiterator *iterator, npeg_context *context) {
  return npeg_CapturingGroup(iterator,  context, _capture_expression2, _name4, 1, 0);
} 

int main(int argc, char *argv[]) {
  const char emptystring[] = "";
  const char test1string[] = TESTSTRING1"blah";
  const char test2string[] = TESTSTRING2"blah";
  const char middle_test12string[] = OTHERSTRING TESTSTRING1 TESTSTRING2"blah";
  const char errmsg[] = "some kind of error";

  npeg_inputiterator iterator;
  npeg_astnode* p_ast;
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
  assert(_capture_expression1(&iterator, &context) == 0 && iterator.index == 0 
	 && rh_stackstack_count(context.sandbox) == 1);
  assert(_capture_expression2(&iterator, &context) == 0 && iterator.index == 0 
	 && rh_stackstack_count(context.sandbox) == 1);
  npeg_inputiterator_destructor(&iterator);
  npeg_destructor(&context);
  puts("\tVerified: no change of internal state when operating on empty string");

  npeg_constructor(&context, NULL);  
  npeg_inputiterator_constructor(&iterator, middle_test12string, strlen(middle_test12string));
  iterator.index = strlen(OTHERSTRING)/2;
  assert(_capture_expression1(&iterator, &context) == 0 && iterator.index == strlen(OTHERSTRING)/2
	 && rh_stackstack_count(context.sandbox) == 1);
  assert(_capture_expression2(&iterator, &context) == 0 && iterator.index == strlen(OTHERSTRING)/2
	 && rh_stackstack_count(context.sandbox) == 1);
  npeg_inputiterator_destructor(&iterator);
  npeg_destructor(&context);
  puts("\tVerified: no change of internal state when no occurence of expression");

  npeg_constructor(&context, NULL);  
  npeg_inputiterator_constructor(&iterator, test1string, strlen(test1string));
  assert(_capture_expression1(&iterator, &context) == 1 && iterator.index == strlen(TESTSTRING1)
	 && rh_stackstack_count(context.sandbox) == 1);
  p_ast = npeg_get_ast(&context);
  assert(0 == strcmp(p_ast->token->name, _name1));
  npeg_inputiterator_destructor(&iterator);
  npeg_destructor(&context);
  npeg_astnode_delete_tree(p_ast, npeg_astnode_tokendeletion_callback);
  puts("\tVerified: capturing of simple occurence of expression1");

  npeg_constructor(&context, NULL);  
  npeg_inputiterator_constructor(&iterator, test1string, strlen(test1string));
  error.message = (char*)errmsg, error.iteratorIndex = 0;
  rh_stack_push(context.errors, &error);
  assert(_capture_expression1(&iterator, &context) == 0 && iterator.index == 0);
  rh_stack_pop(context.errors);
  npeg_inputiterator_destructor(&iterator);
  npeg_destructor(&context);
  puts("\tVerified: Abortion on error without modification of state.");  

  npeg_constructor(&context, NULL);  
  npeg_inputiterator_constructor(&iterator, test2string, strlen(test2string));
  assert(_capture_expression2(&iterator, &context) == 1 && iterator.index == strlen(TESTSTRING2)
	 && rh_stackstack_count(context.sandbox) == 1);
  p_ast = npeg_get_ast(&context);
  assert(0 == strcmp(p_ast->token->name, _name2));
  npeg_astnode_delete_tree(p_ast, npeg_astnode_tokendeletion_callback);
  npeg_inputiterator_destructor(&iterator);
  npeg_destructor(&context);
  puts("\tVerified: capturing of simple occurence of expression2");

  npeg_constructor(&context, NULL);  
  npeg_inputiterator_constructor(&iterator, middle_test12string, strlen(middle_test12string));
  iterator.index = strlen(OTHERSTRING);
  assert(_capture_expression3(&iterator, &context) == 1 
	 && iterator.index == strlen(OTHERSTRING) + strlen(TESTSTRING1) + strlen(TESTSTRING2)
	 && rh_stackstack_count(context.sandbox) == 1);
  p_ast = npeg_get_ast(&context);
  assert(0 == strcmp(p_ast->token->name, _name3) && p_ast->nof_children == 2);
  npeg_astnode_delete_tree(p_ast, npeg_astnode_tokendeletion_callback);
  npeg_inputiterator_destructor(&iterator);
  npeg_destructor(&context);
  puts("\tVerified: capturing of 1/2 sequence");

  npeg_constructor(&context, NULL);  
  npeg_inputiterator_constructor(&iterator, test2string, strlen(test2string));
  assert(_capture_expression2_red(&iterator, &context) == 1 && iterator.index == strlen(TESTSTRING2)
	 && rh_stackstack_count(context.sandbox) == 1);
  p_ast = npeg_get_ast(&context);
  assert(0 == strcmp(p_ast->token->name, _name2) && p_ast->nof_children == 0);
  npeg_astnode_delete_tree(p_ast, npeg_astnode_tokendeletion_callback);
  npeg_inputiterator_destructor(&iterator);
  npeg_destructor(&context);
  puts("\tVerified: capturing of redundant expression with reduction to single node");

  return 0;
}
