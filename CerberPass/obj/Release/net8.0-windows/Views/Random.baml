   M S B A M L   `   `   `  ���� E  ACerberPass, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:CerberPass.ViewsCerberPass.Views  U QSystem.Runtime, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a7,clr-namespace:System;assembly=System.RuntimeSystem     CerberPass.Views.Random   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     0localclr-namespace:CerberPass.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ?system,clr-namespace:System;assembly=System.Runtime     A =Wpf.Ui, Version=3.0.4.0, Culture=neutral, PublicKeyToken=null/ui&http://schemas.lepo.co/wpfui/2022/xaml  .��� .+�  Wpf.Ui.Controls.Button  -   $��0��$	��126��$��50��$��0,723,241,0q�$��Right=�$	��Top=�$��Anuluj��  -   $��0��$	��126��$��50��$��
0,723,95,0q�$��Right=�$	��Top=�    
Appearance$  Primary=�$��Akceptuj��� $��75,120,132,0q�$	��Top=� � RowDefinitions �� �� $��8* �.+�  Wpf.Ui.Controls.TextBox  -   $��PasswordTextBox��$��1��$
��Auto��$��45,3,0,0q�$	��Top=� �� 
IsReadOnly$
 True��   PlaceholderText$  ��  -   $��1��$��45��$��36��$��-5,3,0,0q�$	��Top=�   Icon '  !Wpf.Ui.Markup.SymbolIconExtension  *ArrowSync16+d� -   $��ToggleUppercase��$��95��$��
75,255,0,0q�$	��Top=�$	��A-Z�� d� 	IsChecked$
 TrueY�d� -   $��ToggleLowercase��$��95��$��251,255,0,0q�$	��Top=�$	��a-z��$
 TrueY�d� -   $��ToggleNumbers��$��95��$��439,255,0,0q�$	��Top=�$	��0-9��$
 TrueY�d� -   $��ToggleSpecialChars1��$��95��$��673,255,0,0q�$	��Top=�$��!@#$%^��$
 TrueY�d� -	   $��TogglePunctuation��$��95��$��851,255,0,0q�$	��Top=�$
��;',.��$
 TrueY�d� -
   $��ToggleSpecialChars2��$��95��$��1023,255,0,0q�$	��Top=�$
��?+_{��$
 TrueY��� -   $��PasswordLengthSlider��$��75,340,132,416q� � Maximum$ 32Q� � Minimum$ 0Q� �� TickFrequency$ 1Q�	 �� TickPlacement$
	 Both=��� -   $��PasswordStrengthSlider��$��293,186,350,582q�${�False��$	 100Q�$ 0Q�