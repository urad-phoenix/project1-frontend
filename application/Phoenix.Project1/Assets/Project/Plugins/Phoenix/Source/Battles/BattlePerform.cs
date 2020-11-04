using Phoenix.Project1.Common.Battles;
using Regulus.Utility;
using System.Linq;
using System.Security.Cryptography;

namespace Phoenix.Project1.Battles
{
    interface IEffectHandler
    {
        void Handle(Actor actor, int value);
    }

    class InjureEffect : IEffectHandler
    {
        void IEffectHandler.Handle(Actor actor, int value)
        {
            actor.Injure(value);
        }
    }
    internal class BattlePerform : IStatus
    {
        System.Collections.Generic.Dictionary<EffectType, IEffectHandler> _EffectHandlers;
        private readonly Actor _Actor;
        private readonly Stage _Stage;
        private readonly IBattleTime _Time;
        
        int _EndFrame;

        private readonly System.Collections.Generic.List<ActorFrameEffect> _Effects;


        public System.Action DoneEvent;
        public System.Action<ActorPerformTimestamp> PerformEvent;
        public BattlePerform(Actor actor, Stage stage,IBattleTime time)
        {
            _Actor = actor;
            this._Stage = stage;
            this._Time = time;
            _Effects = new System.Collections.Generic.List<ActorFrameEffect>();

            _EffectHandlers = new System.Collections.Generic.Dictionary<EffectType, IEffectHandler>();
            _EffectHandlers.Add(EffectType.Injure ,new InjureEffect() );
        }

        void IStatus.Enter()
        {
            _Effects.Clear();
            var startFrame = _Time.Frame;
            Spell spell = _Actor.CastSpell();
            
            int location = spell.GetLocation(_Actor , _Stage);
            var motion = _Actor.GetMotion(spell.Motion);
            _EndFrame = motion.Frames;
            for (int i = 0; i < motion.HitFrames.Length; i++)
            {
                var hitFrame = motion.HitFrames[i];
                var effects = spell.CreateEffects(_Actor, _Stage);

                _Effects.Add(new ActorFrameEffect { Frames = hitFrame + startFrame, Effects = effects.ToArray() } );
            }

            ActorPerformTimestamp timestamp = new ActorPerformTimestamp()
            {
                Frames = startFrame,
                ActorPerform = new ActorPerform() { 
                    StarringId = _Actor.Id.Value, SpellId = spell.Id, 
                    Location = location, 
                    TargetEffects = _Effects.ToArray() }
            };
            PerformEvent(timestamp);
        }

        void IStatus.Leave()
        {
            
        }

        void IStatus.Update()
        {
            var frames = _Time.Advance();

            for (int i = 0; i < frames; i++)
            {
                _Advance();
            }
        }

        private void _Advance()
        {            
            _HitEffect(_CheckHitFrame());
            if (_CheckEndFrame())
            {
                DoneEvent();
            }
        }

        private void _HitEffect(System.Collections.Generic.IEnumerable<Effect> effects)
        {
            var gEffects = from e in effects
                           group e by e.Type into g
                            select new { EffectType = g.Key , Effects = g.AsEnumerable() };


            foreach (var item in gEffects)
            {
                var handler = _EffectHandlers[item.EffectType];
                foreach (var effect in item.Effects)
                {
                    handler.Handle(_GetAcror(effect.Actor) , effect.Value);
                }
            }

        }

        private Actor _GetAcror(int actor)
        {
            return _Stage.GetActor(actor);
        }

        private bool _CheckEndFrame()
        {
            if (_EndFrame == 0)
                return true;
            _EndFrame--;
            return false;
        }

        private System.Collections.Generic.IEnumerable<Effect> _CheckHitFrame()
        {
            var now = _Time.Frame;
            var frameEffects = from e in _Effects
                          where e.Frames == now
                          select e;

            foreach (var frameEffect in frameEffects)
            {
                foreach (var effect in frameEffect.Effects)
                {
                    yield return effect;
                }
            }
        }
    }
}