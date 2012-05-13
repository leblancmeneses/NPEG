#ifndef ROBUSTHAVEN_TEXT_EXCEPTIONS_H
#define ROBUSTHAVEN_TEXT_EXCEPTIONS_H

#include <stdexcept>

namespace RobustHaven
{
  namespace Text 
  {
    class ParsingFatalTerminalException : public std::runtime_error {
    private:
      size_t m_errorPos;

    public:
      size_t getErrorPosition(void) const;

    public:
      ParsingFatalTerminalException(const std::string &msg, const size_t pos) 
	: runtime_error(msg), m_errorPos(pos) {}
    };    

    class InfiniteLoopException : public ParsingFatalTerminalException {
    public:
      InfiniteLoopException(const std::string &msg, const size_t pos) 
	: ParsingFatalTerminalException(msg, pos) {}
    };    

    extern inline size_t ParsingFatalTerminalException::getErrorPosition() const {
      return m_errorPos;
    }      
  }
}

#endif
