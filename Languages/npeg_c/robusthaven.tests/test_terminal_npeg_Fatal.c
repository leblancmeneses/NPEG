#include <assert.h>
#include <stdio.h>
#include <string.h>
#include "robusthaven/structures/stack.h"
#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"


int main(int argc, char *argv[]) 
{
	char* string = "A Fatal Exception Message";

	rh_stack_instance disableBackReferenceStack;
	rh_stackstack_instance astNodeStack;
	rh_hashmap_instance backreference_lookup;
	rh_list_instance warnings;
	rh_stack_instance errors;
	npeg_context context;	
	npeg_inputiterator iterator;
	void *node;
	
	// load npeg managed memory
	context.disableBackReferenceStack = &disableBackReferenceStack;
	context.sandbox = &astNodeStack;
	context.backReferenceLookup = &backreference_lookup;
	context.warnings = &warnings; 
	context.errors = &errors;
	npeg_constructor(&context, NULL);
	
	npeg_inputiterator_constructor(&iterator, string, 0);
	
	assert( 0 == rh_stack_count(context.errors) );
	printf("\tVerified: context.errors is zero at start.\n");
	
	assert( 0 == npeg_Fatal(&iterator, &context, string) );
	printf("\tVerified: npeg_Fatal will always return false.\n");

	assert( 1 == rh_stack_count(context.errors));
	printf("\tVerified: npeg_Fatal will add an error to context.errors collection.\n");
	
	node = rh_stack_peek(context.errors);
	assert( string != ((npeg_error*)node)->message );
	printf("\tVerified: that the error message is persisted in an npeg managed area spot and string pointers do not match.\n");
	
	assert(0 == strcmp(((npeg_error*)node)->message, "A Fatal Exception Message"));
	printf("\tVerified: that the original error message was copied in full to the new allocated npeg managed memory.\n");
	
	npeg_inputiterator_destructor(&iterator);
	
	// unload npeg managed memory
	npeg_destructor(&context);
	
	
	return 0;
}
