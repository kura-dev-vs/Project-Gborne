using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace RK
{
    public class VCamScript : MonoBehaviour
    {
        public bool debugPlay = false;
        PlayableDirector director;
        CinemachineBrain brain;
        [SerializeField] TimelineAsset asset;
        public CinemachineVirtualCamera[] burstCameras;

        private void Start()
        {
            CinemachineVirtualCamera[] cam = GetComponentsInChildren<CinemachineVirtualCamera>();
            foreach (var vcam in burstCameras)
            {
                vcam.Follow = gameObject.transform;
                vcam.LookAt = gameObject.transform;
            }
            brain = PlayerCamera.instance.brain;
        }
        private void Update()
        {
            if (debugPlay)
            {
                debugPlay = false;

                director = WorldPlayableDirector.instance.director;
                OnEnable();
                director.playableAsset = asset;
                var outputTracks = (director.playableAsset as TimelineAsset).GetOutputTracks();
                foreach (TrackAsset trackAsset in outputTracks)
                {
                    if (trackAsset is CinemachineTrack)
                    {
                        director.SetGenericBinding(trackAsset, brain);

                        TimelineClip[] cinemachineClips = (TimelineClip[])trackAsset.GetClips();
                        for (int i = 0; i < cinemachineClips.Length; i++)
                        {
                            CinemachineShot cinemachineShot = cinemachineClips[i].asset as CinemachineShot;
                            director.SetReferenceValue(cinemachineShot.VirtualCamera.exposedName, burstCameras[i]);
                        }
                    }
                }
                director.Play();
            }
        }
        private void OnEnable()
        {
            PlayerManager player = GetComponentInParent<PlayerManager>();
            if (player == null)
                return;
            if (!player.IsOwner)
                return;
            if (director != null)
            {
                director.played += OnPlayableDirectorPlayed;
                director.stopped += OnPlayableDirectorStopped;
            }
        }
        private void OnPlayableDirectorPlayed(PlayableDirector aDirector)
        {
            if (director == aDirector)
            {
                Debug.Log("PlayableDirector named " + aDirector.name + " is now played.");
                PlayerCamera.instance.blackObject.SetActive(true);
                PlayerCamera.instance.mainCamera.cullingMask &= ~WorldUtilityManager.instance.GetBurstCameraRemovedLayers();
            }
        }

        private void OnPlayableDirectorStopped(PlayableDirector aDirector)
        {
            if (director == aDirector)
            {
                Debug.Log("PlayableDirector named " + aDirector.name + " is now stopped.");
                PlayerCamera.instance.blackObject.SetActive(false);
                PlayerCamera.instance.mainCamera.cullingMask |= WorldUtilityManager.instance.GetBurstCameraRemovedLayers();
            }
        }

        private void OnDisable()
        {
            PlayerManager player = GetComponentInParent<PlayerManager>();
            if (player == null || director == null)
                return;
            if (!player.IsOwner)
                return;
            director.played -= OnPlayableDirectorPlayed;
            director.stopped -= OnPlayableDirectorStopped;
        }
    }
}
