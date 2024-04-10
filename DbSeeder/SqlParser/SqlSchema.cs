namespace DbSeeder.SqlParser;

public class SQLSchema
{
    public List<Table> Tables { get; }

    public SQLSchema()
    {
        Tables = new List<Table>();
    }

    public void AddTable(Table table)
    {
        Tables.Add(table);
    }
}

public class Table
{
    public string Name { get; }
    public List<Column> Columns { get; }

    public Table(string name)
    {
        Name = name;
        Columns = new List<Column>();
    }

    public void AddColumn(Column column)
    {
        Columns.Add(column);
    }
}

public class Column
{
    public string Name { get; }
    public string DataType { get; }

    public Column(string name, string dataType)
    {
        Name = name;
        DataType = dataType;
    }
}
