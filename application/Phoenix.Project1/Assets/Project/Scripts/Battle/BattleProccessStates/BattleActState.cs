using System.Linq;
using Phoenix.Playables;
using UniRx;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleActState : BattleStateBase
    {
        private ActData _ActData;
        
        private BattleController _Controller;
               
        private CompositeDisposable _Disposable;
        
        public BattleActState(string name, BattleStateMachine stateMachine, ActData actData, BattleController controller) : base(name, stateMachine)
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
                                   
            var director = _Controller.GetPlayableDirector(_ActData.ActKey, _ActData.ActorId);

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
                    var spineBinding = new SpineAnimationBinding();
            
                    spineBinding.Bind(director, _ActData, track, _Controller);    
                }
                else if (track is VFXTrack)
                {
                    var vfxBinding = new VFXBinding();
            
                    vfxBinding.Bind(director, _ActData, track, _Controller);
                }
                else if (track is CameraShotTrack)
                {
                    var cameraBinding = new CameraBinding();
                    
                    cameraBinding.Bind(director, _ActData, track, _Controller);
                }
            }                  
            
            director.PlayAsObservable().Subscribe(_Finished).AddTo(_Disposable);

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
            if(director)
                _Controller.RecyclePlayableDirector(director);  
            
            _SwitchState();
        }
    }
}