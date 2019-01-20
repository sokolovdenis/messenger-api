using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace DataAccess.Mongo.Tests
{
	[TestClass]
	public class UserTests : TestBase
	{
		[TestMethod]
		public async Task CreateReadDelete()
		{
			var name = $"{DateTime.Now.Ticks}";

			var createdModel = await UserDataSource.CreateAsync(name);

			Assert.AreEqual(name, createdModel.Name);

			var readModel = await UserDataSource.ReadAsync(createdModel.Id);

			Assert.AreEqual(createdModel, readModel);

			var deletedModel = await UserDataSource.DeleteAsync(createdModel.Id);

			Assert.AreEqual(createdModel, deletedModel);
		}

		[TestMethod]
		public async Task Multiple()
		{
			var name1 = $"{DateTime.Now.Ticks}.1";
			var name2 = $"{DateTime.Now.Ticks}.2";

			var createdModel1 = await UserDataSource.CreateAsync(name1);
			var createdModel2 = await UserDataSource.CreateAsync(name2);

			Assert.AreEqual(name1, createdModel1.Name);
			Assert.AreEqual(name2, createdModel2.Name);
			Assert.AreNotEqual(createdModel1.Id, createdModel2.Id);

			var count = await UserDataSource.CountAsync();

			Assert.AreEqual(2, count);

			await UserDataSource.DeleteAsync(createdModel1.Id);
			await UserDataSource.DeleteAsync(createdModel2.Id);
		}
	}
}
