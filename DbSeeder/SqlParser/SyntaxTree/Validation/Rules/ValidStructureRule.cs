using System.Collections.Frozen;

namespace DbSeeder.SqlParser.SyntaxTree.Validation.Rules;

public class ValidStructureRule(
    SyntaxTreeNode nodeToValidate,
    HashSet<SyntaxTreeNodeType> allowedChildTypes)
    : IValidStructureRule
{
    public void Apply(ValidationContext validationContext)
    {
        // As we use HashSet, unique errors will be stored, so we can't point to the node
        var violations = new HashSet<string>();
        foreach (var child in nodeToValidate.Children)
        {
            if (child is null)
            {
                violations.Add(AstValidationConstants.Generic.ChildNullError(nodeToValidate.Type));
                continue;
            }

            if (!allowedChildTypes.Contains(child.Type))
            {
                violations.Add(AstValidationConstants.Generic.InvalidChild(
                    nodeToValidate.Type, allowedChildTypes));
            }
        }

        var isValid = violations.Count is 0;
        var result = new ValidationResult(nameof(ValidStructureRule), isValid, violations.ToFrozenSet());
        validationContext.AddValidationResult(result);
    }
}
