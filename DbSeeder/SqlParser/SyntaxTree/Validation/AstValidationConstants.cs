namespace DbSeeder.SqlParser.SyntaxTree.Validation;

// TODO[#3]: Move this to rsx files for localization
public static class AstValidationConstants
{
    public static class CreateStatement
    {
        public static string UnsupportedStatement =>
            "Now DbSeeder support only create table statements";
    }

    public static class Generic
    {
        public static string ChildNullError(SyntaxTreeNodeType nodeType)
            => $"{nodeType}'s child is null";

        public static string UnsupportedChildTypeError(SyntaxTreeNodeType nodeType, SyntaxTreeNodeType unsupportedType)
            => $"{nodeType} node contains unsupported node of type {unsupportedType}";

        public static string InvalidChild(SyntaxTreeNodeType nodeType, IEnumerable<SyntaxTreeNodeType> supportedTypes)
            => $"{nodeType}'s child should be one of: {supportedTypes.Select(x => x.ToString())}";
    }
}
