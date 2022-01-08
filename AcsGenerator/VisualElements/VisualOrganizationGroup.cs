using System.Xml;

using Vipl.AcsGenerator.Layouts;

namespace Vipl.AcsGenerator.VisualElements;

public enum LocalizationFiles
{
    TraitFile,
    DropDownFile
}
public class LocalizationEntry
{
    public string Key { get; set; }
    public string Localization { get; set; }
    public LocalizationFiles File { get; set; }
}
public interface ILocalizable
{
    public LocalizationEntry[] Localizations { get; }
}
public class VisualOrganizationGroup : ILayout
{
    public static readonly IList<VisualOrganizationGroup> All = new List<VisualOrganizationGroup>();
    public VisualOrganizationGroup( XmlElement element)
    {
        IsDropDown = (element.GetAttributeNode("IsDropDown")?.Value).ToBool();
        Variable = !IsDropDown ?   $"acs_filter_trait_{element.GetAttribute("Name")}" : element.GetAttribute("Name") ;
        Localization = element.GetAttributeNode("LocalizationKey") is not null || element.GetAttributeNode("Localization") is not null ?
            new LocalizationEntry{Key = element.GetAttributeNode("LocalizationKey")?.Value ?? $"acs_filter_trait_group_{element.GetAttribute("Name")}", Localization = element.GetAttributeNode("Localization")?.Value, File = IsDropDown ? LocalizationFiles.DropDownFile : LocalizationFiles.TraitFile}
            : null;
            
        All.Add(this);
    }

    public IList<ILayout> Layouts { get; } = new List<ILayout>();

    public static void Parse()
    {
            
        var layoutsDocument = new XmlDocument();
        layoutsDocument.Load("layout.xml");
            
        foreach (var element in layoutsDocument.GetElementsByTagName("Main").OfType<XmlElement>().First()!.ChildNodes.OfType<XmlElement>())
        {
            switch (element.Name)
            {               
                case nameof(VisualOrganizationGroup):
                        
                    var group = new VisualOrganizationGroup(element);
                    foreach (var layout in element.ChildNodes.OfType<XmlElement>())
                    {
                        group.Layouts.Add( layout.Name switch
                        {
                            nameof(NormalLayout) => new NormalLayout(layout),
                            nameof(StarLayout) => new StarLayout(layout),
                            nameof(ThreePackLayout) => new ThreePackLayout(layout),
                            nameof(SixPackLayout) => new SixPackLayout(layout),
                            "SkillLayout" => Skill.ParseTwoSkillsLayout(layout),
                            nameof(CustomLayout) => new CustomLayout(layout),
                            nameof(DropDown) => DropDown.All.Single(d => d.Name == layout.GetAttribute(nameof(d.Name))),
                            nameof(Divider) => new Divider(),
                            _ => throw new Exception("Invalid layout.txt file")
                        });
                    }
                        
                    break;
            }
        }
    }

    public string Variable { get; }
    public bool IsDropDown { get; }
        
    public LocalizationEntry Localization { get; }
        
    private string HeaderBlockOverrides =>
        $@"
blockoverride ""acs_vbox_filter_group_name"" {{
    text = ""{Localization.Key}""
}}

blockoverride ""acs_vbox_filter_group_toggle"" {{
    onclick = ""[GetVariableSystem.Toggle( '{Variable}' )]""
    frame = ""[Select_int32( Not( GetVariableSystem.Exists( '{Variable}' ) ), '(int32)1', '(int32)2' )]""
}}

blockoverride ""acs_filter_visible"" {{
    visible = ""[GetVariableSystem.Exists( '{Variable}' )]""
}}
";
        
        
    public string GuiElement =>
        $@"types  {Variable}_group_types {{
    type {Variable}_group = acs_vbox_filter_group{(Localization is null? "_no_header" : "" )} {{
        name = ""{Variable}""
        {(Localization is not null? HeaderBlockOverrides.Intend(2) : "" )}
        blockoverride ""acs_filters"" {{
            {Layouts.Select(e => e.GuiElement).Join(3)}
        }}
    }}
}}";




    public LocalizationEntry[] Localizations => Localization is null ? Layouts.SelectMany(e => e.Localizations).ToArray() :
        Localization.MakeArray(Layouts.SelectMany(e => e.Localizations));

    public static string GetCompleteLocalization(LocalizationFiles file)
        => "l_english:\n" +
           All.SelectMany(o => o.Localizations)
               .Where(l => !l.Localization.IsNullOrEmpty() && l.File == file)
               .Select(l => $@" {l.Key}:0 ""{l.Localization}""")
               .Join();
        
    public static void GenerateGuiElement(string path)
    {
        foreach (var visualOrganisation in All)
        {
            File.WriteAllText($"{path}gui/{visualOrganisation.Variable}.gui",visualOrganisation.GuiElement, new System.Text.UTF8Encoding(true));
        }
    }

        
    public static void GenerateGuiCall()
    {
        Console.WriteLine(All.Select(o => $"{o.Variable}_group = {{ }}").Join(8, true));
    }
}