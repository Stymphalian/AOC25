// 2025/12/08 9:16
// 2025/12/08 
// https://adventofcode.com/2025/day/9


using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public class Day09 {
  public class Vec2 {
    public long X;
    public long Y;

    public Vec2(long x, long y) {
      X = x;
      Y = y;
    }

    public void Add(Vec2 other) {
      X += other.X;
      Y += other.Y;
    }

    public long Area(Vec2 other) {
      long width = Math.Abs(X - other.X) + 1;
      long height = Math.Abs(Y - other.Y) + 1;
      return width * height;
    }

    public Vec2 Sub(Vec2 o) {
      return new(X - o.X, Y - o.Y);
    }

    public static long Cross(Vec2 a, Vec2 b) {
      return a.X * b.Y - a.Y * b.X;
    }

    public static long getTurn(Vec2 a, Vec2 b, Vec2 c) {
      Vec2 ba = a.Sub(b);
      Vec2 bc = c.Sub(b);
      long value = Vec2.Cross(ba, bc);
      if (value == 0) {
        return 0; // Straight
      } else if (value > 0) {
        return 1; // Clockwise
      } else {
        return -1; // Counter-clockwise
      }
    }

    public bool IsEqual(Vec2 other) {
      return X == other.X && Y == other.Y;
    }
  }

  List<Vec2> Reds = [];
  private void _ReadInput() {
    string filepath = "data/input1.txt";
    string[] lines = File.ReadAllLines(filepath);
    foreach (string line in lines) {
      string[] parts = line.Split(',');
      long x = long.Parse(parts[0]);
      long y = long.Parse(parts[1]);
      Reds.Add(new Vec2(x, y));
    }
  }

  public static int Mod(int v, int n) {
    return (v % n + n) % n;
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


  public const int DIR_UP = 0;
  public const int DIR_RIGHT = 1;
  public const int DIR_DOWN = 2;
  public const int DIR_LEFT = 3;

  public record Edge(Vec2 a, Vec2 b, int forward) {
    public int Normal() {
      return (forward + 1) % 4;
    }
    public int INormal() {
      return (forward + 3) % 4;
    }

    public bool IsEqual(Edge other) {
      return a.IsEqual(other.a) && b.IsEqual(other.b);
    }

    public long Length() {
      if (forward == DIR_UP || forward == DIR_DOWN) {
        return Math.Abs(a.Y - b.Y) + 1;
      } else {
        return Math.Abs(a.X - b.X) + 1;
      }
    }

    public bool isSameOrientation(Edge other) {
      return forward == other.forward || forward == (other.forward + 2) % 4;
    }

    public long DistanceToOpposite(Edge other) {
      if (forward == DIR_UP || forward == DIR_DOWN) {
        return Math.Abs(a.X - other.a.X) + 1;
      } else {
        return Math.Abs(a.Y - other.a.Y) + 1;
      }
    }

    public bool InRange(Edge other) {
      if (forward == DIR_UP || forward == DIR_DOWN) {
        // Check that the Y ranges overlap
        long top = Math.Min(a.Y, b.Y);
        long bottom = Math.Max(a.Y, b.Y);
        long minY = Math.Min(other.a.Y, other.b.Y);
        long maxY = Math.Max(other.a.Y, other.b.Y);
        if (bottom < minY || maxY < top) {
          return false;
        }
      } else {
        // Check that the X ranges overlap
        long left = Math.Min(a.X, b.X);
        long right = Math.Max(a.X, b.X);
        long minX = Math.Min(other.a.X, other.b.X);
        long maxX = Math.Max(other.a.X, other.b.X);
        if (right < minX || maxX < left) {
          return false;
        }
      }
      return true;
    }

    public bool IsBehind(Edge other) {
      // Based of the normal direction, check if the other edge is behind this edge
      if (forward == DIR_UP) {
        // normal is right
        return other.a.X < a.X;
      } else if (forward == DIR_DOWN) {
        return other.a.X > a.X;
      } else if (forward == DIR_RIGHT) {
        return other.a.Y < a.Y;
      } else { // (forward == DIR_LEFT) {
        return other.a.Y > a.Y;
      }
    }


    public static bool Intersects(Edge e1, Edge e2) {
      long d1 = Vec2.getTurn(e1.a, e1.b, e2.a);
      long d2 = Vec2.getTurn(e1.a, e1.b, e2.b);
      long d3 = Vec2.getTurn(e2.a, e2.b, e1.a);
      long d4 = Vec2.getTurn(e2.a, e2.b, e1.b);

      if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
          ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0))) {
        return true;
      }
      return false;
    }
  }

  public Edge? FindClosestEdge(Edge edge, List<Edge> edges) {
    int N = Reds.Count;
    Edge bestEdge = null;
    long bestDistance = long.MaxValue;

    foreach (var other in edges) {
      if (edge.IsEqual(other)) { continue; }
      if (!edge.isSameOrientation(other)) { continue; }
      if (edge.IsBehind(other)) { continue; }
      if (!edge.InRange(other)) { continue; }
      long distance = edge.DistanceToOpposite(other);
      if (distance < bestDistance) {
        bestDistance = distance;
        bestEdge = other;
      }
    }

    return bestEdge;
  }

  public void CompressRedTiles() {
    // For every point, if the edge is just extended in the same direction just remove it.
    List<Vec2> compressed = [];
    int N = Reds.Count;
    for (int i = 0; i < N; i++) {
      Vec2 prev = Reds[Mod(i - 1, N)];
      Vec2 current = Reds[i];
      Vec2 next = Reds[Mod(i + 1, N)];

      long turn = Vec2.getTurn(prev, current, next);
      if (turn != 0) {
        compressed.Add(current);
      }
    }
    Reds = compressed;
  }

  public void Solve2() {
    CompressRedTiles();
    long maxArea = 0;
    int N = Reds.Count;

    int startIndex = Reds
      .Select((x, i) => i)
      .OrderBy(i => Reds[i].Y)
      .ThenBy(i => Reds[i].X)
      .First();
    Vec2 current = Reds[startIndex];
    Vec2 next = Reds[Mod(startIndex + 1, N)];
    int forward = DIR_RIGHT;
    if (next.Y > current.Y) {
      forward = DIR_DOWN;
    } else if (next.X > current.X) {
      forward = DIR_RIGHT;
    } else {
      Debug.Assert(false);
    }

    // Create the list of edges with the forward direction.
    int index = startIndex;
    List<Edge> edges = [];
    for (int i = 0; i < N; i++) {
      Vec2 a = Reds[index];
      Vec2 b = Reds[Mod(index + 1, N)];
      Vec2 c = Reds[Mod(index + 2, N)];
      edges.Add(new Edge(a, b, forward));

      long turn = Vec2.getTurn(b, a, c);
      if (turn > 0) {
        forward = Mod(forward + 1, 4);
      } else if (turn < 0) {
        forward = Mod(forward - 1, 4);
      }
      index = Mod(index + 1, N);
    }

    foreach (var edge in edges) {
      Edge? opposite = FindClosestEdge(edge, edges);
      if (opposite != null) {
        long height = edge.DistanceToOpposite(opposite);
        long width = edge.Length();
        long area = width * height;
        maxArea = Math.Max(area, maxArea);
      }
    }

    Console.WriteLine(maxArea);
  }

  public void Run() {
    _ReadInput();
    Solve2();

    // 122770129
    // 1578115935
    // long answer = SolvePart2Compressed(Reds.Select(v => new Pt(v.X, v.Y)).ToList());
    // Console.WriteLine(answer);
  }
}