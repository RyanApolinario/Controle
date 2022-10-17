using CloudNative.CloudEvents;
using entidades;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Mime;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Controle
{
    /// <summary>
    /// A function that can be triggered in responses to changes in Google Cloud Storage.
    /// The type argument (StorageObjectData in this case) determines how the event payload is deserialized.
    /// The function must be deployed so that the trigger matches the expected payload type. (For example,
    /// deploying a function expecting a StorageObject payload will not work for a trigger that provides
    /// a FirestoreEvent.)
    /// </summary>
    public class FunctionAlter : FunctionsStartup
    {
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddDbContext<Context>(options => options.UseSqlServer());
        }
    }
    [FunctionsStartup(typeof(FunctionAlter))]
    public class AlterarMarca : IHttpFunction
    {
        private readonly ILogger _logger;
        private readonly Context _databaseContext;
        public AlterarMarca(ILogger<Cliente> logger, Context context)
        {
            _databaseContext = context;
            _logger = logger;
        }
        public async Task HandleAsync(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            ContentType contentType = new ContentType(request.ContentType);


            if (contentType.MediaType != "application/json" || request.Method != "POST")
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("Wrong Method");
            }

            using TextReader reader = new StreamReader(request.Body);
            string json = await reader.ReadToEndAsync();
            JsonElement body = JsonSerializer.Deserialize<JsonElement>(json.ToLower());

            if (body.TryGetProperty("id", out JsonElement propertyId) && propertyId.ValueKind == JsonValueKind.String)
            {
                body.TryGetProperty("nome", out JsonElement propertyNome);
                body.TryGetProperty("Documento", out JsonElement propertyDocumento);


                if (string.IsNullOrEmpty(propertyNome.ToString()))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync("Sem Descrição");
                    return;
                }

                Cliente Cliente = _databaseContext.Cliente.Find(Convert.ToInt32(propertyId.GetString()));
                if (Cliente == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync("Cliente não encontrado");
                    return;
                }

                Cliente.Nome = propertyNome.ToString();
                Cliente.Documento = propertyDocumento.ToString();

                _databaseContext.Cliente.Update(Cliente);

                await _databaseContext.SaveChangesAsync();

                await context.Response.WriteAsync("Marca alterada com sucesso.");
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }
}

