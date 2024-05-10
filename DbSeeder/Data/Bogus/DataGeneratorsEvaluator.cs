using DbSeeder.Schema;

namespace DbSeeder.Data.Bogus;

public static class DataGeneratorsEvaluator
{
    public static List<BogusGenerator> FindBestNGenerators(
        this Dictionary<string, List<BogusGenerator>> allGenerators,
        Column column,
        int n = 1)
    {
        var weights = new Dictionary<int, List<BogusGenerator>>();

        foreach (var (_, generatorsCategory) in allGenerators)
        {
            foreach (var generator in generatorsCategory)
            {
                if (generator.Params.Count != 0)
                {
                    // TODO[#26]: Implement generators with params
                    continue;
                }

                var weight = CalculateLevenshteinDistance(column.Name, generator.GeneratorIdentifier);
                if (!weights.ContainsKey(weight))
                {
                    weights[weight] = [];
                }

                weights[weight].Add(generator);
            }
        }

        return weights.OrderBy(x => x.Key)
            .SelectMany(x => x.Value)
            .Take(n)
            .ToList();
    }

    private static int CalculateLevenshteinDistance(string word1, string word2)
    {
        var n = word1.Length;
        var m = word2.Length;
        var distance = new int[n + 1, m + 1];

        for (var i = 0; i <= n; i++) distance[i, 0] = i;
        for (var j = 0; j <= m; j++) distance[0, j] = j;

        for (var i = 1; i <= n; i++)
        {
            for (var j = 1; j <= m; j++)
            {
                var cost = (word2[j - 1] == word1[i - 1]) ? 0 : 1;

                distance[i, j] = Math.Min(
                    Math.Min(
                        distance[i - 1, j] + 1, // Deletion
                        distance[i, j - 1] + 1), // Insertion
                    distance[i - 1, j - 1] + cost); // Substitution
            }
        }

        return distance[n, m];
    }
}
