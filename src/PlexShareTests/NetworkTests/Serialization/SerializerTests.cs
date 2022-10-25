using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Networking.Serialization.Test {

    [TestClass()]
    public class SerializerTests
    {
        // Using the same Serializer object for all tests
        ISerializer serializer = new Serializer();

        [TestMethod()]
        public void SerializeIntegerTest()
        {
            // Integer to be tested
            int value = 10;

            // Encoding the value
            string encodedString = serializer.Serialize(value);

            // Checking if the decoded value is same as the actual value
            int decodedValue = serializer.Deserialize<int>(encodedString);
            Assert.AreEqual(decodedValue, value);

            // Changing the value and making checks
            value += 2;
            Assert.AreNotEqual(decodedValue, value);
        }

        [TestMethod()]
        public void SerializeDoubleTest()
        {
            // Double to be tested
            double value = 10.234;

            // Encoding the value
            string encodedString = serializer.Serialize(value);

            // Checking if the decoded value is same as the actual value
            double decodedValue = serializer.Deserialize<double>(encodedString);
            Assert.AreEqual(decodedValue, value);

            // Changing the value and making checks
            value -= 0.0001;
            Assert.AreNotEqual(decodedValue, value);
        }

        [TestMethod()]
        public void SerializeStringTest()
        {
            // String to be tested
            string message = "Hello world!";

            // Encoding the message
            string encodedString = serializer.Serialize(message);

            // Checking if the decoded string is same as the actual message
            string decodedString = serializer.Deserialize<string>(encodedString);
            Assert.AreEqual(decodedString, message);

            // Referencing to another string and making checks
            message = "Hello world!\n";
            Assert.AreNotEqual(decodedString, message);
        }

        [TestMethod()]
        public void SerializeIntegerArrayTest()
        {
            // Integer array to be tested
            int[] arr = new int[10];

            // Encoding the array
            string encodedString = serializer.Serialize(arr);

            // Checking if the decoded array is same as the actual array
            int[] decodedArray = serializer.Deserialize<int[]>(encodedString);
            CollectionAssert.AreEqual(decodedArray, arr);

            // Initializing the actual array
            for (int i = 0; i < 10; ++i)
                arr[i] = i;

            // Making the same checks to the initialized array
            encodedString = serializer.Serialize(arr);
            decodedArray = serializer.Deserialize<int[]>(encodedString);
            CollectionAssert.AreEqual(decodedArray, arr);

            // Changing an element from the array and making checks
            arr[arr.Length / 2] = -2;
            CollectionAssert.AreNotEqual(decodedArray, arr);
        }

        [TestMethod()]
        public void SerializeCustomObjectTest()
        {
            // TestClass object to be tested
            TestClass testObject = new TestClass(200, "Test message!");

            // Encoding the object
            string encodedString = serializer.Serialize(testObject);

            // Checking if the decoded object is same as the actual object
            TestClass decodedObject = serializer.Deserialize<TestClass>(encodedString);
            Assert.AreEqual(testObject, decodedObject);

            // Changing the value of an instance variable and making checks
            testObject.value = -100;
            Assert.AreNotEqual(testObject, decodedObject);
        }

        [TestMethod()]
        public void SerializeCustomObjectArrayTest()
        {
            // TestClass array to be tested
            TestClass[] testObjectArray = new TestClass[10];

            // Encoding the array
            string encodedString = serializer.Serialize(testObjectArray);

            // Checking if the decoded array is same as the actual array
            TestClass[] decodedArray = serializer.Deserialize<TestClass[]>(encodedString);
            CollectionAssert.AreEqual(testObjectArray, decodedArray);

            // Initializing the actual array
            for (int i = 0; i < 10; ++i)
                testObjectArray[i] = new TestClass(i, "Test message" + i);

            // Making the same checks to the initialized array
            encodedString = serializer.Serialize(testObjectArray);
            decodedArray = serializer.Deserialize<TestClass[]>(encodedString);
            CollectionAssert.AreEqual(decodedArray, testObjectArray);

            // Changing the value of an instance variable of an element in the array and making checks
            testObjectArray[testObjectArray.Length / 2].message = "Okay!";
            CollectionAssert.AreNotEqual(decodedArray, testObjectArray);
        }

        [TestMethod()]
        public void SerializeCustomObjectListTest()
        {
            // TestClass list to be tested
            List<TestClass> testObjectList = new List<TestClass>();

            // Encoding the list
            string encodedString = serializer.Serialize(testObjectList);

            // Checking if the decoded list is same as the actual list
            List<TestClass> decodedList = serializer.Deserialize<List<TestClass>>(encodedString);
            CollectionAssert.AreEqual(testObjectList, decodedList);

            // Initializing the actual list
            for (int i = 0; i < 10; ++i)
                testObjectList.Add(new TestClass(i, "Test message" + i));

            // Making the same checks to the initialized list
            encodedString = serializer.Serialize(testObjectList);
            decodedList = serializer.Deserialize<List<TestClass>>(encodedString);
            CollectionAssert.AreEqual(decodedList, testObjectList);

            // Removing an element from the end and making checks
            testObjectList.Remove(testObjectList[testObjectList.Count / 2]);
            CollectionAssert.AreNotEqual(decodedList, testObjectList);
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
