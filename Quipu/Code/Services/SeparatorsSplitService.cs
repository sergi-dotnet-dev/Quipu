using System;
using System.Threading.Tasks;
using Quipu.Abstarct;

namespace Quipu.Code.Services;
public class SeparatorsSplitService : IParse
{
    readonly Char[] separators = { ' ', ',', '\n', '\t', '\r' };
    public async Task<String[]> Parse(String text)
        => await Task.Run(() => text.Split(separators, StringSplitOptions.RemoveEmptyEntries));
}
