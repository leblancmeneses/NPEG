#ifndef ROBUSTHAVEN_TEXT_INPUTITERATOR_H
#define ROBUSTHAVEN_TEXT_INPUTITERATOR_H

#include <fstream>

namespace RobustHaven
{
  namespace Text 
  {
    class InputIterator 
    {
    protected:
      std::istream *m_stream;
      size_t m_length;

    public:
      size_t getIndex(void) const;

      void setIndex(size_t index);

      bool isAtEnd(void) const;

      size_t getLength(void) const;

    public:
      /*
       * Returns the next character, increments the index (if the index is not already at the 
       * end of the string).
       * If the iterator has reached the end of the string, EOF is returned.
       */
      int getNext(void);

      /*
       * Returns the current character, does not change the index.
       * If the iterator has reached the end of the string, EOF is returned.
       */
      int getCurrent(void) const;

    public:
      /*
       * Returns a null-terminated substring from the string in the iterator, hence the 
       * destination buffer ought to have a size of end - start + 1 bytes.
       * The string maybe shorter than end - start, if the terminating zero of the iterator string
       * lies before the index "end". If start is behind the end of the source string, the substring
       * will be empty. If start < 0, the substring beginning at 0 and ending at end will be copied.
       * The routine returns the number of copied bytes.
       */
      int getText(char *buffer, const size_t start, const size_t end) const;

    public:      
      /*
       * Builds an input iterator based on a stream object that was created by the client.
       * The stream object will be manipulated by the InputIterator, however its memory remains
       * under client control.
       */
      InputIterator(std::istream *stream);

    protected:
      InputIterator(void) {}
      
    public:
      virtual ~InputIterator(void) {}
    };

    extern inline int InputIterator::getCurrent() const {
      return m_stream->peek();
    }

    extern inline int InputIterator::getNext() {
      return m_stream->rdbuf()->snextc();
    }

    extern inline size_t InputIterator::getIndex() const {
      return m_stream->tellg();
    }

    extern inline size_t InputIterator::getLength() const {
      return m_length;
    }

    extern inline bool InputIterator::isAtEnd() const {
      return m_stream->eof() || getIndex() >= m_length;
    }
  }
}

#endif
