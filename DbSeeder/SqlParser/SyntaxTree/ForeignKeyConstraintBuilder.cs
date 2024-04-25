namespace DbSeeder.SqlParser.SyntaxTree;

public class ForeignKeyConstraintBuilder
{
    private SyntaxTreeNode? _constraintLocalRoot;
    private SyntaxTreeNode? _lastAddedNode;

    private bool _referenceKeywordMet;
    public bool IsComplete { get; private set; }

    // FOREIGN KEY subtree structure:
    // FOREIGN KEY -- ForeignKeyDefinition
    //      profile_id -- KeyColumnIdentifier
    //      profiles -- KeyReferencedTable
    //      id -- KeyReferencedColumn
    public void Handle(SqlToken? token, ref SyntaxTreeNode treeLocalRoot)
    {
        ArgumentNullException.ThrowIfNull(token);

        if (_constraintLocalRoot is null)
        {
            // This is a first node and here we have to create a ForeignKeyDefinition node
            CreateConstraintRoot(ref treeLocalRoot);
        }
        else if (token.Type is SqlTokenType.Punctuation)
        {
            HandlePunctuationToken(token, ref treeLocalRoot);
        }

        else if (token.Type is SqlTokenType.Identifier)
        {
            HandleIdentifierToken(token);
        }

        else if (token.Type is SqlTokenType.Keyword
                 && token.Value.Equals("REFERENCES", StringComparison.OrdinalIgnoreCase))
        {
            _referenceKeywordMet = true;
        }
    }

    private void HandleIdentifierToken(SqlToken token)
    {
        ArgumentNullException.ThrowIfNull(_lastAddedNode);
        ArgumentNullException.ThrowIfNull(_constraintLocalRoot);

        if (token.Value.Equals("REFERENCES", StringComparison.OrdinalIgnoreCase))
        {
            _referenceKeywordMet = true;
            return;
        }

        if (_lastAddedNode.Type is SyntaxTreeNodeType.ForeignKeyDefinition)
        {
            // This is a pointer to the constraint owner column
            var node = new SyntaxTreeNode(
                SyntaxTreeNodeType.KeyColumnIdentifier,
                token.Value,
                _constraintLocalRoot);

            _constraintLocalRoot.Children.Add(node);
            _lastAddedNode = node;
        }
        else if (_lastAddedNode.Type is SyntaxTreeNodeType.KeyColumnIdentifier &&
                 _referenceKeywordMet)
        {
            var node = new SyntaxTreeNode(
                SyntaxTreeNodeType.KeyReferencedTable,
                token.Value,
                _constraintLocalRoot);

            _constraintLocalRoot.Children.Add(node);
            _lastAddedNode = node;
        }
        else if (_lastAddedNode.Type is SyntaxTreeNodeType.KeyReferencedTable)
        {
            var node = new SyntaxTreeNode(
                SyntaxTreeNodeType.KeyReferencedColumn,
                token.Value,
                _constraintLocalRoot);

            _constraintLocalRoot.Children.Add(node);
            _lastAddedNode = node;
        }
    }

    private void HandlePunctuationToken(SqlToken token, ref SyntaxTreeNode treeLocalRoot)
    {
        ArgumentNullException.ThrowIfNull(_lastAddedNode);

        if (_lastAddedNode.Type == SyntaxTreeNodeType.KeyReferencedColumn &&
            token.Value is ")")
        {
            // FOREIGN KEY statement is complete
            IsComplete = true;

            // move a pointer back to the cols
            // then this one should be moved to the appropriate column,
            // but this should be managed by the ast builder
            treeLocalRoot = _constraintLocalRoot!.Parent!;
        }
    }

    private void CreateConstraintRoot(ref SyntaxTreeNode treeLocalRoot)
    {
        const string nodeValue = "FOREIGN KEY";
        _constraintLocalRoot = new SyntaxTreeNode(
            SyntaxTreeNodeType.ForeignKeyDefinition, nodeValue, treeLocalRoot.Parent);

        var tempNode = treeLocalRoot;
        treeLocalRoot = _constraintLocalRoot;

        var parent = treeLocalRoot.Parent!;
        parent.Children.Remove(tempNode);
        parent.Children.Add(_constraintLocalRoot);

        _lastAddedNode = _constraintLocalRoot;
    }
}
