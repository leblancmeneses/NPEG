#ifndef ROBUSTHAVEN_TEXT_NPEG_H
#define ROBUSTHAVEN_TEXT_NPEG_H

#include <stack>
#include <string>
#include <map>
#include "Warn.h"
#include "TokenMatch.h"
#include "AstNode.h"
#include "IAstNodeFactory.h"
#include "InputIterator.h"
#include "Exceptions.h"
#include <ext/hash_map>

namespace RobustHaven
{
  namespace Text 
  {
    class Npeg 
    {
    public:
      typedef int (Npeg::*IsMatchPredicate)();

    private:
      std::stack<bool> m_disableBackReferenceStack;
      std::stack<std::stack<AstNode*> > m_sandbox;
      __gnu_cxx::hash_map<const char*, std::stack<std::string> > m_backReferenceLookup;
      std::vector<Warn> m_warnings; /* allows parser to continue and provides information to user */
      InputIterator *m_iterator;
      IAstNodeFactory *m_astNodeFactory;

    public:
      Npeg(InputIterator *iterator, IAstNodeFactory *astNodeFactory = NULL);
      virtual ~Npeg(void);      

      /* 
       * Member Queries & Setters
       */
    public:
      AstNode* getAST(void);

      const std::vector<Warn>& getWarnings(void) const;

    public:
      /*
       * Main routine for matching an expression and a production rule.
       * Has to be user implemented for the specific rule that is to be tested.
       */
      virtual int isMatch(void) throw (ParsingFatalTerminalException) = 0;

      /*
       * Non Terminals
       */
    private:
      void _disableBackReferencePushOnStack(const bool doDisable);

    protected:
      bool andPredicate(IsMatchPredicate expr) throw (ParsingFatalTerminalException);

      bool notPredicate(IsMatchPredicate expr) throw (ParsingFatalTerminalException);

      bool prioritizedChoice(IsMatchPredicate left, IsMatchPredicate right)
	throw (ParsingFatalTerminalException);

      bool sequence(IsMatchPredicate left, IsMatchPredicate right) throw (ParsingFatalTerminalException);

      bool zeroOrMore(IsMatchPredicate expr) throw (ParsingFatalTerminalException);

      bool oneOrMore(IsMatchPredicate expr) throw (ParsingFatalTerminalException);

      bool optional(IsMatchPredicate expr) throw (ParsingFatalTerminalException);

      bool limitingRepetition(int, int, IsMatchPredicate expr) throw (ParsingFatalTerminalException);

      bool recursionCall(IsMatchPredicate expr) throw (ParsingFatalTerminalException);

      /*
       * The npeg_CustomNodeAllocator parameter is optional, pass NULL if no custom node type is required.
       */
      bool capturingGroup(IsMatchPredicate expr, const char *, bool, bool createCustomAstNode)
	throw (ParsingFatalTerminalException);

      /*
       * Terminals
       */
    protected:
      /*
       * Hex (#x), binary (#b), decimal (#) codes for matching are passed in matchcodes.
       * matchlen holds the total length of the code string including the leading type specifier.
       */
      bool codePoint(const char *matchcodes, const int matchlen) throw (ParsingFatalTerminalException);

      bool anyCharacter(void) throw (ParsingFatalTerminalException);

      bool characterClass(const char*, int) throw (ParsingFatalTerminalException);

      bool literal(const char*, const int, const bool) throw (ParsingFatalTerminalException);

      bool dynamicBackReference(const char*, int) throw (ParsingFatalTerminalException);

      bool fatal(const char*) throw (ParsingFatalTerminalException);

      bool warn(const char*);
    };
  }
}
#endif
