#ifndef ROBUSTHAVEN_GENERATEDPARSER_MATHEMATICALFORMULA
#define ROBUSTHAVEN_GENERATEDPARSER_MATHEMATICALFORMULA

#include "robusthaven/text/InputIterator.h"
#include "robusthaven/text/Npeg.h"

using namespace RobustHaven::Text;

class MathematicalFormula : public Npeg
{
public:
  MathematicalFormula(InputIterator* inputstream): Npeg(inputstream){}

  virtual int isMatch() throw (ParsingFatalTerminalException) 
  { return MathematicalFormula_impl_0(); } 


private:
  int MathematicalFormula_impl_0();
  int MathematicalFormula_impl_1();
  int MathematicalFormula_impl_2();
  int MathematicalFormula_impl_28();
  int MathematicalFormula_impl_29();
  int MathematicalFormula_impl_34();
  int MathematicalFormula_impl_44();
  int MathematicalFormula_impl_45();
  int MathematicalFormula_impl_50();
  int MathematicalFormula_impl_54();
  int MathematicalFormula_impl_58();
  int MathematicalFormula_impl_55();
  int MathematicalFormula_impl_57();
  int MathematicalFormula_impl_56();
  int MathematicalFormula_impl_51();
  int MathematicalFormula_impl_52();
  int MathematicalFormula_impl_53();
  int MathematicalFormula_impl_46();
  int MathematicalFormula_impl_47();
  int MathematicalFormula_impl_49();
  int MathematicalFormula_impl_48();
  int MathematicalFormula_impl_35();
  int MathematicalFormula_impl_39();
  int MathematicalFormula_impl_43();
  int MathematicalFormula_impl_40();
  int MathematicalFormula_impl_42();
  int MathematicalFormula_impl_41();
  int MathematicalFormula_impl_36();
  int MathematicalFormula_impl_37();
  int MathematicalFormula_impl_38();
  int MathematicalFormula_impl_30();
  int MathematicalFormula_impl_31();
  int MathematicalFormula_impl_33();
  int MathematicalFormula_impl_32();
  int MathematicalFormula_impl_3();
  int MathematicalFormula_impl_13();
  int MathematicalFormula_impl_14();
  int MathematicalFormula_impl_19();
  int MathematicalFormula_impl_23();
  int MathematicalFormula_impl_27();
  int MathematicalFormula_impl_24();
  int MathematicalFormula_impl_26();
  int MathematicalFormula_impl_25();
  int MathematicalFormula_impl_20();
  int MathematicalFormula_impl_21();
  int MathematicalFormula_impl_22();
  int MathematicalFormula_impl_15();
  int MathematicalFormula_impl_16();
  int MathematicalFormula_impl_18();
  int MathematicalFormula_impl_17();
  int MathematicalFormula_impl_4();
  int MathematicalFormula_impl_8();
  int MathematicalFormula_impl_12();
  int MathematicalFormula_impl_9();
  int MathematicalFormula_impl_11();
  int MathematicalFormula_impl_10();
  int MathematicalFormula_impl_5();
  int MathematicalFormula_impl_6();
  int MathematicalFormula_impl_7();
};

#endif
