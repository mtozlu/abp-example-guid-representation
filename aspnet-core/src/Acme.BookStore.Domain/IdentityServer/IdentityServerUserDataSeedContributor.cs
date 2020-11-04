using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace Acme.BookStore.IdentityServer
{
	public class IdentityServerUserDataSeedContributor : IDataSeedContributor, ITransientDependency
	{
		private readonly IGuidGenerator GuidGenerator;
		private readonly IIdentityUserRepository UserRepository;
		private readonly ILookupNormalizer LookupNormalizer;
		private readonly IdentityUserManager UserManager;

		public IdentityServerUserDataSeedContributor(
			IGuidGenerator guidGenerator,
			IIdentityUserRepository userRepository,
			ILookupNormalizer lookupNormalizer,
			IdentityUserManager userManager)
		{
			GuidGenerator = guidGenerator;
			UserRepository = userRepository;
			LookupNormalizer = lookupNormalizer;
			UserManager = userManager;
		}

		[UnitOfWork]
		public virtual async Task SeedAsync(DataSeedContext context)
		{
			await CreateUserWithExtraProperty();
		}

		private async Task CreateUserWithExtraProperty()
		{
			//"test" user
			const string userName = "test";
			var user = await UserRepository.FindByNormalizedUserNameAsync(LookupNormalizer.NormalizeName(userName));

			if (user != null)
			{
				return;
			}

			user = new Volo.Abp.Identity.IdentityUser(
				GuidGenerator.Create(),
				userName,
				"test@abp.io"
			)
			{
				Name = userName
			};
			user.SetProperty("WarehouseId", GuidGenerator.Create());
			(await UserManager.CreateAsync(user, "1q2w3E*")).CheckErrors();
		}
	}
}
