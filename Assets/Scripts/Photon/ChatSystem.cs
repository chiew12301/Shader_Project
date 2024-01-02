using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using UnityEngine.UI;

namespace CHATSYSTEM
{
    public class ChatSystem : NetworkBehaviour
    {
        [Header("Group")]
        [SerializeField] private GameObject m_group = null;

        [Header("Network")]
        [SerializeField] private NetworkDebugStart m_network = null;

        [Header("Compoenents")]
        [SerializeField] private TextMeshProUGUI m_messageText;
        [SerializeField] private TextMeshProUGUI m_inputText;
        [SerializeField] private Button m_sendMessageButton;

        private string m_userName = "Default";


        //===================================================================
        
        private void Start()
        {
            this.UIStatus(false);
        }

        private void OnEnable() 
        {
            this.m_sendMessageButton.onClick.AddListener(this.CallMessageRPC);
        }

        private void OnDisable() 
        {
            this.m_sendMessageButton.onClick.RemoveListener(this.CallMessageRPC);
        }

        private void Update() 
        {
            if(this.m_network.CurrentStage == NetworkDebugStart.Stage.AllConnected && !this.m_group.activeSelf)
            {
                //Assign player username TODO
                this.UIStatus(true);
            }
        }

        //===================================================================

        private void UIStatus(bool status)
        {
            this.m_group.SetActive(status);
        }

        public void SetUserName(string name)
        {
            this.m_userName = name;
        }

        private void CallMessageRPC()
        {
            string message = this.m_inputText.text;
            this.RPC_SendMessage(this.m_userName, message);
        }

        [Rpc(RpcSources.All,RpcTargets.All)]
        private void RPC_SendMessage(string userName, string msg, RpcInfo rpcinfo = default)
        {
            this.m_messageText.text += $"{userName}: {msg}\n";
        }

        //===================================================================
    }

}
