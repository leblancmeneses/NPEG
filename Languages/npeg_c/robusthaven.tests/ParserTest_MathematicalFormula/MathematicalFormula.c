#include <stdlib.h>
#include "MathematicalFormula.h"

int MathematicalFormula_impl_0(npeg_inputiterator* iterator, npeg_context* context)
{
	return MathematicalFormula_impl_1(iterator, context);
}
int MathematicalFormula_impl_1(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "EXPRESSION";

	return npeg_CapturingGroup(iterator,  context, &MathematicalFormula_impl_2, _nodeName_0, 0, 0);
}
int MathematicalFormula_impl_2(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_3, &MathematicalFormula_impl_28);
}
int MathematicalFormula_impl_28(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_ZeroOrMore(iterator,  context, &MathematicalFormula_impl_29);
}
int MathematicalFormula_impl_29(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_30, &MathematicalFormula_impl_34);
}
int MathematicalFormula_impl_34(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_35, &MathematicalFormula_impl_44);
}
int MathematicalFormula_impl_44(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_ZeroOrMore(iterator,  context, &MathematicalFormula_impl_45);
}
int MathematicalFormula_impl_45(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_46, &MathematicalFormula_impl_50);
}
int MathematicalFormula_impl_50(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_PrioritizedChoice(iterator,  context, &MathematicalFormula_impl_51, &MathematicalFormula_impl_54);
}
int MathematicalFormula_impl_54(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_55, &MathematicalFormula_impl_58);
}
int MathematicalFormula_impl_58(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = ")";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_55(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_56, &MathematicalFormula_impl_57);
}
int MathematicalFormula_impl_57(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_RecursionCall(iterator,  context, &MathematicalFormula_impl_0);
}
int MathematicalFormula_impl_56(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "(";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_51(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "VALUE";

	return npeg_CapturingGroup(iterator,  context, &MathematicalFormula_impl_52, _nodeName_0, 0, 0);
}
int MathematicalFormula_impl_52(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_OneOrMore(iterator,  context, &MathematicalFormula_impl_53);
}
int MathematicalFormula_impl_53(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int MathematicalFormula_impl_46(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "SYMBOL";

	return npeg_CapturingGroup(iterator,  context, &MathematicalFormula_impl_47, _nodeName_0, 0, 0);
}
int MathematicalFormula_impl_47(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_PrioritizedChoice(iterator,  context, &MathematicalFormula_impl_48, &MathematicalFormula_impl_49);
}
int MathematicalFormula_impl_49(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "/";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_48(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "*";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_35(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_PrioritizedChoice(iterator,  context, &MathematicalFormula_impl_36, &MathematicalFormula_impl_39);
}
int MathematicalFormula_impl_39(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_40, &MathematicalFormula_impl_43);
}
int MathematicalFormula_impl_43(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = ")";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_40(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_41, &MathematicalFormula_impl_42);
}
int MathematicalFormula_impl_42(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_RecursionCall(iterator,  context, &MathematicalFormula_impl_0);
}
int MathematicalFormula_impl_41(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "(";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_36(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "VALUE";

	return npeg_CapturingGroup(iterator,  context, &MathematicalFormula_impl_37, _nodeName_0, 0, 0);
}
int MathematicalFormula_impl_37(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_OneOrMore(iterator,  context, &MathematicalFormula_impl_38);
}
int MathematicalFormula_impl_38(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int MathematicalFormula_impl_30(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "SYMBOL";

	return npeg_CapturingGroup(iterator,  context, &MathematicalFormula_impl_31, _nodeName_0, 0, 0);
}
int MathematicalFormula_impl_31(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_PrioritizedChoice(iterator,  context, &MathematicalFormula_impl_32, &MathematicalFormula_impl_33);
}
int MathematicalFormula_impl_33(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "-";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_32(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "+";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_3(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_4, &MathematicalFormula_impl_13);
}
int MathematicalFormula_impl_13(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_ZeroOrMore(iterator,  context, &MathematicalFormula_impl_14);
}
int MathematicalFormula_impl_14(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_15, &MathematicalFormula_impl_19);
}
int MathematicalFormula_impl_19(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_PrioritizedChoice(iterator,  context, &MathematicalFormula_impl_20, &MathematicalFormula_impl_23);
}
int MathematicalFormula_impl_23(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_24, &MathematicalFormula_impl_27);
}
int MathematicalFormula_impl_27(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = ")";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_24(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_25, &MathematicalFormula_impl_26);
}
int MathematicalFormula_impl_26(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_RecursionCall(iterator,  context, &MathematicalFormula_impl_0);
}
int MathematicalFormula_impl_25(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "(";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_20(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "VALUE";

	return npeg_CapturingGroup(iterator,  context, &MathematicalFormula_impl_21, _nodeName_0, 0, 0);
}
int MathematicalFormula_impl_21(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_OneOrMore(iterator,  context, &MathematicalFormula_impl_22);
}
int MathematicalFormula_impl_22(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int MathematicalFormula_impl_15(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "SYMBOL";

	return npeg_CapturingGroup(iterator,  context, &MathematicalFormula_impl_16, _nodeName_0, 0, 0);
}
int MathematicalFormula_impl_16(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_PrioritizedChoice(iterator,  context, &MathematicalFormula_impl_17, &MathematicalFormula_impl_18);
}
int MathematicalFormula_impl_18(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "/";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_17(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "*";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_4(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_PrioritizedChoice(iterator,  context, &MathematicalFormula_impl_5, &MathematicalFormula_impl_8);
}
int MathematicalFormula_impl_8(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_9, &MathematicalFormula_impl_12);
}
int MathematicalFormula_impl_12(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = ")";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_9(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &MathematicalFormula_impl_10, &MathematicalFormula_impl_11);
}
int MathematicalFormula_impl_11(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_RecursionCall(iterator,  context, &MathematicalFormula_impl_0);
}
int MathematicalFormula_impl_10(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "(";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int MathematicalFormula_impl_5(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "VALUE";

	return npeg_CapturingGroup(iterator,  context, &MathematicalFormula_impl_6, _nodeName_0, 0, 0);
}
int MathematicalFormula_impl_6(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_OneOrMore(iterator,  context, &MathematicalFormula_impl_7);
}
int MathematicalFormula_impl_7(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
