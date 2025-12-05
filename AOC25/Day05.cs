// 2025/12/04 9:00
// 2025/12/04 10:22
// https://adventofcode.com/2025/day/5

public class Day05 {
    public class Range(long Begin, long End) {
        public long Begin = Begin;
        public long End = End;

        public bool Overlaps(Range other) {
            long start2 = Math.Max(Begin, other.Begin);
            long end1 = Math.Min(End, other.End);
            return !(start2 > end1);
        }

        public void Merge(Range other) {
            Begin = Math.Min(Begin, other.Begin);
            End = Math.Max(End, other.End);
        }

        public Range MergeNew(Range other) {
            return new Range(
                Math.Min(Begin, other.Begin),
                Math.Max(End, other.End)
            );
        }
    }

    public List<Range> Ranges = new();
    public List<long> Ingredients = new();

    private void _ReadInput() {
        string[] lines = File.ReadAllLines("data/input3.txt");
        int mode = 0;
        Ranges = new();
        Ingredients = new();
        foreach (string line in lines) {
            if (line == "") {
                mode = 1;
                continue;
            }

            if (mode == 0) {
                string[] parts = line.Split("-");
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
        foreach (long ingredient in Ingredients) {

            bool fresh = false;
            foreach (var range in Ranges) {
                if (ingredient >= range.Begin && ingredient <= range.End) {
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

        List<Range> sortedRanges = Ranges.OrderBy((range) => range.End - range.Begin).ToList();
        List<Range> inside = [];
        foreach (Range candidate in Ranges) {

            bool added = false;
            for (int currentIndex = 0; currentIndex < inside.Count; currentIndex++) {
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
                } else if (candidate.End < current.Begin) {
                    inside.Insert(currentIndex, candidate);
                    added = true;
                    break;
                }
            }
            if (!added) {
                inside.Add(candidate);
            }
        }

        long answer = 0;
        foreach (Range range in inside) {
            answer += range.End - range.Begin + 1;
        }
        Console.WriteLine(answer);
    }


    public class Node(Range current) {
        public Range Segment = current;
        public Node? Left = null;
        public Node? Right = null;

        private void AddLeft(Range candidate) {
            if (Left == null) {
                // BELOW: An optimization
                // if (candidate.End + 1 >= Segment.Begin) {
                //     Segment.Begin = candidate.Begin;
                // } else {
                //     Left = new Node(candidate);
                // }
                Left = new Node(candidate);
            } else {
                Left.Add(candidate);
            }
        }

        private void AddRight(Range candidate) {
            if (Right == null) {
                // BELOW: An optimization
                // if (candidate.Begin - 1 <= Segment.End) {
                //     Segment.End = candidate.End;
                // } else {
                //     Right = new Node(candidate);    
                // }
                Right = new Node(candidate);
            } else {
                Right.Add(candidate);
            }
        }

        public void Add(Range candidate) {

            if (Segment.Overlaps(candidate)) {
                long prevStart = Segment.Begin;
                long prevEnd = Segment.End;
                long nextStart = Math.Min(Segment.Begin, candidate.Begin);
                long nextEnd = Math.Max(Segment.End, candidate.End);

                if (nextStart != prevStart) {
                    Range LeftSegment = new(
                        nextStart,
                        Math.Max(nextStart, prevStart) - 1
                    );
                    AddLeft(LeftSegment);
                }
                if (nextEnd != prevEnd) {
                    Range RightSegment = new(
                        Math.Min(nextEnd, prevEnd) + 1,
                        nextEnd
                    );
                    AddRight(RightSegment);
                }
            } else if (candidate.Begin > Segment.End) {
                AddRight(candidate);
            } else if (candidate.End < Segment.Begin) {
                AddLeft(candidate);
            }

            // Merge children into parent if the fully overlap.
            if (Left != null && Left.Segment.End + 1 >= Segment.Begin) {
                Segment.Merge(Left.Segment);
                Left = Left.Left;
            }
            if (Right != null && Right.Segment.Begin - 1 <= Segment.End) {
                Segment.Merge(Right.Segment);
                Right = Right.Right;
            }
        }


        public long Count() {
            long count = Segment.End - Segment.Begin + 1;
            if (Left != null) {
                count += Left.Count();
            }
            if (Right != null) {
                count += Right.Count();
            }
            return count;
        }
    }

    public void Solve2_1() {
        _ReadInput();

        Node? root = null;
        foreach (Range range in Ranges) {
            if (root == null) {
                root = new Node(range);
            } else {
                root.Add(range);
            }
        }

        long answer = root!.Count();
        Console.WriteLine(answer);
    }


    public void GenerateTestData() {
        Random rand = new Random(12345);
        using StreamWriter writer = new StreamWriter("data/input3.txt");
        for (int i = 0; i < 100000000; i++) {
            long start = rand.NextInt64(0, 10000000000);
            long end = start + rand.NextInt64(0, 1000000);
            writer.WriteLine($"{start}-{end}");
        }
        // writer.WriteLine();
        // for (int i = 0; i < 500; i++) {
        //     long ingredient = rand.Next(0, 10500);
        //     writer.WriteLine(ingredient);
        // }
    }

    public void Run() {
        GenerateTestData();
        // time the Solve2 and solve2_1
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Solve2();
        stopwatch.Stop();
        Console.WriteLine($"Solve2 took {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Restart();
        Solve2_1();
        stopwatch.Stop();
        Console.WriteLine($"Solve2_1 took {stopwatch.ElapsedMilliseconds} ms");
    }
}