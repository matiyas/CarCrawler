using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace CarCrawler.Database;

internal class EnumerableConverter : ValueConverter<IEnumerable<string>, string>
{
    public EnumerableConverter () : 
        base(ConvertFromEnumerableToString, ConvertFromStringToEnumerable) 
    { }

    static readonly Expression<Func<IEnumerable<string>, string>> ConvertFromEnumerableToString = e => string.Join(";", e);

    static readonly Expression<Func<string, IEnumerable<string>>> ConvertFromStringToEnumerable = s => new[] { "1" };
}
