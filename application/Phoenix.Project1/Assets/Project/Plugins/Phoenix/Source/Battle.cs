using System.Collections.Generic;
using System;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote;

namespace Phoenix.Project1.Game
{
    public class Battle
    {
        private bool _IsFinished;
        private Camps _Camps;
        private Recorder _Recorder;
        private readonly int _CampSize = 6;
        private BattleStatus _Status;
        public Battle(Camps teams)
        {
            _Camps = teams;
            _Recorder = new Recorder();
        }
        public BattleResult GetResult()
        {
            while (!_CastAction(_Camps.AttackCamp, _Camps.DefendCamp))
            {
                _Recorder.Next();
            }

            var result = _Recorder.GenerateResult();
            result.Camps = _Camps;
            result.Status = _Status;
            return result;
        }

        private bool _CastAction(BattleActor[] attack_team, BattleActor[] defend_team)
        {
            for (int pos = 0;  pos < _CampSize;  pos++)
            {
                var caster = attack_team[pos];
                var action = _Recorder.GenerateAction();
                var defender = attack_team[pos].PickDefender(defend_team);
                if (defender == null)
                {
                    _IsFinished = true;
                    _Status = BattleStatus.Success;
                    break;
                }
                var realDamage = attack_team[pos].Damage(defender);
                action.Caster = caster.CombatId;
                action.Camp = CampType.Attack;
                action.Target = defender.CombatId;
                action.SkillId = caster.PickAbility();
                action.Effect.Type = EffectType.Hit;
                action.Effect.Value = realDamage;

                caster = defend_team[pos];
                action = _Recorder.GenerateAction();
                defender = defend_team[pos].PickDefender(attack_team);
                if (defender == null)
                {
                    _IsFinished = true;
                    _Status = BattleStatus.Fail;
                    break;
                }
                realDamage = defend_team[pos].Damage(defender);
                action.Caster = caster.CombatId;
                action.Target = defender.CombatId;
                action.Camp = CampType.Defend;
                action.SkillId = caster.PickAbility();
                action.Effect.Type = EffectType.Hit;
                action.Effect.Value = realDamage;
            }

            return _IsFinished;
            //foreach (var attacker in attack_team)
            //{
            //    var action = _Recorder.GenerateAction();
            //    var defender = attacker.PickDefender(defend_team);
            //    if (defender == null)
            //    {
            //        _IsFinished = true;
            //        return;
            //    }
            //    var realDamage = attacker.Damage(defender);
            //    action.Caster = attacker.Id;
            //    action.Target = defender.Id;
            //    action.SkillId = attacker.PickAbility();
            //    action.Effect.Type = EffectType.Hit;
            //    action.Effect.Value = realDamage;
            //}
        }
    }
}