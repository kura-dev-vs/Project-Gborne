using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// アイテムのデータベース
    /// inspectorでアイテムを登録し、idから該当のアイテムを認識できる
    /// </summary>
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase instance;
        public WeaponItem unarmedWeapon;
        public GameObject pickUpItemPrefab;
        public Sprite transparent;

        [Header("Weapons")]
        [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();
        [Header("Outfits")]
        [SerializeField] List<HatOutfitItem> hatOutfits = new List<HatOutfitItem>();

        [SerializeField] List<JacketOutfitItem> jacketOutfits = new List<JacketOutfitItem>();

        [SerializeField] List<TopsOutfitItem> topsOutfits = new List<TopsOutfitItem>();

        [SerializeField] List<BottomsOutfitItem> bottomsOutfits = new List<BottomsOutfitItem>();

        [SerializeField] List<ShoesOutfitItem> shoesOutfits = new List<ShoesOutfitItem>();
        // ゲーム内の全アイテムのリスト
        [Header("Items")]
        private List<Item> items = new List<Item>();

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
            // すべての武器をアイテムリストに追加する
            foreach (var weapon in weapons)
            {
                items.Add(weapon);
            }
            foreach (var item in hatOutfits)
            {
                items.Add(item);
            }
            foreach (var item in jacketOutfits)
            {
                items.Add(item);
            }
            foreach (var item in topsOutfits)
            {
                items.Add(item);
            }
            foreach (var item in bottomsOutfits)
            {
                items.Add(item);
            }
            foreach (var item in shoesOutfits)
            {
                items.Add(item);
            }

            // すべてのアイテムに一意のアイテムIDを割り当てる
            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }

            for (int i = 0; i < hatOutfits.Count; i++)
            {
                hatOutfits[i].itemID = i + 1000;
            }

            for (int i = 0; i < jacketOutfits.Count; i++)
            {
                jacketOutfits[i].itemID = i + 2000;
            }

            for (int i = 0; i < topsOutfits.Count; i++)
            {
                topsOutfits[i].itemID = i + 3000;
            }

            for (int i = 0; i < bottomsOutfits.Count; i++)
            {
                bottomsOutfits[i].itemID = i + 4000;
            }

            for (int i = 0; i < shoesOutfits.Count; i++)
            {
                shoesOutfits[i].itemID = i + 5000;
            }
        }
        public Item GetItemByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return items.FirstOrDefault(item => item.itemID == ID);
        }
        public WeaponItem GetWeaponByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
        }
        public HatOutfitItem GetHatOutfitByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return hatOutfits.FirstOrDefault(hatOutfit => hatOutfit.itemID == ID);
        }
        public JacketOutfitItem GetJacketOutfitByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return jacketOutfits.FirstOrDefault(jacketOutfit => jacketOutfit.itemID == ID);
        }
        public TopsOutfitItem GetTopsOutfitByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return topsOutfits.FirstOrDefault(topsOutfit => topsOutfit.itemID == ID);
        }
        public BottomsOutfitItem GetBottomsOutfitByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return bottomsOutfits.FirstOrDefault(bottomsOutfit => bottomsOutfit.itemID == ID);
        }
        public ShoesOutfitItem GetShoesOutfitByID(int ID)
        {
            // 該当するidがなかったら最初の要素を返す
            return shoesOutfits.FirstOrDefault(shoesOutfit => shoesOutfit.itemID == ID);
        }
    }
}
