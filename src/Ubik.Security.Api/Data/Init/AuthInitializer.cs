using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Features.Users.Services;
using Ubik.Security.Contracts.Users.Commands;

namespace Ubik.Security.Api.Data.Init
{
    internal static class AuthInitializer
    {
        internal static async Task InitializeAsync(IUserAuthProviderService authProviderService)
        {
            var check = (await authProviderService.CheckIfUsersPresentInAuth()).MatchAsync<Either<IFeatureError, bool>>(
                RightAsync: async ok =>
                {
                    if (!ok)
                    {
                        foreach (var user in _baseUsers)
                        {
                            var result = await authProviderService.AddUserAsync(user);
                            if (result.IsLeft)
                                throw new Exception("Cannot manage auth users init.");
                        }
                    }
                    return true;
                },
                Left: err => throw new Exception("Cannot manage auth users init.")
                );
        }


        private static readonly List<AddUserCommand> _baseUsers =
        [
            new AddUserCommand()
            {
                Email="admin@test.com",
                Password="test",
                Firstname="Mega",
                Lastname="Admin"
            },
            new AddUserCommand()
            {
                Email="testrw@test.com",
                Password="test",
                Firstname="Testrw",
                Lastname="Test"
            },
             new AddUserCommand()
            {
                Email="testro@test.com",
                Password="test",
                Firstname="Testro",
                Lastname="Test"
            },
             new AddUserCommand()
            {
                Email="testnorole@test.com",
                Password="test",
                Firstname="Testnorole",
                Lastname="Test"
            }
             ,
             new AddUserCommand()
            {
                Email="testothertenant@test.com",
                Password="test",
                Firstname="Testothertenant",
                Lastname="Test"
            }
        ];
    }
}
