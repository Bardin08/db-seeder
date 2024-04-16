namespace DbSeeder.Schema;

public class SqlSchema
{
    private readonly List<Table> _tables = [];
    public IReadOnlyList<Table> Tables => _tables;

    public void AddTable(Table table)
    {
        _tables.Add(table);
    }
}
