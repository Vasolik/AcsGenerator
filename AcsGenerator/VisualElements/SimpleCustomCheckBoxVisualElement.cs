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
        public override string Switch { get; } = "";
        public override string PositiveFlag => $"flag:{Variable}";
        public override string NegativeFlag => $"flag:{Variable}_negative";

        public override Trait[] Traits => new Trait[0];
        public override LogicalGroup LogicalOwner => null;

        public override string[] Localizations
            => new[] {$" {Variable}:0 {Localization}"};

        public string Icon { get; }
        public string Localization { get; }
        
        
    }
}