using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace RK
{
    public class MultiplayerTentativeScript : MonoBehaviour
    {
        private bool pcAssigned;
        IPAddress ipaddr;

        [SerializeField] TextMeshProUGUI ipAddressText;
        [SerializeField] TMP_InputField ip;

        [SerializeField] string ipAddress;
        [SerializeField] UnityTransport transport;

        private void Start()
        {
            ipAddress = "0.0.0.0";
            //SetIpAddress();
            pcAssigned = false;
        }
        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            GetLocalIPAddress();
        }
        public void StartClient()
        {
            bool s = IPAddress.TryParse(ip.text, out ipaddr);
            if (!s)
                return;
            NetworkManager.Singleton.Shutdown();
            ipAddress = ip.text;
            SetIpAddress();
            NetworkManager.Singleton.StartClient();
        }
        /// <summary>
        /// ONLY HOST SIDE
        /// Gets the Ip Address of your connected network and 
        /// shows on the screen in order to let other players join
        /// by inputing that Ip in the input field
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception> <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddressText.text = ip.ToString();
                    ipAddress = ip.ToString();
                    return ip.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }
        /// <summary>
        /// ONLY FOR CLIENT SIDE
        /// Sets the Ip Address of the Connection Data in Unity Transport
        /// to the Ip Address which was input in the input fields
        /// </summary>
        public void SetIpAddress()
        {
            transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = ipAddress;
        }
        public void ShutdownServer()
        {
            NetworkManager.Singleton.Shutdown();
        }

    }
}
