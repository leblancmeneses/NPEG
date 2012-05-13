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
echo "Testing hashmap implementation:"
fn_test_run "bin/test_hashmap_" $isverbose

echo
echo "Testing AST implementation:"
fn_test_run "bin/test_ast_" $isverbose

echo
echo "Testing input iterator implementation:"
fn_test_run "bin/test_inputiterator_" $isverbose

echo
echo "Testing npeg terminals:"
fn_test_run "bin/test_terminal_" $isverbose

echo
echo "Testing npeg non-terminals:"
fn_test_run "bin/test_nonterminal_" $isverbose

echo
echo "Testing ParserTest:"
fn_test_run "ParserTest_NpegNode/program" $isverbose
fn_test_run "ParserTest_PhoneNumber/program" $isverbose
fn_test_run "ParserTest_MathematicalFormula/program" $isverbose
fn_test_run "ParserTest_SimpleXml/program" $isverbose

