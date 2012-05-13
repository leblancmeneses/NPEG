#ifndef ROBUSTHAVEN_TEXT_NPEG_INPUTITERATOR_H
#define ROBUSTHAVEN_TEXT_NPEG_INPUTITERATOR_H

typedef struct npeg_inputiterator {
  const char *string;
  int index;
  int length; // length of the referenced string
} npeg_inputiterator;


// responsibility is to set up a new string, over which is iterated. 
// string length is not implicitly determined.  The user is required to explicitly define stringLength.
void npeg_inputiterator_constructor(npeg_inputiterator *iterator, const char* string, int stringLength);

// responsibility is to release any inputiterator managed memory
void npeg_inputiterator_destructor(npeg_inputiterator *iterator);


/*
 * Returns a null-terminated substring from the string in the iterator, hence the 
 * destination buffer ought to have a size of end - start + 1 bytes.
 * The string maybe shorter than end - start, if the terminating zero of the iterator string
 * lies before the index "end". If start is behind the end of the source string, the substring
 * will be empty. If start < 0, the substring beginning at 0 and ending at end will be copied.
 * The routine returns the number of copied bytes.
 */
int npeg_inputiterator_get_text(char *buffer, const npeg_inputiterator *iterator, 
				const int start, const int end);

/*
 * Returns the current character, does not change the index.
 * If the iterator has reached the end of the string, -1 is returned.
 */
int npeg_inputiterator_get_current(const npeg_inputiterator *iterator);

/*
 * Returns the next character, increments the index (if the index is not already at the end of the string).
 * If the iterator has reached the end of the string, -1 is returned.
 */
int npeg_inputiterator_get_next(npeg_inputiterator *iterator);

/*
 * Returns the previous character, decrements the index (if the index is not already at the beginning 
 * of the string).
 * If the iterator has reached the beginning of the string, i.e. if the character before the first
 * character of the string is requested, -1 is returned.
 */
int npeg_inputiterator_get_previous(npeg_inputiterator *iterator);

#endif
