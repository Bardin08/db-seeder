using DbSeeder.Schema;

namespace DbSeeder.Data;

internal static class GeneratorFactory
{
    private static readonly string[] StringTypes = ["text", "varchar", "nvarchar", "char"];
    private static readonly string[] NumeralTypes = ["int", "long", "bit"];

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