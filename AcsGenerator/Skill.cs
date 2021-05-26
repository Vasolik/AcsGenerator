using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Vipl.AcsGenerator.Layouts;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator
{
    public class Skill : CheckBoxVisualGroup
    {
        public Skill(string name,  int index)
        {
            var traitRegex = new Regex($"(education_{name}_[1-4]|education_martial_{name}_[1-4])");
            Traits = Trait.All.Values.Where(t => traitRegex.Match(t.Name).Success).ToArray();
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
        protected override SimpleCheckBoxVisualElement[] Elements => Traits.Clone() as SimpleCheckBoxVisualElement[] ;
        public override string GetSetScopes(int value) =>
            Index == 6 ? GetSetScopeForProwess(value) : GetSetScopeForSmallGeneral(value);
        
        public string MajorDigit => this.GetDigit(Elements.Min(e => e.Index) / 40);
        public string MinorDigit => this.GetDigit(Elements.Min(e => e.Index) % 40);
        public string GetSetScopeForProwess(int value ) => $"GuiScope.SetRoot( {this.GetDigit(value)} ).End";
        public string GetSetScopeForSmallGeneral(int value ) => 
            $"GuiScope.SetRoot( {this.GetDigit(value)} )" +
            $".AddScope( 'major_digit' , {MajorDigit} )" +
            $".AddScope( 'minor_digit' , {MinorDigit} )" +
            ".End";
        public override Trait[] Traits { get; }
        public string Name { get; }
        public int Index { get; }
        public override string Variable => $"acs_filter_trait_education_{Name}";
        public override string[] Localizations
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
                new IVisualElement[]{All[element["LeftSkill"].GetAttribute("name")]}, 
                new IVisualElement[]{All[element["RightSkill"].GetAttribute("name")]});
        }
    }
}