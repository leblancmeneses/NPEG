JAVAC=javac
OBJ= robusthaven/text/AstNode.class robusthaven/text/InputIterator.class \
robusthaven/text/IAstNodeReplacement.class \
robusthaven/text/TokenMatch.class robusthaven/text/Npeg.class \
robusthaven/text/ParsingFatalTerminalException.class robusthaven/text/Warn.class\
robusthaven/text/InfiniteLoopException.class robusthaven/text/IAstNodeVisitor.class \
robusthaven/text/PrintVisitor.class



BUILDDIR= text 

all: ../robusthaven.jar

../robusthaven.jar:
	# use archiver utility to create library.
	$(JAVAC) robusthaven/text/*.java
	jar cvf robusthaven.jar robusthaven/text/*.class
	mv robusthaven.jar ../
	
clean:
	rm -f ../robusthaven.jar
	rm -f robusthaven/text/*.class
