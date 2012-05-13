#ifndef ROBUSTHAVEN_TEXT_STRINGINPUTITERATOR_H
#define ROBUSTHAVEN_TEXT_STRINGINPUTITERATOR_H

#include "InputIterator.h"

namespace RobustHaven
{
  namespace Text 
  {
    class StringInputIterator : public InputIterator {
    public:
      /*
       * Builds a new InputIterator with an underlying string stream.
       * The user has to know the length of the string since the data is interpreted as being binary.
       * A trailing 0 is not required.
       */
      StringInputIterator(const char* string, const size_t length);

      virtual ~StringInputIterator(void);
    };
  }
}
#endif

