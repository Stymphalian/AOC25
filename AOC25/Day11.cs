// 2025/12/12 09:52
// 2025/12/12
// https://adventofcode.com/2025/day/11

using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using MathNet.Numerics.Optimization;

public class Day11 {

  public const string YOU = "you";
  public const string OUT = "out";
  public const string DAC = "dac";
  public const string FFT = "fft";
  public const string SVR = "svr";

  Dictionary<string, List<string>> Nodes = new();

  private void _ReadInput() {
    string filepath = "data/input1.txt";
    string[] lines = File.ReadAllLines(filepath);
    foreach (string line in lines) {
      var parts = line.Split(": ");
      string key = parts[0];
      string[] children = parts[1].Split(" ");
      Nodes[key] = children.ToList();

      foreach (var child in children) {
        if (!Nodes.ContainsKey(child)) {
          Nodes[child] = [];
        }
      }
    }
  }


  public int BFSCount(string current, HashSet<string> visited) {
    if (current == OUT) { return 1; }

    int answer = 0;
    visited.Add(current);
    foreach (var child in Nodes[current]) {
      if (visited.Contains(child)) { continue; }
      int count = BFSCount(child, visited);
      if (count > 0) {
        answer += count;
      }
    }
    visited.Remove(current);

    return answer;
  }
  public void Solve1() {
    // Run a bfs from YOU to OUT
    int answer = BFSCount(YOU, []);
    Console.WriteLine(answer);
  }


  public long BFSCount2(
    string current,
    string target,
    HashSet<string> targetDownstream,
    HashSet<string> visited,
    Dictionary<string, long> memo) {
    if (targetDownstream.Contains(current)) {
      if (current == target) {
        return 1;
      } else {
        return 0;
      }
    }
    if (memo.ContainsKey(current)) {
      return memo[current];
    }

    long answer = 0;
    visited.Add(current);
    foreach (var child in Nodes[current]) {
      if (visited.Contains(child)) { continue; }
      long count = BFSCount2(child, target, targetDownstream, visited, memo);
      if (count > 0) {
        answer += count;
      }
    }
    visited.Remove(current);

    memo[current] = answer;
    return answer;
  }

  void MarkDownstreamNodes(string current, HashSet<string> downstreamSet, HashSet<string> visited) {
    visited.Add(current);
    foreach (var child in Nodes[current]) {
      if (visited.Contains(child)) { continue; }
      downstreamSet.Add(child);
      MarkDownstreamNodes(child, downstreamSet, visited);
    }
  }

  public void Solve2() {
    HashSet<string> downstreamOfFft = new([FFT]);
    HashSet<string> downstreamOfDac = new([DAC]);
    MarkDownstreamNodes(FFT, downstreamOfFft, []);
    MarkDownstreamNodes(DAC, downstreamOfDac, []);

    long svrToFft = BFSCount2(SVR, FFT, downstreamOfFft, [],[]);
    long fftToDac = BFSCount2(FFT, DAC, downstreamOfDac, [], []);
    long dacToOut = BFSCount2(DAC, OUT, [OUT], [], []);

    long answer = svrToFft * fftToDac * dacToOut;
    Console.WriteLine(answer);
  }

  public void Run() {
    _ReadInput();
    Solve2();
  }
}