// 2025/12/03 11:24
// 2025/12/03 12:01
// https://adventofcode.com/2025/day/3


using Microsoft.VisualBasic;

public class Day04 {
    // public record Grid(List<List<int>> grid);
    // private List<Grid> _cases = new();
    int numRows;
    int numCols;
    List<List<char>> Grid = new();

    private void _ReadInput() {
        string[] lines = File.ReadAllLines("data/input1.txt");
        foreach(var line in lines) {
            Grid.Add(line.ToList());
        }
        numRows = Grid.Count;
        numCols = Grid[0].Count;
    }

    public int CountAccessSlots(List<List<char>> grid, int numRows, int numCols) {
        const char PAPER = '@';
        List<(int, int)> dirs = [
            (-1,-1), (0, -1), (1, -1),
            (-1,0), (1, 0),
            (-1,1), (0, 1), (1, 1)
        ];

        bool OutBounds(int row, int col) {
            return row < 0 || row >= numRows || col < 0 || col >= numCols;
        }

        int count = 0;
        for(int row = 0; row < numRows; row++) {
            for(int col = 0; col < numCols; col++) {
                if (grid[row][col] != PAPER) {continue;}

                int paperCount = 0;
                foreach(var dir in dirs) {
                    int newCol = col + dir.Item1;
                    int newRow = row + dir.Item2;
                    if (OutBounds(newRow, newCol)) {continue;}
                    paperCount += (grid[newRow][newCol] == PAPER) ? 1: 0;
                }

                if (paperCount < 4) {
                    count += 1;
                }
            }
        }

        return count;
    }

    public void Solve1() {
        _ReadInput();
        int answer = CountAccessSlots(Grid, numRows, numCols);
        Console.WriteLine(answer);
    }

    public int CountAccessSlots2(List<List<char>> grid, int numRows, int numCols) {
        const char PAPER = '@';
        const char SPACE = '.';
        List<(int, int)> dirs = [
            (-1,-1), (0, -1), (1, -1),
            (-1,0), (1, 0),
            (-1,1), (0, 1), (1, 1)
        ];

        bool OutBounds(int row, int col) {
            return row < 0 || row >= numRows || col < 0 || col >= numCols;
        }

        int count = 0;
        while(true) {

            int localCount = 0;
            for(int row = 0; row < numRows; row++) {
                for(int col = 0; col < numCols; col++) {
                    if (grid[row][col] != PAPER) {continue;}

                    int paperCount = 0;
                    foreach(var dir in dirs) {
                        int newCol = col + dir.Item1;
                        int newRow = row + dir.Item2;
                        if (OutBounds(newRow, newCol)) {continue;}
                        paperCount += (grid[newRow][newCol] == PAPER) ? 1: 0;
                    }

                    if (paperCount < 4) {
                        grid[row][col] = SPACE;
                        localCount += 1;
                    }
                }
            }

            count += localCount;
            if (localCount == 0) {
                break;
            }
        }
        return count;
    }

    public void Solve2() {
        _ReadInput();
        int answer = CountAccessSlots2(Grid, numRows, numCols);
        Console.WriteLine(answer);
    }

    public void Run() {
        Solve2();
    }
}