﻿using Phoenix.Project1.Common.Battles;
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
            for (int i = 0; i < motionCast.Hits.Length; i++)
            {
                
                var hitFrame = motionCast.Hits[i];
                var startHitFrame = castStart + hitFrame.Frame;
                _Timer.Register(startHitFrame, () => cast.Occurrence(_Stage));
                effects.Add(new ActorFrameEffect() { Frames = startHitFrame, Effects = cast.ToEffects().ToArray() });
            }

            ActorPerformTimestamp timestamp = new ActorPerformTimestamp()
            {
                Frames = startFrame,
                ActorPerform = new ActorPerform(_Actor.Id.Value , cast.Location, "Move", "Back" , "Caster" , effects.ToArray()) 
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