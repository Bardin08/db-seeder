namespace DbSeeder.SqlParser.SyntaxTree.Validation.Validators;

public interface IValidator
{
    public ValidationResult Validate(SyntaxTreeNode tree);
}
