﻿#pragma checksum "..\..\..\..\Views\Random.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "BE8CF954D2B44F743CC712CCA1675B9E3841E3CE"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ten kod został wygenerowany przez narzędzie.
//     Wersja wykonawcza:4.0.30319.42000
//
//     Zmiany w tym pliku mogą spowodować nieprawidłowe zachowanie i zostaną utracone, jeśli
//     kod zostanie ponownie wygenerowany.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// Random
    /// </summary>
    public partial class Random : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 43 "..\..\..\..\Views\Random.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.TextBox PasswordTextBox;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\..\Views\Random.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton ToggleUppercase;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\..\Views\Random.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton ToggleLowercase;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\..\Views\Random.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton ToggleNumbers;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\..\Views\Random.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton ToggleSpecialChars1;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\..\..\Views\Random.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton TogglePunctuation;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\..\Views\Random.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton ToggleSpecialChars2;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\..\Views\Random.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider PasswordLengthSlider;
        
        #line default
        #line hidden
        
        
        #line 116 "..\..\..\..\Views\Random.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider PasswordStrengthSlider;
        
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
            System.Uri resourceLocater = new System.Uri("/CerberPass;component/views/random.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\Random.xaml"
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
            
            #line 23 "..\..\..\..\Views\Random.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Cancel);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 33 "..\..\..\..\Views\Random.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Accept);
            
            #line default
            #line hidden
            return;
            case 3:
            this.PasswordTextBox = ((Wpf.Ui.Controls.TextBox)(target));
            
            #line 49 "..\..\..\..\Views\Random.xaml"
            this.PasswordTextBox.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.PasswordTextBox_MouseDown);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 58 "..\..\..\..\Views\Random.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RefreshPasswordIcon_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ToggleUppercase = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 6:
            this.ToggleLowercase = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 7:
            this.ToggleNumbers = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 8:
            this.ToggleSpecialChars1 = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 9:
            this.TogglePunctuation = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 10:
            this.ToggleSpecialChars2 = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 11:
            this.PasswordLengthSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 114 "..\..\..\..\Views\Random.xaml"
            this.PasswordLengthSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.PasswordLengthSlider_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 12:
            this.PasswordStrengthSlider = ((System.Windows.Controls.Slider)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
