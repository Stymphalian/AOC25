// 2025/12/08 9:16
// 2025/12/08 
// https://adventofcode.com/2025/day/9


using System.Data;
using System.Diagnostics;

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

  public class Rect(Vec2 tl, Vec2 br) {
    public Vec2 TopLeft = tl;
    public Vec2 BottomRight = br;
    public Vec2 TopRight => new(BottomRight.X, TopLeft.Y);
    public Vec2 BottomLeft => new(TopLeft.X, BottomRight.Y);

    public bool Intersects(Edge e) {
      long minX = Math.Min(e.a.X, e.b.X); ;
      long maxX = Math.Max(e.a.X, e.b.X);
      long minY = Math.Min(e.a.Y, e.b.Y); ;
      long maxY = Math.Max(e.a.Y, e.b.Y); ;

      if (maxX <= TopLeft.X || minX >= BottomRight.X || maxY <= TopLeft.Y || minY >= BottomRight.Y) {
        // Outside the rectangle
        return false;
      }
      if (minX == maxX) {
        // vertical
        if (minX == TopLeft.X || minX == BottomRight.X) {
          return false;
        }
        return true;
      } else if (minY == maxY) {
        // horizontal
        if (minY == TopLeft.Y || minY == BottomRight.Y) {
          return false;
        }
        return true;
      } else {
        Debug.Assert(false);
      }
      return false;
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

  public Rect? GetRectFromVectors(Vec2 a, Vec2 b, int dirA) {
    long minX = Math.Min(a.X, b.X);
    long maxX = Math.Max(a.X, b.X);
    long minY = Math.Min(a.Y, b.Y);
    long maxY = Math.Max(a.Y, b.Y);

    Vec2 tl = new Vec2(minX, minY);
    Vec2 br = new Vec2(maxX, maxY);
    var rect = new Rect(tl, br);

    if (dirA == DIR_UP) {
      if (!a.IsEqual(rect.BottomLeft)) {
        return null;
      }
    } else if (dirA == DIR_DOWN) {
      if (!a.IsEqual(rect.TopRight)) {
        return null;
      }
    } else if (dirA == DIR_LEFT) {
      if (!a.IsEqual(rect.BottomRight)) {
        return null;
      }
    } else if (dirA == DIR_RIGHT) {
      if (!a.IsEqual(rect.TopLeft)) {
        return null;
      }
    }
    return rect;
  }

  public void Solve2() {
    int N = Reds.Count;

    // Try and find the starting index where the Edge is at the top-left edge
    // of the enclosed polygon. This ensures we know the correct "inner" orientation
    // of the polygon as we walk around it.
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
    // This is to help with rectangle orientation later, as well as to ensure
    // we know the inward facing direction of each edge.
    int index = startIndex;
    List<Edge> edges = [];
    Dictionary<int, int> VertexDirection = [];
    for (int i = 0; i < N; i++) {
      Vec2 a = Reds[index];
      Vec2 b = Reds[Mod(index + 1, N)];
      Vec2 c = Reds[Mod(index + 2, N)];
      edges.Add(new Edge(a, b, forward));
      VertexDirection[index] = forward;

      long turn = Vec2.getTurn(b, a, c);
      if (turn > 0) {
        forward = Mod(forward + 1, 4);
      } else if (turn < 0) {
        forward = Mod(forward - 1, 4);
      }
      index = Mod(index + 1, N);
    }


    long maxArea = 0;
    for (index = startIndex; index != Mod(startIndex - 1, N); index = Mod(index + 1, N)) {

      for (int other = Mod(index + 1, N); other != index; other = Mod(other + 1, N)) {
        Vec2 a = Reds[index];
        Vec2 b = Reds[other];

        if (a.X == b.X || a.Y == b.Y) {
          // Skip lines which are in-line (either vertical or horizontal)
          continue;
        }

        Rect? rect = GetRectFromVectors(a, b, VertexDirection[index]);
        if (rect == null) {
          // skip rectangles which are curved outwards
          continue;
        }

        // Check to see if any edges intersect through the rectangle
        // if they do then we know that atleast some-part of the path crosses
        // inside the proposed rectangle which would make an area "ouside" 
        // the polygon, therfore making it "invalid"
        bool intersects = false;
        foreach (var edge in edges) {
          if (rect.Intersects(edge)) {
            intersects = true;
            break;
          }
        }
        if (intersects) {
          continue;
        }

        // All conditions pass, so get the Area as a candidate for the max area
        long area = rect.TopLeft.Area(rect.BottomRight);
        maxArea = Math.Max(area, maxArea);
      }
    }

    Console.WriteLine(maxArea);
  }

  public void CheckForLoops() {
    HashSet<(int, int)> visited = new();
    foreach (var red in Reds) {
      var key = ((int)red.X, (int)red.Y);
      if (visited.Contains(key)) {
        Console.WriteLine("Loop detected at " + red.X + "," + red.Y);
      }
      visited.Add(key);
    }

    // Check for fold backs
    for (int index = 0; index < Reds.Count; index++) {
      Vec2 a = Reds[index];
      Vec2 b = Reds[Mod(index + 1, Reds.Count)];
      Vec2 c = Reds[Mod(index + 2, Reds.Count)];
      Vec2 d = Reds[Mod(index + 3, Reds.Count)];
      if (a.X == b.X && c.X == d.X) {
        // Vertical
        if (a.X + 1 == c.X || a.X - 1 == c.X) {
          // fold back detected
          Console.WriteLine($"Fold back detected at index {index}");
        }
      } else if (a.Y == b.Y && c.Y == d.Y) {
        // Horizonta
        if (a.Y + 1 == c.Y || a.Y - 1 == c.Y) {
          // fold back detected
          Console.WriteLine($"Fold back detected at index {index}");
        }
      } else {
        // not in the same axis, so they can not be parllel
      }
    }
  }


  public void Run() {
    _ReadInput();
    CheckForLoops();
    Solve2();

    // 122770129
    // 1578115935
  }
}