#include <stdio.h>
#include <assert.h>

#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"
#include "robusthaven/structures/stack.h"
#include "PhoneNumber.h"

#define BUFFER_SIZE 100

int main(int argc, char *argv[])
{
  char* input1 = "123-456-7890";
	
  char buffer[BUFFER_SIZE];
	
  rh_stack_instance disableBackReferenceStack;
  rh_stackstack_instance sandbox;
  rh_hashmap_instance backreference_lookup;
  rh_list_instance warnings;
  rh_stack_instance errors;
  npeg_astnode *ast, *p_child;
  npeg_context context;
  npeg_inputiterator iterator;
  int (*parsetree)(npeg_inputiterator*, npeg_context*) = &PhoneNumber_impl_0;
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
  npeg_printVisitor(ast, NULL);
  assert(0 == strcmp(ast->token->name, "PhoneNumber"));
  printf("\tVerified: The expected token name: %s.\n", ast->token->name);
	
  npeg_inputiterator_get_text(buffer, &iterator, ast->token->start, ast->token->end);
  assert(0 == strcmp(buffer, input1));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
  
  assert(ast->nof_children == 3);
  puts("\tVerified: Expected number of children.");

  p_child = ast->children[0];
  assert(0 == strcmp(p_child->token->name, "ThreeDigitCode"));
  printf("\tVerified: The expected token name: %s.\n", ast->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "123"));
  printf("\tVerified: The expected matched string of 1st child: '%s'.\n", buffer);
  
  p_child = ast->children[1];
  assert(0 == strcmp(p_child->token->name, "ThreeDigitCode"));
  printf("\tVerified: The expected token name: %s.\n", ast->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "456"));
  printf("\tVerified: The expected matched string of 2nd child: '%s'.\n", buffer);


  p_child = ast->children[2];
  assert(0 == strcmp(p_child->token->name, "FourDigitCode"));
  printf("\tVerified: The expected token name: %s.\n", ast->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "7890"));
  printf("\tVerified: The expected matched string of 3rd child: '%s'.\n", buffer);

  npeg_astnode_delete_tree(ast, npeg_astnode_tokendeletion_callback);
  npeg_destructor(&context); 
  npeg_inputiterator_destructor(&iterator);
  // unload npeg managed memory

  return 0;
}
