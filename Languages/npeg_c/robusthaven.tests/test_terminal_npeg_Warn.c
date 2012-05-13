#include <assert.h>
#include <stdio.h>
#include <string.h>
#include "robusthaven/structures/stack.h"
#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"


int main(int argc, char *argv[]) 
{
	char* string = "A user warning message.";

	rh_stack_instance disableBackReferenceStack;
	rh_stackstack_instance astNodeStack;
	rh_hashmap_instance backreference_lookup;
	rh_list_instance warnings;
	rh_stack_instance errors;
	npeg_context context;	
	npeg_inputiterator iterator;
	
	// load npeg managed memory
	context.disableBackReferenceStack = &disableBackReferenceStack;
	context.sandbox = &astNodeStack;
	context.backReferenceLookup = &backreference_lookup;
	context.warnings = &warnings; 
	context.errors = &errors;
	npeg_constructor(&context, NULL);
	
	npeg_inputiterator_constructor(&iterator, string, 0);
	
	assert( 0 == rh_list_count(context.warnings) );
	printf("\tVerified: context.warnings is zero at start.\n");
	
	assert( 1 == npeg_Warn(&iterator, &context, string) );
	printf("\tVerified: npeg_Warn will always return true.\n");

	assert( 1 == rh_list_count(context.warnings));
	printf("\tVerified: npeg_Warn will add an warning to context.warnings collection.\n");
	
	assert( string != ((npeg_warn*)rh_list_get_item(context.warnings, 0))->message);
	printf("\tVerified: that the warn message is persisted in an npeg managed area spot and string pointers do not match.\n");
	
	assert(0 == strcmp(((npeg_warn*)rh_list_get_item(context.warnings, 0))->message, 
			   "A user warning message."));
	printf("\tVerified: that the original warn message was copied in full to the new allocated npeg managed memory.\n");
	
	npeg_inputiterator_destructor(&iterator);
	
	// unload npeg managed memory
	npeg_destructor(&context);
	
	
	return 0;
}
