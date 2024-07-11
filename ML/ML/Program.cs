using Microsoft.VisualBasic;
using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
//TODO clean up the using

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
uint multigame = 3;//how many games run at the same time!
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


byte t = 3;
Timer tijd = new Timer(repeater, null, 0, 1000);//this makes it so that the function repeater gets called every 40ms
void repeater(object o)
{
        Console.Clear();

    Console.WriteLine("Welcome");
    Console.WriteLine("");
    Console.WriteLine(Convert.ToString((t),2));
    Console.WriteLine(t);
    switch (t)
    {
        case 192: //1100 0000
            t = 5;//0000 0101
        break;
        case 160://1010 0000
            t = 9;
        break;
        case 144: //1001 0000
            t = 17;
        break;
        case 136://1000 1000
            t = 33;
        break;
        case 132://1000 0100
            t =65;
        break;
        case 130://1000 0010
            t = 129;
        break;
        default:
            t = (byte)(t << 1);
        break;
    }
    //it does work (kinda)
        


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
    }

}


Console.ReadLine();



public class ML
{
    public static ulong EXP = 0;//elke keer dat een waarde word aangepast word het verschil hierbij opgeteld
                                //de EXP is niet nodig maar geeft een rough idea van hoeveel de AI al gelearnd heeft. hangt af van de gefindetunde waardes
    private static string credits = "BlossomStudio";

    public static int input_size = 6;//max is 8
    public static int input_size_sq = (byte)Math.Pow(2, input_size);
    public static int output_size = 4;
    static Dictionary<ulong, memory> learned = new Dictionary<ulong, memory>();
    static Dictionary<ulong, bool[]>[] active = new Dictionary<ulong, bool[]>[interaction.twins];//de bool staat voor welke keuzes genomen zijn

    static public class settings
    {
        static public class finetune
        {
            public static ulong start = 1000; //de start value van alle waarden
            public static int punish = 2; //hoeveel het getal omlaag gaat als faal
            public static int reward = 3; //hoeveel het getal omhoog gaat als goed
        }
        public static string name = "Gai"; //Great Artificial Intellegence
        public static bool donkey = true; //hij oonthoud de laatste factoren die hij gevoerd kreeg. en als de keuze direct daarna lijdt is de preciese gecombineerde factoren die keuze naar 0 gezet
        public static bool selfreset = false; //als hij geen opties heeft moet hij kunnen reseten
    }
    static public class interaction
    {
        public static bool stuck = false;//this will be set to true when it detects 0 options if selfreset = true
        public static byte[] inputs;//probs won't be used
        public static int twins = 3;//should be gamestate

    }

    ML(int parainputsize,int paraoutputsize)
    {
        input_size= parainputsize;
        output_size = paraoutputsize;
        input_size_sq = (byte)Math.Pow(2, input_size);
    }
    //we make it an array so that we can work parallels
    private class memory//check if this is the best way or if I need to change 
                       //the construct class to take a int[] as para so I can change it. idk this is a dictionary quirk
    {
        public byte maskID;
        public ulong[] values;//maybe use maskID and internal value to see if it is a legit or not (no 0 where it should not be 0)
        //maybe change values to uint instead?
        public memory(byte parabyte, ulong paralong, int paraint)
        {
            maskID = parabyte;
            values = new ulong[paraint];
            for (int i = 0; i < paraint; i++)
            {
                values[i] = ML.settings.finetune.start;
            }
        }


    }
    public static int decision(byte[] parabytearray, int paraint)
    {
        interaction.stuck = false;
        //make history

        
        return 0;
    }
    private static ulong[] ML_choice_calculator_aid(long paralong) //de paralong is all the inputs combined into 1
    {
        ulong[] returnme = new ulong[output_size];
            for(int i=0;i < output_size; i++)
        {
            returnme[i] = 1;
        }
        for (byte i = 1; i <= input_size_sq; i++) //to check all the code
        {
            memory x = learned[bytemask(i, (ulong)paralong)];
            for (int ii = 0; ii < output_size; ii++)
            {
                returnme[ii] = returnme[ii] * x.values[i];
            }

        }

        return returnme;
    }

     

    //een int kan gebruikt worden met de eerste int de bytemask is van welke getallen
    //het is 4 bytes de eerste is de bytemask de laatste 2 waarvan eentje de - is en de ander een getal?
    //hoe houden we die uit elkaar? x,y-z en y, x-z?
    //daar kunnen we de tweede byte voor gebruiken?

    //de tweede byte heeft 1 bit die overeenkomt met de eerste byte 
    //1100 0000 en 0100 1000
    //dan weten we dat het de tweede bit de eerste is in de formule van x-y
    //als het staat voor abdce en het 1100 en 0100 weten we dat het b-a is
    //we hebben geen negatieve getallen vanwege bytes DUS we kunnen dit gebruiken
    //de tweede byte weten we dat geen bit 1 is waaar die in de tweedeb yte ook 1 is
    //dus bij 1100 0000 en 0100 1000
    //is het b-a & e is

    static ulong bytemask(byte parabyte, ulong paralong) //see the paralong as an array of bytes, 8 of them, and the parabyte as mask to show which bytes are returned
    {
        ulong tempmasklong = 0;
        parabyte = (byte)(parabyte << (8 - input_size));//we get it in format with all the bits centered around the right but we need to check it on the left
        for (int i = 0; i < input_size - 1; i++) //we skipped these steps above
        {
            if (parabyte - 128 >= 0)//als het meest linkse bit 1 is
            {
                tempmasklong = tempmasklong + 255;//voegen we een byte aan 1tjes toe
            }
            tempmasklong = (tempmasklong << 8);//en die bewegen we naar links
            parabyte = (byte)(parabyte << 1);//en de volgende die we moeten bekijken schuiven we nu door
                                                //dit MOET een byte zijn als het een int is dan 1000 word 1 0000 dus groter dan 128
        }
        if (parabyte - 128 >= 0)//hier checken we de laatste bit. dit doen we niet in de loop want als we dan de tempmasklong doorschuiven klopt die niet meer
        {
            tempmasklong = tempmasklong + 255;//de laatste bit in de maskbyte is de eerste byte in de long
        }
        return (paralong & tempmasklong);//we masken de originele en returnen het
                                            // 1100 1010 0100 1010 (de long)
                                            // 1010 (de byte)
                                            //1100 0000 0100 0000 (de uitkomst)
    }
    static ulong bytestoulong(byte[] parabytearray)
    {
        ulong returnme = 0;
        for (int i = 0; i < input_size; i++)
        {
            returnme = returnme << 8;
            returnme = returnme + parabytearray[i];
        }
        return returnme;
    }
    private static void addhistory(ulong[] paraarray)//TODO
    {
        //create memory cells then adds them
    }
    public static void writehistory ()//TODO
    {
        //make it write the entire dictionary in a .txt file in a way that copy/paste could make it functional
    }

}



//copyright owned by BlossomStudio