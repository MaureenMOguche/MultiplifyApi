namespace Multiplify.Application.PageTemplates;
public static class PageTemplates
{
    public static string GetIndexPage(string? assemblyName, string? iconName=null)
    {
        return $@"<!DOCTYPE html>
            <html>
            <head>
                <meta charset=""utf-8"" />
                <link rel=""icon"" type=""image/x-icon"" href=""images/{iconName}""/>
                <title>Regolith - Api Home</title>
            </head>
            <body>
                <h1>Welcome to {assemblyName ?? "Multiplify"}</h1>
                
                <a href=""swagger/index.html"">Go to SwaggerUI</a>

                <a href=""documentation.html"">Go to Documentation</a>

            </body>
            </html>";
    }
}
