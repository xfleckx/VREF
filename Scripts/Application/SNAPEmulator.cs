using UnityEngine; 
using UnityEngine.Events;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System; 
using System.Text;
using System.Linq;

namespace Assets.VREF.Scripts
{
    /// <summary>
    /// SNAP is another presentation framework for neuroscientific experiments
    /// It comes with simple tcp remote control capabilities.
    /// This class severes as interface to be compatible with the existing tools for running experiments.
    /// </summary>
    public class SNAPEmulator : MonoBehaviour
    {
        static readonly char[] commandSeparator = new char[] { '\n' };

        [Range(1026, 65535)]
        public int PortToListenOn = 7897;

        public RemoteCommandRecievedEvent OnCommandRecieved;

        private Socket client;
        TcpListener tcpListener;

        private int BufferSizeForIncomingData = 256;

        void Awake()
        {
            Debug.Log(string.Format("SNAP Emulation online - listen to Port: {0}!", PortToListenOn));
            SetupTcpListener();
        }

        // Update is called once per frame
        void Update()
        {
            if(tcpListener == null)
            {
                SetupTcpListener();
            }

            tcpListener.Start();

            if (tcpListener.Pending())
            {
                //Accept the pending client connection and return a TcpClient object initialized for communication.
                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                int bytesRead = 0;
                SocketError err;
                byte[] receiveData = new byte[BufferSizeForIncomingData];
                bytesRead = tcpClient.Client.Receive(receiveData, 0, BufferSizeForIncomingData, SocketFlags.None, out err);

                if (bytesRead == 0)
                    return;

                // we expecting values from python (LabRecorder) which default encoding is ascii
                var incomingString = Encoding.ASCII.GetString(receiveData);
                
                var stringWithOutZeroBytes = incomingString.RemoveZeroBytes();

                if (stringWithOutZeroBytes != String.Empty)
                    RouteToSubscriber(stringWithOutZeroBytes);
            }
        }

        private void SetupTcpListener()
        {
            tcpListener = new TcpListener(IPAddress.Any, PortToListenOn);
        }

        private void RouteToSubscriber(string result)
        {
            Debug.Log(string.Format("SNAPEmulator recieved: {0}", result));

            var splittedStrings = result.Split(commandSeparator);

            var tempList = new List<string>(splittedStrings);
            // just to avoid unnecessary function calls for empty strings in the array after splitting
            var allNoneEmptyCommands = tempList.Where((s) => !string.IsNullOrEmpty(s));
            
            foreach (var commandText in allNoneEmptyCommands)
            {
                if (OnCommandRecieved != null && OnCommandRecieved.GetPersistentEventCount() > 0)
                    OnCommandRecieved.Invoke(commandText);
            }
        }
    }

    [Serializable]
    public class RemoteCommandRecievedEvent : UnityEvent<String> { }
}
