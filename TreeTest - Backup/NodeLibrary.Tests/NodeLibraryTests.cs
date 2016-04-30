using System;
using System.IO;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NodeLibrary.Tests
{
    [TestClass]
    public class NodeLibraryTests
    {
        private string tab = NodeDescriber.Tab;

        /// <remarks>
        /// Ensure the folder path 'C:\Temp' exists before running these tests,
        /// or change the value of the private string 'filePath'.
        /// 
        /// Note that the methods 'NodeWriterTestInternal' and 'IntegrationTestInternal', delete the file
        /// specified by the 'filePath' variable. The file path includes a GUID value to ensure that
        /// a required file is not accidentally deleted.
        /// </remarks>
        private string filePath = @"C:\Temp\WrittenNode_33ce5d8b-160b-464e-82df-a0d676cb77d5.txt";

        [TestMethod]
        public void NodeDescriberTest()
        {
            // Traverse down and across tree
            INodeDescriber implementation = new NodeDescriber(new NodesHelper());
            var testData = new SingleChildNode("root",
                new TwoChildrenNode("child1",
                    new NoChildrenNode("leaf1"),
                    new SingleChildNode("child2",
                        new NoChildrenNode("leaf2"))));

            var result = implementation.Describe(testData);

            StringBuilder sb = new StringBuilder();
            sb.Append("new SingleChildNode(\"root\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append("new TwoChildrenNode(\"child1\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append("new NoChildrenNode(\"leaf1\"),"); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append("new SingleChildNode(\"child2\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append(tab); sb.Append("new NoChildrenNode(\"leaf2\"))));");
            string expectedResult = sb.ToString();

            Assert.AreEqual(expectedResult, result);


            // Traverse down tree, back up, then back down
            implementation = new NodeDescriber(new NodesHelper());
            testData = new SingleChildNode("root",
                new ManyChildrenNode("child1",
                    new SingleChildNode("child2",
                        new NoChildrenNode("leaf1")),
                    new NoChildrenNode("leaf2")));

            result = implementation.Describe(testData);

            sb = new StringBuilder();
            sb.Append("new SingleChildNode(\"root\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append("new ManyChildrenNode(\"child1\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append("new SingleChildNode(\"child2\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append(tab); sb.Append("new NoChildrenNode(\"leaf1\")),"); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append("new NoChildrenNode(\"leaf2\")));");
            expectedResult = sb.ToString();

            Assert.AreEqual(expectedResult, result);


            // Traverse tree, removing null nodes
            implementation = new NodeDescriber(new NodesHelper());
            var testDataWithNulls = new ManyChildrenNode("root",
                new TwoChildrenNode("child1",
                    null,
                    new SingleChildNode("leaf1",
                        null)));

            result = implementation.Describe(testDataWithNulls);

            sb = new StringBuilder();
            sb.Append("new ManyChildrenNode(\"root\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append("new TwoChildrenNode(\"child1\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append("new SingleChildNode(\"leaf1\")));");
            expectedResult = sb.ToString();

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void NodeTransformerTest()
        {
            // Traverse tree, of many ManyChildrenNode
            INodeTransformer implementation = new NodeTransformer(new NodesHelper());
            var testData = new ManyChildrenNode("root",
                new ManyChildrenNode("child1",
                    new ManyChildrenNode("leaf1"),
                    new ManyChildrenNode("child2",
                        new ManyChildrenNode("leaf2"))));
            var result = implementation.Transform(testData);

            var root = result;
            Assert.IsInstanceOfType(root, typeof(SingleChildNode));
            Assert.AreEqual("root", root.Name);

            var child1 = ((SingleChildNode)root).Child;
            Assert.IsInstanceOfType(child1, typeof(TwoChildrenNode));
            Assert.AreEqual("child1", child1.Name);

            var leaf1 = ((TwoChildrenNode)child1).FirstChild;
            Assert.IsInstanceOfType(leaf1, typeof(NoChildrenNode));
            Assert.AreEqual("leaf1", leaf1.Name);

            var child2 = ((TwoChildrenNode)child1).SecondChild;
            Assert.IsInstanceOfType(child2, typeof(SingleChildNode));
            Assert.AreEqual("child2", child2.Name);

            var leaf2 = ((SingleChildNode)child2).Child;
            Assert.IsInstanceOfType(leaf2, typeof(NoChildrenNode));
            Assert.AreEqual("leaf2", leaf2.Name);


            // Traverse tree, of various types of nodes
            implementation = new NodeTransformer(new NodesHelper());
            testData = new ManyChildrenNode("root",
                new TwoChildrenNode("child1",
                    new ManyChildrenNode("leaf1"),
                    new SingleChildNode("child2",
                        new NoChildrenNode("leaf2"))));
            result = implementation.Transform(testData);

            root = result;
            Assert.IsInstanceOfType(root, typeof(SingleChildNode));
            Assert.AreEqual("root", root.Name);

            child1 = ((SingleChildNode)root).Child;
            Assert.IsInstanceOfType(child1, typeof(TwoChildrenNode));
            Assert.AreEqual("child1", child1.Name);

            leaf1 = ((TwoChildrenNode)child1).FirstChild;
            Assert.IsInstanceOfType(leaf1, typeof(NoChildrenNode));
            Assert.AreEqual("leaf1", leaf1.Name);

            child2 = ((TwoChildrenNode)child1).SecondChild;
            Assert.IsInstanceOfType(child2, typeof(SingleChildNode));
            Assert.AreEqual("child2", child2.Name);

            leaf2 = ((SingleChildNode)child2).Child;
            Assert.IsInstanceOfType(leaf2, typeof(NoChildrenNode));
            Assert.AreEqual("leaf2", leaf2.Name);


            // Traverse tree, of various types of nodes, removing null nodes
            implementation = new NodeTransformer(new NodesHelper());
            testData = new ManyChildrenNode("root",
                new TwoChildrenNode("child1",
                    null,
                    new SingleChildNode("leaf1",
                        null)));
            result = implementation.Transform(testData);

            root = result;
            Assert.IsInstanceOfType(root, typeof(SingleChildNode));
            Assert.AreEqual("root", root.Name);

            child1 = ((SingleChildNode)root).Child;
            Assert.IsInstanceOfType(child1, typeof(SingleChildNode));
            Assert.AreEqual("child1", child1.Name);

            leaf1 = ((SingleChildNode)child1).Child;
            Assert.IsInstanceOfType(leaf1, typeof(NoChildrenNode));
            Assert.AreEqual("leaf1", leaf1.Name);
        }

        [TestMethod]
        public void NodeWriterTest()
        {
            NodeWriterTestInternal();
        }

        private async void NodeWriterTestInternal()
        {
            // One node description to file
            File.Delete(filePath);
            INodeWriter implementation = new NodeWriter(new NodeDescriber(new NodesHelper()));
            await implementation.WriteToFileAsync(new NoChildrenNode("root"), filePath);
            var result = File.ReadAllText(filePath, Encoding.Unicode);

            Assert.AreEqual("new NoChildrenNode(\"root\");", result);


            // Multiple nodes description to file
            File.Delete(filePath);
            implementation = new NodeWriter(new NodeDescriber(new NodesHelper()));
            await implementation.WriteToFileAsync(new SingleChildNode("root",
                new TwoChildrenNode("child1",
                    new NoChildrenNode("leaf1"),
                    new SingleChildNode("child2",
                        new NoChildrenNode("leaf2")))), filePath);
            result = File.ReadAllText(filePath, Encoding.Unicode);

            StringBuilder sb = new StringBuilder();
            sb.Append("new SingleChildNode(\"root\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append("new TwoChildrenNode(\"child1\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append("new NoChildrenNode(\"leaf1\"),"); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append("new SingleChildNode(\"child2\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append(tab); sb.Append("new NoChildrenNode(\"leaf2\"))));");
            string expectedResult = sb.ToString();

            Assert.AreEqual(sb.ToString(), result);
        }

        [TestMethod]
        public void IntegrationTest()
        {
            IntegrationTestInternal();
        }

        private async void IntegrationTestInternal()
        {
            // Traverse tree, of various types of nodes,
            // transform nodes (removing null nodes), describe nodes, write nodes   
            File.Delete(filePath);
            var testData = new ManyChildrenNode("root",
                new TwoChildrenNode("child1",
                    null,
                    new SingleChildNode("leaf1",
                        null)));

            var container = new UnityContainer();
            container.RegisterType<INodesHelper, NodesHelper>();
            container.RegisterType<INodeTransformer, NodeTransformer>();
            container.RegisterType<INodeDescriber, NodeDescriber>();
            container.RegisterType<INodeWriter, NodeWriter>();
            var nodeWriter = container.Resolve<NodeWriter>();

            await nodeWriter.WriteToFileAsync(container.Resolve<NodeTransformer>().Transform(testData), filePath);            
            var result = File.ReadAllText(filePath, Encoding.Unicode);

            StringBuilder sb = new StringBuilder();
            sb.Append("new SingleChildNode(\"root\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append("new SingleChildNode(\"child1\","); sb.Append(Environment.NewLine);
            sb.Append(tab); sb.Append(tab); sb.Append("new NoChildrenNode(\"leaf1\")));");
            string expectedResult = sb.ToString();

            Assert.AreEqual(expectedResult, result);
        }
    }
}