using Microsoft.VisualBasic;
using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
//TODO clean up the using beginning
//wow this code is very minmaxxed
Console.WriteLine("Welcome");
Console.WriteLine("Made by blossom");
Console.WriteLine("");

byte playerY = 1;
byte playerX = 1;

//Action[] ML_output = {testfuncup, testfuncdown,  testfuncdown, testfuncdown, testfuncdown, testfuncdown };
//int ML_output_size = ML_output.Length;
//int ML_input_size = 8;//max is 8
//int ML_input_size_sq = (byte)Math.Pow(2, ML_input_size);
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

gamestate[0, 1, 1] = 1;
gamestate[0, 1, 0] = 2;
gamestate[0, 0, 1] = 2;
gamestate[0, 1, 2] = 2;
gamestate[0, 2, 0] = 2;
gamestate[0, 2, 2] = 2;
gamestate[0, 3, 1] = 3;

/*
    0 = empty .
1 = player P
2 = death
3 = success
made multiple to support multiple games at the same time
    */

byte[] bytearraything= new byte[] { 8,7,6,5,4,3,2,1};
byte[] bytearraything2= new byte[] { 255,255,255,255,255,255,255,255};

GPAI Mika = new GPAI(2, 4,multigame);
GPAI.settings.finetune.start = 100;
GPAI.settings.makesamechoicetwice = true;
Mika.initialiazer();

Timer tijd = new Timer(repeater, null, 0, 1000);//this makes it so that the function repeater gets called every 40ms

void repeater(object o)
{
    Console.Clear();
    Console.WriteLine("Welcome");
    Console.WriteLine("");
    //    Console.WriteLine(Convert.ToString(255,2));
    byte[] bytearray = { playerX, playerY };
    gamestate[0, playerX, playerY] = 0;//we know that our position is good so we remove the 1 for clarity
    int decision = Mika.decision(bytearray,0);
    //we make a decision
    if (decision == 1)
    {
        playerX++;
        Console.WriteLine("go right");
    }
    else if (decision == 2)
    {
        playerX--;
        Console.WriteLine("go left");
    }
    else if (decision == 3)
    {
        playerY++;
        Console.WriteLine("go down");
    }
    else if (decision == 4)
    {
        playerY--;
        Console.WriteLine("go up");
    }
    //we check if that decision was good
    if (gamestate[0, playerX, playerY] == 3)
    {
        Console.WriteLine("GOOD");
        playerY = 1;
        playerX = 1;
        Mika.feedback(true, 0, 1);
    }
    else if (gamestate[0, playerX, playerY] == 2)
    {
        Console.WriteLine("BAD");
        playerY = 1;
        playerX = 1;
        Mika.feedback(false, 0, 1);
    }
    //if it was a success/fail we reset pos to original
    //if it was neiter we set 1 to reflect

    gamestate[0, playerX, playerY] = 1;


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
                } else if(gamestate[i, ix, iy] == 3) { 
                    tempstr += "0";
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