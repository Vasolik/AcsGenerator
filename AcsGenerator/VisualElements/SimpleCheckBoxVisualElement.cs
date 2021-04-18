using System.Diagnostics;

namespace Vipl.AcsGenerator.VisualElements
{
    [DebuggerDisplay("{" + nameof(Variable) + "}")]
    public abstract class SimpleCheckBoxVisualElement : ISimpleVisualElement, ICheckBoxVisualElement
    {
        public string CheckBoxFrameSelector
            => $@"frame = ""[Select_int32( GetScriptedGui( '{Variable}' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)2', Select_int32( GetScriptedGui( '{Variable}_negative' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)3', '(int32)1' ) )]""";

        public virtual string Variable { get; protected init; }
        public abstract string GetGuiElement(string style);
        public string ScriptedGui =>
            $@"{Variable}_negative = {{
    is_shown = {{
        {((ISimpleVisualElement)this).VariableNoCondition}
    }}
}}

{Variable} = {{
 
    is_shown = {{ 
        {((ISimpleVisualElement)this).VariableYesCondition}
    }}  
        
    effect = {{
        acs_save_undo_0_filters = yes
        if = {{
            limit = {{ {((ISimpleVisualElement)this).DontHaveVariableCondition} }}
            {((ISimpleVisualElement)this).SetVariableYesScript}
        }}
        else = {{
            if = {{
                limit = {{ {((ISimpleVisualElement)this).VariableYesCondition} }}
                {((ISimpleVisualElement)this).SetVariableNoScript}
            }}
            else = {{
                {((ISimpleVisualElement)this).ClearVariableScript}
            }}

        }}
        acs_auto_apply_sorting_and_filters = yes
    }}
}}";

        public abstract Trait[] Traits { get; }
        public abstract string[] Localizations { get; }
    }
}