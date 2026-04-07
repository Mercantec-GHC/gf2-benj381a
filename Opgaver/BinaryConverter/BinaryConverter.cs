using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opgaver
{
    public class BinaryConverter
    {
        // Konverterer en binær streng (fx "10101010") til et heltal (fx 170)
        public static int BinaryToDecimal(string binary)
        {
            // TODO: Implementér konvertering fra binær til decimal uden indbyggede konverteringsfunktioner
            
            int num = 0;
            for (int i = 0; i < binary.Length; i++)
                num |= (binary[binary.Length - i - 1] == '0' ? 0 : 1) << i;

            return num;
        }

        // Konverterer et heltal (fx 170) til en binær streng (fx "10101010")
        public static string DecimalToBinary(int number)
        {
            // TODO: Implementér konvertering fra decimal til binær uden indbyggede konverteringsfunktioner
            string str = "";
            int currNum = 0b1;

            while (number > 0)
            {
                str += $"{((number & currNum) == 0 ? 0 : 1)}";
                number -= number & currNum;
                currNum <<= 1;
            }

            return new string(str.Reverse().ToArray());
        }

        // Konverterer en binær talgruppe (fx "10111011.01001011.10101010.01010101") til decimaler (fx "187.75.170.85")
        public static string BinaryGroupToDecimal(string binaryGroup)
        {
            // TODO: Split binærgruppen op og brug BinaryToDecimal på hver del
            string output = "";
            foreach (string binary in binaryGroup.Split('.')) 
                output += $"{BinaryToDecimal(binary)}.";

            return output.Trim('.');
        }

        // Konverterer en decimal talgruppe (fx "187.75.170.85") til binær (fx "10111011.01001011.10101010.01010101")
        public static string DecimalGroupToBinary(string decimalGroup)
        {
            // TODO: Split decimalgruppen op og brug DecimalToBinary på hver del

            string output = "";
            foreach (string num in decimalGroup.Split('.'))
                output += $"{DecimalToBinary(int.Parse(num)).PadLeft(8,'0')}.";

            return output.Trim('.');

        }

        // Brugermenu til at teste konverteringerne
        public static void Run()
        {
            // TODO: Lav en simpel menu, hvor brugeren kan vælge retning og indtaste tal
            Console.WriteLine("Velkommen til binær-decimal konverteringsprogrammet!");
            Console.WriteLine("(0) bin -> dec\n(1) dec -> bin");
            int choice = 0;
            bool succ = false;

            while (!succ || (choice != 0 && choice != 1))
                succ = int.TryParse(Console.ReadLine()!, out choice);

            Console.WriteLine(choice == 0 ? "bin -> dec" : "dec -> bin");
            Console.WriteLine(choice == 0 ? BinaryGroupToDecimal(Console.ReadLine()!) : DecimalGroupToBinary(Console.ReadLine()!));
        }
    }
}
