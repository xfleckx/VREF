using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Paradigms.SearchAndFind.ImageEffects;

namespace Assets.BeMoBI.Paradigms.SearchAndFind
{
    public enum Internal_Trial_State { Searching, Returning }

    public interface ITrial
    {
        void Initialize(string mazeName, int pathID, string category, string objectName);

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

        protected beMobileMaze mazeInstance;

        protected PathInMaze path;

        public HidingSpot hidingSpotInstance;

        public PathController pathController;
        public GameObject objectToRemember;

        public int currentPathID = -1;

        public string currentMazeName = string.Empty;

        public CustomGlobalFog fog;

        public Texture wrongTurnIcon;

        #endregion

        #region Trial state only interesting for the trial itself!

        protected string objectName;

        protected string categoryName;

        protected GameObject activeEnvironment;

        protected LinkedListNode<PathElement> currentPathElement;

        protected MazeUnit currentUnit;

        protected MazeUnit lastUnit;

        protected Internal_Trial_State currentTrialState;

        protected Stopwatch stopWatch;

        protected Stopwatch unitStopWatch;

        protected MazeUnit lastCorrectUnit;

        private bool isReady;
        internal bool acceptsASubmit;

        #endregion

        #region Setup methods

        public virtual void Initialize(string mazeName, int pathID, string category, string objectName)
        {
            UnityEngine.Debug.Log(string.Format("Initialize {0} Trial: {1} {2} {3} {4}", this.GetType().Name, mazeName, pathID, category, objectName));

            var expectedWorld = paradigm.VRManager.ChangeWorld(mazeName);

            if (expectedWorld != null)
            {
                activeEnvironment = expectedWorld.gameObject;
            }
            else
            {
                UnityEngine.Debug.Log(string.Format("Expected VR Environment \"{0}\" not found! Ending Trial!", mazeName));
                OnFinished(TimeSpan.Zero);
            }

            stopWatch = new Stopwatch();

            unitStopWatch = new Stopwatch();

            mazeInstance = activeEnvironment.GetComponent<beMobileMaze>();

            currentMazeName = mazeName;

            paradigm.relativePositionStream.currentMaze = mazeInstance;

            mazeInstance.MazeUnitEventOccured += SubjectMovesWithinTheMaze;

            paradigm.startingPoint.EnterStartPoint += OnStartPointEntered;
            paradigm.startingPoint.LeaveStartPoint += OnStartPointLeaved;

            currentTrialState = Internal_Trial_State.Searching;

            ResetStartConditions();

            ActivatePathAndSetHidingSpot(pathID);

            SwitchAllLightPanelsOff(mazeInstance);

            GatherObjectFromObjectPool(category, objectName);

        }

        private void ResetStartConditions()
        {
            paradigm.objectPresenter.SetActive(true);

        }

        private void ActivatePathAndSetHidingSpot(int pathId)
        {
            pathController = activeEnvironment.GetComponent<PathController>();

            currentPathID = pathId;

            path = pathController.EnablePathContaining(pathId);

            var unitAtPathEnd = path.PathAsLinkedList.Last.Value.Unit;

            // hiding spot look at inactive (open wall)
            var targetRotation = GetRotationFrom(unitAtPathEnd);
            var hidingSpotHost = Instantiate(paradigm.HidingSpotPrefab);

            hidingSpotHost.transform.SetParent(unitAtPathEnd.transform, false);
            hidingSpotHost.transform.localPosition = Vector3.zero;
            hidingSpotHost.transform.Rotate(targetRotation);

            hidingSpotInstance = hidingSpotHost.GetComponent<HidingSpot>();
        }

        private void GatherObjectFromObjectPool(string categoryName, string objectName)
        {
            var objectCategory = paradigm.objectPool.Categories.Where(c => c.name.Equals(categoryName)).FirstOrDefault();

            if (objectCategory == null)
                throw new ArgumentException(string.Format("Expected category \"{0}\" not found!", categoryName));

            var targetObject = objectCategory.GetObjectBy(objectName);

            if (targetObject == null)
                throw new ArgumentException(string.Format("Expected Object \"{0}\" from category \"{1}\" not found!", objectName, categoryName));

            objectToRemember = Instantiate(targetObject);

            objectToRemember.transform.SetParent(paradigm.objectPositionAtTrialStart, false);
            objectToRemember.transform.localPosition = Vector3.zero;
            objectToRemember.transform.rotation = Quaternion.identity;
            objectToRemember.transform.localScale = Vector3.one;

            objectToRemember.transform.LookAt(paradigm.subject.transform);

            var originalRotation = objectToRemember.transform.rotation;

            var rotationWithFixed_XZ = new Vector3(0, originalRotation.eulerAngles.y, 0);

            objectToRemember.transform.rotation = Quaternion.Euler(rotationWithFixed_XZ);

            this.objectName = objectName;

            this.categoryName = categoryName;

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

        protected void SwitchAllLightPanelsOff(beMobileMaze maze)
        {
            var allLights = maze.GetComponentsInChildren<TopLighting>();

            foreach (var light in allLights)
            {
                light.gameObject.transform.AllChildren().ForEach(l =>
                {
                    //if (l.gameObject.name != "Light")
                    l.gameObject.SetActive(false);
                }
                );
            }

        }

        protected virtual void SetLightningOn(PathInMaze path, beMobileMaze maze)
        {
            // UnityEngine.Debug.Log(string.Format("Try enabling lights on Path: {0} in Maze: {1}",path.ID, maze.name));

            var currentElement = path.PathAsLinkedList.First;

            int globalRotation = 0;

            do
            {

                // TODO: special case last element!

                var previousElement = currentElement.Previous;

                var nextElement = currentElement.Next;

                if (previousElement == null)
                    previousElement = currentElement;

                if (nextElement == null)
                {
                    nextElement = previousElement;
                }

                var previousElementsPosition = previousElement.Value.Unit.transform.position;
                var currentElementsPosition = currentElement.Value.Unit.transform.position;
                var nextPathElementsPosition = nextElement.Value.Unit.transform.position;

                var a = previousElementsPosition - currentElementsPosition;

                Vector3 b = Vector3.zero;

                if (currentElement.Next != null)
                    b = currentElementsPosition - nextPathElementsPosition;
                else
                    b = nextPathElementsPosition - currentElementsPosition;

                var turningAngle = a.SignedAngle(b, Vector3.up);

                globalRotation = (globalRotation + (int)turningAngle) % 360;

                //UnityEngine.Debug.Log(string.Format("From {2} to {3} ## Current Angle: {0} ## GlobalRotation {1}", turningAngle, globalRotation, currentElementsPosition, nextPathElementsPosition));

                var topLight = currentElement.Value.Unit.GetComponentInChildren<TopLighting>();

                ChangeLightningOn(topLight, currentElement.Value, globalRotation);

                // topLight.SwitchOn();

                currentElement = currentElement.Next;

            } while (currentElement != null);
        }

        private void ChangeLightningOn(TopLighting light, PathElement current, int globalRotation)
        {
            var lightChildren = light.gameObject.transform.AllChildren();

            var toDirectionPanelName = OrientationDefinition.Current.GetDirectionNameFromEuler(globalRotation);

            int rotationOffset = 0;
            if (current.Type == UnitType.L || current.Type == UnitType.T || current.Type == UnitType.X)
            {
                if (current.Turn == TurnType.LEFT)
                    rotationOffset = -90;

                if (current.Turn == TurnType.RIGHT)
                    rotationOffset = 90;

                if (current.Turn == TurnType.STRAIGHT)
                    rotationOffset = 180;
            }

            if (current.Type == UnitType.I)
            {
                rotationOffset = 180;
            }

            var correctedDirection = (globalRotation + rotationOffset) % 360;

            var fromDirectionPanelName = OrientationDefinition.Current.GetDirectionNameFromEuler(correctedDirection);
            
            // Enable only for open walls and the direction

            foreach (var lightPanel in lightChildren)
            {
                if (lightPanel.name.Equals("Center"))
                {
                    lightPanel.SetActive(true);
                }

                if (lightPanel.name.Equals(toDirectionPanelName) || lightPanel.name.Equals(fromDirectionPanelName))
                {
                    lightPanel.SetActive(true);
                }
            }

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
          
        /// <summary>
        /// This is the method of interest. It implements the behaviour whenever the subject moves from unit to unit.
        /// </summary>
        /// <param name="evt">Contians the information about trigger collisions within a maze</param>
        public virtual void SubjectMovesWithinTheMaze(MazeUnitEvent evt)
        {
            if (evt.MazeUnitEventType == MazeUnitEventType.Entering)
            {
                lastUnit = currentUnit;
                currentUnit = evt.MazeUnit;

                if (lastUnit != null)
                    paradigm.marker.Write(MarkerPattern.FormatMazeUnitEvent(lastUnit, MazeUnitEventType.Exiting));

                paradigm.marker.Write(MarkerPattern.FormatMazeUnitEvent(currentUnit, MazeUnitEventType.Entering));
                
                if (SubjectEntersTheMaze())
                {
                    if (SubjectEnteredThePath())
                    {
                        currentPathElement = path.PathAsLinkedList.First;
                        
                        paradigm.hud.Clear();
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Seems as something entered the maze on the wrong entrance!");
                    }
                }
                else if (SubjectReachedEndOfPath()) 
                {
                    if (currentTrialState == Internal_Trial_State.Searching)
                    {
                        paradigm.marker.Write(MarkerPattern.FormatCorrectTurn(currentPathElement.Value, currentPathElement.Next.Value));

                        StartCoroutine(WaitWithOpeningUntilButtonPress());
                    }
                }
                else if (SubjectFollowsPath())
                {
                    paradigm.marker.Write(
                        MarkerPattern.FormatCorrectTurn(currentPathElement.Value, currentPathElement.Next.Value));
                     
                    if (paradigm.hud.IsRendering)
                        paradigm.hud.Clear();
                    
                    currentPathElement = currentPathElement.Next;
                }
                else if(SubjectMoveToAWrongUnit())
                {
                    paradigm.marker.Write(
                        MarkerPattern.FormatIncorrectTurn(currentUnit, currentPathElement.Value, currentPathElement.Next.Value));

                    paradigm.audioInstructions.play("wrongTurn");

                    //paradigm.hud.ShowWrongTurnIconFor(1.5f);
                    //acceptsASubmit = true;
                }
                else if(SubjectReturnsToPath())
                {
                    //paradigm.hud.ShowInstruction("Du bist wieder auf dem richtigen Weg!", "Gut.");

                    //acceptsASubmit = true;
                }

            }

        }

        private IEnumerator WaitWithOpeningUntilButtonPress()
        {
            acceptsASubmit = true;

            yield return new WaitWhile(() => acceptsASubmit);

            yield return new WaitForSeconds(0.3f);

            RevealObjectWhenSubjectReachesTheTarget();

            yield return null;
        }

        private void RevealObjectWhenSubjectReachesTheTarget()
        {
            hidingSpotInstance.RevealImmediately();

            var socket = hidingSpotInstance.GetSocket();

            socket.transform.LookAt(paradigm.subject.Body.transform);

            var originalRotation = socket.transform.rotation;

            socket.rotation = Quaternion.Euler(0, originalRotation.eulerAngles.y, 0);

            var objectFoundMarker = MarkerPattern.FormatFoundObject(currentMazeName, path.ID, objectName, categoryName);

            paradigm.marker.WriteAtTheEndOfThisFrame(objectFoundMarker);

            paradigm.TrialEndPoint.gameObject.SetActive(true);

            currentTrialState = Internal_Trial_State.Returning;

            StartCoroutine(InitializeTrialEnd());
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

        #region Helper functions

        private Vector3 GetRotationFrom(MazeUnit unit)
        {
            var childs = unit.transform.AllChildren();
            // try LookAt functions
            foreach (var wall in childs)
            {
                if (wall.name.Equals("South") && !wall.activeSelf)
                {
                    return Vector3.zero;
                }

                if (wall.name.Equals("North") && !wall.activeSelf)
                {
                    return new Vector3(0, 180, 0);
                }

                if (wall.name.Equals("West") && !wall.activeSelf)
                {
                    return new Vector3(0, 90, 0);
                }

                if (wall.name.Equals("East") && !wall.activeSelf)
                {
                    return new Vector3(0, 270, 0);
                }

            }

            return Vector3.one;
        }

        #endregion
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
