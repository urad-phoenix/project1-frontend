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
        int _Rounds;
        public Stage(int id, Team attack, Team defend)
        {
            this.Id = id;
            Attacker = attack;
            Defender = defend;

            _Actors = Attacker.Actors.Union(Defender.Actors).ToArray();
            _ActorCircular = new Game.CircularQueue<Actor>(_Actors);

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
            if (Attacker.IsMember(actor))
                return _GetLocation(Defender, target);
            if (Defender.IsMember(actor))
                return _GetLocation(Attacker, target);
            throw new Exception($"沒有陣營的角色 {actor.Id}");
        }

        private Actor _GetLocation(Team team, Configs.EffectTarget target)
        {
            // todo 依類型取出站位
            return _DefaultLocation(team);
        }

        private static Actor _DefaultLocation(Team team)
        {
            return team.GetSurvivors().First();
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
