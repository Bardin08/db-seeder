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
        IsNotNull = constraints.Contains("not null", StringComparer.OrdinalIgnoreCase);
        IsUnique = constraints.Contains("unique", StringComparer.OrdinalIgnoreCase);
        IsPrimaryKey = constraints.Contains("primary key", StringComparer.OrdinalIgnoreCase);

        var fkConstraint = constraints.FirstOrDefault(
            c => c.StartsWith("foreign key", StringComparison.OrdinalIgnoreCase));
        IsForeignKey = fkConstraint is not null;

        if (IsForeignKey)
        {
            var refTableAndCol = fkConstraint![12..].Split("|");

            var refTableName = refTableAndCol[0];
            var refColumnName = refTableAndCol[1];

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
