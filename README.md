# Good for Nothing Compiler in C&#35;
This is a continuation of the old Good for Nothing Compiler presented by Joel Pobar and Joe Duffy at PDC back in 2005! 
The presentation was liked and the article and code posted at https://msdn.microsoft.com/en-us/magazine/cc136756.aspx
has been read and used by many programmers who wanted to learn more about Scanners, Parsers and how to write a simple
compiler in C#.

----------------------------------------------------------------------------------------------------------------------
Before I, MaidenKeebs forked this repository and worked on it, it compiled a simple c-like language
and had support for variables, simple inputs/outputs and a for-loop. I've temporarily removed functionality
for input and for-loops while I worked on enhancing output and improved on function calling. Initially
the language "understood" one function called print which looked like this:

	print "Hello, World!";

I've changed this so now function calling looks like this:

@IO:Print("Hello, World!");

Although I've added extra bits to the syntax, it's more extendible. For example, I've created a small standard library
which defines several functions within multiple modules, such as the IO module, which contains the Print function.

----------------------------------------------------------------------------------------------------------------------

The article by the gentlemen mentioned above contains a Language Definition using a metasyntax called EBNF 
(Extended Backus-Naur Form), that should support parsing and compiling of arithmetic expressions, but the 
code doesn't, so I thought I would try and add support for that and learn something along the way. 

**Feel free to spice the code up!**

##Language Specification##
This is the language specification defined in a simple EBNF style,
I (MaidenKeebs) have modified it quite a bit from what it was
when I forked it:

```
<statement> := <variable_creation>;
	| <variable_assignment>;
	| <function_call>;

<variable_creation> := <data_type> <identifier> = <expression>
<variable_assignment> := <identifier> = <expression>
<function_call> := <stdlib_function_call> | <user_defined_function_call>

<stdlib_function_call> := @<module>:<identifer>(<parameter>*[, <parameter>]*)
<user_defined_function_call> := @identifier(<parameter>*[, <parameter>]*)

<data_type> := integer | string | boolean

<expression> := <identifier> | <integer_literal>+ | <string_literal>

<identifier> := <char>+
<integer_literal> := 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9
<string_literal> := "<character_literal>*"

<character_ltieral> := 'a..z | A..Z | 0..9 | <special_character_literal>'
<special_character_literal> := (anything but a " mark)
```
##Sample Programs##
Here's a basic example of what the language can do at the moment:
```
string    player_name    = "MaidenKeebs";
integer32 player_health  = 100;
boolean   player_is_dead = false;

@IO:Print("Welcome, adventurer!");
```


