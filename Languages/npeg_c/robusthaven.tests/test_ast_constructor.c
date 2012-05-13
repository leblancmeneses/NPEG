#include <assert.h>
#include <stdio.h>
#include "robusthaven/text/npeg_ast.h"

int main(int argc, char *argv[]) {
  npeg_token* const token_test_address = (npeg_token*)0x00001234;
  const uint test_capacity = 152;

  npeg_astnode root;

  npeg_astnode_constructor(&root, token_test_address, test_capacity);
  assert(root.nof_children == 0);
  assert(root.capacity == test_capacity);
  assert(root.token == token_test_address);
  npeg_astnode_destructor(&root);
  printf("\tVerified: Correct initialization.\n");
  
  return 0;
} /* main */
