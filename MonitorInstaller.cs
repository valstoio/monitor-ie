﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Runtime.InteropServices;

namespace vMonitor
{
    [RunInstaller(true)]
    public partial class MonitorInstaller : System.Configuration.Install.Installer
    {
        public MonitorInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.RegisterAssembly(this.GetType().Assembly, AssemblyRegistrationFlags.SetCodeBase))
            {
                throw new InstallException("Failed To Register for COM");
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.UnregisterAssembly(this.GetType().Assembly))
            {
                throw new InstallException("Failed To Unregister for COM");
            }
        }
    }
}
