   M S B A M L   `   `   `  ���� P  LPlayAroundwithImages2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Experiment.Wpf.BehaviorsExperiment.Wpf.Behaviors  =#clr-namespace:PlayAroundwithImages2PlayAroundwithImages2  &     PlayAroundwithImages2.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a3i+http://schemas.microsoft.com/xaml/behaviors  .b&clr-namespace:Experiment.Wpf.Behaviors  /local#clr-namespace:PlayAroundwithImages2  -   $	��600��$	��750��$	��750��$	��600�����	#FF464646  � Title$  Play Around with Images 2�� J� CommandBindings )  #System.Windows.Input.CommandBinding  -      Command$ Paste��  -   $ Delete��  -   $ 	SelectAll�� � 	Resources    NormalButtonBrush �� Color  NormalBackgroundBrush  HoverButtonBrush  HoverBackgroundBrush  HoverButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType	 �� Triggers
 P� Property P� Value �� 
TargetName��%�  &	        &	      &	 *     &	 =     &	 R     7   7   7  7  7  7  7  �� $ 	#FF7A7A7A���� $ 	#FF6A6A6A���� $ #ffffff���� $ 	#FF3E3E3E���� " ��.���  � ��8   ��  � �� �� " ��.R��� $��HoverButtonBorder��$��2q�8�� 8�� ._��� $��Center=�$��Center=�	 P� 
 �@J�IsMouseOver . . ��� $ HoverButtonBorder��8   � ���� $ HoverButtonBorder��8   � ���� 8   � ��P� 
 � �� . . ��� $ HoverButtonBorder��8   � ��.���  � ColumnDefinitions �� �� $	��274 � � RowDefinitions �� .+��� -    ��Name$ 	buttonDel��#��[ $��Delete��$��0,5,5,0q�$	��Top=�$��Right=�$��76��
�������$��20���� 
���   �$��1q�$	��194��$��0,5,10,0q�$	��Top=�$��Right=�$	��259��$��1��._��� -    ��Name$ preview_image���� -   $ Select_All_Button��#��[ $��
Select All��$��	0,5,167,0q�$	��Top=�$��Right=�$��76��
�������$��20���� -   $ Deselect_All_Button��#��[ $��Clear Select��$��0,5,86,0q�$	��Top=�$��Right=�$��76��
�������$��20���� -	    ��Name$ Slider1��$��
0,0,10,134q�$��14(�$��Right=�$	��254��$��1��$��22��$��Bottom=��� -
    ��Name$ limit_filesize_tb��$��174,0,0,161q� �� TextWrapping$
 Wrap=�$��5��$��Center=�$��Center=�$��14(�$
��Left=�$��50��$��1��$��27��$��Bottom=��� $��Limit File Size (JPEG)��$��	5,0,0,160q�$��14(�$��Stretch=�$
��Left=�$	��164��$��1��$��32��$��Bottom=�
��������� $��MB��$��
0,0,10,160q�$��14(�$��Center=�$��Right=�$��40��$��1��$��28��$��Bottom=�
��������� -   $ Convert_Button��#��[ $��CONVERT & COPY��$��	0,0,75,10q�$��18(�$��Right=�$	��189��$��51��$��Bottom=����	#FF999999
����������	#FF382F2F$��1�� � ToolTip$K E変換を実行し結果をクリップボードにコピーします�� ^� InitialShowDelay$	 500���� -   $ Slider2��$��	0,0,10,75q�$��14(�$��Right=�$	��254�� � Maximum$
 8192Q� � SmallChange$ 1Q�$��1��$��22��$��Bottom=��� -   $ limit_longside_tb��$��
0,0,50,101q�$
 Wrap=�$
��2048��$��Center=�$��Center=�$��14(�$��Right=�$��75��$��1��$��27��$��Bottom=��� $��Long Sides Pixel��$��5,0,0,97q�$��14(�$��Stretch=�$
��Left=�$	��113��$��1��$��32��$��Bottom=�
��������� $��px��$��
0,0,10,101q�$��14(�$��Center=�$��Right=�$��40��$��1��$��28��$��Bottom=�
��������� -    ��Name$ Image_ListView��$��
10,32,5,10q� J� 	AllowDrop . $��Disabled=�
�������
���   ����	#FF636363$
��Auto=����� *drop_Images+ � ContextMenu �� .@�m� -    m�Name$ ListView_CntxtMenu��$��Paste from clipboard��m� -   $ ListView_CntxtMenu2��$��Open selected item������ .R�� ���� .R�� $	��200��$	��200��$��0,0,0,30q�.+��� ���� *	thumbnail+�� $��Bottom=�$��	0,0,0,-25q����� *	File_name+�� -    ��Name$ dropHere_Text��$��10,266,10,241q�$
��Wrap=�$��Drop files here��$��36(�$��Dubai Medium)����	#FFCFCFCF . $��62��$	��658��$��Center=�$��Center=� �� TextAlignment$ Center=� �� .@�m� -   $��Paste from clipboard���� 
���   �$��1q�$
��Left=�$��5,286,0,200q�$	��259��$��1��._��� -   $ 
path_textB��$	��2,1q�$
��Wrap=�$��CharacterEllipsis=� �� LineStackingStrategy$ BlockLineHeight=�$��14(�
��������� $�[���� $
�Path���� $�]���� 
���   �$��1q�$
��Left=�$��77��$��	5,204,0,0q�$	��Top=�$	��259��$��1��._��� -   $ Detail_textB��$
��2,-1q�$
��Wrap=�$��14(�
��������� $�[���� $�Detail���� $�s]���� -   $ SettingWindow_Button��#��[ $��>>��$��	0,0,10,10q�$��18(�$��Right=�$��60��$��51��$��Bottom=����	#FF8F8F8F
����������	#FF404040$��1�� ^� ToolTip _� $��変換設定を開く���� 
���   �$��1q�$	��194��$��0,5,10,0q�$	��Top=�$��Right=�$	��259��$��1��._��� -   $ selected_TextB��$��WrapWithOverflow=�$��	TextBlock��$��18(�$
��9,-1q�$ Center=�$��Center=�$��Center=�$ BlockLineHeight=�
���������. �� -     ��Name$  Multiple_ChkBox��$��Multiple��$��16��$��10,9,0,0q�$
��Left=�$	��Top=�$
��Bold"�
�������