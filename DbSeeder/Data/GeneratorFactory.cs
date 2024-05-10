using System.Data;
using DbSeeder.Data.Bogus;
using DbSeeder.Schema;

namespace DbSeeder.Data;

internal static class GeneratorFactory
{
    private static readonly HashSet<string> StringTypes = ["text", "varchar", "nvarchar", "char", "nchar", "ntext"];

    private static readonly HashSet<string> NumeralTypes =
        ["bigint", "int", "smallint", "tinyint", "bit", "decimal", "money", "smallmoney", "float", "real"];

    private static readonly HashSet<string> BinaryTypes = ["binary", "varbinary", "image"];

    private static readonly HashSet<string> DateTimeTypes =
        ["datetime", "smalldatetime", "date", "time", "datetime2", "datetimeoffset"];

    private static readonly HashSet<string> OtherTypes =
            ["uniqueidentifier", "timestamp", "xml", "udt", "structured", "variant"];

    private static readonly Dictionary<string, List<BogusGenerator>> Generators = BogusUtilities.GetBogusGenerators();

    public static object? GetGeneratorByColumnV2(Column col)
    {
        // type filter
        var generators = Generators;
        if (StringTypes.Contains(col.DataType, StringComparer.OrdinalIgnoreCase))
        {
            generators = generators.GetFiltersForReturnType("string");
        }
        else if (NumeralTypes.Contains(col.DataType, StringComparer.OrdinalIgnoreCase))
        {
            generators = generators.GetFiltersForReturnType("number");
        }

        // semantic filter
        var generator = generators.FindBestNGenerators(col, n: 1).First();
        var generatedValue = (object?)BogusUtilities.Generate(generator);
        return generatedValue;
    }

    public static (Type, Func<object>) GetGeneratorByColumn(Column col)
    {
        if (StringTypes.Contains(col.DataType, StringComparer.OrdinalIgnoreCase))
        {
            return (typeof(string), () => Guid.NewGuid().ToString("N"));
        }

        if (NumeralTypes.Contains(col.DataType, StringComparer.OrdinalIgnoreCase)) // add type limits check
        {
            return (typeof(long), () => Random.Shared.NextInt64() * 17);
        }

        throw new NotImplementedException($"{col.DataType} is not currently supported");
    }
}
