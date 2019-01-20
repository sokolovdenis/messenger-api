using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAccess.Mongo.Tests
{
	[TestClass]
	public class AssemblyInitialization
	{
		[AssemblyInitialize]
		public static void AssemblyInit(TestContext context)
		{
			TestBase.Init();
		}

		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
			TestBase.Cleanup();
		}
	}
}
