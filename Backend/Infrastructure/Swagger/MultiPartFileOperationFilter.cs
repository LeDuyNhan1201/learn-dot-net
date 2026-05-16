using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Swagger
{
    public sealed class MultiPartFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formFileParams = context.MethodInfo
                .GetParameters()
                .Where(p =>
                    p.ParameterType == typeof(IFormFile) ||
                    p.ParameterType == typeof(IEnumerable<IFormFile>))
                .ToList();

            if (formFileParams.Count == 0)
            {
                return;
            }

            Dictionary<string, IOpenApiSchema> properties = formFileParams.ToDictionary
            (
                p => p.Name!, 
                _ => new OpenApiSchema 
                {
                    Type = JsonSchemaType.String,
                    Format = "binary"
                } as IOpenApiSchema);

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new()
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Object,
                            Properties = properties
                        }
                    }
                }
            };
        }
    }
}
