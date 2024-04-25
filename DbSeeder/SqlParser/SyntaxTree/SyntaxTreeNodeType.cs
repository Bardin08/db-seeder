namespace DbSeeder.SqlParser.SyntaxTree;

public enum SyntaxTreeNodeType
{
    Root,

    CreateStatement,
    CreateTable,

    TableRoot,
    TableColumns,

    Column,
    ColumnDataType,
    ColumnConstraint,

    DataTypeConstraint,

    KeyDefinition,
    KeyReferencedTable,
    KeyColumnIdentifier,
    KeyReferencedColumn,

    PrimaryKeyDefinition,
    ForeignKeyDefinition
}
