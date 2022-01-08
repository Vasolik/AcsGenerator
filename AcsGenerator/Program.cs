using Vipl.AcsGenerator.LogicalElements;
using Vipl.AcsGenerator.SaveLoad;
using Vipl.AcsGenerator.VisualElements;

const string modFolder = "mod/";

if (Directory.Exists(modFolder))
{
    Directory.Delete(modFolder, true);
}
Directory.CreateDirectory($"{modFolder}common/scripted_triggers");
Directory.CreateDirectory($"{modFolder}common/scripted_guis");
Directory.CreateDirectory($"{modFolder}common/scripted_effects");
Directory.CreateDirectory($"{modFolder}common/script_values");
Directory.CreateDirectory($"{modFolder}gui");
Directory.CreateDirectory($"{modFolder}localization/english");
Directory.CreateDirectory("report");

LogicalOrganisationGroup.PrepareLogicalElements();
CustomCheckBoxVisualGroup.Parse();
VisualOrganizationGroup.Parse();



File.WriteAllText($"{modFolder}/common/scripted_triggers/acs_st_big_switch.txt",LogicalOrganisationGroup.SwitchTrigger, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}/common/scripted_guis/acs_sg_filter_large_groups.txt",LogicalOrganisationGroup.ScriptedGui, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}localization/english/acs_filter_trait_l_english.yml",VisualOrganizationGroup.GetCompleteLocalization(LocalizationFiles.TraitFile), new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}localization/english/acs_filter_dropdown_l_english.yml",VisualOrganizationGroup.GetCompleteLocalization(LocalizationFiles.DropDownFile), new System.Text.UTF8Encoding(true));
VisualOrganizationGroup.GenerateGuiElement(modFolder);
VisualOrganizationGroup.GenerateGuiCall();

SaveSlotGenerator.GenerateSaveSlot();
File.WriteAllText($"{modFolder}common/script_values/acs_sv_filter_save.txt",SaveSlotGenerator.SelectedSlot, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}common/scripted_effects/acs_se_load.txt",SaveSlotGenerator.LoadFromSlots, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}common/scripted_effects/acs_se_save.txt",SaveSlotGenerator.SaveToSlots, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}common/scripted_effects/acs_se_load_undo.txt",SaveSlotGenerator.LoadFromUndo, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}common/scripted_effects/acs_se_save_undo.txt",SaveSlotGenerator.SaveToUndo, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}common/scripted_effects/acs_se_reset_filters.txt",SaveSlotGenerator.Reset, new System.Text.UTF8Encoding(true));
File.WriteAllText($"{modFolder}common/scripted_effects/acs_se_make_reduced_and_count.txt",SaveSlotGenerator.MakeReducedListAndCount, new System.Text.UTF8Encoding(true));
var reportFile = new StreamWriter("report/report.txt");

reportFile.Close();