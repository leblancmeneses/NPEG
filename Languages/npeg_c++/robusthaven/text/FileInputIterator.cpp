#include "FileInputIterator.h"

using namespace std;
using namespace RobustHaven::Text;

FileInputIterator::FileInputIterator(const char path[]) : InputIterator(new ifstream(path)) {}

FileInputIterator::~FileInputIterator() {
  delete m_stream;
}
