using System.Linq;
using Vipl.AcsGenerator.LogicalElements;

namespace Vipl.AcsGenerator.VisualElements
{
    public abstract class CheckBoxVisualGroup : ICheckBoxVisualElement
    {
        public virtual string Variable { get; protected init; }
        protected abstract string  GetGroupGuiElement(string style);
        
        protected abstract SimpleCheckBoxVisualElement[] Elements { get; }
        // ReSharper disable once CoVariantArrayConversion
        public  string GetGuiElement(string style) =>
            $@"flowcontainer = {{ 
    direction = vertical
    spacing = 5
    {GetGroupGuiElement(style).Intend(1)}
    { Elements.Select(t => t.GetGuiElement($"{style}_moved")).Join(1)}
}}";
        
        protected CheckboxLogicalGroup LogicalOwner => Elements[0].CheckBoxLogicalOwner;

        public string ScriptedGuiName => $"{LogicalOwner.ScriptedGuiName}_group";
        public abstract string GetSetScopes(int value);
        public abstract Trait[] Traits { get; }

        public string CheckBoxFrameSelector
            => $@"frame = ""[Select_int32( {this.IsShown(1)}, '(int32)2' , Select_int32( {this.IsShown(2)} , '(int32)3', Select_int32( {this.IsShown(0)}  , '(int32)1', '(int32)4' ) ) )]""";

        public abstract string[] Localizations { get; }
    }
}