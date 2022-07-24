# Outlet-Lang
The Outlet programming language
<<<<<<< HEAD
=======
## Info
>>>>>>> 3a188e3a869fc5ecb920c842e1ae51a2a2d90ab5
## Syntax
### Declarations
Declaring a Variable:
```C#
int a = 5;
// Array and Tuple types
int[] array = [3,4,5,6,7];
(int, string) tuple = (3, "tuple");
// Type constructions can be combined
(int, string)[] arraytuple = [(4, "this is a tuple"), (54, "inside of an array")];
int[][] multidimensional;
// Function types
(int, int) => bool = equal;
// Type Nicknames
type numbertuple = (int, int);
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
```C#
// If Statements
if(true) print("it's true!");
else {
  print("single statement bodies don't need curly brackets");
  print("but multi lines do");
}
// While Loops:
while(a > 0) {
  print("while loops are pretty normal");
  --a;
} 
// For Loops:
for(int i in array){
  print("for loops act like for each loops");
}
```
### Expressions

```C#
// regular arithmetic and logic operators work as expected
bool b == false || true && equal(4, a);
int c = a * -10 + (4/5) + max(3, 5);
// Ternary Operator
int d = 5 < c && ? 10 : 15;
// Tuples
numbertuple dem = (4,5);
// List Literals:
object[] lst = [1, true, "hello!"];
```
