#ifndef ROBUSTHAVEN_TEXT_WARN_H
#define ROBUSTHAVEN_TEXT_WARN_H

#include <string>

namespace RobustHaven
{
	namespace Text 
	{
		class Warn
		{
			public:
				Warn(const char message[], int iteratorIndex) 
					: m_message(message), m_iteratorIndex(iteratorIndex)
				{

				}

				~Warn(void)
				{
				}

				const std::string& getMessage() const;

				int getIteratorIndex() const;

			private: 
				std::string m_message;
				int m_iteratorIndex;
		};
	}
}
#endif
