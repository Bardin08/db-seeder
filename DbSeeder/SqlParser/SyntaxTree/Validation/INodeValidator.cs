namespace DbSeeder.SqlParser.SyntaxTree.Validation;

public interface INodeValidator
{
    void Validate(ValidationContext validationContext, SyntaxTreeNode node);
}
