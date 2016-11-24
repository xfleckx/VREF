using UnityEngine;
using System.Collections;

namespace Assets.VREF.Scripts.Instructions
{
    /// <summary>
    /// Fades the screen from a given color to fully transparent or from transparent to a given color
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class FullScreenFade : MonoBehaviour
    {
        /// <summary>
        /// Seconds between fading states
        /// </summary>
        private float timeToFade = 2.0f;

        /// <summary>
        /// The initial screen color.
        /// </summary>
        public Color colorOpaque = new Color(0.01f, 0.01f, 0.01f, 1.0f);

        public Color colorTransparent = new Color(0.01f, 0.01f, 0.01f, 0.0f);

        public Material material = null;

        private bool isFading = false;
        private YieldInstruction fadeInstruction = new WaitForEndOfFrame();
        private bool stayOpaque;

        /// <summary>
        /// Initialize.
        /// </summary>
        void Awake()
        {
            if (material == null)
                Debug.LogError("FullScreen Fading will not work until you add a material!");
        }

        /// <summary>
        /// Starts the fade in
        /// </summary>
        public void StartFadeIn(float durationInSeconds = 2f)
        {
            timeToFade = durationInSeconds;

            if (material != null)
                StartCoroutine(FadeIn());
        }

        /// <summary>
        /// Starts the fade in
        /// </summary>
        public void StartFadeOut(float durationInSeconds = 2f)
        {
            timeToFade = durationInSeconds;

            if (material != null)
                StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Fades alpha from 1.0 to 0.0
        /// </summary>
        IEnumerator FadeIn()
        {
            float elapsedTime = 0.0f;
            material.color = colorOpaque;
            Color color = colorOpaque;
            isFading = true;
            while (elapsedTime < timeToFade)
            {
                yield return fadeInstruction;
                elapsedTime += Time.deltaTime;
                color.a = 1.0f - Mathf.Clamp01(elapsedTime / timeToFade);
                material.color = color;
            }

            material.color = colorTransparent;

            stayOpaque = false;
            isFading = false;
        }

        /// <summary>
        /// Fades alpha from 0.0 to 1.0
        /// </summary>
        IEnumerator FadeOut()
        {
            float elapsedTime = 0.0f;
            material.color = colorTransparent;
            Color color = colorTransparent;
            isFading = true;
            while (elapsedTime < timeToFade)
            {
                yield return fadeInstruction;
                elapsedTime += Time.deltaTime;
                color.a = 0.0f + Mathf.Clamp01(elapsedTime / timeToFade);
                material.color = color;
            }

            material.color = colorOpaque;

            stayOpaque = true;
            isFading = false;
        }

        void OnPostRender()
        {
            if (isFading || stayOpaque)
            {
                material.SetPass(0);
                GL.PushMatrix();
                GL.LoadOrtho();
                GL.Color(material.color);
                GL.Begin(GL.QUADS);
                GL.Vertex3(0f, 0f, -1f);
                GL.Vertex3(0f, 1f, -1f);
                GL.Vertex3(1f, 1f, -1f);
                GL.Vertex3(1f, 0f, -1f);
                GL.End();
                GL.PopMatrix();
            }
        }
    }

}

