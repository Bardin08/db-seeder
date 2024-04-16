using DbSeeder.SqlParser.SyntaxTree.Validation.Rules;

namespace DbSeeder.SqlParser.SyntaxTree.Validation.Internal;

public class TreeStructureValidator : INodeValidator
{
    private static readonly Dictionary<SyntaxTreeNodeType, HashSet<SyntaxTreeNodeType>> ValidChildren = new()
    {
        { SyntaxTreeNodeType.Root, [SyntaxTreeNodeType.CreateStatement] },
        { SyntaxTreeNodeType.CreateStatement, [SyntaxTreeNodeType.CreateTable] },
        { SyntaxTreeNodeType.CreateTable, [SyntaxTreeNodeType.TableRoot] },
        { SyntaxTreeNodeType.TableRoot, [SyntaxTreeNodeType.TableColumns] },
        { SyntaxTreeNodeType.TableColumns, [SyntaxTreeNodeType.Column] },
        { SyntaxTreeNodeType.Column, [SyntaxTreeNodeType.ColumnDataType, SyntaxTreeNodeType.ColumnConstraint] },
        { SyntaxTreeNodeType.ColumnDataType, [SyntaxTreeNodeType.DataTypeConstraint] },
        { SyntaxTreeNodeType.DataTypeConstraint, [] },
        { SyntaxTreeNodeType.ColumnConstraint, [] },
    };

    public void Validate(ValidationContext validationContext, SyntaxTreeNode node)
    {
        var validator = new ValidStructureRule(node, ValidChildren[node.Type]);
        validator.Apply(validationContext);
    }
}
