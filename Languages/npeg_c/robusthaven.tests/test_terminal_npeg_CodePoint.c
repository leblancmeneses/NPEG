#include <assert.h>
#include <stdio.h>
#include <string.h>
#include "robusthaven/text/npeg.h"

int main(int argc, char *argv[]) 
{
#define OTHER_STRING "blah this is something else"
  const unsigned char input1[] = OTHER_STRING"\x78\x9a\xbc\xde\xf0\x01\x02";
  const unsigned char match_h1[] = "#x789abcdef00102";
  const unsigned char match_h1w[] = "#x789Xbcxef00X02";
  const unsigned char match_b1[] = "#b""111""1000""1001""1010";
  const unsigned char match_b1w[] = "#b""11X""x000""100X""1x10";
  const unsigned char input2[] = OTHER_STRING"\x08\x9a\xbc\xde\xf0\x01\x02";
  const unsigned char match_h2[] = "#x89abcdef00102";
  const unsigned char match_h2w[] = "#x8XabxdeX00x02";
  const unsigned char input3[] = OTHER_STRING"\xf0\x9a\xbc";
  const unsigned char match_h3[] = "#xf09abc";
  const unsigned char match_b3[] = "#b""1111""0000""1001""1010""1011""1100";
  const unsigned char input4[] = "\x08\x9a";
  const unsigned char input5[] = OTHER_STRING"\0";
  const unsigned char match_d5[] = "#0";

  unsigned int match_d3_val, match_d4_val, i;
  char match_d3[100], match_d4[100];
  npeg_inputiterator iterator;
  rh_stack_instance disable_back_reference, errors;
  rh_list_instance warnings;
  rh_stackstack_instance ast_stack;
  rh_hashmap_instance lookup;
  npeg_context context;
  npeg_error error;
  
  context.disableBackReferenceStack = &disable_back_reference;
  context.sandbox = &ast_stack;
  context.backReferenceLookup = &lookup;
  context.warnings = &warnings; 
  context.errors = &errors;

  npeg_constructor(&context, NULL);

  match_d3_val =  0;
  for (i = 0; i < 3; i++) {
    match_d3_val = (match_d3_val << 8) + (uint)input3[strlen(OTHER_STRING)+i];
  }
  sprintf(match_d3, "#%d", match_d3_val);

  match_d4_val = 0;
  for (i = 0; i < 2; i++) {
    match_d4_val = (match_d4_val << 8) + (uint)input4[i];
  }
  sprintf(match_d4, "#%d", match_d4_val);  

  npeg_inputiterator_constructor(&iterator, input1, strlen(input1));
  assert(npeg_CodePoint(&iterator, &context, match_h1, strlen(match_h1)) == 0 
	 && npeg_CodePoint(&iterator, &context, match_b1, strlen(match_b1)) == 0
	 && npeg_CodePoint(&iterator, &context, match_d4, strlen(match_d4)) == 0
	 && iterator.index == 0);
  puts("\tVerfied: Handling of string mismatch");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, input1, strlen(input1));
  assert(npeg_CodePoint(&iterator, &context, match_h1w, strlen(match_h1)) == 0 
	 && npeg_CodePoint(&iterator, &context, match_b1w, strlen(match_b1)) == 0
	 && iterator.index == 0);
  puts("\tVerfied: Handling of string mismatch with wildcards");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, input1, strlen(input1));
  iterator.index = strlen(OTHER_STRING);
  assert(npeg_CodePoint(&iterator, &context, match_h1, strlen(match_h1)) == 1
	 && iterator.index == strlen(input1));
  iterator.index = strlen(OTHER_STRING);
  assert(npeg_CodePoint(&iterator, &context, match_b1, strlen(match_b1)) == 1
	 && iterator.index == strlen(OTHER_STRING) + 2);
  puts("\tVerfied: Parsing of input1: no padding w/ hex, 1bit padding w/ binary");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, input1, strlen(input1));
  iterator.index = strlen(OTHER_STRING);
  assert(npeg_CodePoint(&iterator, &context, match_h1w, strlen(match_h1)) == 1
	 && iterator.index == strlen(input1));
  iterator.index = strlen(OTHER_STRING);
  assert(npeg_CodePoint(&iterator, &context, match_b1w, strlen(match_b1)) == 1
	 && iterator.index == strlen(OTHER_STRING) + 2);
  puts("\tVerfied: Parsing of input1 /w wildcards: no padding w/ hex, 1bit padding w/ binary");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, input2, strlen(input2));
  iterator.index = strlen(OTHER_STRING);
  assert(npeg_CodePoint(&iterator, &context, match_h2, strlen(match_h2)) == 1
	 && iterator.index == strlen(input2));
  puts("\tVerfied: Parsing of input2: padding of 4bits w/ hex");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, input2, strlen(input2));
  iterator.index = strlen(OTHER_STRING);
  assert(npeg_CodePoint(&iterator, &context, match_h2w, strlen(match_h2)) == 1
	 && iterator.index == strlen(input2));
  puts("\tVerfied: Parsing of input2 with wildcards: padding of 4bits w/ hex");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, input3, strlen(input3));
  iterator.index = strlen(OTHER_STRING);
  assert(npeg_CodePoint(&iterator, &context, match_h3, strlen(match_h3)) == 1
	 && iterator.index == strlen(input3));
  iterator.index = strlen(OTHER_STRING);
  assert(npeg_CodePoint(&iterator, &context, match_b3, strlen(match_b3)) == 1
	 && iterator.index == strlen(OTHER_STRING) + 3);
  iterator.index = strlen(OTHER_STRING);
  assert(npeg_CodePoint(&iterator, &context, match_d3, strlen(match_d3)) == 1
	 && iterator.index == strlen(input3));
  puts("\tVerfied: Parsing of input3: no padding");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, input4, strlen(input4));
  assert(npeg_CodePoint(&iterator, &context, match_d4, strlen(match_d4)) == 1
	 && iterator.index == strlen(input4));
  puts("\tVerfied: Parsing of input4: short decimal test");
  npeg_inputiterator_destructor(&iterator);

  npeg_inputiterator_constructor(&iterator, input5, strlen(input5) + 1);
  iterator.index = strlen(OTHER_STRING);
  assert(npeg_CodePoint(&iterator, &context, match_d5, strlen(match_d5)) == 1
	 && iterator.index == strlen(input5) + 1);
  puts("\tVerfied: Parsing of input5: matching 0 with decimals");
  npeg_inputiterator_destructor(&iterator);
  
  npeg_destructor(&context);

  return 0;
}
