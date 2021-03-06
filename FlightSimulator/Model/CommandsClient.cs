﻿using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace FlightSimulator.Model
{
    class CommandsClient : IDisposable
    {
        private IPEndPoint endPoint;
        // The current device as a client.
        private TcpClient currentDeviceClient;
        private StreamWriter writer;
        private static CommandsClient instance = null;

        private CommandsClient() { }

        // Commands singleton.
        public static CommandsClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CommandsClient();
                }
                return instance;
            }
        }

        public void Initialize() { instance = null; }

        // isSimulatorConnected Accessors.
        public bool IsConnectedToSimulator { get; set; } = false;

        // Connect as a client to the simulator which is the server.
        public void Connect(string ip, int portNumber)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(ip), portNumber);
            currentDeviceClient = new TcpClient();
            // Try to connect to the server until success.
            while (currentDeviceClient.Connected == false)
            {
                try
                {
                    currentDeviceClient.Connect(endPoint);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            IsConnectedToSimulator = true;
            writer = new StreamWriter(currentDeviceClient.GetStream());
        }

        // Send the commands to the simulator.
        public void SendCommandsToSimulator(string info)
        {
            char delimiter = '\n';
            int sleepMillisecons = 2000;
            string currentCommand = "";
            // If there is a content in info:
            if (info != null && info != "")
            {
                string[] commandsStr = info.Split(delimiter);
                for (int i = 0; i < commandsStr.Length; i++)
                {
                    currentCommand = commandsStr[i];
                    try
                    {
                        writer.WriteLine(currentCommand);
                        writer.Flush();
                        // Sleep for 2000 milliseconds (delay of 2 seconds).
                        Thread.Sleep(sleepMillisecons);
                    }
                    finally
                    {
                    }
                }
            }
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
