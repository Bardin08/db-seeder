In the heart of our parsing model lies the Abstract Syntax Tree (AST), which has been slightly modified from its
traditional structure. Each node within the tree not only represents a construct from the parsed document but also
contains a link to its parent element. This linkage is crucial for understanding the current parsing context and
predicting subsequent parse states.

#### Node Types in the Syntax Tree:

The syntax tree comprises various node types, each representing a different construct within the parsed SQL script.

|    Node Type     | Description                                                   |
|:----------------:|---------------------------------------------------------------|
|       Root       | Represents the starting point of the SQL script.              |
| CreateStatement  | Indicates the beginning of a CREATE statement in SQL.         |
|   CreateTable    | Signifies the creation of a table.                            |
|    TableRoot     | The root node for a table, containing the table's name.       |
|   TableColumns   | A node representing the collection of columns within a table. |
|      Column      | Denotes a single column within a table.                       |
|  ColumnDataType  | Specifies the data type of a column.                          |
| ColumnConstraint | Represents constraints applied to a column, such as NOT NULL. |

![](../docs/imgs/ast-example.png)

### Syntax Tree Building Rules:

To construct the syntax tree, we process SQL tokens sequentially as received from the Lexer. This process involves
evaluating the tokens' types and values to generate an AST, which can then be utilized to generate the corresponding SQL
Schema as a Plain Old CLR Object (POCO).

1. **Rule 1:** Every SQL script starts with a `Root` node. From there, we traverse and build the tree based on the type
   and value of each encountered token.
2. **Rule 2:** A `CreateStatement` node is added upon encountering a "create" keyword, indicating the start of a create
   statement.
3. **Rule 3:** When a "table" keyword follows a `CreateStatement`, a `CreateTable` node is added, marking the creation
   of a new table.
4. **Rule 4:** Identifiers immediately following a `CreateTable` node specify the table's root (`TableRoot`) and its
   name.
5. **Rule 5:** Subsequent identifiers indicate column names (`Column` nodes), data types (`ColumnDataType` nodes), and
   constraints (`ColumnConstraint` nodes) as the structure is built out.

![](../docs/imgs/ast-building.png)
