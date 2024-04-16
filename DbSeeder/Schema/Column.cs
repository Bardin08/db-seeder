namespace DbSeeder.Schema;

public class Column(string name, string dataType, string dataTypeConstraint, string[] constraints)
{
    public string Name { get; } = name;
    public string DataType { get; } = dataType;
    public string DataTypeConstraint { get; set; } = dataTypeConstraint;
    public string[] Constraints { get; set; } = constraints;
}
