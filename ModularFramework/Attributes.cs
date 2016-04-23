using System;

namespace ModularFramework {
    public class SharedPropertyAttribute : Attribute {
        public SharedPropertyAttribute() : this(null) { }
        public SharedPropertyAttribute(string propertyName) {
            Name = propertyName;
        }
        public string Name { get; private set; }
    }
    public class ModularFrameworkAttribute: Attribute {}
    public class ModuleNameAttribute : ModularFrameworkAttribute {
        public ModuleNameAttribute(string name) {
            Name = name;
        }
        public string Name { get; private set; }
    }
    public class ModuleVersionAttribute : ModularFrameworkAttribute {
        public ModuleVersionAttribute(string version) {
            Version.TryParse(version, out versionCore);
        }
        Version versionCore;
        public Version Version { get { return versionCore; } }
    }
    public class ModuleDescriptionAttribute : ModularFrameworkAttribute {
        public ModuleDescriptionAttribute(string description) {
            Description = description;
        }
        public string Description { get; private set; }
    }
    public class ModuleCompanyNameAttribute : ModularFrameworkAttribute {
        public ModuleCompanyNameAttribute(string companyName) {
            CompanyName = companyName;
        }
        public string CompanyName { get; private set; }
    }
    public class ModuleCompanyInfoAttribute : ModularFrameworkAttribute {
        public ModuleCompanyInfoAttribute(string companyInfo) {
            CompanyInfo = companyInfo;
        }
        public string CompanyInfo { get; private set; }
    }
    public class ModuleLoadAfterAttribute : ModularFrameworkAttribute {
        public ModuleLoadAfterAttribute(params string[] items) {
            After = items;
        }
        public string[] After { get; private set; }
    }
    public class ModuleRequireAttribute : ModularFrameworkAttribute {
        public ModuleRequireAttribute(params string[] items) {
            Requires = items;
        }
        public string[] Requires { get; private set; }
    }

}