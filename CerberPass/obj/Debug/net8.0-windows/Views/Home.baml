   M S B A M L   `   `   `  ����E  ACerberPass, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:CerberPass
CerberPass  5      U QSystem.Runtime, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a7,clr-namespace:System;assembly=System.RuntimeSystem 5          CerberPass.Views.Home   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      *localclr-namespace:CerberPass     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      ?system,clr-namespace:System;assembly=System.Runtime     5      A =Wpf.Ui, Version=3.0.5.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5	      .��5      � .+�5   
     Wpf.Ui.Controls.TextBox  -   5           Name$  	searchBox��5      $��70,120,430,0q�5      $	��Top=�5         PlaceholderText$ Szukaj��5         Icon 5      '  !Wpf.Ui.Markup.SymbolIconExtension  *
Search32+5      � 5   
   -   5         �Name$ favoritesWrapPanel��5      $��70,180,70,0q�5      $	��Top=�5       � Orientation$ 
Horizontal=�5      � 5   
   -   5        $ recentlyCopiedWrapPanel��5       $��70,517,70,0q�5!      $	��Top=�5"      $ 
Horizontal=�5#        Wpf.Ui.Controls.TextBlock  5&   
   $��
70,466,0,0q�5'      $
��Left=�5(      $	��Top=�5)         FontTypography$ Subtitle=�5*      
�������$��Ostatnio kopiowane��5,      #  Wpf.Ui.Controls.ContentDialog  5.   
   -   5          Name$ FavouritePasswordDialog��5/         Title$ Dane logowania��50         CloseButtonText$ Zamknij��52      $y�	Collapsed=�53      .��54      �� $	��349��6   $	��270��6%   .+�55        $��Nazwa:��6     56      -   5        $  readNameTextBox��57      $��0,10,0,0q�58      $ Nazwa��59      �� 5;      $��0,10,0,0q�6   	 �� Orientation$	 
Horizontal=�6/   .+�5<        $��Login:��6#     5=      $��	142,0,0,0q�6#   $��Hasło:��66   5>      �� 5@      $��0,10,0,0q�6   $	 
Horizontal=�6/   .+�5A        -   5        $  readUsernameTextBox��5B      $	��170��5C      $ Login��5D      !  Wpf.Ui.Controls.PasswordBox  5E      -   5        
  Name$
 readPasswordTextBox��5F      $	��170��5G      $��9,0,10,0q�5H      $ Hasło��5I      5J        5L      $��0,10,0,0q�6   $
��URL:��61     5M      -   5        $  readUrlTextBox��5N      $��0,10,0,0q�5O      $	 URL��5P      5Q      5R      5S      5T      