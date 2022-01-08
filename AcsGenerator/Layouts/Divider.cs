using Vipl.AcsGenerator.VisualElements;

namespace Vipl.AcsGenerator.Layouts;

public class Divider : ILayout
{

    public LocalizationEntry[] Localizations => Array.Empty<LocalizationEntry>();
    public string GuiElement => "divider = { size = { 482 4 } }";
}