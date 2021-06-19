using System.Collections.Generic;
using System.Diagnostics;
using Vipl.AcsGenerator.LogicalElements;
using Vipl.AcsGenerator.SaveLoad;

namespace Vipl.AcsGenerator.VisualElements
{
    [DebuggerDisplay("{" + nameof(Variable) + "}")]
    public abstract class SimpleCheckBoxVisualElement :  ICheckBoxVisualElement, ICheckboxLogicalElement
    {
        protected SimpleCheckBoxVisualElement(string id )
        {
            All[id] = this;
        }
        public static IDictionary<string, SimpleCheckBoxVisualElement> All { get; } = new Dictionary<string, SimpleCheckBoxVisualElement>();
        public string CheckBoxFrameSelector
            => $@"frame = ""[Select_int32( {this.IsShown(1)} , '(int32)2' ,  Select_int32( {this.IsShown(2)} , '(int32)3' , '(int32)1' ) )]""";

        public string Variable { get; protected init; }
        public abstract string GetGuiElement(string style);
        public int PositiveIndex => Index;
        public int NegativeIndex => Index + 1;
        public ISavable Owner { get; set; }
        ISavable ILogicalElement.Owner => Owner;
        public CheckboxLogicalGroup CheckBoxLogicalOwner => Owner as CheckboxLogicalGroup;
        public virtual string NegativeTrigger => null;
        public virtual string PositiveTrigger => null;
        public abstract LocalizationEntry[] Localizations { get; }
        
        public string SwitchTrigger =>
$@"{PositiveIndex} = {{
    {PositiveTrigger.Intend(1)} 
}}
{NegativeIndex} = {{ 
    {NegativeTrigger.Intend(1)}   
}}";
        public int NumberOfFlagsNeeded => 2;
        public int Index { get; set; }
        public int IndexInGroup { get; set; }

        public string IndexInGroupName => this.GetDigit(IndexInGroup);
        public string ScriptedGuiName => Owner.ScriptedGuiName;
        public string GetSetScopes(int value ) => $"GuiScope.AddScope( 'ctrl_value', {this.GetDigit(value)} )" +
            $".AddScope( 'position' , {this.GetDigit(Index)} )" +
            (!Owner.IsMain && !Owner.HaveSomethingToSave ? $".AddScope( 'element_in_group' , {IndexInGroupName} )" : "") +
            ".End";

        


    }
}