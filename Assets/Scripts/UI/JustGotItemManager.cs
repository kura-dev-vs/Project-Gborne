using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace RK
{
    /// <summary>
    /// アイテムを拾った際に画面左側に表示されるUI
    /// </summary>
    public class JustGotItemManager : MonoBehaviour
    {
        [Header("Just Got Item Information")]
        [SerializeField] Image itemIcon;
        [SerializeField] TextMeshProUGUI itemName;
        public void SetItemInformation(Item item, int amount)
        {
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName + " x" + amount.ToString();
        }
        private void Start()
        {
            Destroy(gameObject, 5f);
        }
    }
}
