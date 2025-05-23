using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RK
{
    public class BlendShapeBlink : MonoBehaviour
    {
        //Time.unscaledDeltaTime
        public bool canBlink = true;
        [System.Serializable]
        public class Shape
        {
            [HideInInspector]
            public string name;

            public SkinnedMeshRenderer skinnedMeshRenderer;

            public int index;

            public Mesh SharedMesh { get { return skinnedMeshRenderer.sharedMesh; } }

            public virtual void OnValidate()
            {
                if (skinnedMeshRenderer)
                {
                    index = Mathf.Clamp(index, 0, SharedMesh.blendShapeCount - 1);
                    name = SharedMesh.GetBlendShapeName(index);
                }
                else
                {
                    index = 0;
                    name = "";
                }
            }
        }

        [System.Serializable]
        public class BlinkShape : Shape
        {
            [Range(0.0f, 100.0f)]
            public float minWeight;

            [Range(0.001f, 100.0f)]
            public float maxWeight;

            public override void OnValidate()
            {
                base.OnValidate();

                minWeight = Mathf.Clamp(minWeight, 0.0f, 100.0f);

                if (maxWeight <= 0.0f) { maxWeight = 100.0f; }
                maxWeight = Mathf.Clamp(maxWeight, 0.0f, 100.0f);
            }
        }

        [System.Serializable]
        public class AvoidedShape : Shape
        {
            [Range(0.001f, 100.0f)]
            public float thresholdWeight;

            public override void OnValidate()
            {
                base.OnValidate();

                if (thresholdWeight <= 0.0f) { thresholdWeight = 10.0f; }
                thresholdWeight = Mathf.Clamp(thresholdWeight, 0.001f, 100.0f);
            }
        }

        [SerializeField]
        BlinkShape[] blinkShapes;

        [SerializeField]
        AvoidedShape[] avoidedShapes;

        public float closingTime = 0.05f;

        public float openingTime = 0.05f;

        public float minOpenedInterval = 0.5f;

        public float maxOpenedInterval = 8.0f;

        public float minClosedInterval = 0.1f;

        public float maxClosedInterval = 0.2f;

        float openedTime;

        float closedTime;

        delegate void BlinkUpdate();

        BlinkUpdate onBlinkUpdate;

        public bool IsBlinking { get { return onBlinkUpdate != OnBlinkOpened; } }

        float transitionTime;

        bool isPreviousAvoided;
        float currentTime = 0;

        private void OnValidate()
        {
            foreach (var shape in blinkShapes) { shape.OnValidate(); }
            foreach (var shape in avoidedShapes) { shape.OnValidate(); }
        }

        private void OnEnable()
        {
            ResetBlinking();
        }

        private void OnDisable()
        {
            ResetBlinking();
        }

        private void LateUpdate()
        {
            if (canBlink)
            {
                bool isAvoided = IsAvoidedShapesWeighted();

                if (isAvoided != isPreviousAvoided) { ResetBlinking(); }
                isPreviousAvoided = isAvoided;

                if (isAvoided) { return; }

                onBlinkUpdate();
                currentTime += Time.unscaledDeltaTime;
            }
        }

        void OnBlinkOpened()
        {
            var elapsedTime = currentTime - transitionTime;
            if (elapsedTime < openedTime) { return; }

            onBlinkUpdate = OnBlinkClosing;
            transitionTime = currentTime;
        }

        void OnBlinkClosing()
        {
            var elapsedTime = currentTime - transitionTime;
            var t = (closingTime > 0.0f) ? Mathf.Clamp01(elapsedTime / closingTime) : 1.0f;

            foreach (var shape in blinkShapes)
            {
                var weight = Mathf.Lerp(shape.minWeight, shape.maxWeight, t);
                shape.skinnedMeshRenderer.SetBlendShapeWeight(shape.index, weight);
            }

            if (elapsedTime < closingTime) { return; }

            onBlinkUpdate = OnBlinkClosed;
            transitionTime = currentTime;
        }

        void OnBlinkClosed()
        {
            var elapsedTime = currentTime - transitionTime;
            if (elapsedTime < closedTime) { return; }

            onBlinkUpdate = OnBlinkOpening;
            transitionTime = currentTime;
        }

        void OnBlinkOpening()
        {
            var elapsedTime = currentTime - transitionTime;
            var t = (openingTime > 0.0f) ? Mathf.Clamp01(elapsedTime / openingTime) : 1.0f;

            foreach (var shape in blinkShapes)
            {
                var weight = Mathf.Lerp(shape.maxWeight, shape.minWeight, t);
                shape.skinnedMeshRenderer.SetBlendShapeWeight(shape.index, weight);
            }

            if (elapsedTime < openingTime) { return; }

            ResetBlinking();
        }

        public void ResetBlinking()
        {
            foreach (var shape in blinkShapes)
            {
                shape.skinnedMeshRenderer.SetBlendShapeWeight(shape.index, shape.minWeight);
            }

            openedTime = Random.Range(minOpenedInterval, maxOpenedInterval);
            closedTime = Random.Range(minClosedInterval, maxClosedInterval);

            onBlinkUpdate = OnBlinkOpened;
            transitionTime = currentTime;
        }

        bool IsAvoidedShapesWeighted()
        {
            return avoidedShapes.Any(s => s.skinnedMeshRenderer.GetBlendShapeWeight(s.index) >= s.thresholdWeight);
        }
    }
}
