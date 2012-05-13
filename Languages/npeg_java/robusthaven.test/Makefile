JUNIT_PATH=/usr/share/java
TESTS=InputIteratorEmptyTest.class InputIteratorGetTextTest.class InputIteratorStdLookupsTest.class\
AstTreeBuildingTest.class AnyCharacterTerminalTest.class \
CharacterClassTerminalTest.class CodePointTerminalTest.class FatalTerminalTest.class\
LiteralTerminalTest.class WarnTerminalTest.class AndPredicateNonterminalTest.class\
CapturingGroupNonterminalTest.class LimitingRepetitionNonterminalTest.class \
OneOrMoreNonterminalTest.class OptionalNonterminalTest.class \
PrioritizedChoiceNonterminalTest.class SequenceNonterminalTest.class \
ZeroOrMoreNonterminalTest.class

all: ${TESTS} testparsers.jar ${PARSERTESTS} parser_tests.jar

%.class: %.java ../robusthaven.jar
	javac -cp ../robusthaven.jar:${JUNIT_PATH}/junit.jar *.java

parser_tests.jar: parser_tests/*.java testparsers.jar
	javac -cp testparsers.jar:../robusthaven.jar:.:${JUNIT_PATH}/junit.jar parser_tests/*.java
	jar cvf $@ parser_tests/*.class

testparsers.jar: robusthaven/text/npeg/tests/parsers/*.java ../robusthaven.jar
	javac -cp ../robusthaven.jar:${JUNIT_PATH}/junit.jar:robusthaven/text/npeg/tests/parsers/ $<
	jar cvf $@ robusthaven/text/npeg/tests/parsers/*.class

clean:
	rm -f ${TESTS} parser_tests/*.class testparsers.jar parser_tests.jar robusthaven/text/npeg/tests/parsers/*.class