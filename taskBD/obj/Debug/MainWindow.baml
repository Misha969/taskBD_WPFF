   M S B A M L   `   `   `  ����O  KClientAddressManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:ClientAddressManagerClientAddressManager  5   	       taskBD.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4local"clr-namespace:ClientAddressManager     5   	     � Title$6  0Менеджер Адресов Клиентов��5   	   $	��600��6+   $	��800��68    � WindowStartupLocation$ CenterScreen=�6D   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   
�������$��5q�6R   .+�5      �� -   5        $��Клиенты��5      $��5q�6'   $
��10,5q�62   6A   �� 5      -   5        $��Адреса��5      $��5q�6&   $
��10,5q�61   6@   o� 5      $��5q�6   .@�5      m� $��Справочники��6   $
��10,5q�60   .@�5      m� -   5        $��Города��5      6/   m� 5      -   5        $��Регионы��5      60   m� 5      -   5        $��Страны��5      6/   5      5      �� 5      -   5        $��
Выход��5      $��50,5,5,5q�6%   $
��10,5q�67   6F   5       � 5!   
   -   5          �Name$ 	MainFrame��5!      $��1��6#     � NavigationUIVisibility$ Hidden=�60   $��5q�6P   5#      5$      