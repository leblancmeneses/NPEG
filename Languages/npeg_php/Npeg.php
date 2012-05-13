<?
namespace RobustHaven\Text\Npeg;

class Npeg
{	
	private $m_disableBackReferenceStack;
    private $m_sandbox;
    private $m_backReferenceLookup;
    private $m_warnings; 
    private $m_iterator;
	
	public function __construct($iterator)	{ }
	public function __destruct() { 	}
	public function getAST(){}
	public function getWarnings(){}	

    public function isMatch(){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}

    private function _disableBackReferencePushOnStack($doDisablea){}

    protected function andPredicate($expr){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function notPredicate($expr){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function prioritizedChoice($left,$right){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function sequence($left,$right){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function zeroOrMore($expr){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function oneOrMore($expr){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function optional($expr){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function limitingRepetition($x,$y,$expr){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function recursionCall($expr){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function capturingGroup($expr,$x,$y,$z=null){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}

    protected function codePoint($x,$y){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function anyCharacter(){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function characterClass($x,$y){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function literal($x,$y,$z){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function dynamicBackReference($x,$y){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function fatal($error){try {} catch(ParsingFatalTerminalException $ex){throw $ex;}}
    protected function warn($x){}
}

?>