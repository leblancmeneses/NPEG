#include <sstream>
#include "StringInputIterator.h"

using namespace std;
using namespace RobustHaven::Text;

StringInputIterator::StringInputIterator(const char* string, const size_t length) {
  stringstream *p_tmp;

  p_tmp = new stringstream("", ios_base::in|ios_base::out|ios_base::binary);
  p_tmp->write(string, length);
  p_tmp->seekg(0, ios::beg);
  m_stream = p_tmp;
  m_length = length;
}

StringInputIterator::~StringInputIterator() {
  delete m_stream;
}
