﻿#pragma checksum "..\..\..\..\..\..\Views\Accounting\Sums\pSumsBalances.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "210EC7A194C834388E83638698E1ECAD"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.296
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


namespace KöTaf.WPFApplication.Views.Accounting.Sums {
    
    
    /// <summary>
    /// pSumsBalances
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class pSumsBalances : KöTaf.WPFApplication.Views.KPage, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\..\..\..\Views\Accounting\Sums\pSumsBalances.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid AccountDataGrid;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\..\..\..\Views\Accounting\Sums\pSumsBalances.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn AccountNumber;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\..\..\..\Views\Accounting\Sums\pSumsBalances.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn AccountName;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\..\..\Views\Accounting\Sums\pSumsBalances.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn Description;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\..\..\Views\Accounting\Sums\pSumsBalances.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn Balance;
        
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
            System.Uri resourceLocater = new System.Uri("/KöTaf.WPFApplication;component/views/accounting/sums/psumsbalances.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\Views\Accounting\Sums\pSumsBalances.xaml"
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
            this.AccountDataGrid = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 2:
            this.AccountNumber = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 3:
            this.AccountName = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 4:
            this.Description = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            case 5:
            this.Balance = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

