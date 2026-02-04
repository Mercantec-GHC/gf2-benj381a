using System;

namespace Opgaver
{
    public class ControlFlow
    {
        public static void Run()
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine(
                @"Velkommen til opgaver omkring Control Flow med if, else if og else, 
            Switch og Ternary operator!"
            );
            If1();
            If2();

            Switch1();
            Ternary1();

            MiniProjektQuiz();
            MiniProjektKarakterFeedback();
        }

        public static void If1()
        {
            Console.WriteLine(
                "Lav et program som tjekker om en given værdi er højere eller lavere end 18"
            );
            // Lav opgaven herunder!
            string input = Console.ReadLine()!;
            int val;
            if (int.TryParse(input, out val))
            {
                if (val > 18)
                    Console.WriteLine($"val is higher than 18 ({val})");
                else
                    Console.WriteLine($"val is lower than 18 ({val})");
            }

        }

        public static void If2()
        {
            Console.WriteLine("Lav et program som tjekker om en given værdi er lige eller ulige");
            // Lav opgaven herunder!
            string input = Console.ReadLine()!;
            int val;
            if (int.TryParse(input, out val))
            {
                if ((val & 1) == 0)
                    Console.WriteLine($"val is equal ({val})");
                else
                    Console.WriteLine($"val is odd ({val})");
            }
        }

        public static void Switch1()
        {
            Console.WriteLine("Lav et program som tjekker om en given værdi er lige eller ulige");
            // Lav opgaven herunder!
            string input = Console.ReadLine()!;
            int val;
            if (int.TryParse(input, out val))
            {
                switch (val & 1)
                {
                    case 0:
                        Console.WriteLine($"val is equal ({val})");
                        break;
                    case 1:
                        Console.WriteLine($"val is odd ({val})");
                        break;
                }
            }
        }

        public static void Ternary1()
        {
            Console.WriteLine("Lav et program som tjekker om en given værdi er lige eller ulige");
            // Lav opgaven herunder!
            string input = Console.ReadLine()!;
            int val;
            if (int.TryParse(input, out val))
            {
                Console.WriteLine($"val is {((val & 1) == 0 ? "equal" : "odd")} ({val})");
            }
        }

        public static void MiniProjektQuiz()
        {
            Console.WriteLine("\nMini-projekt: Simpelt quiz-spil (skabelon)");
            Console.WriteLine("Opgave:");
            Console.WriteLine(
                "Lav et program, der stiller brugeren tre spørgsmål (du vælger selv spørgsmål og svar)."
            );
            Console.WriteLine("Brugeren skal indtaste sit svar til hvert spørgsmål.");
            Console.WriteLine(
                "Programmet skal tjekke, om svaret er rigtigt eller forkert, og til sidst udskrive, hvor mange rigtige brugeren fik."
            );
            Console.WriteLine(
                "Tip: Brug variabler til at gemme point og svar, og if/else til at tjekke svarene."
            );
            // Lav opgaven herunder!

            int numCorrect = 0;

            Console.Write("Hvad er 1 + 1? ");
            string input = Console.ReadLine()!;
            int answer;
            if (int.TryParse(input, out answer))
                numCorrect += answer == 2 ? 1 : 0;

            Console.Write("Hvad er 2 + 2? ");
            input = Console.ReadLine()!;
            if (int.TryParse(input, out answer))
                numCorrect += answer == 4 ? 1 : 0;

            Console.Write("Hvad er 3 + 3? ");
            input = Console.ReadLine()!;
            if (int.TryParse(input, out answer))
                numCorrect += answer == 6 ? 1 : 0;

            Console.WriteLine($"Du fik {numCorrect}/3 rigtige");
        }

        public static void MiniProjektKarakterFeedback()
        {
            Console.WriteLine("\nMini-projekt: Karakter-feedback (skabelon)");
            Console.WriteLine("Opgave:");
            Console.WriteLine(
                "Lav et program, hvor brugeren indtaster en karakter (fx 12, 10, 7, 4, 02, 00 eller -3)."
            );
            Console.WriteLine(
                @"Programmet skal give en passende feedback baseret på karakteren, 
            fx 'Super flot!', 'Godt klaret', 'Du kan gøre det bedre' osv."
            );
            Console.WriteLine("Brug if/else eller switch til at vælge feedbacken.");

            Console.WriteLine(
                @"Ekstra opgave: Lav så man indtaster flere karaktere 
            for en bruger og man regner gennemsnittet ud."
            );
            // Lav opgaven herunder!

            string input = Console.ReadLine()!;
            string[] grades = input.Split(',');
            double avrGrade = 0;
            foreach (string grade in grades)
            {
                int iGrade;
                if (int.TryParse(grade, out iGrade))
                    avrGrade += iGrade;
                else
                    Console.WriteLine("Der er en fejl i dit input");
            }

            avrGrade /= grades.Length;

            Console.WriteLine($"Du fik {avrGrade} i gennemsnit");
            
        }
    }
}
