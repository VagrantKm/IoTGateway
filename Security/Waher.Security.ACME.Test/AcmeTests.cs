﻿using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Waher.Security.ACME.Test
{
	[TestClass]
	public class AcmeTests
	{
		private AcmeClient client;

		[TestInitialize]
		public void TestInitialize()
		{
			this.client = new AcmeClient("https://acme-staging-v02.api.letsencrypt.org/directory");
		}

		[TestCleanup]
		public void TestCleanup()
		{
			if (this.client != null)
			{
				this.client.Dispose();
				this.client = null;
			}
		}

		[TestMethod]
		public async Task ACME_Test_01_GetDirectory()
		{
			AcmeDirectory Directory = await this.client.GetDirectory();
			Assert.IsNotNull(Directory);
			Assert.IsTrue(Directory.CaaIdentities.Length > 0);
			Assert.IsFalse(Directory.ExternalAccountRequired);
			Assert.IsNotNull(Directory.KeyChange);
			Assert.IsNotNull(Directory.NewAccount);
			Assert.IsNull(Directory.NewAuthz);
			Assert.IsNotNull(Directory.NewNonce);
			Assert.IsNotNull(Directory.NewOrder);
			Assert.IsNotNull(Directory.RevokeCert);
			Assert.IsNotNull(Directory.TermsOfService);
			Assert.IsNotNull(Directory.Website);
		}

		[TestMethod]
		public async Task ACME_Test_02_CreateAccount()
		{
			AcmeAccount Account = await this.client.CreateAccount(new string[] { "mailto:unit.test@example.com" }, true);
			Assert.IsNotNull(Account);
			Assert.AreEqual(AcmeAccountStatus.valid, Account.Status);
			Assert.IsNotNull(Account.Contact);
			Assert.AreEqual(1, Account.Contact.Length);
			Assert.AreEqual("mailto:unit.test@example.com", Account.Contact[0]);
		}
	}
}
