namespace Vipl.AcsGenerator.VisualElements
{
    public interface ISimpleVisualElement: IVisualElement
    {
        public string HasVariableCondition
            => $"has_global_variable = {Variable}";
        public string DontHaveVariableCondition =>
            $"NOT = {{  {HasVariableCondition} }}";
        public string VariableYesCondition
            => $"global_var:{Variable} = yes";
        public string VariableNoCondition
            => $"global_var:{Variable} = no";
        public string SetVariableYesScript
            => $"set_global_variable = {{  name = {Variable} value = yes }}";
        public string SetVariableNoScript
            => $"set_global_variable = {{  name = {Variable} value = no }}";
        public string ClearVariableScript
            => $"remove_global_variable = {Variable}";
    }
}