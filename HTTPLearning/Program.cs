using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Wait 30 seconds for graceful shutdown.
builder.Host.ConfigureHostOptions(o => o.ShutdownTimeout = TimeSpan.FromSeconds(30));
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

app.Logger.LogInformation("The app has started");

app.MapGet("/", () => "Hello World!");

app.MapGet("/message", () => new { name = "Hello World!" } );

app.MapGet("/api/book", () => "Hello World Book!");

app.MapGet("/{id}", (HttpRequest request) =>
{
    var host = request.Host;
    var pro = request.Protocol;
    app.Logger.LogError($"Internal Error { host.Port}, {pro}");
    return TypedResults.BadRequest();
});


//app.MapPost("/name", (HttpRequest request, HttpResponse response) =>
//{
//    var host = request.Host;
//    var pro = request.Protocol;
//    response.Body = request.Body;
//    app.Logger.LogInformation($" {request.Body}");
//    return Results.Ok(request.Body);
//});


app.MapGet("/contact/{id}", (HttpRequest request, HttpContext httpContext) =>
{
    var user = httpContext.User.Identity!.Name ?? "Bossman";
    app.Logger.LogInformation($"Logged in username: {user}");
    return Results.Problem();
})
.Produces(StatusCodes.Status201Created)
.Produces(StatusCodes.Status304NotModified)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError)
.Produces(StatusCodes.Status503ServiceUnavailable);


app.MapGet("/orders/{orderId}", IResult (int orderId)
    => orderId > 999 ? TypedResults.BadRequest() : TypedResults.Ok(new Order(orderId)))
    .Produces(400)
    .Produces<Order>();

//app.StopAsync(TimeSpan.FromMinutes(2));


app.MapGet("/hello", () => Results.Ok(new Message() { Text = "Hello World!" }))
    .Produces<Message>();


//Run
app.Run();


internal class Message
{
    public string Text { get; set; } = string.Empty;
}

internal class Order
{
    private int orderId;

    public Order(int orderId)
    {
        this.orderId = orderId;
    }
}

