#ifndef ROBUSTHAVEN_TEXT_NPEG_AST_H
#define ROBUSTHAVEN_TEXT_NPEG_AST_H

#include "../types/types.h"
#include "npeg_token.h"


typedef struct npeg_astnode {
  struct npeg_astnode **children;
  uint nof_children, capacity; 

  struct npeg_token *token;	
  struct npeg_astnode *parent;
} npeg_astnode;

typedef void (*astnode_visitor)(npeg_astnode *node);

/*
 * Creates a new npeg_astnode, child_capacity is an initial guess for the number of children
 * that will be added to a node. If nped_astnode_add_child is used for adding children the 
 * capacity of children array is automatically adapted if insufficient.
 * An assert assures that child_capacity > 0.
 */
void npeg_astnode_constructor(npeg_astnode *node, npeg_token *token, const uint child_capacity);

/*
 * Deallocates the memory directly associated with a node, i.e. no deallocation of the
 * memory of the token, nor of the children of the node.
 */
void npeg_astnode_destructor(npeg_astnode *node);

/*
 * Adds a child to a AST node.
 * This also sets the parent pointer of the childnode appropriately.
 * ATTENTION: The child is not copied into an array. Merely a pointer is added to the
 * parent node's children array, hence DO NOT FREE THE MEMORY ALLOCATED FOR CHILDNODE!
 */
void npeg_astnode_add_child(npeg_astnode *node, npeg_astnode *childnode);

/*
 * Removes superfluous elements from the children arrays, i.e. reduces the capacity to nof_children,
 * of all nodes in the tree with given root.
 */
void npeg_astnode_trim_childrenarrays(npeg_astnode *root);

/*
 * Frees up all memory except tokens that is allocated for an AST.
 * Allows to pass a callback that frees up payload data in each node 
 * (pass NULL if no such action is required).
 */
void npeg_astnode_delete_tree(npeg_astnode *root, astnode_visitor ondelete_callback);

/*
 * Standard callback that deletes a nodes token & token name.
 */
void npeg_astnode_tokendeletion_callback(npeg_astnode *node);
#endif 
