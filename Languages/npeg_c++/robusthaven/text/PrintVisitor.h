#ifndef ROBUSTHAVEN_TEXT_PRINTVISITOR_H
#define ROBUSTHAVEN_TEXT_PRINTVISITOR_H
#include <iostream>
#include "IAstNodeVisitor.h"
#include "Npeg.h"

namespace RobustHaven {
  namespace Text {
    class PrintVisitor : public IAstNodeVisitor {
    private:
      std::ostream &m_sout;

    public:
      void visitEnter(AstNode &node) {
	m_sout << "VisitEnter: " << node.getToken()->getName();
	if (node.getChildren().size() > 0) m_sout << std::endl;
	else {
	  m_sout << ": captures " << node.getToken()->getStart() << "-" 
		    << node.getToken()->getEnd() << std::endl;
	}
      }

      void visitLeave(AstNode &node) {
	m_sout << "VisitLeave: " << node.getToken()->getName() << std::endl;
      }

    public:
    PrintVisitor(std::ostream &sout) : m_sout(sout) {}

    public:
      /*
       * Prints a linear representation of the AST through sout.
       * Default for sout is std::cout
       */
      static void printAST(AstNode &root, std::ostream &sout = std::cout) {
	PrintVisitor print(sout);

	std::cout << "Start - PrintVisitor\n";
	root.accept(print);
      }
    };
  }
}
#endif
