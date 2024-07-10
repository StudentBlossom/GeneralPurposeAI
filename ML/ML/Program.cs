using System;
using System.Runtime.CompilerServices;
using System.Threading;


Console.WriteLine("Welcome");
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
int ML_input_size = 6;//max is 8
int ML_input_size_sq = (byte)Math.Pow(2, ML_input_size);
//this  does not work for multiple gamesets... yet


uint multigame = 3;//how many games run at the same time!
uint gamesize = 7;//
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


ulong bytemask (byte parabyte, ulong paralong) //see the paralong as an array of bytes, 8 of them, and the parabyte as mask to show which bytes are returned
{
    ulong tempmasklong = 0;
    parabyte =(byte) (parabyte<<(8 - ML_input_size));//we get it in format with all the bits centered around the right but we need to check it on the left
    for (int i = 0; i < ML_input_size-1; i++) //we skipped these steps above
    {
        if (parabyte -128>=0)//als het meest linkse bit 1 is
        {
            tempmasklong = tempmasklong + 255;//voegen we een byte aan 1tjes toe
        }
        tempmasklong = (tempmasklong << 8);//en die bewegen we naar links
        parabyte = (byte)(parabyte << 1);//en de volgende die we moeten bekijken schuiven we nu door
    }
    Console.WriteLine(parabyte);
    if (parabyte - 128>= 0)//hier checken we de laatste bit. dit doen we niet in de loop want als we dan de tempmasklong doorschuiven klopt die niet meer
    {
        tempmasklong = tempmasklong + 255;//de laatste bit in de maskbyte is de eerste byte in de long
    }
    return (paralong&tempmasklong);//we masken de originele en returnen het
 
    // 1100 1010 0100 1010 (de long)
    // 1010 (de byte)
    //1100 0000 0100 0000 (de uitkomst)
}


Dictionary<ulong, memory> learned = new Dictionary<ulong, memory>();
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
    Console.WriteLine(ML_output_size);

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

int[] ML_choice_calculator_aid(long paralong) //de paralong is all the inputs combined into 1
{
    int[] returnme = new int[ML_output_size];

    for (byte i = 1; i <= ML_input_size_sq; i++) //to check all the code
    {
        memory x = learned[bytemask(i, (ulong)paralong)];
    }

    return returnme;
}


static public class ML_finetune
{
    public static uint startvalue = 100;

}

public class memory//check if this is the best way or if I need to change 
    //the construct class to take a int[] as para so I can change it. idk this is a dictionary quirk
{
    public byte maskID;
    public uint[] values;

    public memory(byte parabyte,ulong paralong,int paraint)
    {
         maskID = parabyte;
        values = new uint[paraint];
        for(int i = 0; i < paraint; i++)
        {
            values[i] = ML_finetune.startvalue;
        }
    }


}


//copyright owned by BlossomStudio