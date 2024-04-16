using DbSeeder.SqlParser.SyntaxTree.Validation.Rules;

namespace DbSeeder.SqlParser.SyntaxTree.Validation.Internal;

public class ColumnSubTreeValidator : INodeValidator
{
    public void Validate(ValidationContext validationContext, SyntaxTreeNode node)
    {
        if (node.Type != SyntaxTreeNodeType.Column)
        {
            return;
        }

        var validator = new ValidColumnSubTreeRule(node);
        validator.Apply(validationContext);
    }
}
