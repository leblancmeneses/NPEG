CXX=g++
CXXFLAGS=-ggdb -Wno-deprecated
CPPFLAGS= -I ../

ROBUSTHAVEN_HEADERS = ../../
STATIC_LIBRARIES = $(ROBUSTHAVEN_HEADERS)robusthaven/robusthaven.a
OBJ = NpegNode.o 

%.o: %.cpp
	@mkdir -p bin
	$(CXX) $(CXXFLAGS) $(CPPFLAGS) -I $(ROBUSTHAVEN_HEADERS) -c $< -o bin/$@

all: $(OBJ)
	$(CXX) $(CXXFLAGS) $(CPPFLAGS) -I $(ROBUSTHAVEN_HEADERS) $(addprefix bin/, $(OBJ)) main.cpp ${STATIC_LIBRARIES} -o program

clean:
	rm -Rf $(OBJ) Makefile~ program *.teststderr *.teststdin bin
