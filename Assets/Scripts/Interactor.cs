using System;
using UnityEngine;

namespace ZenvaSurvival
{
    public class Interactor : MonoBehaviour
    {
        public float radius;
        private void Update()
        {
            Shader.SetGlobalVector("_Position", transform.position);
            Shader.SetGlobalFloat("_Radius", radius);
        }
    }
}