# Good for Nothing Compiler  in C&#35; #
This is a continuation of the old Good for Nothing Compiler presented by Joel Pobar and Joe Duffy at PDC back in 2005! 
The presentation was liked and the article and code posted at https://msdn.microsoft.com/en-us/magazine/cc136756.aspx
has been read and used by many programmers who wanted to learn more about Scanners, Parsers and how to write a simple
compiler in C#.

----------------------------------------------------------------------------------------------------------------------

Hi! I'm Ashton, or MaidenKeebs. I've forked this project and decided to just add to it and basically see what I can
do with it really. At the moment, I'm thinking of adding more to the language itself, plus chucking in a standard
library to boot, in an attempt to develop this into a language focusing on the development of text-based adventure
games. I'm doing this because I need something to do in my spare time, and this seems like fun!

----------------------------------------------------------------------------------------------------------------------

Ideally, the end result of this project would produce a language which would look something like this:

```
Object player = Player(hp = 100,lives = 3, name="MaidenKeebs", class="None");

# ${...} denotes use of a variable within a string literal.
@IO:Print("Greetings, ${player.name}.");

# Offer a choice.
Integer player_class = @IO:Choice("What kind of adventurer are you?", "Warrior", "Mage", "Assassin");

# React to the player's class.
if (player_class == "Warrior")
{
	@IO:Print("Ah, a strong warrior, eh?");
}
else if (player_call == "Mage")
{
	@IO:Print("We don't get many of your kind around anymore...");
}
else if (player_call == "Assassin")
{
	@IO:Print("Your kind are not welcome here, assassin!");
}

```

But this could change in the future...