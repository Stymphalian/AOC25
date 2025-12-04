// 2025/12/03 10:54
// 2025/12/03 11:24
// https://adventofcode.com/2025/day/3


public class Day03 {
    public record Bank(List<int> Numbers);
    private List<Bank> _cases = new();

    private void _ReadInput() {
        string[] lines = File.ReadAllLines("data/input1.txt");
        foreach(var line in lines) {
            _cases.Add(new([.. line.ToList().Select(power => power - '0')]));
        }
    }

    public int CalculateJoltage(List<int> bank) {
        int leftIndex = 0;
        for(int index = 0; index < bank.Count-1; index++) {
            if (bank[index] > bank[leftIndex]) {
                leftIndex = index;
            }
        }

        int rightIndex = leftIndex + 1;
        for(int index = leftIndex+1; index < bank.Count; index++) {
            if (bank[index] > bank[rightIndex]) {
                rightIndex = index;
            }
        }

        int left = bank[leftIndex];
        int right = bank[rightIndex];
        return left * 10 + right;
    }

    public void Solve1() {
        _ReadInput();
        long answer = 0;
        foreach(var testcase in _cases) {
            int candidate = CalculateJoltage(testcase.Numbers);
            Console.WriteLine(candidate);
            answer += candidate;
        }
        Console.WriteLine(answer);
    }

    public long CalculateJoltage2(List<int> bank, int numDigits) {
        List<int> collected = [];

        int leftIndex = 0;
        for(int digit = 0; digit < numDigits; digit++) {
            int limit = bank.Count - numDigits + digit + 1;
            for (int index = leftIndex; index < limit; index++) {
                if (bank[index] > bank[leftIndex]) {
                    leftIndex = index;
                }
            }
            collected.Add(bank[leftIndex]);
            leftIndex += 1;
        }

        long value = long.Parse(string.Join("", collected.Select(static i => (char)(i + '0'))));
        return value;
    }

    public void Solve2() {
        _ReadInput();
        long answer = 0;
        foreach(var testcase in _cases) {
            long candidate = CalculateJoltage2(testcase.Numbers, 12);
            Console.WriteLine(candidate);
            answer += candidate;
        }
        Console.WriteLine(answer);
    }

    public void Run() {
        Solve2();
    }
}