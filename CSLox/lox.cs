using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CSLox
{
    public class Lox
    {

        static bool hadError = false;
        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script]");
                System.Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            string code = System.IO.File.ReadAllText(Path.GetFullPath(path));
            Run(code);

            // Indicate an error in the exit code.
            if (hadError) System.Environment.Exit(65);
        }

        private static void RunPrompt()
        {
            for (; ; )
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                if (line == "") break;
                Run(line);
                hadError = false;
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            //For now, just print tokens.
            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }
    }
}