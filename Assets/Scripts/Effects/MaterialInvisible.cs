using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// 特定のマテリアルを非表示する際使用する予定のコンポーネント
/// 直接マテリアルの不透明度を調整する
/// シリアライズされた子要素クラス
[System.Serializable]
public class MultiArrayClass
{
    [Header("非表示にするSkinned Mesh Render InspectorのMaterialsの数字に対応")]
    public int[] multiArray;

    public MultiArrayClass(int[] _multiArray)
    {
        multiArray = _multiArray;
    }
}

public class MaterialInvisible : MonoBehaviour
{
    [Header("Debug Menu")]
    [SerializeField] bool invisibleMat = false;
    [SerializeField] bool visibleMat = false;

    //[Header("置き換えるためのマテリアル(透明)")]
    //public Material invisibleMat;
    [Header("非表示にしたいSkinnedMeshRendererを持つオブジェクトを指定")]
    [SerializeField] GameObject[] skinnedMeshRenderer;
    [SerializeField] MultiArrayClass[] deleteMatNo1;
    List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    private void Start()
    {
        for (int i = 0; i < skinnedMeshRenderer.Length; i++)
        {
            skinnedMeshRenderers.Add(skinnedMeshRenderer[i].GetComponent<SkinnedMeshRenderer>());
        }
    }

    private void Update()
    {
        if (invisibleMat)
        {
            invisibleMat = false;
            for (int i = 0; i < skinnedMeshRenderers.Count; i++)
            {
                for (int j = 0; j < deleteMatNo1[i].multiArray.Length; j++)
                {
                    Invisible(skinnedMeshRenderers[i], deleteMatNo1[i].multiArray[j]);
                }
            }
        }
        if (visibleMat)
        {
            visibleMat = false;
            for (int i = 0; i < skinnedMeshRenderers.Count; i++)
            {
                for (int j = 0; j < deleteMatNo1[i].multiArray.Length; j++)
                {
                    Visible(skinnedMeshRenderers[i], deleteMatNo1[i].multiArray[j]);
                }
            }
        }
    }
    private void Invisible(SkinnedMeshRenderer smr, int num)
    {
        /*
        Material[] mats = smr.materials;
        mats[num] = invisibleMat;
        smr.materials = mats;
        */
        Material[] mats = smr.materials;
        Color theColorToAdjust = mats[num].color;
        theColorToAdjust.a = 0f; // Completely transparent
        mats[num].color = theColorToAdjust;
    }
    private void Visible(SkinnedMeshRenderer smr, int num)
    {

        Material[] mats = smr.materials;
        Color theColorToAdjust = mats[num].color;
        theColorToAdjust.a = 1f; // Completely transparent
        mats[num].color = theColorToAdjust;
    }
    private void DestroyIfExist(GameObject Target, string path)
    {
        if (Target.transform.Find(path) != null)
            GameObject.Destroy(Target.transform.Find(path).gameObject);
        //else
        //    Debug.Log(path + " not found");
    }
}
