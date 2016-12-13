using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace Assets.VREF.Scripts.Logic
{
    public class Timer : MonoBehaviour, IEventSystemHandler
    {
        public string aName = "NameOfYourTimer";

        public float Duration = 10;
        public bool repeatEndless = false;

        [SerializeField]
        public UnityEvent m_OnTimer = new UnityEvent();

        private float currentDuration;
        private float momentOfTimerStart;
        private IEnumerator timerRoutine;

        public void StartTimer(float targetDuration = 0)
        {
            if (targetDuration == 0){
                targetDuration = Duration;
            }

            Duration = targetDuration;

            resetDurationState();

            momentOfTimerStart = Time.realtimeSinceStartup;

            if (timerRoutine == null)
                timerRoutine = Run();

            StartCoroutine(timerRoutine);
        }
          
        public void ResetTimer()
        {
            StopAllCoroutines();
            timerRoutine = null;
            resetDurationState();
        }

        private IEnumerator Run(){

            yield return new WaitWhile(() =>{

                currentDuration = Time.realtimeSinceStartup - momentOfTimerStart; 

                return currentDuration <= Duration;
            });

            OnTimer();

            yield return null;
        }

        // Get the duration the timer is running
        public float GetCurrentDuration(){
            return currentDuration;
        }

        public float GetDurationTilEvent(){
            return Duration - currentDuration;
        }

        private void OnTimer()
        {
            if(m_OnTimer.GetPersistentEventCount() > 0)
                m_OnTimer.Invoke();

            resetDurationState();
            
            StopCoroutine(timerRoutine);

            if (repeatEndless)
            {
                StartTimer(Duration);
                return;
            }
        }

        private void resetDurationState()
        {
            currentDuration = 0;
        }

        public void Reset()
        {
            StopAllCoroutines();
        }

    }
}