﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Ubik.Security.Api.Data;
using System.Diagnostics.Eventing.Reader;

namespace Ubik.CodeGenerator
{
    internal class ContractsGenerator(SecurityDbContext dbContext)
    {
        public void GenerateContractAddResult(bool writeFiles, string? folderPath)
        {
            var entityTypes = dbContext.Model.GetEntityTypes().Where(e => e.ClrType.Name != "InboxState"
                                                                                  && e.ClrType.Name != "OutboxMessage"
                                                                                  && e.ClrType.Name != "OutboxState");

            foreach (var entityType in entityTypes)
            {
                var className = entityType.ClrType.Name;
                var excludedFiels = new List<string>()
                {
                    "CreatedAt",
                    "CreatedBy",
                    "ModifiedAt",
                    "ModifiedBy",
                    "IsOnlyForMegaAdmin",
                    "TenantId"
                };

                var properties = GenerateProperties(entityType, false, excludedFiels);
                var classContent = GetTemplateForContractResultAdd().Replace("{ClassName}", className)
                                                                    .Replace("{Properties}", properties);

                if (writeFiles)
                {
                    var filePath = $"{folderPath}/{className}s/Results/Add{className}Result.cs";
                    WriteClassToFile(filePath, classContent);
                }
                else
                    Console.WriteLine(classContent);
            }
        }
        public void GenerateContractAddedEvent(bool writeFiles, string? folderPath)
        {
            var entityTypes = dbContext.Model.GetEntityTypes().Where(e => e.ClrType.Name != "InboxState"
                                                                                  && e.ClrType.Name != "OutboxMessage"
                                                                                  && e.ClrType.Name != "OutboxState");

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

                var properties = GenerateProperties(entityType, false, excludedFiels);
                var classContent = GetTemplateForContractEventAdded().Replace("{ClassName}", className)
                                                                    .Replace("{Properties}", properties);

                if (writeFiles)
                {
                    var filePath = $"{folderPath}/{className}s/Events/{className}Added.cs";
                    WriteClassToFile(filePath, classContent);
                }
                else
                    Console.WriteLine(classContent);
            }
        }

        public void GenerateContractAddCommand(bool writeFiles, string? folderPath)
        {
            var entityTypes = dbContext.Model.GetEntityTypes().Where(e => e.ClrType.Name != "InboxState"
                                                                      && e.ClrType.Name != "OutboxMessage"
                                                                      && e.ClrType.Name != "OutboxState");

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
                    "Version",
                    "TenantId",
                    "IsOnlyForMegaAdmin"
                };

                var properties = GenerateProperties(entityType, true, excludedFiels);
                var classContent = GetTemplateForContractCommandAdd().Replace("{ClassName}", className)
                                                                    .Replace("{Properties}", properties);

                if (writeFiles)
                {
                    var filePath = $"{folderPath}/{className}s/Commands/Add{className}Command.cs";
                    WriteClassToFile(filePath, classContent);
                }
                else
                    Console.WriteLine(classContent);
            }
        }

        private static void WriteClassToFile(string? filePath, string content)
        {
            if (filePath == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error, cannot detect file.");
                Console.ResetColor();
            }
            else
            {
                string directoryPath = Path.GetDirectoryName(filePath)!;
                string fileName = Path.GetFileName(filePath);

                if (!Directory.Exists(directoryPath))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Directory.CreateDirectory(directoryPath);
                    Console.WriteLine($"Folder'{directoryPath}' has been created.");
                    Console.ResetColor();
                }

                if (File.Exists(filePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"The file '{fileName}' already exists. Cannot write a new one.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    File.WriteAllText(filePath, content);
                    Console.WriteLine(($"Success : file '{filePath}' created."));
                    Console.ResetColor();
                }
            }
        }

        private static string GenerateProperties(IEntityType entityType, bool withAnnotations, List<string> excludedFiedls)
        {
            var properties = entityType.GetProperties();
            var sb = new StringBuilder();

            var i = 1;
            foreach (var property in properties.OrderBy(x => x.PropertyInfo?.MetadataToken))
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

        private static string GenerateProperty(IProperty property, bool firstLine, bool withAnnotations)
        {
            var sb = new StringBuilder();
            var propertyType = GetFriendlyTypeName(property, property.ClrType);

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
            }

            // Add other annotations as needed

            return sb.ToString();
        }

        private static string GetFriendlyTypeName(IProperty property, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return $"{GetFriendlyTypeName(property, type.GetGenericArguments()[0])}?";
            }

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                var genericArguments = type.GetGenericArguments();
                var genericArgumentsString = string.Join(", ", genericArguments.Select(t => GetFriendlyTypeName(property, t)));
                return $"{genericType.Name.Split('`')[0]}<{genericArgumentsString}>";
            }

            var tmpType = string.Empty;

            tmpType = property.IsNullable && !type.IsValueType ? $"{type.Name}?" : type.Name;

            var convertedType = tmpType switch
            {
                "Boolean" => "bool",
                "String" => "string",
                "String?" => "string?",
                "Byte" => "byte",
                "SByte" => "sbyte",
                _ => string.Empty
            };

            return convertedType switch
            {
                "string" => $"required {convertedType}",
                _ => convertedType == string.Empty ? type.Name : convertedType,
            };
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

        public static string GetTemplateForContractEventAdded()
        {
            return
                """
                namespace Ubik.Security.Contracts.{ClassName}s.Events
                {
                    public record {ClassName}Added
                    {
                        {Properties}    }
                }
                """;
        }

        public static string GetTemplateForContractResultAdd()
        {
            return
                """
                namespace Ubik.Security.Contracts.{ClassName}s.Events
                {
                    public record Add{ClassName}Result
                    {
                        {Properties}    }
                }
                """;
        }
    }
}