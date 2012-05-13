#ifndef ROBUSTHAVEN_GENERATEDPARSER_PHONENUMBER
#define ROBUSTHAVEN_GENERATEDPARSER_PHONENUMBER

#include "robusthaven/text/InputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;

class PhoneNumber : public Npeg
{
public:
  PhoneNumber(InputIterator* inputstream): Npeg(inputstream){}
  
  virtual int isMatch() throw (ParsingFatalTerminalException) 
  { return PhoneNumber_impl_0(); }


private:
  int PhoneNumber_impl_0();
  int PhoneNumber_impl_1();
  int PhoneNumber_impl_19();
  int PhoneNumber_impl_20();
  int PhoneNumber_impl_26();
  int PhoneNumber_impl_21();
  int PhoneNumber_impl_25();
  int PhoneNumber_impl_22();
  int PhoneNumber_impl_24();
  int PhoneNumber_impl_23();
  int PhoneNumber_impl_2();
  int PhoneNumber_impl_18();
  int PhoneNumber_impl_3();
  int PhoneNumber_impl_12();
  int PhoneNumber_impl_13();
  int PhoneNumber_impl_17();
  int PhoneNumber_impl_14();
  int PhoneNumber_impl_16();
  int PhoneNumber_impl_15();
  int PhoneNumber_impl_4();
  int PhoneNumber_impl_11();
  int PhoneNumber_impl_5();
  int PhoneNumber_impl_6();
  int PhoneNumber_impl_10();
  int PhoneNumber_impl_7();
  int PhoneNumber_impl_9();
  int PhoneNumber_impl_8();
};

#endif
