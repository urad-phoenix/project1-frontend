using System;
using System.Linq;

namespace Phoenix.Project1.Game
{
    namespace Battles
    {
        public class BattleActor
        { 
            public int Id;
            public int Atk;
            public int Hp;

            public int Damage(BattleActor defender)
            {
                var realDamage = defender.TakeDamage(Atk);
                return realDamage;
            }

            public int TakeDamage(int other_atk)
            {
                Hp = Hp - other_atk;
                return other_atk;
            }

            public bool IsDead()
            {
                return Hp <= 0;
            }

            public BattleActor PickDefender(BattleActor[] defend_team)
            {
                //ToDo 依照 ability 挑選攻擊目標
                var target = from actor in defend_team
                             where actor.IsDead() == false
                             select actor;
                return target.FirstOrDefault();
            }

            public int PickAbility()
            {
                var rnd = new Random();
                return rnd.Next(1, 4);
            }
        }

        public class BattleActors
        {
            public BattleActor[] AttackTeam;
            public BattleActor[] DefendTeam;

            public BattleActors()
            {
                AttackTeam = new BattleActor[0];
                DefendTeam = new BattleActor[0];
            }
        }
    }
}
