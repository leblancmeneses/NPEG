#include <cassert>
#include <iostream>
#include "robusthaven/text/AstNode.h"

using namespace RobustHaven::Text;
using namespace std;

int main(int argc, char *argv[]) {
  TokenMatch *p_token;
  AstNode *p_root;

  p_root = new AstNode(NULL, false);
  assert(p_root->getChildren().size() == 0);
  assert(p_root->getToken() == NULL);  
  delete p_root;
  cout << "\tVerified: Correct initialization w/o token.\n";
  
  p_token = new TokenMatch("a token", 0, 0); 
	// AstNode is responsible to delete p_token
  p_root = new AstNode(p_token, true);
  assert(p_root->getChildren().size() == 0);
  assert(p_root->getToken() == p_token);  
  delete p_root;
  cout << "\tVerified: Correct initialization w/ token.\n";

  return 0;
} /* main */
