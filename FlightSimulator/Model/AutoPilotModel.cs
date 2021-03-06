﻿using System.Threading;

namespace FlightSimulator.Model
{
    class AutoPilotModel
    {
        // Send each command to the simulator in a new separate thread.
        public void SendCommandsToSimulator(string info)
        {
            if (CommandsClient.Instance.IsConnectedToSimulator)
            {
                Thread t = new Thread(() =>
                {
                    CommandsClient.Instance.SendCommandsToSimulator(info);
                });
                t.Start();
            }
        }
    }
}
