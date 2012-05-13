#include <cassert>
#include <cstdlib>
#include "InputIterator.h"
using namespace std;
using namespace RobustHaven::Text;

int InputIterator::getText(char *buffer, const size_t start, const size_t end) const {
  if (start >= end) {
    buffer[0] = 0;
    
    return 0;
  } else {
    size_t saved_pos, nof_chars, nof_read;
  
    saved_pos = getIndex();

    m_stream->seekg(start);
    if (m_stream->fail() || m_stream->bad()) {
      m_stream->seekg(saved_pos);
      buffer[0] = 0;
      
      return 0;
    }

    nof_chars = end - start;
    m_stream->read(buffer, nof_chars);
    nof_read = m_stream->gcount();
    buffer[nof_read] = 0;

    return (int)nof_read;
  }  
} 

InputIterator::InputIterator(istream *stream) {
  m_stream = stream;
  m_stream->seekg(0, ios::end);
  m_length = m_stream->tellg();
  m_stream->seekg(0, ios::beg);
}

void InputIterator::setIndex(size_t index) {
  m_stream->clear();
  m_stream->seekg(index);
}
