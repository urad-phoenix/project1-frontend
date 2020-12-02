using Phoenix.Project1.Battles.Extensions;
namespace Phoenix.Project1.Battles
{
    class DamageFormula
    {
        public readonly float Damage;

        /*public DamageFormula(float caster_attack, float skill101, float skill102, float buff30203, float target_def, float var8, float people_num)
            :this(caster_attack.ToNormalize(), skill101.ToNormalize() , skill102.ToNormalize() , buff30203.ToNormalize() , target_def.ToNormalize() , var8.ToNormalize() , people_num.ToNormalize())
        {

        }*/
        public DamageFormula(float caster_attack, float skill101, float skill102, float buff30203 , float target_def, float var8 , float people_num)
        {
            var v1 = caster_attack * skill101 + skill102;
            var v2_1 = 1- buff30203;
            var v2 = 1- (target_def * v2_1 / caster_attack);
            var v3 = ((target_def * v2_1 / caster_attack) + var8) / people_num;
            var damage = v1 * v2 / v3;

            float result = damage ;            
            if (result < 1)
                result = 1;
            Damage = (float)System.Math.Ceiling(result);
        }
    
    }
}


