namespace DbSeeder.Schema;

public record ForeignKeyRef(
    string TableName,
    string ColumnName);