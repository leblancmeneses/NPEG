#include <stdio.h>
#include "printvisitor.h"

static void _recursive_print(npeg_astnode *root, FILE *out) {
  if (root->nof_children) {
    int i;

    fprintf(out, "Visit enter: %s\n", root->token->name);
    
    for (i = 0; i < root->nof_children; i++) {
      _recursive_print(root->children[i], out);
    }
  } else {
    fprintf(out, "Visit enter: %s: captures %d-%d\n", 
	    root->token->name, root->token->start, root->token->end);
  }
  
  fprintf(out, "Visit leave: %s\n", root->token->name);
}

void npeg_printVisitor(npeg_astnode *root, FILE *out) {
  puts("START - PrintVisitor");  
  _recursive_print(root, out == NULL? stdout : out);
}
