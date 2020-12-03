using System;
using System.Linq;
using Phoenix.Playables;
using Phoenix.Project1.Common.Battles;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleActState : BattleStateBase
    {
        private ActorFrameMotion _ActData;
        
        private BattleController _Controller;
               
        private CompositeDisposable _Disposable;
        
        public BattleActState(string name, BattleStateMachine stateMachine, ActorFrameMotion actData, BattleController controller) : base(name, stateMachine)
        {
            _ActData = actData;
                     
            _Disposable = new CompositeDisposable();
            
            _Controller = controller;          
        }

        public override void Start()
        {
            if (_ActData == null)
            {
                _Finished(null);
                return;
            }

            try
            {
                var motion = _ActData.MotionId;
                
                var director = _Controller.GetPlayableDirector(motion, _ActData.ActorId);

                Debug.Log($"act key {motion}");
                
                if (director == null)
                {
                    _Finished(null);
                    return;
                }

                var tracks = ((TimelineAsset) director.playableAsset).GetOutputTracks();         

                foreach (var track in tracks)
                {
                    if (track is SpineAnimationTrack)
                    {                       
                        TimelineBinding.BindSpineTrack(director, _ActData, track, _Controller);    
                    }
                    else if (track is VFXTrack)
                    {
                        TimelineBinding.BindVFXTrack(director, _ActData, track, _Controller);
                    }
                    else if (track is CameraShotTrack)
                    {
                        TimelineBinding.BindCameraTrack(director, _ActData, track, _Controller);
                    }
                    else if(track is AudioTrack)
                    {
                        TimelineBinding.BindAudioTrack(director, track, _Controller);
                    }
                    else if(track is TransformTweenTrack)
                    {                                    
                        TimelineBinding.BindTransformTrack(director, _ActData, track, _Controller);                        
                    }
                }
            
                director.PlayAsObservable().Subscribe(_Finished).AddTo(_Disposable);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                _Finished(null);
                
                return;
            }            
        }
       
        public override void Update()
        {         
        }

        public override void Dispose()
        {
            _Disposable.Clear();
        }

        private void _Finished(PlayableDirector director)
        {
            if (director)
            {
                _Controller.RecyclePlayableDirector(director);                
            }

            _SwitchState();
        }
    }
}