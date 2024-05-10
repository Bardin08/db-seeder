namespace DbSeeder.SqlParser.SyntaxTree;

public class AstBuilder(List<SqlToken> tokens)
{
    private ForeignKeyConstraintBuilder? _fkBuilder;


    private readonly SyntaxTreeNode _root = new(SyntaxTreeNodeType.Root, "SQL_Script", null!);

    private SyntaxTreeNode? _localRoot;

    public SyntaxTreeNode BuildSyntaxTree()
    {
        _localRoot = _root;
        _fkBuilder = new ForeignKeyConstraintBuilder();

        foreach (var token in tokens)
        {
            HandleToken(token, ref _localRoot);
        }

        return _root;
    }

    private void HandleToken(SqlToken token, ref SyntaxTreeNode? localRoot)
    {
        ArgumentNullException.ThrowIfNull(token);

        if (localRoot.Type is SyntaxTreeNodeType.KeyDefinition)
        {
            HandleKeyConstraint(token, ref localRoot);
            return;
        }
        else if (localRoot.Type is SyntaxTreeNodeType.ForeignKeyDefinition)
        {
            _fkBuilder!.Handle(token, ref localRoot);

            if (_fkBuilder.IsComplete)
            {
                // we have to move FK from the cols' definition to specific col which owns this FK
                var fkDefinition = _localRoot!.Children.Single(n => n!.Type is SyntaxTreeNodeType.ForeignKeyDefinition);
                var fkOwner = fkDefinition!.Children.First(); // According to the FK subtree struct owner is the first child of the FK root

                var fkOwnerColRoot = localRoot.Children.Single(c =>
                    c!.Value.Equals(fkOwner!.Value, StringComparison.OrdinalIgnoreCase));

                localRoot.Children.Remove(fkDefinition);
                fkOwnerColRoot!.Children.Add(fkDefinition);
                _fkBuilder.Reset();
            }
            return;
        }

        switch (token.Type)
        {
            case SqlTokenType.Keyword:
                HandleKeywordToken(token, ref localRoot);
                break;
            case SqlTokenType.Identifier:
                HandleIdentifier(token, ref localRoot!);
                break;
            case SqlTokenType.Number:
                HandleNumber(token, ref localRoot!);
                break;
            case SqlTokenType.Punctuation:
                HandlePunctuation(token, ref localRoot!);
                break;
            case SqlTokenType.StringLiteral:
            case SqlTokenType.Operator:
            case SqlTokenType.Comment:
            case SqlTokenType.Other:
                throw new NotImplementedException("<3>: Not supporter token type");
        }
    }

    #region Keywords

    private void HandleKeywordToken(SqlToken token, ref SyntaxTreeNode? localRoot)
    {
        switch (token.Value.ToLower())
        {
            case "create":
                AddNode(SyntaxTreeNodeType.CreateStatement, "Create_Statement", ref localRoot);
                break;
            case "table":
                HandleTableKeyword(ref localRoot);
                break;
            default:
                throw new NotImplementedException($"Unhandled keyword: {token.Value}");
        }
    }

    private void HandleTableKeyword(ref SyntaxTreeNode? localRoot)
    {
        ArgumentNullException.ThrowIfNull(localRoot);

        if (localRoot.Type == SyntaxTreeNodeType.CreateStatement)
        {
            AddNode(SyntaxTreeNodeType.CreateTable, "Create_Table", ref localRoot);
        }
        else
        {
            throw new NotImplementedException("<1>: Is it possible situation for valid SQL?");
        }
    }

    #endregion

    #region Identifier

    private void HandleIdentifier(SqlToken token, ref SyntaxTreeNode localRoot)
    {
        switch (localRoot.Type)
        {
            case SyntaxTreeNodeType.CreateTable:
                AddNode(SyntaxTreeNodeType.TableRoot, token.Value, ref localRoot!);
                break;
            case SyntaxTreeNodeType.TableColumns:
                if (!IsConstraint(token))
                {
                    AddNode(SyntaxTreeNodeType.Column, token.Value, ref localRoot!);
                }
                else
                {
                    // Here can be another constraints created with a CONSTRAINT keyword, but they're not supported yet
                    BuildKeyConstraint(token, ref localRoot);
                }

                break;
            case SyntaxTreeNodeType.Column:
                AddNode(SyntaxTreeNodeType.ColumnDataType, token.Value, ref localRoot!);
                break;
            case SyntaxTreeNodeType.ColumnDataType:
                AddColumnConstraintNode(token, ref localRoot);
                break;
            case SyntaxTreeNodeType.ColumnConstraint:
                HandleColumnConstraintIdentifier(token, ref localRoot);
                break;
            case SyntaxTreeNodeType.KeyDefinition:
                HandleKeyConstraint(token, ref localRoot);
                break;
            case SyntaxTreeNodeType.ForeignKeyDefinition:
                HandleForeignKeyToken(token);
                break;
            case SyntaxTreeNodeType.PrimaryKeyDefinition:
                HandlePrimaryKeyToken(token, ref localRoot);
                break;
            default:
                throw new NotImplementedException($"Unhandled localRoot type for identifier: {localRoot.Type}");
        }
    }

    private void HandlePrimaryKeyToken(SqlToken token, ref SyntaxTreeNode localRoot)
    {
        throw new NotImplementedException();
    }

    private void HandleForeignKeyToken(SqlToken token)
    {
        _fkBuilder!.Handle(token, ref _localRoot!);
    }

    private void HandleKeyConstraint(SqlToken token, ref SyntaxTreeNode localRoot)
    {
        if (localRoot.Value.Contains("FOREIGN", StringComparison.OrdinalIgnoreCase))
        {
            _fkBuilder!.Handle(token, ref localRoot);
        }

        // if (token.Value.Equals("KEY", StringComparison.OrdinalIgnoreCase) &&
        //     !localRoot.Value.Contains("KEY", StringComparison.OrdinalIgnoreCase))
        // {
        //     var isPrimaryKey = localRoot.Value.Contains("PRIMARY", StringComparison.OrdinalIgnoreCase);
        //     var nodeType = isPrimaryKey
        //         ? SyntaxTreeNodeType.PrimaryKeyDefinition
        //         : SyntaxTreeNodeType.ForeignKeyDefinition;
        //     var nodeValue = string.Concat(localRoot.Value.Trim(), " ", token.Value);
        //     var newNode = new SyntaxTreeNode(nodeType, nodeValue, localRoot.Parent);
        //     newNode.Children.AddRange(localRoot.Children);
        // 
        //     var tempNode = localRoot;
        //     localRoot = newNode;
        // 
        //     // Update parent with a new sub node
        //     var parent = localRoot.Parent!;
        //     parent.Children.Remove(tempNode);
        //     parent.Children.Add(newNode);
        // }
    }

    private bool IsConstraint(SqlToken token)
    {
        var isConstraintKeyWord = token.Value.Equals("constraint", StringComparison.OrdinalIgnoreCase);
        var isIdentifier = ParserConstants.Constraints
            .Any(c => c.Contains(token.Value, StringComparison.OrdinalIgnoreCase));

        return isConstraintKeyWord || isIdentifier;
    }

    private void BuildKeyConstraint(SqlToken token, ref SyntaxTreeNode localRoot)
    {
        // We temporarily add this constraint to the cols' root
        AddNode(SyntaxTreeNodeType.KeyDefinition, token.Value, ref localRoot!);
    }

    private void HandleColumnConstraintIdentifier(SqlToken token, ref SyntaxTreeNode localRoot)
    {
        // Handle composite constraints (e.g., NOT NULL)
        if (ParserConstants.Constraints.Contains(localRoot.Value + " " + token.Value, StringComparer.OrdinalIgnoreCase))
        {
            var updatedConstraintValue = localRoot.Value + " " + token.Value;
            UpdateColumnConstraintNode(updatedConstraintValue, ref localRoot);
        }
        else if (localRoot.Parent?.Type is SyntaxTreeNodeType.TableColumns)
        {
            // PRIMARY KEY (id). If we're here, we are looking at the col name.

            if (localRoot.Value.Equals("FOREIGN KEY", StringComparison.OrdinalIgnoreCase))
            {
                // This is a FOREIGN KEY CONSTRAINT, so we have to handle it appropriately.
                // FOREIGN KEY definition statement looks like:
                //
                // FOREIGN KEY id REFERENCES profiles(user_id)

                if (token.Type is SqlTokenType.Identifier)
                {
                    localRoot.AddChild(new SyntaxTreeNode(SyntaxTreeNodeType.KeyColumnIdentifier, token.Value,
                        localRoot));
                }

                Console.WriteLine("We are parsing FK constraint");
            }

            else if (localRoot.Value.Equals("PRIMARY KEY", StringComparison.OrdinalIgnoreCase))
            {
                var colsContainerNode = localRoot.Parent;

                colsContainerNode.Children.Remove(localRoot);
                var constraintOwner = colsContainerNode.Children.First(x => x!.Value.Equals(token.Value))!;

                var temp = new SyntaxTreeNode(localRoot.Type, localRoot.Value, constraintOwner);
                localRoot = temp;
                constraintOwner.Children.Add(temp);
            }
        }
        else
        {
            AddColumnConstraintNode(token, ref localRoot);
        }
    }

    private void UpdateColumnConstraintNode(string value, ref SyntaxTreeNode localRoot)
    {
        var parent = localRoot.Parent!;
        parent.Children.Remove(localRoot);

        var updatedConstraintNode = new SyntaxTreeNode(SyntaxTreeNodeType.ColumnConstraint, value, parent);
        parent.Children.Add(updatedConstraintNode);
        localRoot = updatedConstraintNode;
    }

    private void AddColumnConstraintNode(SqlToken token, ref SyntaxTreeNode localRoot)
    {
        var parent = localRoot.Parent!;
        var constraintNode = new SyntaxTreeNode(SyntaxTreeNodeType.ColumnConstraint, token.Value, parent);
        parent.Children.Add(constraintNode);
        localRoot = constraintNode;
    }

    #endregion

    #region Punctuation

    private void HandlePunctuation(SqlToken token, ref SyntaxTreeNode localRoot)
    {
        ArgumentNullException.ThrowIfNull(_root);
        ArgumentNullException.ThrowIfNull(localRoot);

        var isStatementClosingBracket = token.Value.Equals(")") && localRoot.Type is SyntaxTreeNodeType.TableColumns;
        if (token.Value.Equals(";") || isStatementClosingBracket)
        {
            // Reset to root on statement end
            localRoot = _root;
        }
        else
        {
            switch (localRoot.Type)
            {
                // TODO: Add Key Token Type Constraint
                case SyntaxTreeNodeType.TableRoot:
                    HandleTableRootPunctuation(ref localRoot);
                    break;
                case SyntaxTreeNodeType.ColumnConstraint:
                case SyntaxTreeNodeType.ColumnDataType:
                case SyntaxTreeNodeType.TableColumns:
                    HandleColumnConstraintOrDataTypePunctuation(token, ref localRoot);
                    break;
                case SyntaxTreeNodeType.DataTypeConstraint:
                    // Move back to the direct parent (likely ColumnDataType)
                    localRoot = localRoot.Parent!;
                    break;
                case SyntaxTreeNodeType.KeyDefinition:
                case SyntaxTreeNodeType.ForeignKeyDefinition:
                case SyntaxTreeNodeType.PrimaryKeyDefinition:
                    HandleKeyConstraintPunctuation(token, ref localRoot);
                    break;
                case SyntaxTreeNodeType.Root:
                case SyntaxTreeNodeType.CreateStatement:
                case SyntaxTreeNodeType.CreateTable:
                case SyntaxTreeNodeType.Column:
                default:
                    throw new NotImplementedException($"Punctuation is not expected after '{localRoot.Type}'");
            }
        }
    }

    private void HandleKeyConstraintPunctuation(SqlToken token, ref SyntaxTreeNode localRoot)
    {
        if (token.Value.Equals("("))
        {
            // just skip
        }
    }

    private void HandleTableRootPunctuation(ref SyntaxTreeNode localRoot)
    {
        ArgumentNullException.ThrowIfNull(localRoot);

        // When punctuation occurs under a TableRoot, transition to TableColumns
        AddNode(SyntaxTreeNodeType.TableColumns, localRoot.Value + "_Columns", ref localRoot!);
    }

    private void HandleColumnConstraintOrDataTypePunctuation(SqlToken token, ref SyntaxTreeNode localRoot)
    {
        if (token.Value.Equals("("))
        {
            // For opening parentheses, no action needed unless specific handling required
            // This might be a placeholder for future logic (e.g., parsing column length constraints)
        }
        else
        {
            if (localRoot.Type != SyntaxTreeNodeType.TableColumns)
            {
                // For other punctuation, validate and potentially move back to TableColumns
                EnsureNodeType(
                    ref localRoot,
                    SyntaxTreeNodeType.TableColumns);
            }
        }
    }

    private void EnsureNodeType(ref SyntaxTreeNode localRoot, SyntaxTreeNodeType expectedType)
    {
        const string errorMessageTemplate = "Expected to find a {0} parent node, but found {1}";

        // Navigate up two levels from the current localRoot, as ColumnDataType and ColumnConstraint
        // nodes are expected to be grandchildren of the TableColumns node.
        var potentialParent = localRoot.Parent?.Parent;
        if (potentialParent?.Type != expectedType)
        {
            var errorMessage = string.Format(errorMessageTemplate, expectedType, potentialParent?.Type);
            throw new InvalidOperationException(errorMessage);
        }

        localRoot = potentialParent;
    }

    #endregion

    #region Number

    private void HandleNumber(SqlToken token, ref SyntaxTreeNode localRoot)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(localRoot);

        if (localRoot.Type == SyntaxTreeNodeType.ColumnDataType)
        {
            // If the current node is a ColumnDataType, add a DataTypeConstraint node with the number.
            AddNode(SyntaxTreeNodeType.DataTypeConstraint, token.Value, ref localRoot!);
        }
        else
        {
            // TODO[#2]: Implement more complex scenarios: e.g., number in value lists or as default values
            throw new NotImplementedException("<2>: Unsupported scenario");
        }
    }

    #endregion

    private void AddNode(SyntaxTreeNodeType type, string value, ref SyntaxTreeNode? currentRootNode)
    {
        var newNode = new SyntaxTreeNode(type, value, currentRootNode);
        currentRootNode?.Children.Add(newNode);
        currentRootNode = newNode;
    }
}

/*

Keyword: CREATE
Keyword: TABLE
Identifier: Users
Punctuation: (
Identifier: Id
Identifier: INT
Identifier: PRIMARY
Identifier: KEY
Punctuation: ,
Identifier: Name
Identifier: VARCHAR
Punctuation: (
Number: 255.55
Punctuation: )
Punctuation: )
Punctuation: ;

    SQL_Script
        Create_Table
            Users
                id int primary key              --> id (column), int (datatype), primary key (constraint)
                name varchar(256) not null      -->

        ------

        Root
            Children:
                - Create_Table:             // local_root (lr)
                    - Columns:              // lr.Children -- iterate over cols
                        - id:
                            datatype        // lr.Children[i].First() - datatype
                            constraints     // lr.Children[1..] - constraints
                        - name:
                            datatype        // lr.Children[i].First() - datatype
                            constraints     // lr.Children[1..] - constraints
        ------

        Create_Table
            Profiles
                id int auto_increment primary key


 */
