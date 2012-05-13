#ifndef ROBUSTHAVEN_TEXT_FILEINPUTITERATOR_H
#define ROBUSTHAVEN_TEXT_FILEINPUTITERATOR_H

#include "InputIterator.h"

namespace RobustHaven
{
  namespace Text 
  {
    class FileInputIterator : public InputIterator {
    public:
      /* 
       * Builds a new InputIterator with an underlying file stream.
       */
      FileInputIterator(const char path[]);

      virtual ~FileInputIterator(void);
    };
  }
}

#endif
