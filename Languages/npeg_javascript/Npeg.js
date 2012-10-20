if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg.Npeg = function(iterator) {
    var m_disableBackReferenceStack = [],
        //2 dim array ('stack' of 'stacks' of AstNode elements)
        m_sandbox = [],
        // map: name => 'stack' of strings
        m_backReferenceLookup = {},
        m_warnings = [],
        m_iterator = iterator;

    var _abortError = function(msg) {
        throw new RobustHaven.Text.Npeg.ParsingFatalTerminalException(msg, m_iterator.getIndex());
    }

    var parseHexBlock = function(outmask, code) {
        var mask = 0xff,
            value = 0,
            i;

        for (i = 0; i < 2; i++) {
            value = value << 4;
            if (code[i] >= '0'.charCodeAt(0) && code[i] <= '9'.charCodeAt(0)) {
                value = value + (code[i] - '0'.charCodeAt(0));
            }
            else {
                var lcode = String.fromCharCode(code[i]).toLowerCase().charCodeAt(0);
                if (lcode >= 'a'.charCodeAt(0) && lcode <= 'f'.charCodeAt(0)) {
                    value += (lcode - 'a'.charCodeAt(0) + 10);
                } else if (lcode == 'x'.charCodeAt(0)) {
                    if (i == 0) mask = mask & 0x0f;
                    else mask = mask & 0xf0;
                } else return -1;
            }
        }

        outmask[0] = mask;
//        assert(value < 1 << 8);

        return value;
    }

    var parseBinBlock = function(outmask, code) {
        var mask = 0x00,
            value = 0, i;

        for (i = 0; i < 8; i++) {
            value = value << 1;
            mask = mask << 1;
            if (code[i] == '0'.charCodeAt(0) || code[i] == '1'.charCodeAt(0)) {
                value = value + (code[i] - '0'.charCodeAt(0));
                mask = mask | 0x01;
            } else if (code[i] == 'x'.charCodeAt(0) || code[i] == 'X'.charCodeAt(0)) {
                /* do nothing */
            } else {
                return -1;
            }
        }

        outmask[0] = mask;
//        assert(value < 1 << 8);

        return value;
    }

    var getCurrentByte = function(iterator) {
        return iterator.getCurrent().charCodeAt(0) & 0xff;
    }

    this.getAST = function(){
        if (m_sandbox.length > 0 && m_sandbox[m_sandbox.length-1].length > 0)
            return m_sandbox[m_sandbox.length-1].pop();
        else
            return null;
    };

    this.getWarnings = function(){
        return m_warnings;
    };

    this.isMatch = function isMatch(){try {} catch(ex){throw ex;}};

    this._disableBackReferencePushOnStack = function (doDisable){
        m_disableBackReferenceStack[m_disableBackReferenceStack.length] = doDisable;
    };

    this.andPredicate =  function (expr){
        var result,
            savedPosition = m_iterator.getIndex();

        this._disableBackReferencePushOnStack(true);

        result = expr.evaluate();
        m_iterator.setIndex(savedPosition);
        m_disableBackReferenceStack.pop();

        return result;
    };

    this.notPredicate =  function (expr){
        var result,
            savedPosition = m_iterator.getIndex();

        this._disableBackReferencePushOnStack(true);

        result = !expr.evaluate();
        m_iterator.setIndex(savedPosition);
        m_disableBackReferenceStack.pop();

        return result;
    };

    this.prioritizedChoice =  function (left, right){
        var savePosition;

        savePosition = m_iterator.getIndex();
        if (left.evaluate()) return true;

        m_iterator.setIndex(savePosition);
        if (right.evaluate()) return true;

        m_iterator.setIndex(savePosition);

        return false;
    };

    this.sequence =  function (left, right){
        var savePosition = m_iterator.getIndex();

        if (left.evaluate() && right.evaluate()) return true;
        else {
            m_iterator.setIndex(savePosition);
            return false;
        }
    };

    this.zeroOrMore =  function (expr){
        var savePosition = m_iterator.getIndex();

        while (expr.evaluate()) {
            if (savePosition == m_iterator.getIndex()) {
                var msg = "Infinite loop detected in limitingRepetition.\n";
                msg += "Grammar Expression: " + grammarExpression;

                throw new RobustHaven.Text.Npeg.InfiniteLoopException(msg, m_iterator.getIndex());
            }

            savePosition = m_iterator.getIndex();
        }

        m_iterator.setIndex(savePosition);

        return true;
    };

    this.oneOrMore =  function (expr, grammarExpression){
        var cnt = 0,
            savePosition = m_iterator.getIndex();

        while (expr.evaluate()) {
            if (savePosition == m_iterator.getIndex()) {
                var msg = "Infinite loop detected in limitingRepetition.\n";
                msg += "Grammar Expression: " + grammarExpression;

                throw new RobustHaven.Text.Npeg.InfiniteLoopException(msg, m_iterator.getIndex());
            }

            savePosition = m_iterator.getIndex();
            cnt++;
        }

        m_iterator.setIndex(savePosition);

        return (cnt > 0);
    };

    this.optional =  function (expr){
        var savePosition = m_iterator.getIndex();

        if (expr.evaluate()) savePosition = m_iterator.getIndex();
        else m_iterator.setIndex(savePosition);

        return true;
    };

    this.limitingRepetition =  function (min, max, expr, grammarExpression){
        var cnt = 0,
            savePosition,
            initialPosition,
            result = false;

        initialPosition = savePosition = m_iterator.getIndex();

        if (min > max && max != -1) _abortError("Repetition bounds invalid: min > max");
        if (min == -1 && max == -1) _abortError("Neither an upper nor a lower bound specified, "
            + "please use zeroOrMore");

        if (min != -1) {
            if (max == -1) {
                // has a minimum but no max cap
                savePosition = m_iterator.getIndex();
                while (expr.evaluate()) {
                    if (savePosition == m_iterator.getIndex()) {
                        var msg = "Infinite loop detected in limitingRepetition.\n";
                        msg += "Grammar Expression: " + grammarExpression;

                        throw new RobustHaven.Text.Npeg.InfiniteLoopException(msg, m_iterator.getIndex());
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
    };

    this.recursionCall =  function (expr){try {} catch(ex){throw ex;}};

    this.capturingGroup =  function(expr, name, doReplaceBySingleChildNode, allocateCustomAstNode) {
        var result = true,
            savePosition = m_iterator.getIndex(),
            sandBox = [],
            astNode;

        // load current sandbox into view.
        sandBox = (m_sandbox[m_sandbox.length-1] != undefined) ? m_sandbox[m_sandbox.length-1] : [];

        // add an empty ast node into the sandbox
        astNode = new RobustHaven.Text.Npeg.AstNode(null);
        sandBox[sandBox.length] = astNode;
        if (m_sandbox.length > 0)
            m_sandbox[m_sandbox.length-1] = sandBox;
        else
            m_sandbox[0] = sandBox;

        if (expr.evaluate()) {
            var astnode,
                capturedText,
                stack = [];

            capturedText = m_iterator.getText(savePosition, m_iterator.getIndex());

            // This conditional block is to save capturedText.
            // The only practical example of this is in xml parsing where backreferencing is needed.
            // an option should exist in Npeg.IsBackReferencingEnabled
            // so in small devices we don't waste memory that isn't needed for a parser implementation
            // by user.
            stack = m_backReferenceLookup[name];
            if (stack == undefined) {
                stack = [];
                stack[stack.length] = capturedText;
                m_backReferenceLookup[name] = stack;
            }

            /*
             * The AST node destruction callback will free up the memory allocated for tokens.
             */
            astnode = sandBox[sandBox.length-1];
            sandBox.pop();
            astnode.setToken(new RobustHaven.Text.Npeg.TokenMatch(name, savePosition, m_iterator.getIndex()));

            if (allocateCustomAstNode) {
                // create a custom astnode
                throw "@todo: Npeg.capturingGroup - create a custom astnode";
            }

            if (doReplaceBySingleChildNode) {
                if (astnode.getChildren().length == 1) {
                    // removes ast node but not it's 1 child
                    astnode = astnode.getChildren()[0];
                }
            }

            // section decides if astnode is pushed in sandbox or to start building the tree
            if (sandBox.length > 0) {
                astnode.setParent(sandBox[sandBox.length-1]);
                astnode.getParent().addChild(astnode);
            } else sandBox[sandBox.length] = astnode;

            savePosition = m_iterator.getIndex();
            result &= true;
        } else {
            sandBox.pop();
            m_iterator.setIndex(savePosition);
            result &= false;
        }

        return result;
    };

    this.codePoint =  function (matchcodes, matchlen){
        var matchcodes = (typeof matchcodes != 'object' && !(matchcodes instanceof Array)) ?
            RobustHaven.Text.Npeg.StringInputIterator.stringToBytes(''+matchcodes) :
            matchcodes;

        if (matchcodes[0] != '#'.charCodeAt(0)) {
            _abortError("Input string not in format \"#[x,b]<CODE><CODE>...\"");
        } else {
            var tmpcode, i,
                saveidx = m_iterator.getIndex(),
                bitmask = [0],
                matchblock = [];

            if (matchcodes[1] == 'x'.charCodeAt(0)) {
                /*
                 * Hex
                 */
                i = 2;
                if (i == matchlen) _abortError("Hex value specified is empty.");
                else if (matchlen%2 == 1) {
                    var tmpblock = [];

                    /* have to pad with one leading 0 */
                    tmpblock[0] = '0'.charCodeAt(0);
                    tmpblock[1] = matchcodes[i];
                    tmpcode = parseHexBlock(bitmask, tmpblock);

                    if (tmpcode == -1) _abortError("Hex value specified contains invalid byteacters.");
                    else if (m_iterator.getIndex() >= m_iterator.getLength()
                        || (bitmask[0] & getCurrentByte(m_iterator)) != tmpcode) {
                        m_iterator.setIndex(saveidx);

                        return false;
                    }

                    i++;
                    m_iterator.getNext();
                }

                for (; i < matchlen; i += 2) {
                    matchblock[0] = matchcodes[i];
                    matchblock[1] = matchcodes[i+1];
                    tmpcode = parseHexBlock(bitmask, matchblock);

                    if (tmpcode == -1) _abortError("Hex value specified contains invalid byteacters.");
                    else if (m_iterator.getIndex() >= m_iterator.getLength()
                        || (bitmask[0] & getCurrentByte(m_iterator)) != tmpcode) {
                        m_iterator.setIndex(saveidx);

                        return false;
                    }

                    m_iterator.getNext();
                }

                return true;
            } else if (matchcodes[1] == 'b'.charCodeAt(0)) {
                /*
                 * Binary
                 */
                i = 2;
                matchblock = [];

                if (i == matchlen) _abortError("Binary value specified is empty.");
                else if ((matchlen - i)%8 != 0) {
                    var first_len, padlen, j;

                    first_len = (matchlen - i)%8;
                    padlen = 8 - first_len;
                    for (j = 0; j < padlen; j++) matchblock[j] = '0'.charCodeAt(0);
                    for (j = 0; j < first_len; j++) matchblock[j+padlen] = matchcodes[i+j];
                    tmpcode = parseBinBlock(bitmask, matchblock);

                    if (tmpcode == -1) _abortError("Binary value specified contains invalid byteacters.");
                    else if (m_iterator.getIndex() >= m_iterator.getLength()
                        || (bitmask[0] & getCurrentByte(m_iterator)) != tmpcode) {
                        m_iterator.setIndex(saveidx);

                        return false;
                    }

                    i += first_len;
                    m_iterator.getNext();
                }

                for (; i < matchlen; i += 8) {
                    var j;

                    for (j = 0; j < 8; j++) matchblock[j] = matchcodes[i+j];
                    tmpcode = parseBinBlock(bitmask, matchblock);

                    if (tmpcode == -1) _abortError("Binary value specified contains invalid byteacters.");
                    else if (m_iterator.getIndex() >= m_iterator.getLength()
                        || (bitmask[0] & getCurrentByte(m_iterator)) != tmpcode) {
                        m_iterator.setIndex(saveidx);

                        return false;
                    }

                    m_iterator.getNext();
                }

                return true;
            } else {
                var dcode = 0,
                    mcode, nof_inbytes, incode,
                    i = 1;

                /*
                 * Decimal:
                 * - Convert match string manually while checking for invalid input and overflow.
                 * -- decide how many bytes of the input that are to be matched.
                 * - Convert input string manually interpreting all consumed bytes as numerical values.
                 */

                dcode = 0;
                for (; i < matchlen; i++) {
                    if (matchcodes[i] >= '0'.charCodeAt(0) && matchcodes[i] <= '9'.charCodeAt(0)) {
                        dcode = dcode*10;
                        dcode = dcode + (matchcodes[i] - '0'.charCodeAt(0));
                    } else {
                        _abortError("Decimal value specified contains invalid byteacters.");
                    }
                }

                if (dcode > Number.MAX_VALUE) {
                    _abortError("Decimal codepoint value exceeds 4 byte maximum length. "
                        + "Largest decimal value possible 2^31 - 1");
                } else if (dcode == 0) {
                    // 0 case
                    dcode = 48;
                }
                mcode = dcode;

                if (mcode >>> 24 != 0) nof_inbytes = 4;
                else if (mcode >>> 16 != 0) nof_inbytes = 3;
                else if (mcode >>> 8 != 0) nof_inbytes = 2;
                else nof_inbytes = 1;

                if (nof_inbytes > (m_iterator.getLength() - m_iterator.getIndex())) {
                    return false;
                }

                incode = 0;
                for (i = 0; i < nof_inbytes; i++) {
                    incode = (incode << 8) + getCurrentByte(m_iterator);
                    m_iterator.getNext();
                }

                if (incode != mcode) {
                    m_iterator.setIndex(saveidx);

                    return false;
                } else return true;
            }
        }
    };

    this.anyCharacter =  function (){
        var result;
        if (m_iterator.getIndex() < m_iterator.getLength()) {
            /*
             * Consume one character since we're neither at the end of a real string,
             * nor operating on the empty string.
             */
            m_iterator.getNext();
            result = true;
        } else result = false;

        return result;
    };

    this.characterClass =  function (characterClass, classLength){
        var result = false,
            ptrIndex = 0,
            toBeEscaped = false,
            current = m_iterator.getCurrent(),
            isPostiveCharacterGroup = true,
            errorMessage0001 = new String("CharacterClass definition must be a minimum of 3 characters [expression]"),
            errorMessage0002 = new String("CharacterClass definition must start with ["),
            errorMessage0003 = new String("CharacterClass definition must end with ]"),
            errorMessage0004 = new String("CharacterClass definition requires user to escape '\\' given location in expression. User must escape by specifying '\\\\'"),
            errorMessage0005 = new String("CharacterClass definition requires user to escape '-' given location in expression. User must escape by specifying '\\-'"),
            errorMessage0006 = new String("CharacterClass definition contains an invalid escape sequence. Accepted sequences: \\\\, \\s, \\S, \\d, \\D, \\w, \\W");

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
            if(characterClass[ptrIndex] == '\\') {
                if(toBeEscaped==true) {
                    toBeEscaped=false;
                    if( current == '\\') {
                        result = true;
                    }
                }  else {
                    toBeEscaped = true;
                    if( (ptrIndex + 1) >= (classLength - 1) ) {
                        _abortError(errorMessage0004);
                    }
                }
            }
            else if(characterClass[ptrIndex] == '-') {
                if(toBeEscaped == true) {
                    toBeEscaped = false;
                    if( current == characterClass[ptrIndex] )
                    {
                        result = true;
                    }
                }  else {
                    if( (ptrIndex - 1) == 0 )  {
                        _abortError(errorMessage0005);
                    }
                    else if( (ptrIndex + 1) >= (classLength - 1) ) {
                        _abortError(errorMessage0005);
                    }  else  {
                        if( current >= characterClass[ptrIndex-1] && current <= characterClass[ptrIndex+1])  {
                            result = true;
                        }
                    }
                }
            }  else  {
                if (toBeEscaped == true)  {
                    toBeEscaped = false;
                    // is it in a valid \d \D \s \S \w \W
                    // d D s S w W
                    switch(characterClass[ptrIndex])  {
                        case 'd':
                            if( current >= '0' && current <= '9' ) {
                                result = true;
                            }
                            break;
                        case 'D':
                            if( !(current >= '0' && current <= '9') )  {
                                result = true;
                            }
                            break;
                        case 's':
                            if(current == ' ' || current == '\f' || current == '\n'
                                || current == '\r' || current == '\t'
                             ) {
                                result = true;
                            }
                            break;
                        case 'S':
                            if( !( current == ' ' || current == '\f' ||
                                current == '\n' || current == '\r' || current == '\t')
                                ) {
                                result = true;
                            }
                            break;
                        case 'w':
                            if((current >= 'a' && current <= 'z' )
                                || ( current >= 'A' && current <= 'Z' )
                                || ( current >= '0' && current <= '9' )
                                || current == '_'
                                ) {
                                result = true;
                            }
                            break;
                        case 'W':
                            if(!(( current >= 'a' && current <= 'z' )
                                || ( current >= 'A' && current <= 'Z' )
                                || ( current >= '0' && current <= '9' )
                                || current == '_'
                                )
                                ) {
                                result = true;
                            }
                            break;
                        default:
                            _abortError(errorMessage0006);
                    }
                }  else {
                    if(current == characterClass[ptrIndex]) {
                        result = true;
                    }
                }
            }
        }

        if(!isPostiveCharacterGroup) {
            //Negative character group.
            result = (true == result)? false : true;
        }

        if(result) {
            m_iterator.getNext();
            // consume this character and move iterator 1 position
        }

        return result;
    };

    this.literal =  function (matchText, matchTextLength, isCaseSensitive){
         var saved_pos = m_iterator.getIndex();

         for (var i = 0; i < matchTextLength; i++) {
             var current = m_iterator.getCurrent();

             if (current == -1) {
                // input not long enough to match the matchText value.
                 m_iterator.setIndex(saved_pos); return false;
             }

             if (isCaseSensitive) {
                 if (current != matchText[i]) {
                     m_iterator.setIndex(saved_pos);
                     return false;
                 }
             } else {
                 if (current.toUpperCase() != matchText[i].toUpperCase()) {
                     m_iterator.setIndex(saved_pos);
                     return false;
                 }
             }

             m_iterator.getNext();
         }

        return true;
    };

    this.dynamicBackReference =  function (x,y){try {} catch(ex){throw ex;}};

    this.fatal =  function (error){
        return _abortError(error);
        return false;
    };

    this.warn =  function (msg){
        m_warnings[m_warnings.length] = new RobustHaven.Text.Npeg.Warn(msg, m_iterator.getIndex());

        return true;
    };
}