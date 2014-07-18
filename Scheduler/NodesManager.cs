using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using Autoscout24.Scheduler.Models;

using AutoScout24.SeleniumTestLibrary.Common;

using log4net;

namespace Autoscout24.Scheduler
{
    public class NodesManager
    {
        private readonly int noOfParallelProcesses;
        private int currentlyUsedNodesCount;
        private readonly Queue<SeleniumNode> chromeQueue = new Queue<SeleniumNode>();
        private readonly Queue<SeleniumNode> firefoxQueue = new Queue<SeleniumNode>();
        private readonly Queue<SeleniumNode> ieQueue = new Queue<SeleniumNode>();
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<int, NodeConfig> nodes = new Dictionary<int, NodeConfig>();

        public NodesManager(int noOfParallelProcesses)
        {
            this.noOfParallelProcesses = noOfParallelProcesses;
            currentlyUsedNodesCount = 0;
            ThreadPool.SetMinThreads(noOfParallelProcesses, noOfParallelProcesses);
            ThreadPool.SetMaxThreads(noOfParallelProcesses, noOfParallelProcesses);
        }

        public void InitTestConfigs(List<SeleniumNode> seleniumNodes)
        {
            foreach (var seleniumNode in seleniumNodes)
            {
                switch (seleniumNode.ClientType)
                {
                    case ClientType.Chrome:
                        chromeQueue.Enqueue(seleniumNode);
                        break;
                    case ClientType.Firefox:
                        firefoxQueue.Enqueue(seleniumNode);
                        break;
                    case ClientType.Ie:
                        ieQueue.Enqueue(seleniumNode);
                        break;
                    default:
                        throw new Exception("Unknown client type");
                }
            }
            log.Info("Initialized the test configurations");
        }

        public NodeConfig GetTestConfig(int key)
        {
            while (noOfParallelProcesses == currentlyUsedNodesCount)
            {
                Thread.Sleep(1000);
            }

            currentlyUsedNodesCount++;
            var testConfig = new NodeConfig
            {
                ChromeNode = GetChromeNode(),
                FirefoxNode = GetFirefoxNode(),
                IeNode = GetIeNode()
            };

            nodes.Add(key, testConfig);
            log.Info(string.Format("Returning config for key:{0} {1}: ", key, testConfig.ChromeNode.Port));
            return testConfig;
        }

        public void FreeNode(int key)
        {
            var testConfig = nodes[key];
            
            if (!chromeQueue.Contains(testConfig.ChromeNode))
            {
                chromeQueue.Enqueue(testConfig.ChromeNode);
            }
            if (!firefoxQueue.Contains(testConfig.FirefoxNode))
            {
                firefoxQueue.Enqueue(testConfig.FirefoxNode);
            }
            if (!ieQueue.Contains(testConfig.IeNode))
            {
                ieQueue.Enqueue(testConfig.IeNode);
            }
            nodes.Remove(key);
            log.Info(string.Format("Released config for key:{0} {1}: ", key, testConfig.ChromeNode.Port));
            currentlyUsedNodesCount--;
        }

        private SeleniumNode GetChromeNode()
        {
            return chromeQueue.Count > 1 ? chromeQueue.Dequeue() : chromeQueue.Peek();
        }

        private SeleniumNode GetFirefoxNode()
        {
            return firefoxQueue.Count > 1 ? firefoxQueue.Dequeue() : firefoxQueue.Peek();
        }

        private SeleniumNode GetIeNode()
        {
            return ieQueue.Count > 1 ? ieQueue.Dequeue() : ieQueue.Peek();
        }
    }
}
