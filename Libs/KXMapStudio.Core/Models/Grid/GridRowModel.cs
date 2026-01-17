namespace KXMapStudio.Core.Models.Grid;

/// <summary>
///     Mutable model representing a grid row with property change notifications.
/// </summary>
public sealed class GridRowModel : INotifyPropertyChanged, IEquatable<GridRowModel>
{
	private int _id;
	private string _name = string.Empty;
	private double _x;
	private double _y;
	private double _z;

	public int Id
	{
		get => _id;
		set => SetField(ref _id, value);
	}

	public string Name
	{
		get => _name;
		set => SetField(ref _name, value ?? string.Empty);
	}

	public double X
	{
		get => _x;
		set => SetField(ref _x, value);
	}

	public double Y
	{
		get => _y;
		set => SetField(ref _y, value);
	}

	public double Z
	{
		get => _z;
		set => SetField(ref _z, value);
	}

	public bool Equals(GridRowModel? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return Id == other.Id && Name == other.Name && X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
			return;

		field = value;
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public static GridRowModel FromDto(GridRowDto dto)
	{
		return new GridRowModel
		{
			Id = dto.Id,
			Name = dto.Name,
			X = dto.X,
			Y = dto.Y,
			Z = dto.Z
		};
	}

	public GridRowDto ToDto()
	{
		return new GridRowDto(Id, Name, X, Y, Z);
	}

	public GridRowModel Clone()
	{
		return new GridRowModel
		{
			Id = Id,
			Name = Name,
			X = X,
			Y = Y,
			Z = Z
		};
	}

	public override bool Equals(object? obj)
	{
		return obj is GridRowModel other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, Name, X, Y, Z);
	}
}