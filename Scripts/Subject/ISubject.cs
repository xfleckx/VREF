using UnityEngine;

namespace Assets.VREF.Scripts.Subject
{
    interface ISubject
    {
        Transform Head { get; }

        Transform Body { get; }

        void ConfigureWith(SubjectDescription description);
    }
}
