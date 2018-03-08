# LazyMeter

LazyMeter is app that tracks you're activity on your desktop and helps to warn You when You're being to lazy!

## Technique

To get information about running applications on machine I used functions from System.Windows.Automation Namespace. This solution turned out to work pretty well although I had top implement some additional filtering of UI effect that were "cached" by algorithm.

## Built With

* [WPF](https://msdn.microsoft.com/pl-pl/library/mt149842.aspx) - Framework for desktop applications
* [LiveCharts](https://lvcharts.net/) - Library for creating stunning graphs and charts
