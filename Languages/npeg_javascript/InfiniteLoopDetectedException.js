if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{
	InfiniteLoopDetectedException : function(message)
	{
		if (typeof message == "undefined") 
			message = "Supplied grammar rules caused an infinite loop.";
		ParsingExpressionGrammarException.prototype.constructor.call(this, message);

	}
	InfiniteLoopDetectedException.prototype = new ParsingExpressionGrammarException;
	InfiniteLoopDetectedException.prototype.constructor = InfiniteLoopDetectedException;
}