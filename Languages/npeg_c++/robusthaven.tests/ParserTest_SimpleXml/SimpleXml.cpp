#include "SimpleXml.h"

using namespace RobustHaven::Text;

int SimpleXml::SimpleXml_impl_0()
{
	const char* _nodeName_0 = "Expression";

	return this->capturingGroup((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_1, _nodeName_0, 0, NULL);
}
int SimpleXml::SimpleXml_impl_1()
{
	return this->oneOrMore((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_2);
}
int SimpleXml::SimpleXml_impl_2()
{
	return this->sequence((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_3, (Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_23);
}
int SimpleXml::SimpleXml_impl_23()
{
	const char* _nodeName_0 = "END_TAG";

	return this->capturingGroup((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_24, _nodeName_0, 0, NULL);
}
int SimpleXml::SimpleXml_impl_24()
{
	return this->sequence((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_25, (Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_28);
}
int SimpleXml::SimpleXml_impl_28()
{
	const char* _literal_0 = ">";

	return this->literal(_literal_0, 1, 1);
}
int SimpleXml::SimpleXml_impl_25()
{
	return this->sequence((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_26, (Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_27);
}
int SimpleXml::SimpleXml_impl_27()
{
	const char* _dynamicBackReference_0 = "TAG";

	return this->dynamicBackReference(_dynamicBackReference_0, 1);
}
int SimpleXml::SimpleXml_impl_26()
{
	const char* _literal_0 = "</";

	return this->literal(_literal_0, 2, 1);
}
int SimpleXml::SimpleXml_impl_3()
{
	return this->sequence((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_4, (Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_12);
}
int SimpleXml::SimpleXml_impl_12()
{
	const char* _nodeName_0 = "Body";

	return this->capturingGroup((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_13, _nodeName_0, 0, NULL);
}
int SimpleXml::SimpleXml_impl_13()
{
	return this->zeroOrMore((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_14);
}
int SimpleXml::SimpleXml_impl_14()
{
	return this->sequence((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_15, (Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_22);
}
int SimpleXml::SimpleXml_impl_22()
{
	return this->anyCharacter();
}
int SimpleXml::SimpleXml_impl_15()
{
	return this->notPredicate((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_16);
}
int SimpleXml::SimpleXml_impl_16()
{
	const char* _nodeName_0 = "END_TAG";

	return this->capturingGroup((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_17, _nodeName_0, 0, NULL);
}
int SimpleXml::SimpleXml_impl_17()
{
	return this->sequence((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_18, (Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_21);
}
int SimpleXml::SimpleXml_impl_21()
{
	const char* _literal_0 = ">";

	return this->literal(_literal_0, 1, 1);
}
int SimpleXml::SimpleXml_impl_18()
{
	return this->sequence((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_19, (Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_20);
}
int SimpleXml::SimpleXml_impl_20()
{
	const char* _dynamicBackReference_0 = "TAG";

	return this->dynamicBackReference(_dynamicBackReference_0, 1);
}
int SimpleXml::SimpleXml_impl_19()
{
	const char* _literal_0 = "</";

	return this->literal(_literal_0, 2, 1);
}
int SimpleXml::SimpleXml_impl_4()
{
	const char* _nodeName_0 = "START_TAG";

	return this->capturingGroup((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_5, _nodeName_0, 0, NULL);
}
int SimpleXml::SimpleXml_impl_5()
{
	return this->sequence((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_6, (Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_11);
}
int SimpleXml::SimpleXml_impl_11()
{
	const char* _literal_0 = ">";

	return this->literal(_literal_0, 1, 1);
}
int SimpleXml::SimpleXml_impl_6()
{
	return this->sequence((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_7, (Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_8);
}
int SimpleXml::SimpleXml_impl_8()
{
	const char* _nodeName_0 = "TAG";

	return this->capturingGroup((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_9, _nodeName_0, 0, NULL);
}
int SimpleXml::SimpleXml_impl_9()
{
	return this->oneOrMore((Npeg::IsMatchPredicate)&SimpleXml::SimpleXml_impl_10);
}
int SimpleXml::SimpleXml_impl_10()
{
	const char* _classExpression_0 = "[a-zA-Z0-9]";

	return this->characterClass(_classExpression_0, 11);
}
int SimpleXml::SimpleXml_impl_7()
{
	const char* _literal_0 = "<";

	return this->literal(_literal_0, 1, 1);
}
