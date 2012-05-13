#include <iostream>
#include <cassert>
#include <cstdio>
#include <cstring>
#include <string>
#include "robusthaven/text/StringInputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;
using namespace std;

const char g_errorStr[] = "some error";

class _FatalTest : public Npeg {
public:
  int isMatch(void) throw (ParsingFatalTerminalException) {
    return fatal(g_errorStr);
  }

public:
  _FatalTest(InputIterator *p_iter) : Npeg(p_iter) {}
};


int main(int argc, char *argv[]) 
{
  const char* string = "A Fatal Exception Message";

  StringInputIterator *p_iterator;
  Npeg *p_context;
  bool errorSeen;
		
  p_iterator = new StringInputIterator(string, 0);
  p_context = new _FatalTest(p_iterator);
  errorSeen = false;
  try {
    p_context->isMatch();
  } catch (ParsingFatalTerminalException e) {
    errorSeen = true;
    assert(e.what() == std::string(g_errorStr));
    cout << "\tVerified: error string matches input\n";
  }
  assert(errorSeen);
  printf("\tVerified: fatal works.\n");
	
  delete p_iterator;
  delete p_context;
	
  return 0;
}
