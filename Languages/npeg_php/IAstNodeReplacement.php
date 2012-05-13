<?
namespace RobustHaven\Text\Npeg;

abstract class IAstNodeReplacement extends AstNode implements IAstNodeVisitor
{
	public abstract function VisitEnter($node);
	public abstract function VisitExecute($node);
	public abstract function VisitLeave($node);
}

?>
