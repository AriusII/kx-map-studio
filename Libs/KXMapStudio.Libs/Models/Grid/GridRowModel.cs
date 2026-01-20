using System.Runtime.CompilerServices;

namespace KXMapStudio.Libs.Models.Grid;

/// <summary>
///     Represents a mutable grid row with property change notifications.
/// </summary>
public sealed class GridRowModel : INotifyPropertyChanged, IEquatable<GridRowModel>
{
	private int _id;
	private string _name = string.Empty;
	private double _x;
	private double _y;
	private double _z;

	/// <summary>
	///     Gets or sets the row identifier.
	/// </summary>
	public int Id
	{
		get => _id;
		set => SetField(ref _id, value);
	}

	/// <summary>
	///     Gets or sets the row display name.
	/// </summary>
	public string Name
	{
		get => _name;
		set => SetField(ref _name, value);
	}

	/// <summary>
	///     Gets or sets the X coordinate.
	/// </summary>
	public double X
	{
		get => _x;
		set => SetField(ref _x, value);
	}

	/// <summary>
	///     Gets or sets the Y coordinate.
	/// </summary>
	public double Y
	{
		get => _y;
		set => SetField(ref _y, value);
	}

	/// <summary>
	///     Gets or sets the Z coordinate.
	/// </summary>
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

	/// <summary>
	///     Occurs when a property value changes.
	/// </summary>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	///     Creates a <see cref="GridRowModel" /> instance from a DTO.
	/// </summary>
	/// <param name="dto">The source DTO.</param>
	/// <returns>A new <see cref="GridRowModel" /> instance.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="dto" /> is <see langword="null" />.</exception>
	public static GridRowModel FromDto(GridRowDto dto)
	{
		ArgumentNullException.ThrowIfNull(dto);

		return new GridRowModel
		{
			Id = dto.Id,
			Name = dto.Name,
			X = dto.X,
			Y = dto.Y,
			Z = dto.Z
		};
	}

	/// <summary>
	///     Converts this instance to a DTO.
	/// </summary>
	/// <returns>A DTO snapshot.</returns>
	public GridRowDto ToDto()
	{
		return new GridRowDto(Id, Name, X, Y, Z);
	}

	/// <summary>
	///     Creates a deep copy of this instance.
	/// </summary>
	/// <returns>A cloned <see cref="GridRowModel" />.</returns>
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

	private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
			return;

		field = value;
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}