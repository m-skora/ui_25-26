using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace projekt
{
    class Leaderboard
    {
        private string scoresDir;

        public Leaderboard(string scoresDirectory)
        {
            scoresDir = scoresDirectory;
            if (!Directory.Exists(scoresDir))
            {
                Directory.CreateDirectory(scoresDir);
            }
        }

        public void SaveScore(string setName, string playerName, int moves, double accuracy, double avgTimePerPair)
        {
            string scoreFile = Path.Combine(scoresDir, setName + "_scores.txt");
            string scoreLine = $"{DateTime.Now:yyyy-MM-dd} | {playerName} | Moves: {moves} | Accuracy: {accuracy:F1}% | Avg time/pair: {avgTimePerPair:F1}s";
            File.AppendAllLines(scoreFile, new[] { scoreLine });
        }

        public void ShowScores(string setName)
        {
            string scoreFile = Path.Combine(scoresDir, setName + "_scores.txt");
            if (!File.Exists(scoreFile))
            {
                Console.WriteLine($"No scores found for set '{setName}'.");
                return;
            }

            var lines = File.ReadAllLines(scoreFile).ToList();
            if (lines.Count == 0)
            {
                Console.WriteLine($"No scores recorded yet for set '{setName}'.");
                return;
            }

            lines.Sort(delegate (string a, string b)
            {
                try
                {
                    int movesA = int.Parse(a.Split('|')[2].Split(':')[1].Trim());
                    int movesB = int.Parse(b.Split('|')[2].Split(':')[1].Trim());
                    if (movesA != movesB) return movesA.CompareTo(movesB);

                    double accA = double.Parse(a.Split('|')[3].Split(':')[1].Trim().TrimEnd('%'));
                    double accB = double.Parse(b.Split('|')[3].Split(':')[1].Trim().TrimEnd('%'));
                    if (accA != accB) return accB.CompareTo(accA);

                    double timeA = double.Parse(a.Split('|')[4].Split(':')[1].Trim().TrimEnd('s'));
                    double timeB = double.Parse(b.Split('|')[4].Split(':')[1].Trim().TrimEnd('s'));
                    return timeA.CompareTo(timeB);
                }
                catch
                {
                    return 0;
                }
            });

            int rank = 1;
            foreach (string line in lines)
            {
                Console.WriteLine($"{rank}. {line}");
                rank++;
                if (rank > 5) break;
            }
        }
    }
}
