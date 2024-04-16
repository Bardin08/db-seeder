using System.Text;
using DbSeeder.SqlParser.SyntaxTree.Validation.Validators;

namespace DbSeeder.SqlParser.SyntaxTree.Validation;

public class AstValidator : IAstVisitor
{
    // TODO: Replace Console.WriteLine with logger.
    public void Visit(SyntaxTreeNode? node)
    {
        Console.WriteLine("Syntax Tree validation started...");

        if (node is null)
        {
            Console.WriteLine("Passed node is null, and can't be validated. Syntax tree is not exists...");
            return;
        }

        var validator = new GenericTreeValidator();
        var validationResult = validator.Validate(node);
        if (!validationResult.IsValid)
        {
            var sb = new StringBuilder("Validation failed.").AppendLine();

            foreach (var error in validationResult.Errors)
            {
                sb.AppendLine($"\t- {error}");
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(sb.ToString());
            Console.ResetColor();
            return;
        }

        Console.WriteLine("Validation Completed! The syntax tree is valid");
    }
}
