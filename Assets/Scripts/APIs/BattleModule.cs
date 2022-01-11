using UnityEngine;

using DefenceGameSystem.OS.Kernel;

namespace DefenceGameSystem.OS.API
{
    public class BattleModule : MonoBehaviour
    {
        public float health;
        public float attack;
        public Team team;
        
        public void GetDamage(float damage)
        {
            if(damage < 0) return;

            health -= damage;
        }
    }
}