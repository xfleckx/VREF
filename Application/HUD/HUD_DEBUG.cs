using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Assets.VREF.Application.HUD
{
    public class HUD_DEBUG : MonoBehaviour
    {
        private const string FPS_LABEL_PATTERN = "FPS: {0}";
        private const string FT_LABEL_PATTERN =  "FT:  {0}";

        public Text FPS_LABEL;
        public Text FT_Label;
        
        public void UpdateFpsAndFTView(float fps, float ft)
        {
            var textColor = Color.green;

            if (fps < 30)
            {
                textColor = Color.red;
            }
            else if (fps < 60)
            {
                textColor = Color.yellow;
            }

            FPS_LABEL.color = textColor;
            FPS_LABEL.text = string.Format(FPS_LABEL_PATTERN, fps);
            FT_Label.text = string.Format(FT_LABEL_PATTERN, ft);
        }

    }

}