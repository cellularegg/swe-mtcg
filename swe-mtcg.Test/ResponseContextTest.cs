using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace swe_mtcg.Test
{
    public class ResponseContextTest
    {
        [Test]
        public void TestRequestContextGetRequestContextGETRequest()
        {
            // Arrange
            string request1Path = "/messages";
            RequestMethod request1Verb = RequestMethod.GET;
            string request1Host = "localhost:8000";
            string request1HeaderUsrAgent = "curl/7.55.1";
            string request1HeaderAccept = "*/*";
            string request1String = $"{request1Verb.ToString()} {request1Path} HTTP/1.1{Environment.NewLine}" +
                $"Host: {request1Host}{Environment.NewLine}" +
                $"User-Agent: {request1HeaderUsrAgent}{Environment.NewLine}" +
                $"Accept: {request1HeaderAccept}{Environment.NewLine}{Environment.NewLine}";
            RequestContext request1 = RequestContext.GetRequestContext(request1String);

            // Act
            // TODO Maybe mock Message collection? to only test 
            ResponseContext response1 = ResponseContext.GetResponseContext(request1);

            ResponseContext response2BadRequest = ResponseContext.GetResponseContext(null);

            // Assert

            Assert.AreEqual("400 Bad Request", response2BadRequest.Status);

        }
    }
}
