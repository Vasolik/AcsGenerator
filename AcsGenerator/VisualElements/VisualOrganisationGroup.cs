using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

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

        public static void Parse()
        {
            
            var layoutsDocument = new XmlDocument();
            layoutsDocument.Load("layout.xml");
            
            foreach (var element in layoutsDocument.GetElementsByTagName("Main").OfType<XmlElement>().First()!.ChildNodes.OfType<XmlElement>())
            {
                switch (element.Name)
                {
                    case "VisualOrganizationGroup":
                        
                        var group = new VisualOrganisationGroup(element.GetAttribute("name"), element.GetAttribute("localization"));
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
                                _ => throw new Exception("Invalid layout.txt file")
                            });
                        }
                        
                        break;
                }
            }
        }

        public string Variable { get; }
        public string Name { get;  init; }
        public string Localization { get;  init; }

        public Trait[] Traits =>
            Layouts.SelectMany(l => l.Traits).ToArray();

        public string GuiElement =>
$@"types acs_filter_trait_{Name}_group_types {{
    type acs_filter_trait_{Name}_group =  acs_vbox_filter_group {{
        parentanchor = top|left
        blockoverride ""acs_vbox_filter_group_name"" {{
            text = ""{Variable}""
        }}

        blockoverride ""acs_vbox_filter_group_toggle"" {{
            onclick = ""[GetVariableSystem.Toggle( '{Variable}' )]""
            frame = ""[Select_int32( Not( GetVariableSystem.Exists( '{Variable}' ) ), '(int32)1', '(int32)2' )]""
        }}
        blockoverride ""acs_filter_visible"" {{
            visible = ""[GetVariableSystem.Exists( '{Variable}' )]""
        }}

        blockoverride ""acs_filters"" {{
            {Layouts.Select(e => e.GuiElement).Join(3)}
        }}
    }}
}}";




        public string[] Localizations
            => new[] {$" {Variable}:0 \"{Localization}\""}
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

        
        public static void GenerateGuiCall()
        {
            Console.WriteLine(All.Select(o => $"acs_filter_trait_{o.Name}_group = {{ }}").Join(8, true));
        }
    }
}