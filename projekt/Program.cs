using projekt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace projekt
{
    class Program
    {
        static void Main(string[] args)
        {
            bool learningMode = false;
            bool showScores = false;
            string scoreSetName = "";

            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectDir = Directory.GetParent(exeDir).Parent.Parent.FullName;
            string learningSetsDir = Path.Combine(projectDir, "learningSets");
            string scoresDir = Path.Combine(projectDir, "scores");
            Directory.CreateDirectory(learningSetsDir);
            Directory.CreateDirectory(scoresDir);
            string filePath = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--help")
                {
                    Console.WriteLine("Usage: memory [--file <fileName>] [--learn] [--scores <setName>]");
                    Console.WriteLine("--learn : show correct pair after wrong guess");
                    Console.WriteLine("--file <fileName> : specify custom file");
                    Console.WriteLine("--scores <setName> : show scores for given set");
                    return;
                }

                if (args[i] == "--file" && i + 1 < args.Length) { filePath = args[i + 1]; i++; }
                if (args[i] == "--learn") learningMode = true;
                if (args[i] == "--scores" && i + 1 < args.Length) { showScores = true; scoreSetName = args[i + 1]; i++; }
            }

            if (showScores)
            {
                scoresDir = Path.Combine(projectDir, "scores");
                var lb = new Leaderboard(scoresDir);
                lb.ShowScores(scoreSetName);
                return;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                var files = Directory.GetFiles(learningSetsDir, "*.txt");
                if (files.Length == 0)
                {
                    Console.WriteLine($"No .txt files found in {learningSetsDir}");
                    return;
                }

                Console.WriteLine("Choose a set to play:");
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{i + 1}) {Path.GetFileNameWithoutExtension(files[i])}");
                }

                int choice = -1;
                bool validChoice = false;

                while (!validChoice)
                {
                    Console.Write("Enter number: ");
                    string input = Console.ReadLine();
                    int number;
                    bool isNumber = int.TryParse(input, out number);

                    if (!isNumber)
                    {
                        Console.WriteLine("Please enter a valid number.");
                        continue;
                    }

                    if (number < 1 || number > files.Length)
                    {
                        Console.WriteLine($"Please choose a number between 1 and {files.Length}.");
                        continue;
                    }

                    choice = number;
                    validChoice = true;
                }

                filePath = files[choice - 1];
            }
            else
            {
                filePath = Path.Combine(learningSetsDir, filePath);
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found: " + filePath);
                return;
            }

            string setName = Path.GetFileNameWithoutExtension(filePath);

            var lines = File.ReadAllLines(filePath);
            var pairs = new List<(string, string)>();
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 2)
                    pairs.Add((parts[0].Trim(), parts[1].Trim()));
            }

            if (pairs.Count == 0)
            {
                Console.WriteLine("No valid pairs in file.");
                return;
            }

            bool playAgain = true;
            while (playAgain)
            {
                var game = new Game(pairs, learningMode);
                var result = game.Play();

                if (!learningMode)
                {
                    Console.Write("Enter your name for the leaderboard: ");
                    string playerName = Console.ReadLine();

                    var leaderboard = new Leaderboard(scoresDir);
                    leaderboard.SaveScore(setName, playerName, result.moves, result.accuracy, result.avgTimePerPair);

                    Console.WriteLine("\n=== TOP 5 Scores ===");
                    leaderboard.ShowScores(setName);
                }
                else
                {
                    Console.WriteLine("\nLearning mode: score not saved");
                }

                Console.WriteLine("\nPress [P] to play again, any other key to quit...");
                string input = Console.ReadLine().Trim().ToUpper();
                if (input != "P") playAgain = false;
            }
        }
    }
}
