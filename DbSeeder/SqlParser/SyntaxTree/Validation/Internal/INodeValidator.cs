namespace DbSeeder.SqlParser.SyntaxTree.Validation.Internal;

public interface INodeValidator
{
    void Validate(ValidationContext validationContext, SyntaxTreeNode node);
}
