﻿#pragma checksum "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F81BDCDD8B3F719EB01A3667A67F30E2"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.1008
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using KöTaf.WPFApplication.Converter;
using KöTaf.WPFApplication.Models;
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


namespace KöTaf.WPFApplication.Views.Client {
    
    
    /// <summary>
    /// pEditClientRevenues
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class pEditClientRevenues : KöTaf.WPFApplication.Views.KPage, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 32 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dtgRevenue2;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn revName;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTemplateColumn revStartDate;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTemplateColumn revEndDate;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn revAmount;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblAmountText;
        
        #line default
        #line hidden
        
        
        #line 110 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblAmount;
        
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
            System.Uri resourceLocater = new System.Uri("/KöTaf.WPFApplication;component/views/client/peditclientrevenues.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
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
            this.dtgRevenue2 = ((System.Windows.Controls.DataGrid)(target));
            
            #line 32 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
            this.dtgRevenue2.SelectedCellsChanged += new System.Windows.Controls.SelectedCellsChangedEventHandler(this.dtgRevenue2_SelectedCellsChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.revName = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 4:
            this.revStartDate = ((System.Windows.Controls.DataGridTemplateColumn)(target));
            return;
            case 5:
            this.revEndDate = ((System.Windows.Controls.DataGridTemplateColumn)(target));
            return;
            case 6:
            this.revAmount = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 8:
            this.lblAmountText = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.lblAmount = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 2:
            
            #line 45 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
            ((System.Windows.Controls.ComboBox)(target)).SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cbRevTyp_SelectionChanged);
            
            #line default
            #line hidden
            break;
            case 7:
            
            #line 91 "..\..\..\..\..\Views\Client\pEditClientRevenues.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.pbAddDelRev_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

