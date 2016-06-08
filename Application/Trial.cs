using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.VREF.Application.FogControl;
using Assets.VREF.Controls; 

namespace Assets.VREF.Application
{
    public enum Internal_Trial_State { Searching, Returning }

    public interface ITrial
    {
        void Initialize(string paradigmName, int pathID, string category, string objectName);

        void SetReady();

        event Action BeforeStart;

        event Action<Trial, TrialResult> Finished;

        void CleanUp();
    }

    public class Trial : MonoBehaviour, ITrial
    {
        #region Dependencies 

        public ParadigmController paradigm;

        public ConditionConfiguration conditionConfig;

        protected DummyInstance dummyInstance;

        public GameObject objectToRemember;

        public int currentPathID = -1;

        public string currentParadigmName = string.Empty;

        public CustomGlobalFog fog;

        public Texture wrongTurnIcon;

        #endregion

        #region Trial state only interesting for the trial itself!

        protected string objectName;

        protected string categoryName;

        protected GameObject activeEnvironment;

        protected Internal_Trial_State currentTrialState;

        protected Stopwatch stopWatch;

        protected Stopwatch unitStopWatch;


        private bool isReady;
        internal bool acceptsASubmit;

        #endregion

        #region Setup methods

        public virtual void Initialize(string paradigmName, int pathID, string category, string objectName)
        {
            UnityEngine.Debug.Log(string.Format("Initialize {0} Trial: {1} {2} {3} {4}", this.GetType().Name, paradigmName, pathID, category, objectName));

            var expectedWorld = paradigm.VRManager.ChangeWorld(paradigmName);

            if (expectedWorld != null)
            {
                activeEnvironment = expectedWorld.gameObject;
            }
            else
            {
                UnityEngine.Debug.Log(string.Format("Expected VR Environment \"{0}\" not found! Ending Trial!", paradigmName));
                OnFinished(TimeSpan.Zero);
            }

            stopWatch = new Stopwatch();

            unitStopWatch = new Stopwatch();

            dummyInstance = activeEnvironment.GetComponent<DummyInstance>();

            currentParadigmName = paradigmName;

            paradigm.relativePositionStream.currentParadigm = dummyInstance;;

            paradigm.startingPoint.EnterStartPoint += OnStartPointEntered;
            paradigm.startingPoint.LeaveStartPoint += OnStartPointLeaved;

            currentTrialState = Internal_Trial_State.Searching;

            ResetStartConditions();


        }

        private void ResetStartConditions()
        {
            paradigm.objectPresenter.SetActive(true);

        }
        

        private void HideSocketAndOpenEntranceAtStart()
        {
            OpenEntrance();

            paradigm.objectPresenter.SetActive(false);
            var socketAtThePathEnd = hidingSpotInstance.GetSocket();

            var objectSocket = socketAtThePathEnd.GetComponent<ObjectSocket>();

            objectSocket.PutIn(objectToRemember);

            objectSocket.gameObject.SetActive(false);

            paradigm.fogControl.RaiseFog();
        }

        private void OpenEntrance()
        {
            var animator = paradigm.entrance.GetComponent<Animator>();

            animator.SetTrigger("Open");

        }

        private void CloseEntrance()
        {
            var animator = paradigm.entrance.GetComponent<Animator>();

            animator.SetTrigger("Close");
        }

        protected virtual void ShowObjectAtStart()
        {
            objectToRemember.SetActive(true);

            paradigm.marker.Write(MarkerPattern.FormatDisplayObject(objectName, categoryName));

            StartCoroutine(
                DisplayObjectAtStartFor(
                    conditionConfig.TimeToDisplayObjectToRememberInSeconds)
                    );
        }
        
        IEnumerator DisplayObjectAtStartFor(float waitingTime)
        {
            yield return new WaitForSeconds(waitingTime);

            HideSocketAndOpenEntranceAtStart();
        }
       
        #endregion

        public virtual void SetReady()
        {
            this.isReady = true; // Trial starts when Subject enters Startpoint

            paradigm.hud.Clear();

            // paradigm.hud.ShowInstruction("Bitte betretet den grünen Start Punkt um zu Beginnen!", "Aufgabe");

            acceptsASubmit = true;
        }

        private void OnStartPointEntered(Collider c)
        {
            if (c.tag == "Subject")
                EntersStartPoint(paradigm.subject);
        }

        private void OnStartPointLeaved(Collider c)
        {
            if (c.tag == "Subject")
                LeavesStartPoint(paradigm.subject);
        }

        public virtual void EntersStartPoint(VRSubjectController subject)
        {
            if (isReady && currentTrialState == Internal_Trial_State.Searching)
            {
                OnBeforeStart();

                paradigm.marker.Write(MarkerPattern.FormatBeginTrial(this.GetType().Name, currentMazeName, path.ID, objectName, categoryName));

                stopWatch.Start();

                ShowObjectAtStart();

            }
        }
        
        public virtual void LeavesStartPoint(VRSubjectController subject)
        {
            if (currentTrialState == Internal_Trial_State.Searching)
            {
                paradigm.startingPoint.gameObject.SetActive(false);
                // write a marker when the subject starts walking!?
                paradigm.hud.Clear();
            }
        }

        public virtual void EntersWaypoint(ActionWaypoint waypoint)
        {
            if (!this.isActiveAndEnabled || waypoint.WaypointId != 0)
                return;

            if (currentTrialState == Internal_Trial_State.Returning)
            {
                paradigm.marker.Write(MarkerPattern.FormatEndTrial(this.GetType().Name, currentMazeName, path.ID, objectName, categoryName));

                paradigm.startingPoint.gameObject.SetActive(true);

                stopWatch.Stop();

                waypoint.HideInfoText();

                if (!conditionConfig.useTeleportation)
                {
                    CloseEntrance();
                }
                else if (conditionConfig.UseShortWayBack)
                {
                   
                }

                paradigm.fogControl.LetFogDisappeare();

                //paradigm.hud.ShowInstruction("Kehre zurück und betretet den grünen \"End\" Punkt", "Aufgabe:");


                OnFinished(stopWatch.Elapsed);
            }
        }
        
        public virtual void LeavesWaypoint(ActionWaypoint waypoint)
        {
            if (!this.isActiveAndEnabled || waypoint.WaypointId != 0)
                return;
        }

        #region semantical helper methods - Clean Code ;)

        private bool SubjectEntersTheMaze()
        {
            return currentPathElement == null;
        }

        private bool SubjectReachedEndOfPath()
        {
            var unitOfTheLastPathElement = path.PathAsLinkedList.Last.Value.Unit;

            return unitOfTheLastPathElement.Equals(currentUnit);
        }

        private bool SubjectEnteredThePath()
        {
            var unitOfTheFirstPathElement = path.PathAsLinkedList.First.Value.Unit;

            return unitOfTheFirstPathElement.Equals(currentUnit);
        }

        private bool SubjectFollowsPath()
        {
            var nextPathElement = currentPathElement.Next;
             
            var unitOfNextPathElement = nextPathElement.Value.Unit;
             
            var newUnitIsThe_Next_CorrectUnit = unitOfNextPathElement.Equals(currentUnit);

            return newUnitIsThe_Next_CorrectUnit;
        }

        private bool SubjectMoveToAWrongUnit()
        {
            var nextPathElement = currentPathElement.Next;

            var nextPathElementsUnit = nextPathElement.Value.Unit;

            var getBackToLastCorrectUnit = currentPathElement.Value.Unit.Equals(currentUnit);

            return !nextPathElementsUnit.Equals(currentUnit) && !getBackToLastCorrectUnit;
        }

        private bool SubjectReturnsToPath()
        {
            var lastUnitWas_Not_a_PathElement = currentPathElement.Previous.Value.Unit.Equals(lastUnit) || currentPathElement.Value.Unit.Equals(lastUnit);

            var currentUnitIsElementInPath = currentPathElement.Value.Unit.Equals(currentUnit);

            return lastUnitWas_Not_a_PathElement && currentUnitIsElementInPath;
        }

        #endregion
       

        private IEnumerator WaitWithOpeningUntilButtonPress()
        {
            acceptsASubmit = true;

            yield return new WaitWhile(() => acceptsASubmit);

            yield return new WaitForSeconds(0.3f);

            RevealObjectWhenSubjectReachesTheTarget();

            yield return null;
        }


        private IEnumerator InitializeTrialEnd()
        {
            yield return new WaitForSeconds(1f);

            if (conditionConfig.useTeleportation)
            {
                paradigm.hud.ShowInstruction("Geschafft! Entspann dich, du wirst zum Endpunkt teleportiert.", "Sehr gut!");
                
                // important here... to get rid of the last MazeUnitEvent when the subject gets teleported!
                // It's too late - so do it here!
                mazeInstance.MazeUnitEventOccured -= SubjectMovesWithinTheMaze;
                paradigm.marker.Write(MarkerPattern.FormatMazeUnitEvent(currentUnit, MazeUnitEventType.Exiting));

                StartCoroutine(BeginTeleportation());

                CloseEntrance();
            }
            else
            {
                //paradigm.hud.ShowInstruction("Geschafft! Kehre nun zurück zum Endpunkt!", "Aufgabe");

                if (conditionConfig.UseShortWayBack) {
                    mazeInstance.gameObject.SetActive(false);
                    paradigm.fogControl.LetFogDisappeare();
                }
                else
                {
                    path.InvertPath();

                    currentPathElement = path.PathAsLinkedList.First;
                }
            }

            yield return null;
        }

        /// <summary>
        /// When the subject presses a button for submit...
        /// </summary>
        public void RecieveSubmit()
        {
            if (paradigm.hud.IsRendering)
                paradigm.hud.Clear();


            //reset until the next opportunity
            acceptsASubmit = false;
        }
        
        IEnumerator BeginTeleportation()
        {
            yield return new WaitForSeconds(conditionConfig.offsetToTeleportation);

            paradigm.teleporter.Teleport();

            yield return null;
        }

        /// <summary>
        /// Warning using this could cause inconsistent behaviour within the paradigm!
        /// In most cases, the trial should end itself!
        /// </summary>
        public virtual void ForceTrialEnd()
        {
            stopWatch.Stop();

            OnFinished(stopWatch.Elapsed);
        }

        public event Action BeforeStart;
        protected void OnBeforeStart()
        {
            if (BeforeStart != null)
                BeforeStart();
        }

        public event Action<Trial, TrialResult> Finished;
        protected void OnFinished(TimeSpan trialDuration)
        {
            if (Finished != null)
                Finished(this, new TrialResult(trialDuration));
        }

        protected void ClearCallbacks()
        {

            Finished = null;
            BeforeStart = null;

        }

        public void CleanUp()
        {
            paradigm.entrance.SetActive(true);

            paradigm.hud.Clear();

            paradigm.fogControl.DisappeareImmediately();

            currentUnit = null;
            lastUnit = null;

            currentPathElement = null;

            if (path != null && path.Inverse)
                path.InvertPath();

            if (mazeInstance != null)
            {
                var lineRenderer = mazeInstance.GetComponent<LineRenderer>();

                Destroy(lineRenderer);

                mazeInstance.MazeUnitEventOccured -= SubjectMovesWithinTheMaze;
            }

            ClearCallbacks();

            paradigm.startingPoint.ClearSubscriptions();

            if (hidingSpotInstance != null)
            {
                Destroy(hidingSpotInstance.gameObject);
            }
        }

    }

    public class TrialResult
    {
        private TimeSpan duration;
        public TimeSpan Duration { get { return duration; } }

        public TrialResult(TimeSpan duration)
        {
            this.duration = duration;
        }
    }
}
