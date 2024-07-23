﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class ML
    {
        private static ulong EXP = 0;//elke keer dat een waarde word aangepast word het verschil hierbij opgeteld
        public ulong XP()
        {//this way you can't set EXP
            return EXP;
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
        private static Dictionary<ulong, bool[]>[] active_memory;//de bool staat voor welke keuzes genomen                                                                  //we make it an array so that we can work parallels
        public void initialiazer()
        {
            for (int i = 0; i < output_size; i++)
            {
                defaultarrays.weight[i] = settings.finetune.start;
            }
            for (int i = 0; i < input_size; i++)
            {
                defaultarrays.bools[i] = false;
            }
            for (int i = 0; i < output_size; i++)
            {
                defaultarrays.baseweight[i] = 1;
            }
            for (int i = 0; i < interaction.twins; i++)
            {
                active_memory[i] = new Dictionary<ulong, bool[]>();
            }

        }
        private static class defaultarrays
        {
            public static bool[] bools;
            public static ulong[] weight;
            public static double[] baseweight;
        }
        internal static class settings
        {
            internal static class finetune
            {
                public static ulong start = 50; //de start value van alle waarden
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
        public ML(int parainputsize, int paraoutputsize, uint paratwins)
        {
            interaction.twins = paratwins;
            defaultarrays.weight = new ulong[paraoutputsize];
            defaultarrays.baseweight = new double[paraoutputsize];
            defaultarrays.bools = new bool[paraoutputsize];
            active_memory = new Dictionary<ulong, bool[]>[paratwins];//de bool staat voor welke keuzes genomen 
            input_size = parainputsize;
            input = new byte[input_size];
            output_size = paraoutputsize;
            input_size_sq = (byte)Math.Pow(2, input_size);
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
                    values[i] = ML.settings.finetune.start;
                }
            }
        }
        public int decision(byte[] parabytearray, int paraint)
        {
            //inizialize to make sure that parallels work!
            interaction.stuck = false;


            //Make the active memory
            //search the whole ulong dictionary
            ulong inputlong = bytestoulong(parabytearray);//theinput in long form
            double[] weights = defaultarrays.baseweight;//we make the base array to modify
            bool nonactive_repeat = active_memory[paraint].TryAdd(inputlong, defaultarrays.bools);//we add the inputlong into active memory so that later we can change it out

            if (!nonactive_repeat && settings.makesamechoicetwice)//if it has been there before AND the settings keep it from making the same choice twice.
            {//THIS IS UNTESTED!
                bool[] tempboolarray = active_memory[paraint][inputlong];
                for (uint i = 0; i < output_size; i++)
                {
                    if (tempboolarray[i])
                    {
                        weights[i] = 0;
                    }
                }
            }//end same choiche check
            for (byte i = 1; i < input_size_sq; i++)//search the whole ulong dictionary
            {
                memory tempmemory;
                bool tempnew = learned_long.TryGetValue(bytemask(i, inputlong), out tempmemory);
                if (tempnew)
                {
                    ulong[] tempulong = tempmemory.values;
                    //no 0 checker so this will 100% return wrong values!
                    weights = addweights(weights, tempulong);
                }
                else
                {
                    learned_long.Add(bytemask(i, inputlong), new memory(i, bytemask(i, inputlong)));
                }

            }//end searching the whole ulong dictionary
            for (byte i = 1; i < input_size - 1; i++)//calculate all comparisons
            {
                for (byte ii = (byte)(i + 1); ii < input_size; ii++)
                {
                    for (byte iii = 0; iii < input_size; iii++)
                    {
                        if (iii != i && iii != ii)
                        {
                            if (input[i] != 0 && input[ii] != 0 && input[iii] != 0)
                            {

                            }
                        }
                    }
                }
            }//end comparison calculator
             //calculate which choice
            bool has_no_option = true;
            for(uint i = 0; i < output_size; i++)
            {//we check to make sure there is at least one option
                if (weights[i] > 0)
                {
                    has_no_option= false;
                }
            }
            if(has_no_option)
            {//if it has no options it returns 0 
                return 0;
            } else
            {
                int temptotalsofar = 1;
            double tempsum = weights[0];
                double randomoption = rng.NextDouble();
                while (tempsum <= randomoption)//we grab a random point between 0 and 1
                {//and we check in which region it lays
                    tempsum += weights[temptotalsofar];
                    temptotalsofar++;
                }
                    return temptotalsofar;
            }
        }
        private double[] addweights(double[] paradoubles, ulong[] paraulongs)
        {//this concept was designed in collaboration with Olivia
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


        private uint bigdivider(ulong[] parabytearray)
        {
            //gcd(a, b, c) = gcd(a, gcd(b, c)) = gcd(gcd(a, b), c) = gcd(gcd(a, c), b).
            //remember make exception for arrayvalues of 0




            return 1;
        }
        private ulong[] differencecalculator(byte paracompare1, byte paracompare2, byte paracompare3)
        {
            ulong[] returnme = new ulong[output_size];
            //just make the int
            return returnme;
        }
        private int difference(byte parabytemask, byte parasecondarycircumstance, byte[] parabytearray)//parabyte stands for which two are compared from the bytearray
        {
            int returnme = (parabytemask << 24);
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
                    tempii = (byte)(tempi + 1);
                }
                tempi++;
                parabytemask = (byte)(parabytemask << 1);
            }
            while (!tempsearchfirst)
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
            Console.WriteLine(byte1 + " " + byte2);
            //       Console.WriteLine(byte1+" "+byte2);
            if (byte1 > byte2)
            {





            }
            else if (byte1 < byte2)
            {

            }
            return returnme;
        }
        private ulong[] ML_choice_calculator_aid(long paralong) //de paralong is all the inputs combined into 1
        {
            ulong[] returnme = new ulong[output_size];
            for (int i = 0; i < output_size; i++)
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
        public void writehistory()//TODO
        {
            //make it write the entire dictionary in a .txt file in a way that copy/paste could make it functional
        }

    }

  