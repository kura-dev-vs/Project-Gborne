using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// PTに含まれるPCの管理、アクセスを行う
    /// </summary>
    public class PlayableCharacterInventoryManager : MonoBehaviour
    {
        EntryManager entry;
        public PlayableCharacter currentCharacter;
        [Header("Quick Slots")]
        public PlayableCharacter[] currentPCPT = new PlayableCharacter[4];
        public int currentCharacterIndex = 0;
        public GameObject[] pcObjects = new GameObject[4];
        protected void Awake()
        {
            entry = GetComponent<EntryManager>();
        }

        /// <summary>
        /// 引数modelに該当する子オブジェクトのPCモデルのアニメーターコンポ―ネントやモデル自身をresultの値にする。
        /// trueの場合はアクティブ状態に、falseの場合は非アクティブ状態に
        /// </summary>
        /// <param name="result"></param>
        /// <param name="model"></param> 
        public void SetActivePlayableCharacter(bool result, GameObject model)
        {
            if (model != null)
            {
                model.GetComponent<Animator>().enabled = result;
                model.GetComponent<PlayerAnimatorManager>().enabled = result;
                foreach (Transform item in model.transform)
                {
                    item.gameObject.SetActive(result);
                }
            }
        }

        /// <summary>
        /// 引数のmodelをアクティブキャラにする際に呼び出される
        /// 3DモデルをVsctor3.zeroの位置に置く
        /// </summary>
        /// <param name="characterModel"></param>
        public void PositioningCharacterModel(GameObject characterModel)
        {
            characterModel.transform.parent = transform;
            characterModel.transform.localPosition = Vector3.zero;
            characterModel.transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// PlayerInputManagerの入力に応じた番号に位置するPCに切り替える際の経由メソッド
        /// </summary>
        /// <param name="slotsIndex"></param>
        public void ChangeCharacter(int slotsIndex)
        {
            int newCharacterID = entry.playableCharacterEntryNetworkManager.currentPTIDNetworkList[slotsIndex];
            if (newCharacterID == WorldPlayableCharacterDatabase.instance.NoCharacter.pcID)
                return;
            entry.playableCharacterEntryManager.FindCharacterByIDFromChildren(newCharacterID);
        }

        /// <summary>
        /// PTをリフレッシュする際に一度全てのPCオブジェクトを削除するために使用
        /// </summary>
        /// <param name="root"></param>
        private void DestroyChildAll(Transform root)
        {
            foreach (Transform child in root)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// PTをリフレッシュする際に使用。
        /// PTPCのネットワーク変数リストに適したPCにcurrentPCPTを入れ替える
        /// </summary>
        public void RefreshDeployedPT()
        {
            if (entry.IsOwner)
                PlayerUIManager.instance.DestroyChildAll(PlayerUIManager.instance.playerUIHudManager.characterSlotParent);
            entry.player.playerEffectsManager.RemoveAllStaticEffects();
            DestroyChildAll(transform);
            for (int i = 0; i < entry.playableCharacterEntryNetworkManager.currentPTIDNetworkList.Count; i++)
            {
                entry.playableCharacterInventoryManager.currentPCPT[i] = WorldPlayableCharacterDatabase.instance.GetPlayableCharacterByID(entry.playableCharacterEntryNetworkManager.currentPTIDNetworkList[i]);
            }

            if (entry.player.IsOwner)
            {
                entry.playableCharacterEntryNetworkManager.resetPTFire.Value = true;
            }

            entry.playableCharacterEntryManager.SetFirstCharacter();

            entry.player.playerCombatManager.lockOnTransform = entry.player.myLockOnTransform.transform;
        }
    }

}