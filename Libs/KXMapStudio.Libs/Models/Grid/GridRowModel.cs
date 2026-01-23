namespace KXMapStudio.Libs.Models.Grid;

/// <summary>
///     Represents a mutable grid row with property change notifications.
/// </summary>
/// <remarks>
///     Uses CommunityToolkit.Mvvm source generators for efficient property change notifications.
/// </remarks>
public sealed partial class GridRowModel : ObservableObject, IEquatable<GridRowModel>
{
	/// <summary>
	///     Gets or sets the row identifier.
	/// </summary>
	[ObservableProperty] private int _id;

	/// <summary>
	///     Gets or sets the row display name.
	/// </summary>
	[ObservableProperty] private string _name = string.Empty;

	/// <summary>
	///     Gets or sets the X coordinate.
	/// </summary>
	[ObservableProperty] private double _x;

	/// <summary>
	///     Gets or sets the Y coordinate.
	/// </summary>
	[ObservableProperty] private double _y;

	/// <summary>
	///     Gets or sets the Z coordinate.
	/// </summary>
	[ObservableProperty] private double _z;

	public bool Equals(GridRowModel? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return Id == other.Id && Name == other.Name && X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
	}

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
}