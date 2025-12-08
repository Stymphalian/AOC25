// 2025/12/07 10:06
// 2025/12/07 11:54
// https://adventofcode.com/2025/day/8

public class Day08 {

  public record Vec3(long X, long Y, long Z) {

    public Vec3 Sub(Vec3 o) {
      return new Vec3(X - o.X, Y - o.Y, Z - o.Z);
    }

    public double Distance(Vec3 other) {
      Vec3 v = Sub(other);
      return Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
    }
  }

  public class UnionFind {
    private readonly int[] Parents;
    private int[] Ranks;
    public int NumElements;

    public UnionFind(int n) {
      NumElements = n;
      Parents = new int[n];
      Ranks = new int[n];
      for (int i = 0; i < n; i++) {
        Parents[i] = i;
        Ranks[i] = 0;
      }
    }

    public int Find(int x) {
      if (Parents[x] != x) {
        Parents[x] = Find(Parents[x]); // Path compression
      }
      return Parents[x];
    }

    public void Union(int a, int b) {
      int rootA = Find(a);
      int rootB = Find(b);
      if (rootA == rootB) {
        return;
      }

      if (Ranks[rootA] <= Ranks[rootB]) {
        Parents[rootA] = rootB;
        Ranks[rootB] += 1;
      } else {
        Parents[rootB] = rootA;
        Ranks[rootA] += 1;
      }
      NumElements -= 1;
    }
  }

  List<Vec3> Boxes = [];
  int NumberConnections;

  private void _ReadInput() {
    string filepath = "data/input1.txt";
    string[] lines = File.ReadAllLines(filepath);
    if (filepath.Contains("input")) {
      NumberConnections = 1000;
    } else {
      NumberConnections = 10;
    }


    foreach (var line in lines) {
      string[] parts = line.Split(',');
      int x = int.Parse(parts[0]);
      int y = int.Parse(parts[1]);
      int z = int.Parse(parts[2]);
      Boxes.Add(new Vec3(x, y, z));
    }
  }

  public void Solve1() {
    PriorityQueue<(int, int), double> pq = new();
    for (int a = 0; a < Boxes.Count; a++) {
      for (int b = a + 1; b < Boxes.Count; b++) {
        double dist = Boxes[a].Distance(Boxes[b]);
        pq.Enqueue((a, b), dist);
      }
    }
    UnionFind uf = new(Boxes.Count);

    for (int i = 0; i < NumberConnections; i++) {
      (int a, int b) = pq.Dequeue();
      if (uf.Find(a) == uf.Find(b)) {
        continue;
      }
      uf.Union(a, b);
    }

    // Find the largest circuits
    Dictionary<int, int> circuitCount = new();
    for (int box = 0; box < Boxes.Count; box++) {
      int root = uf.Find(box);
      circuitCount.TryAdd(root, 0);
      circuitCount[root] += 1;
    }

    int answer = circuitCount
      .Values
      .OrderByDescending(a => a)
      .Take(3)
      .Aggregate((acc, current) => acc * current);
    Console.WriteLine(answer);
  }

  public void Solve2() {
    PriorityQueue<(int, int), double> pq = new();
    for (int a = 0; a < Boxes.Count; a++) {
      for (int b = a + 1; b < Boxes.Count; b++) {
        double dist = Boxes[a].Distance(Boxes[b]);
        pq.Enqueue((a, b), dist);
      }
    }
    UnionFind uf = new(Boxes.Count);

    int outA = 0;
    int outB = 0;
    while (true) {
      (int a, int b) = pq.Dequeue();
      uf.Union(a, b);
      if (uf.NumElements == 1) {
        outA = a;
        outB = b;
        break;
      }
    }

    long answer = Boxes[outA].X * Boxes[outB].X;
    Console.WriteLine(answer);
  }

  public void Run() {
    _ReadInput();
    Solve2();
  }
}