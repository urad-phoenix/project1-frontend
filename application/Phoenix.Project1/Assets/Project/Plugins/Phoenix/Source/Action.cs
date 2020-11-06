using Regulus.Remote;

namespace Phoenix.Project1.Game
{
    namespace Battles
    {
        public enum EffectType
        {
            Hit,
            Critical,
            Heal
        }

        public class Effect
        {
            public EffectType Type;
            public int Value;
        }

        public class Action
        {
            public readonly int Order;
            public int Caster;
            public int Target;
            public int SkillId;
            public Effect Effect;

            public Action(int order)
            {
                Order = order;
            }

            public bool IsValid()
            {
                return Caster != 0;
            }
        }
    }
}
