using NUnit.Framework;

namespace swe_mtcg.Test
{
    public class ServerDataTest
    {
        [Test]
        public void TestRegisterUser()
        {
            string validJsonUser = "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";
            string invalidJsonUser = "{\"name\":\"test\", \"Password\":\"daniel\"}";
            string invalidJson = "{\"Username\":\"kienboec\", \"Password\":\"daniel";

            string duplicateUserName = "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";
            ServerData sd = ServerData.Instance;

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
            string validJsonUser1 = "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}";
            string validJsonUser2 = "{\"Username\":\"admin\", \"Password\":\"pw\"}";

            ServerData sd = ServerData.Instance;

            Assert.IsTrue(sd.RegisterUser(validJsonUser1));
            Assert.IsTrue(sd.RegisterUser(validJsonUser2));

            Assert.AreEqual("kienboec-mtcgToken", sd.GetToken(validJsonUser1));
            Assert.AreEqual("admin-mtcgToken", sd.GetToken(validJsonUser2));

            sd.Reset();
        }

        [Test]
        public void TestCreatePackage()
        {
            ServerData sd = ServerData.Instance;
            string validJsonPackage =
                "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]";
            string invalidJsonPackage = "{\"name\":\"test\", \"Password\":\"daniel\"}";
            string invalidJson = "{\"name\":\"test\", \"Password\":\"daniel";

            Assert.IsTrue(sd.AddPackage(validJsonPackage));
            Assert.IsFalse(sd.AddPackage(invalidJsonPackage));
            Assert.IsFalse(sd.AddPackage(invalidJson));
            // ToDo chekc if package actually exists?

            sd.Reset();
        }
    }
}