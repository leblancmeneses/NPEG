#include <limits.h>
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <string.h>
#include <ctype.h>

#include "../structures/stack.h"
#include "npeg.h"
#include "npeg_inputiterator.h"


#define TRUE 1
#define FALSE 0





static void __npeg_WarnAddToList(rh_list_instance *list, const npeg_warn* warn) 
{
	npeg_warn* warnType;
	
	warnType = (npeg_warn*)malloc(sizeof(npeg_warn));
	
	// +1 to copy null
	warnType->message = (char*)malloc(strlen(warn->message)+1);
	memcpy(warnType->message, warn->message, strlen(warn->message)+1); 
	
	rh_list_add(list, warnType);
}

static void __npeg_WarnDisposing(void *data)
{
  npeg_warn* warning = (npeg_warn*)data;

  free(warning->message);	
  free(warning);
}











static void __npeg_ErrorPushOnStack(rh_stack_instance *stack, const npeg_error* error) 
{
	npeg_error* errorType;
	
	errorType = (npeg_error*)malloc(sizeof(npeg_error));
	
	// +1 to copy null
	errorType->message = (char*)malloc(strlen(error->message)+1);
	memcpy(errorType->message, error->message, strlen(error->message)+1); 
	
	rh_stack_push(stack, (void*)errorType);
}

static void __npeg_ErrorDisposing(void *data)
{
	npeg_error* error = (npeg_error*)data;

	free(error->message);
	free(error);
}











static void __npeg_DisableBackReferencePushOnStack(rh_stack_instance *stack, const int value) 
{
	int *p_val;

	p_val = (int*)malloc(sizeof(int));
	*p_val = value;
	rh_stack_push(stack, p_val);
}

static void __npeg_DisableBackReferenceDisposing(void *data)
{
	free(data);
}












// Stack<Stack<AstNode>> datastructure  (parent, child)
static void __npeg_AstNodeDisposing(void *data)
{
  npeg_astnode* astnode = data;
  
  free(astnode->token->name);
  free(astnode->token);
  npeg_astnode_destructor(astnode);
  free(astnode);
}






// holds captured text which is useful in xml parsing
// this frees the top container holding elements of captured text.
static void __npeg_onBackReferenceLookupDisposing(rh_hashmap_element* node)
{
	rh_stack_instance* instance = (rh_stack_instance*)node->data;
	rh_stack_destructor(instance); // clear stack nodes; which calls __npeg_onBackReferenceMatchedStringFreeing as ondisposing callback
	free(instance);  // clear dynamic stack container
	free(node->key); // clear dynamic char* name
}



static void __npeg_CapturedTextDisposing(void *data)
{
	free(data);
}















void npeg_constructor(npeg_context* context, npeg_CustomAstNodeAllocatorVisitor astNodeAllocator)
{
	rh_stack_constructor(context->disableBackReferenceStack, __npeg_DisableBackReferenceDisposing);
	rh_stackstack_constructor(context->sandbox, __npeg_AstNodeDisposing);
	rh_stackstack_push_empty_stack(context->sandbox);

	rh_hashmap_constructor(context->backReferenceLookup, 100, __npeg_onBackReferenceLookupDisposing);
	rh_list_constructor(context->warnings, &__npeg_WarnDisposing);
	rh_stack_constructor(context->errors, &__npeg_ErrorDisposing);
	context->astNodeAllocator = astNodeAllocator;
}

void npeg_reset(npeg_context* context)
{	
	rh_stack_clear(context->disableBackReferenceStack);
	rh_hashmap_clear(context->backReferenceLookup);
	rh_list_clear(context->warnings);
	rh_stack_clear(context->errors);	
}

void npeg_destructor(npeg_context* context)
{
	rh_stack_destructor(context->disableBackReferenceStack);
	if (rh_stackstack_count(context->sandbox) > 0) {
	  npeg_astnode *p_node;

	  if (rh_stack_count(rh_stackstack_peek_stack(context->sandbox)) > 0) {
	    p_node = (npeg_astnode*)rh_stackstack_pop_from_top(context->sandbox);
	    npeg_astnode_delete_tree(p_node, npeg_astnode_tokendeletion_callback);
	  }
	}
	rh_stackstack_destructor(context->sandbox);
	rh_hashmap_destructor(context->backReferenceLookup);
	rh_list_destructor(context->warnings);
	rh_stack_destructor(context->errors);
}




npeg_astnode* npeg_get_ast(npeg_context* context)
{
	rh_stack_instance* sandBox;
	
	if(rh_stackstack_count(context->sandbox) > 0)
	{
		return (npeg_astnode*)rh_stackstack_pop_from_top(context->sandbox);
	}
	else
	{
		return NULL;	
	}	
}

inline int _has_error(const npeg_context* context) {
  return rh_stack_count(context->errors) > 0;
} 

#define _exit_on_error(context) if (_has_error(context)) return 0

// Non Terminals
int npeg_AndPredicate(npeg_inputiterator* iterator, npeg_context* context, npeg_IsMatchPredicate expr)
{
	int result = 1;
	int savePosition = iterator->index;

	_exit_on_error(context);

	__npeg_DisableBackReferencePushOnStack(context->disableBackReferenceStack, 1);

	if (expr(iterator, context)) {
		iterator->index = savePosition;
		result = 1;
	} else {
		iterator->index = savePosition;
		result = 0;
	}

	__npeg_DisableBackReferenceDisposing(rh_stack_pop(context->disableBackReferenceStack));

	return result;
}


int npeg_NotPredicate(npeg_inputiterator* iterator, npeg_context* context,  npeg_IsMatchPredicate expr) 
{
	int result = 1;
	int savePosition = iterator->index;

	__npeg_DisableBackReferencePushOnStack(context->disableBackReferenceStack, 1);
	if (!expr(iterator, context)) {
		iterator->index = savePosition;
		result = 1;
	} else {
		iterator->index = savePosition;
		result = 0;
	}

	__npeg_DisableBackReferenceDisposing(rh_stack_pop(context->disableBackReferenceStack));

	_exit_on_error(context);

	return result;
} 

int npeg_PrioritizedChoice(npeg_inputiterator* iterator, npeg_context* context, 
			   npeg_IsMatchPredicate left, npeg_IsMatchPredicate right) 
{
	int savePosition = iterator->index;

	if (left(iterator, context)) {
		return 1;
	}
	iterator->index = savePosition;

	if (right(iterator, context)) {
		return 1;
	}
	iterator->index = savePosition;

	return 0;
}

int npeg_Sequence(npeg_inputiterator* iterator, npeg_context* context, 
		  npeg_IsMatchPredicate left, npeg_IsMatchPredicate right) 
{
	int result = 1;
	int savePosition = iterator->index;

	if (left(iterator, context) && right(iterator, context)) 
	{
		result &= 1;
	} else {
		iterator->index = savePosition;
		result &= 0;
	}

	return result;
}

int npeg_ZeroOrMore(npeg_inputiterator* iterator, npeg_context* context, npeg_IsMatchPredicate expr) 
{
	int savePosition = iterator->index;

	while (expr(iterator, context)) {
	  if (savePosition == iterator->index) {
	    npeg_error error;
	  
	    error.message = "Infinite loop detected in ZeroOrMore";
	    __npeg_ErrorPushOnStack(context->errors, &error);

	    return 0;	    
	  }

	  savePosition = iterator->index;
	}

	iterator->index = savePosition;

	_exit_on_error(context);
	
	return 1;
}

int npeg_OneOrMore(npeg_inputiterator* iterator, npeg_context* context, npeg_IsMatchPredicate expr) 
{
	int cnt = 0;
	int savePosition = iterator->index;

	while (expr(iterator, context)) {
	  if (savePosition == iterator->index) {
	    npeg_error error;
	  
	    error.message = "Infinite loop detected in OneOrMore";
	    __npeg_ErrorPushOnStack(context->errors, &error);

	    return 0;	    
	  }
	  savePosition = iterator->index;
	  cnt++;
	}

	iterator->index = savePosition;

	return (cnt > 0);
}

int npeg_Optional(npeg_inputiterator* iterator,  npeg_context* context, npeg_IsMatchPredicate expr) {
	int savePosition = iterator->index;

	if (expr(iterator, context)) {
		savePosition = iterator->index;
	} else {
		iterator->index = savePosition;
	}

	_exit_on_error(context);

	return 1;
}

/*
* Attention: no bound is indicated by a value -1 in min or max!
*/
int npeg_LimitingRepetition(npeg_inputiterator* iterator, npeg_context* context, int min,  int max, 
			    npeg_IsMatchPredicate expr) 
{
	int cnt = 0;
	int savePosition;
	int initialPosition;
	int result = 0;

	initialPosition = (savePosition = iterator->index);

	if (min > max && max != -1) {
	  npeg_error error;
	  
	  error.message = "Repetition bounds invalid: min > max";
	  __npeg_ErrorPushOnStack(context->errors, &error);

	  return 0;
	}

	if (min != -1) {
		if (max == -1) {
			// has a minimum but no max cap
			savePosition = iterator->index;
			while (expr(iterator, context)) {
			  if (savePosition == iterator->index) {
			    npeg_error error;
	  
			    error.message = "Infinite loop detected in LimitingRepetition";
			    __npeg_ErrorPushOnStack(context->errors, &error);

			    return 0;	    
			  }

			  cnt++;
			  savePosition = iterator->index;
			}

			iterator->index = savePosition;
			result = (cnt >= min);
		} else {
			// has a minimum and a max specified
			savePosition = iterator->index;
			while (expr(iterator, context)) {
				cnt++;
				savePosition = iterator->index;

				if (cnt >= max) break;
			}

			iterator->index = savePosition;
			result = (cnt <= max && cnt >= min);
		}
	} else {
		// zero or up to a max matches of e.
		savePosition = iterator->index;
		while (expr(iterator, context)) {
			cnt++;
			savePosition = iterator->index;

			if (cnt >= max) break;
		}

		iterator->index = savePosition;
		result = (cnt <= max);
	}

	if (result == 0) iterator->index = initialPosition;

	_exit_on_error(context);

	return result;
}


int npeg_CapturingGroup(npeg_inputiterator* iterator,  npeg_context* context, 
			npeg_IsMatchPredicate expr, const char* name, int doReplaceBySingleChildNode,
			int allocateCustomAstNode) 
{
  int result = TRUE;
  int savePosition = iterator->index;
  rh_stack_instance *sandBox;
  npeg_astnode *astNode;
  char *dynamicName_4_token;

  // load current sandbox into view.
  sandBox = rh_stackstack_peek_stack(context->sandbox);
	
  // add an empty ast node into the sandbox
  astNode = (npeg_astnode*)malloc(sizeof(npeg_astnode));
  npeg_astnode_constructor(astNode, NULL, 2);
  rh_stackstack_push_on_top(context->sandbox, astNode);
		
  if (expr(iterator, context))
    {
      npeg_astnode* astnode;
      char* capturedText;

      capturedText = (char*)malloc(iterator->index - savePosition + 1);
      npeg_inputiterator_get_text(capturedText, iterator, savePosition, iterator->index);
		
      // This conditional block is to save capturedText.
      // The only practical example of this is in xml parsing where backreferencing is needed.
      // an option should exist in context->IsBackReferencingEnabled
      // so in small devices we don't waste memory that isn't needed for a parser implementation by user.
      if (rh_hashmap_contains_key(context->backReferenceLookup, (char*)name)) {
	rh_stack_instance* stack;
	  
	stack = (rh_stack_instance*)rh_hashmap_lookup(context->backReferenceLookup, name);
	rh_stack_push(stack, capturedText);
      } else {
	// create a stack incase more nested matches are found with the same name... as user pops them out it will match in order.
	rh_hashmap_element hashElement;
	rh_stack_instance* stack;
	char *dynamicName_4_backReference;
	
	stack = (rh_stack_instance*)malloc(sizeof(rh_stack_instance));
	rh_stack_constructor(stack, __npeg_CapturedTextDisposing);
	rh_stack_push(stack, capturedText);

	/*  __npeg_onBackReferenceLookupDisposing disposing is responsible for clean-up */
	dynamicName_4_backReference = (char*)malloc(strlen(name)+1); 
	strcpy(dynamicName_4_backReference, name); 

	hashElement.data = (void*)stack;
	rh_hashmap_insert(context->backReferenceLookup, &hashElement, dynamicName_4_backReference);
      }

      /*
       * The AST node destruction callback will free up the memory allocated for tokens.
       */
      astnode = (npeg_astnode*)rh_stackstack_pop_from_top(context->sandbox);
      dynamicName_4_token = (char*)malloc(strlen(name)+1);
      strcpy(dynamicName_4_token, name);
      astnode->token = (npeg_token*)malloc(sizeof(npeg_token));
      astnode->token->name = dynamicName_4_token;
      astnode->token->start = savePosition;
      astnode->token->end = iterator->index;
		
#warning please give example of a custom struct able to be cast to npeg_astnode; custom struct has dynamic memory; the user needs to have capabilities to dispose.
      if (allocateCustomAstNode) {
	// create a custom astnode
	npeg_astnode *custom;
	int i;
			
	if (context->astNodeAllocator == NULL) {
	  npeg_error error;	

	  error.message = "Configuration error: This parser context has no AST node allocator callback.";
	  __npeg_ErrorPushOnStack(context->errors, &error);

	  return 0;
	}

	custom = context->astNodeAllocator(astnode);
	custom->token = astnode->token;
	custom->parent = astnode->parent;
	custom->children = astnode->children;
	for (i = 0; i < custom->nof_children; i++) 
	  {
	    custom->children[i]->parent = custom;
	  }

	npeg_astnode_destructor(astnode);
	free(astnode);
	astnode = custom;
      }
	
	
      if (doReplaceBySingleChildNode) 
	{
	  if (astnode->nof_children == 1) 
	    {
	      npeg_astnode* remove = astnode;

	      // removes ast node but not it's 1 child
	      astnode = astnode->children[0];
	      remove->nof_children -= 1;
	      assert(remove->nof_children == 0);
	      npeg_astnode_destructor(remove);
	      free(remove->token->name);
	      free(remove->token);
	      free(remove);
	    }
	}
		
		

		
      // section decides if astnode is pushed in sandbox or to start building the tree
      if (rh_stack_count(sandBox) > 0) 
	{	  
	  astnode->parent = (npeg_astnode*)rh_stack_peek(sandBox);
	  npeg_astnode_add_child(astnode->parent, astnode);
	} 
      else 
	{
	  rh_stack_push(sandBox, astnode);
	}

      savePosition = iterator->index;
      result &= TRUE;
    }
  else
    {
      npeg_astnode *p_old_node;

      p_old_node = (npeg_astnode*)rh_stack_pop(sandBox);

      npeg_astnode_destructor(p_old_node);
      free(p_old_node); 
	    
      iterator->index = savePosition;
      result &= FALSE;
    }
	
  return result;
}




// Terminals
int npeg_Literal(npeg_inputiterator* iterator,  npeg_context* context, char* matchText, int matchTextLength, int isCaseSensitive) 
{
	int i;
	char current;

	_exit_on_error(context);

	for(i=0; i<matchTextLength; i++)
	{
		current = npeg_inputiterator_get_current(iterator);
		if ( current == -1)
		{
			// input not long enough to match the matchText value.
			return 0;
		}

		if (isCaseSensitive)
		{
			if (current != matchText[i])
			{
				npeg_inputiterator_get_next(iterator);
				return 0;
			}
		}
		else
		{
			if (toupper(current) != toupper(matchText[i]))
			{
				npeg_inputiterator_get_next(iterator);
				return 0;
			}
		}

		npeg_inputiterator_get_next(iterator);
	}

	return 1;
}



int npeg_AnyCharacter(npeg_inputiterator* iterator,  npeg_context* context)
{
  int result;

  _exit_on_error(context);

  if (iterator->index < iterator->length) {
    /*
     * Consume one character since we're neither at the end of a real string, 
     * nor operating on the empty string.
     */
    iterator->index += 1;
    result = 1;
  } else result = 0;
  
  return result;
}


/*
 * Converts the ANSI string to a numerical value corresponding to the expected bit pattern.
 * Wildcards are incorporated in the mask output parameter.
 */
static int _parse_hexblock(uchar *p_mask, const char code[]) {
  int value;
  int i;

  value = 0, *p_mask = 0xff;  
  for (i = 0; i < 2; i++) {
    value = value << 4;
    if (code[i] >= '0' && code[i] <= '9') value = value + (code[i] - '0');
    else {
      char lcode;
      
      lcode = tolower(code[i]);
      if (lcode >= 'a' && lcode <= 'f') value = value + (lcode - 'a' + 10);
      else if (lcode == 'x') {
	if (i == 0) *p_mask = *p_mask & 0x0f;
	else *p_mask = *p_mask & 0xf0;
      } else return -1;
    }
  }

  assert(value < 1 << 8);

  return value;
} 

/*
 * Converts the ANSI string to a numerical value corresponding to the expected bit pattern.
 * Wildcards are incorporated in the mask output parameter.
 */
static int _parse_binblock(uchar *p_mask, const char code[]) {
  int value, i;

  value = 0, *p_mask = 0x0;
  for (i = 0; i < 8; i++) {
    value = value << 1;
    *p_mask = *p_mask << 1;
    if (code[i] == '0' || code[i] == '1') {
      value = value + (uint)(code[i] - '0');      
      *p_mask = *p_mask | 0x01;
    } else if (code[i] == 'x' || code[i] == 'X') {
      /* do nothing */
    } else {
      return -1;
    }
  }

  assert(value < 1 << 8);

  return value;
}

inline unsigned char _get_current_byte(npeg_inputiterator *iterator) {
  return (uint)npeg_inputiterator_get_current(iterator) & 0xff;
}

int npeg_CodePoint(npeg_inputiterator *iterator,  npeg_context *context, 
		   const char *matchcodes, const int matchlen) {
  npeg_error error;	

  if (*matchcodes != '#') {
    error.message = "Input string not in format \"#[x,b]<CODE><CODE>...\"";
    __npeg_ErrorPushOnStack(context->errors, &error);

    return 0;
  } else {
    const char *p_code, *p_end;
    int codelen, tmpcode, saveidx;
    uchar bitmask;

    saveidx = iterator->index;
    p_end = matchcodes + matchlen;
    if (matchcodes[1] == 'x') {      
      /*
       * Hex
       */
      p_code = matchcodes + 2;
      codelen = matchlen - 2;

      if (codelen == 0) {
	error.message = "Hex value specified is empty.";
	__npeg_ErrorPushOnStack(context->errors, &error);

	return 0;
      } else if (codelen%2 == 1) {
	char tmpblock[2];

	/* have to pad with one leading 0 */
	tmpblock[0] = '0';
	tmpblock[1] = *p_code;
	tmpcode = _parse_hexblock(&bitmask, tmpblock);

	if (tmpcode == -1) {
	  error.message = "Hex value specified contains invalid characters.";
	  __npeg_ErrorPushOnStack(context->errors, &error);
	
	  return 0;
	} else if (iterator->index >= iterator->length 
		   || (bitmask & _get_current_byte(iterator)) != tmpcode) {
	  iterator->index = saveidx;
	  
	  return 0;
	}

	p_code++;	
	npeg_inputiterator_get_next(iterator);
      } 

      for (; p_code < p_end; p_code += 2) {
	tmpcode = _parse_hexblock(&bitmask, p_code);

	if (tmpcode == -1) {
	  error.message = "Hex value specified contains invalid characters.";
	  __npeg_ErrorPushOnStack(context->errors, &error);
	
	  return 0;
	} else if (iterator->index >= iterator->length 
		   || (bitmask & _get_current_byte(iterator)) != tmpcode) {
	  iterator->index = saveidx;

	  return 0;
	}

	npeg_inputiterator_get_next(iterator);
      }

      return 1;
    } else if (matchcodes[1] == 'b') {
      /*
       * Binary
       */
      p_code = matchcodes + 2;
      codelen = matchlen - 2;

      if (codelen == 0) {
	error.message = "Binary value specified is empty.";
	__npeg_ErrorPushOnStack(context->errors, &error);

	return 0;	
      } else if (codelen%8 != 0) {
	int first_len, padlen, i;
	char tmpblock[8];

	first_len = codelen%8;
	padlen = 8 - first_len;
	for (i = 0; i < padlen; i++) tmpblock[i] = '0';
	for (i = 0; i < first_len; i++) tmpblock[i+padlen] = *(p_code + i);
	tmpcode = _parse_binblock(&bitmask, tmpblock);
	
	if (tmpcode == -1) {
	  error.message = "Binary value specified contains invalid characters.";
	  __npeg_ErrorPushOnStack(context->errors, &error);
	
	  return 0;
	} else if (iterator->index >= iterator->length 
		   || (bitmask & _get_current_byte(iterator)) != tmpcode) {
	  iterator->index = saveidx;

	  return 0;
	}

	p_code += first_len;		
	npeg_inputiterator_get_next(iterator);
      }

      for (; p_code < p_end; p_code += 8) {
	tmpcode = _parse_binblock(&bitmask, p_code);

	if (tmpcode == -1) {
	  error.message = "Binary value specified contains invalid characters.";
	  __npeg_ErrorPushOnStack(context->errors, &error);
	
	  return 0;
	} else if (iterator->index >= iterator->length 
		   || (bitmask & _get_current_byte(iterator)) != tmpcode) {
	  iterator->index = saveidx;

	  return 0;
	}

	npeg_inputiterator_get_next(iterator);
      }

      return 1;
    } else {
      double dcode;
      uint mcode, nof_inbytes, i, incode;

      /*
       * Decimal:
       * - Convert match string manually while checking for invalid input and overflow.
       * -- decide how many bytes of the input that are to be matched.
       * - Convert input string manually interpreting all consumed bytes as numerical values.
       */
      codelen = matchlen - 1;

      dcode = 0;
      for (p_code = matchcodes + 1; p_code < p_end; p_code++) {
	if (*p_code >= '0' && *p_code <= '9') {
	  dcode = dcode*10;
	  dcode = dcode + (*p_code - '0');
	}
	else {
	  error.message = "Decimal value specified contains invalid characters.";
	  __npeg_ErrorPushOnStack(context->errors, &error);
	
	  return 0;
	}
      }

      if (dcode > UINT_MAX) {
	error.message = "Decimal codepoint value exceeds 4 byte maximum length. "
	  "Largest decimal value possible 2^32 - 1";
	__npeg_ErrorPushOnStack(context->errors, &error);
	
	return 0;
      }
      mcode = (uint)dcode;
      
      if (mcode >> 24 != 0) nof_inbytes = 4;
      else if (mcode >> 16 != 0) nof_inbytes = 3;
      else if (mcode >> 8 != 0) nof_inbytes = 2;
      else nof_inbytes = 1;

      if (nof_inbytes > iterator->length - iterator->index) {
	return 0;
      }

      incode = 0;
      for (i = 0; i < nof_inbytes; i++) {
	incode = (incode << 8) + _get_current_byte(iterator);
	npeg_inputiterator_get_next(iterator);
      }

      if (incode != mcode) {
	iterator->index = saveidx;
	
	return 0;
      } else return 1;
    }
  }
}


int npeg_CharacterClass(npeg_inputiterator* iterator,  npeg_context* context, char *characterClass, int characterClassDefinitionLength)
{
	int result = FALSE;
	
	char* errorMessage0001 = "CharacterClass definition must be a minimum of 3 characters [expression]";
	char* errorMessage0002 = "CharacterClass definition must start with [";
	char* errorMessage0003 = "CharacterClass definition must end with ]";
	char* errorMessage0004 = "CharacterClass definition requires user to escape '\\' given location in expression. User must escape by specifying '\\\\'";
	char* errorMessage0005 = "CharacterClass definition requires user to escape '-' given location in expression. User must escape by specifying '\\-'";
	char* errorMessage0006 = "CharacterClass definition contains an invalid escape sequence. Accepted sequences: \\\\, \\s, \\S, \\d, \\D, \\w, \\W";

	int ptrIndex = 0;

	int toBeEscaped = FALSE;
	int isPostiveCharacterGroup = TRUE;
	
	npeg_error error;
		
	char current = npeg_inputiterator_get_current(iterator);

	_exit_on_error(context);

	if(current == -1)
		return FALSE;

	if(characterClassDefinitionLength < 3)
	{
		error.message = errorMessage0001;
		__npeg_ErrorPushOnStack(context->errors, &error);
		return FALSE;
	}
	if(characterClass[ptrIndex] != '[')
	{
		error.message = errorMessage0002;
		__npeg_ErrorPushOnStack(context->errors, &error);
		return FALSE;
	}
	if(characterClass[characterClassDefinitionLength - 1] != ']')
	{
		error.message = errorMessage0003;
		__npeg_ErrorPushOnStack(context->errors, &error);
		return FALSE;
	}

	ptrIndex++;
	if(characterClass[ptrIndex] == '^')
	{
		isPostiveCharacterGroup = FALSE;
		ptrIndex++; 
			// is found to be negative character group.
			// will continue with positive character group logic however before returning true or false from this method will ! the result.
	}
	ptrIndex--;
	
	// Positive character group logic.
	while( ++ptrIndex < characterClassDefinitionLength )
	{
		if(characterClass[ptrIndex] == '\\')
		{
			if(toBeEscaped==TRUE)
			{
				toBeEscaped=FALSE;
				if( current == '\\')
				{
					result = TRUE;
				}
			}
			else
			{
				toBeEscaped = TRUE;
				if( (ptrIndex + 1) >= (characterClassDefinitionLength - 1) )
				{
					error.message = errorMessage0004;
					__npeg_ErrorPushOnStack(context->errors, &error);
					return FALSE; // should not continue to match
				}
			}
		}
		else if(characterClass[ptrIndex] == '-')
		{
			if(toBeEscaped == TRUE)
			{
				toBeEscaped = FALSE;
				if( current == characterClass[ptrIndex] )
				{
					result = TRUE;
				}
			}
			else
			{
				if( (ptrIndex - 1) == 0 )
				{
					error.message = errorMessage0005;
					__npeg_ErrorPushOnStack(context->errors, &error);
					return FALSE; // should not continue to match
				}
				else if( (ptrIndex + 1) >= (characterClassDefinitionLength - 1) )
				{
					error.message = errorMessage0005;
					__npeg_ErrorPushOnStack(context->errors, &error);
					return FALSE; // should not continue to match	
				}
				else
				{
					if( current >= characterClass[ptrIndex-1] && current <= characterClass[ptrIndex+1])
					{
						result = TRUE;
					}
				}
			}
		}
		else 
		{
			if(toBeEscaped == TRUE)
			{
				toBeEscaped = FALSE;
				// is it in a valid \d \D \s \S \w \W
				// d D s S w W
				switch(characterClass[ptrIndex])
				{
					case 'd': 
						if( current >= '0' && current <= '9' )
						{
							result = TRUE;
						}
						break;
					case 'D':
						if( !(current >= '0' && current <= '9') )
						{
							result = TRUE;
						}
						break;
					case 's':
						if( current == ' ' || current == '\f' || current == '\n' || current == '\r' || current == '\t' || current == '\v' )
						{
							result = TRUE;
						}
						break;
					case 'S':
						if( !( current == ' ' || current == '\f' || current == '\n' || current == '\r' || current == '\t' || current == '\v') )
						{
							result = TRUE;
						}
						break;
					case 'w':
						if( 
							( current >= 'a' && current <= 'z' )
							|| ( current >= 'A' && current <= 'Z' )
							|| ( current >= '0' && current <= '9' ) 
							|| current == '_'  
							)
						{
							result = TRUE;
						}
						break;
					case 'W':
						if( 
							!( 
								   ( current >= 'a' && current <= 'z' )
								|| ( current >= 'A' && current <= 'Z' )
								|| ( current >= '0' && current <= '9' ) 
								|| current == '_'  
							)
						)
						{
							result = TRUE;
						}
						break;
					default:
						error.message = errorMessage0006;
						__npeg_ErrorPushOnStack(context->errors, &error);
						return FALSE; // should not continue to match
						break;
				}
			}
			else
			{
				if(current == characterClass[ptrIndex])
				{
					result = TRUE;
				}
			}		
		}	
	}




	if(!isPostiveCharacterGroup)
	{
		//Negative character group.
		result = (TRUE == result)? FALSE : TRUE;
	}

	if(result)
	{
		npeg_inputiterator_get_next(iterator);
		// consume this character and move iterator 1 position
	}
	
	return result;
}





int npeg_RecursionCall(npeg_inputiterator* iterator,  npeg_context* context, npeg_IsMatchPredicate expr) 
{
  _exit_on_error(context);

  return expr(iterator, context);
}

int npeg_DynamicBackReference(npeg_inputiterator* iterator,  npeg_context* context, 
			      char* backreferencename, int is_casesensitive) {
  char *p_c, *matchText;
  rh_stack_instance *backref_stack;
  int delete_text;

  _exit_on_error(context);

  backref_stack = (rh_stack_instance*)rh_hashmap_lookup(context->backReferenceLookup, backreferencename);
  if (rh_stack_count(context->disableBackReferenceStack) <= 0) {
    matchText = (char*)rh_stack_pop(backref_stack);
    delete_text = 1;
  } else {
    matchText = (char*)rh_stack_peek(backref_stack);
    delete_text = 0;
  }
  
  for (p_c = matchText; *p_c != 0; p_c++) {
    if (is_casesensitive) {
      if (npeg_inputiterator_get_current(iterator) != *p_c) {
	npeg_error error;
	char buffer[256];

	snprintf(buffer, 256, "DynamicBackReference: %s", matchText);
	error.message = buffer;
	error.iteratorIndex = iterator->index;
	__npeg_ErrorPushOnStack(context->errors, &error);				
	if (delete_text) free(matchText);
	
	return 0;
      }
    } else {
      if (toupper(npeg_inputiterator_get_current(iterator)) != toupper(*p_c)) {
	npeg_error error;
	char buffer[256];


	snprintf(buffer, 256, "DynamicBackReference: %s", matchText);
	error.message = buffer;
	error.iteratorIndex = iterator->index;
	__npeg_ErrorPushOnStack(context->errors, &error);				
	if (delete_text) free(matchText);

	return 0;
      }
    }
    
    npeg_inputiterator_get_next(iterator);
  } /* for character to match */

  if (delete_text) free(matchText);

  return 1;
}

int npeg_Fatal(npeg_inputiterator* iterator, npeg_context* context, char* message) 
{
	npeg_error error;
	error.message = message;
	__npeg_ErrorPushOnStack(context->errors, &error);
	
	return 0;
}

int npeg_Warn(npeg_inputiterator* iterator, npeg_context* context, char* message) 
{
	npeg_warn warn;

	warn.message = message;	
	__npeg_WarnAddToList(context->warnings, &warn);

	return 1;
}

