using System.Collections.Frozen;

namespace DbSeeder.SqlParser.SyntaxTree.Validation.Rules;

public class SinglePrimaryKeyRule(
    SyntaxTreeNode nodeToValidate,
    string tableName) : IValidationRule
{
    private int _primaryKeysFound;
    private readonly HashSet<string> _violations = [];

    public void Apply(ValidationContext validationContext)
    {
        ArgumentNullException.ThrowIfNull(nodeToValidate);
        if (nodeToValidate.Type != SyntaxTreeNodeType.TableRoot)
        {
            return;
        }

        // As we use HashSet, unique errors will be stored, so we can't point to the node
        TraverseTableTree(nodeToValidate);

        if (_primaryKeysFound > 1)
        {
            _violations.Add($"{tableName} contains more than one PRIMARY KEY (x{_primaryKeysFound})");
        }

        var isValid = _violations.Count is 0;
        var result = new ValidationResult(nameof(SinglePrimaryKeyRule), isValid, _violations.ToFrozenSet());
        validationContext.AddValidationResult(result);
    }

    private void TraverseTableTree(SyntaxTreeNode node)
    {
        if (node.Type is SyntaxTreeNodeType.ColumnConstraint &&
            node.Value is "PRIMARY KEY")
        {
            _primaryKeysFound++;
        }

        foreach (var child in node.Children)
        {
            if (child is null)
            {
                _violations.Add(AstValidationConstants.Generic.ChildNullError(node.Type));
                continue;
            }

            TraverseTableTree(child);
        }
    }
}
