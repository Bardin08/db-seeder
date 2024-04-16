using DbSeeder.SqlParser.SyntaxTree.Validation.Internal;

namespace DbSeeder.SqlParser.SyntaxTree.Validation.Validators;

public class GenericTreeValidator : IValidator
{
    private static readonly List<INodeValidator> GenericValidators =
    [
        new TreeStructureValidator()
    ];

    private static readonly Dictionary<SyntaxTreeNodeType, List<INodeValidator>> Validators =
        new()
        {
            {
                SyntaxTreeNodeType.TableRoot,
                [
                    new SinglePrimaryKeyValidator()
                ]
            },
            {
                SyntaxTreeNodeType.Column,
                [
                    new ColumnSubTreeValidator()
                ]
            },
        };

    public ValidationResult Validate(SyntaxTreeNode tree)
    {
        var validationContext = new ValidationContext();

        TraverseSyntaxTree(tree, validationContext);

        return validationContext.FlattenValidationResult();
    }

    private void TraverseSyntaxTree(SyntaxTreeNode node, ValidationContext validationContext)
    {
        var hasAdditionalValidators = Validators.TryGetValue(node.Type, out var additionalValidators);
        var allValidators = hasAdditionalValidators
            ? GenericValidators.Concat(additionalValidators!)
            : GenericValidators;

        foreach (var validator in allValidators)
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
