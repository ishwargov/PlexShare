/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains all the tests written for the Serializer module
/// </summary>

using Networking.Queues;
using System.Collections.Generic;
using Xunit;

namespace Networking.Serialization.Tests
{
    public class SerializerTests
    {
        // Using the same Serializer object for all tests
        ISerializer serializer = new Serializer();

        [Fact]
        public void SerializeIntegerTest()
        {
            // Integer to be tested
            int value = 10;

            // Encoding the value
            string encodedString = serializer.Serialize(value);

            // Checking if the decoded value is same as the actual value
            int decodedValue = serializer.Deserialize<int>(encodedString);
            Assert.Equal(decodedValue, value);

            // Changing the value and making checks
            value += 2;
            Assert.NotEqual(decodedValue, value);
        }

        [Fact]
        public void SerializeDoubleTest()
        {
            // Double to be tested
            double value = 10.234;

            // Encoding the value
            string encodedString = serializer.Serialize(value);

            // Checking if the decoded value is same as the actual value
            double decodedValue = serializer.Deserialize<double>(encodedString);
            Assert.Equal(decodedValue, value);

            // Changing the value and making checks
            value -= 0.0001;
            Assert.NotEqual(decodedValue, value);
        }

        [Fact]
        public void SerializeStringTest()
        {
            // String to be tested
            string message = "Hello world!";

            // Encoding the message
            string encodedString = serializer.Serialize(message);

            // Checking if the decoded string is same as the actual message
            string decodedString = serializer.Deserialize<string>(encodedString);
            Assert.Equal(decodedString, message);

            // Referencing to another string and making checks
            message = "Hello world!\n";
            Assert.NotEqual(decodedString, message);
        }

        [Fact]
        public void SerializeIntegerArrayTest()
        {
            // Integer array to be tested
            int[] arr = new int[10];

            // Encoding the array
            string encodedString = serializer.Serialize(arr);

            // Checking if the decoded array is same as the actual array
            int[] decodedArray = serializer.Deserialize<int[]>(encodedString);
            Assert.Equal(decodedArray, arr);

            // Initializing the actual array
            for (int i = 0; i < 10; ++i)
                arr[i] = i;

            // Making the same checks to the initialized array
            encodedString = serializer.Serialize(arr);
            decodedArray = serializer.Deserialize<int[]>(encodedString);
            Assert.Equal(decodedArray, arr);

            // Changing an element from the array and making checks
            arr[arr.Length / 2] = -2;
            Assert.NotEqual(decodedArray, arr);
        }

        [Fact]
        public void SerializeCustomObjectTest()
        {
            // TestClass object to be tested
            TestClass testObject = new TestClass(200, "Test message!");

            // Encoding the object
            string encodedString = serializer.Serialize(testObject);

            // Checking if the decoded object is same as the actual object
            TestClass decodedObject = serializer.Deserialize<TestClass>(encodedString);
            Assert.Equal(testObject, decodedObject);

            // Changing the value of an instance variable and making checks
            testObject.value = -100;
            Assert.NotEqual(testObject, decodedObject);
        }

        [Fact]
        public void SerializeCustomObjectArrayTest()
        {
            // TestClass array to be tested
            TestClass[] testObjectArray = new TestClass[10];

            // Encoding the array
            string encodedString = serializer.Serialize(testObjectArray);

            // Checking if the decoded array is same as the actual array
            TestClass[] decodedArray = serializer.Deserialize<TestClass[]>(encodedString);
            Assert.Equal(testObjectArray, decodedArray);

            // Initializing the actual array
            for (int i = 0; i < 10; ++i)
                testObjectArray[i] = new TestClass(i, "Test message" + i);

            // Making the same checks to the initialized array
            encodedString = serializer.Serialize(testObjectArray);
            decodedArray = serializer.Deserialize<TestClass[]>(encodedString);
            Assert.Equal(decodedArray, testObjectArray);

            // Changing the value of an instance variable of an element in the array and making checks
            testObjectArray[testObjectArray.Length / 2].message = "Okay!";
            Assert.NotEqual(decodedArray, testObjectArray);
        }

        [Fact]
        public void SerializeCustomObjectListTest()
        {
            // TestClass list to be tested
            List<TestClass> testObjectList = new List<TestClass>();

            // Encoding the list
            string encodedString = serializer.Serialize(testObjectList);

            // Checking if the decoded list is same as the actual list
            List<TestClass> decodedList = serializer.Deserialize<List<TestClass>>(encodedString);
            Assert.Equal(testObjectList, decodedList);

            // Initializing the actual list
            for (int i = 0; i < 10; ++i)
                testObjectList.Add(new TestClass(i, "Test message" + i));

            // Making the same checks to the initialized list
            encodedString = serializer.Serialize(testObjectList);
            decodedList = serializer.Deserialize<List<TestClass>>(encodedString);
            Assert.Equal(decodedList, testObjectList);

            // Removing an element from the end and making checks
            testObjectList.Remove(testObjectList[testObjectList.Count / 2]);
            Assert.NotEqual(decodedList, testObjectList);
        }

        [Fact]
        public void SerializePacketTest()
        {
            Packet packet = new Packet("data", "destination", "module");

            string encodedString = serializer.Serialize(packet);

            Packet decodedPacket = serializer.Deserialize<Packet>(encodedString);
            Assert.Equal(packet._serializedData, decodedPacket._serializedData);
            Assert.Equal(packet._destination, decodedPacket._destination);
            Assert.Equal(packet._moduleOfPacket, decodedPacket._moduleOfPacket);
        }
    }
    
    // Custom class for testing serialization
    public class TestClass
    {
        public int value;
        public string message;

        // Parameter-less constructor
        public TestClass()
        {}

        public TestClass(int value, string message)
        {
            this.value = value;
            this.message = message;
        }

        // Overriding the implementation of the function present in the Object class to compare two instances of 'TestClass'
        public override bool Equals(object other)
        {
            TestClass otherTestClass = (TestClass)other;

            // Comparing the two instance variables
            return this.value == otherTestClass.value && this.message == otherTestClass.message;
        }
    }
}
