using System.ComponentModel;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LogisticsPlatform.Swagger;

internal sealed class DescriptionParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        DescriptionAttribute? description = context.PropertyInfo?
            .GetCustomAttribute<DescriptionAttribute>();

        if (description is null)
            return;

        parameter.Description = description.Description;
    }
}
