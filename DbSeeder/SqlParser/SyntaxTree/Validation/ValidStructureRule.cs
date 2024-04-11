using System.Collections.Frozen;

namespace DbSeeder.SqlParser.SyntaxTree.Validation;

public class ValidStructureRule(
    SyntaxTreeNode nodeToValidate,
    HashSet<SyntaxTreeNodeType> allowedChildTypes)
    : IValidStructureRule
{
    public SyntaxTreeNode NodeToValidate { get; set; } = nodeToValidate;
    public HashSet<SyntaxTreeNodeType> AllowedChildTypes { get; set; } = allowedChildTypes;

    public void Apply(ValidationContext validationContext)
    {
        // As we use HashSet, unique errors will be stored, so we can't point to the node
        var violations = new HashSet<string>();
        foreach (var child in NodeToValidate.Children)
        {
            if (child is null)
            {
                violations.Add(AstValidationConstants.Generic.ChildNullError(SyntaxTreeNodeType.TableColumns));
                continue;
            }

            if (!AllowedChildTypes.Contains(child.Type))
            {
                violations.Add(AstValidationConstants.Generic.InvalidChild(
                    SyntaxTreeNodeType.TableRoot, AllowedChildTypes));
            }
        }

        var isValid = violations.Count is 0;
        var result = new ValidationResult(nameof(ValidStructureRule), isValid, violations.ToFrozenSet());
        validationContext.AddValidationResult(result);
    }
}
