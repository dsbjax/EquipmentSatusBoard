﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EquipmentSatusBoard.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\KBR Wyle\\Equipment Status Board")]
        public string AppDataFolder {
            get {
                return ((string)(this["AppDataFolder"]));
            }
            set {
                this["AppDataFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\KBR Wyle\\Equipment Status Board\\Saved Pages")]
        public string SavedPagesFolder {
            get {
                return ((string)(this["SavedPagesFolder"]));
            }
            set {
                this["SavedPagesFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\KBR Wyle\\Equipment Status Board\\Images")]
        public string ImagesFolder {
            get {
                return ((string)(this["ImagesFolder"]));
            }
            set {
                this["ImagesFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\current.status")]
        public string CurrentStatusBoardFilename {
            get {
                return ((string)(this["CurrentStatusBoardFilename"]));
            }
            set {
                this["CurrentStatusBoardFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\esb.phones")]
        public string SavedPhoneNumbersFilename {
            get {
                return ((string)(this["SavedPhoneNumbersFilename"]));
            }
            set {
                this["SavedPhoneNumbersFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\esb.notes")]
        public string SavedNotesFilename {
            get {
                return ((string)(this["SavedNotesFilename"]));
            }
            set {
                this["SavedNotesFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\esbpwd.bin")]
        public string PasswordFilename {
            get {
                return ((string)(this["PasswordFilename"]));
            }
            set {
                this["PasswordFilename"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\radars.status")]
        public string SavedRadarsFilename {
            get {
                return ((string)(this["SavedRadarsFilename"]));
            }
            set {
                this["SavedRadarsFilename"] = value;
            }
        }
    }
}
