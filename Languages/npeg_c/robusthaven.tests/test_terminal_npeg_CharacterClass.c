#include <assert.h>
#include <stdio.h>
#include <string.h>
#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"


int main(int argc, char *argv[]) 
{
	char input[2] = {'\0','\0'};
	int i = 0;
	
		
	rh_stack_instance disableBackReferenceStack;
	rh_stackstack_instance astNodeStack;
	rh_hashmap_instance backreference_lookup;
	rh_list_instance warnings;
	rh_stack_instance errors;
	npeg_context context;
	npeg_inputiterator iterator;
	void* node;
			
	// load npeg managed memory
	context.disableBackReferenceStack = &disableBackReferenceStack;
	context.sandbox = &astNodeStack;
	context.backReferenceLookup = &backreference_lookup;
	context.warnings = &warnings; 
	context.errors = &errors;

	npeg_constructor(&context, NULL);
	
	printf("\tReached: ensure arguments are validated\n");
	
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(0 == npeg_CharacterClass(&iterator, &context, "", 0));
	printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
	assert(1 == rh_stack_count(context.errors));
	printf("\tVerified: given InvalidExpressionException context->errors collection will have 1 error item.\n");
	node = rh_stack_peek(context.errors);
	assert(0 == strcmp(((npeg_error*)node)->message, "CharacterClass definition must be a minimum of 3 characters [expression]"));	
	printf("\tVerified: expected exception message.  '%s'.\n", ((npeg_error*)node)->message);
	assert(0 == rh_list_count(context.warnings));	
	printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
	assert(0 == iterator.index);
	printf("\tVerified: On no match iterator should not consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);



	printf("\n");		
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(0 == npeg_CharacterClass(&iterator, &context, "aaa", 3));
	printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
	assert(1 == rh_stack_count(context.errors));
	printf("\tVerified: given InvalidExpressionException context->errors collection will have 1 error item.\n");
	node = rh_stack_peek(context.errors);
	assert(0 == strcmp(((npeg_error*)node)->message, "CharacterClass definition must start with ["));	
	printf("\tVerified: expected exception message.  '%s'.\n",((npeg_error*)node)->message);
	assert(0 == rh_list_count(context.warnings));	
	printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
	assert(0 == iterator.index);
	printf("\tVerified: On no match iterator should not consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);




	printf("\n");
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(0 == npeg_CharacterClass(&iterator, &context, "[aa", 3));
	printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
	assert(1 == rh_stack_count(context.errors));
	printf("\tVerified: given InvalidExpressionException context->errors collection will have 1 error item.\n");
	node = rh_stack_peek(context.errors);
	assert(0 == strcmp(((npeg_error*)node)->message, "CharacterClass definition must end with ]"));	
	printf("\tVerified: expected exception message.  '%s'.\n", ((npeg_error*)node)->message);
	assert(0 == rh_list_count(context.warnings));	
	printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
	assert(0 == iterator.index);
	printf("\tVerified: On no match iterator should not consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);

	
	printf("\n");
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(0 == npeg_CharacterClass(&iterator, &context, "[\\]", 3)); // compiler converts \\ to single backslash
	printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
	assert(1 == rh_stack_count(context.errors));
	printf("\tVerified: given InvalidExpressionException context->errors collection will have 1 error item.\n");
	node = rh_stack_peek(context.errors);
	assert(0 == strcmp(((npeg_error*)node)->message, "CharacterClass definition requires user to escape '\\' given location in expression. User must escape by specifying '\\\\'"));	
	printf("\tVerified: expected exception message.  '%s'.\n", ((npeg_error*)node)->message);
	assert(0 == rh_list_count(context.warnings));	
	printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
	assert(0 == iterator.index);
	printf("\tVerified: On no match iterator should not consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);

	
	printf("\n");
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(0 == npeg_CharacterClass(&iterator, &context, "[-]", 3));
	printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
	assert(1 == rh_stack_count(context.errors));
	printf("\tVerified: given InvalidExpressionException context->errors collection will have 1 error item.\n");
	node = rh_stack_peek(context.errors);
	assert(0 == strcmp(((npeg_error*)node)->message, "CharacterClass definition requires user to escape '-' given location in expression. User must escape by specifying '\\-'"));	
	printf("\tVerified: expected exception message.  '%s'.\n", ((npeg_error*)node)->message);
	assert(0 == rh_list_count(context.warnings));	
	printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
	assert(0 == iterator.index);
	printf("\tVerified: On no match iterator should not consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);

	
	printf("\n");
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(0 == npeg_CharacterClass(&iterator, &context, "[a-]", 4));
	printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
	assert(1 == rh_stack_count(context.errors));
	printf("\tVerified: given InvalidExpressionException context->errors collection will have 1 error item.\n");
	node = rh_stack_peek(context.errors);
	assert(0 == strcmp(((npeg_error*)node)->message, "CharacterClass definition requires user to escape '-' given location in expression. User must escape by specifying '\\-'"));	
	printf("\tVerified: expected exception message.  '%s'.\n", ((npeg_error*)node)->message);
	assert(0 == rh_list_count(context.warnings));	
	printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
	assert(0 == iterator.index);
	printf("\tVerified: On no match iterator should not consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);
	
	
	printf("\n");
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(0 == npeg_CharacterClass(&iterator, &context, "[\\L]", 4)); // \L is unknown escape sequence by C compiler
	printf("\tVerified: given InvalidExpressionException npeg_CharacterClass should return false.\n");
	assert(1 == rh_stack_count(context.errors));
	printf("\tVerified: given InvalidExpressionException context->errors collection will have 1 error item.\n");
	node = rh_stack_peek(context.errors);
	assert(0 == strcmp(((npeg_error*)node)->message, "CharacterClass definition contains an invalid escape sequence. Accepted sequences: \\\\, \\s, \\S, \\d, \\D, \\w, \\W"));	
	printf("\tVerified: expected exception message.  '%s'.\n", ((npeg_error*)node)->message);
	assert(0 == rh_list_count(context.warnings));	
	printf("\tVerified: given InvalidExpressionException context->warnings collection will not be affected and have zero items.\n");
	assert(0 == iterator.index);
	printf("\tVerified: On no match iterator should not consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);
	
	
	
	printf("\n");
	printf("\tReached: confirming postive character group.\n");
	printf("\n");
	


	printf("\tReached: validate simple character ranges.\n");
	for(i = 0; i <= 9; i++)
	{
		*input = '0' + i;
		npeg_inputiterator_constructor(&iterator, input, 2);
		assert(1 == npeg_CharacterClass(&iterator, &context, "[0-9]", 5));
		printf("\tVerified: [0-9] match with %c as input.\n", *input);
		assert(1 == iterator.index);
		printf("\tVerified: On match iterator should consume character.\n");
		npeg_inputiterator_destructor(&iterator);
		npeg_reset(&context);
	}
	
	printf("\n");
	printf("\tReached: validate grouped character ranges.\n");
	for(i = 0; i < 26; i++)
	{
		*input = 'a' + i;
		npeg_inputiterator_constructor(&iterator, input, 2);
		assert(1 == npeg_CharacterClass(&iterator, &context, "[A-Z0-9a-z]", 11));
		printf("\tVerified: [A-Z0-9a-z] match with %c as input.\n", *input);
		assert(1 == iterator.index);
		printf("\tVerified: On match iterator should consume character.\n");
		npeg_inputiterator_destructor(&iterator);
		npeg_reset(&context);
		
		*input = 'A' + i;
		npeg_inputiterator_constructor(&iterator, input, 2);
		assert(1 == npeg_CharacterClass(&iterator, &context, "[A-Z0-9a-z]", 11));
		printf("\tVerified: [A-Z0-9a-z] match with %c as input.\n", *input);
		assert(1 == iterator.index);
		printf("\tVerified: On match iterator should consume character.\n");
		npeg_inputiterator_destructor(&iterator);
		npeg_reset(&context);
		
		*input = '0' + i%10;
		npeg_inputiterator_constructor(&iterator, input, 2);
		assert(1 == npeg_CharacterClass(&iterator, &context, "[A-Z0-9a-z]", 11));
		printf("\tVerified: [A-Z0-9a-z] match with %c as input.\n", *input);
		assert(1 == iterator.index);
		printf("\tVerified: On match iterator should consume character.\n");
		npeg_inputiterator_destructor(&iterator);
		npeg_reset(&context);
	}


	
	printf("\n");
	printf("\tReached: interpreted simple escape sequence.\n");
	for(i = 0; i <= 9; i++)
	{
		*input = '0' + i;
		npeg_inputiterator_constructor(&iterator, input, 2);
		assert(1 == npeg_CharacterClass(&iterator, &context, "[\\d]", 4));
		printf("\tVerified: \\d match with %d as input.\n", i);
		assert(1 == iterator.index);
		printf("\tVerified: On match iterator should consume character.\n");
		npeg_inputiterator_destructor(&iterator);
		npeg_reset(&context);
	}
	
	
	printf("\n");
	printf("\tReached: interpreted groupped escape sequence.\n");

	//"[\\d\\D\\s\\S\\w\\W]"
	for(i = 0; i <= 9; i++)
	{
		*input = '0' + i;
		npeg_inputiterator_constructor(&iterator, input, 2);
		assert(1 == npeg_CharacterClass(&iterator, &context, "[\\d\\s]", 6));
		printf("\tVerified: [\\d\\s] match with %d as input.\n", i);
		assert(1 == iterator.index);
		printf("\tVerified: On match iterator should consume character.\n");
		npeg_inputiterator_destructor(&iterator);
		npeg_reset(&context);
		
		npeg_inputiterator_constructor(&iterator, input, 2);
		assert(1 == npeg_CharacterClass(&iterator, &context, "[\\s\\w]", 6));
		printf("\tVerified: [\\s\\w] match with %d as input.\n", i);
		assert(1 == iterator.index);
		printf("\tVerified: On match iterator should consume character.\n");
		npeg_inputiterator_destructor(&iterator);
		npeg_reset(&context);
	}

	printf("\n");
	*input = ' ';
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(1 == npeg_CharacterClass(&iterator, &context, "[\\d\\s]", 6));
	printf("\tVerified: [\\d\\s] match with ' ' as input.\n");
	assert(1 == iterator.index);
	printf("\tVerified: On match iterator should consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);

	*input = '\n';
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(1 == npeg_CharacterClass(&iterator, &context, "[\\d\\s]", 6));
	printf("\tVerified: [\\d\\s] match with \\n as input.\n");
	assert(1 == iterator.index);
	printf("\tVerified: On match iterator should consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);
	
	*input = '\f';
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(1 == npeg_CharacterClass(&iterator, &context, "[\\d\\s]", 6));
	printf("\tVerified: [\\d\\s] match with \\f as input.\n");
	assert(1 == iterator.index);
	printf("\tVerified: On match iterator should consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);
	
	*input = '\r';
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(1 == npeg_CharacterClass(&iterator, &context, "[\\d\\s]", 6));
	printf("\tVerified: [\\d\\s] match with \\r as input.\n");
	assert(1 == iterator.index);
	printf("\tVerified: On match iterator should consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);
	
	*input = '\t';
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(1 == npeg_CharacterClass(&iterator, &context, "[\\d\\s]", 6));
	printf("\tVerified: [\\d\\s] match with \\t as input.\n");
	assert(1 == iterator.index);
	printf("\tVerified: On match iterator should consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);
	
	*input = '\v';
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(1 == npeg_CharacterClass(&iterator, &context, "[\\d\\s]", 6));
	printf("\tVerified: [\\d\\s] match with \\v as input.\n");
	assert(1 == iterator.index);
	printf("\tVerified: On match iterator should consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);
	







	printf("\n");
	printf("\tReached: confirming negative character group.\n");
	printf("\n");
	
	printf("\tReached: confirming single negative character group.\n");
	*input = 'b';
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(1 == npeg_CharacterClass(&iterator, &context, "[^a]", 4));
	printf("\tVerified: [^a] matches input b.\n");
	assert(1 == iterator.index);
	printf("\tVerified: On match iterator should consume character.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);	
	
	*input = 'a';
	npeg_inputiterator_constructor(&iterator, input, 2);
	assert(0 == npeg_CharacterClass(&iterator, &context, "[^a]", 4));
	printf("\tVerified: [^a] does matches input a.\n");
	assert(0 == iterator.index);
	printf("\tVerified: On not match character is not consummed by iterator.\n");
	npeg_inputiterator_destructor(&iterator);
	npeg_reset(&context);	


	printf("\tReached: confirming escaped negative character group.\n");
	for(i = 0; i < 26; i++)
	{
		*input =  'a' + i;
		npeg_inputiterator_constructor(&iterator, input, 2);
		assert(1 == npeg_CharacterClass(&iterator, &context, "[^\\W]", 5));
		printf("\tVerified: [^\\W] which means [\\w] match with %c as input.\n", *input);
		assert(1 == iterator.index);
		printf("\tVerified: On match iterator should consume character.\n");
		npeg_inputiterator_destructor(&iterator);
		npeg_reset(&context);

		npeg_inputiterator_constructor(&iterator, input, 2);
		assert(0 == npeg_CharacterClass(&iterator, &context, "[^\\S]", 5));
		printf("\tVerified: [^\\S] which translates to postive character group [\\s] will not match with %c as input.\n", *input);
		assert(0 == iterator.index);
		printf("\tVerified: On no match character is not consummed by iterator.\n");
		npeg_inputiterator_destructor(&iterator);
		npeg_reset(&context);
	}


	
	
	// unload npeg managed memory
	npeg_destructor(&context);
	
	return 0;
}
