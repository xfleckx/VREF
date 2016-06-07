using UnityEngine;
using System.Collections;

namespace Assets.VREF.Utils.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;

        /**
        Returns the instance of this singleton.
        */
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<T>() as T;
                    
                    DontDestroyOnLoad(instance);

                    if (instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) +
                        " is needed in the scene, but there is none.");
                    }
                }

                return instance;
            }
        }
    }
}