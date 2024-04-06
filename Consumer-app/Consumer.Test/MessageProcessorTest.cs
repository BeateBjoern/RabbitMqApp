using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Models;
using System;

namespace Services.Tests
{
    [TestClass]
    public class MessageProcessorTests
    {
        private MessageProcessor _messageProcessor;

        [TestInitialize]
        public void Setup()
        {
            _messageProcessor = new MessageProcessor();
        }

        [TestMethod]
        public void ProcessMessageForValidTimestamp_NullMessage_ReturnsNegativeOne()
        {
            // Arrange
            Message nullMessage = null;

            // Act
            var result = _messageProcessor.ProcessMessageForValidTimestamp(nullMessage);

            // Assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ProcessMessageForValidTimestamp_MessageTooOld_ReturnsZero()
        {
            // Arrange
            var message = new Message { Value = "Test", Timestamp = DateTime.UtcNow.AddMinutes(-2) };

            // Act
            var result = _messageProcessor.ProcessMessageForValidTimestamp(message);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ProcessMessageForValidTimestamp_EvenSecond_ReturnsOne()
        {
            // Arrange
            var evenSecondTimeStamp = GetCurrentDateTimeWithEvenSecond();
            var message = new Message { Value = "Test", Timestamp = evenSecondTimeStamp};

            // Act
            var result = _messageProcessor.ProcessMessageForValidTimestamp(message);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ProcessMessageForValidTimestamp_OddSecond_ReturnsTwo()
        {
            // Arranges
            var oddSecondTimeStamp = GetCurrentDateTimeWithOddSecond();
            var message = new Message { Value = "Test", Timestamp = oddSecondTimeStamp };

            // Act
            var result = _messageProcessor.ProcessMessageForValidTimestamp(message);

            // Assert
            Assert.AreEqual(2, result);
        }
        
        
        // Helper method to get the current datetime with an odd second
        // Helper method to get the current datetime with an even second
        public DateTime GetCurrentDateTimeWithEvenSecond()
        {
            var now = DateTime.Now;
            // If the second component is already even, return the current datetime
            if (now.Second % 2 == 0)
            {
                return now;
            }

            return now.AddSeconds(1);

        }

        // Helper method to get the current datetime with an odd second
        public DateTime GetCurrentDateTimeWithOddSecond()
        {
            var now = DateTime.Now;
            // If the second component is already even, return the current datetime
            if (now.Second % 2 != 0)
            {
                return now;
            }

            return now.AddSeconds(1);

        }     
    }
}