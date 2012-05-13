<?
namespace RobustHaven\Text\Npeg;

class InfiniteLoopDetectedException extends ParsingExpressionGrammarException
{
	public function __construct()
	{
        $num = func_num_args();
        $args = func_get_args();
        switch($num)
        {
	        case 0:
	            $this->__call('__construct0', null);
	            break;
	        case 1:
	            $this->__call('__construct1', $args);
	            break;        
	        default:
	            throw new Exception();
        }
    }
    
    public function __construct0()
    {
        parent::__construct('Supplied grammar rules caused an infinite loop.');
    }

    public function __construct1($message)
    {
        parent::__construct($message);
    }
}

?>