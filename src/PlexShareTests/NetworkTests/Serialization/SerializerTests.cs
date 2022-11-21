/// <author> Anish Bhagavatula </author>
/// <summary>
/// This file contains all the tests written for the Serializer module
/// </summary>

using PlexShareNetwork.Queues;
using PlexShareNetwork.Serialization;
using System.Collections.Generic;
using Xunit;

namespace PlexShareTests.NetworkTests.Serialization
{
    public class SerializerTests
    {
        // Using the same Serializer object for all tests
        private ISerializer _serializer = new Serializer();

        /// <summary>
        /// Testing serialization of an 'int'
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializeIntegerTest()
        {
            // Integer to be tested
            int value = 10;

            // Encoding the value
            string encodedString = _serializer.Serialize(value);

            // Checking if the decoded value is same as the actual value
            int decodedValue = _serializer.Deserialize<int>(encodedString);
            Assert.Equal(decodedValue, value);

            // Checking if the decoded value is same as the actual value
            Assert.Equal(decodedValue, value);

            // Changing the value and making checks
            value += 2;
            Assert.NotEqual(decodedValue, value);
        }

        /// <summary>
        /// Testing serialization of a 'double'
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializeDoubleTest()
        {
            // Double to be tested
            double value = 10.234;

            // Encoding the value
            string encodedString = _serializer.Serialize(value);

            // Checking if the decoded value is same as the actual value
            double decodedValue = _serializer.Deserialize<double>(encodedString);
            Assert.Equal(decodedValue, value);

            // Changing the value and making checks
            value -= 0.0001;
            Assert.NotEqual(decodedValue, value);
        }

        /// <summary>
        /// Testing serialization of a 'string'
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializeStringTest()
        {
            // String to be tested
            string message = "Hello world!";

            // Encoding the message
            string encodedString = _serializer.Serialize(message);

            // Checking if the decoded string is same as the actual message
            string decodedString = _serializer.Deserialize<string>(encodedString);
            Assert.Equal(decodedString, message);

            // Referencing to another string and making checks
            message = "Hello world!\n";
            Assert.NotEqual(decodedString, message);
        }

        /// <summary>
        /// Testing serialization of an array of 'int'
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializeIntegerArrayTest()
        {
            // Integer array to be tested
            int[] arr = new int[10];

            // Encoding the array
            string encodedString = _serializer.Serialize(arr);

            // Checking if the decoded array is same as the actual array
            int[] decodedArray = _serializer.Deserialize<int[]>(encodedString);
            Assert.Equal(decodedArray, arr);

            // Initializing the actual array
            for (int i = 0; i < 10; ++i)
                arr[i] = i;

            // Making the same checks to the initialized array
            encodedString = _serializer.Serialize(arr);
            decodedArray = _serializer.Deserialize<int[]>(encodedString);
            Assert.Equal(decodedArray, arr);

            // Changing an element from the array and making checks
            arr[arr.Length / 2] = -2;
            Assert.NotEqual(decodedArray, arr);
        }

        /// <summary>
        /// Testing serialization of an object of a custom class
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializeCustomObjectTest()
        {
            // TestClass object to be tested
            TestClass testObject = new TestClass(200, "Test message!");

            // Encoding the object
            string encodedString = _serializer.Serialize(testObject);

            // Checking if the decoded object is same as the actual object
            TestClass decodedObject = _serializer.Deserialize<TestClass>(encodedString);
            Assert.Equal(testObject, decodedObject);

            // Changing the value of an instance variable and making checks
            testObject.value = -100;
            Assert.NotEqual(testObject, decodedObject);
        }

        /// <summary>
        /// Testing serialization of an array of [objects of a custom class]
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializeCustomObjectArrayTest()
        {
            // TestClass array to be tested
            TestClass[] testObjectArray = new TestClass[10];

            // Encoding the array
            string encodedString = _serializer.Serialize(testObjectArray);

            // Checking if the decoded array is same as the actual array
            TestClass[] decodedArray = _serializer.Deserialize<TestClass[]>(encodedString);
            Assert.Equal(testObjectArray, decodedArray);

            // Initializing the actual array
            for (int i = 0; i < 10; ++i)
                testObjectArray[i] = new TestClass(i, "Test message" + i);

            // Making the same checks to the initialized array
            encodedString = _serializer.Serialize(testObjectArray);
            decodedArray = _serializer.Deserialize<TestClass[]>(encodedString);
            Assert.Equal(decodedArray, testObjectArray);

            // Changing the value of an instance variable of an element in the array and making checks
            testObjectArray[testObjectArray.Length / 2].message = "Okay!";
            Assert.NotEqual(decodedArray, testObjectArray);
        }

        /// <summary>
        /// Testing serialization of a list of [objects of a custom class]
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializeCustomObjectListTest()
        {
            // TestClass list to be tested
            List<TestClass> testObjectList = new List<TestClass>();

            // Encoding the list
            string encodedString = _serializer.Serialize(testObjectList);

            // Checking if the decoded list is same as the actual list
            List<TestClass> decodedList = _serializer.Deserialize<List<TestClass>>(encodedString);
            Assert.Equal(testObjectList, decodedList);

            // Initializing the actual list
            for (int i = 0; i < 10; ++i)
                testObjectList.Add(new TestClass(i, "Test message" + i));

            // Making the same checks to the initialized list
            encodedString = _serializer.Serialize(testObjectList);
            decodedList = _serializer.Deserialize<List<TestClass>>(encodedString);
            Assert.Equal(decodedList, testObjectList);

            // Removing an element from the end and making checks
            testObjectList.Remove(testObjectList[testObjectList.Count / 2]);
            Assert.NotEqual(decodedList, testObjectList);
        }

        /// <summary>
        /// Testing serialization of a 'Packet' object
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializePacketTest()
        {
            Packet packet = new Packet("data", "destination", "module");

            string encodedString = _serializer.Serialize(packet);

            // Checking if each field variable is matching
            Packet decodedPacket = _serializer.Deserialize<Packet>(encodedString);
            NetworkTestGlobals.AssertPacketEquality(packet, decodedPacket);
        }

        /// <summary>
        /// Testing serialization of a 'NoEmptyConstructorTestClass' object
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializeNoEmptyConstructorTestClassObjectTest()
        {
            NoEmptyConstructorTestClass noEmptyConstructorTestClass = new NoEmptyConstructorTestClass(10, "Hello");

            // This must return 'null' as the class does not have an empty constructor
            string serializedString = _serializer.Serialize(noEmptyConstructorTestClass);

            Assert.Equal(serializedString, null);
        }

        /// <summary>
        /// Testing serialization of a 'PrivateVariablesTestClass' object
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializePrivateVariablesTestClassObjectTest()
        {
            PrivateVariablesTestClass privateVariablesTestClass = new PrivateVariablesTestClass(10, "Hello");

            // This must return 'null' as the class has private instance variables
            string serializedString = _serializer.Serialize(privateVariablesTestClass);

            Assert.Equal(serializedString, null);
        }

        /// <summary>
        /// Testing serialization of a null object
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void SerializeNullTest()
        {
            string serializedString = _serializer.Serialize<string>(null);
            Assert.Equal(serializedString, null);
        }

        /// <summary>
        /// Testing deserialization of a null object
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void DeserializeNullObjectTest()
        {
            string message = _serializer.Deserialize<string>(null);
            Assert.Equal(message, null);
        }

        /// <summary>
        /// Testing deserialization of a random string
        /// </summary>
        /// <returns> void </returns>
        [Fact]
        public void DeserializeRandomStringTest()
        {
            string message = _serializer.Deserialize<string>("hello");
            Assert.Equal(message, null);
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

    public class NoEmptyConstructorTestClass
    {
        public int value;
        public string message;

        public NoEmptyConstructorTestClass(int value, string message)
        {
            this.value = value;
            this.message = message;
        }

        // Overriding the implementation of the function present in the Object class to compare two instances of
        // 'NoEmptyConstructorTestClass'
        public override bool Equals(object other)
        {
            NoEmptyConstructorTestClass otherTestClass = (NoEmptyConstructorTestClass)other;

            // Comparing the two instance variables
            return this.value == otherTestClass.value && this.message == otherTestClass.message;
        }
    }

    public class PrivateVariablesTestClass
    {
        private int _value;
        private string _message;

        // Getters and setters
        public int GetValue()
        {
            return _value;
        }
        public void SetValue(int value)
        {
            this._value = value;
        }
        public string GetMessage()
        {
            return _message;
        }
        public void SetMessage(string message)
        {
            this._message= message;
        }

        public PrivateVariablesTestClass(int value, string message)
        {
            this._value = value;
            this._message = message;
        }

        // Overriding the implementation of the function present in the Object class to compare two instances of
        // 'PrivateVariablesTestClass'
        public override bool Equals(object other)
        {
            PrivateVariablesTestClass otherTestClass = (PrivateVariablesTestClass)other;

            // Comparing the two instance variables
            return this._value == otherTestClass._value && this._message == otherTestClass._message;
        }
    }
}
