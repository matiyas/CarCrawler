using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace CarCrawler.Database;

internal class EnumerableConverter : ValueConverter<IEnumerable<string>, string>
{
    public EnumerableConverter() :
        base(ConvertFromEnumerableToString, ConvertFromStringToEnumerable)
    { }

    private static readonly Expression<Func<IEnumerable<string>, string>> ConvertFromEnumerableToString = e => string.Join(";", e);

    private static readonly Expression<Func<string, IEnumerable<string>>> ConvertFromStringToEnumerable = s => s.Split(";", StringSplitOptions.None);
}