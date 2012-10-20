#NPEG - .NET Parsing Expression Grammar.  

*	The framework can output equivalent parsers natively in C/C++/C#/Java/PHP/JavaScript. http://www.robusthaven.com/blog/parsing-expression-grammar/language-workbench
*	The framework can read and write it's own DSL.  See samples of the DSL below.


## PEG - Boolean Algebra in CSharp
<code>

    		String grammar = @"

					S: [\s]+;
                    (?<Gate>): ('*' / 'AND') / ('~*' / 'NAND') / ('+' / 'OR') / ('~+' / 'NOR') / ('^' / 'XOR') / ('~^' / 'XNOR');
                    ValidVariable: '""' (?<Variable>[a-zA-Z0-9]+) '""'  / '\'' (?<Variable>[a-zA-Z0-9]+) '\'' / (?<Variable>[a-zA-Z]);
                    VarProjection1: ValidVariable /  (?<Invertor>'!' ValidVariable);
                    VarProjection2: VarProjection1 / '(' Expression ')' / (?<Invertor>'!' '(' Expression ')');
                    Expression: S? VarProjection2 S? (Gate S? VarProjection2 S?)*;
                    (?<BooleanEquation>): Expression !.;
                "
					.Trim();

			AExpression ROOT = PEGrammar.Load(grammar);
			var iterator = new StringInputIterator("((((!X*Y*Z)+(!X*Y*!Z)+(X*Z))))");
			var visitor = new NpegParserVisitor(iterator);
			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
</code>

## PEG - Mathematical Formula in CSharp
<code>

			AExpression ROOT = PEGrammar.Load(
				@"
                    (?<Value>): [0-9]+ / '(' Expr ')';
                    (?<Product>): Value ((?<Symbol>'*' / '/') Value)*;
                    (?<Sum>): Product ((?<Symbol>'+' / '-') Product)*;
                    (?<Expr>): Sum;
                "
				);

			String input = "((((12/3)+5-2*(81/9))+1))";
			var iterator = new StringInputIterator(input);
			var visitor = new NpegParserVisitor(iterator);

			ROOT.Accept(visitor);
			Assert.IsTrue(visitor.IsMatch);
			AstNode node = visitor.AST;
</code>


## PEG - Mathematical Formula in C
<code>

	#include <assert.h>
	#include <stdio.h>
	#include <string.h>

	#include "robusthaven/text/npeg.h"
	#include "robusthaven/text/npeg_inputiterator.h"
	#include "robusthaven/structures/stack.h"

	#include "MathematicalFormula.h"

	#define BUFFER_SIZE 100

	int main(int argc, char *argv[])
	{
	  const char text1[] = "(1*3+4)/5*93";
	  const char text2[] = "9+(9-8)*10";  

	  char buffer[BUFFER_SIZE];
	  rh_stack_instance disableBackReferenceStack;
	  rh_stackstack_instance sandbox;
	  rh_hashmap_instance backreference_lookup;
	  rh_list_instance warnings;
	  rh_stack_instance errors;
	  npeg_context context;
	  npeg_inputiterator iterator;
	  npeg_astnode* ast, *p_child;

	  int (*parsetree)(npeg_inputiterator*, npeg_context*) = &MathematicalFormula_impl_0;
	  int IsMatch = 0;

	  // load npeg managed memory
	  context.disableBackReferenceStack = &disableBackReferenceStack;
	  context.sandbox = &sandbox;
	  context.backReferenceLookup = &backreference_lookup;
	  context.warnings = &warnings; 
	  context.errors = &errors;

	  npeg_inputiterator_constructor(&iterator, text1, strlen(text1));
	  npeg_constructor(&context, NULL);

	  IsMatch = parsetree(&iterator, &context);
	  assert(IsMatch);
	  printf("\tVerified: The expected input was matched by parser.\n");

	  ast = npeg_get_ast(&context);
	  npeg_printVisitor(ast, NULL);
	  assert(0 == strcmp(ast->token->name, "EXPRESSION"));
	  printf("\tVerified: The expected token name: '%s'.\n", ast->token->name);
	  npeg_inputiterator_get_text(buffer, &iterator, ast->token->start, ast->token->end);
	  assert(0 == strcmp(buffer, text1));
	  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
	  
</code>



## PEG - Mathematical Formula in C++
<code>

	#include <cstdio>
	#include <cassert>
	#include <iostream>
	#include <cstring>
	#include "robusthaven/text/StringInputIterator.h"
	#include "robusthaven/text/PrintVisitor.h"
	#include "MathematicalFormula.h"

	using namespace RobustHaven::Text;
	using namespace std;

	#define BUFFER_SIZE 100

	int main(int argc, char *argv[])
	{
	  const char text1[] = "(1*3+4)/5*93";
	  const char text2[] = "9+(9-8)*10";  

	  char buffer[BUFFER_SIZE];
	  InputIterator* input; 
	  MathematicalFormula* context; 
	  AstNode *ast, *p_child;

	  input = new StringInputIterator(text1, strlen(text1));
	  context = new MathematicalFormula(input);
	  assert(context->isMatch());
	  cout << "\tVerified: Matching of input succeeds\n";
	  ast = context->getAST();
	  PrintVisitor::printAST(*ast);
	  assert(ast->getToken()->getName() == "EXPRESSION");
	  printf("\tVerified: The expected token name: '%s'.\n", ast->getToken()->getName().c_str());
	  input->getText(buffer, ast->getToken()->getStart(), ast->getToken()->getEnd());
	  assert(0 == strcmp(buffer, text1));
	  printf("\tVerified: The expected matched string: '%s'.\n", buffer);
	  
</code>


## PEG - Mathematical Formula in Java
<code>

	package parser_tests;
	import robusthaven.text.npeg.tests.parsers.MathematicalFormula;
	import robusthaven.text.*;
	import junit.framework.Assert;
	import junit.framework.TestCase;
	import java.util.*;
	import java.io.*;

	public class MathematicalFormulaTest extends TestCase {
		static final String text1 = "(1*3+4)/5*93";
		static final String text2 = "9+(9-8)*10";  

		InputIterator input;
		MathematicalFormula parser;
		AstNode ast, child;
		
		public void testInput1() {
		String capturedText;

		try {
			input = new StringInputIterator(text1);
			parser = new MathematicalFormula(input);

			Assert.assertTrue(parser.isMatch());
			System.out.println("\tVerified: Matching of input succeeds\n");
		
			ast = parser.getAST();
			PrintVisitor.printAST(ast);
			Assert.assertTrue(0 == ast.getToken().getName().compareTo("EXPRESSION"));
			System.out.println("\tVerified: The expected token name: " + ast.getToken().getName());

</code>

## PEG - Mathematical Formula in Javascript
<code>

	.. framework has been "started" but no tests created yet

</code>

## PEG - Mathematical Formula in PHP
<code>

	.. framework has been "started" but no tests created yet

</code>
 