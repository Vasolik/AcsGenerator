using System.Collections.Generic;

namespace Vipl.AcsGenerator.VisualElements
{
   
    public interface IVisualElement : ILocalizable
    {
        public string Variable { get; }
        string GetGuiElement(string style);
        string ScriptedGui { get; }
        Trait[] Traits { get; }
       
    }
    
    
}