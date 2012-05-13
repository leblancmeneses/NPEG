<?
namespace RobustHaven\Text\Npeg;

class Warn 
{
	private $message;
	private $position;	
	
	function __construct() 
	{
		$num = func_num_args();
        $args = func_get_args();
        switch($num)
        {
	        case 0:
	            $this->__call('__construct0', null);
	            break;
	        case 2:
	            $this->__call('__construct2', $args);
	            break;        
	        default:
	            throw new Exception();
        }
    }
   
    public function __construct0()
    {
        $this->message = "";
    }

    public function __construct2($message,$position)
    {
        $this->message = $message;
        $this->position = $position;
    }
    
	public function setMessage( $value )
	{
		$this->message = $value;
	}

	public function getMessage()
	{
		return $this->message;
	}

	public function setPosition( $value )
	{
		$this->position = $value;
	}

	public function getPosition()
	{
		return $this->position;
	}
}


?>