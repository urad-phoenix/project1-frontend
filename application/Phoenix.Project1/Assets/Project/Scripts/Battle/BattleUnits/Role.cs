using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    public class Role : Unit
    {
        public int Location;

        public int HP;

        public Avatar GetAvatar()
        {
            return GetComponentInChildren<Avatar>();
        }

        public void SettingHP(int hp)
        {
//            Debug.Log($"SettingHP : {ID}, {hp}");
            HP = hp;
        }
    }
}