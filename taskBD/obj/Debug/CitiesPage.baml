   M S B A M L   `   `   `  ����O  KClientAddressManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:taskBDtaskBD  5          taskBD.CitiesPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      &localclr-namespace:taskBD     5      -   5          N� Title$  Города��5	       � 	Resources 5
         RegionsForComboBoxSource  CountriesForComboBoxSource��%:   &	        &	      �� 5   
   6   �� 5   
   6   5      .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ CitiesDataGrid��5      $��0��6+   $��10q�68      AutoGenerateColumns .     CanUserAddRows .     SelectionMode$ Single=�5   6   ��6   �� *
CitiesList+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%   	   
IsReadOnly	 . 1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 61   �� *ID+6H     5      $ Наименование��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6�   
 6;   �� *Name+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   4  .System.Windows.Controls.DataGridComboBoxColumn  5      $ Регион��6)   $	 200 69      SelectedValuePath$ ID��5         DisplayMemberPath$
 Name��5   )      ItemsSource 5   )   ��  �� Source# [     SelectedValueBinding 5   )   �� *
RegionID+$ TwoWay=�$ PropertyChanged=� �� TargetNullValue$  ��5   )     5      $ Страна��6-   $	 200 6=   $ ID��5"   )   $ NameFull��5#   )    5    )   �� # [  5!   )   �� *	CountryID+$ TwoWay=�$ PropertyChanged=�5#   )   5%      �� 5'   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   $��10q�6W   .+�5(      �� -   5        $��Добавить��5(      $��5q�6(   $
��10,5q�63   6B   �� 5)      -   5        $��Удалить��5)      $��5q�6'   $
��10,5q�62   6A   �� 5*      -   5        $+��%Сохранить изменения��5*      $��5q�63   $
��10,5q�6>   6M   �� 5+      -   5        $��Обновить��5+      $��5q�6(   $
��10,5q�63   6B   5,      5-      5.      