using Assets.VREF.Application;

namespace Assets.VREF.Application
{
    public static class MarkerPattern
    {
        public const string BeginTrial = "BeginTrial\t{0}\t{1}\t{2}\t{3}\t{4}";

        public const string Unit = "{0}\tUnit\t{1}\t{2}";

        public const string Enter = "Entering Unit\t{0}";
        public const string Exit = "Exiting Unit\t{0}";

        public const string ShowObject = "ShowObject\t{0}\t{1}";

        public const string ObjectFound = "ObjectFound\t{0}\t{1}\t{2}\t{3}";
        
        /// <summary>
        /// From {0} to {1} TurnType {2} UnitType {3}
        /// </summary>
        public const string Turn = "Turn\tFrom{0}\tTo{1}\t{2}\t{3}";

        /// <summary>
        /// From {0} to {1} expected {2} TurnType {3} UnitType {4}
        /// </summary>
        public const string WrongTurn = "Incorrect\tFrom{0}\tTo{1}\tExpected{2}\t{3}\t{4}";

        public const string EndTrial = "EndTrial\t{0}\t{1}\t{2}\t{3}\t{4}";

        public static string FormatBeginTrial(string trialTypeName, string mazeName, int pathId, string objectName, string categoryName)
        {
            return string.Format(BeginTrial, trialTypeName, mazeName, pathId, objectName, categoryName);
        }

        public static string FormatFoundObject(string currentMazeName, int iD, string objectName, string categoryName)
        {
            return string.Format(ObjectFound, currentMazeName, iD, objectName, categoryName);
        }

        public static string FormatEndTrial(string trialTypeName, string mazeName, int pathId, string objectName, string categoryName)
        {
            return string.Format(EndTrial, trialTypeName, mazeName, pathId, objectName, categoryName);
        }

        public static string FormatDisplayObject(string objectName, string categoryName)
        {
            return string.Format(ShowObject, objectName, categoryName);
        }
    }
}