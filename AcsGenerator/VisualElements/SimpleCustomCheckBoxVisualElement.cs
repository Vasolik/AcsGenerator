namespace Vipl.AcsGenerator.VisualElements
{
    public class SimpleCustomCheckBoxVisualElement : SimpleCheckBoxVisualElement, ICustomCheckBoxVisualElement
    {
        public SimpleCustomCheckBoxVisualElement(string variable, string icon, string localization, string positiveTrigger, string negativeTrigger )
            : base(variable)
        {
            Icon = icon;
            Localization = localization;
            NegativeTrigger = negativeTrigger;
            PositiveTrigger = positiveTrigger;
            Variable = variable;
        }

        public override string GetGuiElement(string style)
            => ((ICustomCheckBoxVisualElement) this).GetCustomCheckBox(style);
        public override string PositiveFlag => $"flag:{Variable}";
        public override string NegativeFlag => $"flag:{Variable}_negative";

        public override Trait[] Traits => new Trait[0];
        public override string[] Localizations
            => new[] {$" {Variable}:0 {Localization}"};

        public string Icon { get; }
        public string Localization { get; }

        public override string NegativeTrigger { get; }

        public override string PositiveTrigger { get; }
    }
}