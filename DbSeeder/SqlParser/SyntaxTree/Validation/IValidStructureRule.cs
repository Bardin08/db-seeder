namespace DbSeeder.SqlParser.SyntaxTree.Validation;

public interface IValidStructureRule
{
    public SyntaxTreeNode NodeToValidate { get; set; }
    public HashSet<SyntaxTreeNodeType> AllowedChildTypes { get; set; }
}
