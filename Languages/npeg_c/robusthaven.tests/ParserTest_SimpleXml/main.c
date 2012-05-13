#include <stdio.h>
#include <string.h>
#include <assert.h>

#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"
#include "robusthaven/text/printvisitor.h"
#include "robusthaven/structures/stack.h"

#include "SimpleXml.h"

int main(int argc, char *argv[])
{
#define BUFFER_SIZE 100

  const char* text1 = "<h1>hello</h1><h2>hello</h2>";

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
  npeg_astnode* ast, *p_child;
  int (*parsetree)(npeg_inputiterator*, npeg_context*) = &SimpleXml_impl_0;
  int IsMatch = 0;

  // load npeg managed memory
  npeg_inputiterator_constructor(&iterator, text1, strlen(text1));
  npeg_constructor(&context, NULL);

  IsMatch = parsetree(&iterator, &context);

  assert(IsMatch);
  printf("\tVerified: The expected input was matched by parser.\n");

  ast = npeg_get_ast(&context);
  npeg_printVisitor(ast, NULL);
  assert(0 == strcmp(ast->token->name, "Expression"));
  printf("\tVerified: The expected token name: %s.\n", ast->token->name);

  npeg_inputiterator_get_text(buffer, &iterator, ast->token->start, ast->token->end);
  assert(0 == strcmp(buffer, text1));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  assert(ast->nof_children == 6);
  puts("\tVerified: Expected number of children.");

  p_child = ast->children[0];
  assert(0 == strcmp(p_child->token->name, "START_TAG"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "<h1>"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[1];
  assert(0 == strcmp(p_child->token->name, "Body"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "hello"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[2];
  assert(0 == strcmp(p_child->token->name, "END_TAG"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "</h1>"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[3];
  assert(0 == strcmp(p_child->token->name, "START_TAG"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "<h2>"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[4];
  assert(0 == strcmp(p_child->token->name, "Body"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "hello"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  p_child = ast->children[5];
  assert(0 == strcmp(p_child->token->name, "END_TAG"));
  printf("\tVerified: The expected token name: '%s'.\n", p_child->token->name);
  npeg_inputiterator_get_text(buffer, &iterator, p_child->token->start, p_child->token->end);
  assert(0 == strcmp(buffer, "</h2>"));
  printf("\tVerified: The expected matched string: '%s'.\n", buffer);

  npeg_astnode_delete_tree(ast, npeg_astnode_tokendeletion_callback);
  npeg_destructor(&context);
  npeg_inputiterator_destructor(&iterator);
  // unload npeg managed memory

  return 0;
}
