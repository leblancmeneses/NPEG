#include "MathematicalFormula.h"

using namespace RobustHaven::Text;

int MathematicalFormula::MathematicalFormula_impl_0()
{
	return this->MathematicalFormula_impl_1();
}
int MathematicalFormula::MathematicalFormula_impl_1()
{
	const char* _nodeName_0 = "EXPRESSION";

	return this->capturingGroup((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_2, _nodeName_0, 0, NULL);
}
int MathematicalFormula::MathematicalFormula_impl_2()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_3, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_28);
}
int MathematicalFormula::MathematicalFormula_impl_28()
{
	return this->zeroOrMore((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_29);
}
int MathematicalFormula::MathematicalFormula_impl_29()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_30, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_34);
}
int MathematicalFormula::MathematicalFormula_impl_34()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_35, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_44);
}
int MathematicalFormula::MathematicalFormula_impl_44()
{
	return this->zeroOrMore((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_45);
}
int MathematicalFormula::MathematicalFormula_impl_45()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_46, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_50);
}
int MathematicalFormula::MathematicalFormula_impl_50()
{
	return this->prioritizedChoice((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_51, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_54);
}
int MathematicalFormula::MathematicalFormula_impl_54()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_55, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_58);
}
int MathematicalFormula::MathematicalFormula_impl_58()
{
	const char* _literal_0 = ")";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_55()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_56, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_57);
}
int MathematicalFormula::MathematicalFormula_impl_57()
{
	return this->recursionCall((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_0);
}
int MathematicalFormula::MathematicalFormula_impl_56()
{
	const char* _literal_0 = "(";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_51()
{
	const char* _nodeName_0 = "VALUE";

	return this->capturingGroup((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_52, _nodeName_0, 0, NULL);
}
int MathematicalFormula::MathematicalFormula_impl_52()
{
	return this->oneOrMore((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_53);
}
int MathematicalFormula::MathematicalFormula_impl_53()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int MathematicalFormula::MathematicalFormula_impl_46()
{
	const char* _nodeName_0 = "SYMBOL";

	return this->capturingGroup((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_47, _nodeName_0, 0, NULL);
}
int MathematicalFormula::MathematicalFormula_impl_47()
{
	return this->prioritizedChoice((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_48, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_49);
}
int MathematicalFormula::MathematicalFormula_impl_49()
{
	const char* _literal_0 = "/";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_48()
{
	const char* _literal_0 = "*";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_35()
{
	return this->prioritizedChoice((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_36, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_39);
}
int MathematicalFormula::MathematicalFormula_impl_39()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_40, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_43);
}
int MathematicalFormula::MathematicalFormula_impl_43()
{
	const char* _literal_0 = ")";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_40()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_41, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_42);
}
int MathematicalFormula::MathematicalFormula_impl_42()
{
	return this->recursionCall((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_0);
}
int MathematicalFormula::MathematicalFormula_impl_41()
{
	const char* _literal_0 = "(";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_36()
{
	const char* _nodeName_0 = "VALUE";

	return this->capturingGroup((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_37, _nodeName_0, 0, NULL);
}
int MathematicalFormula::MathematicalFormula_impl_37()
{
	return this->oneOrMore((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_38);
}
int MathematicalFormula::MathematicalFormula_impl_38()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int MathematicalFormula::MathematicalFormula_impl_30()
{
	const char* _nodeName_0 = "SYMBOL";

	return this->capturingGroup((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_31, _nodeName_0, 0, NULL);
}
int MathematicalFormula::MathematicalFormula_impl_31()
{
	return this->prioritizedChoice((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_32, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_33);
}
int MathematicalFormula::MathematicalFormula_impl_33()
{
	const char* _literal_0 = "-";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_32()
{
	const char* _literal_0 = "+";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_3()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_4, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_13);
}
int MathematicalFormula::MathematicalFormula_impl_13()
{
	return this->zeroOrMore((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_14);
}
int MathematicalFormula::MathematicalFormula_impl_14()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_15, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_19);
}
int MathematicalFormula::MathematicalFormula_impl_19()
{
	return this->prioritizedChoice((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_20, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_23);
}
int MathematicalFormula::MathematicalFormula_impl_23()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_24, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_27);
}
int MathematicalFormula::MathematicalFormula_impl_27()
{
	const char* _literal_0 = ")";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_24()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_25, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_26);
}
int MathematicalFormula::MathematicalFormula_impl_26()
{
	return this->recursionCall((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_0);
}
int MathematicalFormula::MathematicalFormula_impl_25()
{
	const char* _literal_0 = "(";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_20()
{
	const char* _nodeName_0 = "VALUE";

	return this->capturingGroup((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_21, _nodeName_0, 0, NULL);
}
int MathematicalFormula::MathematicalFormula_impl_21()
{
	return this->oneOrMore((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_22);
}
int MathematicalFormula::MathematicalFormula_impl_22()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int MathematicalFormula::MathematicalFormula_impl_15()
{
	const char* _nodeName_0 = "SYMBOL";

	return this->capturingGroup((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_16, _nodeName_0, 0, NULL);
}
int MathematicalFormula::MathematicalFormula_impl_16()
{
	return this->prioritizedChoice((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_17, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_18);
}
int MathematicalFormula::MathematicalFormula_impl_18()
{
	const char* _literal_0 = "/";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_17()
{
	const char* _literal_0 = "*";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_4()
{
	return this->prioritizedChoice((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_5, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_8);
}
int MathematicalFormula::MathematicalFormula_impl_8()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_9, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_12);
}
int MathematicalFormula::MathematicalFormula_impl_12()
{
	const char* _literal_0 = ")";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_9()
{
	return this->sequence((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_10, (Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_11);
}
int MathematicalFormula::MathematicalFormula_impl_11()
{
	return this->recursionCall((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_0);
}
int MathematicalFormula::MathematicalFormula_impl_10()
{
	const char* _literal_0 = "(";

	return this->literal(_literal_0, 1, 1);
}
int MathematicalFormula::MathematicalFormula_impl_5()
{
	const char* _nodeName_0 = "VALUE";

	return this->capturingGroup((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_6, _nodeName_0, 0, NULL);
}
int MathematicalFormula::MathematicalFormula_impl_6()
{
	return this->oneOrMore((Npeg::IsMatchPredicate)&MathematicalFormula::MathematicalFormula_impl_7);
}
int MathematicalFormula::MathematicalFormula_impl_7()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
