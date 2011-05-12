using System;
using System.Configuration;

namespace TeamDeploy.Config
{
    public class MachineElement : ConfigurationElement
    {
        [ConfigurationProperty("computername", IsRequired = true)]
        public String ComputerName
        {
            get
            {
                return (String)this["computername"];
            }
            set
            {
                this["computername"] = value;
            }
        }

        [ConfigurationProperty("username", IsRequired = false)]
        public String Username
        {
            get
            {
                return (String)this["username"];
            }
            set
            {
                this["username"] = value;
            }
        }


        [ConfigurationProperty("password", IsRequired = false)]
        public String Password
        {
            get
            {
                return (String)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }
    }
}