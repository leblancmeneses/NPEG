#!/bin/bash

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


echo "Testing Memory CleanUp:"
fn_test_memory "bin/test_hashmap_" $isverbose
fn_test_memory "bin/test_stack_" $isverbose
fn_test_memory "bin/test_stackstack_" $isverbose
fn_test_memory "bin/test_list_" $isverbose
fn_test_memory "bin/test_ast_" $isverbose
fn_test_memory "bin/test_inputiterator_" $isverbose
fn_test_memory "bin/test_terminal_" $isverbose
fn_test_memory "bin/test_nonterminal_" $isverbose
fn_test_memory "ParserTest_NpegNode/program" $isverbose
fn_test_memory "ParserTest_PhoneNumber/program" $isverbose
fn_test_memory "ParserTest_SimpleSentence/program" $isverbose
fn_test_memory "ParserTest_MathematicalFormula/program" $isverbose
fn_test_memory "ParserTest_SimpleXml/program" $isverbose
