using System.Collections.Generic;

namespace Vipl.AcsGenerator.VisualElements
{
   
    public interface IVisualElement : ILocalizable
    {
        public string Variable { get; }
        string GetGuiElement(string style);
        Trait[] Traits { get; }
        public string ScriptedGuiName { get; }
        public string GetSetScopes(int value);
        
        

    }

    public static class VisualElementExtension
    {
        public static string GetDigit(this IVisualElement _, int digit)
            => $"GetDummyMale.MakeScope.Var( 'acs_digit_{digit}' ).Province.MakeScope";

        public static string IsShown(this IVisualElement element, int index)
            => $"ScriptedGui.IsShown( {element.GetSetScopes(index)} )";
    }
    
    
}