﻿#pragma checksum "..\..\..\..\Views\Register.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "87FA38FA79F6E085A54A3DAE4B7A42E92BCC787C"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

using CerberPass;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Converters;
using Wpf.Ui.Markup;


namespace CerberPass.Views {
    
    
    /// <summary>
    /// Register
    /// </summary>
    public partial class Register : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 23 "..\..\..\..\Views\Register.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.TextBox dbNameTextBox;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\Views\Register.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.PasswordBox repeatPasswordBox;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Views\Register.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.PasswordBox passwordBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CerberPass;component/views/register.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\Register.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.dbNameTextBox = ((Wpf.Ui.Controls.TextBox)(target));
            return;
            case 2:
            this.repeatPasswordBox = ((Wpf.Ui.Controls.PasswordBox)(target));
            return;
            case 3:
            this.passwordBox = ((Wpf.Ui.Controls.PasswordBox)(target));
            return;
            case 4:
            
            #line 48 "..\..\..\..\Views\Register.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CreateDatabase_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 56 "..\..\..\..\Views\Register.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BackToLogin_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

