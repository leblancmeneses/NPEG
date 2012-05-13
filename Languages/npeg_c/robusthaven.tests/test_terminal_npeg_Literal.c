#include <assert.h>
#include <stdio.h>
#include <string.h>
#include "robusthaven/text/npeg.h"
#include "robusthaven/text/npeg_inputiterator.h"

int main(int argc, char *argv[]) 
{  
	char* string = ".nEt Parsing expression grammar";
	char* string2 = ".NET Parsing Expression Grammar";
	char* string3 = "invalid";
	const char errmsg[] = "some kind of error";

	char* matchText = ".NET Parsing Expression Grammar";
	npeg_inputiterator iterator;
	rh_stack_instance disable_back_reference, errors;
	rh_list_instance warnings;
	rh_stackstack_instance ast_stack;
	rh_hashmap_instance lookup;
	npeg_error error;
	npeg_context context = {.disableBackReferenceStack = &disable_back_reference,
				.sandbox = &ast_stack,
				.backReferenceLookup = &lookup,
				.warnings = &warnings,
				.errors = &errors};

	npeg_constructor(&context, NULL);	

	npeg_inputiterator_constructor(&iterator, string, strlen(string));
	assert(1 == npeg_Literal(&iterator, &context, matchText, strlen(matchText), 0));
	printf("\tVerified: branch of isCaseSensitive = false; input1 successfully matches.\n");
	npeg_inputiterator_destructor(&iterator);

	npeg_inputiterator_constructor(&iterator, string, strlen(string));
	error.message = (char*)errmsg, error.iteratorIndex = 0;
	rh_stack_push(context.errors, &error);
	assert(0 == npeg_Literal(&iterator, &context, matchText, strlen(matchText), 0));
	rh_stack_pop(context.errors);
	puts("\tVerified: Abortion on error without modification of state.");  
	npeg_inputiterator_destructor(&iterator);	

	npeg_inputiterator_constructor(&iterator, string2, strlen(string));
	assert(1 == npeg_Literal(&iterator, &context, matchText, strlen(matchText), 0));
	printf("\tVerified: branch of isCaseSensitive = false; input2 successfully matches.\n");
	npeg_inputiterator_destructor(&iterator);

	npeg_inputiterator_constructor(&iterator, string3, strlen(string));
	assert(0 == npeg_Literal(&iterator, &context, matchText, strlen(matchText), 0));
	printf("\tVerified: branch of isCaseSensitive = false; input3 is NOT matched.\n");
	npeg_inputiterator_destructor(&iterator);


	npeg_inputiterator_constructor(&iterator, string, strlen(string));
	assert(0 == npeg_Literal(&iterator, &context, matchText, strlen(matchText), 1));
	printf("\tVerified: branch of isCaseSensitive = true; input1 is NOT matched.\n");
	npeg_inputiterator_destructor(&iterator);

	npeg_inputiterator_constructor(&iterator, string2, strlen(string));
	assert(1 == npeg_Literal(&iterator, &context, matchText, strlen(matchText), 1));
	printf("\tVerified: branch of isCaseSensitive = true; input2 is matched.\n");
	npeg_inputiterator_destructor(&iterator);

	npeg_inputiterator_constructor(&iterator, string3, strlen(string));
	assert(0 == npeg_Literal(&iterator, &context, matchText, strlen(matchText), 1));
	printf("\tVerified: branch of isCaseSensitive = true; input3 is NOT matched.\n");
	npeg_inputiterator_destructor(&iterator);

	npeg_destructor(&context);

	return 0;
}
