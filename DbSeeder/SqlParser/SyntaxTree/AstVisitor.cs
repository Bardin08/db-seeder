namespace DbSeeder.SqlParser.SyntaxTree;

public class AstVisitor
{
    public virtual void Visit(SyntaxTreeNode? node)
    {
    }
}

public class SqlSchemaBuilder : AstVisitor
{
    private readonly SQLSchema _schema = new();

    public SQLSchema BuildSchema(SyntaxTreeNode? astRoot)
    {
        Visit(astRoot);
        return _schema;
    }

    public override void Visit(SyntaxTreeNode? node)
    {
        switch (node.Value)
        {
            case "SQLScript":
            case "CreateStatement":
                Visit(node.Children.First());
                break;

            case "CreateTable":
                ProcessCreateTable(node);
                break;

            default:
                // Ігноруємо інші типи вузлів
                break;
        }
    }

    private void ProcessCreateTable(SyntaxTreeNode? node)
    {
        var tableName = node.Children[0].Value;
        var table = new Table(tableName);

        foreach (var child in node.Children[1].Children)
        {
            var columnName = child.Children[0].Value;
            var columnType = child.Children[1].Value;
            table.AddColumn(new Column(columnName, columnType));
        }

        _schema.AddTable(table);
    }
}
