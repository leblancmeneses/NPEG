#include <assert.h>
#include <stdlib.h>
#include "npeg_ast.h"

void npeg_astnode_constructor(npeg_astnode *node, npeg_token *token, const uint child_capacity) {
  assert(child_capacity > 0);

  node->token = token;
  node->capacity = child_capacity;
  node->nof_children = 0;
  node->children = (npeg_astnode**)malloc(sizeof(npeg_astnode*)*node->capacity);
} 

void npeg_astnode_add_child(npeg_astnode *node, npeg_astnode *childnode) {
  const uint new_node_idx = node->nof_children;

  /*
   * - Increment child counter
   * - Check capacity
   * - If necessary increase capacity by a factor 2.
   * - Add child pointer to children array.
   * - update child's parent pointer.
   */
  node->nof_children += 1;

  if (node->nof_children >= node->capacity) {
    node->capacity = 2*node->capacity;
    node->children = (npeg_astnode**)realloc(node->children, node->capacity*sizeof(npeg_astnode*));
  }

  node->children[new_node_idx] = childnode;
  childnode->parent = node;  
} 

void npeg_astnode_trim_childrenarrays(npeg_astnode *node) {
  if (node->capacity == 1) return;
  else {
    uint i;

    node->capacity = node->nof_children;
    node->children = (npeg_astnode**)realloc(node->children, node->capacity*sizeof(npeg_astnode*));
    for (i = 0; i < node->nof_children; i++) {
      npeg_astnode_trim_childrenarrays(node->children[i]);
    }
  }
} 

void npeg_astnode_delete_tree(npeg_astnode *node, astnode_visitor ondelete_callback) {
  uint i;

  for (i = 0; i < node->nof_children; i++) {
    npeg_astnode_delete_tree(node->children[i], ondelete_callback);
  }
  if (ondelete_callback != NULL) ondelete_callback(node);
  npeg_astnode_destructor(node);
  free(node);
} 

void npeg_astnode_tokendeletion_callback(npeg_astnode *node) {
  if (node->token != NULL) {
    if (node->token->name != NULL) free(node->token->name);
    free(node->token);
  }
} 

void npeg_astnode_destructor(npeg_astnode *node) {
  free(node->children);
} 
