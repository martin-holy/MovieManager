using MH.Utils.BaseClasses;
using System;
using System.Linq;

namespace MovieManager.Common.Features.Genre;

/// <summary>
/// DB fields: Id|Name
/// </summary>
public sealed class GenreR(CoreR coreR) : TableDataAdapter<GenreM>(coreR, "Genres", 2) {
  protected override GenreM _fromCsv(string[] csv) =>
    new(int.Parse(csv[0]), csv[1]);

  protected override string _toCsv(GenreM item) =>
    string.Join("|", item.GetHashCode().ToString(), item.Name);

  public GenreM? GetGenre(string name, bool create) =>
    string.IsNullOrEmpty(name)
      ? null
      : All.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? (create ? ItemCreate(new(GetNextId(), name)) : null);
}