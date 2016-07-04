using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using Assets.VREF.Scripts.Util;

namespace Assets.VREF.Scripts.Instructions
{
    public class InstructionHud : MonoBehaviour
    {
        private const string referenceError = "Please reference Unity GUI Text and Raw Image Components to HUD_Instruction instance!";

        public GameObject panel;

        public Text Heading;

        public Text TextBesideImage;

        public Text TextWide;

        public RawImage ImageLeftFromText;

        public RawImage CenterImage;

        bool isRendering = false;

        public bool IsRendering
        {
            get { return isRendering; }
        }

        void Awake()
        {
        }

        public void ShowInstruction(Texture centerImagePreview)
        {
            CenterImage.texture = centerImagePreview;
        }

        void Start()
        {
            if (!ImageLeftFromText ||
                !Heading ||
                !TextBesideImage ||
                !TextWide)
            {
                throw new MissingReferenceException(referenceError);
            }

            Clear();
        }
        
        public void ShowInstruction(string text)
        {
            panel.SetActive(true);

            TextWide.text = text;
            TextWide.gameObject.SetActive(true);
            isRendering = true;
        }

        public void ShowInstruction(string text, string heading)
        {
            Clear();

            Heading.text = heading;
            Heading.gameObject.SetActive(true);

            ShowInstruction(text);
        }

        public void ShowInstruction(string text, string heading, Texture image)
        {
            Clear();

            panel.SetActive(true);

            Heading.text = heading;
            Heading.gameObject.SetActive(true);

            ImageLeftFromText.texture = image;
            ImageLeftFromText.gameObject.SetActive(true);

            TextBesideImage.text = text;
            TextBesideImage.gameObject.SetActive(true);

            isRendering = true;
        }

        public void Clear()
        {
            Heading.text = String.Empty;
            TextWide.text = String.Empty;
            TextBesideImage.text = String.Empty;
            ImageLeftFromText.texture = null;
            CenterImage.texture = null;

            isRendering = false;

            var children = panel.transform.AllChildren();

            foreach (var item in children)
            {
                item.SetActive(false);
            }

            panel.SetActive(false);
        }

        #region complex instruction management deprecated

        //   public InstructionSet currentInstructionSet;

        //   public Instruction currentInstruction;

        //   [SerializeField]
        //public IDictionary<string, InstructionSet> KnownSets = new SortedDictionary<string, InstructionSet>();

        //public void StartDisplaying(string nameOfInstructionSet)
        //{
        //	if (!KnownSets.ContainsKey(nameOfInstructionSet)) {
        //		throw new ArgumentException("The requested instruction set is not available! Please load it or create it.");
        //	}

        //	currentInstructionSet = KnownSets[nameOfInstructionSet];

        //	ActivateRendering();

        //	StartCoroutine(InstructionRendering());
        //}

        //void ActivateRendering()
        //{
        //	for (int i = 0; i < transform.childCount; i++)
        //	{
        //		transform.GetChild(i).gameObject.SetActive(true);
        //	}
        //}

        //   public void StartDisplaying()
        //   {
        //       if (currentInstructionSet == null)
        //           return;

        //       ActivateRendering();
        //       StartCoroutine(InstructionRendering());
        //   }

        //   public void DisplayDirect(InstructionSet set){
        //       ActivateRendering();

        //       currentInstructionSet = set;
        //       currentInstruction = set.instructions.First();

        //       Display(currentInstruction);
        //   }

        //public void StartDisplaying(InstructionSet set)
        //{
        //	currentInstructionSet = set;
        //	ActivateRendering();
        //	StartCoroutine(InstructionRendering());
        //}

        //public void StartDisplaying(Instruction instruction)
        //{
        //	currentInstructionSet = new InstructionSet();
        //	currentInstructionSet.instructions.AddLast(instruction);

        //       if (!this.isActiveAndEnabled)
        //           this.gameObject.SetActive(true);

        //	ActivateRendering();
        //	StartCoroutine(InstructionRendering());
        //}

        //   public void StopDisplaying()
        //   {
        //       forceStop = true;
        //       HideInstructionHUD();
        //   }

        //   public void SkipCurrent()
        //   {
        //       SwitchToNextInstruction = true;
        //   }

        //IEnumerator InstructionRendering()
        //{
        //       forceStop = false;
        //       SwitchToNextInstruction = false;

        //	IEnumerator<Instruction> instructionEnumerator = currentInstructionSet.instructions.GetEnumerator();

        //       Debug.Log("Start Instruction rendering");

        //	while (instructionEnumerator.MoveNext() && !forceStop)
        //	{
        //		var instruction = instructionEnumerator.Current;

        //		Display(instruction);

        //           isRendering = true;

        //           if (instruction.DisplayTime == 0 || SwitchToNextInstruction)
        //           {
        //               Debug.Log("Skip Instruction");
        //			yield return new WaitForEndOfFrame();

        //               SwitchToNextInstruction = false;
        //		} 

        //		yield return new WaitForSeconds(instruction.DisplayTime);
        //	}

        //       currentInstructionSet.instructions.Clear();
        //       currentInstructionSet = null;

        //	HideInstructionHUD();

        //       Debug.Log("End Instruction");

        //	yield return null;
        //}

        //void Display(Instruction instruction){

        //       if (instruction.ImageTexture != null)
        //       {
        //           InstructionTextBesideImage.text = instruction.Text;
        //           InstructionTextWide.text = string.Empty;
        //           InstructionImage.gameObject.SetActive(true);

        //           if (instruction.ImageTexture != null)
        //           {
        //               InstructionImage.texture = instruction.ImageTexture;
        //           }
        //       }
        //       else
        //       {

        //           InstructionTextBesideImage.text = string.Empty;
        //           InstructionTextWide.text = instruction.Text;
        //           InstructionImage.gameObject.SetActive(false);

        //       }
        //}

        //void Display(GameObject aVisibleObject)
        //{
        //	InstructionTextBesideImage.gameObject.SetActive(false);
        //	InstructionTextWide.gameObject.SetActive(false);
        //	InstructionImage.gameObject.SetActive(false);

        //}

        //void HideInstructionHUD()
        //{
        //	Debug.Log("Calling HideInstructionHUD");

        //       isRendering = false;

        //	for (int i = 0; i < transform.childCount; i++)
        //	{
        //		transform.GetChild(i).gameObject.SetActive(false);    
        //	}
        //}

        // Idea of building a Factory with fluent interface pattern

        //public HUDInstruction Give(string instruction)
        //{
        //    if (InstructionTextBesideImage)
        //        InstructionTextBesideImage.textFromSomewhere = instruction;

        //    return this;
        //}

        //public HUDInstruction With(Texture image)
        //{
        //    if (InstructionImage)
        //        InstructionImage.texture = image;

        //    return this;
        //}

        //public HUDInstruction For(float presentationTime)
        //{
        //    this.presentationTime = presentationTime; 
        //    return this;
        //}

        //public void Close()
        //{
        //    InstructionImage.texture = null;
        //    InstructionTextBesideImage.text = string.Empty;
        //}

        #endregion

        #region Serialization (deprecated)

        //[SerializeField]
        //private List<InstructionSet> serializebaleInstructionList = new List<InstructionSet>();

        //public void OnAfterDeserialize()
        //{
        //    foreach (var item in serializebaleInstructionList)
        //    {
        //        KnownSets.Add(item.name, item);
        //    }
        //}

        //public void OnBeforeSerialize()
        //{
        //    serializebaleInstructionList.Clear();

        //    var test = KnownSets.Values.ToArray();

        //    serializebaleInstructionList.AddRange(test); 
        //}

        #endregion
    }

    #region deprecated classes for instruction serialization
    //public class InstructionFactory
    //{ 
    //	public static InstructionSet ReadInstructionSetFrom(string fileName){

    //		var lines = File.ReadAllLines(fileName, System.Text.Encoding.UTF8);

    //		var resultSet = ScriptableObject.CreateInstance<InstructionSet>();

    //		resultSet.name = Path.GetFileNameWithoutExtension(fileName);

    //		Process(lines.ToList(), resultSet);

    //		return resultSet;
    //	}

    //	public static void Process(List<string> lines, InstructionSet set)
    //	{
    //		if (!lines.Any())
    //			return;

    //		string head = lines.First();

    //		if (head.ContainsTextEndingWithLineBreak())
    //		{
    //			set.instructions.AddLast(new Instruction());
    //			set.instructions.Last.Value.Text = head; 
    //		}

    //		if (Regex.IsMatch(head, @"^t:[0-9]+$"))
    //		{
    //			string timeDesc = head;
    //			string timeValue = timeDesc.Substring(2);
    //			float time = 0f;

    //			if (!float.TryParse(timeValue, out time))
    //				Debug.LogError("Could not parse time value for instruction using 0!");

    //			set.instructions.Last.Value.DisplayTime = time;

    //		}

    //		if (Regex.IsMatch(head, @"^i:[0-9]+[.]?[a-zA-Z]+$"))
    //		{
    //			string imageReference = lines.First();
    //			string imageName = imageReference.Substring(2); 

    //			var temp = set.instructions.Last.Value;
    //			set.instructions.RemoveLast();

    //			var instructionContainingImage = new Instruction();
    //			instructionContainingImage.Text = temp.Text;
    //			instructionContainingImage.DisplayTime = temp.DisplayTime;
    //			instructionContainingImage.ImageName = imageName;

    //			set.instructions.AddLast(instructionContainingImage);
    //		}

    //		lines.Remove(head);

    //		Process(lines, set);
    //	}

    //	public static void SaveTo(InstructionSet instructions, string fileName)
    //	{
    //		throw new NotImplementedException("currently unimportant feature");
    //		// TODO
    //		// File.WriteAllLines(fileName, lines);
    //	}

    //}

    //[Serializable]
    //public class InstructionSet : ScriptableObject, ISerializationCallbackReceiver
    //{
    //    void OnEnable()
    //    {
    //        Debug.Log(string.Format( "InstructionSet {0} created", name), this);
    //    }

    //	public LinkedList<Instruction> instructions = new LinkedList<Instruction>();

    //    [SerializeField]
    //    private List<Instruction> serializedInstructionList = new List<Instruction>();

    //    public void OnBeforeSerialize()
    //    {
    //        serializedInstructionList.Clear();
    //        foreach (var item in instructions)
    //        {
    //            serializedInstructionList.Add(item);
    //        }
    //    }

    //    public void OnAfterDeserialize()
    //    {
    //        foreach (var item in serializedInstructionList)
    //        {
    //            instructions.AddLast(item);
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return string.Format("IS: {0}", name);
    //    }
    //}

    //[Serializable]
    //public class Instruction
    //{ 
    //	public string Text;

    //	public float DisplayTime;

    //    public string ImageName;

    //    public string PathToImageAsResource;

    //    public Texture ImageTexture;
    //	public Instruction()
    //	{
    //        Text = String.Empty;
    //		DisplayTime = 0f;
    //	}

    //	public Instruction(string textFromSomewhere, float time)
    //	{
    //		this.Text = textFromSomewhere;
    //		this.DisplayTime = time;
    //	}
    //} 

    #endregion

    public static class StringExtensions
    {
        public static bool ContainsTextEndingWithLineBreak(this string line)
        {
            return Regex.IsMatch(line, @"^[^it:][a-zA-Z0-9!? ,#.;,':%&§|><()\[\]{}=ßÄÖÜäöü]+$");
        }
    }

}