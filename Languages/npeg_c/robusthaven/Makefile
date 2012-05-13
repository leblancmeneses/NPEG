CFLAGS=-ggdb

OBJ = structures/hashmap.o structures/inthashmap.o structures/stack.o structures/stackstack.o \
	structures/list.o text/npeg.o text/npeg_ast.o text/npeg_inputiterator.o text/printvisitor.o


define cc-command
	# compile do not link use: -c
	$(CC) $(CFLAGS) -c $< -o $@
endef

%.o: %.c
	$(cc-command)



all: robusthaven.a

robusthaven.a: $(OBJ)
	# use archiver utility to create library.
	ar rvs robusthaven.a $(OBJ)
	
clean:
	rm -f robusthaven.a
	rm -f -R $(OBJ)
