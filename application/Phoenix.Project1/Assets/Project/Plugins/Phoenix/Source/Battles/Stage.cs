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

        public Stage(int id, Team attack, Team defend)
        {
            this.Id = id;
            Attacker = attack;
            Defender = defend;

            _Actors = Attacker.Actors.Union(Defender.Actors).ToArray();
            _ActorCircular = new Game.CircularQueue<Actor>(_Actors);

        }

        public Actor NextPerformer()
        {
            Actor actor;
            do
            {
                actor = _ActorCircular.GetCurrentAndNext();
            }
            while (actor.IsMovable());

            return actor;
        }
        public static Stage GetDemo()
        {
            var attacker = new Actor(1, 2, 100);
            var defender = new Actor(2, 8, 100);
            var aTeam = new Team(attacker);
            var dTeam = new Team(defender);
            var stage = new Stage(1, aTeam, dTeam);
            return stage;
        }

        public Actor GetActor(int actor)  
        {
            return _Actors.Where(a => a.Id == actor).Single();
        }

        public static Stage GetAttackWin()
        {
            var attacker = new Actor(1, 2, 10);
            var defender = new Actor(2, 8, 0);
            var aTeam = new Team(attacker);
            var dTeam = new Team(defender);
            var stage = new Stage(1, aTeam, dTeam);
            return stage;
        }
    }
}
