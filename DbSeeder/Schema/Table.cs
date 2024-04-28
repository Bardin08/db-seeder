namespace DbSeeder.Schema;

public class Table(string name, SqlSchema schema)
{
    private readonly List<Column> _columns = [];
    private readonly List<ForeignKey> _foreignKeys = [];

    // TODO[#21]: Implement key define logic if not specified manually
    // There are some rules how to deal if PK is not defined
    // ex: find the first attribute with NOT NULL & UNIQUE constraints
    public PrimaryKey? PrimaryKey { get; private set; }
    public string Name { get; } = name;
    public IReadOnlyList<Column> Columns => _columns;
    public IReadOnlyList<ForeignKey> ForeignKeys => _foreignKeys;

    public void AddColumns(IEnumerable<Column> columns)
    {
        foreach (var column in columns)
        {
            AddColumn(column);
        }
    }

    private void AddColumn(Column column)
    {
        ArgumentNullException.ThrowIfNull(column);
        _columns.Add(column);

        if (column.IsPrimaryKey)
        {
            SetTablePrimaryKey(column);
        }
        else if (column.IsForeignKey)
        {
            SetTableForeignKey(column);
        }
    }

    private void SetTablePrimaryKey(Column column)
    {
        if (PrimaryKey != null)
        {
            throw new ArgumentException("Table can not contains more that one PRIMARY KEY");
        }

        PrimaryKey = new PrimaryKey(this, column);
    }

    private void SetTableForeignKey(Column column)
    {
        ArgumentNullException.ThrowIfNull(column.ForeignKeyRef);

        var refTable = schema.GetTableByName(column.ForeignKeyRef.TableName);
        if (refTable is null)
        {
            throw new InvalidOperationException($"Referenced table {column.ForeignKeyRef.TableName} is not exists " +
                                                $"in current schema. Validate the order of the create statements, " +
                                                $"it's matter");
        }

        var refColumn = refTable.GetColumnByName(column.ForeignKeyRef.ColumnName);
        if (refColumn is null)
        {
            throw new InvalidOperationException($"Referenced table {column.ForeignKeyRef.TableName} is not exists " +
                                                $"in current schema. Validate the order of the create statements, " +
                                                $"it's matter");
        }

        var fk = new ForeignKey(this, column, refTable, refColumn);
        _foreignKeys.Add(fk);
    }

    private Column? GetColumnByName(string columnName)
        => Columns.FirstOrDefault(c => c.Name.Equals(columnName, StringComparison.Ordinal));
}
