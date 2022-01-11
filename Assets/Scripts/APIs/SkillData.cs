using System;

using UnityEngine;

using DefenceGameSystem.OS.Kernel;

namespace DefenceGameSystem.OS.API
{
    [Serializable]
    public class SkillData
    {
        public GameObject mainEffect;
        public GameObject hitEffect;
        public float cooltime;

        [HideInInspector]
        public float leftCooltime;
        public float skillConstant = 1.0f;
    }
}