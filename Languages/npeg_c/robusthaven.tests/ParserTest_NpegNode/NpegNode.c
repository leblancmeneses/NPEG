#include <stdlib.h>
#include "NpegNode.h"

int NpegNode_impl_0(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _nodeName_0 = "NpegNode";

	return npeg_CapturingGroup(iterator,  context, &NpegNode_impl_1, _nodeName_0, 0, 0);
}
int NpegNode_impl_1(npeg_inputiterator* iterator, npeg_context* context)
{
	return npeg_PrioritizedChoice(iterator,  context, &NpegNode_impl_2, &NpegNode_impl_3);
}
int NpegNode_impl_3(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = ".NET Parsing Expression Grammar";

	return npeg_Literal(iterator,  context, _literal_0, 31, 0);
}
int NpegNode_impl_2(npeg_inputiterator* iterator, npeg_context* context)
{
	char* _literal_0 = "NPEG";

	return npeg_Literal(iterator,  context, _literal_0, 4, 0);
}
