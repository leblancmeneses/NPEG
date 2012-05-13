#ifndef ROBUSTHAVEN_TEXT_TOKENMATCH_H
#define ROBUSTHAVEN_TEXT_TOKENMATCH_H

#include <string>

namespace RobustHaven
{
  namespace Text 
  {
    class TokenMatch
    {
      std::string m_name;	
      int m_start, m_end;

    public:
      const std::string& getName(void) const;

      int getStart(void) const;

      int getEnd(void) const;

    public:
      TokenMatch(const char name[], const int start, const int end);  
    };

    extern inline const std::string& TokenMatch::getName(void) const {
      return m_name;
    }

    extern inline int TokenMatch::getStart(void) const {
      return m_start;
    }

    extern inline int TokenMatch::getEnd(void) const {
      return m_end;
    }

    extern inline TokenMatch::TokenMatch(const char name[], const int start, const int end) 
      : m_name(name), m_start(start), m_end(end) {    
    }
  }
}
#endif
