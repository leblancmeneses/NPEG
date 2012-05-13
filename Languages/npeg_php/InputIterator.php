<?
namespace RobustHaven\Text\Npeg;

abstract class InputIterator
{
	private $index;
	private $length;
	
	public function __construct($length)
	{
		$this->index = 0;
		$this->length = $length;
	}
	
	public function setIndex( $value )
	{
		$this->index = $value;
	}

	public function getIndex()
	{
		return $this->index;
	}
	
	public function getLength()
	{
		return $this->length;
	}
	
	public abstract function Text($start, $end);

	public abstract function Current();
	public abstract function Next();
	public abstract function Previous();
	
}

?>