#include <stdlib.h>
#include "PhoneNumber.h"

const int False = 0;
const int True = 1;

int PhoneNumber_impl_0(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "PhoneNumber";

	return npeg_CapturingGroup(iterator,  context, &PhoneNumber_impl_1, _nodeName_0, False, False);
}
int PhoneNumber_impl_1(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_2, &PhoneNumber_impl_19);
}
int PhoneNumber_impl_19(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "FourDigitCode";

	return npeg_CapturingGroup(iterator,  context, &PhoneNumber_impl_20, _nodeName_0, False, False);
}
int PhoneNumber_impl_20(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_21, &PhoneNumber_impl_26);
}
int PhoneNumber_impl_26(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int PhoneNumber_impl_21(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_22, &PhoneNumber_impl_25);
}
int PhoneNumber_impl_25(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int PhoneNumber_impl_22(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_23, &PhoneNumber_impl_24);
}
int PhoneNumber_impl_24(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int PhoneNumber_impl_23(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int PhoneNumber_impl_2(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_3, &PhoneNumber_impl_18);
}
int PhoneNumber_impl_18(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "-";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int PhoneNumber_impl_3(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_4, &PhoneNumber_impl_12);
}
int PhoneNumber_impl_12(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "ThreeDigitCode";

	return npeg_CapturingGroup(iterator,  context, &PhoneNumber_impl_13, _nodeName_0, False, False);
}
int PhoneNumber_impl_13(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_14, &PhoneNumber_impl_17);
}
int PhoneNumber_impl_17(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int PhoneNumber_impl_14(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_15, &PhoneNumber_impl_16);
}
int PhoneNumber_impl_16(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int PhoneNumber_impl_15(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int PhoneNumber_impl_4(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_5, &PhoneNumber_impl_11);
}
int PhoneNumber_impl_11(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "-";

	return npeg_Literal(iterator,  context, _literal_0, 1, 1);
}
int PhoneNumber_impl_5(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "ThreeDigitCode";

	return npeg_CapturingGroup(iterator,  context, &PhoneNumber_impl_6, _nodeName_0, False, False);
}
int PhoneNumber_impl_6(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_7, &PhoneNumber_impl_10);
}
int PhoneNumber_impl_10(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int PhoneNumber_impl_7(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_Sequence(iterator,  context, &PhoneNumber_impl_8, &PhoneNumber_impl_9);
}
int PhoneNumber_impl_9(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
int PhoneNumber_impl_8(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _classExpression_0 = "[0-9]";

	return npeg_CharacterClass(iterator,  context, _classExpression_0, 5);
}
