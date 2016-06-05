using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace Assets.VREF.Application
{
    public class Experiment : Trial
    {
        /// <summary>
        /// A Trial Start may caused from external source (e.g. a key press)
        /// </summary>
        public override void SetReady()
        {
            SwitchAllLightPanelsOff(mazeInstance);

            base.SetReady();
        }

        protected override void ShowObjectAtStart()
        {
            paradigm.hud.Clear();

            // paradigm.hud.ShowInstruction("Erinnere dich an den Pfad für das folgende Objekt und\nfinde es im Labyrinth wieder.","Aufgabe");

            base.ShowObjectAtStart();
        }



        
    }
}
