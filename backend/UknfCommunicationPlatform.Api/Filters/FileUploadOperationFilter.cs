using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace UknfCommunicationPlatform.Api.Filters;

/// <summary>
/// Operation filter to properly document file upload endpoints in Swagger
/// </summary>
public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if this operation has IFormFile parameters
        var formFileParams = context.ApiDescription.ParameterDescriptions
            .Where(p => p.ModelMetadata != null &&
                       (p.ModelMetadata.ModelType == typeof(IFormFile) ||
                        p.ModelMetadata.ModelType == typeof(IFormFile[])))
            .ToList();

        if (!formFileParams.Any())
            return;

        // Clear existing parameters since we'll define them in the request body
        operation.Parameters.Clear();

        // Create multipart/form-data request body
        var properties = new Dictionary<string, OpenApiSchema>();
        var required = new HashSet<string>();

        // Add all parameters from the action
        foreach (var param in context.ApiDescription.ParameterDescriptions)
        {
            if (param.Source?.Id == "Form" || param.Source?.Id == "FormFile")
            {
                var paramName = param.Name;

                if (param.ModelMetadata?.ModelType == typeof(IFormFile))
                {
                    properties[paramName] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    };
                    if (param.IsRequired)
                    {
                        required.Add(paramName);
                    }
                }
                else if (param.ModelMetadata?.ModelType == typeof(IFormFile[]))
                {
                    properties[paramName] = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    };
                    if (param.IsRequired)
                    {
                        required.Add(paramName);
                    }
                }
                else
                {
                    // String or other form parameter
                    properties[paramName] = new OpenApiSchema
                    {
                        Type = "string"
                    };
                    if (param.IsRequired)
                    {
                        required.Add(paramName);
                    }
                }
            }
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = properties,
                        Required = required
                    }
                }
            }
        };
    }
}
