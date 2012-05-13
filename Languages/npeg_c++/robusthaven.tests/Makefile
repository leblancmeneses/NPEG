CXX=g++
CXXFLAGS=-ggdb -Wno-deprecated
CPPFLAGS= -I ../

STATIC_LIBRARIES = ../robusthaven/robusthaven.a

PARSER_TESTS = ParserTest_PhoneNumber.o ParserTest_NpegNode.o \
	ParserTest_MathematicalFormula.o ParserTest_SimpleXml.o

TESTS = $(subst .cpp,.o,$(shell ls *.cpp))

%.o: %.cpp
	@mkdir -p bin
	$(CXX) $(CXXFLAGS) $(CPPFLAGS) $< ${STATIC_LIBRARIES} -o bin/$@


all: ${STATIC_LIBRARIES} ${TESTS} ${PARSER_TESTS}

../robusthaven/robusthaven.a: 
	$(MAKE) -f Makefile --directory=../robusthaven all

${PARSER_TESTS}:
	$(MAKE) -f Makefile --directory=./$(subst .o,,$@) all

clean: 
	$(MAKE) -f Makefile --directory=../robusthaven clean
	for folder in $(subst .o,,${PARSER_TESTS}); do \
		$(MAKE) -f Makefile --directory=./$${folder} clean; \
	done ;

	rm -Rf *.c~ *.o Makefile~ *.teststderr *.teststdin *.sh~ bin ../robusthaven/robusthaven.a

