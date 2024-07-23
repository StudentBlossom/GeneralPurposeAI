using Microsoft.VisualBasic;
using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Linq;
//TODO clean up the using beginning
//wow this code is very minmaxxed
Console.WriteLine("Welcome");
Console.WriteLine("Made by blossom");
Console.WriteLine("");

int playerY = 4;
void testfuncup () {
    playerY++;
}
void testfuncdown () {
    playerY--;
}

Action[] ML_output = {testfuncup, testfuncdown,  testfuncdown, testfuncdown, testfuncdown, testfuncdown };
int ML_output_size = ML_output.Length;
int ML_input_size = 8;//max is 8
int ML_input_size_sq = (byte)Math.Pow(2, ML_input_size);
//this  does not work for multiple gamesets... yet

//todo put everything into 1 
uint multigame = 1;//how many games run at the same time!
uint gamesize = 10;//max should be less then byte.maxvalue (255)
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

byte[] bytearraything= new byte[] { 1,2,3,4,5,6,7,8};
ML Mika = new ML(2, 2,multigame);
Mika.initialiazer();
Timer tijd = new Timer(repeater, null, 0, 1000);//this makes it so that the function repeater gets called every 40ms

void repeater(object o)
{
        Console.Clear();
    Console.WriteLine("Welcome");
    Console.WriteLine("");
//    Console.WriteLine(Convert.ToString(255,2));

    int x = Mika.decision(bytearraything,0);
    Console.WriteLine(x);




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
        Console.WriteLine(tempstr);
    }

}


Console.ReadLine();






//copyright owned by BlossomStudio