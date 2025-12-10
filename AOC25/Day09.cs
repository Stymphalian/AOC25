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

  public void Solve2() {
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

    int index = startIndex;
    for (int i = 0; i < N; i++) {
      Vec2 p1 = Reds[index];
      Vec2 p2 = Reds[Mod(index + 1, N)];
      Vec2 p3 = Reds[Mod(index - 1, N)];
      Vec2 p4 = Reds[Mod(index + 2, N)];

      Vec2 other;
      if (forward == DIR_UP) {
        // go right
        List<long> cands = [];
        if (p3.X >= p1.X) { cands.Add(p3.X); }
        if (p4.X >= p2.X) { cands.Add(p4.X); }
        long candX = (cands.Count == 0) ? p1.X : cands.Min();
        other = new(candX, p2.Y);
      } else if (forward == DIR_RIGHT) {
        // go down
        List<long> cands = [];
        if (p3.Y >= p1.Y) { cands.Add(p3.Y); }
        if (p4.Y >= p2.Y) { cands.Add(p4.Y); }
        long candY = (cands.Count == 0) ? p1.Y : cands.Min();
        other = new(p2.X, candY);
      } else if (forward == DIR_DOWN) {
        // go left
        List<long> cands = [];
        if (p3.X <= p1.X) { cands.Add(p3.X); }
        if (p4.X <= p2.X) { cands.Add(p4.X); }
        long candX = (cands.Count == 0) ? p1.X : cands.Max();
        other = new(candX, p2.Y);
      } else { // (forward == DIR_LEFT) {
        // go up
        List<long> cands = [];
        if (p3.Y <= p1.Y) { cands.Add(p3.Y); }
        if (p4.Y <= p2.Y) { cands.Add(p4.Y); }
        long candY = (cands.Count == 0) ? p1.Y : cands.Max();
        other = new(p2.X, candY);
      }

      long turn = Vec2.getTurn(p2, p1, p4);
      if (turn > 0) {
        forward = Mod(forward + 1, 4);
      } else if (turn < 0) {
        forward = Mod(forward - 1, 4);
      }

      long area = p1.Area(other);
      maxArea = Math.Max(area, maxArea);
      index = Mod(index + 1, N);
    }


    Console.WriteLine(maxArea);
  }

  public void Run() {
    _ReadInput();
    Solve2();
  }
}