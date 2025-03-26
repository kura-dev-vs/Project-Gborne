using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// プレイヤーキャラ専用のステータス関連を追加したいときに使う予定
    /// </summary>
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;
        protected override void Awake()
        {
            base.Awake();
            player = GetComponentInParent<PlayerManager>();
        }
        protected override void Start()
        {
            base.Start();
            if (player == null)
                return;
            // なぜここで計算するのか？
            // キャラクター作成メニューを作成し、クラスによってステータスを設定すると、そこで計算される
            // セーブファイルが存在する場合、シーンにロードするときに上書きされる
            //CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);
            //CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
        }
        public void CalculateTotalOutfitAbsorption()
        {
            outfitPhysicalDamageAbsorption = 0;
            outfitMagicDamageAbsorption = 0;
            outfitFireDamageAbsorption = 0;
            outfitLightningDamageAbsorption = 0;
            outfitHolyDamageAbsorption = 0;

            outfitImmunity = 0;
            outfitRobustness = 0;
            outfitFocus = 0;
            outfitVitality = 0;

            basePoiseDefense = 0;

            if (player.playerInventoryManager.hatOutfitItem != null)
            {
                outfitPhysicalDamageAbsorption += player.playerInventoryManager.hatOutfitItem.physicalDamageAbsorption;
                outfitMagicDamageAbsorption += player.playerInventoryManager.hatOutfitItem.magicDamageAbsorption;
                outfitFireDamageAbsorption += player.playerInventoryManager.hatOutfitItem.fireDamageAbsorption;
                outfitLightningDamageAbsorption += player.playerInventoryManager.hatOutfitItem.lightningDamageAbsorption;
                outfitHolyDamageAbsorption += player.playerInventoryManager.hatOutfitItem.holyDamageAbsorption;

                outfitImmunity += player.playerInventoryManager.hatOutfitItem.immunity;
                outfitRobustness += player.playerInventoryManager.hatOutfitItem.robustness;
                outfitFocus += player.playerInventoryManager.hatOutfitItem.focus;
                outfitVitality += player.playerInventoryManager.hatOutfitItem.vitality;

                basePoiseDefense += player.playerInventoryManager.hatOutfitItem.poise;
            }

            if (player.playerInventoryManager.jacketOutfitItem != null)
            {
                outfitPhysicalDamageAbsorption += player.playerInventoryManager.jacketOutfitItem.physicalDamageAbsorption;
                outfitMagicDamageAbsorption += player.playerInventoryManager.jacketOutfitItem.magicDamageAbsorption;
                outfitFireDamageAbsorption += player.playerInventoryManager.jacketOutfitItem.fireDamageAbsorption;
                outfitLightningDamageAbsorption += player.playerInventoryManager.jacketOutfitItem.lightningDamageAbsorption;
                outfitHolyDamageAbsorption += player.playerInventoryManager.jacketOutfitItem.holyDamageAbsorption;

                outfitImmunity += player.playerInventoryManager.jacketOutfitItem.immunity;
                outfitRobustness += player.playerInventoryManager.jacketOutfitItem.robustness;
                outfitFocus += player.playerInventoryManager.jacketOutfitItem.focus;
                outfitVitality += player.playerInventoryManager.jacketOutfitItem.vitality;

                basePoiseDefense += player.playerInventoryManager.jacketOutfitItem.poise;
            }

            if (player.playerInventoryManager.topsOutfitItem != null)
            {
                outfitPhysicalDamageAbsorption += player.playerInventoryManager.topsOutfitItem.physicalDamageAbsorption;
                outfitMagicDamageAbsorption += player.playerInventoryManager.topsOutfitItem.magicDamageAbsorption;
                outfitFireDamageAbsorption += player.playerInventoryManager.topsOutfitItem.fireDamageAbsorption;
                outfitLightningDamageAbsorption += player.playerInventoryManager.topsOutfitItem.lightningDamageAbsorption;
                outfitHolyDamageAbsorption += player.playerInventoryManager.topsOutfitItem.holyDamageAbsorption;

                outfitImmunity += player.playerInventoryManager.topsOutfitItem.immunity;
                outfitRobustness += player.playerInventoryManager.topsOutfitItem.robustness;
                outfitFocus += player.playerInventoryManager.topsOutfitItem.focus;
                outfitVitality += player.playerInventoryManager.topsOutfitItem.vitality;

                basePoiseDefense += player.playerInventoryManager.topsOutfitItem.poise;
            }

            if (player.playerInventoryManager.bottomsOutfitItem != null)
            {
                outfitPhysicalDamageAbsorption += player.playerInventoryManager.bottomsOutfitItem.physicalDamageAbsorption;
                outfitMagicDamageAbsorption += player.playerInventoryManager.bottomsOutfitItem.magicDamageAbsorption;
                outfitFireDamageAbsorption += player.playerInventoryManager.bottomsOutfitItem.fireDamageAbsorption;
                outfitLightningDamageAbsorption += player.playerInventoryManager.bottomsOutfitItem.lightningDamageAbsorption;
                outfitHolyDamageAbsorption += player.playerInventoryManager.bottomsOutfitItem.holyDamageAbsorption;

                outfitImmunity += player.playerInventoryManager.bottomsOutfitItem.immunity;
                outfitRobustness += player.playerInventoryManager.bottomsOutfitItem.robustness;
                outfitFocus += player.playerInventoryManager.bottomsOutfitItem.focus;
                outfitVitality += player.playerInventoryManager.bottomsOutfitItem.vitality;

                basePoiseDefense += player.playerInventoryManager.bottomsOutfitItem.poise;
            }

            if (player.playerInventoryManager.shoesOutfitItem != null)
            {
                outfitPhysicalDamageAbsorption += player.playerInventoryManager.shoesOutfitItem.physicalDamageAbsorption;
                outfitMagicDamageAbsorption += player.playerInventoryManager.shoesOutfitItem.magicDamageAbsorption;
                outfitFireDamageAbsorption += player.playerInventoryManager.shoesOutfitItem.fireDamageAbsorption;
                outfitLightningDamageAbsorption += player.playerInventoryManager.shoesOutfitItem.lightningDamageAbsorption;
                outfitHolyDamageAbsorption += player.playerInventoryManager.shoesOutfitItem.holyDamageAbsorption;

                outfitImmunity += player.playerInventoryManager.shoesOutfitItem.immunity;
                outfitRobustness += player.playerInventoryManager.shoesOutfitItem.robustness;
                outfitFocus += player.playerInventoryManager.shoesOutfitItem.focus;
                outfitVitality += player.playerInventoryManager.shoesOutfitItem.vitality;

                basePoiseDefense += player.playerInventoryManager.shoesOutfitItem.poise;
            }

        }

        public void SetManager()
        {
            character = GetComponentInParent<CharacterManager>();
            player = GetComponentInParent<PlayerManager>();
        }

    }
}
