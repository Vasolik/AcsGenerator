using System.Collections.Generic;

namespace Vipl.AcsGenerator.VisualElements
{
    public class SimpleCustomCheckBoxVisualElement : SimpleCheckBoxVisualElement, ICustomCheckBoxVisualElement
    {
        public SimpleCustomCheckBoxVisualElement(string variable, string icon, string localization, string positiveTrigger, string negativeTrigger )
            : base(variable)
        {
            Icon = icon;
            Localization = new LocalizationEntry{Key = variable, Localization = localization, File = LocalizationFiles.TraitFile};
            NegativeTrigger = negativeTrigger;
            PositiveTrigger = positiveTrigger;
            Variable = variable;
        }
        public override string GetGuiElement(string style)
            => ((ICustomCheckBoxVisualElement) this).GetCustomCheckBox(style);


        public override LocalizationEntry[] Localizations => Localization.MakeArray();

        public string Icon { get; }
        public LocalizationEntry Localization { get;  }

        public override string NegativeTrigger { get; }

        public override string PositiveTrigger { get; }
    }
}