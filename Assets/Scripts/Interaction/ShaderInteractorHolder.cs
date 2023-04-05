using System;
using UnityEngine;

namespace ZenvaSurvival
{
    [ExecuteInEditMode]
    public class ShaderInteractorHolder : MonoBehaviour
    {
        private ShaderInteractorPosition[] interactors;
        private Vector4[] positions = new Vector4[100];
        private float[] radiuses = new float[100];
        private Vector4[] boxBounds = new Vector4[100];
        private Vector4[] rotations = new Vector4[100];
        
        [Range(0,1)]
        public float ShapeCutoff;

        [Range(0, 1)] public float ShapeSmoothness = .1f;
        
        private void Start()
        {
            FindInteractors();
        }

        private void OnEnable()
        {
            FindInteractors();
        }

        void FindInteractors()
        {
            interactors = FindObjectsOfType<ShaderInteractorPosition>();
        }

        private void Update()
        {
            for (int i = 0; i < interactors.Length; i++)
            {
                positions[i] = interactors[i].transform.position;
                radiuses[i] = interactors[i].radius;
                boxBounds[i] = interactors[i].transform.localScale;
                rotations[i] = interactors[i].transform.eulerAngles;
            }
            
            Shader.SetGlobalVectorArray("_ShaderInteractorsPositions", positions);
            Shader.SetGlobalFloatArray("_ShaderInteractorsRadiuses", radiuses);
            Shader.SetGlobalVectorArray("_ShaderInteractorsBoxBounds", boxBounds);
            Shader.SetGlobalVectorArray("_ShaderInteractorRotation", rotations);
            
            Shader.SetGlobalFloat("_ShapeCutOff", ShapeCutoff);
            Shader.SetGlobalFloat("_ShapeSmoothness", ShapeSmoothness);
        }
    }
}