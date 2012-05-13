#include <stdlib.h>
#include "SimpleXml.h"

int SimpleXml_impl_0(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "Expression";

	return npeg_CapturingGroup(iterator,  context, &SimpleXml_impl_1, _nodeName_0, 0, 0);
}
int SimpleXml_impl_1(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_OneOrMore(iterator,  context, &SimpleXml_impl_2);
}
int SimpleXml_impl_2(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &SimpleXml_impl_3, &SimpleXml_impl_23);
}
int SimpleXml_impl_23(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "END_TAG";

	return npeg_CapturingGroup(iterator,  context, &SimpleXml_impl_24, _nodeName_0, 0, 0);
}
int SimpleXml_impl_24(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &SimpleXml_impl_25, &SimpleXml_impl_28);
}
int SimpleXml_impl_28(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = ">";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int SimpleXml_impl_25(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &SimpleXml_impl_26, &SimpleXml_impl_27);
}
int SimpleXml_impl_27(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _dynamicBackReference_0 = "TAG";

	return npeg_DynamicBackReference(iterator,  context, _dynamicBackReference_0, 1);
}
int SimpleXml_impl_26(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "</";

	return npeg_Literal(iterator,  context, _literal_0, 2, 1);
}
int SimpleXml_impl_3(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &SimpleXml_impl_4, &SimpleXml_impl_12);
}
int SimpleXml_impl_12(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "Body";

	return npeg_CapturingGroup(iterator,  context, &SimpleXml_impl_13, _nodeName_0, 0, 0);
}
int SimpleXml_impl_13(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_ZeroOrMore(iterator,  context, &SimpleXml_impl_14);
}
int SimpleXml_impl_14(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &SimpleXml_impl_15, &SimpleXml_impl_22);
}
int SimpleXml_impl_22(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_AnyCharacter(iterator,  context);
}
int SimpleXml_impl_15(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_NotPredicate(iterator,  context, &SimpleXml_impl_16);
}
int SimpleXml_impl_16(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "END_TAG";

	return npeg_CapturingGroup(iterator,  context, &SimpleXml_impl_17, _nodeName_0, 0, 0);
}
int SimpleXml_impl_17(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &SimpleXml_impl_18, &SimpleXml_impl_21);
}
int SimpleXml_impl_21(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = ">";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int SimpleXml_impl_18(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &SimpleXml_impl_19, &SimpleXml_impl_20);
}
int SimpleXml_impl_20(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _dynamicBackReference_0 = "TAG";

	return npeg_DynamicBackReference(iterator,  context, _dynamicBackReference_0, 1);
}
int SimpleXml_impl_19(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "</";

	return npeg_Literal(iterator,  context, _literal_0, 2, 1);
}
int SimpleXml_impl_4(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "START_TAG";

	return npeg_CapturingGroup(iterator,  context, &SimpleXml_impl_5, _nodeName_0, 0, 0);
}
int SimpleXml_impl_5(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &SimpleXml_impl_6, &SimpleXml_impl_11);
}
int SimpleXml_impl_11(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = ">";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int SimpleXml_impl_6(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &SimpleXml_impl_7, &SimpleXml_impl_8);
}
int SimpleXml_impl_8(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "TAG";

	return npeg_CapturingGroup(iterator,  context, &SimpleXml_impl_9, _nodeName_0, 0, 0);
}
int SimpleXml_impl_9(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_OneOrMore(iterator,  context, &SimpleXml_impl_10);
}
int SimpleXml_impl_10(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[a-zA-Z0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 11);
}
int SimpleXml_impl_7(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "<";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
