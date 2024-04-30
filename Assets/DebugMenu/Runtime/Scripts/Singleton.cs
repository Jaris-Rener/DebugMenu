namespace DebugMenu
{
    using UnityEngine;

    public class Singleton<T>
        : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<T>();
                }

                return _instance;
            }
        }

        public new bool DontDestroyOnLoad;

        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (transform.root == transform && DontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else if (_instance.gameObject != gameObject)
            {
                Debug.Log($"Existing instance found when initialising Singleton<{typeof(T).Name}>. Destroying this.", this);
                Destroy(gameObject);
            }
        }
    }
}