using Ubik.Accounting.Api.Data;
using Ubik.Security.Api.Data;

namespace Ubik.CodeGenerator
{
    internal class ServicesGenerator(AccountingDbContext dbContext)
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
                public interface I{ClassName}QueryService
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
                public class {ClassName}QueryService(SecurityDbContext ctx) : I{ClassName}QueryService
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
                public interface I{ClassName}CommandService
                {
                    public Task<Either<IServiceAndFeatureError, {ClassName}>> AddAsync(Add{ClassName}Command command);
                    public Task<Either<IServiceAndFeatureError, {ClassName}>> UpdateAsync(Update{ClassName}Command command);
                    public Task<Either<IServiceAndFeatureError, bool>> DeleteAsync(Guid id);
                }
                """;
        }

        private static string GetTemplateForServiceCommandClass()
        {
            return
                """"
                    public async Task<Either<IServiceAndFeatureError, {ClassName}>> AddAsync(Add{ClassName}Command command)
                    {
                        return await ValidateIfNotAlreadyExistsAsync(command.To{ClassName}())
                            .BindAsync(AddInDbContextAsync)
                            .BindAsync(AddSaveAndPublishAsync);
                    }

                    public async Task<Either<IServiceAndFeatureError, {ClassName}>> UpdateAsync(Update{ClassName}Command command)
                    {
                        var model = command.To{ClassName}();

                        return await GetAsync(model.Id)
                            .BindAsync(x => MapInDbContextAsync(x, model))
                            .BindAsync(ValidateIfNotAlreadyExistsWithOtherIdAsync)
                            .BindAsync(UpdateInDbContextAsync)
                            .BindAsync(UpdateSaveAndPublishAsync);
                    }

                    public async Task<Either<IServiceAndFeatureError, bool>> DeleteAsync(Guid id)
                    {
                        return await GetAsync(id)
                            .BindAsync(DeleteInDbContextAsync)
                            .BindAsync(DeletedSaveAndPublishAsync);
                    }

                    private async Task<Either<IServiceAndFeatureError, bool>> DeletedSaveAndPublishAsync({ClassName} current)
                    {
                        await publishEndpoint.Publish(new {ClassName}Deleted { Id = current.Id }, CancellationToken.None);
                        await ctx.SaveChangesAsync();

                        return true;
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> DeleteInDbContextAsync({ClassName} current)
                    {
                        ctx.Entry(current).State = EntityState.Deleted;

                        await Task.CompletedTask;
                        return current;
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> UpdateSaveAndPublishAsync({ClassName} current)
                    {
                        try
                        {
                            await publishEndpoint.Publish(current.To{ClassName}Updated(), CancellationToken.None);
                            await ctx.SaveChangesAsync();

                            return current;
                        }
                        catch (UpdateDbConcurrencyException)
                        {
                            return new ResourceUpdateConcurrencyError("{ClassName}", current.Version.ToString());
                        }
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> GetAsync(Guid id)
                    {
                        var result = await ctx.{ClassName}s.FindAsync(id);

                        return result == null
                            ? new ResourceNotFoundError("{ClassName}", "Id", id.ToString())
                            : result;
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> UpdateInDbContextAsync({ClassName} current)
                    {
                        ctx.Entry(current).State = EntityState.Modified;
                        ctx.SetAuditAndSpecialFields();

                        await Task.CompletedTask;
                        return current;
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> ValidateIfNotAlreadyExistsWithOtherIdAsync({ClassName} current)
                    {
                        var exists = await ctx.{ClassName}s.AnyAsync(a => a.Code == current.Code && a.Id != current.Id);

                        return exists
                            ? new ResourceAlreadyExistsError("{ClassName}", "Code", current.Code)
                            : current;
                    }

                    private static async Task<Either<IServiceAndFeatureError, {ClassName}>> MapInDbContextAsync
                        ({ClassName} current, {ClassName} forUpdate)
                    {
                        current = forUpdate.To{ClassName}(current);
                        await Task.CompletedTask;
                        return current;
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> AddSaveAndPublishAsync({ClassName} current)
                    {
                        await publishEndpoint.Publish(current.To{ClassName}Added(), CancellationToken.None);
                        await ctx.SaveChangesAsync();
                        return current;
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> AddInDbContextAsync({ClassName} current)
                    {
                        current.Id = NewId.NextGuid();
                        await ctx.{ClassName}s.AddAsync(current);
                        ctx.SetAuditAndSpecialFields();
                        return current;
                    }

                    private async Task<Either<IServiceAndFeatureError, {ClassName}>> ValidateIfNotAlreadyExistsAsync({ClassName} current)
                    {
                        var exists = await ctx.{ClassName}s.AnyAsync(a => a.Code == current.Code);
                        return exists
                            ? new ResourceAlreadyExistsError("{ClassName}", "Code", current.Code)
                            : current;
                    }
                """";
        }
    }
}
