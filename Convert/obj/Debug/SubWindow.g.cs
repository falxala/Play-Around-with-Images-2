﻿#pragma checksum "..\..\SubWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E23B6FE25BCA680D607F20E4472D3865E2A53B83D842AC8647A83DED1D48C4E7"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

using PlayAroundwithImages2;
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


namespace PlayAroundwithImages2 {
    
    
    /// <summary>
    /// SubWindow
    /// </summary>
    public partial class SubWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\SubWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Select_folder;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\SubWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ComboBox_extension;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\SubWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Width_TextBox;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\SubWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Height_TextBox;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\SubWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox Dock_checkbox;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\SubWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ChainButton;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\SubWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox tf_checkbox;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\SubWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox selectDirTextBox;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\SubWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ow_checkbox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PlayAroundwithImages2;component/subwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SubWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\SubWindow.xaml"
            ((PlayAroundwithImages2.SubWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 8 "..\..\SubWindow.xaml"
            ((PlayAroundwithImages2.SubWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            
            #line 8 "..\..\SubWindow.xaml"
            ((PlayAroundwithImages2.SubWindow)(target)).LocationChanged += new System.EventHandler(this.Window_LocationChanged);
            
            #line default
            #line hidden
            
            #line 8 "..\..\SubWindow.xaml"
            ((PlayAroundwithImages2.SubWindow)(target)).MouseMove += new System.Windows.Input.MouseEventHandler(this.Window_MouseMove);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Select_folder = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\SubWindow.xaml"
            this.Select_folder.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ComboBox_extension = ((System.Windows.Controls.ComboBox)(target));
            
            #line 19 "..\..\SubWindow.xaml"
            this.ComboBox_extension.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ComboBox_extension_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Width_TextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 27 "..\..\SubWindow.xaml"
            this.Width_TextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.Width_TextBox_TextChanged);
            
            #line default
            #line hidden
            
            #line 27 "..\..\SubWindow.xaml"
            this.Width_TextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.Width_TextBox_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 5:
            this.Height_TextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 28 "..\..\SubWindow.xaml"
            this.Height_TextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.Height_TextBox_TextChanged);
            
            #line default
            #line hidden
            
            #line 28 "..\..\SubWindow.xaml"
            this.Height_TextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.Height_TextBox_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            case 6:
            this.Dock_checkbox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 29 "..\..\SubWindow.xaml"
            this.Dock_checkbox.Click += new System.Windows.RoutedEventHandler(this.CheckBox_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ChainButton = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\SubWindow.xaml"
            this.ChainButton.Click += new System.Windows.RoutedEventHandler(this.ChainButton_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.tf_checkbox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 49 "..\..\SubWindow.xaml"
            this.tf_checkbox.Unchecked += new System.Windows.RoutedEventHandler(this.tf_checkbox_Unchecked);
            
            #line default
            #line hidden
            
            #line 49 "..\..\SubWindow.xaml"
            this.tf_checkbox.Checked += new System.Windows.RoutedEventHandler(this.tf_checkbox_Checked);
            
            #line default
            #line hidden
            return;
            case 9:
            this.selectDirTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 59 "..\..\SubWindow.xaml"
            this.selectDirTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.selectDirTextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.ow_checkbox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 60 "..\..\SubWindow.xaml"
            this.ow_checkbox.Click += new System.Windows.RoutedEventHandler(this.ow_checkbox_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

