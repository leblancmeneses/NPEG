#include <assert.h>
#include <stdio.h>
#include <string.h>

#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"
#include "robusthaven/structures/stack.h"

#include "MathematicalFormula.h"

#define BUFFER_SIZE 100

int main(int argc, char *argv[])
{
  const char text1[] = "(1*3+4)/5*93";
  const char text2[] = "9+(9-8)*10";  

  char buffer[BUFFER_SIZE];
  rh_stack_instance disableBackReferenceStack;
  rh_stackstack_instance sandbox;
  rh_hashmap_instance backreference_lookup;
  rh_list_instance warnings;
  rh_stack_instance errors;
  npeg_context context;
  npeg_inputiterator iterator;
  npeg_astnode* ast, *p_child;

  int (*parsetree)(npeg_inputiterator*, npeg_context*) = &MathematicalFormula_impl_0;
  int IsMatch = 0;

  // load npeg managed memory
  context.disableBackReferenceStack = &disableBackReferenceStack;
  context.sandbox = &sandbox;
  context.backReferenceLookup = &backreference_lookup;
  context.warnings = &warnings; 
  context.errors = &errors;

  npeg_inputiterator_constructor(&iterator, text1, strlen(text1));
  npeg_constructor(&context, NULL);

  IsMatch = parsetree(&iterator, &context);
  assert(IsMatch);
  printf("\tVerified: The expected input was matched by parser.\n");

  ast = npeg_get_ast(&context);
  npeg_printVisitor(ast, NULL);
  assert(0 == strcmp(ast->token->name, "EXPRESSION"));
  printf("\tVerified: The expected token name: '%s'.\n", ast->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, ast->token->start, ast->token->end);
  assert(0 == strcmp(buffer, text1));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  assert(ast->nof_children == 5);
  puts("\tVerified: Expected number of children.");
  p_child = ast->children[0];
  assert(0 == strcmp(p_child->token->name, "EXPRESSION"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "1*3+4"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[1];
  assert(0 == strcmp(p_child->token->name, "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "/"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
  
  p_child = ast->children[2];
  assert(0 == strcmp(p_child->token->name, "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "5"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[3];
  assert(0 == strcmp(p_child->token->name, "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "*"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[4];
  assert(0 == strcmp(p_child->token->name, "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "93"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  assert(ast->children[0]->nof_children == 5);
  puts("\tVerified: Expected number of children.");

  p_child = ast->children[0]->children[0];
  assert(0 == strcmp(p_child->token->name, "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "1"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[0]->children[1];
  assert(0 == strcmp(p_child->token->name, "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "*"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[0]->children[2];
  assert(0 == strcmp(p_child->token->name, "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "3"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[0]->children[3];
  assert(0 == strcmp(p_child->token->name, "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "+"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[0]->children[4];
  assert(0 == strcmp(p_child->token->name, "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "4"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  // unload npeg managed memory
  npeg_astnode_delete_tree(ast, npeg_astnode_tokendeletion_callback);
  npeg_destructor(&context);
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, text2, strlen(text2));
  npeg_constructor(&context, NULL);

  IsMatch = parsetree(&iterator, &context);
  assert(IsMatch);
  printf("\tVerified: The expected input was matched by parser.\n");

  ast = npeg_get_ast(&context);
  assert(0 == strcmp(ast->token->name, "EXPRESSION"));
  printf("\tVerified: The expected token name: '%s'.\n", ast->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, ast->token->start, ast->token->end);
  assert(0 == strcmp(buffer, text2));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  assert(ast->nof_children == 5);
  puts("\tVerified: Expected number of children.");

  p_child = ast->children[0];
  assert(0 == strcmp(p_child->token->name, "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "9"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
  
  p_child = ast->children[1];
  assert(0 == strcmp(p_child->token->name, "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "+"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[2];
  assert(0 == strcmp(p_child->token->name, "EXPRESSION"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "9-8"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[3];
  assert(0 == strcmp(p_child->token->name, "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "*"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[4];
  assert(0 == strcmp(p_child->token->name, "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "10"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
  
  assert(ast->children[2]->nof_children == 3);
  puts("\tVerified: Expected number of children.");

  p_child = ast->children[2]->children[0];
  assert(0 == strcmp(p_child->token->name, "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "9"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[2]->children[1];
  assert(0 == strcmp(p_child->token->name, "SYMBOL"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "-"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[2]->children[2];
  assert(0 == strcmp(p_child->token->name, "VALUE"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "8"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  // unload npeg managed memory
  npeg_astnode_delete_tree(ast, npeg_astnode_tokendeletion_callback);
  npeg_destructor(&context);
  npeg_inputiterator_destructor(&iterator);

  return 0;
}
