using Phoenix.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        static private AudioManager _Instance;

        private AudioSource _MusicSource;

        [SerializeField]
        [Range(0f, 1f)]
        private float _VoiseVolume = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _SoundVolume = 1f;
        
        [SerializeField]
        [Range(0f, 1f)]
        private float _MusicVolume = 1f;

        private bool _IsChangingClip = false;

        private Coroutine fadingCoroutine = null;

        private ObjectPool _ObjectPool;

        List<AudioSource> _AudioSources;

        List<AudioSource> _VoiseSources;

        private string _PoolName = "AudioPool";        

        static public AudioManager Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = FindObjectOfType<AudioManager>();

                    DontDestroyOnLoad(_Instance);
                }
                return _Instance;
            }
        }        

        void Start()
        {
            _MusicSource = GetComponent<AudioSource>();

            if(_MusicSource == null)
            {
                InitAudioSource();
            }         

            var audioObject = new GameObject("AutioSource");
                                  
            _ObjectPool = PoolManager.Instance.AddPool(new ObjectPool(_PoolName, audioObject, this.transform, 10));

            _ObjectPool.OnAfterSpawn += _AfterSpawn;

            _AudioSources = new List<AudioSource>();

            _VoiseSources = new List<AudioSource>();
        }

        private void _AfterSpawn(GameObject go)
        {
            go.AddComponent<AudioSource>();            
        }


        private void InitAudioSource()
        {
            _MusicSource = this.gameObject.AddComponent<AudioSource>();
            _MusicSource.volume = 1;
            _MusicSource.loop = true;
        }

        public void PlayMusic(AudioClip newClip, bool isLoop, float fadeInTime = 0, float fadeOutTime = 0)
        {
            // if same music and still playing, skip
            if(_MusicSource != null && _MusicSource.isPlaying && _MusicSource.clip == newClip)
                return;

            //should use coroutine
            if(_IsChangingClip)
            {
                StopCoroutine(fadingCoroutine);
                _MusicSource.volume = _MusicVolume;
            }

            if(_MusicVolume <= 0.00f)
            {
                _MusicSource.clip = newClip;
                _MusicSource.volume = _MusicVolume;
                _MusicSource.Play();
                _MusicSource.loop = isLoop;              
                return;
            }

            fadingCoroutine = StartCoroutine(Fading(newClip, fadeInTime, fadeOutTime));

            _IsChangingClip = true;
        }

        IEnumerator Fading(AudioClip newClip, float fadeInTime, float fadeOutTime)
        {
            while(_MusicSource == null)
            {
                InitAudioSource();
                yield return null;
            }

            float volumeStep = _MusicSource.volume / fadeOutTime;
            while(_MusicSource.volume > 0)
            {
                _MusicSource.volume -= volumeStep * Time.deltaTime;
                yield return null;
            }
            _MusicSource.Stop();

            _MusicSource.clip = newClip;

            if(newClip == null)
            {
                _MusicSource.volume = _MusicVolume;
            }
            else
            {
                volumeStep = _MusicVolume / fadeInTime;
                _MusicSource.Play();
                while(_MusicSource.volume < _MusicVolume)
                {
                    _MusicSource.volume += volumeStep * Time.deltaTime;
                    yield return null;
                }
            }
            _IsChangingClip = false;
        }

        public void MusicPlay()
        {
            if(_MusicSource.isPlaying == false)
            {
                _MusicSource.Play();
            }
        }

        public void MusicStop()
        {
            _MusicSource.Stop();
        }

        public void MusicPause()
        {
            if(_MusicSource.isPlaying)
            {
                _MusicSource.Pause();
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if(clip == null)
            {
                Debug.LogWarning("[PlaySoundFx] sound clip not assigned");
                return;
            }

            var obj = PoolManager.Instance.GetObject<GameObject>(_PoolName);            
            var audio = obj.GetComponent<AudioSource>();
            audio.volume = _SoundVolume;
            audio.playOnAwake = false;
            audio.clip = clip;

            _AudioSources.Add(audio);

            var observable = audio.PlayAsObserver();

            observable.Subscribe(ob =>
            {
                _AudioSources.Remove(ob);

                PoolManager.Instance.Recycle<GameObject>(_PoolName, ob.gameObject);
            });
        }

        public void PlaySoundLoop(AudioClip clip)
        {
            if(clip == null)
            {
                Debug.LogWarning("[PlaySoundLoop] sound clip not assigned");
                return;
            }

            var obj = PoolManager.Instance.GetObject<GameObject>(_PoolName);

            var audio = obj.GetComponent<AudioSource>();

            audio.playOnAwake = false;
            audio.clip = clip;
            audio.volume = _SoundVolume;
            audio.loop = true;

            _AudioSources.Add(audio);

            var observable = audio.PlayAsObserver();
            observable.Subscribe(ob =>
            {
                _AudioSources.Remove(ob);

                PoolManager.Instance.Recycle<GameObject>(_PoolName, ob.gameObject);
            });                       
        }

        public void PlayVoise(AudioClip clip)
        {            
            if(clip == null)
            {
                Debug.LogWarning("[PlaySoundLoop] sound clip not assigned");
                return;
            }

            var obj = PoolManager.Instance.GetObject<GameObject>(_PoolName);

            var audio = obj.GetComponent<AudioSource>();

            audio.playOnAwake = false;
            audio.clip = clip;
            audio.volume = _VoiseVolume;

            _VoiseSources.Add(audio);

            var observable = audio.PlayAsObserver();

            observable.Subscribe(ob =>
            {
                _VoiseSources.Remove(ob);

                PoolManager.Instance.Recycle<GameObject>(_PoolName, ob.gameObject);
            });
        }

        public void SetMusicVolume(float volume)
        {
            _MusicVolume = volume;

            if(_MusicSource == null)
                return;
            
            _MusicSource.volume = _MusicVolume;
        }       

        public void SetSoundVolume(float volume)
        {
            _SoundVolume = volume;

            if(_AudioSources != null)
            {
                for(int i = 0; i < _AudioSources.Count; ++i)
                {
                    if(_AudioSources[i] == null)
                        continue;

                    _AudioSources[i].volume = volume;
                }
            }
        }
        public void SetVoiseVolume(float volume)
        {
            _VoiseVolume = volume;

            if(_VoiseSources != null)
            {
                for(int i = 0; i < _VoiseSources.Count; ++i)
                {
                    if(_VoiseSources[i] == null)
                        continue;

                    _VoiseSources[i].volume = volume;
                }
            }
        }
        
    }

    public static class AudioRx
    {                
        public static IObservable<AudioSource> PlayAsObserver(this AudioSource audio)
        {
            audio.Play();

            return Observable.Create<AudioSource>(ob =>
            {
                Observable.EveryUpdate().Subscribe(u =>
                {
                    if (!audio.isPlaying)
                    {
                        ob.OnNext(audio);
                        ob.OnCompleted();                        
                    }                    
                }).AddTo(audio);
                
                return Disposable.Empty;
            });
        }        
    }
}