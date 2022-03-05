using System.Text.RegularExpressions;
using System.Xml;
using Vipl.AcsGenerator.Layouts;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator;

public class Skill : CheckBoxVisualGroup
{
    public Skill(string name,  int index)
    {
        var traitRegex = new Regex($"(education_{name}_[1-4]|education_martial_{name}_[1-4])");
        Elements = Trait.All.Values.Where(t => traitRegex.Match(t.Name).Success).Cast<SimpleCheckBoxVisualElement>().ToArray();
        Name = name;
        Index = index;
    }

    public static IDictionary<string, Skill> All => AllSkillsPrivate.Value;
    private static readonly Lazy<IDictionary<string, Skill>> AllSkillsPrivate = new ( () => 
        new Skill[] {
            new ("diplomacy", 1),
            new ("martial", 2),
            new ("stewardship", 3),
            new ("intrigue", 4),
            new ("learning", 5),
            new ("prowess", 6),
        }.ToDictionary(s=> s.Name, s => s));
    protected override SimpleCheckBoxVisualElement[] Elements { get; } 
    public override string GetSetScopes(int value) =>
        Index == 6 ? GetSetScopeForProwess(value) : GetSetScopeForSmallGeneral(value);
        
    public string GetSetScopeForProwess(int value ) => $"GuiScope.AddScope( 'ctrl_value', {this.GetDigit(value)} ).End";
    public string GetSetScopeForSmallGeneral(int value ) => 
        $"GuiScope.AddScope('ctrl_value', {this.GetDigit(value)} )" +
        $".AddScope( 'position' , {this.GetDigit(Elements.Min(e => e.Index))} )" +
        ".End";
       
    public string Name { get; }
    public int Index { get; }
    public override LocalizationEntry[] Localizations
        => Elements.SelectMany(e => e.Localizations).ToArray();

    protected override string GetGroupGuiElement(string style) =>
        $@"acs_trait_item_skill_{style} = {{
    datacontext = ""[GetScriptedGui( '{ScriptedGuiName}' )]"" 
    blockoverride ""acs_trait_item_action"" {{
        onclick = ""[ScriptedGui.Execute( {GetSetScopes(0)} )]""
    }}
    blockoverride ""acs_checkbox_state"" {{
        {CheckBoxFrameSelector}
    }}
    blockoverride ""acs_filter_name"" {{  
        text = ""[{Name}|E]""
    }}
    blockoverride ""acs_skill_frame"" {{  
        frame = {Index}
    }}
}}";
    public static ILayout ParseTwoSkillsLayout(XmlElement element)
    {

        return new CustomLayout( 
            new ICheckBoxVisualElement[]{All[element["LeftSkill"]!.GetAttribute("Name")]}, 
            new ICheckBoxVisualElement[]{All[element["RightSkill"]!.GetAttribute("Name")]});
    }
}