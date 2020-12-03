using Phoenix.Project1.Common.Battles;
using Regulus.Utility;
using System.Linq;
using System.Security.Cryptography;

namespace Phoenix.Project1.Battles
{
    
    
    internal class BattlePerform : IStatus
    {
        
        private readonly Actor _Actor;
        private readonly Stage _Stage;
        private readonly IBattleTime _Time;
        
        int _EndFrames;
        int _CurrentFrames;

        public System.Action DoneEvent;
        public System.Action<ActorPerformTimestamp> PerformEvent;

        readonly FrameTimer _Timer;
        public BattlePerform(Actor actor, Stage stage,IBattleTime time)
        {
            _Actor = actor;
            this._Stage = stage;
            this._Time = time;
            _Timer = new FrameTimer();
        }

        void IStatus.Enter()
        {
            var startFrame = _Time.Frame;
            Spell spell = _Actor.CastSpell();            
            var cast = spell.CreateCast(_Actor, _Stage);

            var motionCast = _Actor.GetMotion(spell.Motion);
            var motionForward = _Actor.GetMotion(MotionType.Forward);
            var motionBack = _Actor.GetMotion(MotionType.Back);

            _EndFrames = motionBack.TotalFrame +  motionForward.TotalFrame + motionCast.TotalFrame + startFrame;
            _CurrentFrames = startFrame;

            var castStart = motionForward.TotalFrame + startFrame;

            var effects = new System.Collections.Generic.List<ActorFrameEffect>();

            var hitEffects = cast.ToEffects().ToArray();
            for (int i = 0; i < motionCast.Hits.Length; i++)
            {                
                var hitFrame = motionCast.Hits[i];
                var startHitFrame = castStart + hitFrame.Frame;

                _Timer.Register(startHitFrame, () => cast.Occurrence(_Stage));
                effects.Add(new ActorFrameEffect() { Frames = startHitFrame, Effects = hitEffects });
            }

            if(effects.Count == 0)
            {

                Regulus.Utility.Log.Instance.WriteInfo($"The motion {motionCast.Key} has no hit point.");
                _Timer.Register(castStart, () => cast.Occurrence(_Stage));
                effects.Add(new ActorFrameEffect() { Frames = castStart, Effects = hitEffects });
            }
                

            ActorPerformTimestamp timestamp = new ActorPerformTimestamp()
            {
                Frames = startFrame,
                ActorPerform = new ActorPerform(effects.ToArray() , new ActorFrameMotion() {
                    ActorId = _Actor.Id , 
                    StartFrames = startFrame , 
                    EndFrames = startFrame + motionForward.TotalFrame ,
                    MotionId = MotionType.Forward,
                    TargetLocation = cast.Location
                } , new ActorFrameMotion()
                {
                    ActorId = _Actor.Id,
                    StartFrames = startFrame + motionForward.TotalFrame + motionCast.TotalFrame,
                    EndFrames = startFrame + motionForward.TotalFrame + motionCast.TotalFrame + motionBack.TotalFrame,
                    MotionId = MotionType.Back,
                    TargetLocation = _Actor.Location
                }, new ActorFrameMotion()
                {
                    ActorId = _Actor.Id,
                    StartFrames = startFrame + motionForward.TotalFrame ,
                    EndFrames = startFrame + motionForward.TotalFrame + motionCast.TotalFrame,
                    MotionId = MotionType.Cast1,
                    TargetLocation = cast.Location
                })
            };
            PerformEvent(timestamp);
        }

        

        

        void IStatus.Leave()
        {
            
        }

        void IStatus.Update()
        {
            var frames = _Time.Advance();

            bool done = false;
            for (int i = 0; i < frames; i++)
            {
                if (_Advance())
                {
                    continue;                    
                }

                done = true;
                break;
            }
            if (done)
                DoneEvent();
        }

        private bool _Advance()
        {
            _CurrentFrames++;
            _Timer.Run(_CurrentFrames);
            if (_CheckEndFrame())
            {                
                return false;
            }
            return true;
        }

        private bool _CheckEndFrame()
        {
            return _CurrentFrames >= _EndFrames;
        }

       
    }
}