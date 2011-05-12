using System.Configuration;

namespace TeamDeploy.Config
{
    public class MachineConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("machines")]
        public MachineElementCollection Machines
        {
            get
            {
                return this["machines"] as MachineElementCollection;
            }
        }
    }
}
