﻿#pragma checksum "..\..\..\..\..\Views\User\pNewUser.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6316A4F14DBB979D5A558C28E9CCB3F5"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.1008
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using KöTaf.WPFApplication.Views;
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
using System.Windows.Shell;


namespace KöTaf.WPFApplication.Views.User {
    
    
    /// <summary>
    /// pNewUser
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class pNewUser : KöTaf.WPFApplication.Views.KPage, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\..\..\..\Views\User\pNewUser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbBenutzername;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\..\Views\User\pNewUser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox tbPasswort;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\..\..\Views\User\pNewUser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox tbPasswortConfirmation;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\..\Views\User\pNewUser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkbIsAdmin;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\..\..\Views\User\pNewUser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbImage;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\..\..\Views\User\pNewUser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button browse;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\..\Views\User\pNewUser.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image userImage;
        
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
            System.Uri resourceLocater = new System.Uri("/KöTaf.WPFApplication;component/views/user/pnewuser.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\User\pNewUser.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.tbBenutzername = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.tbPasswort = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 3:
            this.tbPasswortConfirmation = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 4:
            this.chkbIsAdmin = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.tbImage = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.browse = ((System.Windows.Controls.Button)(target));
            
            #line 25 "..\..\..\..\..\Views\User\pNewUser.xaml"
            this.browse.Click += new System.Windows.RoutedEventHandler(this.browse_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.userImage = ((System.Windows.Controls.Image)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

