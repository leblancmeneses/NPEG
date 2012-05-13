if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{
	ParsingFatalTerminalException : function(message)
	{
		if (typeof message == "undefined") 
			message = "";
		ParsingExpressionGrammarException.prototype.constructor.call(this, message);

	}
	ParsingFatalTerminalException.prototype = new ParsingExpressionGrammarException;
	ParsingFatalTerminalException.prototype.constructor = ParsingFatalTerminalException;
}