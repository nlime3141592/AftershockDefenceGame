using UnityEngine;

namespace DefenceGameSystem.OS.API
{
    public class Detectee : MonoBehaviour
    {
        public bool CanGetComponent<T_Component>(out T_Component component)
        where T_Component: MonoBehaviour
        {
            T_Component foundComponent;
            bool canGet;

            canGet = this.TryGetComponent<T_Component>(out foundComponent);
            component = foundComponent;
            
            return canGet;
        }
    }
}