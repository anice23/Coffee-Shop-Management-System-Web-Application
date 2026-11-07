using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoffeeShop.API.Swagger;

public class CsrfHeaderOperationFilter : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        var method = context.ApiDescription.HttpMethod;

        if (method is not ("POST" or "PUT" or "PATCH" or "DELETE"))
            return;

        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-CSRF-TOKEN",
            In = ParameterLocation.Header,
            Required = true,
            Description = "CSRF request token from GET /api/Auth/csrf",
            Schema = new OpenApiSchema
            {
                Type = "string"
            }
        });
    }
}