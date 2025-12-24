var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello You're welcome to TaskMaster!");


app.Run();
