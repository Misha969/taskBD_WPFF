   M S B A M L   `   `   `  ����O  KClientAddressManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:taskBDtaskBD  5          taskBD.PersonsPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      &localclr-namespace:taskBD     5      -   5          N� Title$  Клиенты��5	      .��5
      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��
10,10,10,5q�6   
�������$��1q�6J    �� Padding$ 5q�6^   ._�5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� $/��)Поиск (Фамилия/Имя/Email):��6   $��Center=�6>   $��0,0,5,0q�6Y   �� 5      -   5         ��Name$ TxtSearchTerm��5      $	��200��61   $��0,0,10,0q�6=   $��Center=�6O   6q   �� 5      -   5         ��Name$ BtnSearchPerson��5      $��
Найти��62   $��0,0,5,0q�6B   $
��10,2q�6S   6b   �� 5      -   5        $ BtnClearSearchPerson��5      $��
Сброс��67   $
��10,2q�6G   6V   5      5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ PersonsDataGrid��5      $��1��6,   $��
10,0,10,10q�69      AutoGenerateColumns .     CanUserAddRows .  	   SelectionMode$	 Single=�5    7   
   AlternatingRowBackground

 �����   RowBackground
 �����
�������$��1q�5"   *   ��5       �� *PersonsList+!   RowValidationErrorTemplate 5$      �� 5%      .R�5&      � $��	0,-2,0,-2q�6    � ToolTip 5'      ��  �� RelativeSource �� *FindAncestor+ �� AncestorType)  #System.Windows.Controls.DataGridRow"   �� Path$) #(Validation.Errors)[0].ErrorContent�.+�5(      A� $��0��6"   
���  ��$��12��6A   $��12��6L   �� 5)      $��!��6$   $��10(�6-   $
��Bold"�6;   
�������$��Center=�6`   $��Center=�6}   $��0,-1,0,0q�6�   5*      5+         Columns 5.      0  *System.Windows.Controls.DataGridTextColumn  5/      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      
IsReadOnly .    Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 6Z   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *ID+6Z     51      $ Фамилия��6%   $	 150 66    52      �� 53      $ Surname�6"    �� Mode$ TwoWay=�61    �� UpdateSourceTrigger$ PropertyChanged=�6?    �� ValidatesOnExceptions .  �� NotifyOnValidationError .  �� ValidationRules 55         taskBD.RequiredFieldRule  56   "      	FieldName$ Фамилия��6:   58         EditingElementStyle 5:      �� 5;       �� 
TargetType" ��6     �� Triggers 5<      P� 5=   "    P� Property �@=�HasError  P� Value  . . �5>   &   �� ! �� Property! �@�ToolTip" �� Value" 6@   ��  �� *Self+$) #(Validation.Errors)[0].ErrorContent��� 5?   &   ! � ��
" �����6C   �� 5@   &   ! � ��
" �  ��6D   5A   #   5C      5E        5G      $ Имя��6%   $	 150 62    5H      �� 5I      $
 Name�6"   $ TwoWay=�6.   $ PropertyChanged=�6<    .  .  5K        5L   "   $ Имя��6:   5O       5Q      �� 5R      " ��6     5S      P� 5T   "    �@=�HasError  . . �5U   &   �� ! �@�ToolTip" 6@   ��  �� *Self+$) #(Validation.Errors)[0].ErrorContent��� 5V   &   ! � ��
" �����6C   �� 5W   &   ! � ��
" �  ��6D   5X   #   5Z      5\        5^      $ Отчество��6%   $	 150 6�    67   �� *
Patronymic+$ TwoWay=�$ PropertyChanged=�# �� TargetNullValue$#  ��6�     5_      $ Телефон��6%   $	 120 6�    66   �� *PhoneNumber+$ TwoWay=�$ PropertyChanged=�$#  ��6�     5a      $ Email��6%   $ * 64    5b      �� 5c      $ Email�6"   $ TwoWay=�6/   $ PropertyChanged=�6=    .  . $#  ��5d   ^    5e          taskBD.EmailValidationRule  5f   "   5h       5j      �� 5k      " ��6     5l      P� 5m   "    �@=�HasError  . . �5n   &   �� ! �@�ToolTip" 6@   ��  �� *Self+$) #(Validation.Errors)[0].ErrorContent��� 5o   &   ! � ��
" �����6C   �� 5p   &   ! � ��
" �  ��6D   5q   #   5s      5u      5w      �� 5y   
   $��2��6   $ 
Horizontal=�6"   $��Right=�6;   $��10q�6W   .+�5z      �� -   5        $ BtnAddPerson��5z      $��Добавить��6+   $��5q�6>   $
��10,5q�6I   6X   �� 5{      -   5        $ BtnEditPerson��5{      $+��%Редактировать (окно)��6,   $��5q�6K   $
��10,5q�6V   {�.  6�   �� 5}      -   5        $ BtnDeletePerson��5}      $��Удалить��6.   $��5q�6@   $
��10,5q�6K   6Z   �� 5~      -	   5        $ BtnSaveChanges��5~      $+��%Сохранить изменения��6-   $��5q�6K   $
��10,5q�6V   6e   �� 5      -
   5        $ BtnRefreshPersons��5      $#��Обновить список��60   $��5q�6J   $
��10,5q�6U   6d   5�      5�      5�      