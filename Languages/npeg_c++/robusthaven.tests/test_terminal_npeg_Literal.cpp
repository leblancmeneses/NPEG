#include <cassert>
#include <cstdio>
#include <cstring>
#include "robusthaven/text/StringInputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;

const char* g_matchText = ".NET Parsing Expression Grammar";

class _LiteralTest : public Npeg {
private:
  int m_isCaseSensitive;

public:
  void makeCaseSensitive() {
    m_isCaseSensitive = true;
  }

  int isMatch(void) throw (ParsingFatalTerminalException) {
    return literal(g_matchText, strlen(g_matchText), m_isCaseSensitive);
  }

public:
  _LiteralTest(InputIterator *p_iter) : Npeg(p_iter) {
    m_isCaseSensitive = false;
  }
};

int main(int argc, char *argv[]) 
{  
  const char* string = ".nEt Parsing expression grammar";
  const char* string2 = ".NET Parsing Expression Grammar";
  const char* string3 = "invalid";
  const char errmsg[] = "some kind of error";

  StringInputIterator *p_iterator;
  _LiteralTest *p_context;

  p_iterator = new StringInputIterator(string, strlen(string));
  p_context = new _LiteralTest(p_iterator);
  assert(1 == p_context->isMatch());
  printf("\tVerified: branch of isCaseSensitive = false; input1 successfully matches.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(string2, strlen(string));
  p_context = new _LiteralTest(p_iterator);
  assert(1 == p_context->isMatch());
  printf("\tVerified: branch of isCaseSensitive = false; input2 successfully matches.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(string3, strlen(string));
  p_context = new _LiteralTest(p_iterator);
  assert(0 == p_context->isMatch());
  printf("\tVerified: branch of isCaseSensitive = false; input3 is NOT matched.\n");
  delete p_iterator; delete p_context;


  p_iterator = new StringInputIterator(string, strlen(string));
  p_context = new _LiteralTest(p_iterator);
  p_context->makeCaseSensitive();
  assert(0 == p_context->isMatch());
  printf("\tVerified: branch of isCaseSensitive = true; input1 is NOT matched.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(string2, strlen(string));
  p_context = new _LiteralTest(p_iterator);
  p_context->makeCaseSensitive();
  assert(1 == p_context->isMatch());
  printf("\tVerified: branch of isCaseSensitive = true; input2 is matched.\n");
  delete p_iterator; delete p_context;

  p_iterator = new StringInputIterator(string3, strlen(string));
  p_context = new _LiteralTest(p_iterator);
  p_context->makeCaseSensitive();
  assert(0 == p_context->isMatch());
  printf("\tVerified: branch of isCaseSensitive = true; input3 is NOT matched.\n");
  delete p_iterator; delete p_context;

  return 0;
}
