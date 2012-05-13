#include <stdio.h>
#include <string.h>
#include <assert.h>

#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"
#include "robusthaven/structures/stack.h"
#include "SimpleSentence.h"

#define BUFFER_SIZE 100

int main(int argc, char *argv[]) {
  const char input1[] = "I eat an apple.";
  const char input2[] = "You drive a car.";
	
  char buffer[BUFFER_SIZE];	
  rh_stack_instance disableBackReferenceStack;
  rh_stackstack_instance sandbox;
  rh_hashmap_instance backreference_lookup;
  rh_list_instance warnings;
  rh_stack_instance errors;    
  npeg_context context = {
    &disableBackReferenceStack, 
    &sandbox,
    &backreference_lookup,
    &warnings,
    &errors
  };    
  npeg_inputiterator iterator;
  npeg_astnode *ast, *subjnode, *verbnode, *objnode;
  int (*parsetree)(npeg_inputiterator*, npeg_context*) = &sentence_root;
  int IsMatch = 0;

  // load npeg managed memory
  npeg_inputiterator_constructor(&iterator, input1, strlen(input1));
  npeg_constructor(&context, NULL);	
    
  printf("\tReached: parsetree(&iterator, &context) for input1.\n");
  IsMatch = parsetree(&iterator, &context);
  assert(IsMatch);
  printf("\tVerified: The expected input was matched by parser.\n");
    
  ast = npeg_get_ast(&context);
  assert(0 == strcmp(ast->token->name, "Sentence"));
  printf("\tVerified: The expected token name: %s.\n", ast->token->name);

  assert(3 == ast->nof_children);  
  subjnode = ast->children[0];
  verbnode = ast->children[1];
  objnode = ast->children[2];
  printf("\tVerified: #children of AST root.\n", ast->token->name);

  assert(0 == strcmp(subjnode->token->name, "subject"));
  printf("\tVerified: The expected token name: %s.\n", subjnode->token->name);

  assert(0 == strcmp(verbnode->token->name, "verb"));
  printf("\tVerified: The expected token name: %s.\n", verbnode->token->name);

  assert(0 == strcmp(objnode->token->name, "object"));
  printf("\tVerified: The expected token name: %s.\n", objnode->token->name);
    
  npeg_inputiterator_get_text(buffer, &iterator, ast->token->start, ast->token->end);
  assert(0 == strcmp(buffer, input1));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
			    
  npeg_destructor(&context); 
  npeg_inputiterator_destructor(&iterator);
  npeg_astnode_delete_tree(ast, npeg_astnode_tokendeletion_callback);
  // unload npeg managed memory

  // load npeg managed memory
  npeg_inputiterator_constructor(&iterator, input2, strlen(input2));
  npeg_constructor(&context, NULL);	
    
  printf("\tReached: parsetree(&iterator, &context) for input2.\n");
  IsMatch = parsetree(&iterator, &context);
  assert(IsMatch);
  printf("\tVerified: The expected input was matched by parser.\n");
    
  ast = npeg_get_ast(&context);
  assert(0 == strcmp(ast->token->name, "Sentence"));
  printf("\tVerified: The expected token name: %s.\n", ast->token->name);
    
  npeg_inputiterator_get_text(buffer, &iterator, ast->token->start, ast->token->end);
  assert(0 == strcmp(buffer, input2));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  assert(3 == ast->nof_children);  
  subjnode = ast->children[0];
  verbnode = ast->children[1];
  objnode = ast->children[2];
  printf("\tVerified: #children of AST root.\n", ast->token->name);

  assert(0 == strcmp(subjnode->token->name, "subject"));
  printf("\tVerified: The expected token name: %s.\n", subjnode->token->name);

  assert(0 == strcmp(verbnode->token->name, "verb"));
  printf("\tVerified: The expected token name: %s.\n", verbnode->token->name);

  assert(0 == strcmp(objnode->token->name, "object"));
  printf("\tVerified: The expected token name: %s.\n", objnode->token->name);
			    
  npeg_astnode_delete_tree(ast, npeg_astnode_tokendeletion_callback);
  npeg_destructor(&context); 
  npeg_inputiterator_destructor(&iterator);
  // unload npeg managed memory

  return 0;
}
