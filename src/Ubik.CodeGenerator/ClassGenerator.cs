using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Ubik.CodeGenerator
{
    internal class ClassGenerator(Type dbContextType)
    {
        public void GenerateClassesContractAddCommand()
        {
            var entityTypes = dbContextType.GetProperties()
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.PropertyType.GetGenericArguments().First())
                .ToList();

            foreach (var entityType in entityTypes)
            {
                var className = entityType.Name;
                var excludedFiels = new List<string>()
                {
                    "CreatedAt",
                    "CreatedBy",
                    "ModifiedAt",
                    "ModifiedBy",
                    "Id"
                };
                var properties = GenerateProperties(entityType, true, excludedFiels);
                var classContent = GetTemplateForContractCommandAdd().Replace("{ClassName}", className)
                                                                    .Replace("{Properties}", properties);

                Console.WriteLine(classContent);
            }
        }

        private string GenerateProperties(Type entityType, bool withAnnotations, List<string>excludedFiedls)
        {
            var properties = entityType.GetProperties();
            var sb = new StringBuilder();

            var i = 1;
            foreach (var property in properties)
            {
                if (!excludedFiedls.Contains(property.Name))
                {
                    var propertyType = GetFriendlyTypeName(property.PropertyType);
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

        private string GenerateProperty(PropertyInfo property, bool firstLine, bool withAnnotations)
        {
            var sb = new StringBuilder();
            var propertyType = GetFriendlyTypeName(property.PropertyType);

            if (firstLine)
                if (withAnnotations)
                    if(String.IsNullOrEmpty(GenerateAnnotations(property, true)))
                        sb.AppendLine($"public {propertyType} {property.Name} {{ get; init; }}");
                    else
                        sb.AppendLine($"        public {propertyType} {property.Name} {{ get; init; }}");
                else
                    sb.AppendLine($"public {propertyType} {property.Name} {{ get; init; }}");
            else
                sb.AppendLine($"        public {propertyType} {property.Name} {{ get; init; }}");

            return sb.ToString();
        }

        private static string GenerateAnnotations(PropertyInfo property, bool firstLine)
        {
            var sb = new StringBuilder();
            var alreadyFoundOneAnnotation = false;

            // Check for [Required] attribute
            if (property.GetCustomAttribute(typeof(RequiredAttribute)) != null)
            {
                if (!firstLine)
                    sb.Append($"        ");

                sb.AppendLine("[Required]");
                alreadyFoundOneAnnotation = true;
            }

            // Check for [MaxLength] attribute
            var maxLengthAttr = property.GetCustomAttribute(typeof(MaxLengthAttribute));
            if (maxLengthAttr != null)
            {
                var length = ((MaxLengthAttribute)(maxLengthAttr)).Length;
                if (!firstLine || alreadyFoundOneAnnotation)
                    sb.Append($"        ");

                sb.AppendLine($"[MaxLength({length})]");
                alreadyFoundOneAnnotation = true;
            }

            var minLengthAttr = property.GetCustomAttribute(typeof(MinLengthAttribute));
            if (minLengthAttr != null)
            {
                var length = ((MinLengthAttribute)(minLengthAttr)).Length;
                if (!firstLine || alreadyFoundOneAnnotation)
                    sb.Append($"        ");

                sb.AppendLine($"[MinLength({length})]");
                alreadyFoundOneAnnotation = true;
            }

            // Check for [EmailAddress] attribute
            if (property.GetCustomAttribute(typeof(EmailAddressAttribute)) != null)
            {
                if (!firstLine || alreadyFoundOneAnnotation)
                    sb.Append($"        ");

                sb.AppendLine("[EmailAddress]");
                alreadyFoundOneAnnotation = true;
            }

            return sb.ToString();
        }

        private string GetFriendlyTypeName(Type type)
        {
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
