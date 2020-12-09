using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        [OnMethodBoundaryAttribute1]
        static void Main(string[] args)
        {
            string input = @"http://192.168.30.100:11234/PersonInfoQuery/Info";
            string pattern = @"(\w+)";
            var result = Regex.Matches(input, pattern);
            var result1 = Regex.Split(input, @":\/\/([^/:]+)(:\d*)?");
            var result2 = Regex.Match(input, @"(?<=\/\/)([^/:]+)(:\d*)?");
            Console.WriteLine(result2.Value);
        }
    }
}
