using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace swe_mtcg.Test
{
    public class ServerDataTest
    {
        [Test]
        public void TestRegisterUser()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validJsonUser = "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";
            string invalidJsonUser = "{\"name\":\"test\", \"Password\":\"daniel\"}";
            string invalidJson = "{\"Username\":\"kienboec\", \"Password\":\"daniel";

            string duplicateUserName = "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";

            Assert.IsTrue(sd.RegisterUser(validJsonUser));
            Assert.IsFalse(sd.RegisterUser(invalidJsonUser));
            Assert.IsFalse(sd.RegisterUser(invalidJson));
            Assert.IsFalse(sd.RegisterUser(duplicateUserName));
            // TODO Check if user actually exists?
            sd.Reset();
        }

        [Test]
        public void TestGetToken()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validJsonUser1 = "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";
            string validJsonUser2 = "{\"Username\":\"admin\", \"Password\":\"pw\"}";
            string invalidJsonUser1 = "{\"Username\":\"admin\", \"Password\":\"wrongpw\"}";
            string invalidJson = "{\"Username\":\"admin\", \"Password\":\"pw";


            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            Assert.IsTrue(sd.RegisterUser(validJsonUser2));

            Assert.AreEqual("{\"token\":\"kienboec-mtcgToken\"}", sd.GetToken(validJsonUser1));
            Assert.AreEqual("{\"token\":\"admin-mtcgToken\"}", sd.GetToken(validJsonUser2));
            Assert.IsEmpty(sd.GetToken(invalidJsonUser1));
            Assert.IsEmpty(sd.GetToken(invalidJson));

            sd.Reset();
        }

        [Test]
        public void TestCreatePackage()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validJsonPackage =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";
            string invalidJsonDuplicateId = validJsonPackage;
            string validJsonPackageWithoutId =
                "[{\"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Name\":\"Ork\", \"Damage\": 45.0}, {\"Name\":\"FireSpell\",    \"Damage\": 25.0}]";

            string invalidJsonPackage = "{\"name\":\"test\", \"Password\":\"daniel\"}";
            string invalidJson = "{\"name\":\"test\", \"Password\":\"daniel";

            Assert.IsTrue(sd.AddPackage(validJsonPackage));
            Assert.IsTrue(sd.AddPackage(validJsonPackageWithoutId));
            Assert.IsFalse(sd.AddPackage(invalidJsonDuplicateId));
            Assert.IsFalse(sd.AddPackage(invalidJsonPackage));
            Assert.IsFalse(sd.AddPackage(invalidJson));
            // ToDo check if package actually exists?

            sd.Reset();
        }

        [Test]
        public void TestAcquirePackage()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validUserToken = "admin-mtcgToken";
            string invalidUserToken = "admin";
            string validJsonUser = "{\"Username\":\"admin\", \"Password\":\"geheim\"}";
            string validJsonPackage1 =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";
            string validJsonPackage2 =
                "[{\"Id\":\"644808c2-f87a-4600-b313-122b02322fd5\", \"Name\":\"WaterGoblin\", \"Damage\":  9.0}, {\"Id\":\"4a2757d6-b1c3-47ac-b9a3-91deab093531\", \"Name\":\"Dragon\", \"Damage\": 55.0}, {\"Id\":\"91a6471b-1426-43f6-ad65-6fc473e16f9f\", \"Name\":\"WaterSpell\", \"Damage\": 21.0}, {\"Id\":\"4ec8b269-0dfa-4f97-809a-2c63fe2a0025\", \"Name\":\"Ork\", \"Damage\": 55.0}, {\"Id\":\"f8043c23-1534-4487-b66b-238e0c3c39b5\", \"Name\":\"WaterSpell\",   \"Damage\": 23.0}]";
            string validJsonPackage3 =
                "[{\"Id\":\"b017ee50-1c14-44e2-bfd6-2c0c5653a37c\", \"Name\":\"WaterGoblin\", \"Damage\": 11.0}, {\"Id\":\"d04b736a-e874-4137-b191-638e0ff3b4e7\", \"Name\":\"Dragon\", \"Damage\": 70.0}, {\"Id\":\"88221cfe-1f84-41b9-8152-8e36c6a354de\", \"Name\":\"WaterSpell\", \"Damage\": 22.0}, {\"Id\":\"1d3f175b-c067-4359-989d-96562bfa382c\", \"Name\":\"Ork\", \"Damage\": 40.0}, {\"Id\":\"171f6076-4eb5-4a7d-b3f2-2d650cc3d237\", \"Name\":\"RegularSpell\", \"Damage\": 28.0}]";
            string validJsonPackage4 =
                "[{\"Id\":\"ed1dc1bc-f0aa-4a0c-8d43-1402189b33c8\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"65ff5f23-1e70-4b79-b3bd-f6eb679dd3b5\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"55ef46c4-016c-4168-bc43-6b9b1e86414f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"f3fad0f2-a1af-45df-b80d-2e48825773d9\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"8c20639d-6400-4534-bd0f-ae563f11f57a\", \"Name\":\"WaterSpell\",   \"Damage\": 25.0}]";
            string validJsonPackage5 =
                "[{\"Id\":\"d7d0cb94-2cbf-4f97-8ccf-9933dc5354b8\", \"Name\":\"WaterGoblin\", \"Damage\":  9.0}, {\"Id\":\"44c82fbc-ef6d-44ab-8c7a-9fb19a0e7c6e\", \"Name\":\"Dragon\", \"Damage\": 55.0}, {\"Id\":\"2c98cd06-518b-464c-b911-8d787216cddd\", \"Name\":\"WaterSpell\", \"Damage\": 21.0}, {\"Id\":\"951e886a-0fbf-425d-8df5-af2ee4830d85\", \"Name\":\"Ork\", \"Damage\": 55.0}, {\"Id\":\"dcd93250-25a7-4dca-85da-cad2789f7198\", \"Name\":\"FireSpell\",    \"Damage\": 23.0}]";
            // Try acquiring package when there is no user
            Assert.IsFalse(sd.AcquirePackage(validUserToken));
            Assert.IsTrue(sd.RegisterUser(validJsonUser));
            // Try acquiring package when there is no package
            Assert.IsFalse(sd.AcquirePackage(validUserToken));
            // Add 5 Packages
            Assert.IsTrue(sd.AddPackage(validJsonPackage1));
            Assert.IsTrue(sd.AddPackage(validJsonPackage2));
            Assert.IsTrue(sd.AddPackage(validJsonPackage3));
            Assert.IsTrue(sd.AddPackage(validJsonPackage4));
            Assert.IsTrue(sd.AddPackage(validJsonPackage5));

            // Maybe add GetCoins method to check current balance
            // Acquire Packages
            Assert.IsFalse(sd.AcquirePackage(invalidUserToken));
            Assert.IsTrue(sd.AcquirePackage(validUserToken));
            Assert.IsTrue(sd.AcquirePackage(validUserToken));
            Assert.IsTrue(sd.AcquirePackage(validUserToken));
            Assert.IsTrue(sd.AcquirePackage(validUserToken));
            // Not enough money
            Assert.IsFalse(sd.AcquirePackage(validUserToken));
            sd.Reset();
        }

        [Test]
        public void TestConfigureDeck()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validToken1 = "admin-mtcgToken";
            string validJsonUser1 = "{\"Username\":\"admin\", \"Password\":\"geheim\"}";
            string invalidToken = "aaa";
            string validJsonPackage1 =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";
            string validJsonPackage2 =
                "[{\"Id\":\"2272ba48-6662-404d-a9a1-41a9bed316d9\", \"Name\":\"WaterGoblin\", \"Damage\": 11.0}, {\"Id\":\"3871d45b-b630-4a0d-8bc6-a5fc56b6a043\", \"Name\":\"Dragon\", \"Damage\": 70.0}, {\"Id\":\"166c1fd5-4dcb-41a8-91cb-f45dcd57cef3\", \"Name\":\"Knight\", \"Damage\": 22.0}, {\"Id\":\"237dbaef-49e3-4c23-b64b-abf5c087b276\", \"Name\":\"WaterSpell\", \"Damage\": 40.0}, {\"Id\":\"27051a20-8580-43ff-a473-e986b52f297a\", \"Name\":\"FireElf\", \"Damage\": 28.0}]";
            string jsonDecksValid1 =
                "[\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\"]";
            string jsonDecksValid2 =
                "[\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"dfdd758f-649c-40f9-ba3a-8657f4b3439f\"]";

            // Cards do not exists
            string jsonDeckInvalid1 =
                "[\"b017ee50-1c14-44e2-bfd6-2c0c5653a37c\", \"d04b736a-e874-4137-b191-638e0ff3b4e7\", \"171f6076-4eb5-4a7d-b3f2-2d650cc3d237\", \"ed1dc1bc-f0aa-4a0c-8d43-1402189b33c8\"]";
            // Only 3 Cards - not allowed
            string jsonDeckInvalid2 =
                "[\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"e85e3976-7c86-4d06-9a80-641c2019a79f\"]";
            // Invalid Json format
            string jsonDeckInvalid3 =
                "[\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"e85e3976-7c86-4d06-9a80-641c2019a79f";

            // Register User
            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            Assert.AreEqual("admin", sd.GetLoginNameFromToken(validToken1));
            // Add package
            Assert.IsTrue(sd.AddPackage(validJsonPackage1));

            // Configure Deck with not owned Cards
            Assert.IsFalse(sd.ConfigureDeck(jsonDeckInvalid1, validToken1));
            Assert.IsFalse(sd.ConfigureDeck(jsonDeckInvalid1, invalidToken));

            // Acquire package
            Assert.IsTrue(sd.AcquirePackage(validToken1));

            // Configure Deck
            Assert.IsFalse(sd.ConfigureDeck(jsonDecksValid1, invalidToken));
            Assert.IsTrue(sd.ConfigureDeck(jsonDecksValid1, validToken1));

            Assert.IsTrue(sd.ConfigureDeck(jsonDecksValid2, validToken1));
            Assert.IsFalse(sd.ConfigureDeck(jsonDeckInvalid2, validToken1));
            Assert.IsFalse(sd.ConfigureDeck(jsonDeckInvalid3, validToken1));


            sd.Reset();
        }

        [Test]
        public void TestGetLoginNameFromToken()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validToken1 = "admin-mtcgToken";
            string validJsonUser1 = "{\"Username\":\"admin\", \"Password\":\"geheim\"}";

            string invalidToken1 = "david-mtcgToken";
            string invalidToken2 = "admin";
            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            Assert.AreEqual("admin", sd.GetLoginNameFromToken(validToken1));
            Assert.IsEmpty(sd.GetLoginNameFromToken(invalidToken1));
            Assert.IsEmpty(sd.GetLoginNameFromToken(invalidToken2));

            sd.Reset();
        }

        [Test]
        public void TestGetStackAsJson()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validToken1 = "admin-mtcgToken";
            string invalidToken = "invalidtoken";
            string validJsonUser1 = "{\"Username\":\"admin\", \"Password\":\"geheim\"}";
            string validJsonPackage1 =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";

            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            Assert.IsTrue(sd.AddPackage(validJsonPackage1));
            Assert.AreEqual("[]", sd.GetStack(validToken1, true).Trim().Replace(Environment.NewLine, ""));
            Assert.IsTrue(sd.AcquirePackage(validToken1));
            Assert.IsEmpty(sd.GetStack(invalidToken, true));

            string expectedJsonCard1 =
                "\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\",\"Name\":\"WaterGoblin\",\"Damage\":10.0"
                    .ToLower();
            string expectedJsonCard2 =
                "\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\",\"Name\":\"Dragon\",\"Damage\":50.0".ToLower();
            string expectedJsonCard3 =
                "\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\",\"Name\":\"WaterSpell\",\"Damage\":20.0".ToLower();
            string expectedJsonCard4 =
                "\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\",\"Name\":\"Ork\",\"Damage\":45.0".ToLower();
            string expectedJsonCard5 =
                "\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\",\"Name\":\"FireSpell\",\"Damage\":25.0".ToLower();

            // Remove whitespace
            string actualJsonStack = string.Concat(sd.GetStack(validToken1, true).ToLower()
                .Replace(Environment.NewLine, "").Where(c => !Char.IsWhiteSpace(c)));
            StringAssert.Contains(expectedJsonCard1, actualJsonStack);
            StringAssert.Contains(expectedJsonCard2, actualJsonStack);
            StringAssert.Contains(expectedJsonCard3, actualJsonStack);
            StringAssert.Contains(expectedJsonCard4, actualJsonStack);
            StringAssert.Contains(expectedJsonCard5, actualJsonStack);

            sd.Reset();
        }

        [Test]
        public void TestGetStackAsPlain()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validToken1 = "admin-mtcgToken";
            string invalidToken = "invalidtoken";
            string validJsonUser1 = "{\"Username\":\"admin\", \"Password\":\"geheim\"}";
            string validJsonPackage1 =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";

            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            Assert.IsTrue(sd.AddPackage(validJsonPackage1));
            Assert.AreEqual("", sd.GetStack(validToken1, false).Trim().Replace(Environment.NewLine, ""));
            Assert.IsTrue(sd.AcquirePackage(validToken1));
            Assert.IsEmpty(sd.GetStack(invalidToken, false));

            string expectedJsonCard1 =
                "Id:845f0dc7-37d0-426e-994e-43fc3ac83c08,Name:WaterGoblin,Damage:10".ToLower();
            string expectedJsonCard2 =
                "Id:99f8f8dc-e25e-4a95-aa2c-782823f36e2a,Name:Dragon,Damage:50".ToLower();
            string expectedJsonCard3 =
                "Id:e85e3976-7c86-4d06-9a80-641c2019a79f,Name:WaterSpell,Damage:20".ToLower();
            string expectedJsonCard4 =
                "Id:1cb6ab86-bdb2-47e5-b6e4-68c5ab389334,Name:Ork,Damage:45".ToLower();
            string expectedJsonCard5 =
                "Id:dfdd758f-649c-40f9-ba3a-8657f4b3439f,Name:FireSpell,Damage:25".ToLower();
            // Remove whitespace
            string actualJsonStack = string.Concat(sd.GetStack(validToken1, false).ToLower()
                .Replace(Environment.NewLine, "").Where(c => !Char.IsWhiteSpace(c)));
            StringAssert.Contains(expectedJsonCard1, actualJsonStack);
            StringAssert.Contains(expectedJsonCard2, actualJsonStack);
            StringAssert.Contains(expectedJsonCard3, actualJsonStack);
            StringAssert.Contains(expectedJsonCard4, actualJsonStack);
            StringAssert.Contains(expectedJsonCard5, actualJsonStack);

            sd.Reset();
        }

        [Test]
        public void TestGetDeckAsJson()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validToken1 = "admin-mtcgToken";
            string invalidToken = "invalidtoken";
            string validJsonUser1 = "{\"Username\":\"admin\", \"Password\":\"geheim\"}";
            string validJsonPackage1 =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";
            string jsonDecksValid1 =
                "[\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\"]";

            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            Assert.IsTrue(sd.AddPackage(validJsonPackage1));
            Assert.IsTrue(sd.AcquirePackage(validToken1));
            Assert.IsEmpty(sd.GetDeck(invalidToken, true));
            Assert.AreEqual("[]", sd.GetDeck(validToken1, true).Trim().Replace(Environment.NewLine, ""));
            Assert.IsTrue(sd.ConfigureDeck(jsonDecksValid1, validToken1));

            string expectedJsonCard1 =
                "\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\",\"Name\":\"WaterGoblin\",\"Damage\":10.0"
                    .ToLower();
            string expectedJsonCard2 =
                "\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\",\"Name\":\"Dragon\",\"Damage\":50.0".ToLower();
            string expectedJsonCard3 =
                "\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\",\"Name\":\"WaterSpell\",\"Damage\":20.0".ToLower();
            string expectedJsonCard4 =
                "\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\",\"Name\":\"Ork\",\"Damage\":45.0".ToLower();

            // Remove whitespace
            string actualJsonDeck = string.Concat(sd.GetDeck(validToken1, true).ToLower()
                .Replace(Environment.NewLine, "").Where(c => !Char.IsWhiteSpace(c)));
            StringAssert.Contains(expectedJsonCard1, actualJsonDeck);
            StringAssert.Contains(expectedJsonCard2, actualJsonDeck);
            StringAssert.Contains(expectedJsonCard3, actualJsonDeck);
            StringAssert.Contains(expectedJsonCard4, actualJsonDeck);
            sd.Reset();
        }

        [Test]
        public void TestGetDeckAsPlain()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validToken1 = "admin-mtcgToken";
            string invalidToken = "invalidtoken";
            string validJsonUser1 = "{\"Username\":\"admin\", \"Password\":\"geheim\"}";
            string validJsonPackage1 =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";
            string jsonDecksValid1 =
                "[\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\"]";

            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            Assert.IsTrue(sd.AddPackage(validJsonPackage1));
            Assert.IsTrue(sd.AcquirePackage(validToken1));
            Assert.IsEmpty(sd.GetDeck(invalidToken, false));
            Assert.AreEqual("", sd.GetDeck(validToken1, false).Replace(Environment.NewLine, ""));
            Assert.IsTrue(sd.ConfigureDeck(jsonDecksValid1, validToken1));

            string expectedJsonCard1 =
                "Id:845f0dc7-37d0-426e-994e-43fc3ac83c08,Name:WaterGoblin,Damage:10".ToLower();
            string expectedJsonCard2 =
                "Id:99f8f8dc-e25e-4a95-aa2c-782823f36e2a,Name:Dragon,Damage:50".ToLower();
            string expectedJsonCard3 =
                "Id:e85e3976-7c86-4d06-9a80-641c2019a79f,Name:WaterSpell,Damage:20".ToLower();
            string expectedJsonCard4 =
                "Id:1cb6ab86-bdb2-47e5-b6e4-68c5ab389334,Name:Ork,Damage:45".ToLower();

            string actualJsonDeck = string.Concat(sd.GetDeck(validToken1, false).ToLower()
                .Replace(Environment.NewLine, "").Where(c => !Char.IsWhiteSpace(c)));
            StringAssert.Contains(expectedJsonCard1, actualJsonDeck);
            StringAssert.Contains(expectedJsonCard2, actualJsonDeck);
            StringAssert.Contains(expectedJsonCard3, actualJsonDeck);
            StringAssert.Contains(expectedJsonCard4, actualJsonDeck);
            sd.Reset();
        }

        [Test]
        public void TestGetUserData()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validJsonUser = "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";

            Assert.IsTrue(sd.RegisterUser(validJsonUser));
            string username = "kienboec";
            string token = "kienboec-mtcgToken";
            string json = sd.GetUserData(username, token);
            json = string.Concat(json.ToLower().Replace(Environment.NewLine, "").Where(c => !Char.IsWhiteSpace(c)));
            StringAssert.Contains("\"loginname\":\"kienboec\"", json);
            StringAssert.Contains("\"coins\":20", json);
            StringAssert.Contains("\"name\":\"kienboec\"", json);
            StringAssert.Contains("\"bio\":\"\"", json);
            StringAssert.Contains("\"wins\":0", json);
            StringAssert.Contains("\"looses\":0", json);
            StringAssert.Contains("\"elo\":100", json);
            sd.Reset();
        }

        [Test]
        public void TestUpdateUserData()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validJsonUser = "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";

            Assert.IsTrue(sd.RegisterUser(validJsonUser));
            string token = "kienboec-mtcgToken";
            string username = "kienboec";
            string validUpdateUserJson = "{\"Name\": \"Hoax\",  \"Bio\": \"me playin...\", \"Status\": \":-)\"}";
            string invalidUpdateUserJson1 = "{\"username\": \"Hoax\",  \"Bio\": \"me playin...\", \"Status\": \":-)\"}";
            string invalidUpdateUserJson2 = "{Name\": \"Hoax\",  \"Bio\": \"me playin...\", \"Status\": \":-)\"}";
            string invalidToken = "admin";
            Assert.IsTrue(sd.UpdateUserData(token, validUpdateUserJson));
            Assert.IsFalse(sd.UpdateUserData(token, invalidUpdateUserJson1));
            Assert.IsFalse(sd.UpdateUserData(token, invalidUpdateUserJson2));
            Assert.IsFalse(sd.UpdateUserData(invalidToken, validJsonUser));
            string json = sd.GetUserData(username, token);
            json = string.Concat(json.ToLower().Replace(Environment.NewLine, "").Where(c => !Char.IsWhiteSpace(c)));
            StringAssert.Contains("\"name\":\"hoax\"", json);
            StringAssert.Contains("\"bio\":\"meplayin...\"", json);
            StringAssert.Contains("\"status\":\":-)\"", json);
            sd.Reset();
        }

        [Test]
        public void TestCreateTradingDeal()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validToken1 = "david-mtcgToken";
            string validJsonUser1 = "{\"Username\":\"david\", \"Password\":\"geheim\"}";
            string validJsonPackage1 =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";
            string validJsonTradingDeal1 =
                "{\"Id\": \"6cd85277-4590-49d4-b0cf-ba0a921faad0\", \"CardToTrade\": \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Type\": \"monster\", \"MinimumDamage\": 15}";
            string validJsonTradingDeal2 =
                "{\"CardToTrade\": \"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Type\": \"monster\", \"MinimumDamage\": 15}";

            string validJsonTradingDeal3 =
                "{\"Id\": \"6cd85277-4590-49d4-b0cf-ba0a921faad1\", \"CardToTrade\": \"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Type\": \"monster\", \"MinimumDamage\": 15, \"EloWanted\": 5}";

            // Duplicate Trading Id
            string invalidJsonTradingDeal1 =
                "{\"Id\": \"6cd85277-4590-49d4-b0cf-ba0a921faad0\", \"CardToTrade\": \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Type\": \"monster\", \"MinimumDamage\": 15}";
            // Invalid Json 
            string invalidJsonTradingDeal2 =
                "6cd85277-4590-49d4-b0cf-ba0a921faad0\", \"CardToTrade\": \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Type\": \"monster\", \"MinimumDamage\": 15}";

            // Register User
            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            // Add package
            Assert.IsTrue(sd.AddPackage(validJsonPackage1));
            Assert.IsTrue(sd.AcquirePackage(validToken1));
            Assert.IsTrue(sd.CreateTradingDeal(validToken1, validJsonTradingDeal1));
            Assert.IsTrue(sd.CreateTradingDeal(validToken1, validJsonTradingDeal2));
            Assert.IsTrue(sd.CreateTradingDeal(validToken1, validJsonTradingDeal3));
            // Player should not own card anymore
            Assert.IsFalse(sd.CreateTradingDeal(validToken1, validJsonTradingDeal1));
            Assert.IsFalse(sd.CreateTradingDeal(validToken1, invalidJsonTradingDeal1));
            Assert.IsFalse(sd.CreateTradingDeal(validToken1, invalidJsonTradingDeal2));
            // 
        }

        [Test]
        public void TestDeleteTradingDeal()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validToken1 = "david-mtcgToken";
            string invalidToken1 = "admin-mtcgToken";
            string validJsonUser1 = "{\"Username\":\"david\", \"Password\":\"geheim\"}";
            string validJsonPackage1 =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";
            string validJsonTradingDeal1 =
                "{\"Id\": \"6cd85277-4590-49d4-b0cf-ba0a921faad0\", \"CardToTrade\": \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Type\": \"monster\", \"MinimumDamage\": 15}";

            // Register User
            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            // Add package
            Assert.IsTrue(sd.AddPackage(validJsonPackage1));
            Assert.IsTrue(sd.AcquirePackage(validToken1));
            Assert.IsTrue(sd.CreateTradingDeal(validToken1, validJsonTradingDeal1));
            // Player should not own card anymore

            Assert.IsFalse(sd.DeleteTradingDeal(invalidToken1, "6cd85277-4590-49d4-b0cf-ba0a921faad0"));
            Assert.IsFalse(sd.DeleteTradingDeal(validToken1, "invalidId"));
            Assert.IsTrue(sd.DeleteTradingDeal(validToken1, "6cd85277-4590-49d4-b0cf-ba0a921faad0"));
        }

        [Test]
        public void TestTrade()
        {
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validToken1 = "david-mtcgToken";
            string validJsonUser1 = "{\"Username\":\"david\", \"Password\":\"geheim\"}";
            string validToken2 = "admin-mtcgToken";
            string validJsonUser2 = "{\"Username\":\"admin\", \"Password\":\"geheim\"}";
            string validJsonPackage1 =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";
            string validJsonPackage2 =
                "[{\"Id\":\"67f9048f-99b8-4ae4-b866-d8008d00c53d\", \"Name\":\"WaterGoblin\", \"Damage\": 100.0}, {\"Id\":\"aa9999a0-734c-49c6-8f4a-651864b14e62\", \"Name\":\"RegularSpell\", \"Damage\": 150.0}, {\"Id\":\"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\", \"Name\":\"Knight\", \"Damage\": 120.0}, {\"Id\":\"02a9c76e-b17d-427f-9240-2dd49b0d3bfd\", \"Name\":\"RegularSpell\", \"Damage\": 145.0}, {\"Id\":\"2508bf5c-20d7-43b4-8c77-bc677decadef\", \"Name\":\"FireElf\", \"Damage\": 125.0}]";
            string trade1Id = "6cd85277-4590-49d4-b0cf-ba0a921faad0";
            string validJsonTradingDeal1 =
                "{\"Id\": \"" + trade1Id +
                "\", \"CardToTrade\": \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Type\": \"monster\", \"MinimumDamage\": 15}";
            string trade2Id = "6cd85277-4590-49d4-b0cf-ba0a921faad1";
            string validJsonTradingDeal2 =
                "{\"Id\": \"" + trade2Id +
                "\", \"CardToTrade\": \"67f9048f-99b8-4ae4-b866-d8008d00c53d\", \"Type\": \"monster\", \"MinimumDamage\": 1000, \"EloWanted\": 5}";

            // Trade for card
            string validJsonTrade1 = "{\"CardToTrade\": \"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\"}";
            // Trade for elo
            string validJsonTrade2 = "{\"CardToTrade\": \"\"}";
            // Trade with yourself
            string invalidJsonTrade1 = "{\"CardToTrade\": \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\"}";
            // Card does not fulfill reqs
            string invalidJsonTrade2 = "{\"CardToTrade\": \"02a9c76e-b17d-427f-9240-2dd49b0d3bfd\"}";
            string invalidJsonTrade3 = "{\"CardToTrade\": \"invalidCardID\"}";

            // Register User
            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            Assert.IsTrue(sd.RegisterUser(validJsonUser2));
            // Add package
            Assert.IsTrue(sd.AddPackage(validJsonPackage1));
            Assert.IsTrue(sd.AcquirePackage(validToken1));
            Assert.IsTrue(sd.AddPackage(validJsonPackage2));
            Assert.IsTrue(sd.AcquirePackage(validToken2));

            // Register Trades
            Assert.IsTrue(sd.CreateTradingDeal(validToken1, validJsonTradingDeal1));
            Assert.IsTrue(sd.CreateTradingDeal(validToken2, validJsonTradingDeal2));

            // Trade with yourself
            Assert.IsFalse(sd.Trade(trade1Id, validToken1, invalidJsonTrade1));
            Assert.IsFalse(sd.Trade(trade1Id, validToken2, invalidJsonTrade2));
            Assert.IsFalse(sd.Trade(trade1Id, validToken2, invalidJsonTrade3));
            
            // Successful trades
            Assert.IsTrue(sd.Trade(trade1Id, validToken2, validJsonTrade1));
            Assert.IsTrue(sd.Trade(trade2Id, validToken1, validJsonTrade2));
        }

        [Test]
        public void TestQueueForBattle()
        {
            // Not really a Unit test more a manual check if everything works
            ServerData sd = ServerData.Instance;
            sd.Reset();
            string validToken1 = "david-mtcgToken";
            string validJsonUser1 = "{\"Username\":\"david\", \"Password\":\"geheim\"}";
            string validToken2 = "admin-mtcgToken";
            string validJsonUser2 = "{\"Username\":\"admin\", \"Password\":\"geheim\"}";
            string validJsonPackage1 =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";
            string validJsonPackage2 =
                "[{\"Id\":\"67f9048f-99b8-4ae4-b866-d8008d00c53d\", \"Name\":\"WaterGoblin\", \"Damage\": 100.0}, {\"Id\":\"aa9999a0-734c-49c6-8f4a-651864b14e62\", \"Name\":\"RegularSpell\", \"Damage\": 150.0}, {\"Id\":\"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\", \"Name\":\"Knight\", \"Damage\": 120.0}, {\"Id\":\"02a9c76e-b17d-427f-9240-2dd49b0d3bfd\", \"Name\":\"RegularSpell\", \"Damage\": 145.0}, {\"Id\":\"2508bf5c-20d7-43b4-8c77-bc677decadef\", \"Name\":\"FireElf\", \"Damage\": 125.0}]";
            string jsonDecksValid1 =
                "[\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\"]";
            string jsonDecksValid2 =
                "[\"67f9048f-99b8-4ae4-b866-d8008d00c53d\", \"aa9999a0-734c-49c6-8f4a-651864b14e62\", \"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\", \"2508bf5c-20d7-43b4-8c77-bc677decadef\"]";

            // Register User
            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            Assert.IsTrue(sd.RegisterUser(validJsonUser2));
            // Add package
            Assert.IsTrue(sd.AddPackage(validJsonPackage1));
            Assert.IsTrue(sd.AcquirePackage(validToken1));
            Assert.IsTrue(sd.AddPackage(validJsonPackage2));
            Assert.IsTrue(sd.AcquirePackage(validToken2));

            // Acquire package

            // Configure Deck
            Assert.IsTrue(sd.ConfigureDeck(jsonDecksValid1, validToken1));
            Assert.IsTrue(sd.ConfigureDeck(jsonDecksValid2, validToken2));


            var res1 = Task<string>.Run(() => sd.QueueForBattle(validToken1));
            var res2 = Task<string>.Run(() => sd.QueueForBattle(validToken2));

            Console.WriteLine(res1.Result);
            Console.WriteLine(res2.Result);

            sd.Reset();
        }
    }
}