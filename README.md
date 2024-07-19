# SelfLearningAI1.0
I already have made a self learning AI before in JS now I want to recreate the same concept in C# and improve on it

Conceptual:
the AI has 8 possible bytes as inputs:
	-We are going to assume you use all 8 for the explenation
	-none of the inputs are 0
the AI has a number of possible actions
	-this number can basically be max array
The AI get an input and returns a random number,between 0 and n where n=amount of actions
It calculates this using it's memory. 
the memory is a dictionary using longs and a memory class. the long is comprised of all the input bytes
it gets a return of all possible combinations of inputs (x,y,z,xy,xz,xyz) and uses the values to calculate the weights
these weights are used in a random number generator to get an option

when the AI gets a success it will increase the weights for all of the decisions it took at the points
when it gets a fail it decreases




All rights are owned by me
