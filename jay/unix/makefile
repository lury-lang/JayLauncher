CC	= gcc
CFLAGS	= -s

c	= closure.c error.c lalr.c lr0.c main.c mkpar.c output.c reader.c \
	  symtab.c verbose.c warshall.c
e	= jay

all: jay
clean: ; rm -f jay *.exe *.o
jay: $(c:.c=.o); $(CC) -o $e $(CFLAGS) $(c:.c=.o)