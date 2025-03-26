using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


namespace RK
{
    /// <summary>
    /// インタラクトオブジェクト用スクリプトの大元
    /// </summary>
    public class Interactable : NetworkBehaviour
    {
        public string interactableText; // インタラクションコライダー（アイテムを拾う、レバーを引くなど）に入る際のテキストプロンプト。
        public Sprite interactableIcon; // 上記テキストの左隣に表示される画像。アイテムの場合はアイテム画像。他の場合は応じたアイコンを設定
        [SerializeField] protected Collider interactableCollider;   // プレイヤーとのインタラクションをチェックするコライダー
        [SerializeField] protected bool hostOnlyInteractable = true;    // 有効な場合、オブジェクトはCo-opプレーヤーとインタラクションできない

        protected virtual void Awake()
        {
            if (interactableCollider == null)
                interactableCollider = GetComponent<Collider>();
        }
        protected virtual void Start()
        {

        }
        public virtual void Interact(PlayerManager player)
        {
            if (!player.IsOwner)
                return;

            interactableCollider.enabled = false;
            player.playerInteractionManager.RemoveInteractionFromList(this);
            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindow();
        }
        /*
        public virtual void OnTriggerEnter(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player != null)
            {
                if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                    return;
                if (!player.IsOwner)
                    return;
                // プレイヤーにインタラクションを渡す
                player.playerInteractionManager.AddInteractionToList(this);
            }
        }
        */
        public virtual void OnTriggerStay(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player != null)
            {
                if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                    return;
                if (!player.IsOwner)
                    return;
                // プレイヤーにインタラクションを渡す
                player.playerInteractionManager.AddInteractionToList(this);
            }
        }
        public virtual void OnTriggerExit(Collider other)
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player != null)
            {
                if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                    return;
                if (!player.IsOwner)
                    return;
                // プレイヤーからインタラクションを取り除く
                player.playerInteractionManager.RemoveInteractionFromList(this);
                PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindow();
            }
        }
    }
}
