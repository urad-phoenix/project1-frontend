using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Phoenix.Project1.Game
{
   /* public class Combat : ICombat
    {
        private Dictionary<int, Camps> BattleMap;

        public Combat()
        {
            BattleMap = new Dictionary<int, Camps>();
        }

        public Value<BattleInfo> GetCampsByOpponentId(int opponent_id)
        {
            //todo: reading from in game table by opponent id
            var info = new BattleInfo();
            info.BattleId = BattleMap.Count;
            info.Camps = _GetTestBattleTeam();
            BattleMap.Add(info.BattleId, info.Camps);

            var remoteInfo = new Regulus.Remote.Value<BattleInfo>();
            remoteInfo.SetValue(info);
            return remoteInfo;
        }

        public Value<BattleInfo> GetCampsByStageId(int stage_id)
        {
            //todo: reading from in game table by stage id
            var info = new BattleInfo();
            info.BattleId = BattleMap.Count;
            info.Camps = _GetTestBattleTeam();
            BattleMap.Add(info.BattleId, info.Camps);

            var remoteInfo = new Regulus.Remote.Value<BattleInfo>();
            remoteInfo.SetValue(info);
            return remoteInfo;
        }

        public Value<BattleResult> ToFight(int battle_id)
        {
            Camps camps;
            var remoteValue = new Regulus.Remote.Value<BattleResult>();
            if (!BattleMap.TryGetValue(battle_id, out camps))
            {
                var failResult = new BattleResult();
                failResult.Status = BattleStatus.Fail;
                remoteValue.SetValue(failResult);
                return remoteValue;
            }

            var battle = new Battle(camps);
            var result = battle.GetResult();
            remoteValue.SetValue(result);
            return remoteValue;

            //get battle actors from remote
            //new a battle
            //battle init
            //battle start
            //battle end
        }

        private Camps _GetTestBattleTeam()
        {
            Camps camps = new Camps();

            var attackCamp = new List<BattleActor>();
            var attacker1 = new BattleActor();
            attacker1.Id = 1;
            attacker1.Hp.SetCurrent(500);
            attacker1.Atk.SetCurrent(100);
            attackCamp.Add(attacker1);

            var attacker2 = new BattleActor();
            attacker2.Id = 2;
            attacker2.Hp.SetCurrent(666);
            attacker2.Atk.SetCurrent(450);
            attackCamp.Add(attacker2);

            var attacker3 = new BattleActor();
            attacker3.Id = 3;
            attacker3.Hp.SetCurrent(700);
            attacker3.Atk.SetCurrent(600);
            attackCamp.Add(attacker3);

            var attacker4 = new BattleActor();
            attacker4.Id = 4;
            attacker4.Hp.SetCurrent(400);
            attacker4.Atk.SetCurrent(1100);
            attackCamp.Add(attacker4);

            var attacker5 = new BattleActor();
            attacker5.Id = 5;
            attacker5.Hp.SetCurrent(200);
            attacker5.Atk.SetCurrent(1100);
            attackCamp.Add(attacker5);

            var attacker6 = new BattleActor();
            attacker6.Id = 6;
            attacker6.Hp.SetCurrent(560);
            attacker6.Atk.SetCurrent(130);
            attackCamp.Add(attacker6);

            camps.AttackCamp = attackCamp.ToArray();

            var defendCamp = new List<BattleActor>();
            var defender1 = new BattleActor();
            defender1.Id = 1;
            defender1.Hp.SetCurrent(550);
            defender1.Atk.SetCurrent(100);
            defendCamp.Add(defender1);

            var defender2 = new BattleActor();
            defender2.Id = 2;
            defender2.Hp.SetCurrent(660);
            defender2.Atk.SetCurrent(200);
            defendCamp.Add(defender2);

            var defender3 = new BattleActor();
            defender3.Id = 3;
            defender3.Hp.SetCurrent(770);
            defender3.Atk.SetCurrent(600);
            defendCamp.Add(defender3);

            var defender4 = new BattleActor();
            defender4.Id = 4;
            defender4.Hp.SetCurrent(400);
            defender4.Atk.SetCurrent(20);
            defendCamp.Add(defender4);

            var defender5 = new BattleActor();
            defender5.Id = 5;
            defender5.Hp.SetCurrent(230);
            defender5.Atk.SetCurrent(120);
            defendCamp.Add(defender5);

            var defender6 = new BattleActor();
            defender6.Id = 6;
            defender6.Hp.SetCurrent(560);
            defender6.Atk.SetCurrent(130);
            defendCamp.Add(defender6);
            camps.DefendCamp = defendCamp.ToArray();

            return camps;
        }
    }*/
}
