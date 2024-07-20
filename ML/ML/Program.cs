using Microsoft.VisualBasic;
using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
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

byte[] bytearraything= new byte[] { 1,2,3,4,5,6,7,8};
ML Mika = new ML(8, 3,gamesize);
Timer tijd = new Timer(repeater, null, 0, 1000);//this makes it so that the function repeater gets called every 40ms
void repeater(object o)
{
        Console.Clear();

    Console.WriteLine("Welcome");
    Console.WriteLine("");
    Console.WriteLine(Convert.ToString(255,2));

    int x = Mika.decision(bytearraything,1);

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
    private static ulong EXP = 0;//elke keer dat een waarde word aangepast word het verschil hierbij opgeteld
    public ulong XP(){//this way you can't set EXP
        return EXP;
    }
    public string HELP()
    {
        //I know this is absolutely helpless
        return "Please first take a look at the comments and otherwise visit https://github.com/StudentBlossom/GeneralPurposeAI for any help";
    }
                                //de EXP is niet nodig maar geeft een rough idea van hoeveel de AI al gelearnd heeft. hangt af van de gefindetunde waardes
    private static string credits = "BlossomStudio";
    
    public byte[] input;
    public static int input_size;//max is 8
    public static int input_size_sq;
    public static int output_size;
    private Dictionary<ulong, memory> learned_long = new Dictionary<ulong, memory>();

    private Dictionary<ulong, bool[]>[] active_memory = new Dictionary<ulong, bool[]>[interaction.twins];//de bool staat voor welke keuzes genomen 
    //we make it an array so that we can work parallels
    private static bool[] default_values_bool_array;
    public void initialiazer()
    {
        for(int i = 0; i < output_size; i++)
        {
            defaultarrays.weight[i] = settings.finetune.start;
        }
    }
    private static class defaultarrays
    {
        public static bool[] bools;
        public static ulong[] weight;
        public static ulong[] baseweight;
    }
    public class settings
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
        public static bool makesamechoicetwice = true;//if it has the exact same parameters in active work memory disallow the same options
        public static bool learning = true;
    }
    public class interaction
    {
        public static bool stuck = false;//this will be set to true when it detects 0 options if selfreset = true
        public byte[] inputs;//probs won't be used
        public static uint twins;//how many ML you are running parallel
    }
    public ML(int parainputsize,int paraoutputsize,uint paratwins)
    {
        interaction.twins = paratwins;
        input_size= parainputsize;
        input = new byte[input_size];
        default_values_bool_array = new bool[input_size];
        for (int i = 0; i < input_size; i++)
        {
            defaultarrays.bools[i] = false;
        }
        for (int i = 0; i < output_size; i++)
        {
            defaultarrays.baseweight[i] = 1;
        }

        output_size = paraoutputsize;
        input_size_sq = (byte)Math.Pow(2, input_size);
    }
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
    public int decision(byte[] parabytearray, int paraint)
    {
        //inizialize to make sure that parallels work!
        interaction.stuck = false;
        //Make the active memory
        ulong inputlong = bytestoulong(parabytearray);//theinput in long form
        ulong[] weights = defaultarrays.baseweight;//we make the base array to modify

        bool nonactive_repeat = active_memory[paraint].TryAdd(inputlong,default_values_bool_array);//we add the inputlong into active memory so that later we can change it out
        if (!nonactive_repeat&&settings.makesamechoicetwice)//if it has been there before AND the settings keep it from making the same choice twice.
        {
            bool[] tempboolarray = active_memory[paraint][inputlong];
            for(uint i = 0; i < output_size; i++)
            {
                if (tempboolarray[i])
                {
                    weights[i] = 0;
                }
            }
        }//end same choiche check


        for(byte i=0; i <input_size_sq; i++)//search the whole ulong dictionary
        {
            ulong[] tempulong = learned_long[bytemask(i,inputlong)].values;
            //no 0 checker so this will 100% return errors!
            for(uint ii = 0; ii < output_size; ii++) //build up the weight
            {
                weights[ii] = weights[ii] * tempulong[ii];
            }
            uint biggestdivider = bigdivider(weights);
            for (uint ii = 0; ii < output_size; ii++)
            {
                weights[ii] = weights[ii]/biggestdivider ;
            }
        }//end searching the whole ulong dictionary

        //gcd(a, b, c) = gcd(a, gcd(b, c)) = gcd(gcd(a, b), c) = gcd(gcd(a, c), b).
        //do need definetly figure this out

        for (byte i = 0; i < input_size-1; i++)
        {
            for(byte ii = (byte)(i + 1); ii < input_size; ii++)
            {
                for(byte iii = 0; iii < input_size; iii++)
                {
                    if (iii != i && iii != ii)
                    {
                        if (input[i] != 0 && input[ii] != 0 && input[iii]!=0)
                        {

                        }
                    }
                }
            }
        }//end comparison calculator
        /**
        int x = 0;
        byte tempcomparebyte = 3;

        while (tempcomparebyte != 0)
        {
            x = difference(tempcomparebyte, 0, parabytearray);
            switch (tempcomparebyte)//it never breaks out of here
            {
                case 192: //1100 0000
                    tempcomparebyte = 5;//0000 0101
                    break;
                case 160://1010 0000
                    tempcomparebyte = 9;
                    break;
                case 144: //1001 0000
                    tempcomparebyte = 17;
                    break;
                case 136://1000 1000
                    tempcomparebyte = 33;
                    break;
                case 132://1000 0100
                    tempcomparebyte = 65;
                    break;
                case 130://1000 0010
                    tempcomparebyte = 129;
                    break;
                case 129:
                    tempcomparebyte = 0;
                    break;
                default:
                    tempcomparebyte = (byte)(tempcomparebyte << 1);
                    break;
            }
        } //this entire thing is wortheless
        **/

        //okay so what if instead of adding up all the options weights 
        //we grab the biggest one and set that as 1 and the rest as divided by that and then we add all those numbers together
        //and we use that number (float) to calculate the random number
        return 0;
    }
    private uint bigdivider(ulong[]parabytearray) {
        //gcd(a, b, c) = gcd(a, gcd(b, c)) = gcd(gcd(a, b), c) = gcd(gcd(a, c), b).
        //remember make exception for arrayvalues of 0
        



        return 0;
    }
    private ulong[] differencecalculator(byte paracompare1, byte paracompare2, byte paracompare3)
    {
        ulong[] returnme = new ulong[output_size];
        //just make the int
        return returnme;
    }
    private int difference(byte parabytemask, byte parasecondarycircumstance, byte[] parabytearray)//parabyte stands for which two are compared from the bytearray
    {
        int returnme = (parabytemask<<24);
        byte byte1 = 0;
        byte byte2 = 0;
        bool tempsearchfirst = true;
        byte tempi = 0;
        byte tempii = 0;
        while (tempsearchfirst)
        {
            if (parabytemask - 128 >= 0)
            {
                byte1 = parabytearray[tempi];
                tempsearchfirst = false;                
                tempii = (byte)(tempi+1);
            }
            tempi++;
            parabytemask =(byte) (parabytemask << 1);
        }
        while(!tempsearchfirst)
        {
            if (parabytemask - 128 >= 0)
            {
                byte2 = parabytearray[tempii];
                tempsearchfirst = true;
            }
            tempii++;
            parabytemask = (byte)(parabytemask << 1);

        }//we now have the 2 we need to compare
         //TODO the third one
        Console.WriteLine(byte1+" "+byte2);
        //       Console.WriteLine(byte1+" "+byte2);
        if (byte1 > byte2)
        {





        } else if(byte1 < byte2)
        {

        }
        return returnme;
    }
    private ulong[] ML_choice_calculator_aid(long paralong) //de paralong is all the inputs combined into 1
    {
        ulong[] returnme = new ulong[output_size];
            for(int i=0;i < output_size; i++)
        {
            returnme[i] = 1;
        }
        for (byte i = 1; i <= input_size_sq; i++) //to check all the code
        {
            memory x = learned_long[bytemask(i, (ulong)paralong)];
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

    private ulong bytemask(byte parabyte, ulong paralong) //see the paralong as an array of bytes, 8 of them, and the parabyte as mask to show which bytes are returned
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
    private ulong bytestoulong(byte[] parabytearray)
    {
        ulong returnme = 0;
        for (int i = 0; i < input_size; i++)
        {
            returnme = returnme << 8;//could be more minmaxxed like bytemask
            returnme = returnme + parabytearray[i];
        }
        return returnme;
    }
    private void addhistory()//TODO
    {
        //create memory cells then adds them
    }
    public void writehistory ()//TODO
    {
        //make it write the entire dictionary in a .txt file in a way that copy/paste could make it functional
    }

}



//copyright owned by BlossomStudio