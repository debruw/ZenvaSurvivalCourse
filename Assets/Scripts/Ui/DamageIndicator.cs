using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZenvaSurvival.Ui
{
    public class DamageIndicator : MonoBehaviour
    {
        public Image image;
        public float flashSpeed = .5f;

        private Coroutine fadeAway;
        
        public void Flash()
        {
            if (fadeAway != null)
            {
                StopCoroutine(fadeAway);
            }

            image.enabled = true;
            image.color = Color.white;

            fadeAway = StartCoroutine(FadeAway());
        }

        IEnumerator FadeAway()
        {
            float a = 1f;
            while (a > 0)
            {
                a -= (1 / flashSpeed) * Time.deltaTime;
                image.color = new Color(1, 1, 1, a);
                yield return null;
            }

            image.enabled = false;
        }
    }
}