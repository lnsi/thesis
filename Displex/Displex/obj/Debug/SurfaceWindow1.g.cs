﻿#pragma checksum "..\..\SurfaceWindow1.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EB421399976B07B0E49BC4CB8B0D960A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4214
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Controls.ContactVisualizations;
using Microsoft.Surface.Presentation.Controls.Primitives;
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


namespace Displex {
    
    
    /// <summary>
    /// SurfaceWindow1
    /// </summary>
    public partial class SurfaceWindow1 : Microsoft.Surface.Presentation.Controls.SurfaceWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\SurfaceWindow1.xaml"
        internal Microsoft.Surface.Presentation.Controls.ScatterView Displex;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\SurfaceWindow1.xaml"
        internal Microsoft.Surface.Presentation.Controls.ScatterViewItem DisplayExtension;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\SurfaceWindow1.xaml"
        internal VncSharp.RemoteDesktopWPF rdfWPF;
        
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
            System.Uri resourceLocater = new System.Uri("/OpenTable;component/surfacewindow1.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SurfaceWindow1.xaml"
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
            this.Displex = ((Microsoft.Surface.Presentation.Controls.ScatterView)(target));
            return;
            case 2:
            this.DisplayExtension = ((Microsoft.Surface.Presentation.Controls.ScatterViewItem)(target));
            return;
            case 3:
            this.rdfWPF = ((VncSharp.RemoteDesktopWPF)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
