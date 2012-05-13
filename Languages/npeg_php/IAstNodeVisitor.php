<?
namespace RobustHaven\Text\Npeg;

interface IAstNodeVisitor
{
	public function VisitEnter($node);
	public function VisitExecute($node);
	public function VisitLeave($node);
}


?>