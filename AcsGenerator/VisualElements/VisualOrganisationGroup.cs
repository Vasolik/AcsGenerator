using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Vipl.AcsGenerator.Layouts;

namespace Vipl.AcsGenerator.VisualElements
{
    public interface ILocalizable
    {
        public string[] Localizations { get; }
    }
    [DebuggerDisplay("{Name} - {Localization}")]
    public class VisualOrganisationGroup : ILayout
    {
        public static readonly IList<VisualOrganisationGroup> All = new List<VisualOrganisationGroup>();
        public VisualOrganisationGroup( string name,  string localization)
        {
            Variable =  $"acs_filter_trait_group_{name}";
            Name = name;
            Localization = localization;
            All.Add(this);
        }

        public IList<ILayout> Layouts { get; } = new List<ILayout>();

        public static void Parse(string toParse)
        {
            VisualOrganisationGroup last = null;
            string layoutType = null;
            foreach (var row in toParse.Tokenized())
            {
                var groupRegex = new Regex("^=(?<name>\\w+):(?<type>\\w+)\\s+(?<localization>\"[^\"]+\")");
                var groupInfo = groupRegex.Match(row);
                if (groupInfo.Success)
                {
                    last = new VisualOrganisationGroup(groupInfo.Groups["name"].Value,
                        groupInfo.Groups["localization"].Value);
                    layoutType = groupInfo.Groups["type"].Value;
                    continue;
                }
                var layoutRegex = new Regex("^\\+(?<layout>\\w+)");
                var layoutInfo = layoutRegex.Match(row);
                if (layoutInfo.Success)
                {
                    layoutType = layoutInfo.Groups["layout"].Value;
                    continue;
                }

                last?.Layouts.Add( layoutType switch
                {
                    "n" => new NormalLayout(row),
                    "s" => new StarLayout(row),
                    "t" => new ThreePackLayout(row),
                    "6pack" => new SixPackLayout(row),
                    "e" => Skill.ParseTwoSkillsLayout(row),
                    "cg" => new CustomLayout(row),
                    _ => throw new Exception("Invalid layout.txt file")
                });
            }
        }

        public string Variable { get; }
        public string Name { get;  init; }
        public string Localization { get;  init; }

        public Trait[] Traits =>
            Layouts.SelectMany(l => l.Traits).ToArray();

        public string GuiElement =>
$@"types acs_filter_trait_{Name}_group_types {{
    type acs_filter_trait_{Name}_group =  acs_vbox_filter_group_trait {{
        parentanchor = top|left
        blockoverride ""acs_vbox_filter_group_name"" {{
            text = ""{Variable}""
        }}

        blockoverride ""acs_vbox_filter_group_toggle"" {{
            onclick = ""[GetVariableSystem.Toggle( '{Variable}' )]""
            frame = ""[Select_int32( Not( GetVariableSystem.Exists( '{Variable}' ) ), '(int32)1', '(int32)2' )]""
        }}
        blockoverride ""filters"" {{
            flowcontainer = {{
                visible = ""[GetVariableSystem.Exists( '{Variable}' )]""
                position = {{ 0 26 }}
                direction = vertical
                spacing = 5
                {Layouts.Select(e => e.GuiElement).Join(4)}
            }}    
        }}
    }}
}}";


        public string ScriptedGui =>    
            Layouts.Select(e => e.ScriptedGui).Join();

        public string[] Localizations
            => new[] {$" {Variable}:0 {Localization}"}
                .Concat(Layouts.SelectMany(e => e.Localizations)).ToArray();

        public static string CompleteLocalization
            => "l_english:\n" +
                All.SelectMany(o => o.Localizations.Where(l => l is not null)).Join();
        
        public static void GenerateGuiElement(string path)
        {
            foreach (var visualOrganisation in All)
            {
                File.WriteAllText($"{path}gui/acs_filter_trait_{visualOrganisation.Name}.gui",visualOrganisation.GuiElement, new System.Text.UTF8Encoding(true));
            }
        }
        public static void GenerateScriptedGui(string path)
        {
            foreach (var visualOrganisation in All)
            {
                File.WriteAllText($"{path}common/scripted_guis/acs_filter_trait_{visualOrganisation.Name}.txt",visualOrganisation.ScriptedGui, new System.Text.UTF8Encoding(true));
            }
        }
        
        public static void GenerateGuiCall()
        {
            Console.WriteLine(All.Select(o => $"acs_filter_trait_{o.Name}_group = {{ }}").Join(8, true));
        }
    }
}