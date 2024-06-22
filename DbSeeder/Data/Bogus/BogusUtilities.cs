using System.Reflection;
using Bogus;

namespace DbSeeder.Data.Bogus;

public static class BogusUtilities
{
    private static readonly Dictionary<string, HashSet<Type>> AllowedTypes = new()
    {
        { "string", [typeof(string), typeof(char?), typeof(char[])] },
        {
            "number",
            [
                typeof(int?), typeof(float?), typeof(double?), typeof(byte?), typeof(int), typeof(float),
                typeof(double), typeof(byte)
            ]
        }
    };

    public static Dictionary<string, List<BogusGeneratorDescriptor>> GetBogusGenerators()
    {
        var generators = new Dictionary<string, List<BogusGeneratorDescriptor>>();

        var fakerType = typeof(Faker);

        var props = fakerType.GetProperties()
            .Where(p => p.CustomAttributes.Any(x => x.AttributeType.Name == "RegisterMustasheMethodsAttribute"))
            .ToList();

        foreach (var p in props)
        {
            var generatorCategory = p.Name.ToLower();
            if (!generators.ContainsKey(generatorCategory))
            {
                generators[generatorCategory] = [];
            }

            var methods = p.PropertyType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => m.IsPublic &&
                            !(m.Name.StartsWith("get_") || m.Name.StartsWith("set_")))
                .ToList();

            foreach (var m in methods)
            {
                var methodParams = m.GetParameters().ToDictionary(x => x.Name!, x => x.ParameterType);
                var generator = new BogusGeneratorDescriptor(
                    generatorCategory,
                    generatorCategory + m.Name.ToLower(),
                    m.ReturnType,
                    methodParams);

                generators[generatorCategory].Add(generator);
            }
        }

        return generators;
    }

    public static Dictionary<string, List<BogusGeneratorDescriptor>> GetFiltersForReturnType(
        this Dictionary<string, List<BogusGeneratorDescriptor>> src, string returnType)
    {
        var allowed = AllowedTypes[returnType];

        var allowedGenerators = new Dictionary<string, List<BogusGeneratorDescriptor>>();
        foreach (var (category, generators) in src)
        {
            allowedGenerators.Add(category, []);
            foreach (var generator in generators)
            {
                if (allowed.Contains(generator.ReturnType))
                {
                    allowedGenerators[category].Add(generator);
                }
            }
        }

        return allowedGenerators;
    }

    public static dynamic? Generate(BogusGeneratorDescriptor generatorDescriptor)
    {
        var faker = new Faker();
        var generationMethod = generatorDescriptor.GeneratorIdentifier[generatorDescriptor.Category.Length..];

        var generatorProperty = faker.GetType().GetProperty(generatorDescriptor.Category,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (generatorProperty != null)
        {
            var categoryGenerator = generatorProperty.GetValue(faker);
            var generatorMethod = categoryGenerator.GetType().GetMethod(generationMethod,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (generatorMethod != null)
            {
                var @params = (object[])null!;

                var parameters = generatorMethod.GetParameters();
                if (parameters.Length > 0)
                {
                    @params = ParamsGenerator.GetParams(generatorDescriptor.GeneratorIdentifier);
                }

                // TODO[#27]: Implement constraints handling
                var result = generatorMethod.Invoke(categoryGenerator, @params);
                return Convert.ChangeType(result, generatorMethod.ReturnType)!;
            }

            Console.WriteLine($"Method '{generationMethod}' not found in '{generatorDescriptor.Category}' category");
        }
        else
        {
            Console.WriteLine($"Category '{generatorDescriptor.Category}' not found on Faker object");
        }

        return null;
    }
}
