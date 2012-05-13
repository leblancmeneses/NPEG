#include <string.h>
#include <stdio.h>
#include <assert.h>

#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"
#include "robusthaven/structures/stack.h"
#include "NpegNode.h"

#define BUFFER_SIZE 100

int main(int argc, char *argv[])
{
  char* input1 = "NpEg";
  char* input2 = ".NET Parsing Expression Grammar";
  char* input3 = "NET Parsing Expression Grammar";
	
  char buffer[BUFFER_SIZE];
	
  rh_stack_instance disableBackReferenceStack;
  rh_stackstack_instance sandbox;
  rh_hashmap_instance backreference_lookup;
  rh_list_instance warnings;
  rh_stack_instance errors;
	
  npeg_context context;
  npeg_inputiterator iterator;
  npeg_astnode* ast;

  int (*parsetree)(npeg_inputiterator*, npeg_context*) = &NpegNode_impl_0;
  int IsMatch = 0;
	
	
  // load npeg managed memory
  context.disableBackReferenceStack = &disableBackReferenceStack;
  context.sandbox = &sandbox;
  context.backReferenceLookup = &backreference_lookup;
  context.warnings = &warnings; 
  context.errors = &errors;
  npeg_inputiterator_constructor(&iterator, input1, 31);
  npeg_constructor(&context, NULL);	
	
  printf("\tReached: parsetree(&iterator, &context) for input1.\n");
  IsMatch = parsetree(&iterator, &context);
  assert(IsMatch);
  printf("\tVerified: The expected input was matched by parser.\n");
	
  ast = npeg_get_ast(&context);
  assert(0 == strcmp(ast->token->name, "NpegNode"));
  printf("\tVerified: The expected token name: '%s'.\n", ast->token->name);
	
  npeg_inputiterator_get_text(buffer, &iterator, ast->token->start, ast->token->end);
  assert(0 == strcmp(buffer, "NpEg"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
	
  npeg_astnode_delete_tree(ast, npeg_astnode_tokendeletion_callback);
  npeg_destructor(&context); 
  npeg_inputiterator_destructor(&iterator);
  // unload npeg managed memory

  printf("\n");

  // load npeg managed memory
  npeg_inputiterator_constructor(&iterator, input2, 31);
  npeg_constructor(&context, NULL);	
	
  printf("\tReached: parsetree(&iterator, &context) for input2.\n");
  IsMatch = parsetree(&iterator, &context);
  assert(IsMatch);
  printf("\tVerified: The expected input was matched by parser.\n");
	
  ast = npeg_get_ast(&context);
  assert(0 == strcmp(ast->token->name, "NpegNode"));
  printf("\tVerified: The expected token name: '%s'.\n", ast->token->name);
	
  npeg_inputiterator_get_text(buffer, &iterator, ast->token->start, ast->token->end);
  assert(0 == strcmp(buffer, ".NET Parsing Expression Grammar"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
	
  npeg_astnode_delete_tree(ast, npeg_astnode_tokendeletion_callback);
  npeg_destructor(&context); 
  npeg_inputiterator_destructor(&iterator);
  // unload npeg managed memory




  printf("\n");



  // load npeg managed memory
  npeg_inputiterator_constructor(&iterator, input3, 31);
  npeg_constructor(&context, NULL);	
	
  printf("\tReached: parsetree(&iterator, &context) for input3.\n");
  IsMatch = parsetree(&iterator, &context);
  assert(!IsMatch);
  printf("\tVerified: The expected input would NOT be matched by parser.\n");
	
  ast = npeg_get_ast(&context);
  assert(ast == NULL);
  printf("\tVerified: Retrieving an ast when not a match; npeg_get_ast would return null.\n");
	
  npeg_destructor(&context); 
  npeg_inputiterator_destructor(&iterator);
  // unload npeg managed memory

	
  return 0;
}
