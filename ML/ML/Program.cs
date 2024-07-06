using System;
using System.Threading;


Console.WriteLine("Welcome");
Console.WriteLine("");



uint multigame = 3;//how many games run at the same time!
uint gamesize = 10;
byte[,,] gamestate = new byte[multigame,gamesize,gamesize];


for (int i = 0; i < multigame; i++)//to draw the game state
{
    for (int ii = 0; ii < gamesize; ii++)
    {
        for (int iii = 0; iii < gamesize; iii++)
        {
            gamestate[i,ii,iii]= 0;
        }
    }
}

/*
 0 = empty .
1 = player P
2 = death
made multiple to support multiple games at the same time
 */



// array aan dictionaries
// x,y,x-y,xy er is een constante hoveelheid van omgevingsfactoren die altijd hetzellfde is
//elke getal in de array staat voor een van die omgevingsfactoren
//in de dictionariy zoek je de waarde van die factor en daarin staat waarde die onthouden moet worden

string[] teststring = {"a","b","c","d","e"};
List<string> testlist = new List<string>();
for (int i = 0; i < 5; i++)
{
    testlist.Add(teststring[i]);
}
for(int i = 0;  i < 5;i++)
{
    for(int ii = i + 1; ii < 5;ii++)    
    {
        testlist.Add(teststring[i] + "-" + teststring[ii]);
    }
}


//input is bytes
//6 bytes
//lets say 8 bytes at max
//long = 8 bytes
//we can remember the input USING BIT MASK
//of 1 byte
// DUS als we werken met 8 inputs 
//moeten we het getal 0000 0101 opzoeken wat X en Z is!!!!!!!!!
//en 0000 0111 is XYZ en dat kunnen we gebruiken om een bitmask te generen voor die operatie
//die voeren we uit op de long die we creeren uit de invoer!!!!
//we moeten wel alle 8 bytes in een long zetten!
//we kunnen dit principe ook toepassen op de x-y
//dan moeten we een long voor alle x-y x-z ff kijken hoe we die doen maar dat kan
//
//WE KUNNEN DAN OOK EEN LONG CREEREN VAN X-Y&Z WANT 
//als we de long van de x nemen en die willen we combineren met
//de z in de invoerlong hebben we 2 longs
//de ene is (in bytemask) 0000 0001 voor Z en 0000 0010 voor x-y
//moeten wel kijken of we die in 2 dictionaries moeten zetten want wat nu als 
//x-y= een y die al gedefinieerd is?

//alleen probleem als een getal 0 is maar dat kunnen we oplossen

//TODO CODE BYTEMASK met parabyte en paralong



//we willen heel veel factoren maken
//DUS we moeten x,y, z x-y, x-z,y-z, xy, xz,yz z x-y, y x-z, x y-z,xyz
//als basis nemen we x,y,z,x-y,x-z,z-y
//daarop moeten we alle gecombineerde variabelen nemen. dus xy,xz,yz
//kunnen we voorkomen dat we xy x-y hebben? geen idee


// 1100 1010 0100 1010
// 1010
//1100 0000 0100 0000
//0000 0000 0000 0000
//1111 0000 0000 0000 >> 0000 1111 0000 0000
//
ulong bytemask (byte parabyte, ulong paralong) //see the paralong as an array of bytes, 8 of them, and the parabyte as mask to show which bytes are returned
{
    ulong returnme = paralong;
    ulong tempmasklong = 0;
    for(int i = 0; i < 7; i++)
    {
        if (parabyte -128>=0)//als het meest linkse getal 1 is
        {
            tempmasklong = tempmasklong + 255;//voegen we een byte aan 1tjes toe
        }
        tempmasklong = (tempmasklong << 8);//en die bewegen we naar links
        parabyte = (byte)(parabyte << 1);//en de volgende die we moeten bekijken schuiven we nu door
    }
    if (parabyte - 128 >= 0)//als het meest rechste getal 0 is
    {
        tempmasklong = tempmasklong + 255;
    }
    return (returnme&tempmasklong);
}


byte x = 0; 
Timer tijd = new Timer(repeater, null, 0, 1000);//this makes it so that the function repeater gets called every 40ms
void repeater(object o)
{
      Console.Clear();

    Console.WriteLine("Welcome");
    Console.WriteLine("");
    x++;
    ulong testvalue = bytemask(x, ulong.MaxValue);
    Console.WriteLine(Convert.ToString((long)testvalue,2));
    Console.WriteLine(Convert.ToString(x,2));
    for(int i = 0; i < testlist.Count; i++)
    {
        Console.WriteLine(testlist[i]);
    }

    for (int iy = 0; iy < gamesize; iy++)
    { //y
        string tempstr = "";
        for (int i = 0; i < multigame; i++)
        { //to draw the game state
            tempstr += " ";
            for (int ix = 0; ix < gamesize; ix++)
            {
                if (gamestate[i, ix, iy] == 0)
                {
                    tempstr += ".";
                } else if(gamestate[i, ix, iy] == 2)
                {
                    tempstr += "X";
                } else
                {
                    tempstr += "p";
                }
            }
        }
 //      Console.WriteLine(tempstr);
    }

}


Console.ReadLine();





//copyright owned by BlossomStudio