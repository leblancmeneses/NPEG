#include "PhoneNumber.h"

using namespace RobustHaven::Text;

int PhoneNumber::PhoneNumber_impl_0()
{
	const char* _nodeName_0 = "PhoneNumber";

	return this->capturingGroup((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_1, _nodeName_0, 0, NULL);
}
int PhoneNumber::PhoneNumber_impl_1()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_2, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_19);
}
int PhoneNumber::PhoneNumber_impl_19()
{
	const char* _nodeName_0 = "FourDigitCode";

	return this->capturingGroup((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_20, _nodeName_0, 0, NULL);
}
int PhoneNumber::PhoneNumber_impl_20()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_21, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_26);
}
int PhoneNumber::PhoneNumber_impl_26()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int PhoneNumber::PhoneNumber_impl_21()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_22, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_25);
}
int PhoneNumber::PhoneNumber_impl_25()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int PhoneNumber::PhoneNumber_impl_22()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_23, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_24);
}
int PhoneNumber::PhoneNumber_impl_24()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int PhoneNumber::PhoneNumber_impl_23()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int PhoneNumber::PhoneNumber_impl_2()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_3, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_18);
}
int PhoneNumber::PhoneNumber_impl_18()
{
	const char* _literal_0 = "-";

	return this->literal(_literal_0, 1, 1);
}
int PhoneNumber::PhoneNumber_impl_3()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_4, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_12);
}
int PhoneNumber::PhoneNumber_impl_12()
{
	const char* _nodeName_0 = "ThreeDigitCode";

	return this->capturingGroup((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_13, _nodeName_0, 0, NULL);
}
int PhoneNumber::PhoneNumber_impl_13()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_14, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_17);
}
int PhoneNumber::PhoneNumber_impl_17()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int PhoneNumber::PhoneNumber_impl_14()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_15, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_16);
}
int PhoneNumber::PhoneNumber_impl_16()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int PhoneNumber::PhoneNumber_impl_15()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int PhoneNumber::PhoneNumber_impl_4()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_5, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_11);
}
int PhoneNumber::PhoneNumber_impl_11()
{
	const char* _literal_0 = "-";

	return this->literal(_literal_0, 1, 1);
}
int PhoneNumber::PhoneNumber_impl_5()
{
	const char* _nodeName_0 = "ThreeDigitCode";

	return this->capturingGroup((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_6, _nodeName_0, 0, NULL);
}
int PhoneNumber::PhoneNumber_impl_6()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_7, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_10);
}
int PhoneNumber::PhoneNumber_impl_10()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int PhoneNumber::PhoneNumber_impl_7()
{
	return this->sequence((Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_8, (Npeg::IsMatchPredicate)&PhoneNumber::PhoneNumber_impl_9);
}
int PhoneNumber::PhoneNumber_impl_9()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
int PhoneNumber::PhoneNumber_impl_8()
{
	const char* _classExpression_0 = "[0-9]";

	return this->characterClass(_classExpression_0, 5);
}
