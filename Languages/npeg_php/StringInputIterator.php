<?
namespace RobustHaven\Text\Npeg;

class StringInputIterator extends InputIterator
{
	private $input;
	
	public function __construct($input)
	{
		parent::__construct(count($input));
		$this->input = $input;
	}
		public abstract function Text($start, $end);

	public function Text($start, $end)
	{
		$n = $end - $start;
		$bytes = array();
		for($i = 0; $i < $n; $i++)
		{
			$bytes[$i] = $this->input[$start+$i];
		}
		return $bytes;
	}
	
	public function Current()
	{
		 if ($this->index >= $this->length) return -1;
            else 
            {
                return $this->input[$this->index];
            }
	}
	
	public  function Next()
	{
		if ($this->index >= $this->length) return -1;
        else 
        {
        	$this->index += 1;
            if ($this->index >= $this->length) return -1;
            else return $this->input[$this->index];
         }
	}
	
	public  function Previous()
	{
		if ($this->index <= 0)
        {
        	return -1;
        }
        else 
        {
        	if($this->Length == 0)throw new Exception();
            $this->index -= 1;
            return $this->input[$this->index];
         }	
	}
}

?>