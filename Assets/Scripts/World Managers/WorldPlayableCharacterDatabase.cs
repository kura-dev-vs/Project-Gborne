using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RK
{
    /// <summary>
    /// プレイアブルキャラクターのためのデータベース
    /// idから該当のキャラクターを取り出す
    /// </summary>
    public class WorldPlayableCharacterDatabase : MonoBehaviour
    {
        [HideInInspector] public const int MaxPTCount = 4;
        public static WorldPlayableCharacterDatabase instance;
        public PlayableCharacter NoCharacter;
        [Header("Weapons")]
        [SerializeField] List<PlayableCharacter> playableCharacters = new List<PlayableCharacter>();

        [Header("Spawn Effects")]
        public GameObject spawnEffect;
        public GameObject uiSpawnEffect;
        public GameObject uiSpawnSparkle;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // すべてのキャラクターにidを付与する
            for (int i = 0; i < playableCharacters.Count; i++)
            {
                playableCharacters[i].pcID = i;
            }
        }
        /// <summary>
        /// idからPlayableCharacterを返す
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public PlayableCharacter GetPlayableCharacterByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return playableCharacters.FirstOrDefault(playableCharacter => playableCharacter.pcID == ID);
        }
        /// <summary>
        /// 現在使用できるPlayableCharacterの総数を返す
        /// </summary>
        /// <returns></returns> 
        public int GetPlayableCharacterCount()
        {
            int a = playableCharacters.Count;
            return a;
        }
        /// <summary>
        /// 各pcモデルが持つinventoryManagerのoutfitにPlayableCharacterで設定されたoutfitを付け替える
        /// </summary>
        /// <param name="pc"></param>
        public void SetEquipment(PlayableCharacter pc)
        {
            if (pc.rightWeapon != null)
                pc.characterModel.GetComponent<PlayerInventoryManager>().weaponsInRightHandSlots[0] = pc.rightWeapon;
            else
                pc.characterModel.GetComponent<PlayerInventoryManager>().weaponsInRightHandSlots[0] = WorldItemDatabase.instance.unarmedWeapon;

            if (pc.leftWeapon != null)
                pc.characterModel.GetComponent<PlayerInventoryManager>().weaponsInLeftHandSlots[0] = pc.leftWeapon;
            else
                pc.characterModel.GetComponent<PlayerInventoryManager>().weaponsInLeftHandSlots[0] = WorldItemDatabase.instance.unarmedWeapon;


            if (pc.hatOutfit != null)
                pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem = pc.hatOutfit;
            else
                pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem = null;

            if (pc.jacketOutfit != null)
                pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem = pc.jacketOutfit;
            else
                pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem = null;

            if (pc.topsOutfit != null)
                pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem = pc.topsOutfit;
            else
                pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem = null;

            if (pc.bottomsOutfit != null)
                pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem = pc.bottomsOutfit;
            else
                pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem = null;

            if (pc.shoesOutfit != null)
                pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem = pc.shoesOutfit;
            else
                pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem = null;

            pc.characterModel.GetComponent<PlayerInventoryManager>().ChangeCurrentWeapon();
            CalculateTotalOutfitAbsorption(pc);
        }
        /// <summary>
        /// outfitの防具性能を計算して各pcモデルが持つstatsmanagerに適用させる
        /// </summary>
        /// <param name="pc"></param> 
        public void CalculateTotalOutfitAbsorption(PlayableCharacter pc)
        {
            pc.characterModel.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption = 0;
            pc.characterModel.GetComponent<PlayerStatsManager>().outfitMagicDamageAbsorption = 0;
            pc.characterModel.GetComponent<PlayerStatsManager>().outfitFireDamageAbsorption = 0;
            pc.characterModel.GetComponent<PlayerStatsManager>().outfitLightningDamageAbsorption = 0;
            pc.characterModel.GetComponent<PlayerStatsManager>().outfitHolyDamageAbsorption = 0;

            pc.characterModel.GetComponent<PlayerStatsManager>().outfitImmunity = 0;
            pc.characterModel.GetComponent<PlayerStatsManager>().outfitRobustness = 0;
            pc.characterModel.GetComponent<PlayerStatsManager>().outfitFocus = 0;
            pc.characterModel.GetComponent<PlayerStatsManager>().outfitVitality = 0;

            pc.characterModel.GetComponent<PlayerStatsManager>().basePoiseDefense = 0;

            if (pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem != null)
            {
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem.physicalDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitMagicDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem.magicDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitFireDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem.fireDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitLightningDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem.lightningDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitHolyDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem.holyDamageAbsorption;

                pc.characterModel.GetComponent<PlayerStatsManager>().outfitImmunity += pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem.immunity;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitRobustness += pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem.robustness;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitFocus += pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem.focus;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitVitality += pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem.vitality;

                pc.characterModel.GetComponent<PlayerStatsManager>().basePoiseDefense += pc.characterModel.GetComponent<PlayerInventoryManager>().hatOutfitItem.poise;
            }

            if (pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem != null)
            {
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem.physicalDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitMagicDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem.magicDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitFireDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem.fireDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitLightningDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem.lightningDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitHolyDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem.holyDamageAbsorption;

                pc.characterModel.GetComponent<PlayerStatsManager>().outfitImmunity += pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem.immunity;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitRobustness += pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem.robustness;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitFocus += pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem.focus;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitVitality += pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem.vitality;

                pc.characterModel.GetComponent<PlayerStatsManager>().basePoiseDefense += pc.characterModel.GetComponent<PlayerInventoryManager>().jacketOutfitItem.poise;
            }

            if (pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem != null)
            {
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem.physicalDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitMagicDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem.magicDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitFireDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem.fireDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitLightningDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem.lightningDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitHolyDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem.holyDamageAbsorption;

                pc.characterModel.GetComponent<PlayerStatsManager>().outfitImmunity += pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem.immunity;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitRobustness += pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem.robustness;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitFocus += pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem.focus;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitVitality += pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem.vitality;

                pc.characterModel.GetComponent<PlayerStatsManager>().basePoiseDefense += pc.characterModel.GetComponent<PlayerInventoryManager>().topsOutfitItem.poise;
            }

            if (pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem != null)
            {
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem.physicalDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitMagicDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem.magicDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitFireDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem.fireDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitLightningDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem.lightningDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitHolyDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem.holyDamageAbsorption;

                pc.characterModel.GetComponent<PlayerStatsManager>().outfitImmunity += pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem.immunity;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitRobustness += pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem.robustness;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitFocus += pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem.focus;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitVitality += pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem.vitality;

                pc.characterModel.GetComponent<PlayerStatsManager>().basePoiseDefense += pc.characterModel.GetComponent<PlayerInventoryManager>().bottomsOutfitItem.poise;
            }

            if (pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem != null)
            {
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitPhysicalDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem.physicalDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitMagicDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem.magicDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitFireDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem.fireDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitLightningDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem.lightningDamageAbsorption;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitHolyDamageAbsorption += pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem.holyDamageAbsorption;

                pc.characterModel.GetComponent<PlayerStatsManager>().outfitImmunity += pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem.immunity;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitRobustness += pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem.robustness;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitFocus += pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem.focus;
                pc.characterModel.GetComponent<PlayerStatsManager>().outfitVitality += pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem.vitality;

                pc.characterModel.GetComponent<PlayerStatsManager>().basePoiseDefense += pc.characterModel.GetComponent<PlayerInventoryManager>().shoesOutfitItem.poise;
            }

        }
        public void SpawnVFX(GameObject vfx, GameObject player)
        {
            // パーティクルシステムのインスタンスを生成する。
            GameObject newParticle = Instantiate(vfx);
            newParticle.GetComponent<TrackingVFX>().PlayEffect(player);
        }
        public void SpawnUIVFX(GameObject vfx, GameObject player)
        {
            // パーティクルシステムのインスタンスを生成する。
            GameObject newParticle = Instantiate(vfx, player.transform);
            newParticle.GetComponent<UIVFX>().PlayEffect(player);

            GameObject newSparkle = Instantiate(uiSpawnSparkle, player.transform);
            newSparkle.GetComponent<UIVFX>().PlayEffect(player);
            var sh = newSparkle.GetComponent<ParticleSystem>().shape;
            sh.skinnedMeshRenderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }
}