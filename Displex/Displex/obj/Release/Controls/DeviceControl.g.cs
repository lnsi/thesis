﻿#pragma checksum "..\..\..\Controls\DeviceControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4D79DD94CE051C3E03F035A78D3A91E2"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4214
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Displex;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Controls.ContactVisualizations;
using Microsoft.Surface.Presentation.Controls.Primitives;
using Microsoft.Surface.Presentation.Generic;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VncSharp;


namespace Displex.Controls {
    
    
    /// <summary>
    /// DeviceControl
    /// </summary>
    public partial class DeviceControl : Microsoft.Surface.Presentation.Controls.SurfaceUserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\Controls\DeviceControl.xaml"
        internal System.Windows.Controls.Grid MainGrid;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\Controls\DeviceControl.xaml"
        internal System.Windows.Media.ImageBrush MainBg;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\Controls\DeviceControl.xaml"
        internal VncSharp.RemoteDesktopWPF rdfWPF;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Controls\DeviceControl.xaml"
        internal System.Windows.Controls.Grid Footer;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Displex;component/controls/devicecontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\DeviceControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.MainGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.MainBg = ((System.Windows.Media.ImageBrush)(target));
            return;
            case 3:
            this.rdfWPF = ((VncSharp.RemoteDesktopWPF)(target));
            return;
            case 4:
            this.Footer = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
