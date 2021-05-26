using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public class SixPackLayout : ILayout
    {
        public SixPackLayout(XmlElement element)
        {
            Name = element.GetAttribute("localizationKey");
            Localization = element.GetAttribute("localization");
            Elements = element.ChildNodes.OfType<XmlElement>()
                .Select(x => SimpleCheckBoxVisualElement.All[x.GetAttribute("name")])
                .Cast<IVisualElement>()
                .ToArray();
        }
        private string Name { get; }
        private string Localization { get; }
        public IVisualElement[] Elements { get; }
   
        public string GuiElement =>
            $@"flowcontainer = {{
    widget = {{
        size = {{ 69 65 }}
        text_single = {{
            position = {{ 7 20 }}
            layoutpolicy_horizontal = expanding
            block ""acs_filter_name"" {{  
                text = ""{Name}""
            }}
            default_format = ""#medium""
            
            align = bottom|right
        }}
    }}
    spacing = 5
    flowcontainer = {{
        direction = vertical
        spacing = 5
        flowcontainer = {{
            {Elements.Take(3).Select(e => e.GetGuiElement("right_small")).Join(3)}
        }}
        flowcontainer = {{
            {Elements.Skip(3).Select(e => e.GetGuiElement("right_small")).Join(3)}
        }}
    }}
}}";
        public Trait[] Traits
            => Elements.SelectMany(e => e.Traits).ToArray();
        
        public string[] Localizations
            => new[] {$" {Name}:0 \"{Localization}\""}
                .Concat(Elements.SelectMany(e => e.Localizations)).ToArray();

    }
}