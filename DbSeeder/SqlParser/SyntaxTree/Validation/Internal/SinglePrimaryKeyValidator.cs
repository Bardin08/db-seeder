using DbSeeder.SqlParser.SyntaxTree.Validation.Rules;

namespace DbSeeder.SqlParser.SyntaxTree.Validation.Internal;

public class SinglePrimaryKeyValidator : INodeValidator
{
    public void Validate(ValidationContext validationContext, SyntaxTreeNode node)
    {
        if (node.Type != SyntaxTreeNodeType.TableRoot)
        {
            return;
        }

        var tableName = node.Value;
        var validator = new SinglePrimaryKeyRule(node, tableName);
        validator.Apply(validationContext);
    }
}
