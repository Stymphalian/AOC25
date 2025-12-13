// 2025/12/10 18:00
// 2025/12/12 21:38
// https://adventofcode.com/2025/day/10

using System.Data;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

public class Machine {
  public List<bool> Config = new();
  public List<List<int>> Buttons = new();
  public List<int> Joltage = new();
  public uint ConfigNumber = 0;
  public List<uint> ButtonsAsNumbers = new();

  public static uint ConfigToUInt(List<bool> config) {
    uint result = 0;
    for (int i = config.Count - 1; i >= 0; i--) {
      if (config[i]) {
        result |= (uint)1 << i;
      }
    }
    return result;
  }

  public static uint ButtonsToUInt(List<int> buttons) {
    uint result = 0;
    for (int i = 0; i < buttons.Count; i++) {
      result |= (uint)1 << buttons[i];
    }
    return result;
  }

  public Machine(string line) {
    // [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
    var parts = line.Split(" ");

    foreach (var part in parts) {
      if (part.StartsWith("[") && part.EndsWith("]")) {
        // Parse Config
        for (int i = 1; i < part.Length - 1; i++) {
          if (part[i] == '#') {
            Config.Add(true);
          } else {
            Config.Add(false);
          }
        }
      } else if (part.StartsWith("(") && part.EndsWith(")")) {
        // Parse buttons
        var nums = part
          .Substring(1, part.Length - 2)
          .Split(',')
          .Select(s => int.Parse(s))
          .ToList();
        Buttons.Add(nums);
      } else if (part.StartsWith("{") && part.EndsWith("}")) {
        // Parse joltage
        var nums = part
          .Substring(1, part.Length - 2)
          .Split(',')
          .Select(s => int.Parse(s))
          .ToList();
        Joltage = nums;
      }
    }

    ConfigNumber = ConfigToUInt(Config);
    ButtonsAsNumbers = Buttons.Select(b => ButtonsToUInt(b)).ToList();
  }
}


public class Day10 {

  List<Machine> Machines = [];
  private void _ReadInput() {
    string filepath = "data/input1.txt";
    string[] lines = File.ReadAllLines(filepath);
    foreach (string line in lines) {
      Machines.Add(new Machine(line));
    }
  }

  public bool IsInteger(double value) {
    return Math.Abs(value - Math.Round(value)) < 1e-9;
  }

  public IEnumerable<List<int>> GenerateCombinations(int index, int count) {
    if (index == count) {
      yield return [index];
      yield return [];
    } else {
      foreach (var chosen in GenerateCombinations(index + 1, count)) {
        yield return chosen;

        chosen.Add(index);
        yield return chosen;
        chosen.Remove(index);
      }
    }
  }

  public void PrintList(List<int> ls) {
    Console.Write("[");
    foreach (var c in ls) {
      Console.Write(c);
      Console.Write(",");
    }
    Console.WriteLine("]");
  }

  public int GetButtonPresses(Machine machine) {
    int minButtons = int.MaxValue;
    foreach (var combo in GenerateCombinations(0, machine.Buttons.Count - 1)) {
      uint result = machine.ConfigNumber;
      foreach (var bi in combo) {
        result ^= machine.ButtonsAsNumbers[bi];
      }

      if (result == 0) {
        minButtons = Math.Min(minButtons, combo.Count);
      }
    }
    return minButtons;
  }

  public void Solve1() {
    int answer = 0;
    foreach (var machine in Machines) {
      answer += GetButtonPresses(machine);
    }
    Console.WriteLine(answer);
  }

  public List<int> MinIndex(List<int> ls) {
    int minIndex = -1;
    for (int i = 0; i < ls.Count; i++) {
      if (ls[i] == 0) { continue; }
      if (minIndex == -1 || ls[i] < ls[minIndex]) {
        minIndex = i;
      }
    }
    if (minIndex == -1) {
      return [];
    }

    List<int> minIndices = [];
    for (int index = 0; index < ls.Count; index++) {
      if (ls[index] == ls[minIndex]) {
        minIndices.Add(index);
      }
    }
    return minIndices;
  }

  public void ApplyButton(List<int> state, List<int> button, int incr = -1) {
    foreach (var toggleIndex in button) {
      state[toggleIndex] += incr;
    }
  }

  public (Matrix<double>, Vector<double> y, int) SolveSystemOfEquations(Matrix<double> A, Vector<double> y) {
    // For underdetermined system with integer constraint:
    // Try all non-negative integer combinations up to reasonable bounds
    int numRows = A.RowCount;
    int numCols = A.ColumnCount;
    Debug.Assert(A.RowCount == y.Count);


    void SwapRows(int i, int j) {
      for (int col = 0; col < numCols; col++) {
        double temp = A[i, col];
        A[i, col] = A[j, col];
        A[j, col] = temp;
      }
      double tempY = y[i];
      y[i] = y[j];
      y[j] = tempY;
    }

    void MultRow(int i, double factor) {
      for (int col = 0; col < numCols; col++) {
        A[i, col] *= factor;
      }
      y[i] *= factor;
    }

    void AddRowMult(int sourceRow, int targetRow, double factor) {
      for (int col = 0; col < numCols; col++) {
        A[targetRow, col] += A[sourceRow, col] * factor;
      }
      y[targetRow] += y[sourceRow] * factor;
    }

    bool ColIsZero(int col) {
      for (int row = 0; row < numRows; row++) {
        if (A[row, col] != 0) {
          return false;
        }
      }
      return true;
    }

    int FindNonZeroRowBelow(int row, int col) {
      int candidateRow = -1;
      for (int r = row + 1; r < numRows; r++) {
        if (A[r, col] == 1.0) {
          // Prefer a row with a leading 1
          return r;
        } else if (A[r, col] == -1.0) {
          // Prefer a row with a leading -1
          return r;
        } else if (A[r, col] != 0) {
          candidateRow = r;
        }
      }
      return candidateRow;
    }

    bool CheckZerosBelow(int row, int col) {
      for (int r = row + 1; r < numRows; r++) {
        if (A[r, col] != 0) {
          return false;
        }
      }
      return true;
    }

    int FindRowBelowWithLeadingOne(int row, int col) {
      for (int r = row + 1; r < numRows; r++) {
        if (A[r, col] == 1.0 || A[r, col] == -1.0) {
          return r;
        }
      }
      return -1;
    }

    void ConvertToRowEchelonForm() {
      int pivot = -1;
      for (int col = 0; col < numCols; col++) {
        Debug.Assert(!ColIsZero(col));

        if (CheckZerosBelow(pivot, col)) {
          // This column is already done, keep going
          continue;
        }
        pivot += 1;

        // Swap the pivot row into position
        if (A[pivot, col] == 0) {
          int nonZeroRow = FindNonZeroRowBelow(pivot, col);
          Debug.Assert(nonZeroRow != -1);
          SwapRows(pivot, nonZeroRow);
        }

        // Make the leading cell a 1.0
        // If a row with a leading Abs(1.0) exists, swap it instead of dividing
        int candSwitch = FindRowBelowWithLeadingOne(pivot, col);
        if (candSwitch != -1) {
          SwapRows(pivot, candSwitch);
        }
        MultRow(pivot, 1.0 / A[pivot, col]);

        // Make all the rows below the leading cell zeroes
        for (int r = pivot + 1; r < numRows; r++) {
          AddRowMult(pivot, r, -A[r, col]);
        }
      }
    }

    int findPivotColumnInRow(int r) {
      // Find the pivot column
      int pivot = -1;
      for (int c = 0; c < numCols; c++) {
        if (A[r, c] == 1.0) {
          pivot = c;
          break;
        }
      }
      return pivot;
    }

    void ConvertToReducedRowEchelonForm() {
      for (int r = 0; r < numRows; r++) {

        int pivot = findPivotColumnInRow(r);
        if (pivot == -1) { continue; }

        // Eliminate all rows above
        for (int upperRow = 0; upperRow < r; upperRow++) {
          AddRowMult(r, upperRow, -A[upperRow, pivot]);
        }
      }
    }

    int CountNumberFreeVariables() {
      int numPivots = 0;
      for (int r = 0; r < numRows; r++) {
        int pivot = findPivotColumnInRow(r);
        if (pivot != -1) {
          numPivots += 1;
        }
      }
      return numCols - numPivots;
    }

    // Console.WriteLine("Original Matrix A:");
    // Console.WriteLine(A.ToString());
    // Console.WriteLine(y.ToString());
    ConvertToRowEchelonForm();
    // Console.WriteLine("REF Matrix A:");
    // Console.WriteLine(A.ToString());
    // Console.WriteLine(y.ToString());
    ConvertToReducedRowEchelonForm();
    // Console.WriteLine("RREF Matrix A:");
    // Console.WriteLine(A.ToString());
    // Console.WriteLine(y.ToString());
    int freeVars = CountNumberFreeVariables();
    // Console.WriteLine($"Number of free variables: {CountNumberFreeVariables()}");
    // if (freeVars == 0) {
    //   Console.WriteLine(A.ToString());
    //   Console.WriteLine(y.ToString());
    // }

    return (A, y, freeVars);
  }

  public Vector<double> BackSubstituteMatrix(Matrix<double> A, Vector<double> y) {
    int numRows = A.RowCount;
    int numCols = A.ColumnCount;
    Debug.Assert(A.RowCount == y.Count);

    double[] solution = new double[numCols];
    for (int r = numRows - 1; r >= 0; r--) {
      // Find the pivot column
      int pivotCol = -1;
      for (int c = 0; c < numCols; c++) {
        if (A[r, c] == 1.0) {
          pivotCol = c;
          break;
        }
      }
      if (pivotCol == -1) {
        // All-zero row
        continue;
      }

      // solution[pivotCol] = (int)Math.Round(y[r]);
      solution[pivotCol] = y[r];
    }

    return DenseVector.OfArray(solution);
  }

  public int HandleFreeVars(
    Machine machine,
    Matrix<double> A,
    Vector<double> y
  ) {
    // Need to try all combinations of free variables up to reasonable bounds
    int numRows = A.RowCount;
    int numCols = A.ColumnCount;
    Debug.Assert(A.RowCount == y.Count);


    int GetPivotInRow(int r) {
      for (int c = 0; c < numCols; c++) {
        if (A[r, c] != 0.0) {
          return c;
        }
      }
      return -1;
    }

    List<(int, int)> pivots = [];
    for (int r = 0; r < numRows; r++) {
      int pivot = GetPivotInRow(r);
      if (pivot == -1) { continue; }
      pivots.Add((r, pivot));
    }

    List<int> freeVariableCols = [];
    for (int col = 0; col < numCols; col++) {
      if (pivots.Any(p => p.Item2 == col)) {
        // this is a pivot column, so just ignore
        continue;
      }
      for (int r = 0; r < numRows; r++) {
        if (A[r, col] != 0.0) {
          freeVariableCols.Add(col);
          break;
        }
      }
    }

    List<int> freeVarLimits = [];
    foreach (var buttonIndex in freeVariableCols) {
      var button = machine.Buttons[buttonIndex];
      int minValue = int.MaxValue;
      foreach (var toggleIndex in button) {
        minValue = Math.Min(minValue, machine.Joltage[toggleIndex]);
      }
      freeVarLimits.Add(minValue);
    }


    // Make a copy of A and y which we can modify
    var A_copy = A.Clone();
    foreach (var freeCol in freeVariableCols) {
      // Zero out the free variable columns in A_copy
      for (int r = 0; r < numRows; r++) {
        A_copy[r, freeCol] = 0.0;
      }
    }

    IEnumerable<List<int>> GetCands(int index, List<int> limits) {
      if (index == limits.Count) {
        yield return [];
      } else {
        for (int value = 0; value <= limits[index]; value++) {
          foreach (var cand in GetCands(index + 1, limits)) {
            var newCand = new List<int>(cand);
            newCand.Insert(0, value);
            yield return newCand;
          }
        }
      }
    }

    bool CheckXSolution(Machine machine, Vector<double> x) {
      Debug.Assert(x.Count == machine.Buttons.Count);
      // Double check the solution of X makes the Joltage state
      List<int> state = new(machine.Joltage);
      for (int buttonIndex = 0; buttonIndex < machine.Buttons.Count; buttonIndex++) {
        // Check that the x[buttonIndex] is an integer
        if (!IsInteger(x[buttonIndex])) {
          return false;
        }

        // Apply the button presses
        int presses = (int)Math.Round(x[buttonIndex]);

        // Can't press a button a negative number of times
        if (presses < 0) {
          return false;
        }
        ApplyButton(state, machine.Buttons[buttonIndex], -presses);
      }
      return state.Sum() == 0;
    }

    // Console.Write(A.ToString());
    // Console.Write(y.ToString());
    // Console.WriteLine($"Pivots: {string.Join(", ", pivots.Select(p => p.ToString()))}");
    // Console.WriteLine($"Free variable columns: {string.Join(", ", freeVariableCols)}");
    // Console.WriteLine($"Free variable limits: {string.Join(", ", freeVarLimits)}");
    // Console.WriteLine($"Number of free variable combinations to try: {freeVarLimits.Aggregate(1, (a, b) => a * (b + 1))}");
    // Console.WriteLine($"Joltage state: {string.Join(", ", machine.Joltage)}");

    int minPresses = int.MaxValue - 1;
    foreach (var cand in GetCands(0, freeVarLimits)) {
      var y_copy = y.Clone();

      // Subtract the free variable contributions from y_copy
      for (int index = 0; index < freeVariableCols.Count; index++) {
        int freeCol = freeVariableCols[index];
        int freeValue = cand[index];

        for (int r = 0; r < numRows; r++) {
          y_copy[r] -= A[r, freeCol] * freeValue;
        }
      }

      var x = BackSubstituteMatrix(A_copy, y_copy);
      for (int index = 0; index < freeVariableCols.Count; index++) {
        int freeCol = freeVariableCols[index];
        int freeValue = cand[index];
        x[freeCol] = freeValue;
      }
      var x_cand = x.ToList().Sum();
      if (IsInteger(x_cand) && x_cand <= minPresses) {
        if (CheckXSolution(machine, x)) {
          minPresses = Math.Min(minPresses, (int)Math.Round(x_cand));
        }
      }
    }

    if (minPresses == int.MaxValue - 1) {
      Console.Write(A.ToString());
      Console.Write(y.ToString());
      Console.WriteLine($"Pivots: {string.Join(", ", pivots.Select(p => p.ToString()))}");
      Console.WriteLine($"Free variable columns: {string.Join(", ", freeVariableCols)}");
      Console.WriteLine($"Free variable limits: {string.Join(", ", freeVarLimits)}");
      Console.WriteLine($"Number of free variable combinations to try: {freeVarLimits.Aggregate(1, (a, b) => a * (b + 1))}");
      Console.WriteLine($"Joltage state: {string.Join(", ", machine.Joltage)}");
    }


    return minPresses;
  }

  public int GetJoltageButtonPresses(Machine machine) {
    double[,] _A = new double[machine.Joltage.Count, machine.Buttons.Count];
    for (int buttonIndex = 0; buttonIndex < machine.Buttons.Count; buttonIndex++) {
      foreach (var toggleIndex in machine.Buttons[buttonIndex]) {
        _A[toggleIndex, buttonIndex] = 1.0;
      }
    }
    double[] _y = new double[machine.Joltage.Count];
    for (int ji = 0; ji < machine.Joltage.Count; ji++) {
      _y[ji] = machine.Joltage[ji];
    }

    var A = DenseMatrix.OfArray(_A);
    var y = DenseVector.OfArray(_y);
    var (A1, y1, freeVars) = SolveSystemOfEquations(A, y);
    if (freeVars == 0) {
      var x = BackSubstituteMatrix(A1, y1);

      // Double check the solution of X makes the Joltage state
      List<int> state = new(machine.Joltage);
      for (int buttonIndex = 0; buttonIndex < machine.Buttons.Count; buttonIndex++) {
        int presses = (int)Math.Round(x[buttonIndex]);
        ApplyButton(state, machine.Buttons[buttonIndex], -presses);
      }
      Debug.Assert(state.Sum() == 0);

      var result = x.ToList().Sum();
      return (int)result;
    } else {
      int result = HandleFreeVars(machine, A1, y1);
      return result;
    }
  }

  public void Solve2() {
    int answer = 0;
    for (int index = 0; index < Machines.Count; index++) {
      int cand = GetJoltageButtonPresses(Machines[index]);
      Console.WriteLine($"{index}: {cand}");
      answer += cand;
    }
    Console.WriteLine(answer);
  }

  public void Run() {
    _ReadInput();
    Solve2();
  }
}