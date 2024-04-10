namespace DbSeeder.SqlParser.SyntaxTree;

public class SyntaxTreeNode(SyntaxTreeNodeType type, string value, SyntaxTreeNode? parent)
{
    public string Value { get; } = value;
    public SyntaxTreeNodeType Type { get; } = type;

    public SyntaxTreeNode? Parent { get; } = parent;
    public List<SyntaxTreeNode?> Children { get; } = [];

    public void AddChild(SyntaxTreeNode? child)
    {
        Children.Add(child);
    }
}
