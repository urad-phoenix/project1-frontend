namespace Phoenix.Playables
{
    using Phoenix.Playables.Utilities;
    using UnityEngine;
    using UnityEngine.Playables;


    public class StateColorBehaviour : PlayableBehaviour
    {
        Color m_DefaultColor;
        Color m_DefaultShadowColor;
        Color m_DefaultMapColor;
        float m_DefaultEffectGain;
        float m_DefaultDissolveControl;
        Shader m_DefaultShader;
        Shader targetShader;
        Color m_DefaultFresnelColor;
        GameObject m_TrackBinding;
        bool m_FirstFrameHappened;
        Camera FightCamera;
        My_RimGlowObjectCmd m_My_RimGlowObjectCmd;
        My_RimGlowController MyMy_RimGlowController;
        My_GlowComposite MyMy_GlowComposite;
        private Renderer m_Renderer;
        //private SetColorFromCharacterToMap m_SetColorFromCharacterToMap;
        private bool m_IsFirstClipPlay;
        private bool m_IsFinishReset;
        private bool isUseMapColor;
        private bool isUseShadowColor;
        private bool isUseDissolveControl;
        private bool isUseEffectGain;
        private bool isUseTintColor;

        private MaterialPropertyBlock m_Props;

        private bool m_IsEnable = true;
        
        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.

         
        public override void OnGraphStart(Playable playable)
        {
            ScriptPlayable<StateColorBehaviourData> inputPlayable = (ScriptPlayable<StateColorBehaviourData>)playable.GetInput(0);
            StateColorBehaviourData input = inputPlayable.GetBehaviour();
            isUseMapColor = input.UseMapColor;
            isUseShadowColor = input.UseShadowColor;
            isUseDissolveControl = input.UseDissolveControl;
            isUseEffectGain = input.UseEffectGain;
            isUseTintColor = input.UseTintColor;
            targetShader = input.SpShader;
            foreach (var i in Camera.allCameras)
            {
                if (i.gameObject.tag == "FightCamera")
                {
                    FightCamera = i;
                }
            }
            
        }



        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            m_TrackBinding = playerData as GameObject;

            if (!m_TrackBinding)
                return;

            if (!m_FirstFrameHappened)
            {
                m_Props = new MaterialPropertyBlock();
                m_Renderer = m_TrackBinding.GetComponent<Renderer>();
                
                if (m_Renderer != null)
                {
                    m_IsEnable = m_Renderer.enabled;
                    if (!m_Renderer.enabled)
                        m_Renderer.enabled = true;
                    if (isUseTintColor)
                    {
                        m_DefaultColor = m_Renderer.sharedMaterial.GetColor("_TintColor");
                    }
                    else
                    {
                        m_DefaultColor = m_Renderer.sharedMaterial.color;
                    }
                        
                    if (isUseEffectGain) m_DefaultEffectGain = m_Renderer.sharedMaterial.GetFloat("_EffectGain");
                    if (isUseDissolveControl) m_DefaultDissolveControl = m_Renderer.sharedMaterial.GetFloat("_DissolveControl");
                    if (isUseShadowColor) m_DefaultShadowColor = m_Renderer.sharedMaterial.GetColor("_ShadowColor");
                    if (isUseMapColor) m_DefaultMapColor = Shader.GetGlobalColor("_UnifyMapColor");
                    if (targetShader != null)
                    {
                        m_DefaultShader = m_Renderer.sharedMaterial.shader;
                        m_Renderer.material.shader = targetShader;
                        m_DefaultFresnelColor = m_Renderer.material.GetColor("_FresnelColor");
                        //
                        m_My_RimGlowObjectCmd = m_TrackBinding.GetComponent<My_RimGlowObjectCmd>();
                        if (m_My_RimGlowObjectCmd == null) m_My_RimGlowObjectCmd = m_TrackBinding.AddComponent(typeof(My_RimGlowObjectCmd)) as My_RimGlowObjectCmd;

                        MyMy_GlowComposite = FightCamera.gameObject.GetComponent<My_GlowComposite>();
                        MyMy_RimGlowController = FightCamera.gameObject.GetComponent<My_RimGlowController>();



                        if (m_My_RimGlowObjectCmd != null && m_My_RimGlowObjectCmd.enabled == false) { m_My_RimGlowObjectCmd.enabled = true; }
                        if (MyMy_GlowComposite != null && MyMy_GlowComposite.enabled == false) { MyMy_GlowComposite.enabled = true; }
                        if (MyMy_RimGlowController != null && MyMy_RimGlowController.enabled == false) { MyMy_RimGlowController.enabled = true; }
                        
                        //
                    }
                    m_Renderer.SetPropertyBlock(m_Props);
                }
                m_FirstFrameHappened = true;
            }

            int inputCount = playable.GetInputCount();

            Color blendedColor = Color.clear;
            Color blendedColor_s = Color.clear;
            Color blendedColor_m = Color.clear;
            Color blendedColor_f = Color.clear;
            float blendedEffectGain = 0;
            float blendedDissolveControl = 0;

            float totalWeight = 0f;
            float greatestWeight = 0f;
            int currentInputs = 0;

            bool isDisabel = false;
          
            for (int i = 0; i < inputCount; i++)
            {                
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<StateColorBehaviourData> inputPlayable = (ScriptPlayable<StateColorBehaviourData>)playable.GetInput(i);
                StateColorBehaviourData input = inputPlayable.GetBehaviour();
                isUseMapColor = input.UseMapColor;
                blendedColor += input.Color * inputWeight;
                if (isUseShadowColor) blendedColor_s += input.ShadowColor * inputWeight;
                if (isUseMapColor) { blendedColor_m += input.MapColor * inputWeight; }

                if (isUseEffectGain) blendedEffectGain += input.EffectGain * inputWeight;
                if (isUseDissolveControl) blendedDissolveControl += input.DissolveControl * inputWeight;
                if (targetShader != null)blendedColor_f += input.FresnelColor * inputWeight;
                    totalWeight += inputWeight;

                if (inputWeight > greatestWeight)
                {
                    greatestWeight = inputWeight;
                }

                if (!Mathf.Approximately(inputWeight, 0f))
                    currentInputs++;

                if (input.IsFinishReset)
                {
                    m_IsFinishReset = true;
                }
            }

            if (!m_IsFinishReset && totalWeight <= 0)
            {
                return;
            }

            if (isUseTintColor)
            { m_Props.SetColor("_TintColor", blendedColor + m_DefaultColor * (1f - totalWeight)); }
            else { m_Props.SetColor("_Color", blendedColor + m_DefaultColor * (1f - totalWeight)); }
            
            if (isUseShadowColor) m_Props.SetColor("_ShadowColor", blendedColor_s + m_DefaultShadowColor * (1f - totalWeight));
            if(isUseMapColor) Shader.SetGlobalColor("_UnifyMapColor", blendedColor_m + m_DefaultMapColor * (1f - totalWeight));
            if (targetShader != null) m_Renderer.material.SetColor("_FresnelColor", blendedColor_f + m_DefaultFresnelColor * (1f - totalWeight));
            if (isUseEffectGain) m_Props.SetFloat("_EffectGain", blendedEffectGain + m_DefaultEffectGain * (1f - totalWeight));
            if (isUseDissolveControl) m_Props.SetFloat("_DissolveControl", blendedDissolveControl + m_DefaultDissolveControl * (1f - totalWeight));
            m_Renderer.SetPropertyBlock(m_Props);
                       
            //m_TrackBinding.GetComponent<Renderer>().sharedMaterial.color = blendedColor + m_DefaultColor * (1f - totalWeight);

        }
        public override void OnPlayableDestroy(Playable playable)
        {
            m_FirstFrameHappened = false;

            //if (m_TrackBinding == null)
            //  return;
            if (m_Renderer == null)
                return;
            
            if (m_IsFinishReset)
            {
                if (isUseTintColor)
                { m_Props.SetColor("_TintColor", m_DefaultColor); }
                else { m_Props.SetColor("_Color", m_DefaultColor); }
                if (isUseEffectGain) m_Props.SetFloat("_EffectGain", m_DefaultEffectGain);
                if (isUseDissolveControl) m_Props.SetFloat("_DissolveControl", m_DefaultDissolveControl);
                if (isUseShadowColor) m_Props.SetColor("_ShadowColor", m_DefaultShadowColor);
                if (isUseMapColor) Shader.SetGlobalColor("_UnifyMapColor", m_DefaultMapColor);
                if (targetShader != null)
                {
                    m_Renderer.material.shader = m_DefaultShader;
                    m_Renderer.material.SetColor("_FresnelColor", m_DefaultFresnelColor);
                    //
                    if (m_My_RimGlowObjectCmd != null && m_My_RimGlowObjectCmd.enabled == true) { m_My_RimGlowObjectCmd.enabled = false; }
                    if (MyMy_GlowComposite != null && MyMy_GlowComposite.enabled == true) { MyMy_GlowComposite.enabled = false; }
                    if (MyMy_RimGlowController != null && MyMy_RimGlowController.enabled == true) { MyMy_RimGlowController.enabled = false; }
                    

                    //
                }
                m_Renderer.SetPropertyBlock(m_Props);    
                m_Renderer.enabled = m_IsEnable;
                
            }
            
            //m_TrackBinding.GetComponent<Renderer>().sharedMaterial.color = m_DefaultColor;
        }
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_FirstFrameHappened = false;
            //if (m_TrackBinding == null)
            //  return;
            if (m_Renderer == null)
                return;
            if (m_IsFinishReset)
            {
                if (isUseTintColor)
                { m_Props.SetColor("_TintColor", m_DefaultColor); }
                else { m_Props.SetColor("_Color", m_DefaultColor); }
                if (isUseEffectGain) m_Props.SetFloat("_EffectGain", m_DefaultEffectGain);
                if (isUseDissolveControl) m_Props.SetFloat("_DissolveControl", m_DefaultDissolveControl);
                if (isUseShadowColor) m_Props.SetColor("_ShadowColor", m_DefaultShadowColor);
                if (isUseMapColor) Shader.SetGlobalColor("_UnifyMapColor", m_DefaultMapColor);
                if (targetShader != null)
                {
                    m_Renderer.material.shader = m_DefaultShader;
                    m_Renderer.material.SetColor("_FresnelColor", m_DefaultFresnelColor);
                    //
                    if (m_My_RimGlowObjectCmd != null && m_My_RimGlowObjectCmd.enabled == true) { m_My_RimGlowObjectCmd.enabled = false; }
                    if (MyMy_GlowComposite != null && MyMy_GlowComposite.enabled == true) { MyMy_GlowComposite.enabled = false; }
                    if (MyMy_RimGlowController != null && MyMy_RimGlowController.enabled == true) { MyMy_RimGlowController.enabled = false; }
                    

                    //
                }
                m_Renderer.SetPropertyBlock(m_Props);  
                m_Renderer.enabled = m_IsEnable;
                
            }
         
            //m_TrackBinding.GetComponent<Renderer>().sharedMaterial.color = m_DefaultColor;
        }

        public override void OnGraphStop(Playable playable)
        {
            m_FirstFrameHappened = false;
            // if (m_TrackBinding == null)
            if (m_Renderer == null)
                return;

            if (m_IsFinishReset)
            {
                if (isUseTintColor)
                { m_Props.SetColor("_TintColor", m_DefaultColor); }
                else { m_Props.SetColor("_Color", m_DefaultColor); }
                    
                if (isUseEffectGain) m_Props.SetFloat("_EffectGain", m_DefaultEffectGain);
                if (isUseDissolveControl) m_Props.SetFloat("_DissolveControl", m_DefaultDissolveControl);
                if (isUseShadowColor) m_Props.SetColor("_ShadowColor", m_DefaultShadowColor);
                if (isUseMapColor) Shader.SetGlobalColor("_UnifyMapColor", m_DefaultMapColor);
                if (targetShader != null)
                {
                    m_Renderer.material.shader = m_DefaultShader;
                    m_Renderer.material.SetColor("_FresnelColor", m_DefaultFresnelColor);
                    //
                    if (m_My_RimGlowObjectCmd != null && m_My_RimGlowObjectCmd.enabled == true) { m_My_RimGlowObjectCmd.enabled = false; }
                    
                    if (MyMy_GlowComposite != null && MyMy_GlowComposite.enabled == true) { MyMy_GlowComposite.enabled = false; }
                    if (MyMy_RimGlowController != null && MyMy_RimGlowController.enabled == true) { MyMy_RimGlowController.enabled = false; }
                   
                    //
                }
                m_Renderer.SetPropertyBlock(m_Props);   
                m_Renderer.enabled = m_IsEnable;
                
            }
          
            //m_TrackBinding.GetComponent<Renderer>().sharedMaterial.color = m_DefaultColor;
        }
    }
}
