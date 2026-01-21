# Memory Game (Console)

Simple console-based memory game written in C#.

## About
This is a small memory game where you match pairs loaded from a text file.  
The game can be played in normal mode or learning mode. Scores can be saved and viewed later.

## How to run
Run the project as a standard .NET console application:

dotnet run

## Learning sets
Game data is stored in the `learningSets` folder.

Each `.txt` file should look like this:
apple;banana
cat;dog
one;two

Each line is one pair, separated by `;`.

## Command line options
- `--learn` – learning mode (shows correct pair after a wrong guess)
- `--file <fileName>` – play a specific set
- `--scores <setName>` – show scores for a set
- `--help` – show help

Example:
dotnet run -- --file animals.txt

## Scores
Scores are saved in the `scores` folder.  
The leaderboard is sorted by moves, accuracy, and time.

## Notes
This is a simple educational project made for practice.
