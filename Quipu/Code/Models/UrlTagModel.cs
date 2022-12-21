using System;

namespace Quipu.Code.Models;
public class UrlTagModel
{
    private String _url;
    private String _tag;
    private Int32 _tagCount;

    public UrlTagModel(String url, String tag, Int32 tagCount)
    {
        _url = url;
        _tag = tag;
        _tagCount = tagCount;
    }

    public String Url => _url;
    public String Tag => _tag;
    public Int32 TagCount => _tagCount;
}
