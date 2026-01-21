using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace projekt
{
    class Game
    {
        private List<Card> cards;
        private Dictionary<string, string> pairDict;
        private List<ConsoleColor> availableColors;
        private int moves;
        private int matchedPairs;
        private int totalPairs;
        private int cardWidth = 15;
        private int columns;
        private bool learningMode;

        public Game(List<(string, string)> pairs, bool learning)
        {
            cards = new List<Card>();
            pairDict = new Dictionary<string, string>();
            learningMode = learning;

            foreach (var pair in pairs)
            {
                cards.Add(new Card { Text = pair.Item1 });
                cards.Add(new Card { Text = pair.Item2 });
                pairDict[pair.Item1] = pair.Item2;
                pairDict[pair.Item2] = pair.Item1;
            }

            availableColors = new List<ConsoleColor>
            {
                ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Green, ConsoleColor.Magenta,
                ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.DarkRed, ConsoleColor.DarkBlue,
                ConsoleColor.DarkGreen, ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow, ConsoleColor.DarkCyan
            };

            var rnd = new Random();
            List<Card> shuffled = new List<Card>();
            while (cards.Count > 0)
            {
                int index = rnd.Next(cards.Count);
                shuffled.Add(cards[index]);
                cards.RemoveAt(index);
            }
            cards = shuffled;

            moves = 0;
            matchedPairs = 0;
            totalPairs = pairs.Count;
        }

        public (int moves, double accuracy, double avgTimePerPair) Play()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (matchedPairs < totalPairs)
            {
                Console.Clear();
                columns = Math.Max(2, Console.WindowWidth / cardWidth);
                Console.WriteLine($"Moves: {moves} | Matched pairs: {matchedPairs}/{totalPairs}");
                DrawBoard();

                int firstIndex = GetCardChoice("Select first card: ");
                cards[firstIndex].Revealed = true;

                Console.Clear();
                Console.WriteLine($"Moves: {moves} | Matched pairs: {matchedPairs}/{totalPairs}");
                DrawBoard();

                int secondIndex = GetCardChoice("Select second card: ", firstIndex);
                cards[secondIndex].Revealed = true;

                Console.Clear();
                Console.WriteLine($"Moves: {moves} | Matched pairs: {matchedPairs}/{totalPairs}");
                DrawBoard();

                moves++;

                if (pairDict[cards[firstIndex].Text] == cards[secondIndex].Text)
                {
                    List<ConsoleColor> usedColors = new List<ConsoleColor>();
                    foreach (var c in cards)
                    {
                        if (c.PairColor.HasValue) usedColors.Add(c.PairColor.Value);
                    }

                    ConsoleColor colorForPair = ConsoleColor.Green;
                    foreach (var c in availableColors)
                    {
                        if (!usedColors.Contains(c))
                        {
                            colorForPair = c;
                            break;
                        }
                    }

                    cards[firstIndex].PairColor = colorForPair;
                    cards[secondIndex].PairColor = colorForPair;
                    cards[firstIndex].Matched = true;
                    cards[secondIndex].Matched = true;
                    matchedPairs++;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Match!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not a match.");
                    Console.ResetColor();

                    if (learningMode)
                        Console.WriteLine($"Correct pair: {cards[firstIndex].Text} => {pairDict[cards[firstIndex].Text]}");

                    cards[firstIndex].Revealed = false;
                    cards[secondIndex].Revealed = false;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }

            sw.Stop();

            Console.Clear();
            Console.WriteLine($"Moves: {moves} | Matched pairs: {matchedPairs}/{totalPairs}");
            DrawBoard();

            double accuracy = 100.0 * matchedPairs / moves;
            double avgTimePerPair = sw.Elapsed.TotalSeconds / totalPairs;

            Console.WriteLine($"You won in {moves} moves!");
            Console.WriteLine($"Accuracy: {accuracy:F1}% | Avg time per pair: {avgTimePerPair:F1} sec");

            return (moves, accuracy, avgTimePerPair);
        }

        private void DrawBoard()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                Card c = cards[i];
                string displayText;
                ConsoleColor color;

                if (c.Matched || c.Revealed)
                {
                    displayText = c.Text;
                    color = c.PairColor ?? ConsoleColor.Yellow;
                }
                else
                {
                    displayText = (i + 1).ToString();
                    color = ConsoleColor.Cyan;
                }

                Console.ForegroundColor = color;
                Console.Write($"[{displayText}]".PadRight(cardWidth));
                Console.ResetColor();

                if ((i + 1) % columns == 0) Console.WriteLine();
            }
            Console.WriteLine();
        }

        private int GetCardChoice(string prompt, int? exclude = null)
        {
            int choice = -1;
            bool valid = false;

            while (!valid)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                int number;
                bool isNumber = int.TryParse(input, out number);
                if (!isNumber)
                {
                    Console.WriteLine("Please enter a valid number.");
                    continue;
                }

                number = number - 1;

                if (number < 0 || number >= cards.Count)
                {
                    Console.WriteLine("Number out of range.");
                    continue;
                }

                if (cards[number].Matched || cards[number].Revealed)
                {
                    Console.WriteLine("Card already matched or revealed.");
                    continue;
                }

                if (exclude.HasValue && number == exclude.Value)
                {
                    Console.WriteLine("You already selected this card.");
                    continue;
                }

                choice = number;
                valid = true;
            }

            return choice;
        }
    }
}
