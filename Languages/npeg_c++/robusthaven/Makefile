CXXFLAGS=-ggdb -Wall -Wno-deprecated

OBJ= text/AstNode.o text/InputIterator.o text/Npeg.o text/Warn.o text/StringInputIterator.o \
text/FileInputIterator.o 


# compile do not link use: -c
define cpp-command
	$(CXX) $(CXXFLAGS) $(CPPFLAGS) -c $< -o $@
endef


BUILDDIR= text 

%.o: %.cpp
	$(cpp-command)



all: robusthaven.a

robusthaven.a: $(OBJ)
	# use archiver utility to create library.
	ar rvs robusthaven.a $(OBJ)
	
clean:
	rm -f robusthaven.a
	rm -f -R $(OBJ) *.h~ *.cpp~
