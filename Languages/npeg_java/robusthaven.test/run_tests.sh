#!/bin/bash
# Format of test app names: test_<structure>_<tested routines>

echo "Building tests...."
make clean all
status=$?
if [ "$status" != "0" ]
then
	echo "Build error status code: $status";
	exit $status
fi
 
echo
echo


. ./lib_tests.sh


isverbose=0
[ "$1" = "--verbose" ] && isverbose=1

echo
echo "Testing AST implementation:"
fn_test_run "Ast*.class" $isverbose

echo
echo "Testing input iterator implementation:"
fn_test_run "InputIterator*.class" $isverbose

echo
echo "Testing npeg terminals:"
fn_test_run "*TerminalTest.class" $isverbose

echo
echo "Testing npeg non-terminals:"
fn_test_run "*NonterminalTest.class" $isverbose

echo
echo "Testing ParserTest:"
fn_test_run "parser_tests/NpegNodeTest.class" $isverbose
fn_test_run "parser_tests/MathematicalFormulaTest.class" $isverbose
#fn_test_run "ParserTest_PhoneNumber/program" $isverbose
#fn_test_run "ParserTest_MathematicalFormula/program" $isverbose
#fn_test_run "ParserTest_SimpleXml/program" $isverbose
