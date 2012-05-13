<?
namespace RobustHaven\Text\Npeg;

class TokenMatch 
{
	private $name;
	private $start;
	private $end;
	
	function __construct($name,$start,$end) 
	{
       $this->name = $name;
       $this->start = $start;
       $this->end = $end;
    }
   
	public function setName( $value )
	{
		$this->name = $value;
	}

	public function getName()
	{
		return $this->name;
	}

	public function setStart( $value )
	{
		$this->start = $value;
	}

	public function getStart()
	{
		return $this->start;
	}
	
	public function setEnd( $value )
	{
		$this->end = $value;
	}

	public function getEnd()
	{
		return $this->end;
	}
}

?>
