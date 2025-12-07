// 2025/12/05 9:00
// 2025/12/05 10:17
// https://adventofcode.com/2025/day/6

public class Day07 {

  List<string> Grid = new();
  int SPosition = 0;

  private void _ReadInput() {
    string[] lines = File.ReadAllLines("data/input1.txt");

    Grid = lines.ToList();
    for (int index = 0; index < Grid[0].Length; index++) {
      if (Grid[0][index] == 'S') {
        SPosition = index;
        break;
      }
    }
  }

  public void Solve1() {
    HashSet<int> activeBeams = [SPosition];
    int splitCount = 0;
    for (int nextRow = 1; nextRow < Grid.Count - 1; nextRow++) {

      HashSet<int> nextActiveBeams = [];
      foreach (int beam in activeBeams) {
        char value = Grid[nextRow][beam];
        if (value == '.') {
          nextActiveBeams.Add(beam);
        } else {
          nextActiveBeams.Add(beam - 1);
          nextActiveBeams.Add(beam + 1);
          splitCount += 1;
        }
      }
      activeBeams = nextActiveBeams;
    }

    Console.WriteLine(splitCount);
  }

  public int Count(int beamIndex, int currentRow, Dictionary<(int, int), int> memo) {
    if (currentRow >= Grid.Count - 1) { return 1; }

    int nextRow = currentRow + 1;
    char value = Grid[nextRow][beamIndex];
    int count = 0;
    if (value == '.') {
      count = Count(beamIndex, nextRow, memo);
    } else {
      int left = Count(beamIndex - 1, nextRow, memo);
      int right = Count(beamIndex + 1, nextRow, memo);
      count = left + right;
    }
    memo[(beamIndex, currentRow)] = count;
    return count;
  }

  public void Solve2() {
    Dictionary<int, long> activeBeams = new() { { SPosition, 1 } };

    for (int nextRow = 1; nextRow < Grid.Count - 1; nextRow++) {
      Dictionary<int, long> nextActiveBeams = [];

      foreach (var (beam, beamCount) in activeBeams) {
        char value = Grid[nextRow][beam];

        if (value == '.') {
          if (!nextActiveBeams.ContainsKey(beam)) {
            nextActiveBeams[beam] = 0;
          }
          nextActiveBeams[beam] += beamCount;
        } else {
          if (!nextActiveBeams.ContainsKey(beam - 1)) {
            nextActiveBeams[beam - 1] = 0;
          }
          if (!nextActiveBeams.ContainsKey(beam + 1)) {
            nextActiveBeams[beam + 1] = 0;
          }
          nextActiveBeams[beam - 1] += beamCount;
          nextActiveBeams[beam + 1] += beamCount;
        }
      }
      activeBeams = nextActiveBeams;
    }

    long splitCount = activeBeams.Values.Sum();
    Console.WriteLine(splitCount);
  }

  public void Run() {
    _ReadInput();
    Solve2();
  }
}