## Abstract Syntax Tree (AST) Validation

At the core of our SQL parsing logic is the AST validation process. This crucial step ensures that the constructed tree
accurately represents the SQL script and adheres to SQL standards (e.g., a table must contain a single PRIMARY KEY).

### Validation Rules:

Our validation framework is designed around a set of predefined rules. Each rule targets specific aspects of the AST to
ensure compliance with SQL syntax and structure.

| Rule                              | Description                                                                                       | Classes Containing Validation Logic               |
|-----------------------------------|---------------------------------------------------------------------------------------------------|---------------------------------------------------|
| Syntax Tree Structure Validation  | Ensures the SQL query was parsed correctly, and the AST structure adheres to expected hierarchies | `TreeStructureValidator`, `ValidStructureRule`    |
| Primary Key Constraint Validation | Verifies that each table has a single PRIMARY KEY constraint                                      | `PrimaryKeyConstraintValidator`, `PrimaryKeyRule` |
| ...                               | Additional rules can be implemented as needed                                                     | ...                                               |

### Validation Module Architecture

The validation module leverages a visitor pattern through the `IAstVisitor` interface to traverse the AST. Validation
logic is encapsulated in rules and validators, allowing for modular and extensible design. The `ValidationContext`
record plays a pivotal role in collecting and managing validation outcomes.

1. **Validation Context (`ValidationContext`):** Centralizes the management of validation rules and outcomes. It
   supports adding rules and accumulating validation results, which can be aggregated into a single outcome.

2. **AST Validator (`AstValidator`):** Implements the `IAstVisitor` interface. It initiates the validation process,
   traverses the AST, and utilizes `GenericTreeValidator` to apply validation rules.

3. **Generic Tree Validator (`GenericTreeValidator`):** Coordinates the application of validation rules to each node in
   the AST. It relies on a collection of `INodeValidator` instances to validate node-specific constraints.

4. **Validation Rules:** Implementations of `IValidationRule` and `INodeValidator` define specific validation logic,
   such as `ValidStructureRule`, which checks the node against allowed child types.

![](./imgs/ast-validation.png)

### Implementing New Validation Rules

To add a new validation rule:

1. **Define the Rule:** Implement `IValidationRule` with the specific validation logic.
2. **Register the Rule:** Add the new rule instance to the `ValidationContext` within `GenericTreeValidator`.

This modular approach facilitates the easy expansion of the validation framework to accommodate new SQL syntax rules and
AST structures.
