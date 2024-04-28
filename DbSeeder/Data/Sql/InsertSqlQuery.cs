namespace DbSeeder.Data.Sql;

public record InsertSqlQuery(
    string Table,
    IReadOnlyDictionary<string, string> Value) // col: value
{
    public override string ToString()
    {
        return $"insert into {Table}({GetInsertCols()}) " +
               $"value ({GetInsertValues()});";
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