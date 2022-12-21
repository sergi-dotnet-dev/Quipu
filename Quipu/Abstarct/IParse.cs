using System;
using System.Threading.Tasks;

namespace Quipu.Abstarct;
public interface IParse
{
    Task<String[]> Parse(String text);
}
