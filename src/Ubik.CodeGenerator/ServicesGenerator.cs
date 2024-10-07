using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Security.Api.Data;

namespace Ubik.CodeGenerator
{
    internal class ServicesGenerator(SecurityDbContext dbContext)
    {
        public void  GenerateAllServicesAndInterfaces(string? type = null)
        {
            GenerateQueryServiceWithInterface(type);
            GenerateCommandServiceWithInterface(type);
        }

        public void GenerateQueryServiceWithInterface(string? type = null)
        {
            GenerateQueryServiceInterface(type);
            GenerateQueryService(type);
        }

        public void GenerateCommandServiceWithInterface(string? type = null)
        {
            GenerateCommandServiceInterface(type);
            GenerateCommandService(type);
        }

        public void GenerateCommandService(string? type = null)
        {
            var entityTypes = dbContext.Model.GetEntityTypes().Where(e => e.ClrType.Name != "InboxState"
                                                                                  && e.ClrType.Name != "OutboxMessage"
                                                                                  && e.ClrType.Name != "OutboxState");
            if (type != null)
                entityTypes = entityTypes.Where(e => e.ClrType.Name == type);

            foreach (var entityType in entityTypes)
            {
                var className = entityType.ClrType.Name;
                var classContent = GetTemplateForServiceCommandClass().Replace("{ClassName}", className);

                Console.WriteLine(classContent);
            }
        }

        public void GenerateCommandServiceInterface(string? type = null)
        {
            var entityTypes = dbContext.Model.GetEntityTypes().Where(e => e.ClrType.Name != "InboxState"
                                                                                  && e.ClrType.Name != "OutboxMessage"
                                                                                  && e.ClrType.Name != "OutboxState");
            if (type != null)
                entityTypes = entityTypes.Where(e => e.ClrType.Name == type);

            foreach (var entityType in entityTypes)
            {
                var className = entityType.ClrType.Name;
                var classContent = GetTemplateForServiceCommandInterface().Replace("{ClassName}", className);

                Console.WriteLine(classContent);
            }
        }


        public void GenerateQueryService(string? type = null)
        {
            var entityTypes = dbContext.Model.GetEntityTypes().Where(e => e.ClrType.Name != "InboxState"
                                                                                  && e.ClrType.Name != "OutboxMessage"
                                                                                  && e.ClrType.Name != "OutboxState");
            if (type != null)
                entityTypes = entityTypes.Where(e => e.ClrType.Name == type);

            foreach (var entityType in entityTypes)
            {
                var className = entityType.ClrType.Name;
                var classContent = GetTemplateForServiceQueryClass().Replace("{ClassName}", className);

                Console.WriteLine(classContent);
            }
        }

        public void GenerateQueryServiceInterface(string? type = null)
        {
            var entityTypes = dbContext.Model.GetEntityTypes().Where(e => e.ClrType.Name != "InboxState"
                                                                                  && e.ClrType.Name != "OutboxMessage"
                                                                                  && e.ClrType.Name != "OutboxState");
            if (type != null)
                entityTypes = entityTypes.Where(e => e.ClrType.Name == type);

            foreach (var entityType in entityTypes)
            {
                var className = entityType.ClrType.Name;
                var classContent = GetTemplateForServiceQueryInterface().Replace("{ClassName}", className);

                Console.WriteLine(classContent);
            }
        }

        private static string GetTemplateForServiceQueryInterface()
        {
            return
                """
                public interface I{ClassName}sQueriesService
                {
                    Task<Either<IServiceAndFeatureError, {ClassName}>> GetAsync(Guid id);
                    Task<IEnumerable<{ClassName}>> GetAllAsync();
                }
                """;
        }

        private static string GetTemplateForServiceQueryClass()
        {
            return
                """
                public class {ClassName}sQueriesService(SecurityDbContext ctx) : I{ClassName}sQueriesService
                {
                    public async Task<IEnumerable<{ClassName}>> GetAllAsync()
                    {
                        var result = await ctx.{ClassName}s.ToListAsync();

                        return result;
                    }

                    public async Task<Either<IServiceAndFeatureError, {ClassName}>> GetAsync(Guid id)
                    {
                        var result = await ctx.{ClassName}s.FindAsync(id);

                        return result == null
                            ? new ResourceNotFoundError("{ClassName}", "Id", id.ToString())
                            : result;
                    }
                }
                """;
        }

        private static string GetTemplateForServiceCommandInterface()
        {
            return
                """
                public interface I{ClassName}sCommandsService
                {
                    public Task<Either<IServiceAndFeatureError, {ClassName}>> AddAsync(Add{ClassName}Command command);
                    public Task<Either<IServiceAndFeatureError, {ClassName}>> UpdateAsync(Update{ClassName}Command command);
                    public Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id);
                }
                """;
        }

        private static string GetTemplateForServiceCommandClass()
        {
            return
                """
                public class {ClassName}sCommandsService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint) : I{ClassName}sCommandsService
                {
                    public async Task<Either<IServiceAndFeatureError, {ClassName}>> AddAsync(Add{ClassName}Command command)
                    {
                        var result = await Add{ClassName}Async(command.To{ClassName}());

                        return await result.MatchAsync<Either<IServiceAndFeatureError, {ClassName}>>(
                        RightAsync: async r =>
                        {
                            await publishEndpoint.Publish(r.To{ClassName}Added(), CancellationToken.None);
                            await ctx.SaveChangesAsync();
                            return r;
                        },
                        Left: err =>
                        {
                            return Prelude.Left(err);
                        });
                    }

                    public async Task<Either<IServiceAndFeatureError, {ClassName}>> UpdateAsync(Update{ClassName}Command command)
                    {
                        var result = await Update{ClassName}Async(command.To{ClassName}());

                        return await result.MatchAsync<Either<IServiceAndFeatureError, {ClassName}>>(
                            RightAsync: async r =>
                            {
                                try
                                {
                                    //Store and publish AccountGroupAdded event
                                    await publishEndpoint.Publish(r.To{ClassName}Updated(), CancellationToken.None);
                                    await ctx.SaveChangesAsync();
                                    return r;
                                }
                                catch (UpdateDbConcurrencyException)
                                {
                                    return new ResourceUpdateConcurrencyError("{ClassName}", r.Version.ToString());
                                }
                            },
                            Left: err =>
                            {
                                return Prelude.Left(err);
                            });
                    }

                    public async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id)
                    {
                        var res = await ExecuteDelete{ClassName}Async(id);

                        return await res.MatchAsync<Either<IServiceAndFeatureError, bool>>(
                            RightAsync: async r =>
                            {
                                await publishEndpoint.Publish(new {ClassName}Deleted { Id = id }, CancellationToken.None);
                                await ctx.SaveChangesAsync();
                                return true;
                            },
                            Left: err =>
                            {
                                return Prelude.Left(err);
                            });
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> GetAsync(Guid id)
                    {
                        var result = await ctx.{ClassName}s.FindAsync(id);

                        return result == null
                            ? new ResourceNotFoundError("{ClassName}", "Id", id.ToString())
                            : result;
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> Update{ClassName}Async({ClassName} current)
                    {
                        return await GetAsync(current.Id).ToAsync()
                           .Map(c => c = current.To{ClassName}(c))
                           .Bind(c => ValidateIfNotAlreadyExistsWithOtherIdAsync(c).ToAsync())
                           .Map(c =>
                           {
                               ctx.Entry(c).State = EntityState.Modified;
                               ctx.SetAuditAndSpecialFields();
                               return c;
                           });
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> Add{ClassName}Async({ClassName} current)
                    {
                        return await ValidateIfNotAlreadyExistsAsync(current).ToAsync()
                           .MapAsync(async ac =>
                           {
                               ac.Id = NewId.NextGuid();
                               await ctx.{ClassName}s.AddAsync(ac);
                               ctx.SetAuditAndSpecialFields();
                               return ac;
                           });
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> ValidateIfNotAlreadyExistsAsync({ClassName} current)
                    {
                        var exists = await ctx.{ClassName}s.AnyAsync(a => a.Code == current.Code);
                        return exists
                            ? new ResourceAlreadyExistsError("{ClassName}", "Code", current.Code)
                            : current;
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> ValidateIfNotAlreadyExistsWithOtherIdAsync({ClassName} current)
                    {
                        var exists = await ctx.{ClassName}s.AnyAsync(a => a.Code == current.Code && a.Id != current.Id);

                        return exists
                            ? new ResourceAlreadyExistsError("{ClassName}", "Code", current.Code)
                            : current;
                    }

                    private async Task<Either<IServiceAndFeatureError, bool>> ExecuteDelete{ClassName}Async(Guid id)
                    {
                        return await GetAsync(id).ToAsync()
                                .MapAsync(async ac =>
                                {
                                    await ctx.{ClassName}s.Where(x => x.Id == id).ExecuteDeleteAsync();
                                    return true;
                                });
                    }
                }
                """;
        }
    }
}
