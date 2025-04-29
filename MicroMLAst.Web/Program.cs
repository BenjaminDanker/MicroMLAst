using MicroMLAst.Web; 
using MicroMLAst.Core;
using MicroMLAst.Core.Parsing;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// register the Core parser service
builder.Services.AddMicroML();

var app = builder.Build();

// serve index.html, CSS, JS
app.UseStaticFiles();

// JSON POST endpoint
app.MapPost("/api/parse", (CodeDto dto, IParser parser) =>
{
    var ast     = parser.Parse(dto.code);
    var mermaid = new MicroMLAst.Core.Ast.MermaidVisitor().Render(ast);
    return Results.Json(new { mermaid });
});

app.MapFallbackToFile("index.html");

app.Run();
