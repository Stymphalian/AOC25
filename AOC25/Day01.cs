// 2025/12/01 7:54
// 2025/12/01 8:49
// https://adventofcode.com/2025/day/1


public class Day01 {
    public const int RIGHT = 1;
    public const int LEFT = -1;
    public record Instruction(int Direction, int Value);
    private List<Instruction> _instructions = new();

    private void _ReadInput() {
        string[] lines = File.ReadAllLines("data/input1.txt");
        foreach(var line in lines) {
            int value = int.Parse(line[1..]);
            if(line[0] == 'L') {
                _instructions.Add(new(LEFT, value));
            } else {
                _instructions.Add(new(RIGHT, value));
            }
        }
    }

    public int MOD(int value, int mod) {
        if(value < 0) {
            return (value % mod) + mod;
        } else {
            return value % mod;
        }
    }

    public void Solve1() {
        _ReadInput();

        int currentPosition = 50;
        int answer = 0;
        foreach(var instruction in _instructions) {
            Console.WriteLine(instruction);
            if(instruction.Direction == RIGHT) {
                currentPosition = (currentPosition + instruction.Value) % 100;
            } else {
                currentPosition = (currentPosition - instruction.Value) % 100;
            }
            if(currentPosition == 0) {
                answer += 1;
            }
        }

        Console.WriteLine(answer);
    }

    public void Solve2() {
        _ReadInput();

        int currentPosition = 50;
        int answer = 0;
        foreach(var instruction in _instructions) {

            int modValue = instruction.Value;
            if(instruction.Value >= 100) {
                answer += instruction.Value / 100;
                modValue = instruction.Value % 100;
            }

            int nextPosition = MOD(currentPosition + instruction.Direction * modValue, 100);
            if(nextPosition == 0) {
                answer += 1;
            } else if(instruction.Direction == RIGHT && nextPosition < currentPosition && currentPosition != 0) {
                answer += 1;
            } else if(instruction.Direction == LEFT && nextPosition > currentPosition && currentPosition != 0) {
                answer += 1;
            }

            currentPosition = nextPosition;
            // Console.WriteLine(instruction);
            // Console.WriteLine($"Current Position: {currentPosition}, answer = {answer}");
        }

        Console.WriteLine(answer);
    }

    public void Run() {
        Solve2();
    }
}