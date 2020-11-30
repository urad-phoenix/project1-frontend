using Regulus.Remote;
using System;
using System.Linq;

namespace Phoenix.Project1.Battles
{
    public class Stage 
    {
        public readonly int Id;
        public readonly Team Attacker;
        public readonly Team Defender;

        readonly Actor[] _Actors;
        readonly Game.CircularQueue<Actor> _ActorCircular;
        readonly LocationCalculator _LocationCalculator;
        int _Rounds;
        public Stage(int id, Team attack, Team defend)
        {
            this.Id = id;
            Attacker = attack;
            Defender = defend;

            _Actors = Attacker.Actors.Union(Defender.Actors).OrderBy(a => a, new ActorComparer()).ToArray();
            _ActorCircular = new Game.CircularQueue<Actor>(_Actors);
            _LocationCalculator = new LocationCalculator(Common.Battles.Configs.StageSetting.YCount);
        }

        internal bool CheckFinish()
        {
            return _Rounds++ > 999;
        }

        public Actor NextPerformer()
        {
            Actor actor;
            do
            {
                actor = _ActorCircular.GetCurrentAndNext();
            }
            while (!actor.IsMovable());

            return actor;
        }
        

        internal Actor[] GetActors()
        {
            return _Actors;
        }

        public Actor GetActor(int actor)  
        {
            return _Actors.Where(a => a.Id == actor).Single();
        }

        internal Actor GetLocation(Actor actor, Configs.EffectTarget target)
        {

            // todo 混亂應該也是在這找
            if (Attacker.IsMember(actor))
                return _GetLocation(actor,Defender, Attacker, target);
            if (Defender.IsMember(actor))
                return _GetLocation(actor,Attacker, Defender, target);
            throw new Exception($"沒有陣營的角色 {actor.Id}");
        }

        private Actor _GetLocation(Actor actor,Team defender, Team attacker, Configs.EffectTarget target)
        {
            var targetActor = _FindWithTarget(actor, defender, attacker, target);
            if(targetActor == null || !targetActor.IsDamageable())
                return _DefaultLocation(defender);
            return targetActor;
        }
        Actor _FindWithTarget(Actor actor, Team defender, Team attacker, Configs.EffectTarget target)
        {
            // todo 依類型取出站位
            if (target.Id == 10001)
                return _FrontSignle(actor, attacker, defender);
            if (target.Id == 10002)
                return _BackSignle(actor, attacker, defender);
            return null;
        }
        private Actor _BackSignle(Actor actor, Team attacker, Team defender)
        {
            var actorLocation = actor.Location.Value;
            var location = _LocationCalculator.BackSignle(actorLocation);            
            

            var locations =new[] { location }.Union(_LocationCalculator.BackMultiple(actorLocation));
            var targets =   from l in locations
                            let t = defender.FindActorByLocation(l)
                            where t != null && t.IsDamageable()
                            select t;
            return targets.FirstOrDefault();
        }

        private Actor _FrontSignle(Actor actor, Team attacker, Team defender)
        {            
            var location = _LocationCalculator.FrontSignle(actor.Location.Value);
            var target = defender.FindActorByLocation(location);
            if(target!=null)
                return target;
            return null;
        }
        private static Actor _DefaultLocation(Team team)
        {
            return team.GetDamageables().First();
        }

        internal System.Collections.Generic.IEnumerable<Actor> GetActorsByEffectTarget(Actor actor, int location, Configs.EffectTarget target)
        {

            // todo 依類型取出影響的角色
            if (Attacker.IsMember(actor))
                yield return _DefaultLocation(Defender);
            if (Defender.IsMember(actor))
                yield return _DefaultLocation(Attacker);
            throw new Exception($"沒有陣營的角色 {actor.Id}");

        }
    }
}

