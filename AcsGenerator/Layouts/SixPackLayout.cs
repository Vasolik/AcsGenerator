using System;
using System.Linq;
using System.Text.RegularExpressions;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public class SixPackLayout : ILayout
    {
        public SixPackLayout(string name, IVisualElement[] elements, string localization)
        {
            Elements = elements;
            Localization = localization;
            Name = name;
            
        }
        public SixPackLayout(string row)
        {
        
            var sixPackRegex = new Regex("^(?<traits>\\w+(?:\\s+\\w+){5})\\s+(?<name>\\w+)\\s+(?<localization>\"[^\"]+\")");
            var rowInfo = sixPackRegex.Match(row);
            if(!rowInfo.Success)
                throw new Exception("Invalid layout file");

            Name = rowInfo.Groups["name"].Value;
            Localization = rowInfo.Groups["localization"].Value;
            Elements = rowInfo.Groups["traits"].Value
                .Tokenized(true)
                .Select(x => Trait.All[x])
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
        public string ScriptedGui =>
            Elements.Select(e => e.ScriptedGui).Join();
        public Trait[] Traits
            => Elements.SelectMany(e => e.Traits).ToArray();
        
        public string[] Localizations
            => new[] {$" {Name}:0 {Localization}"}
                .Concat(Elements.SelectMany(e => e.Localizations)).ToArray();

    }
}