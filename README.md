#NPEG - .NET Parsing Expression Grammar.  

*	The framework can output equivalent parsers natively in C/C++/C#/Java/PHP/Javascript by swapping out the visitor (most are already built).
*	The framework can read and write it's own DSL.  See samples of the DSL below.


## PEG - Boolean Algebra
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

## PEG - Mathematical Formula
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

