using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using System;
using System.Linq;

namespace Phoenix.Project1.Battles
{
    public class Actor : IActor
    {
        readonly System.Collections.Generic.Dictionary<MotionType, Motion> _Motions;

        public void Injure(int value)
        {
            Hp.Value -= value;
            if (Hp.Value < 0)
                Hp.Value = 0;
        }

        public Actor(int id, int location, Game.IConfigurationDatabase configuration)
        {
            var motion = configuration.Query<Configs.Motion>().Where(m => m.Key == "AttackTesttTimeline").Single();
            _Motions = new System.Collections.Generic.Dictionary<MotionType, Motion>();
            _Motions.Add(MotionType.Entrance, new Motion { Frames = 60 });            
            _Motions.Add(MotionType.Spell1, new Motion { Frames = motion.TotalFrame, HitFrames = motion.HitFrames.Select(h => h.Frame).ToArray() });
            Id = new Property<int>(id);
            Location = new Property<int>(location);
            Hp = new Property<int>(100);
        }
        public Actor(int id, int location, int hp)
        {
            _Motions = new System.Collections.Generic.Dictionary<MotionType, Motion>();
            _Motions.Add(MotionType.Entrance, new Motion { Frames = 60 });
            _Motions.Add(MotionType.Spell1, new Motion { Frames = 30 , HitFrames = new int[] { 20} });
            Id = new Property<int>(id);
            Location = new Property<int>(location);
            Hp = new Property<int>(hp);
        }

        public readonly Property<int> Id;
        Property<int> IActor.InstanceId => Id;
        

        public readonly Property<int> Location;
        public readonly Property<int> Hp;

        Property<int> IActor.Location => Location;

        Property<int> IActor.Hp => Hp;

        internal bool IsMovable()
        {
            return Hp == 0;
        }

        public Project1.Battles.Spell CastSpell()
        {
            return new Project1.Battles.Spell();
        }

        public Motion GetMotion(MotionType type)
        {
            return _Motions[type];
        }
    }
}
