using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Mebiustos.MMD4MecanimFaciem {
    [InitializeOnLoad]
    static class FaceRepaintManager {
        //static bool isPlaying = false;
        static string currentScene;

        static bool firstSceneChange;
        static FaceRepaintManager() {
            //Debug.Log("Wakeup MMD4MecanimFaciem.FaceRepaintManager.");
            //currentScene = EditorApplication.currentScene;
            EditorApplication.playmodeStateChanged += PlaymodeStop; // プレイ停止用
            //EditorApplication.hierarchyWindowChanged += StateChange2; // プレイ停止用
            EditorApplication.hierarchyWindowChanged += SceneChange; // シーン変更用
        }

        static void SceneChange() {
            //Debug.LogWarning("SceneChange");
            if (currentScene != EditorApplication.currentScene) {
                currentScene = EditorApplication.currentScene;
                //if (!firstSceneChange) {
                //    firstSceneChange = true;
                //    return;
                //}
                //var databases = (FaciemDatabase[])Resources.FindObjectsOfTypeAll(typeof(FaciemDatabase));
                //foreach (var database in databases) {
                //    database._selectedIdx = database.GetDefaultFaceIndex();
                //    database._selectedGuid = database.DefaultGuid;
                //    EditorUtility.SetDirty(database);
                //}
                //RepaintAllFace();

                var faciemInspectors = (FaciemDatabaseInspector[])Resources.FindObjectsOfTypeAll(typeof(FaciemDatabaseInspector));
                foreach (var inspector in faciemInspectors) {
                    inspector.isRequestLoad = true;
                }
            }
        }

        //static void StateChange2() {
        //    if (EditorApplication.isPlayingOrWillChangePlaymode == false && EditorApplication.isPlaying == false) {
        //        EditorApplication.hierarchyWindowChanged -= StateChange2;
        //        RepaintAllFace();
        //    }
        //}

        static void PlaymodeStop() {
            //Debug.LogWarning("PlaymodeStop");
            RepaintAllFace();
        }

        static void RepaintAllFace() {
            //Debug.Log("RepaintAllFace");
            if (EditorApplication.isPlayingOrWillChangePlaymode == false && EditorApplication.isPlaying == false) {
            //if (EditorApplication.isPlayingOrWillChangePlaymode == false) {
                //Debug.LogWarning("RepaintAllFace.");
                var databases = (FaciemDatabase[])Resources.FindObjectsOfTypeAll(typeof(FaciemDatabase));
                foreach (var database in databases) {
                    //Debug.Log("RepaintFace: " + database.gameObject.name);
                    //FaciemDatabaseEditor.KeepMMD4MMorphList(database); ;
                    FaciemDatabaseEditor.MfaceToMMD4M(database);
                }
            }
        }
    }
}