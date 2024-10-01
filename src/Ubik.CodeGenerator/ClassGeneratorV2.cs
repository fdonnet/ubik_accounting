using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Ubik.Security.Api.Data;

namespace Ubik.CodeGenerator
{
    internal class ClassGeneratorV2(SecurityDbContext dbContext)
    {
        public void GenerateClassesContractAddCommand()
        {
            var entityTypes = dbContext.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                var className = entityType.ClrType.Name;
                var excludedFiels = new List<string>()
                {
                    "CreatedAt",
                    "CreatedBy",
                    "ModifiedAt",
                    "ModifiedBy",
                    "Id",
                    "Version"
                };

                var properties = GenerateProperties(entityType, true, excludedFiels);
                var classContent = GetTemplateForContractCommandAdd().Replace("{ClassName}", className)
                                                                    .Replace("{Properties}", properties);

                Console.WriteLine(classContent);
            }
        }

        private string GenerateProperties(IEntityType entityType, bool withAnnotations, List<string> excludedFiedls)
        {
            var properties = entityType.GetProperties();
            var sb = new StringBuilder();

            var i = 1;
            foreach (var property in properties)
            {
                if (!excludedFiedls.Contains(property.Name))
                {
                    if (i == 1)
                    {
                        if (withAnnotations)
                            sb.Append(GenerateAnnotations(property, true));

                        sb.Append(GenerateProperty(property, true, withAnnotations));
                    }
                    else
                    {
                        if (withAnnotations)
                            sb.Append(GenerateAnnotations(property, false));

                        sb.Append(GenerateProperty(property, false, withAnnotations));
                    }
                    i++;
                }
            }

            return sb.ToString();
        }

        private string GenerateProperty(IProperty property, bool firstLine, bool withAnnotations)
        {
            var sb = new StringBuilder();
            var propertyType = GetFriendlyTypeName(property.ClrType);

            if (firstLine)
                if (withAnnotations)
                    if (String.IsNullOrEmpty(GenerateAnnotations(property, true)))
                        sb.AppendLine($"public {propertyType} {property.Name} {{ get; init; }}");
                    else
                        sb.AppendLine($"        public {propertyType} {property.Name} {{ get; init; }}");
                else
                    sb.AppendLine($"public {propertyType} {property.Name} {{ get; init; }}");
            else
                sb.AppendLine($"        public {propertyType} {property.Name} {{ get; init; }}");

            return sb.ToString();
        }

        private static string GenerateAnnotations(IProperty property, bool firstLine)
        {
            var sb = new StringBuilder();
            var alreadyFoundOneAnnotation = false;

            // Check for required property
            if (!property.IsNullable)
            {
                if (!firstLine)
                    sb.Append($"        ");

                sb.AppendLine("[Required]");
                alreadyFoundOneAnnotation = true;
            }

            // Check for max length
            var maxLength = property.GetMaxLength();
            if (maxLength.HasValue)
            {
                if (!firstLine || alreadyFoundOneAnnotation)
                    sb.Append($"        ");

                sb.AppendLine($"[MaxLength({maxLength.Value})]");
                alreadyFoundOneAnnotation = true;
            }

            // Add other annotations as needed

            return sb.ToString();
        }

        private string GetFriendlyTypeName(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return $"{GetFriendlyTypeName(type.GetGenericArguments()[0])}?";
            }

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                var genericArguments = type.GetGenericArguments();
                var genericArgumentsString = string.Join(", ", genericArguments.Select(GetFriendlyTypeName));
                return $"{genericType.Name.Split('`')[0]}<{genericArgumentsString}>";
            }

            return type.Name;
        }

        public static string GetTemplateForContractCommandAdd()
        {
            return
                """
                using System.ComponentModel.DataAnnotations;

                namespace Ubik.Security.Contracts.{ClassName}s.Commands
                {
                    public record Add{ClassName}Command
                    {
                        {Properties}    }
                }
                """;
        }
    }
}
