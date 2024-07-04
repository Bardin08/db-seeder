using Bogus;
using Bogus.DataSets;

namespace DbSeeder.Data.Bogus;

public static class ParamsGenerator
{
    private const int MinNegativeNumber = int.MinValue;
    private const int MaxNegativeNumber = -1;
    private const int MinPositiveNumber = 0;
    private const int MaxPositiveNumber = int.MaxValue;

    private static readonly Faker Faker = new();

    public static object[] GetParams(string generatorId)
    {
        return GetRandomGeneratorParams(generatorId);
    }

    private static object[] GetRandomGeneratorParams(string generatorId)
    {
        var minMaxInt = GetMinMaxIntValues();
        var minMaxDouble = GetMinMaxDoubleValues();

        return generatorId switch
        {
            "randomodd" => [0, 100],
            "randomeven" => [0, 100],
            "randomnumber" => [minMaxInt[1]],
            "randomdigits" => [GetRandomInt(1, 10), 0, 9],
            "randomdecimal" => [minMaxDouble.Min, minMaxDouble.Max],
            "randomfloat" => [minMaxDouble.Min, minMaxDouble.Max],
            "randombytes" => [GetRandomInt(1, 100)],
            "randomsbyte" => [MinNegativeNumber, MaxNegativeNumber],
            "randomuint" => [MinPositiveNumber, MaxPositiveNumber],
            "randomulong" => [0, ulong.MaxValue],
            "randomlong" => [0, long.MaxValue],
            "randomshort" => [short.MinValue, short.MaxValue],
            "randomushort" => [ushort.MinValue, ushort.MaxValue],
            "randomchar" => ['a', 'z'],
            "randomchars" => ['a', 'z', GetRandomInt(1, 10)],
            "randomstring" => [GetRandomInt(1, 100), 'a', 'z'],
            "randomstring2" => [GetRandomInt(1, 100), "abcdef"],
            "randomutf16string" => [GetRandomInt(1, 100), GetRandomInt(1, 100), true],
            "randomhash" => [GetRandomInt(1, 100), true],
            "randombool" => [0.5],
            "randomarrayelement" => [GetRandomArray()],
            "randomarrayelements" => [GetRandomArray(), GetRandomInt(1, 10)],
            "randomlistitem" => [GetRandomList()],
            "randomlistitems" => [GetRandomList(), GetRandomInt(1, 10)],
            "randomcollectionitem" => [GetRandomCollection()],
            "randomreplacenumbers" => ["###-##-####", '#'],
            "randomreplacesymbols" => ["###-##-####", '#', (Func<char, char>)(_ => '*')],
            "randomreplace" => ["###-##-####"],
            "randomclampstring" => ["sample", minMaxInt[0], minMaxInt[1]],
            "randomenum" => [Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToArray()],
            "randomenumvalues" => [GetRandomInt(1, 7), Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToArray()],
            "randomshuffle" => [GetRandomList()],
            "randomwords" => [GetRandomInt(1, 10)],
            "randomwordsarray" => [GetRandomInt(1, 10)],
            "randomalphanumeric" => [GetRandomInt(1, 100)],
            "randomhexadecimal" => [GetRandomInt(1, 100), true],
            "randomweightedrandom" => [GetRandomList(), GetRandomWeights()],
            "phonephonenumber" => [GetPhoneNumberFormat()],
            "phonephonenumberformat" => [GetRandomInt(0, 5)],
            "namefirstname" => [GetRandomGender()],
            "namelastname" => [GetRandomGender()],
            "namefullname" => [GetRandomGender()],
            "nameprefix" => [GetRandomGender()],
            "namefindname" => [GetRandomFirstName(), GetRandomLastName(), true, false, GetRandomGender()],
            "loremwords" => [GetRandomInt(1, 10)],
            "loremletter" => [GetRandomInt(1, 26)],
            "loremsentence" => [GetRandomInt(1, 20), GetRandomInt(1, 5)],
            "loremsentences" => [GetRandomInt(1, 10), " "],
            "loremparagraph" => [GetRandomInt(1, 5)],
            "loremparagraphs" => [GetRandomInt(1, 10), " "],
            "loremparagraphsminmax" => [GetRandomInt(1, 5), GetRandomInt(5, 10), " "],
            "loremlines" => [GetRandomInt(1, 10), "\n"],
            "loremslug" => [GetRandomInt(1, 10)],
            "imagedatauri" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomColor()],
            "imageplaceimgurl" =>
            [
                GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomCategory(), GetRandomFilter()
            ],
            "imagepicsumurl" =>
            [
                GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomInt(1, 10),
                GetRandomInt(1, 1000)
            ],
            "imageplaceholderurl" =>
            [
                GetRandomInt(100, 1000), GetRandomInt(100, 1000), "Placeholder", GetRandomColor(), GetRandomColor(),
                "png"
            ],
            "imageloremflickrurl" =>
            [
                GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomKeywords(), GetRandomBool(), GetRandomBool(),
                GetRandomInt(1, 1000)
            ],
            "imagelorempixelurl" =>
            [
                GetRandomCategory(), GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()
            ],
            "imageimage" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imageabstract" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imageanimals" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagebusiness" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagecats" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagecity" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagefood" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagenightlife" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagefashion" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagepeople" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagenature" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagesports" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagetechnics" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "imagetransport" => [GetRandomInt(100, 1000), GetRandomInt(100, 1000), GetRandomBool(), GetRandomBool()],
            "financeaccount" => [GetRandomInt(1, 20)],
            "financeamount" => [minMaxDouble.Min, minMaxDouble.Max, GetRandomInt(0, 5)],
            "financecurrency" => [GetRandomBool()],
            "financecreditcardnumber" => [GetRandomProvider()],
            "financeiban" => [GetRandomBool(), GetRandomCountryCode()],
            "addresszipcode" => [GetRandomFormat()],
            "addressstreetaddress" => [GetRandomBool()],
            "addresscountrycode" => [GetRandomFormat()],
            "addresslatitude" => [minMaxDouble.Min, minMaxDouble.Max],
            "addresslongitude" => [minMaxDouble.Min, minMaxDouble.Max],
            "addressdirection" => [GetRandomBool()],
            "addresscardinaldirection" => [GetRandomBool()],
            "addressordinaldirection" => [GetRandomBool()],
            "datepast" => [GetRandomInt(1, 100), DateTime.Now],
            "datepastoffset" => [GetRandomInt(1, 100), DateTimeOffset.Now],
            "datesoon" => [GetRandomInt(1, 100), DateTime.Now],
            "datesoonoffset" => [GetRandomInt(1, 100), DateTimeOffset.Now],
            "datefuture" => [GetRandomInt(1, 100), DateTime.Now],
            "datefutureoffset" => [GetRandomInt(1, 100), DateTimeOffset.Now],
            "datebetween" => [DateTime.Now.AddYears(-10), DateTime.Now.AddYears(10)],
            "datebetweenoffset" => [DateTimeOffset.Now.AddYears(-10), DateTimeOffset.Now.AddYears(10)],
            "daterecent" => [GetRandomInt(1, 10), DateTime.Now],
            "daterecentoffset" => [GetRandomInt(1, 10), DateTimeOffset.Now],
            "datetimespan" => [GetRandomInt(1, 100)],
            "datemonth" => [GetRandomBool(), GetRandomBool()],
            "dateweekday" => [GetRandomBool(), GetRandomBool()],
            "datebetweendateonly" => [DateTime.Now.AddYears(-10).Date, DateTime.Now.AddYears(10).Date],
            "datepastdateonly" => [GetRandomInt(1, 100), DateTime.Now.Date],
            "datesoondateonly" => [GetRandomInt(1, 100), DateTime.Now.Date],
            "datefuturedateonly" => [GetRandomInt(1, 100), DateTime.Now.Date],
            "daterecentdateonly" => [GetRandomInt(1, 10), DateTime.Now.Date],
            "datebetweentimeonly" => [DateTime.Now.TimeOfDay, DateTime.Now.AddHours(10).TimeOfDay],
            "datesoontimeonly" => [GetRandomInt(1, 60), DateTime.Now.TimeOfDay],
            "daterecenttimeonly" => [GetRandomInt(1, 60), DateTime.Now.TimeOfDay],
            "companycompanyname" => [GetRandomInt(0, 5)],
            "companycompanynameformat" => [GetRandomInt(0, 5)],
            "internetemail" => [GetRandomFirstName(), GetRandomLastName(), GetRandomProvider(), GetRandomBool()],
            "internetexampleemail" => [GetRandomFirstName(), GetRandomLastName()],
            "internetusername" => [GetRandomFirstName(), GetRandomLastName()],
            "internetusernameunicode" => [GetRandomFirstName(), GetRandomLastName()],
            "internetmac" => [GetRandomSeparator()],
            "internetpassword" => [GetRandomInt(6, 20), GetRandomBool(), "^[a-zA-Z0-9]+$", "prefix"],
            "internetcolor" =>
            [
                GetRandomInt(0, 255), GetRandomInt(0, 255), GetRandomInt(0, 255), GetRandomBool(), "hex"
            ],
            "interneturlwithpath" => ["https", "example.com", "jpg"],
            "interneturlrootedpath" => ["jpg"],
            "commercedepartment" => [GetRandomInt(1, 10), GetRandomBool()],
            "commerceprice" => [minMaxDouble.Min, minMaxDouble.Max, GetRandomInt(0, 5), "$"],
            "commercecategories" => [GetRandomInt(1, 10)],
            "systemfilename" => ["txt"],
            "systemcommonfilename" => ["txt"],
            "systemfileext" => ["text/plain"],
            "rantreview" => ["product"],
            "rantreviews" => ["product", GetRandomInt(1, 10)],
            "vehiclevin" => [GetRandomBool()],
            _ => throw new ArgumentException($"Unknown generatorId: {generatorId}")
        };
    }

    private static int[] GetMinMaxIntValues()
    {
        var random = Random.Shared;
        var num1 = random.Next(MinPositiveNumber, MaxPositiveNumber);
        var num2 = random.Next(MinPositiveNumber, MaxPositiveNumber);

        var min = Math.Min(num1, num2);
        var max = Math.Max(num1, num2);

        return [min, max];
    }

    private static (double Min, double Max) GetMinMaxDoubleValues()
    {
        var random = Random.Shared;
        var num1 = random.NextDouble() * MaxPositiveNumber;
        var num2 = random.NextDouble() * MaxPositiveNumber;

        var min = Math.Min(num1, num2);
        var max = Math.Max(num1, num2);

        return (min, max);
    }

    private static int GetRandomInt(int min, int max) => Faker.Random.Number(min, max);
    private static object[] GetRandomArray() => GetRandomCollection().ToArray();
    private static List<object> GetRandomList() => GetRandomCollection().ToList();

    private static ICollection<object> GetRandomCollection()
        => Enumerable.Range(0, Faker.Random.Number(100))
            .Select(_ => (object)Guid.NewGuid().ToString())
            .ToList();

    private static List<int> GetRandomWeights() => Faker.Random.Digits(5).ToList();
    private static string GetPhoneNumberFormat() => "+1-###-###-####";
    private static Name.Gender GetRandomGender() => Faker.Random.ArrayElement([Name.Gender.Male, Name.Gender.Female]);
    private static string GetRandomFirstName() => Faker.Person.FirstName;
    private static string GetRandomLastName() => Faker.Person.LastName;
    private static string GetRandomColor() => "#FF5733";
    private static string GetRandomCategory() => "nature";
    private static string GetRandomFilter() => "grayscale";
    private static bool GetRandomBool() => Faker.Random.Bool(0.5f);
    private static string GetRandomProvider() => "Visa";
    private static string GetRandomCountryCode() => "US";
    private static string GetRandomFormat() => "#####";
    private static string[] GetRandomKeywords() => Faker.Lorem.Words();
    private static string GetRandomSeparator() => "-";
}
