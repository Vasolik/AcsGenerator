using System;
using System.Linq;
using System.Text.RegularExpressions;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public class CustomLayout : ILayout
    {
        public CustomLayout(IVisualElement[] leftElements, IVisualElement[] rightElements)
        {
            LeftElements = leftElements;
            RightElements = rightElements;
        }

        public CustomLayout(string row)
        {
            var regex = new Regex(@"(?<left>(?:(?:(?:\w+:g)|(?:\w+))\s+)*(?:(?:(?:\w+:g)|(?:\w+))\s*))\s*\-\s*(?<right>(?:(?:(?:\w+:g)|(?:\w+))\s+)*(?:(?:(?:\w+:g)|(?:\w+))\s*))");
            var rowInfo = regex.Match(row);
            if(!rowInfo.Success)
                throw new Exception("Invalid layout file");
            LeftElements = rowInfo.Groups["left"].Value.Tokenized(true)
                .Select(GetVisualElement).ToArray();
            RightElements = rowInfo.Groups["right"].Value.Tokenized(true)
                .Select(GetVisualElement).ToArray();
        }

        public Trait[] Traits
            => LeftElements.SelectMany(e => e.Traits)
                .Concat(RightElements.SelectMany(e => e.Traits)).ToArray();

        private static IVisualElement GetVisualElement(string t)
        {
            if (t.Contains(":"))
            {
                return CustomCheckBoxVisualGroup.All[t.Replace(":g", "")];
            }

            return Trait.All[t];
        }

        public IVisualElement[] LeftElements { get; }
        public IVisualElement[] RightElements { get; }

        public string GuiElement => 
            $@"flowcontainer = {{
    flowcontainer = {{ 
        direction = vertical
        spacing = 5
        {LeftElements.Select(e => e.GetGuiElement("left")).Join(2)}
    }}
    flowcontainer = {{ 
        direction = vertical
        spacing = 5
        {RightElements.Select(e => e.GetGuiElement("right")).Join(2)}
    }}
}}";
        public string ScriptedGui 
            =>  LeftElements.Select(e => e.ScriptedGui).Join()
               + "\n" + RightElements.Select(e => e.ScriptedGui).Join();
        
        public string[] Localizations
            => LeftElements.SelectMany(e => e.Localizations)
                .Concat(RightElements.SelectMany(e => e.Localizations)).ToArray();
    }
}