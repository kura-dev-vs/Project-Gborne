using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 各キャラクターモデルに必須なコンポーネント
    /// 親のオブジェクト (entry) から各モデル固有のコンポーネントにアクセスするときはここを通る
    /// </summary>
    public class PlayableCharacterManager : MonoBehaviour
    {
        public int playableCharacterID;
        PlayerManager player;
        public PlayableCharacter playableCharacter;
        public PlayerAnimatorManager playerAnimatorManager;
        public PlayerEquipmentManager playerEquipmentManager;
        public PlayerEffectsManager playerEffectsManager;
        public PlayerInventoryManager playerInventoryManager;
        public PlayerStatsManager playerStatsManager;
        public AnimationEventTentative animationEventTentative;
        public PlayerSoundFXManager playerSoundFXManager;
        private void Awake()
        {
            player = GetComponent<PlayerManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            animationEventTentative = GetComponent<AnimationEventTentative>();
            playerSoundFXManager = GetComponent<PlayerSoundFXManager>();
        }

        /// <summary>
        /// モデルをinstantiateしたときに親のコンポーネントへのアクセスを付けておく
        /// </summary>
        /// <param name="entryManager"></param>
        /// <param name="playableCharacter"></param>
        public void SetCharacterInformation(EntryManager entryManager, PlayableCharacter playableCharacter)
        {
            playableCharacterID = playableCharacter.pcID;
            this.playableCharacter = playableCharacter;
            playerAnimatorManager.SetManager();
            playerEquipmentManager.SetManger();
            playerEffectsManager.SetManager();
            animationEventTentative.SetManger();
            playerSoundFXManager.SetAudioSource();
            playerStatsManager.SetManager();
        }
    }
}
