using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace Assets.BeMoBI.Paradigms.SearchAndFind
{
    public class Training : Trial
    {
        /// <summary>
        /// A Trial Start may caused from external source (e.g. a key press)
        /// </summary>
        public override void SetReady()
        {
            base.SetReady();

            SetLightningOn(path, mazeInstance);

        }

        protected override void ShowObjectAtStart()
        {
            StartCoroutine(DisplayInstruction());
        }

        IEnumerator DisplayInstruction()
        {
            paradigm.hud.Clear();

          //  paradigm.hud.ShowInstruction("Merke dir das Objekt und den Pfad durch das Labyrinth!", "Aufgabe");

            yield return new WaitForSeconds(2);

            paradigm.hud.Clear();

            base.ShowObjectAtStart();

            yield return null;
        }
    }
}
