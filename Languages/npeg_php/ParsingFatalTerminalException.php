<?
namespace RobustHaven\Text\Npeg;

class ParsingFatalTerminalException extends ParsingExpressionGrammarException
{
	public function __construct($message)
	{
		parent::__construct($message);       
    }        
}

?>