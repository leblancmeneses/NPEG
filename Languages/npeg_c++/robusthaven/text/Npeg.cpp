#include <stack>
#include <climits>
#include <cstdio>
#include <cstdlib>
#include <cassert>
#include <cstring>
#include <cctype>

#include "IAstNodeReplacement.h"
#include "Warn.h"
#include "TokenMatch.h"
#include "Npeg.h"

using namespace std;
using namespace RobustHaven::Text;


typedef unsigned char uchar;
typedef stack<string> Stringstack;

#define _abortError(message) throw ParsingFatalTerminalException(message, m_iterator->getIndex())

AstNode* Npeg::getAST() {
  if(!m_sandbox.empty() && !m_sandbox.top().empty()) {
    AstNode *p_root;

    p_root = m_sandbox.top().top();
    m_sandbox.top().pop();

    return p_root;
  } else return NULL;	
} 


const std::vector<Warn>& Npeg::getWarnings(void) const {
  return m_warnings;
}

void Npeg::_disableBackReferencePushOnStack(const bool doDisable) {
  m_disableBackReferenceStack.push(doDisable);
} 


Npeg::Npeg(InputIterator *iterator, IAstNodeFactory *astNodeFactory) 
  : m_iterator(iterator), m_astNodeFactory(astNodeFactory) {
  m_sandbox.push(stack<AstNode*>());
}

static void _freeSandbox(std::stack<std::stack<AstNode*> > &r_sandbox) {
  while (!r_sandbox.empty()) {
    std::stack<AstNode*> &r_childSandbox = r_sandbox.top();
    
    while (!r_childSandbox.empty()) {
      AstNode *p_node;
      
      p_node = r_childSandbox.top();
      delete p_node;
      r_childSandbox.pop();
    }
    r_sandbox.pop();
  }
}

Npeg::~Npeg() {
  AstNode *p_root;

  while (!m_disableBackReferenceStack.empty()) m_disableBackReferenceStack.pop();
  p_root = getAST();
  if (p_root != NULL) {
    AstNode::deleteAST(p_root);
  }
  _freeSandbox(m_sandbox);
  m_warnings.clear();
}

// Non Terminals
bool Npeg::andPredicate(IsMatchPredicate expr) throw (ParsingFatalTerminalException) {
  bool result;
  int savePosition = m_iterator->getIndex();

  _disableBackReferencePushOnStack(true);

  if ((this->*expr)()) result = true;
  else result = false;
  m_iterator->setIndex(savePosition);

  m_disableBackReferenceStack.pop();

  return result;
}

bool Npeg::notPredicate(IsMatchPredicate expr) throw (ParsingFatalTerminalException) {
  bool result;
  int savePosition = m_iterator->getIndex();

  _disableBackReferencePushOnStack(true);

  if (!(this->*expr)()) result = true;
  else result = false;
  m_iterator->setIndex(savePosition);

  m_disableBackReferenceStack.pop();

  return result;
} 

bool Npeg::prioritizedChoice(IsMatchPredicate left, IsMatchPredicate right) 
  throw (ParsingFatalTerminalException) {
  int savePosition;

  savePosition = m_iterator->getIndex();
  if ((this->*left)()) return true;

  m_iterator->setIndex(savePosition);
  if ((this->*right)()) return true;

  m_iterator->setIndex(savePosition);

  return false;
}

bool Npeg::sequence(IsMatchPredicate left, IsMatchPredicate right) throw (ParsingFatalTerminalException) {
  int result;
  int savePosition = m_iterator->getIndex();

  if ((this->*left)() && (this->*right)()) {
    result = true;
  } else {
    m_iterator->setIndex(savePosition);
    result = false;
  }

  return result;
}

bool Npeg::zeroOrMore(IsMatchPredicate expr) throw (ParsingFatalTerminalException) {
  size_t savePosition = m_iterator->getIndex();

  while ((this->*expr)()) {
    if (savePosition == m_iterator->getIndex()) {
      throw InfiniteLoopException("Infinite loop detected in zeroOrMore", m_iterator->getIndex());
    }
    savePosition = m_iterator->getIndex();
  }

  m_iterator->setIndex(savePosition);
	
  return 1;
}

bool Npeg::oneOrMore(IsMatchPredicate expr) throw (ParsingFatalTerminalException) {
  int cnt = 0;
  size_t savePosition = m_iterator->getIndex();

  while ((this->*expr)()) {
    if (savePosition == m_iterator->getIndex()) {
      throw InfiniteLoopException("Infinite loop detected in oneOrMore", m_iterator->getIndex());
    }
    savePosition = m_iterator->getIndex();
    cnt++;
  }

  m_iterator->setIndex(savePosition);

  return (cnt > 0);
}

bool Npeg::optional(IsMatchPredicate expr) throw (ParsingFatalTerminalException) {
  int savePosition = m_iterator->getIndex();

  if ((this->*expr)()) savePosition = m_iterator->getIndex();
  else {
    m_iterator->setIndex(savePosition);
  }

  return 1;
}

/*
 * Attention: no bound is indicated by a value -1 in min or max!
 */
bool Npeg::limitingRepetition(int min,  int max, IsMatchPredicate expr) 
  throw (ParsingFatalTerminalException) {
  int cnt = 0;
  size_t savePosition;
  size_t initialPosition;
  bool result = 0;

  initialPosition = (savePosition = m_iterator->getIndex());

  if (min > max && max != -1) _abortError("Repetition bounds invalid: min > max");

  if (min != -1) {
    if (max == -1) {
      // has a minimum but no max cap
      savePosition = m_iterator->getIndex();
      while ((this->*expr)()) {
	if (savePosition == m_iterator->getIndex()) {
	  throw InfiniteLoopException("Infinite loop detected in limitingRepetition", 
				      m_iterator->getIndex());
	}

	cnt++;
	savePosition = m_iterator->getIndex();
      }

      m_iterator->setIndex(savePosition);
      result = (cnt >= min);
    } else {
      // has a minimum and a max specified
      savePosition = m_iterator->getIndex();
      while ((this->*expr)()) {
	cnt++;
	savePosition = m_iterator->getIndex();

	if (cnt >= max) break;
      }

      m_iterator->setIndex(savePosition);
      result = (cnt <= max && cnt >= min);
    }
  } else {
    // zero or up to a max matches of e.
    savePosition = m_iterator->getIndex();
    while ((this->*expr)()) {
      cnt++;
      savePosition = m_iterator->getIndex();

      if (cnt >= max) break;
    }

    m_iterator->setIndex(savePosition);
    result = (cnt <= max);
  }

  if (result == 0) m_iterator->setIndex(initialPosition);

  return result;
}

bool Npeg::capturingGroup(IsMatchPredicate expr, const char* name, bool doReplaceBySingleChildNode,
			  bool createCustomAstNode) throw (ParsingFatalTerminalException) {
  bool result = true;
  int savePosition = m_iterator->getIndex();
  stack<AstNode*> *sandBox;
  AstNode *astNode;

  // load current sandbox into view.
  sandBox = &m_sandbox.top();
	
  // add an empty ast node into the sandbox
  astNode = new AstNode(true);
  m_sandbox.top().push(astNode);
		
  if ((this->*expr)()) {
    AstNode* astnode;
    char* capturedText;
    Stringstack* stack;

    capturedText = new char[m_iterator->getIndex() - savePosition + 1];
    m_iterator->getText(capturedText, savePosition, m_iterator->getIndex());
    
    // This conditional block is to save capturedText.
    // The only practical example of this is in xml parsing where backreferencing is needed.
    // an option should exist in Npeg->IsBackReferencingEnabled
    // so in small devices we don't waste memory that isn't needed for a parser implementation by user.
    stack = &m_backReferenceLookup[name];
    stack->push(capturedText);
    delete[] capturedText;

    /*
     * The AST node destruction callback will free up the memory allocated for tokens.
     */
    astnode = sandBox->top();
    sandBox->pop();
    astnode->setToken(new TokenMatch(name, savePosition, m_iterator->getIndex()));

    if (createCustomAstNode) {
      // create a custom astnode      
      uint i;
      IAstNodeReplacement *custom;
      
      if (m_astNodeFactory == NULL) {
	throw std::invalid_argument("Configuration error: This parser object has no AST node factory.");
      }

      custom = m_astNodeFactory->create(astnode);
      astnode->accept(*custom);
      custom->getChildren() = astnode->getChildren();
      for (i = 0; i < custom->getChildren().size(); i++) custom->getChildren()[i]->setParent(custom);
      
      delete astnode;
      astnode = custom;
    }    
    
    if (doReplaceBySingleChildNode) {
      if (astnode->getChildren().size() == 1) {
	AstNode* remove = astnode;
	    
	// removes ast node but not it's 1 child
	astnode = astnode->getChildren()[0];
	delete remove;
      }
    }
    
    // section decides if astnode is pushed in sandbox or to start building the tree
    if (!sandBox->empty()) {	  
      astnode->setParent(sandBox->top());
      astnode->getParent()->addChild(astnode);
    } else sandBox->push(astnode);
    
    savePosition = m_iterator->getIndex();
    result &= true;
  } else {
    AstNode *p_old_node;
    
    p_old_node = sandBox->top();
    sandBox->pop();
    
    delete p_old_node;
    
    m_iterator->setIndex(savePosition);
    result &= false;
  }
  
  return result;
}

#define _exit_restore(saved_pos) m_iterator->setIndex(saved_pos); return false

// Terminals
bool Npeg::literal(const char* matchText, const int matchTextLength, const bool isCaseSensitive) 
  throw (ParsingFatalTerminalException) {
  int i;
  char current;
  size_t saved_pos;

  saved_pos = m_iterator->getIndex();
  for(i=0; i < matchTextLength; i++) {
    current = m_iterator->getCurrent();
    if (current == -1) {
      // input not long enough to match the matchText value.
      _exit_restore(saved_pos);
    }
    
    if (isCaseSensitive) {
      if (current != matchText[i]) {
	_exit_restore(saved_pos);
      }
    } else {
      if (toupper(current) != toupper(matchText[i])) {
	_exit_restore(saved_pos);
      }
    }

    m_iterator->getNext();
  }

  return true;
}

bool Npeg::anyCharacter() throw (ParsingFatalTerminalException) {
  bool result;

  if (m_iterator->getIndex() < m_iterator->getLength()) {
    /*
     * Consume one character since we're neither at the end of a real string, 
     * nor operating on the empty string.
     */
    m_iterator->getNext();
    result = 1;
  } else result = 0;
  
  return result;
}


/*
 * Converts the ANSI string to a numerical value corresponding to the expected bit pattern.
 * Wildcards are incorporated in the mask output parameter.
 */
static int _parse_hexblock(uchar *p_mask, const char code[]) {
  int value;
  int i;

  value = 0, *p_mask = 0xff;  
  for (i = 0; i < 2; i++) {
    value = value << 4;
    if (code[i] >= '0' && code[i] <= '9') value = value + (code[i] - '0');
    else {
      char lcode;
      
      lcode = tolower(code[i]);
      if (lcode >= 'a' && lcode <= 'f') value = value + (lcode - 'a' + 10);
      else if (lcode == 'x') {
	if (i == 0) *p_mask = *p_mask & 0x0f;
	else *p_mask = *p_mask & 0xf0;
      } else return -1;
    }
  }

  assert(value < 1 << 8);

  return value;
} 

/*
 * Converts the ANSI string to a numerical value corresponding to the expected bit pattern.
 * Wildcards are incorporated in the mask output parameter.
 */
static int _parse_binblock(uchar *p_mask, const char code[]) {
  int value, i;

  value = 0, *p_mask = 0x0;
  for (i = 0; i < 8; i++) {
    value = value << 1;
    *p_mask = *p_mask << 1;
    if (code[i] == '0' || code[i] == '1') {
      value = value + (uint)(code[i] - '0');      
      *p_mask = *p_mask | 0x01;
    } else if (code[i] == 'x' || code[i] == 'X') {
      /* do nothing */
    } else {
      return -1;
    }
  }

  assert(value < 1 << 8);

  return value;
}

inline unsigned char _get_current_byte(InputIterator *iterator) {
  return (uint)iterator->getCurrent() & 0xff;
}

bool Npeg::codePoint(const char *matchcodes, const int matchlen) throw (ParsingFatalTerminalException) {
  if (*matchcodes != '#') {
    _abortError("Input string not in format \"#[x,b]<CODE><CODE>...\"");
  } else {
    const char *p_code, *p_end;
    int codelen, tmpcode, saveidx;
    uchar bitmask;

    saveidx = m_iterator->getIndex();
    p_end = matchcodes + matchlen;
    if (matchcodes[1] == 'x') {      
      /*
       * Hex
       */
      p_code = matchcodes + 2;
      codelen = matchlen - 2;

      if (codelen == 0) _abortError("Hex value specified is empty.");
      else if (codelen%2 == 1) {
	char tmpblock[2];

	/* have to pad with one leading 0 */
	tmpblock[0] = '0';
	tmpblock[1] = *p_code;
	tmpcode = _parse_hexblock(&bitmask, tmpblock);

	if (tmpcode == -1) _abortError("Hex value specified contains invalid characters.");
	else if (m_iterator->getIndex() >= m_iterator->getLength() 
		 || (bitmask & _get_current_byte(m_iterator)) != tmpcode) {
	  m_iterator->setIndex(saveidx);
	  
	  return 0;
	}

	p_code++;	
	m_iterator->getNext();
      } 

      for (; p_code < p_end; p_code += 2) {
	tmpcode = _parse_hexblock(&bitmask, p_code);

	if (tmpcode == -1) _abortError("Hex value specified contains invalid characters.");
	else if (m_iterator->getIndex() >= m_iterator->getLength() 
		   || (bitmask & _get_current_byte(m_iterator)) != tmpcode) {
	  m_iterator->setIndex(saveidx);

	  return false;
	}

	m_iterator->getNext();
      }

      return true;
    } else if (matchcodes[1] == 'b') {
      /*
       * Binary
       */
      p_code = matchcodes + 2;
      codelen = matchlen - 2;

      if (codelen == 0) _abortError("Binary value specified is empty.");
      else if (codelen%8 != 0) {
	int first_len, padlen, i;
	char tmpblock[8];

	first_len = codelen%8;
	padlen = 8 - first_len;
	for (i = 0; i < padlen; i++) tmpblock[i] = '0';
	for (i = 0; i < first_len; i++) tmpblock[i+padlen] = *(p_code + i);
	tmpcode = _parse_binblock(&bitmask, tmpblock);
	
	if (tmpcode == -1) _abortError("Binary value specified contains invalid characters.");
	else if (m_iterator->getIndex() >= m_iterator->getLength() 
		   || (bitmask & _get_current_byte(m_iterator)) != tmpcode) {
	  m_iterator->setIndex(saveidx);

	  return false;
	}

	p_code += first_len;		
	m_iterator->getNext();
      }

      for (; p_code < p_end; p_code += 8) {
	tmpcode = _parse_binblock(&bitmask, p_code);

	if (tmpcode == -1) _abortError("Binary value specified contains invalid characters.");
	else if (m_iterator->getIndex() >= m_iterator->getLength() 
		   || (bitmask & _get_current_byte(m_iterator)) != tmpcode) {
	  m_iterator->setIndex(saveidx);

	  return 0;
	}

	m_iterator->getNext();
      }

      return true;
    } else {
      double dcode;
      uint mcode, nof_inbytes, i, incode;

      /*
       * Decimal:
       * - Convert match string manually while checking for invalid input and overflow.
       * -- decide how many bytes of the input that are to be matched.
       * - Convert input string manually interpreting all consumed bytes as numerical values.
       */
      codelen = matchlen - 1;

      dcode = 0;
      for (p_code = matchcodes + 1; p_code < p_end; p_code++) {
	if (*p_code >= '0' && *p_code <= '9') {
	  dcode = dcode*10;
	  dcode = dcode + (*p_code - '0');
	} else _abortError("Decimal value specified contains invalid characters.");
      }

      if (dcode > UINT_MAX) {	
	_abortError("Decimal codepoint value exceeds 4 byte maximum length. "
	      "Largest decimal value possible 2^32 - 1");
      }
      mcode = (uint)dcode;
      
      if (mcode >> 24 != 0) nof_inbytes = 4;
      else if (mcode >> 16 != 0) nof_inbytes = 3;
      else if (mcode >> 8 != 0) nof_inbytes = 2;
      else nof_inbytes = 1;

      if ((int)nof_inbytes > (int)m_iterator->getLength() - (int)m_iterator->getIndex()) {
	return 0;
      }

      incode = 0;
      for (i = 0; i < nof_inbytes; i++) {
	incode = (incode << 8) + _get_current_byte(m_iterator);
	m_iterator->getNext();
      }

      if (incode != mcode) {
	m_iterator->setIndex(saveidx);
	
	return false;
      } else return true;
    }
  }
}


bool Npeg::characterClass(const char *characterClass, int characterClassDefinitionLength) 
  throw (ParsingFatalTerminalException) {
  bool result = false;
	
  const char* errorMessage0001 = "CharacterClass definition must be a minimum of 3 characters [expression]";
  const char* errorMessage0002 = "CharacterClass definition must start with [";
  const char* errorMessage0003 = "CharacterClass definition must end with ]";
  const char* errorMessage0004 = "CharacterClass definition requires user to escape '\\' given location in expression. User must escape by specifying '\\\\'";
  const char* errorMessage0005 = "CharacterClass definition requires user to escape '-' given location in expression. User must escape by specifying '\\-'";
  const char* errorMessage0006 = "CharacterClass definition contains an invalid escape sequence. Accepted sequences: \\\\, \\s, \\S, \\d, \\D, \\w, \\W";

  int ptrIndex = 0;

  int toBeEscaped = false;
  int isPostiveCharacterGroup = true;
		
  char current = m_iterator->getCurrent();

  if (current == -1) return false;

  if (characterClassDefinitionLength < 3) {
    _abortError(errorMessage0001);
  }

  if (characterClass[ptrIndex] != '[') {
    _abortError(errorMessage0002);
  }

  if (characterClass[characterClassDefinitionLength - 1] != ']') {
    _abortError(errorMessage0003);
  }

  ptrIndex++;
  if (characterClass[ptrIndex] == '^') {
    isPostiveCharacterGroup = false;
    ptrIndex++; 
    // is found to be negative character group.
    // will continue with positive character group logic however before returning true or false from this method will ! the result.
  }
  ptrIndex--;
  
  // Positive character group logic.
  while (++ptrIndex < characterClassDefinitionLength) {
    if(characterClass[ptrIndex] == '\\')
      {
	if(toBeEscaped==true)
	  {
	    toBeEscaped=false;
	    if( current == '\\')
	      {
		result = true;
	      }
	  }
	else
	  {
	    toBeEscaped = true;
	    if( (ptrIndex + 1) >= (characterClassDefinitionLength - 1) ) {
	      _abortError(errorMessage0004);
	    }
	  }
      }
    else if(characterClass[ptrIndex] == '-')
      {
	if(toBeEscaped == true)
	  {
	    toBeEscaped = false;
	    if( current == characterClass[ptrIndex] )
	      {
		result = true;
	      }
	  }
	else
	  {
	    if( (ptrIndex - 1) == 0 )
	      {
		_abortError(errorMessage0005);
	      }
	    else if( (ptrIndex + 1) >= (characterClassDefinitionLength - 1) )
	      {
		_abortError(errorMessage0005);
	      }
	    else
	      {
		if( current >= characterClass[ptrIndex-1] && current <= characterClass[ptrIndex+1])
		  {
		    result = true;
		  }
	      }
	  }
      }
    else 
      {
	if(toBeEscaped == true)
	  {
	      toBeEscaped = false;
	      // is it in a valid \d \D \s \S \w \W
	      // d D s S w W
	      switch(characterClass[ptrIndex])
		{
		case 'd': 
		  if( current >= '0' && current <= '9' )
		    {
		      result = true;
		    }
		  break;
		case 'D':
		  if( !(current >= '0' && current <= '9') )
		    {
		      result = true;
		    }
		  break;
		case 's':
		  if( current == ' ' || current == '\f' || current == '\n' || current == '\r' || current == '\t' || current == '\v' )
		    {
		      result = true;
		    }
		  break;
		case 'S':
		  if( !( current == ' ' || current == '\f' || current == '\n' || current == '\r' || current == '\t' || current == '\v') )
		    {
		      result = true;
		    }
		  break;
		case 'w':
		  if( 
		     ( current >= 'a' && current <= 'z' )
		     || ( current >= 'A' && current <= 'Z' )
		     || ( current >= '0' && current <= '9' ) 
		     || current == '_'  
		      )
		    {
		      result = true;
		    }
		  break;
		case 'W':
		  if( 
		     !( 
		       ( current >= 'a' && current <= 'z' )
		       || ( current >= 'A' && current <= 'Z' )
		       || ( current >= '0' && current <= '9' ) 
		       || current == '_'  
			)
		      )
		    {
		      result = true;
		    }
		  break;
		default:
		  _abortError(errorMessage0006);
		}
	    }
	  else
	    {
	      if(current == characterClass[ptrIndex])
		{
		  result = true;
		}
	    }		
	}	
    }

  if(!isPostiveCharacterGroup)
    {
      //Negative character group.
      result = (true == result)? false : true;
    }

  if(result)
    {
      m_iterator->getNext();
      // consume this character and move iterator 1 position
    }
	
  return result;
}

bool Npeg::recursionCall(IsMatchPredicate expr) throw (ParsingFatalTerminalException) {
  return (this->*expr)();
}

bool Npeg::dynamicBackReference(const char* backreferencename, int is_casesensitive) 
  throw (ParsingFatalTerminalException) {
  string tmpstore;
  const char *p_c, *matchText;
  Stringstack *backref_stack;

  if (m_backReferenceLookup.count(backreferencename) == 0) return false;
  backref_stack = &m_backReferenceLookup[backreferencename];
  if (backref_stack->empty()) return false;

  if (m_disableBackReferenceStack.empty()) {
    tmpstore = backref_stack->top();
    matchText = tmpstore.c_str();
    backref_stack->pop();
  } else matchText = backref_stack->top().c_str();
  
  for (p_c = matchText; *p_c != 0; p_c++) {
    if (is_casesensitive) {
      if (m_iterator->getCurrent() != *p_c) {
	char buffer[256];

	m_iterator->getNext();

	snprintf(buffer, 256, "DynamicBackReference: %s", matchText);
	_abortError(buffer);
      }
    } else {
      if (toupper(m_iterator->getCurrent()) != toupper(*p_c)) {
	char buffer[256];

	m_iterator->getNext();

	snprintf(buffer, 256, "DynamicBackReference: %s", matchText);
	_abortError(buffer);
      }
    }
    
    m_iterator->getNext();
  } 

  return true;
}

bool Npeg::warn(const char* message) 
{
  m_warnings.push_back(Warn(message, m_iterator->getIndex()));

  return true;
}

bool Npeg::fatal(const char* message) throw (ParsingFatalTerminalException) {
  _abortError(message);
}
