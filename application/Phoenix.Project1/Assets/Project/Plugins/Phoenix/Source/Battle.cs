using System.Collections.Generic;
using System;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote;

namespace Phoenix.Project1.Game
{
    public class Battle
    {
        private bool _IsFinished;
        private BattleActors _Teams;
        private Recorder _Recorder;
        private readonly int _TeamSize = 6;

        public Battle(BattleActors teams)
        {
            _Teams = teams;
            _Recorder = new Recorder();
        }
        public BattleResult GetResult()
        {
            while (!_CastAction(_Teams.AttackTeam, _Teams.DefendTeam))
            {
                _Recorder.Next();
            }

            var result = _Recorder.GenerateResult();
            result.Teams = _Teams;
            return result;
        }

        private bool _CastAction(BattleActor[] attack_team, BattleActor[] defend_team)
        {
            for (int pos = 0;  pos < _TeamSize;  pos++)
            {
                var caster = attack_team[pos];
                var action = _Recorder.GenerateAction();
                var defender = attack_team[pos].PickDefender(defend_team);
                if (defender == null)
                {
                    _IsFinished = true;
                    break;
                }
                var realDamage = attack_team[pos].Damage(defender);
                action.Caster = caster.Id;
                action.Target = defender.Id;
                action.SkillId = caster.PickAbility();
                action.Effect.Type = EffectType.Hit;
                action.Effect.Value = realDamage;

                caster = defend_team[pos];
                action = _Recorder.GenerateAction();
                defender = defend_team[pos].PickDefender(attack_team);
                if (defender == null)
                {
                    _IsFinished = true;
                    break;
                }
                realDamage = defend_team[pos].Damage(defender);
                action.Caster = caster.Id;
                action.Target = defender.Id;
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