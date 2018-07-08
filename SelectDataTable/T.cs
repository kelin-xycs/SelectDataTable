using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectDataTable
{
    class T
    {
        private static bool Debug = true;

        public static void WriteLine(object obj)
        {
            if (!Debug)
            {
                return;
            }

            Console.WriteLine(obj);
        }

        public static void WriteLine(List<string> list)
        {
            if ( !Debug )
            {
                return;
            }
               
            
            foreach(string str in list)
            {
                Console.WriteLine("\"" + str + "\"");
            }

            Console.WriteLine();
        }
    }
}
