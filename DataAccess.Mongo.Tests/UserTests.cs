using Abstractions.DataSources;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace DataAccess.Mongo.Tests
{
	[TestClass]
	public class UserTests
	{
		private static IUserDataSource _dataSource;

		[ClassInitialize]
		public static void Init(TestContext context)
		{
			var options = new MongoConnection.Options()
			{
				ConnectionString = "mongodb://localhost:27017",
				Database = "Messenger.Test"
			};
			var db = new MongoConnection(Options.Create(options));

			_dataSource = new UserDataSource(db);
		}

		[TestMethod]
		public async Task CreateReadDelete()
		{
			var name = $"{DateTime.Now.Ticks}";

			var createdModel = await _dataSource.CreateAsync(name);

			Assert.AreEqual(name, createdModel.Name);

			var readModel = await _dataSource.ReadAsync(createdModel.Id);

			Assert.AreEqual(createdModel, readModel);

			var deletedModel = await _dataSource.DeleteAsync(createdModel.Id);

			Assert.AreEqual(createdModel, deletedModel);
		}

		[TestMethod]
		public async Task Multiple()
		{
			var name1 = $"{DateTime.Now.Ticks}.1";
			var name2 = $"{DateTime.Now.Ticks}.2";

			var createdModel1 = await _dataSource.CreateAsync(name1);
			var createdModel2 = await _dataSource.CreateAsync(name2);

			Assert.AreEqual(name1, createdModel1.Name);
			Assert.AreEqual(name2, createdModel2.Name);
			Assert.AreNotEqual(createdModel1.Id, createdModel2.Id);

			var count = await _dataSource.CountAsync();

			Assert.AreEqual(2, count);

			await _dataSource.DeleteAsync(createdModel1.Id);
			await _dataSource.DeleteAsync(createdModel2.Id);
		}
	}
}
