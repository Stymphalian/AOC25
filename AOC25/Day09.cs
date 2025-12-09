// 2025/12/08 9:16
// 2025/12/08 
// https://adventofcode.com/2025/day/9


public class Day09 {

  public class Vec2 {
    public long X;
    public long Y;

    public Vec2(long x, long y) {
      X = x;
      Y = y;
    }

    public long Area(Vec2 other) {
      long width = Math.Abs(X - other.X) + 1;
      long height = Math.Abs(Y - other.Y) + 1;
      return width * height;
    }

    public Vec2 Sub(Vec2 o) {
      return new(X - o.X, Y - o.Y);
    }
  }

  public class LineSegment(Vec2 p1, Vec2 p2) {
    public Vec2 p1 = p1;
    public Vec2 p2 = p2;

    public bool Overlaps(LineSegment other) {
      return false;
    }
  }

  // public class Range(long Begin, long End) {
  //   public long Begin = Begin;
  //   public long End = End;

  //   public Range? Overlaps(Range other) {
  //     long start2 = Math.Max(Begin, other.Begin);
  //     long end1 = Math.Min(End, other.End);
  //     if (start2 > end1) {
  //       return null;
  //     } else {
  //       return new(start2, end1);
  //     }
  //   }
  // }

  List<Vec2> Reds = [];

  private void _ReadInput() {
    string filepath = "data/example1.txt";
    string[] lines = File.ReadAllLines(filepath);
    foreach (string line in lines) {
      string[] parts = line.Split(',');
      long x = long.Parse(parts[0]);
      long y = long.Parse(parts[1]);
      Reds.Add(new Vec2(x, y));
    }
  }

  public void Solve1() {
    long maxArea = 0;
    for (int a = 0; a < Reds.Count; a++) {
      for (int b = a + 1; b < Reds.Count; b++) {
        Vec2 bl = Reds[a];
        Vec2 tr = Reds[b];
        maxArea = Math.Max(bl.Area(tr), maxArea);
      }
    }

    Console.WriteLine(maxArea);
  }

  public void Solve2() {
    List<LineSegment> segments = [];
    for (int index = 0; index < Reds.Count; index++) {
      int next = (index + 1) % Reds.Count;
      segments.Add(new(Reds[index], Reds[next]));
    }
    long minX = Reds.Select(a => a.X).Min();
    long maxX = Reds.Select(a => a.X).Max();
    long minY = Reds.Select(a => a.Y).Min();
    long maxY = Reds.Select(a => a.Y).Max();
    long maxArea = 0;

    HashSet<int> activeSegments = [];
    for (long scanY = minY; scanY <= maxY; scanY++) {
      LineSegment ySegment = new(new(minX, scanY), new(maxX, scanY));

      // Create the active set of segments
      HashSet<int> nextSegments = [];
      for (int segmentIndex = 0; segmentIndex < segments.Count; segmentIndex++) {
        var segment = segments[segmentIndex];
        if (segment.Overlaps(ySegment)) {
          nextSegments.Add(segmentIndex);
        }
      }





    }


    Console.WriteLine(maxArea);
  }

  public void Run() {
    _ReadInput();
    Solve2();
  }
}