   M S B A M L   `   `   `  ����O  KClientAddressManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:taskBDtaskBD  5          taskBD.AddressesPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      &localclr-namespace:taskBD     5      -   5          N� Title$  Адреса��5	       � 	Resources 5
         PersonsForComboBoxSource  CountriesForComboBoxSource  RegionsForComboBoxSource  CitiesForComboBoxSource ! FilterCitiesForComboBoxSource��%�   &	        &	      &	 &     &	 9     &	 L     �� 5   
   6   �� 5   
   6   �� 5   
   6   �� 5   
   6   �� 5   
   6   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��
10,10,10,5q�6   
�������$��1q�6J    �� Padding$ 5q�6^   ._�5      ��  �� Orientation$ 
Horizontal=�6   .+�5       �� $%��Фильтр по городу:��6   $��Center=�65   $��0,0,5,0q�6P   �� 5!      -   5         ��Name$ CmbFilterCity��5!      $	��200��62   $��0,0,10,0q�6>   $��Center=�6P    �� DisplayMemberPath$
 Name��5#       �� SelectedValuePath$ ID��5$      ��5"      ��  �� Source# [ 5%      �� 5&      -   5        	 ��Name$	 BtnClearAddressFilter��5&      $��Сброс фильтра��68   $
��10,2q�6P   6_   5'      5(      &   System.Windows.Controls.DataGrid  5*   
   -   5        
  Name$
 AddressesDataGrid��5*      $��1��6.   $��
10,0,10,10q�6;      AutoGenerateColumns .     CanUserAddRows .     SelectionMode$ Single=�5,   9      AlternatingRowBackground
 �����   RowBackground
 �����
�������$��1q�5.   &   ��5,      �� *AddressesList+!   RowValidationErrorTemplate 50      �� 51      .R�52      � $��	0,-2,0,-2q�6    � ToolTip 53      ��  �� RelativeSource �� *FindAncestor+ �� AncestorType)  #System.Windows.Controls.DataGridRow"   �� Path$) #(Validation.Errors)[0].ErrorContent�.+�54      A� $��0��6"   
���  ��$��12��6A   $��12��6L   �� 55      $��!��6$   $��10(�6-   $
��Bold"�6;   
�������$��Center=�6`   $��Center=�6}   $��0,-1,0,0q�6�   56      57         Columns 5:      0  *System.Windows.Controls.DataGridTextColumn  5;      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      
IsReadOnly .    Width5  /System.Windows.Controls.DataGridLengthConverter$ 40 6Z   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *ID+6Z   4  .System.Windows.Controls.DataGridComboBoxColumn  5=      $ Клиент��6)   $	 150 69      SelectedValuePath$ ID��5@   )      DisplayMemberPath$ Surname��5A   )      ItemsSource 5>   )   �� # [     SelectedValueBinding 5?   )   �� *
PersonID+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5A   )     5D      $ Индекс��6%   $ 70 65    5E      �� 5F      $ IndexAddress�6"   $ TwoWay=�66   $ PropertyChanged=�6D     �� TargetNullValue$   ��6j   5H        5K      $ Страна��6)   $	 150 69   $ ID��5N   )   $ NameFull��5O   )    5L   )   �� # [  5M   )   �� *	CountryID+$ TwoWay=�$ PropertyChanged=�5O   )     5Q      $ Регион��6)   $	 150 69   $ ID��5T   )   $
 Name��5U   )    5R   )   �� # [  5S   )   �� *
RegionID+$ TwoWay=�$ PropertyChanged=�$   ��5U   )     5W      $ 
Город��6)   $	 150 68   $ ID��5Z   )   $
 Name��5[   )    5X   )   �� # [  5Y   )   �� *CityID+$ TwoWay=�$ PropertyChanged=�$   ��5[   )     5]      $ 
Улица��6%   $ * 64    5^      �� 5_      $ Street�6"   $ TwoWay=�60   $ PropertyChanged=�6>   ! �� ValidationRules! 5`         taskBD.RequiredFieldRule  5a   "   "   	FieldName$" 
Улица��6:   5c      #   EditingElementStyle# 5e      �� 5f      $ �� 
TargetType"$ ��6    % �� Triggers% 5g      P� 5h   "   & P� Property& �@=�HasError' P� Value' . . �5i   &   �� ( �� Property( �@�ToolTip) �� Value) 6@   ��  �� *Self+$) #(Validation.Errors)[0].ErrorContent��� 5j   &   ( � ��
) �����6C   5k   #   5m      5o        5q      $ Дом��6%   $ 70 62    5r      �� 5s      $ Building�6"   $ TwoWay=�62   $ PropertyChanged=�6@   ! 5t        5u   "   $" Дом��6:   5w      # 5y      �� 5z      "$ ��6    % 5{      P� 5|   "   & �@=�HasError' . . �5}   &   �� ( �@�ToolTip) 6@   ��  �� *Self+$) #(Validation.Errors)[0].ErrorContent��� 5~   &   ( � ��
) �����6C   5   #   5�      5�        5�      $ Офис��6%   $ 70 63    5�      �� 5�      $ Office�6"   $ TwoWay=�60   $ PropertyChanged=�6>   $   ��6d   5�      5�      �� 5�   
   $��2��6   $ 
Horizontal=�6"   $��Right=�6;   $��10q�6W   .+�5�      �� -   5        $��Добавить��5�      $��5q�6(   $
��10,5q�63   6B   �� 5�      -   5        $��Удалить��5�      $��5q�6'   $
��10,5q�62   6A   �� 5�      -   5        $+��%Сохранить изменения��5�      $��5q�63   $
��10,5q�6>   6M   �� 5�      -   5        $��Обновить��5�      $��5q�6(   $
��10,5q�63   6B   5�      5�      5�      