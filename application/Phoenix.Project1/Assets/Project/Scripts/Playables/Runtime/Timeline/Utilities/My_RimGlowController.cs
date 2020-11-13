
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace Phoenix.Playables.Utilities
{
        /// <summary>
        /// Creates and maintains a command buffer to set up the textures used in the glowing object image effect.
        /// </summary>
    public class My_RimGlowController : MonoBehaviour
    {
        public Shader P_MyGlowCmdShader;
        public Shader P_MyBlur;
        private static My_RimGlowController _instance;

        private CommandBuffer _commandBuffer;

        private List<My_RimGlowObjectCmd> _glowableObjects = new List<My_RimGlowObjectCmd>();
        //
        private Material _glowMat;


        private Material _blurMaterial;
        private Vector2 _blurTexelSize;
        int screenCopyID;

        private int _prePassRenderTexID;
        private int _blurPassRenderTexID;
        private int _tempRenderTexID;
        private int _blurSizeID;
        private int FresnelColorID;
        private int FresnelAmountID;
        private int FresnelThresholdID;

        private int FresnelCoverAmountID;
        private int FresnelCoverFrequencyID;

        public float BlurSize = 3.0f;
        ///add from glowcomposite.cs
        //[Range(0, 10)]
        //public float Intensity = 2;

        // private Material _compositeMat;
        ///
        /// <summary>
        /// On Awake, we cache various values and setup our command buffer to be called Before Image Effects.
        /// </summary>
        private void Awake()
        {
            _instance = this;
            if(P_MyGlowCmdShader != null)
            {
                _glowMat = new Material(P_MyGlowCmdShader);
            }
            if(P_MyBlur != null)
            {
                _blurMaterial = new Material(P_MyBlur);
            }
            screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");

            _prePassRenderTexID = Shader.PropertyToID("_GlowPrePassTex");
            _blurPassRenderTexID = Shader.PropertyToID("_GlowBlurredTex");
            _tempRenderTexID = Shader.PropertyToID("_TempTex0");
            _blurSizeID = Shader.PropertyToID("_BlurSize");
            FresnelColorID = Shader.PropertyToID("My_FresnelColor");


            FresnelAmountID = Shader.PropertyToID("My_FresnelAmount");

            FresnelThresholdID = Shader.PropertyToID("My_FresnelThreshold");
            FresnelCoverAmountID = Shader.PropertyToID("My_FresnelCoverAmount");
            FresnelCoverFrequencyID = Shader.PropertyToID("My_FresnelCoverFrequency");

            _commandBuffer = new CommandBuffer();
            _commandBuffer.name = "Glowing Objects Buffer"; // This name is visible in the Frame Debugger, so make it a descriptive!
            GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeImageEffects, _commandBuffer);


        }
        ///add from glowcomposite.cs
        void OnDisable()
        {
            if(_commandBuffer != null) _commandBuffer.Clear();

        }

        // void OnRenderImage(RenderTexture src, RenderTexture dst)
        //  {
        //     _compositeMat.SetFloat("_Intensity", Intensity);
        //   Graphics.Blit(src, dst, _compositeMat, 0);
        //}
        ///
        /// <summary>
        /// TODO: Add a degister method.
        /// </summary>
        public static void RegisterObject(My_RimGlowObjectCmd glowObj)
        {
            if(_instance != null)
            {
                _instance._glowableObjects.Add(glowObj);
            }
        }

        /// <summary>
        /// Adds all the commands, in order, we want our command buffer to execute.
        /// Similar to calling sequential rendering methods insde of OnRenderImage().
        /// </summary>
        private void RebuildCommandBuffer()
        {
            _commandBuffer.Clear();
            //
            //_commandBuffer.GetTemporaryRT(screenCopyID, Screen.width, Screen.height, 0, FilterMode.Bilinear);
            //
            _commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenCopyID);
            _commandBuffer.GetTemporaryRT(_prePassRenderTexID, Screen.width, Screen.height, 16, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            _commandBuffer.SetRenderTarget(_prePassRenderTexID);
            _commandBuffer.ClearRenderTarget(true, true, Color.clear);

            //print(string.Format("glowable obj count: {0}", _glowableObjects.Count));
            for(int i = 0; i < _glowableObjects.Count; i++)
            {
                if(_glowableObjects[i].GetComponent<My_RimGlowObjectCmd>().enabled == true)
                {
                    //_commandBuffer.SetGlobalColor(_glowColorID, _glowableObjects[i].CurrentColor);
                    _commandBuffer.SetGlobalColor(FresnelColorID, _glowableObjects[i].gameObject.GetComponent<Renderer>().material.GetColor("_FresnelColor"));

                    _commandBuffer.SetGlobalFloat(FresnelAmountID, _glowableObjects[i].gameObject.GetComponent<Renderer>().material.GetFloat("_FresnelAmount"));
                    _commandBuffer.SetGlobalFloat(FresnelThresholdID, _glowableObjects[i].gameObject.GetComponent<Renderer>().material.GetFloat("_FresnelThreshold"));
                    _commandBuffer.SetGlobalFloat(FresnelCoverAmountID, _glowableObjects[i].gameObject.GetComponent<Renderer>().material.GetFloat("_FresnelCoverAmount"));
                    _commandBuffer.SetGlobalFloat(FresnelCoverFrequencyID, _glowableObjects[i].gameObject.GetComponent<Renderer>().material.GetFloat("_FresnelCoverFrequency"));
                    for(int j = 0; j < _glowableObjects[i].Renderers.Length; j++)
                    {
                        //print(string.Format("{0} length: {1}", _glowableObjects[i].name, _glowableObjects[i].Renderers.Length));

                        _commandBuffer.DrawRenderer(_glowableObjects[i].Renderers[j], _glowMat);
                    }
                }
            }


            _commandBuffer.GetTemporaryRT(_blurPassRenderTexID, Screen.width, Screen.height, 0, FilterMode.Bilinear);
            _commandBuffer.GetTemporaryRT(_tempRenderTexID, Screen.width, Screen.height, 0, FilterMode.Bilinear);
            _commandBuffer.Blit(_prePassRenderTexID, _blurPassRenderTexID);

            _blurTexelSize = new Vector2(BlurSize / (Screen.width), BlurSize / (Screen.height));
            _commandBuffer.SetGlobalVector(_blurSizeID, _blurTexelSize);

            for(int i = 0; i < 4; i++)
            {
                _commandBuffer.Blit(_blurPassRenderTexID, _tempRenderTexID, _blurMaterial, 0);
                _commandBuffer.Blit(_tempRenderTexID, _blurPassRenderTexID, _blurMaterial, 1);
            }


        }

        /// <summary>
        /// Rebuild the Command Buffer each frame to account for changes in color.
        /// This could be improved to only rebuild when necessary when colors are changing.
        /// 
        /// Could be further optimized to not include objects which are currently black and not
        /// affect thing the glow image.
        /// </summary>
        private void Update()
        {
            RebuildCommandBuffer();
        }

    }

}