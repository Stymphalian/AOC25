// 2025/12/04 9:00
// 2025/12/04 10:22
// https://adventofcode.com/2025/day/5

public class Day05 {
    public class Range(long Start, long End) {
        public long Start = Start;
        public long End = End;

        public bool Overlaps(Range other) {
            long start2 = Math.Max(Start, other.Start);
            long end1 = Math.Min(End, other.End);
            return !(start2 > end1);
        }

        public void Merge(Range other) {
            Start = Math.Min(Start, other.Start);
            End = Math.Max(End, other.End);
        }
    }

    public List<Range> Ranges = new();
    public List<long> Ingredients = new();

    private void _ReadInput() {
        string[] lines = File.ReadAllLines("data/input1.txt");
        int mode = 0;
        foreach(var line in lines) {
            if (line == "") {
                mode = 1;
                continue;
            }

            if (mode == 0) {
                var parts = line.Split("-");
                long start = long.Parse(parts[0]);
                long end = long.Parse(parts[1]);
                Ranges.Add(new(start, end));
            } else {
                Ingredients.Add(long.Parse(line));
            }
        }
    }

    public void Solve1() {
        _ReadInput();
        long answer = 0;
        foreach(long ingredient in Ingredients) {

            bool fresh = false;
            foreach(var range in Ranges) {
                if (ingredient >= range.Start && ingredient <= range.End) {
                    fresh = true;
                    break;
                }
            }
            if (fresh) {
                answer += 1;
            }
        }
        Console.WriteLine(answer);
    }

    public void Solve2() {
        _ReadInput();

        List<Range> sortedRanges = Ranges.OrderBy((range) => range.End - range.Start).ToList();
        List<Range> inside = [];
        foreach(Range candidate in Ranges) {

            bool added = false;
            for(int currentIndex = 0; currentIndex < inside.Count; currentIndex++) {
                Range current = inside[currentIndex];
                if (current.Overlaps(candidate)) {
                    current.Merge(candidate);

                    // Merge right
                    int right = currentIndex + 1;
                    while (right < inside.Count) {
                        if (inside[right].Overlaps(current)) {
                            current.Merge(inside[right]);
                            inside.RemoveAt(right);
                            continue;
                        }
                        break;
                    }

                    added = true;
                    break;
                } else if (candidate.End < current.Start) {
                    inside.Insert(currentIndex, candidate);
                    added = true;
                    break;
                }
            }
            if(!added) {
                inside.Add(candidate);
            }
        }

        long answer = 0;
        foreach(Range range in inside) {
            answer += range.End - range.Start + 1;
        }
        Console.WriteLine(answer);
    }

    public void Run() {
        Solve2();
    }
}