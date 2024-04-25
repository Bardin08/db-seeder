using DbSeeder.SqlParser.SyntaxTree;

namespace DbSeeder.Schema;

public class SqlSchemaBuilder : IAstVisitor
{
    private SqlSchema? _sqlSchema;

    public SqlSchema Build(SyntaxTreeNode rootNode)
    {
        _sqlSchema = new SqlSchema();
        Visit(rootNode);
        return _sqlSchema;
    }

    public void Visit(SyntaxTreeNode? rootNode)
    {
        ArgumentNullException.ThrowIfNull(rootNode);

        if (rootNode.Type != SyntaxTreeNodeType.Root)
        {
            throw new ArgumentException("Unexpected node type: passe node must be a root node");
        }

        TraverseTree(rootNode);
    }

    private void TraverseTree(SyntaxTreeNode rootNode)
    {
        foreach (var child in rootNode.Children)
        {
            if (child?.Type is SyntaxTreeNodeType.CreateStatement)
            {
                TraverseCreateStatement(child);
            }
            else
            {
                TraverseTree(child!);
            }
        }
    }

    private void TraverseCreateStatement(SyntaxTreeNode createStatementNode)
    {
        var child = createStatementNode.Children.First();
        if (child!.Type is SyntaxTreeNodeType.CreateTable)
        {
            var tableRoot = child.Children.First();
            var table = CreateTable(tableRoot!);
            _sqlSchema!.AddTable(table);
        }
    }

    private Table CreateTable(SyntaxTreeNode tableRoot)
    {
        var tableName = tableRoot.Value;
        var table = new Table(tableName);

        // Actually, now we expect that table root contains only cols, but in the future more sub-nodes can be added,
        // so we have to be sure that we're working exactly with a node that contains cols.
        var colsRoot = tableRoot.Children.First(x => x?.Type is SyntaxTreeNodeType.TableColumns);
        var cols = ParseColumns(colsRoot!);
        table.Columns.AddRange(cols);

        return table;
    }

    private IEnumerable<Column> ParseColumns(SyntaxTreeNode colsRoot)
    {
        return colsRoot.Children.Select(GetColumn!).ToList();
    }

    private Column GetColumn(SyntaxTreeNode columnNode)
    {
        var name = columnNode.Value;
        var dataType = string.Empty;
        var dataTypeConstraint = string.Empty;
        var constraints = new List<string>();

        foreach (var node in columnNode.Children)
        {
            ArgumentNullException.ThrowIfNull(node);

            switch (node.Type)
            {
                case SyntaxTreeNodeType.ColumnDataType:
                    dataType = node.Value;
                    foreach (var typeConstraint in node.Children)
                    {
                        dataTypeConstraint = typeConstraint!.Value;
                    }
                    break;
                case SyntaxTreeNodeType.DataTypeConstraint:
                    dataTypeConstraint = node.Value;
                    break;
                case SyntaxTreeNodeType.ColumnConstraint:
                    constraints.Add(node.Value);
                    break;
                case SyntaxTreeNodeType.ForeignKeyDefinition:
                    // TODO: Implement processing FK constraint correctly
                    break;
            }
        }

        return new Column(name, dataType, dataTypeConstraint, constraints.ToArray());
    }
}
