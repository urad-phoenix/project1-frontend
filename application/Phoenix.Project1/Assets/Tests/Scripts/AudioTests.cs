using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Phoenix.Project1.Client.Audio;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    public class AudioTests : MonoBehaviour
    {
        public AudioClip TestClip;

        public AudioClip[] TestBackground;

        public Slider MusicVolume;
        
        public Slider SoundVolume;

        private int _Index;

        private void Start()
        {
            var mObs = MusicVolume.ObserveEveryValueChanged(slider => _ChangeSlider(slider));

            mObs.Subscribe(_ChangeMusicVolume).AddTo(gameObject);
                        
            var sObs = SoundVolume.ObserveEveryValueChanged(slider => _ChangeSlider(slider));

            sObs.Subscribe(_ChangeSoundVolume).AddTo(gameObject);
        }       

        private float _ChangeSlider(Slider slider)
        {
            return slider.value;
        }

        private void _ChangeMusicVolume(float value)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }

        private void _ChangeSoundVolume(float value)
        {
            AudioManager.Instance.SetSoundVolume(value);
        }

        public void TestPlaySound()
        {
            AudioManager.Instance.PlaySound(TestClip);
        }

        public void TestPlayMusic()
        {         
            AudioManager.Instance.PlayMusic(TestBackground[_Index], true, 0.5f, 1.0f);
            
            _Index = (_Index + 1) % TestBackground.Length;
                     
            Debug.Log(_Index);
        }
    }
}