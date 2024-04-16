namespace DbSeeder.Schema;

public class SqlSchema
{
    private readonly List<Table> _tables = [];

    public IReadOnlyList<Table> Tables => _tables;

    public void AddTable(Table table)
        => _tables.Add(table);

    public Table? GetTableByName(string tableName)
        => _tables.FirstOrDefault(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
}
