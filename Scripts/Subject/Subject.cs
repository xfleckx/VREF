using System;
using UnityEngine;


namespace Assets.VREF.Scripts.Subject
{
    [RequireComponent(typeof(CharacterController))]
    public class Subject : MonoBehaviour, ISubject {

        private CharacterController character;

        private Transform body;
        public Transform Body
        {
            get
            {
                return body;
            }
        }

        public Camera eyes;

        private Transform head;
        public Transform Head
        {
            get
            {
                return head;
            }
        }

        public SubjectDescription description;

        void Awake()
        {
            character = GetComponent<CharacterController>();
            body = character.transform;
            head = eyes.transform;
        }

        public void ConfigureWith(SubjectDescription newDescription)
        {
            description = newDescription;
            character.radius = description.ShoulderWidth / 2;
            head.position.Set(0f, description.HeightFromFeetToEyes, 0f);
        }
    }
}
