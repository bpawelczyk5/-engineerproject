   M S B A M L   `   `   `  ����E  ACerberPass, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:CerberPass
CerberPass  5          CerberPass.MainWindow   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      *localclr-namespace:CerberPass     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      A =Wpf.Ui, Version=3.0.5.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5        � Title$  
MainWindow��5	      $
��1076��5
      $	��851��5      $	��900��5         ApplicationBackgroundBrush  Wpf.Ui.Markup.Design   
Background# �   5        TextFillColorPrimaryBrush   
Foreground# �  5      "  Wpf.Ui.Controls.FluentWindow!   ExtendsContentIntoTitleBar$
 True��5         WindowBackdropType$
 Mica=�5         WindowCornerPreference$ Round=�5       � WindowStartupLocation$ CenterScreen=�5      .��5      � .+�5   
    � -   5          �Name$ 	MainFrame��5        � NavigationUIVisibility$ Hidden=�6#   $  Wpf.Ui.Controls.NavigationView  5   
   $	��300��5      $��0,40,0,0q�5      	   IsBackButtonVisible$	 	Collapsed=�5      
   IsPaneToggleVisible$
 False��5         PaneDisplayMode$ 
LeftFluent=�5         	MenuItems 5       (  "Wpf.Ui.Controls.NavigationViewItem  5!      -   5        $��95��5"      $
��Home��5#      
 � Tag$
 Home��5&         Icon 5$      '  !Wpf.Ui.Markup.SymbolIconExtension  *Home24+5&        5'      -   5        $��95��5(      $
��Pass��5)      $
 Pass��5,       5*        *
Password24+5,        5-      -   5        $��95��5.      $��Vault��5/      $ Vault��52       50        *	Vault16+52        53      -   5        $��95��54      $��PassGen��55      $ PassGen��58       56        *Edit16+58      5:        Wpf.Ui.Controls.TitleBar  5<   
      Title$ 
CerberPass��5=      $��35��5>         Icon 5?        *ArrowLeft48+  Wpf.Ui.Controls.TextBlock  5A   
   $��
172,80,0,0q�5B      $
��Left=�5C      $	��Top=�5D         FontTypography$ 
TitleLarge=�5E      
�������$��
CerberPass��5G      �� 5I   
   -   5         ��Name$ MainContent��5I      $��
105,43,0,0q�6.   5J      5K      