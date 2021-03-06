﻿using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Runner.v2;
using Xunit.v3;
using v2Mocks = Xunit.Runner.v2.Mocks;

public class Xunit2MessageSinkAdapterTests
{
	[Fact]
	public void CallsAdapterWhenAdaptingMessage()
	{
		var resultMessage = new _MessageSinkMessage();
		var incomingMessage = default(IMessageSinkMessage);
		var incomingTypes = default(HashSet<string>);
		IMessageSinkMessage adapter(IMessageSinkMessage message, HashSet<string>? types)
		{
			incomingMessage = message;
			incomingTypes = types;
			return resultMessage;
		}

		var v2Message = v2Mocks.TestAssemblyStarting();
		var v3Messages = new List<IMessageSinkMessage>();
		var v3Sink = SpyMessageSink.Create(messages: v3Messages);
		var v2Sink = Xunit2MessageSinkAdapter.Adapt(v3Sink, adapter);

		v2Sink.OnMessage(v2Message);

		// Ensure we got passed the right message and types
		Assert.Same(v2Message, incomingMessage);
		Assert.NotNull(incomingTypes);
		Assert.Contains(typeof(ITestAssemblyStarting).FullName!, incomingTypes);

		// Ensure we passed back (just) the converted message
		var v3Message = Assert.Single(v3Messages);
		Assert.Same(resultMessage, v3Message);
	}
}
