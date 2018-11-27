using System;
using System.Collections.Generic;
using ProjectTrinity.Root;
using ProjectTrinity.Simulation;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectTrinity.UI
{
    public class ClassSelection : MonoBehaviour
    {
        [Serializable]
        private class ClassSelectionButton
        {
            public ClassType ClassType;
            public Image ButtonImage;
        }

        [SerializeField]
        private List<ClassSelectionButton> classSelectionButtons;

        [SerializeField]
        private Color selectedColor;

        public ClassType SelectedClassType { get; private set;}

        private void Awake()
        {
            SelectClassType(ClassType.Warrior);
        }

        public void SelectClassType(ClassType classType)
        {
            SelectedClassType = classType;

            foreach (var classSelectionButton in classSelectionButtons)
            {
                classSelectionButton.ButtonImage.color = classSelectionButton.ClassType == classType ? selectedColor : Color.white;
            }
        }

        public void SelectClassType(string classTypeString)
        {
            ClassType classType;
            if(!Enum.TryParse(classTypeString, out classType))
            {
                DIContainer.Logger.Warn(string.Format("ClassType couldnt be parsed from string: {0}", classTypeString));
                return;
            }

            SelectClassType(classType);
        }
    }
}