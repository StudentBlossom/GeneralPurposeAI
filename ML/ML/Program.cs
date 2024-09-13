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


//Action[] ML_output = {testfuncup, testfuncdown,  testfuncdown, testfuncdown, testfuncdown, testfuncdown };
//int ML_output_size = ML_output.Length;
//int ML_input_size = 8;//max is 8
//int ML_input_size_sq = (byte)Math.Pow(2, ML_input_size);
//this  does not work for multiple gamesets... yet

//todo put everything into 1 
uint multigame = 10;//how many games run at the same time!
uint gamesize = 10;//max should be less then byte.maxvalue (255)
byte[,,] gamestate = new byte[multigame,gamesize,gamesize];
Random rnjesus = new Random();

byte[] playerY = new byte[multigame];
byte[] playerX = new byte[multigame];
byte[] dangerY = new byte[multigame];
byte[] dangerX = new byte[multigame];

//TIME TO DO A DOUBLE AND IT IS GOING TO GO WRONG
for (int i = 0; i < multigame; i++)//to draw the game state
{
    playerX[i] = (byte)0;
    playerY[i] = (byte)4;
    dangerX[i] = (byte)(gamesize-1);
    dangerY[i] = (byte)4;
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
3 = success
made multiple to support multiple games at the same time
    */

byte[] bytearraything= new byte[] { 8,7,6,5,4,3,2,1};
byte[] bytearraything2= new byte[] { 255,255,255,255,255,255,255,255};

GPAI Mika = new GPAI(2, 3,multigame);//IT WORKS? 
GPAI.settings.makesamechoicetwice = true;//
GPAI.settings.donkey= false;
Mika.initialiazer();

Timer tijd = new Timer(repeater, null, 0, 750);//this makes it so that the function repeater gets called every 40ms
void repeater(object o)
{
    Console.Clear();
    Console.WriteLine("Welcome");
    Console.WriteLine("");
    //TEST THE TWIN SYSTEM IT DID NOT TEST PROPERLY BEFORE

    //    Console.WriteLine(Convert.ToString(255,2));
    for (int i = 0; i < multigame; i++)
    {
        byte[] inputarray = { (byte)(playerY[i] + 1), (byte)(dangerY[i] + 1) };
        gamestate[i, 0, playerY[i]] = 0;

        for (int ii = 0; ii < gamesize; ii++)
        {
            gamestate[i, dangerX[i], ii] = 0;
        }

        int decision = Mika.decision(inputarray, i);

        if (decision == 1)
        {
            playerY[i]--;
        }
        else if (decision == 2)
        {
        }
        else if (decision == 3)
        {
            playerY[i]++;
        }

        if (playerY[i] < 0 || playerY[i] >= gamesize)
        {
            Mika.feedback(false, i, 1);
            dangerY[i] = (byte)(gamesize * rnjesus.NextDouble());
            dangerX[i] = (byte)(gamesize - 1);
            playerY[i] = 4;
        }//we check that player Y is not out of bounds

        if (dangerX[i] == 0)
        {
            dangerX[i] = (byte)(gamesize - 1);
            if (dangerY[i] == playerY[i])
            {
                Mika.feedback(true, i, 1);
            }
            else
            {
                Mika.feedback(false, i, 1);
                playerY[i] = 4;
            }
            dangerY[i] = (byte)(gamesize * rnjesus.NextDouble());
        }
        else
        {
            dangerX[i]--;
        }//we check that it has not arrived yet
        gamestate[i, 0, playerY[i]] = 1;

    for (int ii = 0; ii < gamesize; ii++)
    {
        if (ii != dangerY[i])
        {
            gamestate[i, dangerX[i], ii] = 2;
        }
    }
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
                    }
                    else if (gamestate[i, ix, iy] == 2)
                    {
                        tempstr += "X";
                    }
                    else if (gamestate[i, ix, iy] == 3)
                    {
                        tempstr += "0";
                    }
                    else
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