using DokanNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmpfs
{
    class HscfsManager
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"    __  _______ _________________
   / / / / ___// ____/ ____/ ___/
  / /_/ /\__ \/ /   / /_   \__ \ 
 / __  /___/ / /___/ __/  ___/ / 
/_/ /_//____/\____/_/    /____/  
                                 ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"HIGH SPEED COMPRESSION FILE SYSTEM
VERSION {0}

COPYRIGHT (C) http://github.com/ralz0ne 2020.
", HscfsImage.VER);
            Console.ResetColor();
            HscfsDriver driver = new HscfsDriver(new HscfsImage());
            Dokan.Mount(driver, "n:");
        }
    }
}
