using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.SymbolStore;
using System.Reflection.Metadata.Ecma335;

public class GPAI
{
    private static ulong EXP = 0;//elke keer dat een waarde word aangepast word het verschil hierbij opgeteld
    public ulong XP()
    {//this way you can't set EXP
        return EXP;
    }
    public static string Version()
    {
        return "Alpha 0.8";
    }
    public string HELP()
    {
        //I know this is absolutely helpless
        return "Please first take a look at the comments and otherwise visit https://github.com/StudentBlossom/GeneralPurposeAI for any help";
    }
    //de EXP is niet nodig maar geeft een rough idea van hoeveel de AI al gelearnd heeft. hangt af van de gefindetunde waardes
    private static string credits = "BlossomStudio";
    private Random rng = new Random();
    public byte[] input;
    public static int input_size;//max is 8
    public static int input_size_sq;
    public static int output_size;
    private Dictionary<ulong, memory> learned_long = new Dictionary<ulong, memory>();
    private Dictionary<uint, memory> learned_short = new Dictionary<uint, memory>();//I know the name should have been learned_int but this is funnier
    private static Dictionary<ulong, bool[]>[] active_memory_long;//de bool staat voor welke keuzes genomen                                                                  //we make it an array so that we can work parallels
    private static Dictionary<uint, bool[]>[] active_memory_short;//de bool staat voor welke keuzes genomen                                                                  //we make it an array so that we can work parallels
    public void initialiazer()
    {
        for (int i = 0; i < output_size; i++)
        {
            defaultarrays.weight[i] = settings.finetune.start;
        }
        for (int i = 0; i < output_size; i++)
        {
            defaultarrays.bools[i] = false;
        }
        for (int i = 0; i < output_size; i++)
        {
            defaultarrays.baseweight[i] = 1;
        }
        for (int i = 0; i < interaction.twins; i++)
        {
            active_memory_long[i] = new Dictionary<ulong, bool[]>();
            active_memory_short[i] = new Dictionary<uint, bool[]>();
        }

    }
    private static class defaultarrays
    {
        public static bool[] bools;
        public static ulong[] weight;
        public static double[] baseweight;
    }
    public static class settings
    {
        public static class finetune
        {
            public static ulong start = 50; //de start value van alle waarden
            public static int punish = 1; //hoeveel het getal omlaag gaat als faal
            public static int reward = 5; //hoeveel het getal omhoog gaat als goed
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
    public GPAI(int parainputsize, int paraoutputsize, uint paratwins)
    {
        interaction.twins = paratwins;
        defaultarrays.weight = new ulong[paraoutputsize];
        defaultarrays.baseweight = new double[paraoutputsize];
        defaultarrays.bools = new bool[paraoutputsize];
        active_memory_long = new Dictionary<ulong, bool[]>[paratwins];//de bool staat voor welke keuzes genomen 
        active_memory_short = new Dictionary<uint, bool[]>[paratwins];//de bool staat voor welke keuzes genomen 
        input_size = parainputsize;
        input = new byte[input_size];
        output_size = paraoutputsize;
        input_size_sq = (byte)(Math.Pow(2, input_size) - 1);//I am so sorry
                                                            //hours wasted: 6
    }
    private class memory//check if this is the best way or if I need to change 
                        //the construct class to take a int[] as para so I can change it. idk this is a dictionary quirk
    {
        public byte maskID;
        public ulong[] values;//maybe use maskID and internal value to see if it is a legit or not (no 0 where it should not be 0)
                              //maybe change values to uint instead?
        public memory(byte parabyte, ulong paralong)
        {
            maskID = parabyte;
            values = new ulong[output_size];
            for (int i = 0; i < output_size; i++)
            {
                values[i] = settings.finetune.start;
            }
        }
    }
    public int decision(byte[] parabytearray, int paraint)
    {
        //reset values
        interaction.stuck = false;

        //search the whole ulong dictionary
        ulong inputlong = bytestoulong(parabytearray);//theinput in long form
        double[] weights = defaultarrays.baseweight;//we make the base array to modify
        bool nonactive_repeat = active_memory_long[paraint].TryAdd(inputlong, defaultarrays.bools);//we add the inputlong into active memory so that later we can change it out

        if (!nonactive_repeat && !settings.makesamechoicetwice) {//same choice check
            bool[] tempboolarray = active_memory_long[paraint][inputlong];
            for (uint i = 0; i < output_size; i++)
            {
                if (tempboolarray[i])
                {
                    weights[i] = 0;
                }
            }
        }//end same choiche check

        ulong[] saved_generated_ulong = new ulong[input_size_sq];

        for (byte i = 1; i <= input_size_sq && i != 0; i++)//search the whole ulong dictionary with every combination
        {
            //it is <= to deal with the 111111 when input is 8 but after that it goes back to 0 WHICH IS BAD
            //so. what we do is simple we add a !=0 
            //this does not work when input is 1 I think?
            ulong tempUlongbytemasked = bytemask(i, inputlong);
            saved_generated_ulong[i - 1] = tempUlongbytemasked;//we save all the generated so we don't need to do it again later
            memory tempmemory;
            bool tempnew = learned_long.TryGetValue(tempUlongbytemasked, out tempmemory);
            if (tempnew){
                ulong[] tempulong = tempmemory.values;
                //no 0 checker so this will 100% return wrong values!
                weights = addweights(weights, tempulong);
            }else{
                learned_long.Add(tempUlongbytemasked, new memory(i, bytemask(i, inputlong)));
            }

        }//end searching the whole ulong dictionary

        for (byte i = 0; i < input_size - 1; i++)//calculate all comparisons
        {//full explenation at max indent
            uint tempfirstbyte = (uint)1 << (24 + i);//the first representing bit
            for (byte ii = (byte)(i + 1); ii < input_size; ii++)
            {
                uint tempsecondbyte = (uint)1 << (24 + ii);//the second represeting bit
                uint tempdatabyte1 = 0;
                uint tempfourthbyte = 0;
                if (parabytearray[i] > parabytearray[ii])//which of the 2 previous is the fourth representing bit
                {
                    tempfourthbyte = (uint)1 << (16 + i);
                    tempdatabyte1 = (uint)((parabytearray[i] - parabytearray[ii]) << 8);
                }
                else
                {
                    tempdatabyte1 = (uint)((parabytearray[ii] - parabytearray[i]) << 8);
                    tempfourthbyte = (uint)1 << (16 + ii);
                }
                uint tempcompiled = tempfirstbyte + tempfourthbyte + tempsecondbyte;
                active_memory_short[paraint].TryAdd(tempcompiled, defaultarrays.bools);
                //handle the int here!!!!!
                weights = addweights(weights, intweight(tempcompiled));//we first deal with it being general (so x>y y<x)
                tempcompiled += +tempdatabyte1;
                active_memory_short[paraint].TryAdd(tempcompiled, defaultarrays.bools);
                weights = addweights(weights, intweight(tempcompiled)); //then we do and get the answer ontop of it
                for (byte iii = 0; iii < input_size; iii++)
                {
                    if (iii != i && iii != ii)
                    {
                        if (parabytearray[i] != 0 && parabytearray[ii] != 0 && parabytearray[iii] != 0)
                        {
                            uint tempthirdbyte = (uint)1 << (16 + iii);//third represented bit
                            uint tempdatabyte2 = parabytearray[iii];
                            tempcompiled = tempcompiled + tempthirdbyte + tempdatabyte2;//IT WORKS
                            active_memory_short[paraint].TryAdd(tempcompiled, defaultarrays.bools);
                            weights = addweights(weights, intweight(tempcompiled));
                            /*
                            /!\ INFO DUMP
                            
                            So lets imagine the int as 4 bytes
                            We use the first 2 bytes to store information about the second 2 bytes

                            in the first byte we have 2 positive bits. they represent which of the 2 inputs are being compared
                            so a positive bit on the third position means the third bit is represented
                            NOW because a byte cannot be negative we have to know which of the 2 bytes is infact the bigger one
                            2-1 =1 and 2-1 = -1 (let's not delve into the problems it could cause)
                            Now how do we solve this issue? We store a bit in the second byte. 
                            So the second byte has a positive bit that is in the same position as one of the first bytes
                            That represted bit is the bigger value!
                            
                            now after we handle that we add a second positive bit.
                            Again that bit represents a value of input, so again the second bit is positive it means the second input\
                            

                            The last 2 bytes are the easiest. the first byte is the difference of the 2 inputs that are represented in the first byte
                            with x-y or y-x depending on the positive bit in the second byte that is also positive in the first byte
                            Now we add to the last byte is simple an input
                            We know which one due to the other positive bit in the second byte.

                            THis means that every combination is represented but also individual. And because x-y=0 is also that
                            And because of the first few bits the numbers it generates is always unique!!!!!
                             */
                        }
                    }
                }
            }
        }//end comparison calculator
         //calculate which choice
        bool has_no_option = true;

        //before we calculate a choiche we have to make sure we can make a choice (it's not all 0)
        for (uint i = 0; i < output_size; i++)
        {//we check to make sure there is at least one option
            Console.WriteLine(weights[i]);
            if (weights[i] > 0)
            {
                has_no_option = false;
            }
        }

        if (has_no_option)
        {//if it has no options it returns 0 
            return 0;
        } else
        {
            //we know the total value is 0
            //we choose a random number between 1 and 0
            //when tempsum is bigger then we know the option selected
            int temptotalsofar = 1;
            double tempsum = weights[0];
            double randomoption = rng.NextDouble();

            while (tempsum <= randomoption)//we grab a random point between 0 and 1
            {//and we check in which region it lays
                tempsum += weights[temptotalsofar];
                temptotalsofar++;
            }
            //out of bounds?
            //one of the generated lengths of the underthing is infact wrongly initialized
            active_memory_long[paraint][inputlong][temptotalsofar - 1] = true;//remember we have made this choice before!

            bool[] tempbool = defaultarrays.bools;
            tempbool[temptotalsofar - 1] = true;
            for (int i = 0; i < input_size_sq; i++)
            {//add all generated to active memory so that we can use it when applying learning.
                if (!active_memory_long[paraint].TryAdd(saved_generated_ulong[i], tempbool))
                {//and when it is already added, change the option taken to true
                 
                    active_memory_long[paraint][saved_generated_ulong[i]][temptotalsofar - 1] = true;
                } 
            }
            //we need to do something so that we can set all the saved to true
            //
            //           foreach (KeyValuePair<uint, bool[]> i in active_memory_short[paraint])
            //           {//this does infact not work
            //               Console.WriteLine(i.Value[temptotalsofar - 1]);
            //               i.Value[temptotalsofar - 1] = true;
            //           }
            return temptotalsofar;//this is the return option.
        }
    }
    private ulong[]  intweight(uint parauint){
        bool tempbool = learned_short.TryAdd(parauint, new memory(0, (ulong)parauint));//we check if it is already in memory
        if (tempbool)
        {
            //if it is not we return default
            return defaultarrays.weight;
        } else
        {
            //if it is we return the stored values
            return learned_short[parauint].values;
        }
    }
    private double[] addweights(double[] paradoubles, ulong[] paraulongs)
        {//this concept was designed in collaboration with Olivia
        //it adds all the current weights, then divides it by the total so it basically becomes a percentage
        //this works, will lose small number details but in general it will work!
            double temptotal = 0;
            for (int i = 0; i < output_size; i++)
            {
                paradoubles[i] = paradoubles[i] * paraulongs[i];
                temptotal += paradoubles[i];
            }
            if (temptotal == 0)
            {
                return paradoubles;
            } else { 
                for (int i = 0; i < output_size; i++)
                {
                    paradoubles[i] = paradoubles[i] / temptotal;
                }
                return paradoubles;
            }
        }
        public void memorytofile()
    {




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
        public void feedback (bool parasuccess,int paratwinnumber, float paramultiplier)
    {
        if (parasuccess)
        {
            foreach (KeyValuePair<ulong, bool[]> kvp in active_memory_long[paratwinnumber])
            {
                for (int i = 0; i < output_size; i++)
                {
                    if (kvp.Value[i])
                    {//THIS DOES NOT WORK????????

                        ulong temppreulong = learned_long[kvp.Key].values[i];
                        ulong temppostulong = temppreulong + (ulong)(settings.finetune.reward*paramultiplier);
                        if (temppostulong >= temppreulong)//if it doesn't overflows 
                        {
                            learned_long[kvp.Key].values[i] = temppostulong;
                        }
                        else//it has overflown!
                        {
                            learned_long[kvp.Key].values[i] = ulong.MaxValue;
                        }
                        //we add the reward
                        //now we check to make sure it has not looped around
                    }
                }
            }
        }
        else
        {
            foreach (KeyValuePair<ulong, bool[]> kvp in active_memory_long[paratwinnumber])
            {
                for (int i = 0; i < output_size; i++)
                {
                    if (kvp.Value[i])
                    {//WHY ARE THE VALUES WRONG OH IT IS 
                        ulong temppreulong = learned_long[kvp.Key].values[i];
                        ulong temppostulong = temppreulong - (ulong)(settings.finetune.punish * paramultiplier);
                        if (temppostulong <= temppreulong)//if it does not underflow to positive
                        {
                            learned_long[kvp.Key].values[i] = temppostulong;
                        }
                        else//it has underflown
                        {
                            learned_long[kvp.Key].values[i] = 1;
                        }
                        kvp.Value[i] = false;//WHY IS THIS WORKING?
                    }
                }
            }
        }//end else
        active_memory_long[paratwinnumber].Clear();
        active_memory_short[paratwinnumber].Clear();
    }
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
    }

  