/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains all the tests written for the Serializer module
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Serialization;
using System;
using System.Collections.Generic;
using Xunit;

namespace PlexShareTests.NetworkTests.Serialization
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

            // Datatype of the value
            string dataType = value.GetType().ToString();

            // Encoding the value
            string encodedString = serializer.Serialize(value);

            // Checking if the decoded value is same as the actual value
            int decodedValue = serializer.Deserialize<int>(encodedString);
            Assert.Equal(decodedValue, value);

            // Checking if the data types are matching as well
            string decodedDataType = serializer.GetObjectType<int>(encodedString);
            Assert.Equal(dataType, decodedDataType);

            // Checking if the decoded value is same as the actual value
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

            // Datatype of the value
            string dataType = value.GetType().ToString();

            // Encoding the value
            string encodedString = serializer.Serialize(value);

            // Checking if the decoded value is same as the actual value
            double decodedValue = serializer.Deserialize<double>(encodedString);
            Assert.Equal(decodedValue, value);

            // Checking if the data types are matching as well
            string decodedDataType = serializer.GetObjectType<double>(encodedString);
            Assert.Equal(dataType, decodedDataType);

            // Changing the value and making checks
            value -= 0.0001;
            Assert.NotEqual(decodedValue, value);
        }

        [Fact]
        public void SerializeStringTest()
        {
            // String to be tested
            string message = "Hello world!";

            // Datatype of the message
            string dataType = message.GetType().ToString();

            // Encoding the message
            string encodedString = serializer.Serialize(message);

            // Checking if the decoded string is same as the actual message
            string decodedString = serializer.Deserialize<string>(encodedString);
            Assert.Equal(decodedString, message);

            // Checking if the data types are matching as well
            string decodedDataType = serializer.GetObjectType<string>(encodedString);
            Assert.Equal(dataType, decodedDataType);

            // Referencing to another string and making checks
            message = "Hello world!\n";
            Assert.NotEqual(decodedString, message);
        }

        [Fact]
        public void SerializeIntegerArrayTest()
        {
            // Integer array to be tested
            int[] arr = new int[10];

            // Datatype of the array
            string dataType = arr.GetType().ToString();

            // Encoding the array
            string encodedString = serializer.Serialize(arr);

            // Checking if the decoded array is same as the actual array
            int[] decodedArray = serializer.Deserialize<int[]>(encodedString);
            Assert.Equal(decodedArray, arr);

            // Checking if the data types are matching as well
            string decodedDataType = serializer.GetObjectType<int[]>(encodedString);
            Assert.Equal(dataType, decodedDataType);

            // Initializing the actual array
            for (int i = 0; i < 10; ++i)
                arr[i] = i;

            // Making the same checks to the initialized array
            encodedString = serializer.Serialize(arr);
            decodedArray = serializer.Deserialize<int[]>(encodedString);
            Assert.Equal(decodedArray, arr);

            // Checking if the data types are matching as well
            decodedDataType = serializer.GetObjectType<int[]>(encodedString);
            Assert.Equal(dataType, decodedDataType);

            // Changing an element from the array and making checks
            arr[arr.Length / 2] = -2;
            Assert.NotEqual(decodedArray, arr);
        }

        [Fact]
        public void SerializeCustomObjectTest()
        {
            // TestClass object to be tested
            TestClass testObject = new TestClass(200, "Test message!");

            // Datatype of the test object
            string dataType = testObject.GetType().ToString();

            // Encoding the object
            string encodedString = serializer.Serialize(testObject);

            // Checking if the decoded object is same as the actual object
            TestClass decodedObject = serializer.Deserialize<TestClass>(encodedString);
            Assert.Equal(testObject, decodedObject);

            // Checking if the data types are matching as well
            string decodedDataType = serializer.GetObjectType<TestClass>(encodedString);
            Assert.Equal(dataType, decodedDataType);

            // Changing the value of an instance variable and making checks
            testObject.value = -100;
            Assert.NotEqual(testObject, decodedObject);
        }

        [Fact]
        public void SerializeCustomObjectArrayTest()
        {
            // TestClass array to be tested
            TestClass[] testObjectArray = new TestClass[10];

            // Datatype of the array
            string dataType = testObjectArray.GetType().ToString();

            // Encoding the array
            string encodedString = serializer.Serialize(testObjectArray);

            // Checking if the decoded array is same as the actual array
            TestClass[] decodedArray = serializer.Deserialize<TestClass[]>(encodedString);
            Assert.Equal(testObjectArray, decodedArray);

            // Checking if the data types are matching as well
            string decodedDataType = serializer.GetObjectType<TestClass[]>(encodedString);
            Assert.Equal(dataType, decodedDataType);

            // Initializing the actual array
            for (int i = 0; i < 10; ++i)
                testObjectArray[i] = new TestClass(i, "Test message" + i);

            // Making the same checks to the initialized array
            encodedString = serializer.Serialize(testObjectArray);
            decodedArray = serializer.Deserialize<TestClass[]>(encodedString);
            Assert.Equal(decodedArray, testObjectArray);

            // Checking if the data types are matching as well
            decodedDataType = serializer.GetObjectType<TestClass[]>(encodedString);
            Assert.Equal(dataType, decodedDataType);

            // Changing the value of an instance variable of an element in the array and making checks
            testObjectArray[testObjectArray.Length / 2].message = "Okay!";
            Assert.NotEqual(decodedArray, testObjectArray);
        }

        [Fact]
        public void SerializeCustomObjectListTest()
        {
            // TestClass list to be tested
            List<TestClass> testObjectList = new List<TestClass>();

            // Datatype of the list
            string dataType = testObjectList.GetType().ToString();

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

            // Checking if the data types are matching as well
            string decodedDataType = serializer.GetObjectType<List<TestClass>>(encodedString);
            Assert.Equal(dataType, decodedDataType);

            // Removing an element from the end and making checks
            testObjectList.Remove(testObjectList[testObjectList.Count / 2]);
            Assert.NotEqual(decodedList, testObjectList);
        }

        [Fact]
        public void SerializePacketTest()
        {
            Packet packet = new Packet("data", "destination", "module");

            // Datatype of the packet
            string dataType = packet.GetType().ToString();

            string encodedString = serializer.Serialize(packet);

            // Checking if each field variable is matching
            Packet decodedPacket = serializer.Deserialize<Packet>(encodedString);
            NetworkTestGlobals.AssertPacketEquality(packet, decodedPacket);

            // Checking if the data types are matching as well
            string decodedDataType = serializer.GetObjectType<Packet>(encodedString);
            Assert.Equal(dataType, decodedDataType);
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
