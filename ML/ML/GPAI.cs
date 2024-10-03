using System.Reflection;

public class GPAI
{
    private static ulong EXP = 0;//elke keer dat een waarde word aangepast word het verschil hierbij opgeteld
    public ulong XP()
    {//this way you can't set EXP
        return EXP;
    }
    public const string version = "1.1";
    public const byte savedatacompatability = 3;
    public string HELP()
    {
        //I know this is absolutely helpless
        return "Please first take a look at the comments and otherwise visit https://github.com/StudentBlossom/GeneralPurposeAI for any help";
    }
    //de EXP is niet nodig maar geeft een rough idea van hoeveel de AI al gelearnd heeft. hangt af van de gefindetunde waardes
    public const string credits = "BlossomStudio";
    private Random rng = new Random();
    public byte[] input;
    public static int input_size;//max is 8
    public static int input_size_sq;
    public static int output_size;
    private static uint comparisons_amount;
    private static ulong[] previous_exact_input;
    private static int[] previous_exact_output;
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
            public static ulong start = 1000; //de start value van alle waarden
            public static int punish_static = 10; //hoeveel het getal omlaag gaat als faal
            public static int reward_static = 30; //hoeveel het getal omhoog gaat als goed
            public static int punish_relative = 20; //hoeveel het getal omlaag gaat als faal
            public static int reward_relative = 50; //hoeveel het getal omhoog gaat als goed
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
        previous_exact_input = new ulong[paratwins];
        previous_exact_output = new int[paratwins];
        active_memory_long = new Dictionary<ulong, bool[]>[paratwins];//de bool staat voor welke keuzes genomen 
        active_memory_short = new Dictionary<uint, bool[]>[paratwins];//de bool staat voor welke keuzes genomen 
        input_size = parainputsize;
        input = new byte[input_size];
        output_size = paraoutputsize;
        comparisons_amount = (uint)(((parainputsize * (parainputsize + 1)) / 2) * (parainputsize - 2) + parainputsize);//sum of input * input-2 + input
        input_size_sq = (byte)(Math.Pow(2, input_size) - 1);//I am so sorry
                                                            //hours wasted: 6
    }
    private class memory//necessary for the donkey principal. I cannot do that any different!!!!
    {
        public byte maskID;
        public bool[] donkey;
        public ulong[] values;//maybe use maskID and internal value to see if it is a legit or not (no 0 where it should not be 0)
        public memory(byte parabyte)
        {
            maskID = parabyte;
            values = new ulong[output_size];
            donkey = new bool[output_size];
            for (int i = 0; i < output_size; i++)
            {
                values[i] = settings.finetune.start;
                donkey[i] = false;
            }
        }
    }
    public int decision(byte[] parabytearray, int paraint)
    {
        //reset values
        interaction.stuck = false;

        //search the whole ulong dictionary
        bool[] boolarrayfalse = new bool[output_size];
        double[] weights = new double[output_size];
        defaultarrays.bools.CopyTo(boolarrayfalse, 0);
        defaultarrays.baseweight.CopyTo(weights, 0);//we make the base arrays to modify
        ulong inputlong = bytestoulong(parabytearray);//theinput in long form
        bool nonactive_repeat = active_memory_long[paraint].TryAdd(inputlong, boolarrayfalse);//we add the inputlong into active memory, IF it is already there we know we need to check makesamechoicetwice
        previous_exact_input[paraint] = inputlong;

        if (!nonactive_repeat && !settings.makesamechoicetwice)
        {//same choice check
            bool[] tempboolarray = active_memory_long[paraint][inputlong];
            for (uint i = 0; i < output_size; i++)
            {
                if (tempboolarray[i])
                {
                    weights[i] = 0;
                }
            }
        }//end same choiche check

        //we use the saved_generated_ulong to save all the generated ulongs(duh) so that later on we can use it after we made a decision before we return it so that we add it to active memory. does not work with 0
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
            if (tempnew)
            {
                ulong[] tempulong = tempmemory.values;
                //no 0 checker so this will 100% return wrong values!
                weights = addweights(weights, tempulong);
            }
            else
            {
                learned_long.Add(tempUlongbytemasked, new memory(i));
            }
        }//end searching the whole ulong dictionary

        uint[] saved_generated_uint = new uint[comparisons_amount];//The length is 
        uint saved_generated_uint_counter = 0;
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
                else//add a thing for if they are equal?
                {
                    tempdatabyte1 = (uint)((parabytearray[ii] - parabytearray[i]) << 8);
                    tempfourthbyte = (uint)1 << (16 + ii);
                }
                uint tempcompiled = tempfirstbyte + tempfourthbyte + tempsecondbyte;//compile the data
                                                                                    //save the compiled data
                saved_generated_uint[saved_generated_uint_counter] = tempcompiled;
                saved_generated_uint_counter++;
                //use the compiled data
                weights = addweights(weights, intweight(tempcompiled));//we first deal with it being general (so x>y y<x)
                tempcompiled += +tempdatabyte1;//add more data to the compiled data
                                               //save the recompiled data
                saved_generated_uint[saved_generated_uint_counter] = tempcompiled;
                saved_generated_uint_counter++;
                //use the recompiled data
                weights = addweights(weights, intweight(tempcompiled)); //then we do and get the answer ontop of it
                for (byte iii = 0; iii < input_size; iii++)
                {
                    if (iii != i && iii != ii)
                    {
                        if (parabytearray[i] != 0 && parabytearray[ii] != 0 && parabytearray[iii] != 0)
                        {
                            uint tempthirdbyte = (uint)1 << (16 + iii);//third represented bit
                            uint tempdatabyte2 = parabytearray[iii];
                            //save the info/bits/data
                            tempcompiled = tempcompiled + tempthirdbyte + tempdatabyte2;
                            saved_generated_uint[saved_generated_uint_counter] = tempcompiled;
                            saved_generated_uint_counter++;
                            //recompile data and save it
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

                            now after we handle that we add a second positive bit to the second byte.
                            Again that bit represents a value of input, so again the second bit is positive it means the second input

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
            if (weights[i] > 0)
            {
                has_no_option = false;
            }
        }
        if (has_no_option)
        {//if it has no options it returns 0 
            return 0;
        }
        else
        {
            //we know the total value is 0
            //we choose a random number between 1 and 0
            //when tempsum is bigger then we know the option selected
            int decisiontaken = 1;
            double tempsum = weights[0];
            double randomoption = rng.NextDouble();

            while (tempsum <= randomoption)//we grab a random point between 0 and 1
            {//and we check in which region it lays
                tempsum += weights[decisiontaken];
                decisiontaken++;
            }
            active_memory_long[paraint][inputlong][decisiontaken - 1] = true;//remember we have made this choice before!
            boolarrayfalse[decisiontaken - 1] = true;

            for (int i = 0; i < input_size_sq; i++)
            {//add all generated to active memory so that we can use it when applying learning.
                bool[] tempboolarray = new bool[output_size];//we copy the pregenerated array with the taken choice already being true.
                boolarrayfalse.CopyTo(tempboolarray, 0);
                if (!active_memory_long[paraint].TryAdd(saved_generated_ulong[i], tempboolarray))
                {//and when it is already added, change the option taken to true
                    active_memory_long[paraint][saved_generated_ulong[i]][decisiontaken - 1] = true;
                }
            }
            for (int i = 0; i < comparisons_amount; i++)
            {//this is the exact same as the thing above but instead for the ints
                bool[] tempboolarray = new bool[output_size];
                boolarrayfalse.CopyTo(tempboolarray, 0);
                if (!active_memory_short[paraint].TryAdd(saved_generated_uint[i], tempboolarray))
                {
                    active_memory_short[paraint][saved_generated_uint[i]][decisiontaken - 1] = true;
                }
            }
            previous_exact_output[paraint] = decisiontaken;
            return decisiontaken;//this is the return option.
        }
    }
    private ulong[] intweight(uint parauint)
    {
        bool tempbool = learned_short.TryAdd(parauint, new memory(0));//we check if it is already in memory
        if (tempbool)
        {
            //if it is not we return default
            return defaultarrays.weight;
        }
        else
        {
            //if it is we return the stored values
            return learned_short[parauint].values;
        }
    }
    private double[] addweights(double[] paradoubles, ulong[] paraulongs)
    {//this concept was designed in collaboration with Olivia
     //it adds all the current weights, then divides it by the total so it basically becomes a percentage
     //this works, will lose small number details but in general it will work!
     //please don't ask me about this code or calculations it makes. I did a few test runs, it seems to be accurate, I do not understand it, at all.
        double temptotal = 0;
        for (int i = 0; i < output_size; i++)
        {
            paradoubles[i] = paradoubles[i] * paraulongs[i];
            temptotal += paradoubles[i];
        }
        if (temptotal == 0)
        {
            return paradoubles;
        }
        else
        {
            for (int i = 0; i < output_size; i++)
            {
                paradoubles[i] = paradoubles[i] / temptotal;
            }
            return paradoubles;
        }
    }
    public void memorytofile_debugreadable()
    {

        // Change this 
        string docPath =
Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Substring(0, 60);//BEWARE THIS ONLY WORKS FOR ME BLOSSOM ALWAYS
                                                                                 //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        Console.WriteLine(docPath);

        // Write the string array to a new file named "WriteLines.txt".
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "GPAImemoryreadable.txt")))
        {
            foreach (KeyValuePair<ulong, memory> kvp in learned_long)
            {
                byte[] tempbytearray = BitConverter.GetBytes(kvp.Key);
                string tempboolwrittenout = "Donkey= ";
                string tempvalueswrittenout = "values=[";
                string tempbytearraywriteenout = "";
                for (int i = 0; i < output_size; i++)
                {
                    tempvalueswrittenout += kvp.Value.values[i] + ",";
                    if (kvp.Value.donkey[i])
                    {
                        tempboolwrittenout += "t";
                    }
                    else
                    {
                        tempboolwrittenout += "f";
                    }
                }
                for (int i = 0; i < input_size; i++)
                {
                    tempbytearraywriteenout += tempbytearray[i] + ",";
                }
                string templine = $"key=({kvp.Key}){tempbytearraywriteenout}  bitmask={Convert.ToString(kvp.Value.maskID, 2)}  {tempvalueswrittenout}]  {tempboolwrittenout}";
                outputFile.WriteLine(templine);
            }
            foreach (KeyValuePair<uint, memory> kvp in learned_short)
            {
                byte[] tempbytearray = BitConverter.GetBytes(kvp.Key);
                string tempboolwrittenout = "Donkey= ";
                string tempvalueswrittenout = "values=[";
                string tempbytearraywriteenout = "";
                for (int i = 0; i < output_size; i++)
                {
                    tempvalueswrittenout += kvp.Value.values[i] + ",";
                    if (kvp.Value.donkey[i])
                    {
                        tempboolwrittenout += "t";
                    }
                    else
                    {
                        tempboolwrittenout += "f";
                    }
                }
                for (int i = 0; i < input_size; i++)
                {
                    tempbytearraywriteenout += tempbytearray[i] + ",";
                }
                string templine = $"key=({kvp.Key}){tempbytearraywriteenout}  bitmask={Convert.ToString(kvp.Value.maskID, 2)}  {tempvalueswrittenout}]  {tempboolwrittenout}";
                outputFile.WriteLine(templine);
            }
        }
    }
    public void memorytofile(string parapath)
    {
        /**
        We start by creating a binary file
        We only save the memory cells (for now)
        and we start with ONLY the static since this is testing
        my idea is that we get an empty space of 1 to save: how many memory cells, and info about the version of the AI that created it
        and if possible input and output sizes. will have to do the math

        here is the size of 1 cell
        calculated in bytes

        8 bytes for the key
        1 byte for byte mask
        1 byte for booleans
        8*outputsize bytes

        10+8*outputsize bytes.?

        here is how it is written out
        8 bytes for the key. 1 byte for the bytemask.


        //we are going to assume 2 outputs at the minimum
        which leaves us with
        8+1+1+8+8 bytes total
        26 bytes!

        at the end there is 17 bytes of meta data.
        it is at the end because then when viewing in hex editor I can still see what is going on.

        **/
        // Change this 
        //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Write the string array to a new file named "WriteLines.txt".
        using (BinaryWriter outputFile = new BinaryWriter(File.Open(parapath, FileMode.Create)))
        {//use a binary writer to write a binary file.
            foreach (KeyValuePair<ulong, memory> kvp in learned_long)
            {//dump the entire long memory into the file
                outputFile.Write(kvp.Key);//we start with they key
                outputFile.Write(kvp.Value.maskID);//we then put in the mask
                int tempboolfaker = 0; ;
                for (int i = 0; i < output_size; i++)
                {
                    tempboolfaker = tempboolfaker << 1;//then we move it so that the bits represent the bools representing the outputs
                    outputFile.Write(kvp.Value.values[i]);//we then write the values 
                    if (kvp.Value.donkey[i])
                    {
                        tempboolfaker++;//if donkey is true put a positive bit
                    }
                }
                outputFile.Write((byte)tempboolfaker);
            }
            foreach (KeyValuePair<uint, memory> kvp in learned_short)
            {
                int tempboolfaker = 0;
                outputFile.Write(kvp.Key);
                outputFile.Write(kvp.Value.maskID);
                //                Console.WriteLine(Convert.ToString(, 2));
                //IK MIS STUK IN MIJN SAFEFILE?!
                for (int i = 0; i < output_size; i++)
                {
                    tempboolfaker = tempboolfaker << 1;
                    outputFile.Write(kvp.Value.values[i]);
                    if (kvp.Value.donkey[i])
                    {
                        tempboolfaker++;
                    }
                }
                outputFile.Write((byte)tempboolfaker);
            }
            outputFile.Write(input_size);
            outputFile.Write(output_size);
            outputFile.Write((long)learned_long.Count);//writes down the size of learned long
            outputFile.Write(savedatacompatability);
            outputFile.Close();
        }
    }
    public void readfile(string parapath, bool paraoverride)
    {
        //key=8 + mask 1 + 8*outputsizevalues + 1 donkey
        //in that order
        /*
         I am going to assume: 1 outputs minimum.
         but the first bytecounter amount is infact used for simply meta data
        assuming 1 output.
         * 
        //alright here is more important stuff to know:
        the first bytecounter will be meta info
        the first 4 bytes is the amount of inputs
        the second 4 bytes is amount of outputs
        then 8 bytes as a ulong to amount of static memory
        then 1 bytes as a memory compatability
        so 8+8+1 = 17 bytes of meta data.         
         */

        const int metadatabyteamount = 17;

        byte[] importedbytes = File.ReadAllBytes(parapath);//get the file
        if (importedbytes[importedbytes.Length - 1] != savedatacompatability)
        {
            Console.WriteLine("/!\\ WARNING SAVE DATA NOT COMPATIBLE. AI WILL ATTEMPT TO USE IT REGARDLESS!");
        }
        byte[] metadatastorage = new byte[8];
        Array.Copy(importedbytes, importedbytes.Length - metadatabyteamount, metadatastorage, 0, 8);
        ulong inputandoutput = bytestoulongreverse(metadatastorage);
        int importedinput = (int)inputandoutput;//the first half is input so 4 bytes
        int importedoutput_size = (int)(inputandoutput >> 32);//the second half is outputsize also 4 bytes
        ulong bytecounter = (ulong)(8 + 1 + (8 * importedoutput_size) + 1);//this is the amount of output size we have in bytes
        //we do +8 cause first 8 is the inputandoutput long the second is the ulong
        Array.Copy(importedbytes, importedbytes.Length - metadatabyteamount + 8, metadatastorage, 0, 8);
        ulong staticmemoryamount = bytestoulongreverse(metadatastorage) * (ulong)bytecounter;

        //TODO check the code if i needs to be an int or a ulong

        for (ulong i = 0; i < staticmemoryamount; i += bytecounter)//we know each part is the size of 1 bytecounter
        {
            byte[] allbytesofonememorycell = new byte[bytecounter];//we get only the relevant bytes
            Array.Copy(importedbytes, (int)i, allbytesofonememorycell, 0, (int)bytecounter);

            ulong[] tempkey = new ulong[importedoutput_size + 1];//a ulong array to store both the key and the values
            tempkey[0] = bytestoulongreverse(allbytesofonememorycell);//save the key
            for (int ii = 1; ii <= importedoutput_size; ii++)//we use this to get all the values
            {
                byte[] tempbytearray = new byte[8];
                Array.Copy(allbytesofonememorycell, 1 + ii * 8, tempbytearray, 0, 8);//we start at 1 cause 0=key and the +1 is cause of maskID byte
                tempkey[ii] = bytestoulongreverse(tempbytearray);
            }
            memory importedmemory = new memory(allbytesofonememorycell[8]);//create an empty memory
                                                                           //now we have to try add it 
            bool alreadyknown = learned_long.TryAdd(tempkey[0], importedmemory);//does it already exist?
            if (alreadyknown)//if it is new we copy it fully
            {
                for (int ii = 0; ii < importedoutput_size; ii++)
                {
                    learned_long[tempkey[0]].values[ii] = tempkey[ii + 1];

                }
            }
            else if (paraoverride)//if it is not new BUT we can override it, we override it
            {
                for (int ii = 0; ii < importedoutput_size; ii++)
                {
                    learned_long[tempkey[0]].values[ii] = tempkey[ii + 1];
                }
            }
        }

        bytecounter -= 4;//long wordt int dus -4 bytes 
        for (ulong i = (ulong)staticmemoryamount; i <= (ulong)(importedbytes.Length - metadatabyteamount - (long)bytecounter); i += (ulong)bytecounter)
        {
            byte[] allbytesofonememorycell = new byte[bytecounter];//we get only the relevant bytes
            Array.Copy(importedbytes, (long)i, allbytesofonememorycell, 0, (long)bytecounter);

            ulong[] tempkey = new ulong[importedoutput_size];//a ulong array to store both the key and the values
            for (int ii = 0; ii < importedoutput_size; ii++)//we use this to get all the values
            {
                byte[] tempbytearray = new byte[8];
                Array.Copy(allbytesofonememorycell, 5 + ii * 8, tempbytearray, 0, 8);//we start at 1 cause 0=key and the +1 is cause of maskID byte
                tempkey[ii] = bytestoulongreverse(tempbytearray);
            }
            //TODO update the comments here to reflect the 4 cause ints
            //nvm TODO actually fix this...

            byte[] tempbyteforkeyvalue = new byte[4];
            Array.Copy(importedbytes, (long)i, tempbyteforkeyvalue, 0, 4);



            uint tempkeyvalue = bytestouintreverse(tempbyteforkeyvalue);
            memory importedmemory = new memory(allbytesofonememorycell[4]);//create an empty memory
                                                                           //now we have to try add it 
            bool alreadyknown = learned_short.TryAdd(tempkeyvalue, importedmemory);//does it already exist?
            if (alreadyknown)//if it is new we copy it fully
            {
                for (int ii = 0; ii < importedoutput_size; ii++)
                {
                    learned_short[tempkeyvalue].values[ii] = tempkey[ii];

                }
            }
            else if (paraoverride)//if it is not new BUT we can override it, we override it
            {
                for (int ii = 0; ii < importedoutput_size; ii++)
                {
                    learned_short[tempkeyvalue].values[ii] = tempkey[ii];
                }
            }

        }
    }
    public void feedback(bool parasuccess, int paratwinnumber, float paramultiplier)
    {
        //only the first one will be explained as the rest is the same 
        if (parasuccess)
        {
            foreach (KeyValuePair<ulong, bool[]> kvp in active_memory_long[paratwinnumber])
            {//we walk through the active memory
                for (int i = 0; i < output_size; i++)
                {
                    if (kvp.Value[i])
                    {
                        //after we find a value we have to check every step that has been taken with those specific circumstance
                        ulong temppreulong = learned_long[kvp.Key].values[i];
                        ulong temppostulong = temppreulong + (ulong)(settings.finetune.reward_static * paramultiplier);
                        if (temppostulong >= temppreulong)//if it doesn't overflows 
                        {
                            EXP += (ulong)(settings.finetune.reward_static * paramultiplier);
                            learned_long[kvp.Key].values[i] = temppostulong;//then we simply add it. 
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
            foreach (KeyValuePair<uint, bool[]> kvp in active_memory_short[paratwinnumber])
            {
                for (int i = 0; i < output_size; i++)
                {
                    if (kvp.Value[i])
                    {
                        ulong temppreulong = learned_short[kvp.Key].values[i];
                        ulong temppostulong = temppreulong + (ulong)(settings.finetune.reward_relative * paramultiplier);
                        if (temppostulong >= temppreulong)//if it doesn't overflows 
                        {
                            EXP += (ulong)(settings.finetune.reward_relative * paramultiplier);
                            learned_short[kvp.Key].values[i] = temppostulong;
                        }
                        else//it has overflown!
                        {
                            learned_short[kvp.Key].values[i] = ulong.MaxValue;
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
                    {
                        ulong temppreulong = learned_long[kvp.Key].values[i];
                        ulong temppostulong = temppreulong - (ulong)(settings.finetune.punish_static * paramultiplier);

                        if (temppostulong <= temppreulong)//if it does not underflow to positive
                        {
                            EXP += (ulong)(settings.finetune.punish_static * paramultiplier);

                            learned_long[kvp.Key].values[i] = temppostulong;
                        }
                        else//it has underflown
                        {
                            if (learned_long[kvp.Key].donkey[previous_exact_output[paratwinnumber - 1]])
                            {//check if previos exact output is 0 or not!
                                learned_long[kvp.Key].values[i] = 0;
                            }
                            else
                            {
                                learned_long[kvp.Key].values[i] = 1;
                            }
                        }
                    }
                }
            }
            if (settings.donkey && previous_exact_output[paratwinnumber] != 0)
            {
                learned_long[previous_exact_input[paratwinnumber]].values[previous_exact_output[paratwinnumber] - 1] = 0;
                learned_long[previous_exact_input[paratwinnumber]].donkey[previous_exact_output[paratwinnumber] - 1] = true;
            }
            //we need to make sure that the donkey principal can activate 
            //and down under we need to also make sure it can... somehow
            //do we use mask id?
            //how can we use mask id?
            //1,3,7,15 only the 1 does not work!!!!!!
            //fuck the 1 inputs
            foreach (KeyValuePair<uint, bool[]> kvp in active_memory_short[paratwinnumber])
            {
                for (int i = 0; i < output_size; i++)
                {
                    if (kvp.Value[i])
                    {
                        ulong temppreulong = learned_short[kvp.Key].values[i];
                        ulong temppostulong = temppreulong - (ulong)(settings.finetune.punish_relative * paramultiplier);
                        if (temppostulong <= temppreulong && temppostulong != 0)//if it doesn't overflows 
                        {
                            EXP += (ulong)(settings.finetune.punish_relative * paramultiplier);
                            learned_short[kvp.Key].values[i] = temppostulong;
                        }
                        else//it has overflown!
                        {
                            learned_short[kvp.Key].values[i] = 1;
                        }
                        //we add the reward
                        //now we check to make sure it has not looped around
                    }
                }
            }
        }//end else
        active_memory_long[paratwinnumber].Clear();
        active_memory_short[paratwinnumber].Clear();
    }
    private ulong bytemask(byte parabyte, ulong paralong) //see the paralong as an array of bytes, 8 of them, and the parabyte as mask to show which bytes are returned
    {// 1000 
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
        //a really important aspect of this function is that it can take arrays bigger then inputsize
        //because for testing and reading memory it will just get a full 8 array
        ulong returnme = 0;
        for (int i = 0; i < input_size; i++)
        {
            returnme = returnme << 8;//could be more minmaxxed like bytemask
            returnme = returnme + parabytearray[i];
        }
        return returnme;
    }
    private ulong bytestoulongreverse(byte[] parabytearray)
    {
        //a really important aspect of this function is that it can take arrays bigger then inputsize
        //because for testing and reading memory it will just get a full 8 array
        ulong returnme = 0;
        for (int i = 7; i >= 0; i--)
        {
            returnme = returnme << 8;//could be more minmaxxed like bytemask
            returnme = returnme + parabytearray[i];
        }
        return returnme;
    }
    private uint bytestouintreverse(byte[] parabytearray)
    {
        //a really important aspect of this function is that it can take arrays bigger then inputsize
        //because for testing and reading memory it will just get a full 8 array
        uint returnme = 0;
        for (int i = 3; i >= 0; i--)
        {
            returnme = returnme << 8;//could be more minmaxxed like bytemask
            returnme = returnme + parabytearray[i];
        }
        return returnme;
    }

}

