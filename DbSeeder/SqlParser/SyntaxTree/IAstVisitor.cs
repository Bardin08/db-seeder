namespace DbSeeder.SqlParser.SyntaxTree;

public interface IAstVisitor
{
    public void Visit(SyntaxTreeNode? node);
}

