   M S B A M L   `   `   `  ����Q  MVisiotechSystemMonitor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:VisiotechSystemMonitor.ViewModels!VisiotechSystemMonitor.ViewModels  5   	   '    !VisiotechSystemMonitor.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   >vm/clr-namespace:VisiotechSystemMonitor.ViewModels     5   	     � Title$  System Monitor��5   	   $	��450��6    $	��800��6-   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5	      $��* �6   .+�5   
   ��  �� Orientation$ 
Horizontal=�6   $��0,0,0,10q�6.   $	��Top=�6@   .+�5      �� $��Start / Stop��6   $	��100��6Q   $��0,0,10,0q�6]   ��6,   �� *StartStopCommand+6]   �� 5      $��Interval (ms):��6   $��Center=�6.   $��0,0,5,0q�6I   �� 5      $��80��6a   ��6   �� *IntervalMilliseconds+ �� UpdateSourceTrigger$ PropertyChanged=�.��6l   ��5      �� 5       �� 
TargetType" ��6    �� Triggers 5      �� 5       �� Binding 6*   �� *IsIntervalValid+ �� Value	 False6N   .a�5   "   ��  �� Property � ��	 �� Value
	 �����6?   �� 5   "    �@�ToolTip$/	 )Interval must be between 500 and 10000 ms��6<   5      5      .��5   !   5      5      &   System.Windows.Controls.DataGrid  5   
   $��1��6   
   AutoGenerateColumns$
 False��6@      
IsReadOnly$
 True��6\   ��6    �� *	Samples+   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$
 Time��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 63   �� *	Timestamp+  5       $ 
CPU Serial��6%    69   �� *	CpuSerial+  5!      $ Motherboard��6%    6:   �� *MotherboardSerial+  5"      $	 GPU��6%    62   �� *	GpuSerial+  5#      $ CPU Usage %��6%    6:   �� *
CpuUsage+  5$      $ RAM Usage %��6%    6:   �� *
RamUsage+5&      5'      5(      