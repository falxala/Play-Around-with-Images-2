   M S B A M L   `   `   `  ���� P  LPlayAroundwithImages2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:PlayAroundwithImages2PlayAroundwithImages2  G CToggleSwitch, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK5clr-namespace:WpfControlLibrary;assembly=ToggleSwitchWpfControlLibrary %    PlayAroundwithImages2.SubWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5local#clr-namespace:PlayAroundwithImages2     NToggleSwitch5clr-namespace:WpfControlLibrary;assembly=ToggleSwitch     -     � Title$  	SubWindow��$	��600��$	��312�����	#FF464646$	��312��$	��600�� � 	Resources    NormalButtonBrush �� Color  NormalBackgroundBrush  HoverButtonBrush  HoverBackgroundBrush  HoverButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� Triggers	 P� Property
 P� Value �� 
TargetName��%�  &	        &	      &	 *     &	 =     &	 R     7   7   7  7  7  7  7  �� $ 	#FF7A7A7A���� $ 	#FF6A6A6A���� $ #ffffff���� $ 	#FF3E3E3E���� " ��.���  � ��8   ��  � �� �� " ��.R��� $ HoverButtonBorder��$��2q�8�� 8�� ._��� $��Center=�$��Center=� P� 	 �@J�IsMouseOver
 . . ��� $ HoverButtonBorder��8   � ���� $ HoverButtonBorder��8   � ���� 8   � ��P� 	 � ��
 . . ��� $ HoverButtonBorder��8   � ��.��� $	��1,0q� � RowDefinitions �� .+��� 
���   �$��1q�$	��126��$��10,143,10,0q�$	��Top=�._��� -    ��Name$ Rotate_Slider��$��	9,43,9,52q� � Maximum$	 180Q� � Minimum$
 -180Q��� 
���   �$��1q�$	��126��$��
10,12,10,0q�$	��Top=��� 
���   �$��1q�$	��114��$��10,274,10,0q�$	��Top=��� -    ��Name$ Select_folder��$��
0,303,18,0q�$	��Top=�$��23��$��Right=�$��28��
�������.���� $��Resources/Folder.png���� -    ��Name$ ComboBox_extension��$	��Top=�$��80,357,18,0q�$��14(�����  �� EndPoint$	 0,1&� �� 
StartPoint$	 0,0&�.���  � Color$ 	#FFF0F0F0�� � Offset$ 0Q�� $ 	#FFE5E5E5��$ 1Q��� -    ��Name$ Width_TextBox��$
��Left=�$��22��$��	60,43,0,0q� �� TextWrapping$
 Wrap=�$��1��$	��Top=�$��60��$��14(�$��Center=��� -   $ Height_TextBox��$��22��$��
200,43,0,0q�$
 Wrap=�$��1��$	��Top=�$
��Left=�$��60��$��14(�$��Center=��� -    ��Name$ Dock_checkbox��$
��Dock��$
��Left=�$��	10,0,0,10q� J� RenderTransformOrigin$ 0.5,0.5&�$��15��$��Bottom=�
�������z�V� .~���  �� ScaleX$	 1.2Q� �� ScaleY$	 1.2Q��� �� T� �� $��
18,360,0,0q�$
��Wrap=�$��Format��$	��Top=�$��22��$
��Left=�$��62��
�������$��14(��� $
��Left=�$��	18,44,0,0q�$
��Wrap=�$��Width��$	��Top=�$��23��$��14(�
��������� $
��Left=�$��
153,44,0,0q�$
��Wrap=�$��Height��$	��Top=�$��14(�$��23��$ 0.553,0.174&�
��������� -   $ ChainButton��$
��Left=�$��
147,72,0,0q�$	��Top=�$��24��$��24�����	#FFE5E5E5��Z� ��Z� .���� $��Resources/UnChain.png���  � Points �   ��� �;G��� 
�������$��2�� �� StrokeDashArray$ 1R�$��	87,70,0,0q�$
��Left=�$��59��$��19��$	��Top=��  �   �;G�;G��� ��� 
�������$��2��$ 1R�$��
175,70,0,0q�$
��Left=�$��62��$��18��$	��Top=��� $
��Left=�$��
120,46,0,0q�$
��Wrap=�$
��[px]��$	��Top=�
��������� $
��Left=�$��
260,46,0,0q�$
��Wrap=�$
��[px]��$	��Top=�
��������� -	   $ tf_checkbox��$��Transform (priority)��$
��Left=�$��	16,18,0,0q�$	��Top=�$	��160��$��19��$	 0,0&�
�������
�������$��4,-1,0,0q� d� 	IsChecked$
 TrueY�z�V� .~��� $	 1.2Q�$	 1.2Q��� �� T� �� -
   $ selectDirTextBox��$��23��$��18,303,51,0q�$��TextBox��$	��Top=�$��14(� �� MaxLines$ 1���� -   $ ow_checkbox��$��Overwrite if same name��$
��Left=�$��
17,333,0,0q�$	��Top=�$	��160��$	 0,0&�
�������
�������z�V� .~��� $	 1.2Q�$	 1.2Q��� �� T� �� $��
18,278,0,0q�$
��Wrap=�$��
OutputPath��$	��Top=�$��22��$
��Left=�$��96��$��14(�
��������� -   $ Info_TextBox��$��10,461,10,37q�$
 Wrap=����	#FF575757���	#FF0AE6BE�� $��Debug��$
��Left=�$��
11,431,0,0q�$	��Top=����	#FF0AE6BE$��14(��� -   $ 
OpenFolder��#��[ $��Open save directory��$
��Left=�$��
10,393,0,0q�$	��Top=�$	��136��
�������$��24��$��14(��� $��1q�$��36��$��10,102,10,0q�$	��Top=����	#FF595959$  WpfControlLibrary.ToggleSwitch  -      Name$  LiquidRescale_toggle��$
��Left=�$��24��$��240,108,0,0q�$	��Top=�$��40��!   CircleBorderColor$! White��"   CircleBackgroundColor$" White��#   Duration$	# 0.1Q�$   TrackBackgroundOnColor$$ 	#FF319C3F���� $
��Left=�$��
18,108,0,0q�$
��Wrap=�$	��Top=�$��24��$��14(�
��������� $�Liquid���� $� ���� $�Rescale���� $� ���� $�(���� $�v���� $
�ery ���� $�s���� $
�low)���� $
��Left=�$��
18,156,0,0q�$
��Wrap=�$��Rotate��$	��Top=�$��23��$��14(�
�������$��56���� $
��Left=�$��
18,223,0,0q�$
��Wrap=�$	��Top=�$��23��$��14(�
�������$��56���� $�M���� $�irror��  -   $  Mirror_toggle��$
��Left=�$��24��$��240,223,0,0q�$	��Top=�$��40��$! White��$" White��$	# 0.1Q�$$ 	#FF319C3F���� -   $ Rotate_TextBox��$��22��$��200,155,0,0q�$
 Wrap=�$��0��$	��Top=�$
��Left=�$��47��$��14(�$��Center=�$��Center=��� $
��Left=�$��252,156,0,0q�$
��Wrap=�$	��deg��$	��Top=�$��23��$��14(�
�������$��28��