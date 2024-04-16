namespace DbSeeder.Schema;

public record Column
{
    public Column(string name,
        string dataType,
        string dataTypeConstraint,
        string[] constraints)
    {
        Name = name;
        DataType = dataType;
        DataTypeConstraint = dataTypeConstraint;

        constraints = constraints.Select(c => c.ToLower()).ToArray();
        IsAutoIncrement = constraints.Contains("auto_increment", StringComparer.OrdinalIgnoreCase);
        IsPrimaryKey = constraints.Contains("primary key", StringComparer.OrdinalIgnoreCase);
        IsForeignKey = constraints.Contains("foreign key", StringComparer.OrdinalIgnoreCase);
        IsNotNull = constraints.Contains("not null", StringComparer.OrdinalIgnoreCase);
        IsUnique = constraints.Contains("unique", StringComparer.OrdinalIgnoreCase);

        if (IsForeignKey)
        {
            // TODO[#15]: Update when working on FKs support.
            // This will cause an exception during table's construction, as 'ref_TableName' won't be in a schema
            const string refTableName = "ref_TableName";
            const string refColumnName = "ref_ColumnName";
            ForeignKeyRef = new ForeignKeyRef(refTableName, refColumnName);
        }
    }

    public string Name { get; }
    public string DataType { get; }
    public string DataTypeConstraint { get; }

    public bool IsAutoIncrement { get; }
    public bool IsPrimaryKey { get; }
    public bool IsForeignKey { get; }
    public bool IsNotNull { get; }
    public bool IsUnique { get; }

    public ForeignKeyRef? ForeignKeyRef { get; }
}
