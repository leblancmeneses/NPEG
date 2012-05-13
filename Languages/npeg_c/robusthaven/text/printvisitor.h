#ifndef ROBUSTHAVEN_TEXT_PRINTVISITOR_H
#define ROBUSTHAVEN_TEXT_PRINTVISITOR_H
#include <stdio.h>
#include "npeg.h"

/*
 * Recursively prints an AST
 * out is the FILE pointer used for output. If the user passes NULL, sdout is used.
 */
void npeg_printVisitor(npeg_astnode *root, FILE *out);

#endif
