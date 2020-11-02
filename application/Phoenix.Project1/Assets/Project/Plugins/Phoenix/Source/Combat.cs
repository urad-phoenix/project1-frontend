using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Phoenix.Project1.Game
{
    public class Combat : ICombat
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
            Camps teams = new Camps();

            var attackTeam = new List<BattleActor>();
            var attacker1 = new BattleActor();
            attacker1.Id = 1;
            attacker1.CombatId = 0;
            attacker1.Hp = 500;
            attacker1.Atk = 100;
            attackTeam.Add(attacker1);

            var attacker2 = new BattleActor();
            attacker2.Id = 2;
            attacker2.CombatId = 1;
            attacker2.Hp = 666;
            attacker2.Atk = 450;
            attackTeam.Add(attacker2);

            var attacker3 = new BattleActor();
            attacker3.Id = 3;
            attacker3.CombatId = 2;
            attacker3.Hp = 700;
            attacker3.Atk = 600;
            attackTeam.Add(attacker3);

            var attacker4 = new BattleActor();
            attacker4.Id = 4;
            attacker4.CombatId = 3;
            attacker4.Hp = 400;
            attacker4.Atk = 1100;
            attackTeam.Add(attacker4);

            var attacker5 = new BattleActor();
            attacker5.Id = 5;
            attacker5.CombatId = 4;
            attacker5.Hp = 200;
            attacker5.Atk = 1100;
            attackTeam.Add(attacker5);

            var attacker6 = new BattleActor();
            attacker6.Id = 6;
            attacker6.CombatId = 5;
            attacker6.Hp = 560;
            attacker6.Atk = 130;
            attackTeam.Add(attacker6);

            teams.AttackCamp = attackTeam.ToArray();

            var defendTeam = new List<BattleActor>();
            var defender1 = new BattleActor();
            defender1.Id = 1;
            defender1.CombatId = 0;
            defender1.Hp = 550;
            defender1.Atk = 100;
            defendTeam.Add(defender1);

            var defender2 = new BattleActor();
            defender2.Id = 2;
            defender1.CombatId = 1;
            defender2.Hp = 660;
            defender2.Atk = 50;
            defendTeam.Add(defender2);

            var defender3 = new BattleActor();
            defender3.Id = 3;
            defender1.CombatId = 2;
            defender3.Hp = 770;
            defender3.Atk = 600;
            defendTeam.Add(defender3);

            var defender4 = new BattleActor();
            defender4.Id = 4;
            defender1.CombatId = 3;
            defender4.Hp = 400;
            defender4.Atk = 110;
            defendTeam.Add(defender4);

            var defender5 = new BattleActor();
            defender5.Id = 5;
            defender1.CombatId = 4;
            defender5.Hp = 230;
            defender5.Atk = 120;
            defendTeam.Add(defender5);

            var defender6 = new BattleActor();
            defender6.Id = 6;
            defender1.CombatId = 5;
            defender6.Hp = 560;
            defender6.Atk = 130;
            defendTeam.Add(defender6);
            teams.DefendCamp = defendTeam.ToArray();

            return teams;
        }
    }
}
