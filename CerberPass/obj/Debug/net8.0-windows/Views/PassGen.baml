   M S B A M L   `   `   `  ����E  ACerberPass, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:CerberPass.ViewsCerberPass.Views  5          CerberPass.Views.PassGen   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:CerberPass.Views     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      A =Wpf.Ui, Version=3.0.5.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  5      .��5      � .+�5   
     Wpf.Ui.Controls.Button  -   5        $��0��5      $	��126��5      $��50��5      $��0,723,241,0q�5      $��Right=�5      $	��Top=�5      $��Resetuj��5        5   
   -   5        $��0��5      $	��126��5      $��50��5      $��
0,723,95,0q�5      $��Right=�5      $	��Top=�5          
Appearance$  Primary=�5      $��Kopiuj��5!      � 5$   
   $��75,120,132,0q�6   $	��Top=�6%    � RowDefinitions 5%      �� 5&      �� 5'      $��8* �6    .+�5)        Wpf.Ui.Controls.TextBox  -   5        $��PasswordTextBox��5*      $��1��5+      $
��Auto��5,      $��45,3,0,0q�5-      $	��Top=�5.       �� 
IsReadOnly$
 True��5/         PlaceholderText$  ��51        53      -   5        $��1��54      $��45��55      $��36��56      $��-5,3,0,0q�57      $	��Top=�58         Icon 5:      '  !Wpf.Ui.Markup.SymbolIconExtension  *ArrowSync16+5;      d� 5>   
   -   5        $��ToggleUppercase��5?      $��95��5@      $��
75,255,0,0q�5A      $	��Top=�5B      $	��A-Z��5C       d� 	IsChecked$
 TrueY�5D      d� 5E   
   -   5        $��ToggleLowercase��5F      $��95��5G      $��251,255,0,0q�5H      $	��Top=�5I      $	��a-z��5J      $
 TrueY�5K      d� 5L   
   -   5        $��ToggleNumbers��5M      $��95��5N      $��439,255,0,0q�5O      $	��Top=�5P      $	��0-9��5Q      $
 TrueY�5R      d� 5S   
   -   5        $��ToggleSpecialChars1��5T      $��95��5U      $��673,255,0,0q�5V      $	��Top=�5W      $��!@#$%^��5X      $
 TrueY�5Y      d� 5Z   
   -	   5        $��TogglePunctuation��5[      $��95��5\      $��851,255,0,0q�5]      $	��Top=�5^      $
��;',.��5_      $
 TrueY�5`      d� 5a   
   -
   5        $��ToggleSpecialChars2��5b      $��95��5c      $��1023,255,0,0q�5d      $	��Top=�5e      $
��?+_{��5f      $
 TrueY�5g      �� 5j   
   -   5        $��PasswordLengthSlider��5k      $��75,340,132,416q�5l       � Maximum$ 32Q�5m       � Minimum$ 0Q�5n       �� TickFrequency$ 1Q�5o      	 �� TickPlacement$
	 Both=�5p      5q      �� 5r   
   -   5        $��PasswordStrengthSlider��5s      $��293,186,350,582q�5t      ${�False��5u      $	 100Q�5v      $ 0Q�5w      5x      5y      