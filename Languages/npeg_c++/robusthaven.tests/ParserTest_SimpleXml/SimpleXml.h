#ifndef ROBUSTHAVEN_GENERATEDPARSER_SIMPLEXML
#define ROBUSTHAVEN_GENERATEDPARSER_SIMPLEXML

#include "robusthaven/text/InputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;

class SimpleXml : public Npeg
{
 public:
  SimpleXml(InputIterator* inputstream): Npeg(inputstream){}

  virtual int isMatch() throw (ParsingFatalTerminalException) 
  { return SimpleXml_impl_0(); }


 private:
  int SimpleXml_impl_0();
  int SimpleXml_impl_1();
  int SimpleXml_impl_2();
  int SimpleXml_impl_23();
  int SimpleXml_impl_24();
  int SimpleXml_impl_28();
  int SimpleXml_impl_25();
  int SimpleXml_impl_27();
  int SimpleXml_impl_26();
  int SimpleXml_impl_3();
  int SimpleXml_impl_12();
  int SimpleXml_impl_13();
  int SimpleXml_impl_14();
  int SimpleXml_impl_22();
  int SimpleXml_impl_15();
  int SimpleXml_impl_16();
  int SimpleXml_impl_17();
  int SimpleXml_impl_21();
  int SimpleXml_impl_18();
  int SimpleXml_impl_20();
  int SimpleXml_impl_19();
  int SimpleXml_impl_4();
  int SimpleXml_impl_5();
  int SimpleXml_impl_11();
  int SimpleXml_impl_6();
  int SimpleXml_impl_8();
  int SimpleXml_impl_9();
  int SimpleXml_impl_10();
  int SimpleXml_impl_7();
};

#endif
