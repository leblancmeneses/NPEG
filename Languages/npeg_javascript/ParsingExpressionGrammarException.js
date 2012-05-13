if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{
	ParsingExpressionGrammarException : function(message)
	{
		//Base.prototype.constructor.call(this, message);
	}
	
	//ParsingExpressionGrammarException.prototype = new Base;	
	ParsingExpressionGrammarException.prototype.constructor = ParsingExpressionGrammarException;	
}