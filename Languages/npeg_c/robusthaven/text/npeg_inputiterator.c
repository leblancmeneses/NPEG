#include <string.h>
#include <assert.h>
#include "npeg_inputiterator.h"

typedef unsigned int uint;

void npeg_inputiterator_constructor(npeg_inputiterator *iterator, const char* string, int stringlength)
{
    iterator->string = string;
    iterator->index = 0;
    iterator->length = stringlength;
} 

void npeg_inputiterator_destructor(npeg_inputiterator *iterator)
{
	// currently no managed memory to clean up.
} 


int npeg_inputiterator_get_text(char *buffer, const npeg_inputiterator *iterator, const int start, const int end) 
{
  if (start >= end || start >= iterator->length) 
  {
    buffer[0] = 0;
    return 0;
  } 
  else if (start < 0)
  { 
  	return npeg_inputiterator_get_text(buffer, iterator, 0, end);
  }
  else 
  {
    int i;
    char *p_dst;    

    if (end < iterator->length) 
    {
      for (i = start, p_dst = buffer; i < end; i++, p_dst++) *p_dst = iterator->string[i];      
      *p_dst = 0;

      return end - start;
    } else {
      for (i = start, p_dst = buffer; i < iterator->length; i++, p_dst++) *p_dst = iterator->string[i];
      *p_dst = 0;

      return iterator->length - start;
    }
  }
}



int npeg_inputiterator_get_current(const npeg_inputiterator *iterator) {
  if (iterator->index >= iterator->length) return -1;
  else {
    return iterator->string[iterator->index];
  }
}


int npeg_inputiterator_get_next(npeg_inputiterator *iterator) {
  if (iterator->index >= iterator->length) return -1;
  else {
    iterator->index += 1;

    if (iterator->index >= iterator->length) return -1;
    else return iterator->string[iterator->index];
  }
}


int npeg_inputiterator_get_previous(npeg_inputiterator *iterator) {
  if (iterator->index <= 0) return -1;
  else {
    assert(iterator->length > 0);

    iterator->index -= 1;

    return iterator->string[iterator->index];
  }
}
