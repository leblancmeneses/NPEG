#include <iostream>
#include <cassert>
#include <cstdio>
#include <cstring>
#include "robusthaven/text/StringInputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;
using namespace std;

class _CodeTest : public Npeg {
private:
  const char *m_codeStr;
  int m_len;

public:
  void setCode(const char codeStr[], const int len) {
    m_codeStr = codeStr;
    m_len = len;
  }

  int isMatch(void) throw (ParsingFatalTerminalException) {
    return codePoint(m_codeStr, m_len);
  }

public:
  _CodeTest(InputIterator *p_iter) : Npeg(p_iter) {}
};

int main(int argc, char *argv[]) 
{
#define OTHER_STRING "blah this is something else"
  const unsigned char input1[] = OTHER_STRING"\x78\x9a\xbc\xde\xf0\x01\x02";
  const char match_h1[] = "#x789abcdef00102";
  const char match_h1w[] = "#x789Xbcxef00X02";
  const char match_b1[] = "#b""111""1000""1001""1010";
  const char match_b1w[] = "#b""11X""x000""100X""1x10";
  const unsigned char input2[] = OTHER_STRING"\x08\x9a\xbc\xde\xf0\x01\x02";
  const char match_h2[] = "#x89abcdef00102";
  const char match_h2w[] = "#x8XabxdeX00x02";
  const unsigned char input3[] = OTHER_STRING"\xf0\x9a\xbc";
  const char match_h3[] = "#xf09abc";
  const char match_b3[] = "#b""1111""0000""1001""1010""1011""1100";
  const unsigned char input4[] = "\x08\x9a";
  const unsigned char input5[] = OTHER_STRING"\0";
  const char match_d5[] = "#0";

  unsigned int match_d3_val, match_d4_val, i;
  char match_d3[100], match_d4[100];
  StringInputIterator *p_iterator;
  _CodeTest *p_context;

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

  p_iterator = new StringInputIterator((const char*)input1, strlen((const char*)input1));
  p_context = new _CodeTest(p_iterator);
  p_context->setCode(match_h1, strlen(match_h1));
  assert(p_context->isMatch() == 0);
  p_context->setCode(match_b1, strlen(match_b1));
  assert(p_context->isMatch() == 0);
  p_context->setCode(match_d4, strlen(match_d4));
  assert(p_context->isMatch() == 0);
  assert(p_iterator->getIndex() == 0);
  puts("\tVerfied: Handling of string mismatch");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator((const char*)input1, strlen((const char*)input1));
  p_context = new _CodeTest(p_iterator);
  p_context->setCode(match_h1w, strlen(match_h1));
  assert(p_context->isMatch() == 0);
  p_context->setCode(match_b1w, strlen(match_b1));
  assert(p_context->isMatch() == 0);
  assert(p_iterator->getIndex() == 0);
  puts("\tVerfied: Handling of string mismatch with wildcards");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator((const char*)input1, strlen((const char*)input1));
  p_context = new _CodeTest(p_iterator);
  p_iterator->setIndex(strlen(OTHER_STRING));
  p_context->setCode(match_h1, strlen(match_h1));
  assert(p_context->isMatch() == 1);
  assert(p_iterator->getIndex() == strlen((const char*)input1));
  p_iterator->setIndex(strlen(OTHER_STRING));
  p_context->setCode(match_b1, strlen(match_b1));
  assert(p_context->isMatch() == 1 && p_iterator->getIndex() == strlen(OTHER_STRING) + 2);
  puts("\tVerfied: Parsing of input1: no padding w/ hex, 1bit padding w/ binary");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator((const char*)input1, strlen((const char*)input1));
  p_context = new _CodeTest(p_iterator);
  p_iterator->setIndex(strlen(OTHER_STRING));
  p_context->setCode(match_h1w, strlen(match_h1));
  assert(p_context->isMatch() == 1);
  assert(p_iterator->getIndex() == strlen((const char*)input1));
  p_iterator->setIndex(strlen(OTHER_STRING));
  p_context->setCode(match_b1w, strlen(match_b1));
  assert(p_context->isMatch() == 1);
  assert(p_iterator->getIndex() == strlen(OTHER_STRING) + 2);
  puts("\tVerfied: Parsing of input1 /w wildcards: no padding w/ hex, 1bit padding w/ binary");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator((const char*)input2, strlen((const char*)input2));
  p_context = new _CodeTest(p_iterator);
  p_iterator->setIndex(strlen(OTHER_STRING));
  p_context->setCode(match_h2, strlen(match_h2));
  assert(p_context->isMatch() == 1);
  assert(p_iterator->getIndex() == strlen((const char*)input2));
  puts("\tVerfied: Parsing of input2: padding of 4bits w/ hex");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator((const char*)input2, strlen((const char*)input2));
  p_context = new _CodeTest(p_iterator);
  p_iterator->setIndex(strlen(OTHER_STRING));
  p_context->setCode(match_h2w, strlen(match_h2w));
  assert(p_context->isMatch() == 1);
  assert(p_iterator->getIndex() == strlen((const char*)input2));
  puts("\tVerfied: Parsing of input2 with wildcards: padding of 4bits w/ hex");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator((const char*)input3, strlen((const char*)input3));
  p_context = new _CodeTest(p_iterator);
  p_iterator->setIndex(strlen(OTHER_STRING));
  p_context->setCode(match_h3, strlen(match_h3));
  assert(p_context->isMatch() == 1);
  assert(p_iterator->getIndex() == strlen((const char*)input3));
  p_iterator->setIndex(strlen(OTHER_STRING));
  p_context->setCode(match_b3, strlen(match_b3));
  assert(p_context->isMatch() == 1);
  assert(p_iterator->getIndex() == strlen(OTHER_STRING) + 3);
  p_iterator->setIndex(strlen(OTHER_STRING));
  p_context->setCode(match_d3, strlen(match_d3));
  assert(p_context->isMatch() == 1);
  assert(p_iterator->getIndex() == strlen((const char*)input3));
  puts("\tVerfied: Parsing of input3: no padding");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator((const char*)input4, strlen((const char*)input4));
  p_context = new _CodeTest(p_iterator);
  p_context->setCode(match_d4, strlen(match_d4));
  assert(p_context->isMatch() == 1);
  puts("\tVerfied: Parsing of input4: short decimal test");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator((const char*)input5, strlen((const char*)input5) + 1);
  p_context = new _CodeTest(p_iterator);
  p_iterator->setIndex(strlen(OTHER_STRING));
  p_context->setCode(match_d5, strlen(match_d5));
  assert(p_context->isMatch() == 1);
  assert(p_iterator->getIndex() == strlen((const char*)input5) + 1);
  puts("\tVerfied: Parsing of input5: matching 0 with decimals");
  delete p_iterator; delete p_context;
  
  return 0;
}
