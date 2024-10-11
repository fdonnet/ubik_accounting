using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;
using Ubik.Security.Api.Data;

namespace Ubik.CodeGenerator
{
    internal class MappersGenerator(SecurityDbContext dbContext)
    {
        public void GenerateMappers(string? type = null)
        {
            var entityTypes = dbContext.Model.GetEntityTypes().Where(e => e.ClrType.Name != "InboxState"
                                                                                  && e.ClrType.Name != "OutboxMessage"
                                                                                  && e.ClrType.Name != "OutboxState");
            if (type != null)
                entityTypes = entityTypes.Where(e => e.ClrType.Name == type);

            foreach (var entityType in entityTypes)
            {
                var className = entityType.ClrType.Name;
                var excludedFiels = new List<string>()
                {
                    "CreatedAt",
                    "CreatedBy",
                    "ModifiedAt",
                    "ModifiedBy",
                    "TenantId"
                };

                var excludedFielsForAdd = new List<string>()
                {
                    "CreatedAt",
                    "CreatedBy",
                    "ModifiedAt",
                    "ModifiedBy",
                    "TenantId",
                    "Id",
                    "Version"
                };

                var SetForCollection = GenerateSet(entityType, excludedFiels, "SetCollection");
                var SetForStandard = GenerateSet(entityType, excludedFiels, "SetStandard");
                var SetForStandardAdd = GenerateSet(entityType, excludedFielsForAdd, "SetStandardAdd");
                var SetStandardForUpdateCopy = GenerateSet(entityType, excludedFiels, "SetStandardUpdateCopy");

                var classContent = GetTemplateForMappers().Replace("{ClassName}", className)
                                                                    .Replace("{SetCollection}", SetForCollection)
                                                                    .Replace("{SetStandard}", SetForStandard)
                                                                    .Replace("{SetStandardAdd}", SetForStandardAdd)
                                                                    .Replace("{SetStandardUpdateCopy}", SetStandardForUpdateCopy);

                Console.WriteLine(classContent);
            }
        }

        private static string GenerateSet(IEntityType entityType, List<string> excludedFiedls, string SetType)
        {
            var properties = entityType.GetProperties();
            var sb = new StringBuilder();

            var i = 1;
            foreach (var property in properties.OrderBy(x => x.PropertyInfo?.MetadataToken))
            {
                if (!excludedFiedls.Contains(property.Name))
                {

                    switch (SetType)
                    {
                        case "SetCollection":
                            sb.Append(GenerateSetForCollection(property, i == 1 ? true : false));
                            break;
                        case "SetStandardAdd":
                            sb.Append(GenerateSetForStandardAdd(property, i == 1 ? true : false));
                            break;
                        case "SetStandard":
                            sb.Append(GenerateSetForStandard(property, i == 1 ? true : false));
                            break;
                        case "SetStandardUpdateCopy":
                            sb.Append(GenerateSetStandardForUpdateCopy(property, i == 1 ? true : false));
                            break;
                    }

                    i++;
                }
            }

            return sb.ToString();
        }

        private static string GenerateSetForCollection(IProperty property, bool firstLine)
        {
            var sb = new StringBuilder();

            if (firstLine)
                sb.AppendLine($"{property.Name} = x.{property.Name},");
            else
                sb.AppendLine($"            {property.Name} = x.{property.Name},");

            return sb.ToString();
        }

        private static string GenerateSetForStandardAdd(IProperty property, bool firstLine)
        {
            var sb = new StringBuilder();

            if (firstLine)
                sb.AppendLine($"{property.Name} = current.{property.Name},");
            else
                sb.AppendLine($"            {property.Name} = current.{property.Name},");

            return sb.ToString();
        }

        private static string GenerateSetForStandard(IProperty property, bool firstLine)
        {
            var sb = new StringBuilder();

            if (firstLine)
                sb.AppendLine($"{property.Name} = current.{property.Name},");
            else
                sb.AppendLine($"            {property.Name} = current.{property.Name},");

            return sb.ToString();
        }

        private static string GenerateSetStandardForUpdateCopy(IProperty property, bool firstLine)
        {
            var sb = new StringBuilder();

            if (firstLine)
                sb.AppendLine($"model.{property.Name} = forUpd.{property.Name};");
            else
                sb.AppendLine($"            model.{property.Name} = forUpd.{property.Name};");

            return sb.ToString();
        }

        private static string GetTemplateForMappers()
        {
            return
                """
                public static class {ClassName}Mappers
                {
                    public static IEnumerable<{ClassName}StandardResult> To{ClassName}StandardResults(this IEnumerable<{ClassName}> current)
                    {
                        return current.Select(x => new {ClassName}StandardResult()
                        {
                            {SetCollection}        });
                    }

                    public static {ClassName} To{ClassName}(this Add{ClassName}Command current)
                    {
                        return new {ClassName}
                        {
                            {SetStandardAdd}       };
                    }

                    public static {ClassName} To{ClassName}(this Update{ClassName}Command current)
                    {
                        return new {ClassName}
                        {
                            {SetStandard}       };
                    }

                    public static {ClassName}Added To{ClassName}Added(this {ClassName} current)
                    {
                        return new {ClassName}Added()
                        {
                            {SetStandard}       };
                    }

                    public static {ClassName}Updated To{ClassName}Updated(this {ClassName} current)
                    {
                        return new {ClassName}Updated()
                        {
                            {SetStandard}       };
                    }

                    public static {ClassName} To{ClassName}(this {ClassName} forUpd, {ClassName} model)
                    {
                            {SetStandardUpdateCopy}
                            return model;
                    }

                    public static {ClassName}StandardResult To{ClassName}StandardResult(this {ClassName} current)
                    {
                        return new {ClassName}StandardResult()
                        {
                            {SetStandard}       };
                    }
                }
                """;
        }
    }
}
