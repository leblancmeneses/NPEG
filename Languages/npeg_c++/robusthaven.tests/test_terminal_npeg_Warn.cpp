#include <cassert>
#include <cstdio>
#include <cstring>
#include "robusthaven/text/StringInputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;

const char* g_string = "A user warning message.";

class _WarnTest : public Npeg {
public:
  int isMatch(void) throw (ParsingFatalTerminalException) {
    return warn(g_string);
  }

public:
  _WarnTest(InputIterator *p_iter) : Npeg(p_iter) {}
  
  virtual ~_WarnTest() {}
};

int main(int argc, char *argv[]) 
{
  StringInputIterator *p_iterator;
  Npeg *p_context;
	
  p_iterator = new StringInputIterator(g_string, 0);
  p_context = new _WarnTest(p_iterator);
	
  assert( 0 == p_context->getWarnings().size() );
  printf("\tVerified: context warnings is zero at start.\n");
	
  assert( 1 == p_context->isMatch() );
  printf("\tVerified: npeg_Warn will always return true.\n");

  assert( 1 == p_context->getWarnings().size());
  printf("\tVerified: npeg_Warn will add an warning to context warnings collection.\n");
	
  assert((*(p_context->getWarnings().end() - 1)).getMessage() == "A user warning message.");
  printf("\tVerified: that the original warn message was copied in full to the new allocated npeg managed memory.\n");
	
  delete p_iterator; delete p_context;
	
	
  return 0;
}
