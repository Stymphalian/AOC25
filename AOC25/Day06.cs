// 2025/12/05 9:00
// 2025/12/05 10:17
// https://adventofcode.com/2025/day/6

public class Day06 {
    public const string ADD = "+";
    public const string MULT = "*";
    public const char SPACE = ' ';

    Dictionary<int, List<int>> Problems = new();
    List<string> ProblemLines = new();
    List<string> Operations = new();

    private void _ReadInput() {
        string[] lines = File.ReadAllLines("data/input1.txt");

        foreach (string line in lines) {
            string[] parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (parts[0] == ADD || parts[0] == MULT) {
                Operations = parts.ToList();
            } else {
                ProblemLines.Add(line);
                for (int index = 0; index < parts.Length; index++) {
                    if (!Problems.ContainsKey(index)) {
                        Problems[index] = new();
                    }
                    Problems[index].Add(int.Parse(parts[index]));
                }
            }
        }
    }

    public void Solve1() {
        long answer = 0;
        foreach (var entry in Problems) {
            string operation = Operations[entry.Key];
            long problemResult = operation == ADD ? 0 : 1;
            foreach (int num in entry.Value) {
                if (operation == ADD) {
                    problemResult += num;
                } else {
                    problemResult *= num;
                }
            }

            Console.WriteLine($"Index = {entry.Key}, Numbers = {entry.Value[0]}, Got = {problemResult}");

            answer += problemResult;
        }

        Console.WriteLine(answer);
    }

    public void Solve2() {
        long ApplyOp(List<int> problemNumbers, string operation) {
            // Process the collected numbers
            long problemResult = operation == ADD ? 0 : 1;
            foreach (int num in problemNumbers) {
                if (operation == ADD) {
                    problemResult += num;
                } else {
                    problemResult *= num;
                }
            }
            return problemResult;
        }

        long finalAnswer = 0;
        List<int> problemNumbers = [];
        int opIndex = Operations.Count - 1;
        int currentCol = ProblemLines[0].Length - 1;
        int numRows = ProblemLines.Count;
        while (currentCol >= 0) {
            // Read off the num from the column and add to problem numbers
            List<char> digits = new();
            bool allSpaces = true;
            for (int row = 0; row < numRows; row++) {
                if (ProblemLines[row][currentCol] == SPACE) { continue; }
                allSpaces = false;
                digits.Add(ProblemLines[row][currentCol]);
            }

            if (allSpaces) {
                // We reached the space boundary for a problem
                // Process the last group of numbers we have collected and 
                // reset the state.
                finalAnswer += ApplyOp(problemNumbers, Operations[opIndex]);
                problemNumbers.Clear();
                opIndex -= 1;
            } else {
                int columnNumber = int.Parse(string.Join("", digits));
                problemNumbers.Add(columnNumber);
            }

            currentCol -= 1;
        }

        finalAnswer += ApplyOp(problemNumbers, Operations[opIndex]);

        Console.WriteLine(finalAnswer);
    }

    public void Run() {
        _ReadInput();
        Solve2();
    }
}