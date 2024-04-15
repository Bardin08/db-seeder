namespace DbSeeder.Schema;

public class Table(string name)
{
    public string Name { get; } = name;
    public List<Column> Columns { get; } = [];
}
