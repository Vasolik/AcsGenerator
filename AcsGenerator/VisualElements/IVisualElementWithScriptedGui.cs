using System.Collections.Generic;

namespace Vipl.AcsGenerator.VisualElements
{
    public interface IVisualElementWithScriptedGui : IVisualElement
    {
        public string ScriptedGuiName { get; }
        public string GetSetScopes(int value);
    }

    public static class VisualElementExtension
    {
        public static string GetDigit(this IVisualElementWithScriptedGui _, int digit)
            => $"GetDummyMale.MakeScope.Var( 'acs_digit_{digit}' ).Province.MakeScope";

        public static string IsShown(this IVisualElementWithScriptedGui element, int index)
            => $"ScriptedGui.IsShown( {element.GetSetScopes(index)} )";
    }
    
    
}