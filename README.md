# Outlet-Lang
The Outlet programming language
## Info
## Syntax
### Declarations
Declaring a Variable:
```C#
int a = 5;
// Array and Tuple types
(int, string)[] = [(4, "this is a tuple"), (54, "inside of an array")];
int[][] multidimensional;
// Function types
(int, int) => bool = equal;
```
Defining a Function:
```C#
// inline
bool equal(int a, int b) => a == b;
// traditional (and demonstration of mutual recursion)
bool even(int n) {
  if(n == 0) return true;
  return odd(n-1);
}
bool odd(int n) {
  if(n == 0) return false;
  return even(n-1);
}
```
### Statements
### Expressions
