using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Mongo.Tests
{
	[TestClass]
	public class IdentityTests : TestBase
	{
		[TestMethod]
		public async Task CreateReadDelete()
		{
			var id = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var salt = Encoding.UTF8.GetBytes($"{DateTime.Now.Ticks}.Salt");
			var hash = Encoding.UTF8.GetBytes($"{DateTime.Now.Ticks}.Hash");

			var createdModel = await IdentityDataSource.CreateAsync(id, userId, salt, hash);

			Assert.AreEqual(id, createdModel.Id);
			Assert.AreEqual(userId, createdModel.UserId);
			CollectionAssert.AreEqual(salt, createdModel.Salt);
			CollectionAssert.AreEqual(hash, createdModel.Hash);

			var readModel = await IdentityDataSource.ReadAsync(createdModel.Id);

			Assert.AreEqual(createdModel, readModel);

			var deletedModel = await IdentityDataSource.DeleteAsync(createdModel.Id);

			Assert.AreEqual(createdModel, deletedModel);
		}
	}
}
