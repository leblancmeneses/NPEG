#ifndef ROBUSTHAVEN_GENERATEDPARSER_NPEGNODE
#define ROBUSTHAVEN_GENERATEDPARSER_NPEGNODE

#include "robusthaven/text/InputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;

class NpegNode : public Npeg
{
 public:
  NpegNode(InputIterator* inputstream): Npeg(inputstream){}

  virtual int isMatch() throw (ParsingFatalTerminalException) 
  { return NpegNode_impl_0(); }


 private:
  int NpegNode_impl_0();
  int NpegNode_impl_1();
  int NpegNode_impl_3();
  int NpegNode_impl_2();
};

#endif
