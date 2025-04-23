
using Fora.Infrastructure.Api.SEC;

namespace Fora.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<Infrastructure.Api.SEC.Models.SECConfig>(c => {
                var configService  = c.GetService<IConfiguration>();
                var url = configService.GetValue<string>("SEC:Url") ?? throw new Exception("Missing the SEC URL");
                return new Infrastructure.Api.SEC.Models.SECConfig() { Url = url };
            });

            builder.Services.AddAutoMapper(typeof(SECHttpService).Assembly, typeof(Program).Assembly);

            Business.Services.Configure(builder.Services);
            Infrastructure.Api.SEC.Services.Configure(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
