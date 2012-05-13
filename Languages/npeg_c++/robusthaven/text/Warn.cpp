#include "Warn.h"

using namespace RobustHaven::Text;

const std::string& Warn::getMessage() const
{
	return this->m_message;
}

int Warn::getIteratorIndex() const
{
	return this->m_iteratorIndex;
}

