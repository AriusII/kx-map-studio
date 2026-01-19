namespace KXMapStudio.Libs.ViewModels.RightSide.GridEditor;

public sealed partial class GridEditorRowViewModel : ObservableObject
{
	[ObservableProperty] private int _id;
	[ObservableProperty] private string _name = string.Empty;
	[ObservableProperty] private double _x;
	[ObservableProperty] private double _y;
	[ObservableProperty] private double _z;
}
