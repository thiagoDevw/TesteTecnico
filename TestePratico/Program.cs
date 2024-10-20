using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    class Candidate
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public int Age { get; set; }
        public string State { get; set; }
    }

    static void Main(string[] args)
    {
        string filePath = "Academy_Candidates.txt";

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Arquivo não encontrado.");
            return;
        }

        var candidates = LoadCandidates(filePath);

        var percentageByJob = CalculatePercentageByJob(candidates);
        var averageAgeByJob = CalculateAverageAgeByJob(candidates);
        var (oldestByJob, youngestByJob) = FindOldestAndYoungestByJob(candidates);
        var totalAgeByJob = CalculateTotalAgeByJob(candidates);
        var distinctStatesCount = CountDistinctStates(candidates);
        var qaInstructor = FindQAInstructor(candidates);
        var mobileInstructor = FindMobileInstructor(candidates);

        Console.WriteLine("Proporção de candidatos por vaga (porcentagem):");
        foreach (var job in percentageByJob)
        {
            Console.WriteLine($"{job.Key}: {job.Value.ToString("F2")}%");
        }

        Console.WriteLine($"\nIdade média dos candidatos de QA: {averageAgeByJob["QA"].ToString("F2")} anos");
        Console.WriteLine($"\nIdade do candidato mais velho de Mobile: {oldestByJob["Mobile"]} anos");
        Console.WriteLine($"\nIdade do candidato mais novo de Web: {youngestByJob["Web"]} anos");
        Console.WriteLine($"\nSoma das idades dos candidatos de QA: {totalAgeByJob["QA"].ToString("N0")} anos");
        Console.WriteLine($"\nNúmero de estados distintos: {distinctStatesCount}");
        Console.WriteLine();
        CreateSortedCsv(candidates, "Sorted_Academy_Candidates.csv");
        Console.WriteLine($"\nNome do instrutor de QA: {qaInstructor}");
        Console.WriteLine($"\nNome do instrutor de Mobile: {mobileInstructor}");
    }
    static List<Candidate> LoadCandidates(string filePath)
    {
        var candidates = new List<Candidate>();
        var lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var data = line.Split(';');

            string ageString = data[1].Replace(" anos", "").Trim();

            if (int.TryParse(ageString, out int age))
            {
                candidates.Add(new Candidate
                {
                    Name = data[0],
                    Job = data[2],
                    Age = age,
                    State = data[3]
                });
            }
        }
        return candidates;
    }

    static Dictionary<string, double> CalculatePercentageByJob(List<Candidate> candidates)
    {
        int total = candidates.Count;
        return candidates
            .GroupBy(c => c.Job)
            .ToDictionary(g => g.Key, g => (g.Count() / (double)total) * 100);
    }

    static Dictionary<string, double> CalculateAverageAgeByJob(List<Candidate> candidates)
    {
        return candidates
            .GroupBy(c => c.Job)
            .ToDictionary(g => g.Key, g => g.Average(c => c.Age));
    }

    static (Dictionary<string, int> oldest, Dictionary<string, int> youngest) FindOldestAndYoungestByJob(List<Candidate> candidates)
    {
        var oldest = candidates.GroupBy(c => c.Job).ToDictionary(g => g.Key, g => g.Max(c => c.Age));
        var youngest = candidates.GroupBy(c => c.Job).ToDictionary(g => g.Key, g => g.Min(c => c.Age));
        return (oldest, youngest);
    }

    static Dictionary<string, int> CalculateTotalAgeByJob(List<Candidate> candidates)
    {
        return candidates.GroupBy(c => c.Job).ToDictionary(g => g.Key, g => g.Sum(c => c.Age));
    }

    static int CountDistinctStates(List<Candidate> candidates)
    {
        return candidates.Select(c => c.State).Distinct().Count();
    }

    static void CreateSortedCsv(List<Candidate> candidates, string filePath)
    {
        var sortedCandidates = candidates.OrderBy(c => c.Name).ToList();
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Nome,Vaga,Idade,Estado");
            foreach (var candidate in sortedCandidates)
            {
                writer.WriteLine($"{candidate.Name},{candidate.Job},{candidate.Age},{candidate.State}");
            }
        }
        Console.WriteLine($"Arquivo {filePath} criado com sucesso.");
    }

    static string FindQAInstructor(List<Candidate> candidates)
    {
        var qaCandidates = candidates.Where(c =>
            c.Job == "QA" &&
            c.State == "SC"
        ).ToList();

        if (qaCandidates.Count == 0)
            return "Instrutor de QA não encontrado.";
        
        return qaCandidates.FirstOrDefault(c =>
            CheckPerfectSquare(c.Age) &&
            CheckPalindrome(c.Name.Split(' ')[0])
        )?.Name ?? "Instrutor de QA não encontrado.";
    }
    static bool CheckPerfectSquare(int n)
    {
        int sqrt = (int)Math.Sqrt(n);
        return sqrt * sqrt == n;
    }
    static bool CheckPalindrome(string s)
    {
        var cleanedString = new string(s.Where(char.IsLetterOrDigit).ToArray()).ToLower();

        return cleanedString.SequenceEqual(cleanedString.Reverse());
    }

    static string FindMobileInstructor(List<Candidate> candidates)
    {
        return candidates.FirstOrDefault(c =>
            c.Job == "Mobile" &&
            c.State == "PI" &&
            c.Age >= 30 && c.Age <= 40 &&
            c.Age % 2 == 0 &&
            c.Name.Split(' ').Last().StartsWith("C")
        )?.Name ?? "Instrutor de Mobile não encontrado.";
    }
}
