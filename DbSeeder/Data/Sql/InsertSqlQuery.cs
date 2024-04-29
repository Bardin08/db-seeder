using System.Text;

namespace DbSeeder.Data.Sql;

public record InsertSqlQuery(
    string Table,
    IReadOnlyDictionary<string, string> Value,
    IReadOnlyList<InsertSqlQuery>? RelatedQueries = null)
{
    public override string ToString()
    {
        var sb = new StringBuilder();

        if (RelatedQueries is not null && RelatedQueries.Any())
        {
            foreach (var query in RelatedQueries)
            {
                sb.AppendLine(query.ToString());
            }
        }

        sb.AppendLine(BuildSqlQuery());
        return sb.ToString();

        string BuildSqlQuery()
        {
            return $"insert into {Table}({GetInsertCols()}) " +
                   $"value ({GetInsertValues()});";
        }
    }

    private string GetInsertCols()
    {
        return string.Join(",", Value.Keys);
    }

    private string GetInsertValues()
    {
        return string.Join(",", Value.Values);
    }
}
