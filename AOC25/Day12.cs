// 2025/12/12 09:52
// 2025/12/12
// https://adventofcode.com/2025/day/12
// This question is dumb shit.

using System.Diagnostics;

public class Day12 {

  public class Shape {
    public List<List<int>> Area = new();
    public int Width { get { return Area[0].Count; } }
    public int Height { get { return Area.Count; } }
    public List<Shape> Variations = new();
    public int Number = -1;

    public void AddLine(string line) {
      Area.Add(line.ToList().Select(x => x == '#' ? 1 : 0).ToList());
    }

    public int _count = -1;
    public int Active() {
      if (_count != -1) {
        return _count;
      }
      int count = 0;
      for (int r = 0; r < Height; r++) {
        for (int c = 0; c < Width; c++) {
          if (Area[r][c] == 1) {
            count += 1;
          }
        }
      }
      _count = count;
      return count;
    }

    public Shape Rotate90() {
      Shape newShape = new();
      for (int c = 0; c < Width; c++) {
        List<int> newRow = new();
        for (int r = Height - 1; r >= 0; r--) {
          newRow.Add(Area[r][c]);
        }
        newShape.Area.Add(newRow);
      }
      newShape.Number = this.Number;
      return newShape;
    }

    public Shape FlipHorizontal() {
      Shape newShape = new();
      for (int r = 0; r < Height; r++) {
        List<int> newRow = new();
        for (int c = Width - 1; c >= 0; c--) {
          newRow.Add(Area[r][c]);
        }
        newShape.Area.Add(newRow);
      }
      newShape.Number = this.Number;
      return newShape;
    }

    public Shape FlipVertical() {
      Shape newShape = new();
      for (int r = Height - 1; r >= 0; r--) {
        List<int> newRow = new();
        for (int c = 0; c < Width; c++) {
          newRow.Add(Area[r][c]);
        }
        newShape.Area.Add(newRow);
      }
      newShape.Number = this.Number;
      return newShape;
    }

    public List<Shape> getVariations() {
      List<Shape> variations = new();
      Shape current = this;
      for (int i = 0; i < 4; i++) {
        variations.Add(current);
        variations.Add(current.FlipHorizontal());
        variations.Add(current.FlipVertical());
        current = current.Rotate90();
      }

      // Remove duplicates
      variations = variations
        .DistinctBy(s => s.ToString2())
        .ToList();
      return variations;
    }

    public IEnumerable<(int, int)> GetTranslatedActives(int row, int col) {
      for (int c = 0; c < Height; c++) {
        for (int r = 0; r < Width; r++) {
          int newRow = row + r;
          int newCol = col + c;

          if (Area[r][c] == 1) {
            yield return (newRow, newCol);
          }
        }
      }
    }

    public string ToString2() {
      string result = "";
      for (int row = 0; row < Area.Count; row++) {
        result += string.Join(" ", Area[row]) + "\n";
      }
      return result;
    }
  }

  public class Region() {
    public int Width;
    public int Height;
    public List<int> Presents = new();
    public HashSet<(int, int)> Area = new();

    public string ToString2() {
      string result = "";
      for (int row = 0; row < Height; row++) {
        for (int col = 0; col < Width; col++) {
          result += Area.Contains((row, col)) ? "." : "#";
        }
        result += "\n";
      }
      return result;
    }
  }

  public List<Shape> Shapes = new();
  public List<Region> Regions = new();

  private void _ReadInput() {
    string filepath = "data/input1.txt";
    string[] lines = File.ReadAllLines(filepath);
    int lineIndex = 0;
    while (lineIndex < lines.Length) {
      string line = lines[lineIndex];

      if (line.Contains(":")) {
        if (line.Contains("x")) {
          // Region defintion
          var parts = line.Split(": ");
          var dimsParts = parts[0].Split("x");
          int Width = int.Parse(dimsParts[0]);
          int Height = int.Parse(dimsParts[1]);
          Region region = new Region();
          region.Width = Width;
          region.Height = Height;

          var presentsParts = parts[1].Split(" ");
          foreach (var presentPart in presentsParts) {
            region.Presents.Add(int.Parse(presentPart));
          }
          Regions.Add(region);

        } else {
          // Shape definition
          Shape shape = new();
          shape.AddLine(lines[lineIndex + 1]);
          shape.AddLine(lines[lineIndex + 2]);
          shape.AddLine(lines[lineIndex + 3]);
          lineIndex += 3;
          shape.Number = Shapes.Count + 1;
          shape.Variations = shape.getVariations();
          Shapes.Add(shape);
        }

      }
      lineIndex += 1;
    }
  }

  public HashSet<(int, int)> MakeEmptyArea(Region region) {
    HashSet<(int, int)> Area = new();
    for (int row = 0; row < region.Height; row++) {
      for (int col = 0; col < region.Width; col++) {
        Area.Add((row, col));
      }
    }
    return Area;
  }

  public int getNextPresent(List<int> Presents) {
    for (int index = 0; index < Presents.Count; index++) {
      if (Presents[index] != 0) {
        return index;
      }
    }
    return -1;
  }

  public bool CanPut(Region region, Shape shape, int row, int col) {
    foreach (var (newRow, newCol) in shape.GetTranslatedActives(row, col)) {
      if (newRow < 0 || newRow >= region.Height || newCol < 0 || newCol >= region.Width) {
        return false;
      }
      if (!region.Area.Contains((newRow, newCol))) {
        return false;
      }
    }
    return true;
  }

  public void StampShape(Region region, Shape shape, int row, int col, int value) {
    foreach (var (newRow, newCol) in shape.GetTranslatedActives(row, col)) {
      if (value == 1) {
        region.Area.Remove((newRow, newCol));
      } else {
        region.Area.Add((newRow, newCol));
      }
    }
  }

  public bool _CanFit(Region region) {
    // Console.WriteLine(region.ToString2());
    int shapeIndex = getNextPresent(region.Presents);
    if (shapeIndex == -1) {
      return true;
    }
    Shape shape = Shapes[shapeIndex];
    region.Presents[shapeIndex] -= 1;

    foreach (var (r, c) in region.Area.ToList()) {
      foreach (var shapeVariant in shape.Variations) {
        if (CanPut(region, shapeVariant, r, c)) {
          StampShape(region, shapeVariant, r, c, 1);
          if (_CanFit(region)) {
            return true;
          }
          StampShape(region, shapeVariant, r, c, 0);
        }
      }
    }

    region.Presents[shapeIndex] += 1;
    return false;
  }

  public bool CanFit(Region region) {
    var totalPresents = region.Presents.Sum();
    var areaForPresents = (region.Width/3) * (region.Height /3);
    if(areaForPresents >= totalPresents) {
      return true;
    }

    var maxSpaces = region.Width * region.Height;
    var neededSpaces = region.Presents
      .Select((count, pi) => count*Shapes[pi].Active())
      .Sum();
    if (neededSpaces > maxSpaces) {
      return false;
    }

    Debug.Assert(false);
    return false;
  }


  public void Solve1() {
    int answer = 0;

    foreach (var region in Regions) {
      region.Area = MakeEmptyArea(region);
      if (CanFit(region)) {
        answer += 1;
      }
    }

    Console.WriteLine(answer);

  }

  public void Run() {
    _ReadInput();
    Solve1();
  }
}