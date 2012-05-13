/*
include <stack>
include <string>
include <map>
include "Warn.h"
include "TokenMatch.h"
include "AstNode.h"
include "InputIterator.h"
include "Exceptions.h"
include <ext/hash_map>
*/

if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{
	Npeg : function(iterator)
	{
		var $m_disableBackReferenceStack;
	    var $m_sandbox;
	    var $m_backReferenceLookup;
	    var $m_warnings; 
	    var $m_iterator;	    
		
	    this.getAST = function(){};
		this.getWarnings = function(){};
	    
		this.isMatch = function isMatch(){try {} catch(ex){throw ex;}};
		this._disableBackReferencePushOnStack = function (doDisablea){};
		
	    this.andPredicate =  function (expr){try {} catch(ex){throw ex;}};
	    this.notPredicate =  function (expr){try {} catch(ex){throw ex;}};
	    this.prioritizedChoice =  function (left,right){try {} catch(ex){throw ex;}};
	    this.sequence =  function (left,right){try {} catch(ex){throw ex;}};
	    this.zeroOrMore =  function (expr){try {} catch(ex){throw ex;}};
	    this.oneOrMore =  function (expr){try {} catch(ex){throw ex;}};
	    this.optional =  function (expr){try {} catch(ex){throw ex;}};
	    this.limitingRepetition =  function (x,y,expr){try {} catch(ex){throw ex;}};
	    this.recursionCall =  function (expr){try {} catch(ex){throw ex;}};
	    this.capturingGroup =  function (expr,x,y,z=null){try {} catch(ex){throw ex;}};
	
	    this.codePoint =  function (x,y){try {} catch(ex){throw ex;}};
	    this.anyCharacter =  function (){try {} catch(ex){throw ex;}};
	    this.characterClass =  function (x,y){try {} catch(ex){throw ex;}};
	    this.literal =  function (x,y,z){try {} catch(ex){throw ex;}};
	    this.dynamicBackReference =  function (x,y){try {} catch(ex){throw ex;}};
	    this.fatal =  function (error){try {} catch(ex){throw ex;}};
	    this.warn =  function (x){};
	}
}