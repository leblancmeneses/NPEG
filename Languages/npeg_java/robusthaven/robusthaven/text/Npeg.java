package robusthaven.text;
import java.util.Stack;
import java.util.HashMap;
import java.util.Vector;
import java.io.IOException;

public abstract class Npeg {
    /*
     * Force client modules to define their MatchPredicates inside the scope
     * of the Npeg descendant class.
     */
    protected interface IsMatchPredicate {
	boolean evaluate() throws ParsingFatalTerminalException, InfiniteLoopException, IOException;
    }

    Stack<Boolean> m_disableBackReferenceStack;
    Stack<Stack<AstNode>> m_sandbox;
    HashMap<String, Stack<String>> m_backReferenceLookup;
    Vector<Warn> m_warnings;
    InputIterator m_iterator;
    ICreateCustomAstNode m_astNodeFactory;
    
    public Npeg(InputIterator iterator, ICreateCustomAstNode astNodeFactory) {
	m_iterator = iterator;
	m_warnings = new Vector<Warn>(10, 10);
	m_disableBackReferenceStack = new Stack<Boolean>();
	m_backReferenceLookup = new HashMap<String, Stack<String>>();
	m_sandbox = new Stack<Stack<AstNode>>();
	m_sandbox.push(new Stack<AstNode>());
	m_astNodeFactory = astNodeFactory;
    }

    public Vector<Warn> getWarnings() {
	return m_warnings;
    }

    public InputIterator getInputIterator() {
	return m_iterator;
    }

    public AstNode getAST() {
	if (!m_sandbox.empty() && !m_sandbox.peek().empty()) return m_sandbox.peek().pop();
	else return null;
    }

    private void _disableBackReferencePushOnStack(boolean doDisable) {
	m_disableBackReferenceStack.push(new Boolean(doDisable));
    }

    private void _abortError(String message) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	throw new ParsingFatalTerminalException(message, m_iterator.getIndex());
    }

    /*
     * Non-Terminals
     */
    /*
     * The basic evaluation routine that has to provided by all descendant classes
     */
    public abstract boolean isMatch() throws IOException, ParsingFatalTerminalException;    

    protected boolean andPredicate(IsMatchPredicate expr) 
	throws IOException, ParsingFatalTerminalException {
	boolean result;
	long savePosition = m_iterator.getIndex();

	_disableBackReferencePushOnStack(true);

	result = expr.evaluate();
	m_iterator.setIndex(savePosition);

	m_disableBackReferenceStack.pop();

	return result;
    }

    protected boolean notPredicate(IsMatchPredicate expr) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	boolean result;
	long savePosition = m_iterator.getIndex();

	_disableBackReferencePushOnStack(true);
	
	result = !expr.evaluate();
	m_iterator.setIndex(savePosition);
	
	m_disableBackReferenceStack.pop();
	
	return result;
    }

    protected boolean prioritizedChoice(IsMatchPredicate left, IsMatchPredicate right)
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	long savePosition;

	savePosition = m_iterator.getIndex();
	if (left.evaluate()) return true;

	m_iterator.setIndex(savePosition);
	if (right.evaluate()) return true;

	m_iterator.setIndex(savePosition);

	return false;
    }

    protected boolean recursionCall(IsMatchPredicate expr) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	return expr.evaluate();
    }

    protected boolean sequence(IsMatchPredicate left, IsMatchPredicate right) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	long savePosition = m_iterator.getIndex();

	if (left.evaluate() && right.evaluate()) return true;
	else {
	    m_iterator.setIndex(savePosition);
	    return false;
	}
    }

    protected boolean zeroOrMore(IsMatchPredicate expr, String grammarExpression) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	  long savePosition = m_iterator.getIndex();

	  while (expr.evaluate()) {
	      if (savePosition == m_iterator.getIndex()) {
		  throw new InfiniteLoopException("Infinite loop detected in zeroOrMore.\n"
						  + "Grammar Expression: " + grammarExpression, 
						  m_iterator.getIndex());
	      }
	      savePosition = m_iterator.getIndex();
	  }

	  m_iterator.setIndex(savePosition);
	
	  return true;
    }

    protected boolean oneOrMore(IsMatchPredicate expr, String grammarExpression) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	  int cnt = 0;
	  long savePosition = m_iterator.getIndex();

	  while (expr.evaluate()) {
	      if (savePosition == m_iterator.getIndex()) {
		  throw new InfiniteLoopException("Infinite loop detected in oneOrMore.\n"
						  + "Grammar Expression: " + grammarExpression, 
						  m_iterator.getIndex());
	      }
	      savePosition = m_iterator.getIndex();
	      cnt++;
	  }

	  m_iterator.setIndex(savePosition);
	  
	  return (cnt > 0);
    }

    protected boolean optional(IsMatchPredicate expr) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	long savePosition = m_iterator.getIndex();

	if (expr.evaluate()) savePosition = m_iterator.getIndex();
	else m_iterator.setIndex(savePosition);

	return true;
    }

    protected boolean limitingRepetition(int min, int max, IsMatchPredicate expr, String grammarExpression) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	int cnt = 0;
	long savePosition;
	long initialPosition;
	boolean result = false;

	initialPosition = (savePosition = m_iterator.getIndex());

	if (min > max && max != -1) _abortError("Repetition bounds invalid: min > max");
	if (min == -1 && max == -1) _abortError("Neither an upper nor a lower bound specified, "
						+ "please use zeroOrMore");				

	if (min != -1) {
	    if (max == -1) {
		// has a minimum but no max cap
		savePosition = m_iterator.getIndex();
		while (expr.evaluate()) {
		    if (savePosition == m_iterator.getIndex()) {
			throw new InfiniteLoopException("Infinite loop detected in limitingRepetition.\n"
							+ "Grammar Expression: " + grammarExpression, 
							m_iterator.getIndex());
		    }
		    cnt++;
		    savePosition = m_iterator.getIndex();
		}

		m_iterator.setIndex(savePosition);
		result = (cnt >= min);
	    } else {
		// has a minimum and a max specified
		savePosition = m_iterator.getIndex();
		while (expr.evaluate()) {
		    cnt++;
		    savePosition = m_iterator.getIndex();

		    if (cnt >= max) break;
		}

		m_iterator.setIndex(savePosition);
		result = (cnt <= max && cnt >= min);
	    }
	} else {
	    // zero or up to a max matches of e.
	    savePosition = m_iterator.getIndex();
	    while (expr.evaluate()) {
		cnt++;
		savePosition = m_iterator.getIndex();

		if (cnt >= max) break;
	    }

	    m_iterator.setIndex(savePosition);
	    result = (cnt <= max);
	}

	if (result == false) m_iterator.setIndex(initialPosition);

	return result;
    }

    protected boolean capturingGroup(IsMatchPredicate expr, String name, 
				     boolean doReplaceBySingleChildNode, 
				     boolean allocateCustomAstNode)
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	boolean result = true;
	long savePosition = m_iterator.getIndex();
	Stack<AstNode> sandBox;
	AstNode astNode;

	// load current sandbox into view.
	sandBox = m_sandbox.peek();
	
	// add an empty ast node into the sandbox
	astNode = new AstNode(null);
	m_sandbox.peek().push(astNode);
		
	if (expr.evaluate()) {
	    AstNode astnode;
	    byte[] capturedText;
	    Stack<String> stack;

	    capturedText = new byte[(int)(m_iterator.getIndex()-savePosition+1)];
	    m_iterator.getText(capturedText, (int)savePosition, (int)m_iterator.getIndex());
    
	    // This conditional block is to save capturedText.
	    // The only practical example of this is in xml parsing where backreferencing is needed.
	    // an option should exist in Npeg.IsBackReferencingEnabled
	    // so in small devices we don't waste memory that isn't needed for a parser implementation 
	    // by user.
	    stack = m_backReferenceLookup.get(name);
	    if (stack == null) {
		stack = new Stack<String>();
		m_backReferenceLookup.put(name, stack);
	    }
	    stack.push(new String(capturedText));

	    /*
	     * The AST node destruction callback will free up the memory allocated for tokens.
	     */
	    astnode = sandBox.peek();
	    sandBox.pop();
	    astnode.setToken(new TokenMatch(name, savePosition, m_iterator.getIndex()));

	    if (allocateCustomAstNode) {
		// create a custom astnode      
		int i;
		IAstNodeReplacement custom;
      
		if (m_astNodeFactory == null) {
		    throw new NullPointerException("Configuration error: This parser object has no "
						   + "AST node factory.");
		}

		custom = m_astNodeFactory.create(astnode);
		astnode.accept(custom);
		for (i = 0; i < astnode.nofChildren(); i++) {
		    astnode.addChild(astnode.getChildren()[i]);
		    custom.getChildren()[i].setParent(custom);
		}
      
		astnode = custom;
	    }    
    
	    if (doReplaceBySingleChildNode) {
		if (astnode.nofChildren() == 1) {
		    // removes ast node but not it's 1 child
		    astnode = astnode.getChildren()[0];
		}
	    }
    
	    // section decides if astnode is pushed in sandbox or to start building the tree
	    if (!sandBox.empty()) {	  
		astnode.setParent(sandBox.peek());
		astnode.getParent().addChild(astnode);
	    } else sandBox.push(astnode);
    
	    savePosition = m_iterator.getIndex();
	    result &= true;
	} else {
	    sandBox.pop();
    
	    m_iterator.setIndex(savePosition);
	    result &= false;
	}
  
	return result;
    }

    /*
     * Terminals
     */

    /*
     * Converts the ANSI string to a numerical value corresponding to the expected bit pattern.
     * Wildcards are incorporated in the mask output parameter.
     */
    private static int _parse_hexblock(int[] outmask, byte code[]) {
	int mask;
	int value;
	int i;

	value = 0; mask = (byte)0xff;  
	for (i = 0; i < 2; i++) {
	    value = value << 4;
	    if (code[i] >= '0' && code[i] <= '9') value = value + (code[i] - '0');
	    else {
		byte lcode;
      
		lcode = (byte)Character.toLowerCase((char)code[i]);
		if (lcode >= 'a' && lcode <= 'f') value = value + (lcode - 'a' + 10);
		else if (lcode == 'x') {
		    if (i == 0) mask = mask & 0x0f;
		    else mask = mask & 0xf0;
		} else return -1;
	    }
	}

	outmask[0] = mask;
	assert(value < 1 << 8);

	return value;
    } 

    /*
     * Converts the ANSI string to a numerical value corresponding to the expected bit pattern.
     * Wildcards are incorporated in the mask output parameter.
     */
    private static int _parse_binblock(int[] outmask, byte code[]) {
	int mask;
	int value, i;

	value = 0; mask = 0x0;
	for (i = 0; i < 8; i++) {
	    value = value << 1;
	    mask = mask << 1;
	    if (code[i] == (byte)'0' || code[i] == (byte)'1') {
		value = value + (int)(code[i] - (byte)'0');      
		mask = mask | 0x01;
	    } else if (code[i] == 'x' || code[i] == 'X') {
		/* do nothing */
	    } else {
		return -1;
	    }
	}

	outmask[0] = mask;
	assert(value < 1 << 8);

	return value;
    }

    private static int _get_current_byte(InputIterator iterator) throws IOException {
	return iterator.getCurrent() & 0xff;
    }

    protected boolean codePoint(byte[] matchcodes, int matchlen) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	byte[] matchblock;

	if (matchcodes[0] != '#') {
	    _abortError("Input string not in format \"#[x,b]<CODE><CODE>...\"");
	} else {
	    int tmpcode, i;
	    long saveidx;
	    int[] bitmask;

	    bitmask = new int[1];
	    saveidx = m_iterator.getIndex();
	    matchblock = new byte[2];
	    if (matchcodes[1] == 'x') {      
		/*
		 * Hex
		 */
		i = 2;
		if (i == matchlen) _abortError("Hex value specified is empty.");
		else if (matchlen%2 == 1) {
		    byte[] tmpblock = new byte[2];

		    /* have to pad with one leading 0 */
		    tmpblock[0] = '0';
		    tmpblock[1] = matchcodes[i];
		    tmpcode = _parse_hexblock(bitmask, tmpblock);

		    if (tmpcode == -1) _abortError("Hex value specified contains invalid byteacters.");
		    else if (m_iterator.getIndex() >= m_iterator.getLength() 
			     || (bitmask[0] & _get_current_byte(m_iterator)) != tmpcode) {
			m_iterator.setIndex(saveidx);
	  
			return false;
		    }

		    i++;	
		    m_iterator.getNext();
		} 

		for (; i < matchlen; i += 2) {
		    matchblock[0] = matchcodes[i];
		    matchblock[1] = matchcodes[i+1];
		    tmpcode = _parse_hexblock(bitmask, matchblock);

		    if (tmpcode == -1) _abortError("Hex value specified contains invalid byteacters.");
		    else if (m_iterator.getIndex() >= m_iterator.getLength() 
			     || (bitmask[0] & _get_current_byte(m_iterator)) != tmpcode) {
			m_iterator.setIndex(saveidx);

			return false;
		    }

		    m_iterator.getNext();
		}

		return true;
	    } else if (matchcodes[1] == 'b') {
		/*
		 * Binary
		 */
		i = 2;
		matchblock = new byte[8];

		if (i == matchlen) _abortError("Binary value specified is empty.");
		else if ((matchlen - i)%8 != 0) {
		    int first_len, padlen, j;

		    first_len = (matchlen - i)%8;
		    padlen = 8 - first_len;
		    for (j = 0; j < padlen; j++) matchblock[j] = '0';
		    for (j = 0; j < first_len; j++) matchblock[j+padlen] = matchcodes[i+j];
		    tmpcode = _parse_binblock(bitmask, matchblock);
	
		    if (tmpcode == -1) _abortError("Binary value specified contains invalid byteacters.");
		    else if (m_iterator.getIndex() >= m_iterator.getLength() 
			     || (bitmask[0] & _get_current_byte(m_iterator)) != tmpcode) {
			m_iterator.setIndex(saveidx);

			return false;
		    }

		    i += first_len;		
		    m_iterator.getNext();
		}

		for (; i < matchlen; i += 8) {
		    int j;

		    for (j = 0; j < 8; j++) matchblock[j] = matchcodes[i+j];
		    tmpcode = _parse_binblock(bitmask, matchblock);

		    if (tmpcode == -1) _abortError("Binary value specified contains invalid byteacters.");
		    else if (m_iterator.getIndex() >= m_iterator.getLength() 
			     || (bitmask[0] & _get_current_byte(m_iterator)) != tmpcode) {
			m_iterator.setIndex(saveidx);

			return false;
		    }

		    m_iterator.getNext();
		}

		return true;
	    } else {
		double dcode;
		int mcode, nof_inbytes, incode;

		/*
		 * Decimal:
		 * - Convert match string manually while checking for invalid input and overflow.
		 * -- decide how many bytes of the input that are to be matched.
		 * - Convert input string manually interpreting all consumed bytes as numerical values.
		 */
		i = 1;

		dcode = 0;
		for (; i < matchlen; i++) {
		    if (matchcodes[i] >= '0' && matchcodes[i] <= '9') {
			dcode = dcode*10;
			dcode = dcode + (matchcodes[i] - '0');
		    } else {
			_abortError("Decimal value specified contains invalid byteacters.");
		    }
		}

		if (dcode > Integer.MAX_VALUE) {	
		    _abortError("Decimal codepoint value exceeds 4 byte maximum length. "
				+ "Largest decimal value possible 2^31 - 1");
		}
		mcode = (int)dcode;
      
		if (mcode >>> 24 != 0) nof_inbytes = 4;
		else if (mcode >>> 16 != 0) nof_inbytes = 3;
		else if (mcode >>> 8 != 0) nof_inbytes = 2;
		else nof_inbytes = 1;

		if ((int)nof_inbytes > (int)m_iterator.getLength() - (int)m_iterator.getIndex()) {
		    return false;
		}

		incode = 0;
		for (i = 0; i < nof_inbytes; i++) {
		    incode = (incode << 8) + _get_current_byte(m_iterator);
		    m_iterator.getNext();
		}

		if (incode != mcode) {
		    m_iterator.setIndex(saveidx);
	
		    return false;
		} else return true;
	    }
	}
	
	return false;
    }    

    protected boolean anyCharacter() 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	boolean result;

	if (m_iterator.getIndex() < m_iterator.getLength()) {
	    /*
	     * Consume one character since we're neither at the end of a real string, 
	     * nor operating on the empty string.
	     */
	    m_iterator.getNext();
	    result = true;
	} else result = false;
  
	return result;
    }

    protected boolean characterClass(byte[] characterClass, int classLength) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	boolean result = false;
	
	final String errorMessage0001 = new String("CharacterClass definition must be a minimum of 3 characters [expression]");
	final String errorMessage0002 = new String("CharacterClass definition must start with [");
	final String errorMessage0003 = new String("CharacterClass definition must end with ]");
	final String errorMessage0004 = new String("CharacterClass definition requires user to escape '\\' given location in expression. User must escape by specifying '\\\\'");
	final String errorMessage0005 = new String("CharacterClass definition requires user to escape '-' given location in expression. User must escape by specifying '\\-'");
	final String errorMessage0006 = new String("CharacterClass definition contains an invalid escape sequence. Accepted sequences: \\\\, \\s, \\S, \\d, \\D, \\w, \\W");

	int ptrIndex = 0;

	boolean toBeEscaped = false;
	boolean isPostiveCharacterGroup = true;
		
	int current = m_iterator.getCurrent();

	if (current == -1) return false;

	if (classLength < 3) {
	    _abortError(errorMessage0001);
	}

	if (characterClass[ptrIndex] != '[') {
	    _abortError(errorMessage0002);
	}

	if (characterClass[classLength - 1] != ']') {
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
	while (++ptrIndex < classLength) {
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
			    if( (ptrIndex + 1) >= (classLength - 1) ) {
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
			    else if( (ptrIndex + 1) >= (classLength - 1) )
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
				    if( current == ' ' || current == '\f' || current == '\n' || current == '\r' || current == '\t' )
					{
					    result = true;
					}
				    break;
				case 'S':
				    if( !( current == ' ' || current == '\f' || current == '\n' || current == '\r' || current == '\t') )
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
		m_iterator.getNext();
		// consume this character and move iterator 1 position
	    }
	
	return result;	
    }

    protected boolean literal(byte[] matchText, int matchTextLength, boolean isCaseSensitive) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	int i;
	int current;
	long saved_pos;

	saved_pos = m_iterator.getIndex();
	for(i=0; i < matchTextLength; i++) {
	    current = m_iterator.getCurrent();
	    if (current == -1) {
		// input not long enough to match the matchText value.
		m_iterator.setIndex(saved_pos); return false;
	    }
    
	    if (isCaseSensitive) {
		if (current != matchText[i]) {
		    m_iterator.setIndex(saved_pos); return false;
		}
	    } else {
		if (Character.toUpperCase((char)current) != Character.toUpperCase((char)matchText[i])) {
		    m_iterator.setIndex(saved_pos); return false;
		}
	    }

	    m_iterator.getNext();
	}

	return true;
    }

    protected boolean dynamicBackReference(String backreferencename, boolean isCaseSensitive) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	String tmpstore;
	Stack<String> backref_stack;
	byte[] matchText;
	int i;

	backref_stack = m_backReferenceLookup.get(backreferencename);
	if (backref_stack == null || backref_stack.empty()) return false;

	tmpstore = backref_stack.peek();
	matchText = new byte[tmpstore.length()+1];
	matchText = tmpstore.getBytes("UTF-8");
	if (m_disableBackReferenceStack.empty()) {
	    backref_stack.pop();	    
	} 
  
	for (i = 0; matchText[i] != 0; i++) {
	    if (isCaseSensitive) {
		if ((byte)m_iterator.getCurrent() != matchText[i]) {
		    m_iterator.getNext();
		    
		    _abortError("DynamicBackReference: " + new String(matchText));
		}
	    } else {
		if (Character.toUpperCase((char)m_iterator.getCurrent()) 
		    != Character.toUpperCase((char)matchText[i])) {
		    m_iterator.getNext();
		    
		    _abortError("DynamicBackReference: " + new String(matchText));
		}
	    }
    
	    m_iterator.getNext();
	} 

	return true;
    }

    protected boolean fatal(String message) 
	throws ParsingFatalTerminalException, InfiniteLoopException, IOException {
	_abortError(message);

	return false;
    }

    protected boolean warn(String message) {
	m_warnings.add(new Warn(message, m_iterator.getIndex()));

	return true;
    }
}