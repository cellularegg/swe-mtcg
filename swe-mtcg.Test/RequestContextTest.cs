using NUnit.Framework;
using System;

namespace swe_mtcg.Test
{
    public class RequestContextTest
    {

        [Test]
        public void TestRequestContextGetRequestContextGetRequest()
        {
            // Arrange
            string request1Path = "/messages";
            RequestMethod request1Verb = RequestMethod.GET;
            string request1Host = "localhost:8000";
            string request1HeaderUsrAgent = "curl/7.55.1";
            string request1HeaderAccept = "*/*";
            string request1Valid = $"{request1Verb.ToString()} {request1Path} HTTP/1.1{Environment.NewLine}" +
                $"Host: {request1Host}{Environment.NewLine}" +
                $"User-Agent: {request1HeaderUsrAgent}{Environment.NewLine}" +
                $"Accept: {request1HeaderAccept}{Environment.NewLine}{Environment.NewLine}";
            string request2InvalidEmpty = string.Empty;
            string request3InvalidVerb = @"IVALVERB /messages HTTP/1.1
Host: localhost:8080
User-Agent: curl/7.55.1
Accept: */*
";
            string request4InvalidHeader = @"GET /messages HTTP/1.1
aaa:: localhost:8080
bbbbb:: curl/7.55.1
CCCCC:: */*
";
            // TODO Test Body and other VERBS
            // Act
            RequestContext actualContext1 = RequestContext.GetRequestContext(request1Valid);
            string actualContext1HeaderUsrAgent = actualContext1.Headers["User-Agent".ToLower()];
            string actualContext1HeaderAccept = actualContext1.Headers["Accept".ToLower()];
            RequestContext actualContext2 = RequestContext.GetRequestContext(request2InvalidEmpty);
            RequestContext actualContext3 = RequestContext.GetRequestContext(request3InvalidVerb);
            RequestContext actualContext4 = RequestContext.GetRequestContext(request4InvalidHeader);
            // Assert
            Assert.AreEqual(request1Verb, actualContext1.Method);
            Assert.AreEqual(request1Path, actualContext1.Path);
            Assert.AreEqual(request1Host, actualContext1.Host);
            Assert.AreEqual(string.Empty, actualContext1.Body);
            Assert.AreEqual(request1HeaderUsrAgent, actualContext1HeaderUsrAgent);
            Assert.AreEqual(request1HeaderAccept, actualContext1HeaderAccept);
            Assert.IsNull(actualContext2);
            Assert.IsNull(actualContext3);
            Assert.IsNull(actualContext4);

        }

        [Test]
        public void TestRequestContextGetRequestContextPostRequest()
        {
            // Arrange
            string request1Path = "/messages";
            RequestMethod request1Verb = RequestMethod.POST;
            string request1Host = "localhost:8000";
            string request1HeaderUsrAgent = "curl/7.55.1";
            string request1HeaderAccept = "*/*";
            string request1Body = $"Hey this {Environment.NewLine}is my Body{Environment.NewLine}";
            string request1Valid = $"{request1Verb.ToString()} {request1Path} HTTP/1.1{Environment.NewLine}" +
                $"Host: {request1Host}{Environment.NewLine}" +
                $"User-Agent: {request1HeaderUsrAgent}{Environment.NewLine}" +
                $"Accept: {request1HeaderAccept}{Environment.NewLine}{Environment.NewLine}" +
                $"{request1Body}";
            string request2Valid = $"{request1Verb.ToString()} {request1Path} HTTP/1.1{Environment.NewLine}" +
                            $"Host: {request1Host}{Environment.NewLine}" +
                            $"User-Agent: {request1HeaderUsrAgent}{Environment.NewLine}" +
                            $"Accept: {request1HeaderAccept}{Environment.NewLine}{Environment.NewLine}";
            // TODO Test Body and other VERBS
            // Act
            RequestContext actualContext1 = RequestContext.GetRequestContext(request1Valid);
            string actualContext1HeaderUsrAgent = actualContext1.Headers["User-Agent".ToLower()];
            string actualContext1HeaderAccept = actualContext1.Headers["Accept".ToLower()];
            RequestContext actualContext2 = RequestContext.GetRequestContext(request2Valid);
            string actualContext2HeaderUsrAgent = actualContext2.Headers["User-Agent".ToLower()];
            string actualContext2HeaderAccept = actualContext2.Headers["Accept".ToLower()];

            // Assert
            Assert.AreEqual(request1Verb, actualContext1.Method);
            Assert.AreEqual(request1Path, actualContext1.Path);
            Assert.AreEqual(request1Host, actualContext1.Host);
            Assert.AreEqual(request1Body, actualContext1.Body);
            Assert.AreEqual(request1HeaderUsrAgent, actualContext1HeaderUsrAgent);
            Assert.AreEqual(request1HeaderAccept, actualContext1HeaderAccept);
            Assert.AreEqual(request1Verb, actualContext2.Method);
            Assert.AreEqual(request1Path, actualContext2.Path);
            Assert.AreEqual(request1Host, actualContext2.Host);
            Assert.AreEqual(string.Empty, actualContext2.Body);
            Assert.AreEqual(request1HeaderUsrAgent, actualContext2HeaderUsrAgent);
            Assert.AreEqual(request1HeaderAccept, actualContext2HeaderAccept);
        }

    }
}
