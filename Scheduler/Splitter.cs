using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Autoscout24.Scheduler
{
    public class Splitter
    {
        public List<string> GetNamespaces(string assemblyPath, int testsGroupLimit)
        {            
            var testClassess = GetTestClasses(assemblyPath);
            testClassess.Sort((size, andSize) => andSize.MethodCount.CompareTo(size.MethodCount));
            return PutSmallTestsTogether(testClassess, testsGroupLimit);
        }

        // TODO improve the splitting so that not too many small tests are put together
        private static List<string> PutSmallTestsTogether(IEnumerable<TypeAndSize> testClasses, int testsGroupLimit)
        {
            var newShortenedList = new List<TypeAndSize>();
            var allSmallTestClasses = new StringBuilder();
            var min = testsGroupLimit;
            testClasses = testClasses.ToList();
            foreach (var testClass in testClasses)
            {
                if (testClass.MethodCount <= min)
                {
                    allSmallTestClasses.AppendFormat(",{0}", testClass.TypeName);
                }
                else
                {
                    newShortenedList.Add(testClass);
                }
            }
            var testNames = allSmallTestClasses.ToString();
            if (testNames.StartsWith(","))
                testNames = testNames.Remove(0, 1);

            var testsList = newShortenedList.Select(typeAndSize => typeAndSize.TypeName).ToList();
            testsList.Add(testNames);
            return testsList;
        }

        private static List<TypeAndSize> GetTestClasses(string assemblyPath)
        {
            var typeAndSizeList = new List<TypeAndSize>();
            Console.WriteLine("Loading tests from {0}", assemblyPath);
            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException(assemblyPath);
            }
            var assembly = Assembly.LoadFile(assemblyPath);
            var types = assembly.GetTypes();
            var testTypes = types.Where(t => t.Name.EndsWith("Tests")).ToList();
            foreach (var testType in testTypes)
            {
                var typeAndSize = new TypeAndSize
                {
                    TypeName = testType.FullName,
                    MethodCount = testType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Count()
                };
                typeAndSizeList.Add(typeAndSize);
            }
            return typeAndSizeList;
        }
    }

    internal class TypeAndSize
    {
        public string TypeName { get; set; }
        public int MethodCount { get; set; }
    }
}