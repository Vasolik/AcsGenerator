using System.Linq;

namespace Vipl.AcsGenerator.VisualElements
{
    public abstract class CheckBoxVisualGroup : ICheckBoxVisualElement
    {
        public virtual string Variable { get; protected init; }
        protected abstract string  GetGroupGuiElement(string style);
        
        public abstract SimpleCheckBoxVisualElement[] Elements { get; }
        // ReSharper disable once CoVariantArrayConversion
        public ISimpleVisualElement[] CastedElements => Elements;
        public  string GetGuiElement(string style) =>
            $@"flowcontainer = {{ 
    direction = vertical
    spacing = 5
    {GetGroupGuiElement(style).Intend(1)}
    { Elements.Select(t => t.GetGuiElement($"{style}_moved")).Join(1)}
}}";

        public virtual string ScriptedGui =>
            Elements.Select(e => e.ScriptedGui).Join() + "\n" +ScriptedGuiForThisElement;
        private string ScriptedGuiForThisElement =>
            $@"{Variable}_negative = {{
    is_shown = {{
        AND = {{
            {CastedElements.Select(e => e.VariableNoCondition).Join(3)}
        }}
    }}
}}
{Variable}_none = {{
    is_shown = {{
        NOR = {{
            {CastedElements.Select(e => e.HasVariableCondition).Join(3)}
        }}
    }}
}}
{Variable} = {{
 
    is_shown = {{
        AND = {{
            {CastedElements.Select(e => e.VariableYesCondition).Join(3)}
        }} 
    }} 
        
    effect = {{
        acs_save_undo_0_filters = yes
        if = {{
            limit = {{ 
                NOR = {{
                    {CastedElements.Select(e => e.HasVariableCondition).Join(5)}
                }}
            }}
            {CastedElements.Select(e => e.SetVariableYesScript).Join(3)}
        }}
        else = {{
            if = {{
                limit = {{
                    AND = {{
                        {CastedElements.Select(e => e.VariableYesCondition).Join(6)}
                    }}
                }}
                {CastedElements.Select(e => e.SetVariableNoScript).Join(4)}
            }}
            else = {{
                {CastedElements.Select(e => e.ClearVariableScript).Join(3)}
            }}

        }}
        acs_auto_apply_sorting_and_filters = yes
    }}
}}";

        public abstract Trait[] Traits { get; }

        public string CheckBoxFrameSelector
            => $@"frame = ""[Select_int32( GetScriptedGui( '{Variable}' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)2', Select_int32( GetScriptedGui( '{Variable}_negative' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)3', Select_int32( GetScriptedGui( '{Variable}_none' ).IsShown( GuiScope.SetRoot( GetPlayer.MakeScope ).End ) , '(int32)1', '(int32)4' ) ) )]""";

        public abstract string[] Localizations { get; }
    }
}