// 2025/12/03 10:15
// 2025/12/03 10:54
// https://adventofcode.com/2025/day/2


public class Day02 {
    public record Case(long Start, long End);
    private List<Case> _cases = new();

    private void _ReadInput() {
        string[] lines = File.ReadAllLines("data/input1.txt");
        string line = lines[0];
        var parts = line.Split(",");
        foreach(var part in parts) {
            var range = part.Split("-");
            long start = long.Parse(range[0]);
            long end = long.Parse(range[1]);
            _cases.Add(new Case(start, end));
        }
    }


    public bool isInvalid(long value) {
        string valueString= value.ToString();
        int numDigits = valueString.Length;
        if (numDigits % 2 == 1) { return false; }
        int halfIndex = numDigits/2;
        for(int left = 0; left < halfIndex; left++) {
            int right = left + halfIndex;
            if (valueString[left] != valueString[right]) {
                return false;
            }
        }
        return true;
    }

    public long countInvalids(long start, long end) {
        long invalidSum = 0;
        for(long index = start; index <= end; index++) {
            if (isInvalid(index)) {
                invalidSum += index;
            }
        }
        return invalidSum;
    }

    public void Solve1() {
        _ReadInput();
        long answer = 0;
        foreach(var testcase in _cases) {
            answer += countInvalids(testcase.Start, testcase.End);
        }
        Console.WriteLine(answer);
    }


    public bool isInvalid2(long value) {
        string valueString= value.ToString();
        int numDigits = valueString.Length;
        int half = numDigits / 2;

        for(int limit = 1; limit <= half; limit++) {
            // Cannot evenly chop up the number into 'limit' number of equal pieces
            if (numDigits % limit != 0) {continue;}

            bool invalid = true;
            int left = 0;
            int right = left + limit;
            while (right < numDigits) {
                if(valueString[left] != valueString[right]) {
                    invalid = false;
                    break;
                }
                left = (left + 1)%limit;
                right += 1;
            }
            if (invalid) {
                return true;
            }
        }

        return false;
    }

    public long countInvalids2(long start, long end) {
        long invalidSum = 0;
        for(long index = start; index <= end; index++) {
            if (isInvalid2(index)) {
                invalidSum += index;
            }
        }
        return invalidSum;
    }

    public void Solve2() {
        _ReadInput();
        long answer = 0;
        foreach(var testcase in _cases) {
            answer += countInvalids2(testcase.Start, testcase.End);
        }
        Console.WriteLine(answer);
    }

    public void Run() {
        Solve2();
    }
}