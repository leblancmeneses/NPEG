#include "NpegNode.h"

using namespace RobustHaven::Text;

int NpegNode::NpegNode_impl_0()
{
	const char* _nodeName_0 = "NpegNode";

	return this->capturingGroup((Npeg::IsMatchPredicate)&NpegNode::NpegNode_impl_1, _nodeName_0, 0, NULL);
}
int NpegNode::NpegNode_impl_1()
{
	return this->prioritizedChoice((Npeg::IsMatchPredicate)&NpegNode::NpegNode_impl_2, (Npeg::IsMatchPredicate)&NpegNode::NpegNode_impl_3);
}
int NpegNode::NpegNode_impl_3()
{
	const char* _literal_0 = ".NET Parsing Expression Grammar";

	return this->literal(_literal_0, 31, 0);
}
int NpegNode::NpegNode_impl_2()
{
	const char* _literal_0 = "NPEG";

	return this->literal(_literal_0, 4, 0);
}
