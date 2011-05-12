using System.Configuration;

namespace TeamDeploy.Config
{
    [ConfigurationCollection(typeof(MachineElementCollection), AddItemName = "machine")]
    public class MachineElementCollection : ConfigurationElementCollection
    {
        public MachineElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as MachineElement;
            }

            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MachineElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MachineElement) element).ComputerName;
        }
    }
}