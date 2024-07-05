using System;
using System.Threading;


Console.WriteLine("Welcome");
Console.WriteLine("");

Timer tijd = new Timer(repeater, null, 0, 15);//this makes it so that the function repeater gets called every 15ms
void repeater(object o)
{

}


uint multigame = 1;//how many games run at the same time!
byte[,,] gamestate = new byte[multigame,10,10];
/*
 0 = empty .
1 = player P
2 = 
  made multiple to support multiple games at the same time
  
  
 */



//




//copyright owned by BlossomStudio