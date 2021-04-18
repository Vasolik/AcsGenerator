using System;
using System.Linq;
using System.Text.RegularExpressions;
using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts
{
    public class StarLayout : ILayout
    {
        public StarLayout(IVisualElement top, IVisualElement left, IVisualElement right)
        {
            Top = top;
            Left = left;
            Right = right;
        }
        public StarLayout(string row)
        {
	        var starLayoutRegex = new Regex("^(?<top>\\w+)\\s+(?<left>\\w+)\\s+(?<right>\\w+)");
	        var rowInfo = starLayoutRegex.Match(row);
	        if (!rowInfo.Success)
		        throw new Exception("Invalid layout file");
	 
	        Top = Trait.All[rowInfo.Groups["top"].Value];
	        Left = Trait.All[rowInfo.Groups["left"].Value];
	        Right = Trait.All[rowInfo.Groups["right"].Value];
        }
        


        public IVisualElement Top { get; }
        public IVisualElement Left { get; }
        public IVisualElement Right { get; }
        public string GuiElement =>
            $@"widget = {{
	allow_outside = yes
	size = {{ 555 35 }}
	flowcontainer = {{ 
		position = {{ 0 -100 }}
		direction = vertical
		{Top.GetGuiElement("top").Intend(2)}
		flowcontainer = {{ 
			{Left.GetGuiElement("right").Intend(3)}
			{Right.GetGuiElement("left").Intend(3)}
		}}
	}}
}}";
        public string ScriptedGui => 
            Top.ScriptedGui + Environment.NewLine + Left.ScriptedGui + Environment.NewLine + Right.ScriptedGui;
        
        public Trait[] Traits
	        => Left.Traits.AsEnumerable()
		        .Concat(Right.Traits)
		        .Concat(Top.Traits)
		        .ToArray();
        public string[] Localizations
	        => Top.Localizations
		        .Concat(Left.Localizations)
		        .Concat(Right.Localizations)
		        .ToArray();
    }
}