using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace Assets.VREF.Scripts.Logic
{
    public class Timer : MonoBehaviour, IEventSystemHandler
    {
        public float Milliseconds = 10;
        public bool repeatEndless = false;

        [SerializeField]
        public UnityEvent m_OnTimer = new UnityEvent();

        private float currentMilliseconds;
        private bool timerEnabled = false;

        public void StartTimer()
        {
            timerEnabled = true;
            currentMilliseconds = Milliseconds;
        }

        // Update is called once per frame
        void Update()
        {

            if (timerEnabled && currentMilliseconds > 0)
            {
                currentMilliseconds = currentMilliseconds - Time.deltaTime;
            }
            else if (timerEnabled && currentMilliseconds <= 0)
            {
                OnTimer();
            }
        }

        private void OnTimer()
        {
            if (!repeatEndless)
            {
                timerEnabled = false;
            }

            currentMilliseconds = Milliseconds;

            m_OnTimer.Invoke();
        }

        public void Reset()
        {
            timerEnabled = false;
            currentMilliseconds = Milliseconds;
        }

    }
}