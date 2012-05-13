CFLAGS=-ggdb 
ROBUSTHAVEN_HEADERS = ../../
STATIC_LIBRARIES = $(ROBUSTHAVEN_HEADERS)robusthaven/robusthaven.a
OBJ = NpegNode.o 

%.o: %.c
	@mkdir -p bin
	gcc $(CFLAGS) -I $(ROBUSTHAVEN_HEADERS) -c $< -o bin/$@

all: $(OBJ)
	gcc $(CFLAGS) -I $(ROBUSTHAVEN_HEADERS) $(addprefix bin/, $(OBJ)) main.c ${STATIC_LIBRARIES} -o program

clean:
	rm -Rf $(OBJ) Makefile~ program *.teststderr *.teststdin bin
