using System.Linq;
using Phoenix.Playables;
using UniRx;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Client.Battles
{
    public class MoveState : BattleStateBase
    {
        private MoveData _Data;
     
        private BattleController _Controller;
        
        private CompositeDisposable _Disposable;              
        
        public MoveState(string name, BattleStateMachine stateMachine, MoveData data, BattleController controller) : base(name, stateMachine)
        {            
            _Disposable = new CompositeDisposable();
            
            _Data = data;                        

            _Controller = controller;
        }

        public override void Start()
        {
            if (_Data == null)
            {
                _Finished(null);
                return;
            }

            var director = _Controller.GetPlayableDirector(ActionKey.Move, _Data.MoveActorId);           

            if (director == null)
            {
                _Finished(null);
                return;
            }
            
            var tracks = from outputTrack in ((TimelineAsset) director.playableAsset).GetOutputTracks()
                where outputTrack is TransformTweenTrack
                select outputTrack as TransformTweenTrack;
            
            var trackBinding = new TransformBinding();
            
            trackBinding.Bind(director, _Data, tracks, _Controller);
            
            var obs = director.PlayAsObservable();

            obs.Subscribe(_Finished).AddTo(_Disposable);
        }

        public override void Stop()
        {
            
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
            if(director && _Controller)
                _Controller.RecyclePlayableDirector(director);                       
            
            _SwitchState();
        }
    }
}