#ifndef ROBUSTHAVEN_TEXT_NPEG_H
#define ROBUSTHAVEN_TEXT_NPEG_H

#include "../structures/stack.h"
#include "../structures/stackstack.h"
#include "../structures/hashmap.h"
#include "../structures/list.h"
#include "npeg_token.h"
#include "npeg_ast.h"
#include "npeg_inputiterator.h"

/*
 * Custom AST node allcator callback:
 * Allocates a customized representation of an AST node.
 * Updating of the parent relation is carried out after calling the call back.
 * Other operations that need to be applied to the replaced node and its children have to be done
 * in the visitor routine.
 */
typedef npeg_astnode* (*npeg_CustomAstNodeAllocatorVisitor)(npeg_astnode*);

typedef struct npeg_context {
  rh_stack_instance* disableBackReferenceStack;
  rh_stackstack_instance* sandbox;
  rh_hashmap_instance *backReferenceLookup;
  rh_list_instance* warnings; /* allows parser to continue and provides information to user */
  rh_stack_instance* errors;   /* causes parser to stop parsing and return to user. */
  npeg_CustomAstNodeAllocatorVisitor astNodeAllocator;
} npeg_context;

typedef struct npeg_warn {
  char* message;
  int iteratorIndex;
} npeg_warn;

typedef struct npeg_error {
  char* message;
  int iteratorIndex;
} npeg_error;

typedef int (*npeg_IsMatchPredicate)(npeg_inputiterator*,  npeg_context*);

void npeg_constructor(npeg_context*, npeg_CustomAstNodeAllocatorVisitor astNodeAllocator);
void npeg_reset(npeg_context*);
void npeg_destructor(npeg_context*);

npeg_astnode* npeg_get_ast(npeg_context*);

// Non Terminals
int npeg_AndPredicate(npeg_inputiterator*,  npeg_context*, 
	npeg_IsMatchPredicate expr );
int npeg_NotPredicate(npeg_inputiterator*,  npeg_context*, 
	npeg_IsMatchPredicate expr );

int npeg_PrioritizedChoice(npeg_inputiterator*,  npeg_context*, 
	npeg_IsMatchPredicate left,
	npeg_IsMatchPredicate right );

int npeg_Sequence(npeg_inputiterator*,  npeg_context*, 
	npeg_IsMatchPredicate left,
	npeg_IsMatchPredicate right );

int npeg_ZeroOrMore(npeg_inputiterator*,  npeg_context*, 
	npeg_IsMatchPredicate expr );
int npeg_OneOrMore(npeg_inputiterator*,  npeg_context*, 
	npeg_IsMatchPredicate expr );
int npeg_Optional(npeg_inputiterator*,  npeg_context*, 
	npeg_IsMatchPredicate expr );
int npeg_LimitingRepetition(npeg_inputiterator*, npeg_context*, 
	int, int, npeg_IsMatchPredicate expr );

/*
 * The npeg_CustomNodeAllocator parameter is optional, pass NULL if no custom node type is required.
 */
int npeg_CapturingGroup(npeg_inputiterator*,  npeg_context*,
			npeg_IsMatchPredicate expr,
			const char *, int, int);

// Terminals
/*
 * Hex (#x), binary (#b), decimal (#) codes for matching are passed in matchcodes.
 * matchlen holds the total length of the code string including the leading type specifier.
 */
int npeg_CodePoint(npeg_inputiterator*,  npeg_context*, const char *matchcodes, const int matchlen);

int npeg_AnyCharacter(npeg_inputiterator*,  npeg_context*);
int npeg_CharacterClass(npeg_inputiterator*,  npeg_context*, char*, int);
int npeg_Literal(npeg_inputiterator*,  npeg_context*, char*, int, int);
int npeg_RecursionCall(npeg_inputiterator*, npeg_context*, npeg_IsMatchPredicate expr);
int npeg_DynamicBackReference(npeg_inputiterator*,  npeg_context*, char*, int);
int npeg_Fatal(npeg_inputiterator*,  npeg_context*, char*);
int npeg_Warn(npeg_inputiterator*,  npeg_context*, char*);

#endif
