using System.Xml;

namespace Vipl.AcsGenerator.VisualElements;

public sealed class CustomCheckBoxVisualGroup : CheckBoxVisualGroup, ICustomCheckBoxVisualElement
{
    public static IDictionary<string, CustomCheckBoxVisualGroup> All { get; } = new Dictionary<string, CustomCheckBoxVisualGroup>();

    public CustomCheckBoxVisualGroup(XmlElement element)
    {
        Variable = element.GetAttribute(nameof(Variable));
        Elements = element.ChildNodes.OfType<XmlElement>().Select(t => Trait.All[t.GetAttribute("Name")]).Cast<SimpleCheckBoxVisualElement>().ToArray();
        Icon = element.GetAttribute(nameof(Icon));
        Localization = new LocalizationEntry(){Key = Variable, Localization = element.GetAttribute(nameof(Localization)), File = LocalizationFiles.TraitFile} ;
        All[Variable] = this;
    }
    public LocalizationEntry Localization;
    public string Variable { get;  init; }
    public string Icon { get;  }
    protected override string GetGroupGuiElement(string style) 
        => ((ICustomCheckBoxVisualElement)this).GetCustomCheckBox(style);
    protected override SimpleCheckBoxVisualElement[] Elements { get; }
        
    public override string GetSetScopes(int value) =>
        LogicalOwner.IsSmall ? GetSetScopeForSmallGroups(value) : GetSetScopeForLargeGroups(value);
        
    public string GetSetScopeForLargeGroups(int value ) => $"GuiScope.AddScope( 'ctrl_value', {this.GetDigit(value)} ).End";
    public string GetSetScopeForSmallGroups(int value ) => 
        $"GuiScope.AddScope( 'ctrl_value', {this.GetDigit(value)} )" +
        $".AddScope( 'position' , {this.GetDigit(LogicalOwner.Index)} )" +
        ".End";

    public override LocalizationEntry[] Localizations
        => Localization.MakeArray(Elements.SelectMany(e => e.Localizations));
    public static void Parse()
    {
            
        var layoutsDocument = new XmlDocument();
        layoutsDocument.Load("visual_groups.xml");
            
        foreach (var element in layoutsDocument.GetElementsByTagName("Main").OfType<XmlElement>().First().ChildNodes.OfType<XmlElement>())
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CustomCheckBoxVisualGroup(element);
        }
            
    }
}