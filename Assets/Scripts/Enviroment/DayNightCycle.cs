using System;
using UnityEngine;

namespace ZenvaSurvival.Enviroment
{
    public class DayNightCycle : MonoBehaviour
    {
        [Range(0, 1)] public float time; //0 to 1
        public float fullDayLength;
        public float startTime = .4f;
        private float timeRate;
        public Vector3 noon;

        [Header("Sun")] public Light sun;
        public Gradient sunColor;
        public AnimationCurve sunIntensity;

        [Header("Moon")] public Light moon;
        public Gradient moonColor;
        public AnimationCurve moonIntensity;

        [Header("Other lightning")] public AnimationCurve lightningIntensityMultiplier;
        public AnimationCurve reflectionsIntensityMultiplier;

        private void Start()
        {
            timeRate = 1f / fullDayLength;
            time = startTime;
        }

        private void Update()
        {
            time += timeRate * Time.deltaTime;

            if (time >= 1)
                time = 0;

            sun.transform.eulerAngles = (time - .25f) * noon * 4f;
            moon.transform.eulerAngles = (time - .75f) * noon * 4f;

            sun.intensity = sunIntensity.Evaluate(time);
            moon.intensity = moonIntensity.Evaluate(time);

            sun.color = sunColor.Evaluate(time);
            moon.color = moonColor.Evaluate(time);

            if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
                sun.gameObject.SetActive(false);
            else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
                sun.gameObject.SetActive(true);

            if (moon.intensity == 0 && moon.gameObject.activeInHierarchy)
                moon.gameObject.SetActive(false);
            else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy)
                moon.gameObject.SetActive(true);

            RenderSettings.ambientIntensity = lightningIntensityMultiplier.Evaluate(time);
            RenderSettings.reflectionIntensity = reflectionsIntensityMultiplier.Evaluate(time);
        }
    }
}