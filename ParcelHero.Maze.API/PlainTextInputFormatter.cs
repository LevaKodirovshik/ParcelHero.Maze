using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace ParcelHero.Maze.API;

public class PlainTextInputFormatter : TextInputFormatter
{
    public PlainTextInputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));

        SupportedEncodings.Add(Encoding.ASCII);
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding effectiveEncoding)
    {
        var request = context.HttpContext.Request;
        using (var reader = new StreamReader(request.Body, effectiveEncoding))
        {
            var stringContent = await reader.ReadToEndAsync();
            return await InputFormatterResult.SuccessAsync(stringContent);
        }
    }
}