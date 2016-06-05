using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.VREF.Application.Editor
{
    public class ConfigurationWindowModel : ScriptableObject {

        private static ConfigurationWindowModel _instance;
        
        public string PathToCurrentConfig;

        public int indexOfSelectedHeadController;

        public int indexOfSelectedBodyController;

        public int indexOfSelectedCombinedController;

        public List<ViewOfSelectableMazes> SelectableMazes;

        internal string[] headControllerNames;
        internal string[] bodyControllerNames;
        internal string[] combiControllerNames;
        internal int lastSelectedCondition;

        public static ConfigurationWindowModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = CreateInstance<ConfigurationWindowModel>();

                return _instance;
            } 
        }
    }


    public class ViewOfSelectableMazes
    {
        public bool selected;

        public string mazeName;

        public List<SelectablePath> SelectablePaths;
        
    }


    public class SelectablePath
    {
        public bool selected;

        [SerializeField]
        private readonly int pathId;

        [SerializeField]
        private readonly int difficulty;

        public SelectablePath(int id, int difficulty)
        {
            pathId = id;

            this.difficulty = difficulty;
        }

        public int Difficulty
        {
            get
            {
                return difficulty;
            }
        }

        public int Id
        {
            get
            {
                return pathId;
            }
        }
        
    }

}