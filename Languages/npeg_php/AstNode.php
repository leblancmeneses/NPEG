<?
namespace RobustHaven\Text\Npeg;

class AstNode 
{
	private $parent;
	private $children;
	private $token;

	function __construct() 
	{
       $this->children = array();
    }
    
	public function setParent( $value )
	{
		$this->parent = $value;
	}

	public function getParent()
	{
		return $this->parent;
	}

	public function setChildren( $value )
	{
		$this->children = $value;
	}

	public function getChildren()
	{
		return $this->children;
	}
	
	public function setToken( $value )
	{
		$this->token = $value;
	}

	public function getToken()
	{
		return $this->token;
	}
	
	public function Accept( $visitor)
    {
    	$visitor->VisitEnter($this);
		$isFirstTime = true;
        foreach ($this->Children as $node)
        {
        	if (!$isFirstTime)
            {
            	$visitor->VisitExecute($this);
            }
            $isFirstTime = false;
            $node->Accept($visitor);
        }
        $visitor->VisitLeave($this);
	}
}
?>
