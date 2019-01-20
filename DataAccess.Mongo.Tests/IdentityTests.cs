using Abstractions.DataSources;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Mongo.Tests
{
	[TestClass]
	public class IdentityTests
	{
		private static IIdentityDataSource _dataSource;

		[ClassInitialize]
		public static void Init(TestContext context)
		{
			var options = new MongoConnection.Options()
			{
				ConnectionString = "mongodb://localhost:27017",
				Database = "Messenger.Test"
			};
			var db = new MongoConnection(Options.Create(options));

			_dataSource = new IdentityDataSource(db);
		}

		[TestMethod]
		public async Task CreateReadDelete()
		{
			var id = Guid.NewGuid();
			var userId = Guid.NewGuid();
			var salt = Encoding.UTF8.GetBytes($"{DateTime.Now.Ticks}.Salt");
			var hash = Encoding.UTF8.GetBytes($"{DateTime.Now.Ticks}.Hash");

			var createdModel = await _dataSource.CreateAsync(id, userId, salt, hash);

			Assert.AreEqual(id, createdModel.Id);
			Assert.AreEqual(userId, createdModel.UserId);
			CollectionAssert.AreEqual(salt, createdModel.Salt);
			CollectionAssert.AreEqual(hash, createdModel.Hash);

			var readModel = await _dataSource.ReadAsync(createdModel.Id);

			Assert.AreEqual(createdModel, readModel);

			var deletedModel = await _dataSource.DeleteAsync(createdModel.Id);

			Assert.AreEqual(createdModel, deletedModel);
		}
	}
}
