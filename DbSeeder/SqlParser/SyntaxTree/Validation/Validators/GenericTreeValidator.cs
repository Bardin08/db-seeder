using DbSeeder.SqlParser.SyntaxTree.Validation.Internal;

namespace DbSeeder.SqlParser.SyntaxTree.Validation.Validators;

public class GenericTreeValidator : IValidator
{
    private readonly List<INodeValidator> _validators =
    [
        new TreeStructureValidator()
    ];

    public ValidationResult Validate(SyntaxTreeNode tree)
    {
        var validationContext = new ValidationContext();

        TraverseSyntaxTree(tree, validationContext);

        return validationContext.FlattenValidationResult();
    }

    private void TraverseSyntaxTree(SyntaxTreeNode node, ValidationContext validationContext)
    {
        foreach (var validator in _validators)
        {
            validator.Validate(validationContext, node);
        }

        foreach (var child in node.Children)
        {
            if (child is null)
            {
                return;
            }

            TraverseSyntaxTree(child, validationContext);
        }
    }
}
