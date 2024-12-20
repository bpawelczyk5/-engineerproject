﻿#pragma checksum "..\..\..\..\Views\Passwords.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "F385D786B834D3508AABB9BDD1785BABE1C06CD8"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

using CerberPass.Converters;
using CerberPass.Views;
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
    /// Passwords
    /// </summary>
    public partial class Passwords : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 40 "..\..\..\..\Views\Passwords.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.InfoBar infoCopy;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\..\Views\Passwords.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.ListView dataListView;
        
        #line default
        #line hidden
        
        
        #line 292 "..\..\..\..\Views\Passwords.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.ContentDialog addPasswordDialog;
        
        #line default
        #line hidden
        
        
        #line 300 "..\..\..\..\Views\Passwords.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.TextBox nameTextBox;
        
        #line default
        #line hidden
        
        
        #line 311 "..\..\..\..\Views\Passwords.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.TextBox usernameTextBox;
        
        #line default
        #line hidden
        
        
        #line 315 "..\..\..\..\Views\Passwords.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.PasswordBox passwordTextBox;
        
        #line default
        #line hidden
        
        
        #line 323 "..\..\..\..\Views\Passwords.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.TextBox urlTextBox;
        
        #line default
        #line hidden
        
        
        #line 327 "..\..\..\..\Views\Passwords.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.Button addButton;
        
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
            System.Uri resourceLocater = new System.Uri("/CerberPass;component/views/passwords.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\Passwords.xaml"
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
            this.infoCopy = ((Wpf.Ui.Controls.InfoBar)(target));
            return;
            case 2:
            
            #line 53 "..\..\..\..\Views\Passwords.xaml"
            ((Wpf.Ui.Controls.TextBox)(target)).TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.dataListView = ((Wpf.Ui.Controls.ListView)(target));
            return;
            case 10:
            
            #line 288 "..\..\..\..\Views\Passwords.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.addPasswordDialog = ((Wpf.Ui.Controls.ContentDialog)(target));
            
            #line 294 "..\..\..\..\Views\Passwords.xaml"
            this.addPasswordDialog.ButtonClicked += new Wpf.Ui.Controls.TypedEventHandler<Wpf.Ui.Controls.ContentDialog, Wpf.Ui.Controls.ContentDialogButtonClickEventArgs>(this.CloseButton_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.nameTextBox = ((Wpf.Ui.Controls.TextBox)(target));
            return;
            case 13:
            this.usernameTextBox = ((Wpf.Ui.Controls.TextBox)(target));
            return;
            case 14:
            this.passwordTextBox = ((Wpf.Ui.Controls.PasswordBox)(target));
            return;
            case 15:
            this.urlTextBox = ((Wpf.Ui.Controls.TextBox)(target));
            return;
            case 16:
            this.addButton = ((Wpf.Ui.Controls.Button)(target));
            
            #line 330 "..\..\..\..\Views\Passwords.xaml"
            this.addButton.Click += new System.Windows.RoutedEventHandler(this.addPasswordDialog_ButtonClicked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 4:
            
            #line 104 "..\..\..\..\Views\Passwords.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CopyLoginButton_Click);
            
            #line default
            #line hidden
            break;
            case 5:
            
            #line 144 "..\..\..\..\Views\Passwords.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CopyPasswordButton_Click);
            
            #line default
            #line hidden
            break;
            case 6:
            
            #line 189 "..\..\..\..\Views\Passwords.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ShareButton_Click);
            
            #line default
            #line hidden
            break;
            case 7:
            
            #line 216 "..\..\..\..\Views\Passwords.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteButton_Click);
            
            #line default
            #line hidden
            break;
            case 8:
            
            #line 243 "..\..\..\..\Views\Passwords.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.FavouriteButton_Click);
            
            #line default
            #line hidden
            break;
            case 9:
            
            #line 264 "..\..\..\..\Views\Passwords.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.EditButton_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

