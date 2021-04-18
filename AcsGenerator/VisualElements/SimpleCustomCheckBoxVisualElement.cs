namespace Vipl.AcsGenerator.VisualElements
{
    public class SimpleCustomCheckBoxVisualElement : SimpleCheckBoxVisualElement, ICustomCheckBoxVisualElement
    {
        public SimpleCustomCheckBoxVisualElement(string icon, string localization)
        {
            Icon = icon;
            Localization = localization;
        }

        public override string GetGuiElement(string style)
            => ((ICustomCheckBoxVisualElement) this).GetCustomCheckBox(style);

        public override Trait[] Traits => new Trait[0];

        public override string[] Localizations
            => new[] {$" {Variable}:0 {Localization}"};

        public string Icon { get; }
        public string Localization { get; }
        
        
    }
}