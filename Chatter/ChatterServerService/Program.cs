namespace ChatterServerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create the web application builder.
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddGrpc();

            // Build the app.
            var app = builder.Build();

            // Configure the web application.
            app.MapGrpcService<ChatterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through the Chatter client.");

            // Run the app.
            app.Run();
        }
    }
}