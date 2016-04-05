namespace ModularFramework {
    public delegate void ModuleLoadedEventHandler(object sender, ModuleLoadedEventArgs ea);
    public class ModuleLoadedEventArgs {
        public ModuleLoadedEventArgs(IModuleInfo moduleInfo) {
            ModuleInfo = moduleInfo;
        }
        public IModuleInfo ModuleInfo { get; private set; }
    }
    public delegate void ModuleUnloadedEventHandler(object sender, ModuleUnloadedEventArgs ea);
    public class ModuleUnloadedEventArgs {

        public ModuleUnloadedEventArgs(IModuleInfo moduleInfo) {
            ModuleInfo = moduleInfo;
        }
        public IModuleInfo ModuleInfo { get; private set; }
    }
}
